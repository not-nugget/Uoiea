using System;
using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace Uoiea
{
    static class Program
    {
        private const string FFMPEG = "ffmpeg";
        private const string FFMPEGPath = "ffmpegPath";
        private static readonly string[] FFMPEGArguments = new[] { "" };

        static async Task Main(string[] args)
        {
            Logger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            ILoggerFactory loggerFactory = new LoggerFactory().AddSerilog(logger);

            await
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {

            })
            .ConfigureLogging((ctx, log) =>
            {
                log.ClearProviders();
                log.AddSerilog(logger, true);
            })
            .ConfigureServices((ctx, svs) =>
            {
                svs.AddSingleton(loggerFactory);
                svs.AddTransient(FfmpegCommandFactory);
            })
            .UseConsoleLifetime()
            .Build()
            .RunAsync();
        }

        static Command FfmpegCommandFactory(IServiceProvider _) => Cli
            .Wrap(FFMPEG)
            .WithArguments(FFMPEGArguments)
            .WithWorkingDirectory(FFMPEGPath);

        //using AppConfig config = new(factory);
        //using DiscordBot bot = new(config);

        //string handleA = config.TTSRead.GetClientHandleAsString(), handleB = config.TTSWrite.GetClientHandleAsString();

        ////start TTS speak process which connects to the pipe
        //ProcessStartInfo startInfo = new (@"E:\GitHub\Uoiea\SpeakServer\bin\Debug\SpeakServer.exe", string.Join(' ', handleA, handleB))
        //{
        //    WindowStyle = ProcessWindowStyle.Normal,
        //    UseShellExecute = false,
        //    CreateNoWindow = false,
        //};

        //using Process ttsProcess = Process.Start(startInfo);

        //config.TTSRead.DisposeLocalCopyOfClientHandle();
        //config.TTSWrite.DisposeLocalCopyOfClientHandle();

        //await bot.StartAsync();

        //int input = 250;
        //byte[] read = new byte[input];
        //await config.TTSWrite.WriteAsync(Encoding.UTF8.GetBytes("test pipe write string"));
        //await config.TTSRead.ReadAsync(read.AsMemory(0, input));
        //Console.Write(Encoding.UTF8.GetString(read).ToCharArray(), 0, read.Length);

        //await Task.Delay(-1);
    }
}
