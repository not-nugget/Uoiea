using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using OpusDotNet;
using System.IO;
using System;

namespace Uoiea.Models
{
    public class TTSCommands : BaseCommandModule
    {
        private NamedPipeServerStream TTSPipe { get; init; }

        public TTSCommands(NamedPipeServerStream ttsPipe) => TTSPipe = ttsPipe;

        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            VoiceNextExtension voice;
            VoiceNextConnection conn;
            VoiceTransmitSink sink;
            //StreamWriter writer;

            DiscordMember member = ctx.Member;

            if (member.VoiceState is null || member.VoiceState.Channel is null)
            {
                await ctx.RespondAsync("You must be in a voice channel to use this command");
                return;
            }

            voice = ctx.Client.GetVoiceNext();
            conn = await voice.ConnectAsync(member.VoiceState.Channel);
            sink = conn.GetTransmitSink();
            //writer = new(TTSPipe);

            string temp = Guid.NewGuid().ToString();
            await TTSPipe.WriteAsync(Encoding.UTF8.GetBytes(temp));
            await TTSPipe.FlushAsync();

            Memory<byte> buffer = new();
            await TTSPipe.ReadAsync(buffer);
            string read = Encoding.UTF8.GetString(buffer.ToArray());

            string output = $"generated guid: {temp} \nreceived guid: {read}";
            await ctx.RespondAsync(output);

            //await ctx.RespondAsync("done, retard");

            //byte[] buffer = Encoding.UTF8.GetBytes("hello");
            //byte[] opus = new byte[1024];
            //await TTSPipe.WriteAsync(buffer);
            //await TTSPipe.FlushAsync();

            //buffer = new byte[1024];
            //await TTSPipe.ReadAsync(buffer);

            //MemoryStream opusStream = new(opus);
            //await opusStream.CopyToAsync(opusStream);
            //await TTSPipe.CopyToAsync(sink);
        }

        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
            VoiceNextExtension voice;

            DiscordMember member = ctx.Member;
            if (member.VoiceState is null || member.VoiceState.Channel is null)
            {
                await ctx.RespondAsync("You must be in a voice channel to use this command");
                return;
            }

            voice = ctx.Client.GetVoiceNext();
            voice.GetConnection(ctx.Guild).Disconnect();
        }
        [Command("die")]
        public async Task Die(CommandContext ctx) => await Leave(ctx);

        [Command("say")]
        public async Task Tts(CommandContext ctx, [RemainingText] string tts)
        {

        }
    }
}
