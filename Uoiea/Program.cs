using System;
using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Uoiea.Models;
using SharpTalk;
using System.Linq;
using Uoiea.Commands;

namespace Uoiea
{
    static class Program
    {
        //-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1
        //-i pipe:0 -isr 11025 -ich 1 -och 2 -osr 48000 pipe:1

        private static readonly string[] FfmpegArgs = new[] { "-i", "pipe:0", "-f", "s16le", "-isr", "11025", "-ich", "1", "-osr", "48000", "-och", "2", "pipe:1" };
        private static readonly Command Ffmpeg = Cli.Wrap("ffmpeg").WithArguments(FfmpegArgs).WithStandardErrorPipe(PipeTarget.ToDelegate(Console.Error.WriteLine));
        static async Task Main(string[] args)
        {
            Logger logger = new LoggerConfiguration().WriteTo.Console().Enrich.FromLogContext().CreateLogger();
            ILoggerFactory loggerFactory = new LoggerFactory().AddSerilog(logger);

            await
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.AddJsonFile("appsettings.json", false);
                cfg.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", true);
            })
            .ConfigureLogging((ctx, log) =>
            {
                log.ClearProviders();
                log.AddSerilog(logger, true);
            })
            .ConfigureServices((ctx, svs) =>
            {
                svs.Configure<DiscordConfig>(ctx.Configuration.GetSection(nameof(DiscordConfig)));

                svs.AddSingleton<FonixTalkEngine>();
                svs.AddSingleton(loggerFactory);
                svs.AddSingleton(Ffmpeg);

                svs.AddHostedService<DiscordBot>();

#if DEBUG
                if(args.Any(x => x == "--debug"))
                {
                    svs.AddSingleton<DECTalkCommands>();
                    svs.AddHostedService<DebugHS>();
                }
#endif
            })
            .UseConsoleLifetime()
            .Build()
            .RunAsync();
        }

    }
}
