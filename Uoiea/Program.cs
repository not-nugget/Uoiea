using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System.Diagnostics;
using System.Threading.Tasks;
using Uoiea.Models;

namespace Uoiea
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            Logger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            ILoggerFactory factory = new LoggerFactory().AddSerilog(logger);

            string pipeName = "ttsPipe";
            using AppConfig config = new(factory, pipeName);
            using DiscordBot bot = new(config);

            //start TTS speak process which connects to the pipe
            ProcessStartInfo startInfo = new (@"E:\GitHub\Uoiea\SpeakServer\bin\x86\Debug\SpeakServer.exe", pipeName)
            {
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false,
                CreateNoWindow = false,
            };
            using Process ttsProcess = Process.Start(startInfo);
            config.TTSPipe.WaitForConnection();

            await bot.StartAsync();
            await Task.Delay(-1);
        }
    }
}
