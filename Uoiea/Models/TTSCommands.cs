using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static Uoiea.Models.DiscordBot;

namespace Uoiea.Models
{
    public class TTSCommands : BaseCommandModule
    {
        private StreamHandle TTSPipe { get; init; }

        public TTSCommands(StreamHandle ttsPipe) => TTSPipe = ttsPipe;

        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            VoiceNextExtension voice;
            VoiceNextConnection conn;
            VoiceTransmitSink sink;

            DiscordMember member = ctx.Member;

            if (member.VoiceState is null || member.VoiceState.Channel is null)
            {
                await ctx.RespondAsync("You must be in a voice channel to use this command");
                return;
            }
            
            voice = ctx.Client.GetVoiceNext();
            conn = await voice.ConnectAsync(member.VoiceState.Channel);
            sink = conn.GetTransmitSink();

            //string temp = Guid.NewGuid().ToString();

            //await TTSPipe.Writer.WriteAsync(temp);

            //string read = await TTSPipe.Reader.ReadLineAsync();

            //string output = $"generated guid: {temp} \nreceived guid: {read}";
            //await ctx.RespondAsync(output);

            await ctx.RespondAsync("done, retard");

            //await TTSPipe.Writer.WriteAsync("hello!");
            TTSPipe.Writer.Write("hello!");
            //TTSPipe.WaitForPipeDrain();
            
            await TTSPipe.Reader.CopyToAsync(sink);
            //TTSPipe.Reader.CopyTo(sink);
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
