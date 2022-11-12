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

                svs.AddSingleton<SpeakConnectionContext>();
                svs.AddSingleton<FonixTalkEngine>();
                svs.AddSingleton(loggerFactory);

                svs.AddHostedService<DiscordBot>();

#if DEBUG
                if(args.Any(x => x == "--debug"))
                {
                    svs.AddSingleton<TalkCommands>();
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
