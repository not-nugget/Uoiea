using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using SharpTalk;
using System.Threading.Tasks;

namespace Uoiea.Models
{
    public class TTSCommands : BaseCommandModule
    {
        private FonixTalkEngine TTSEngine { get; init; }

        public TTSCommands(FonixTalkEngine ttsEngine) => TTSEngine = ttsEngine;

        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            VoiceNextExtension voice;
            VoiceNextConnection conn;

            DiscordMember member = ctx.Member;

            if (member.VoiceState is null || member.VoiceState.Channel is null)
            {
                await ctx.RespondAsync("You must be in a voice channel to use this command");
                return;
            }
            
            voice = ctx.Client.GetVoiceNext();
            conn = await voice.ConnectAsync(member.VoiceState.Channel);
            await ctx.RespondAsync("done, retard");
        }
    }
}
