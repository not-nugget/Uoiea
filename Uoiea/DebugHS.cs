using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Uoiea.Commands;

namespace Uoiea
{
#if DEBUG
    class DebugHS : IHostedService
    {
        public TalkCommands Ttsc { get; }
        public Command Ffmpeg { get; }

        public DebugHS(TalkCommands ttsc, Command ffmpeg)
        {
            Ttsc = ttsc;
            Ffmpeg = ffmpeg;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Ttsc.DebugTranscodeStringAsync("a");
            await Ttsc.DebugTranscodeStringAsync("b");
            await Ttsc.DebugTranscodeStringAsync("239048230482309482304");
            await Ttsc.DebugTranscodeStringAsync("asadkddfjhsdkjfhsdkf");
            await Ttsc.DebugTranscodeStringAsync("testing 124");

            _ = "";
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
#endif
}
