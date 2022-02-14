using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Uoiea.Models
{
    public sealed class DiscordBot : IDisposable
    {
        public class StreamHandle //TODO own place to stay
        {
            public Stream Reader { get; init; }
            public Stream Writer { get; init; }

            readonly AnonymousPipeServerStream readStream;
            readonly AnonymousPipeServerStream writeStream;

            public StreamHandle(AnonymousPipeServerStream read, AnonymousPipeServerStream write)
            {
                readStream = read;
                Reader = readStream;
                writeStream = write;
                Writer = writeStream;
            }
        }

        public CancellationToken Cancellation { get; init; }
        private CancellationTokenSource CancellationSource { get; init; }

        private DiscordClient Client { get; init; }
        private CommandsNextExtension Commands { get; init; }

        public DiscordBot(AppConfig config)
        {
            IServiceCollection services = new ServiceCollection().AddSingleton(new StreamHandle(config.TTSRead, config.TTSWrite));

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
