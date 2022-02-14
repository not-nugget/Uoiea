using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uoiea.Models
{
    public sealed class DiscordBot : IDisposable
    {
        public CancellationToken Cancellation { get; init; }
        private CancellationTokenSource CancellationSource { get; init; }

        private DiscordClient Client { get; init; }
        private CommandsNextExtension Commands { get; init; }

        public DiscordBot(AppConfig config)
        {
            IServiceCollection services = new ServiceCollection().AddSingleton(config.TTSPipe);

            CancellationSource = new CancellationTokenSource();
            Cancellation = CancellationSource.Token;

            Client = new(new DiscordConfiguration()
            {
                Intents = DiscordIntents.AllUnprivileged,
                LoggerFactory = config.LoggerFactory,
                Token = config.Token,
            });

            Commands = Client.UseCommandsNext(new()
            {
                EnableDms = false,
                EnableDefaultHelp = false,
                EnableMentionPrefix = false,
                StringPrefixes = new[] { "!" },
                Services = services.BuildServiceProvider(),
            });
            Commands.RegisterCommands<TTSCommands>();

            Client.UseVoiceNext();
        }

        public async Task StartAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            await Client.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            await Client.DisconnectAsync();
        }

        public void Dispose() => Client.Dispose();
    }
}
