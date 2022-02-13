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

            using AppConfig config = new(factory);
            using DiscordBot bot = new(config);

            string handleA = config.TTSRead.GetClientHandleAsString(), handleB = config.TTSWrite.GetClientHandleAsString();

            //start TTS speak process which connects to the pipe
            ProcessStartInfo startInfo = new (@"E:\GitHub\Uoiea\SpeakServer\bin\Debug\SpeakServer.exe", string.Join(' ', handleA, handleB))
            {
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false,
                CreateNoWindow = false,
            };
            using Process ttsProcess = Process.Start(startInfo);

            config.TTSRead.DisposeLocalCopyOfClientHandle();
            config.TTSWrite.DisposeLocalCopyOfClientHandle();

            await bot.StartAsync();
            await Task.Delay(-1);
        }
    }
}
