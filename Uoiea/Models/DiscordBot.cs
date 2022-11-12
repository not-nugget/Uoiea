using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Uoiea.Commands;

namespace Uoiea.Models
{
    internal sealed class DiscordBot : IHostedService, IDisposable
    {
        private DiscordClient Client { get; init; }
        private SlashCommandsExtension Commands { get; init; }

        public DiscordBot(IServiceProvider services, IOptions<DiscordConfig> config)
        {
            Client = new(new DiscordConfiguration()
            {
                Intents = DiscordIntents.AllUnprivileged,
                LoggerFactory = services.GetService<ILoggerFactory>(),
                Token = config.Value.Token,
            });

            Client.UseVoiceNext();

            Commands = Client.UseSlashCommands(new()
            {
                Services = services,
            });

            Commands.RegisterCommands<DECTalkCommands>();
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
