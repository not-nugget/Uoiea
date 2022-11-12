using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using NAudio.Wave;
using SharpTalk;
using Uoiea.Models;

namespace Uoiea.Commands
{
    [SlashCommandGroup("uoiea", "John Madden!"), SlashModuleLifespan(SlashModuleLifespan.Singleton), SlashCommandPermissions(Permissions.SendMessages)]
    internal sealed class DECTalkCommands : ApplicationCommandModule
    {
        private const string JoinTitle = "Join Command";
        private const string LeaveTitle = "Leave Command";
        private const string SpeakTitle = "Speak Command";
        private const string ConnectSuccess = "Joined! Use \"/tts speak <phrase>\" to get me to speak!";
        private const string DisconnectSuccess = "Successfully disconnected";
        private const string VerifyJoinError = "There was an error when verifying or creating a voice connection to your voice channel. I might not have the proper permissions to join, or I am already in another voice channel";
        private const string BotNotInChannelError = "The bot is not currently in a voice channel";
        private const string MemberNotInChannelError = "You must be in a voice channel to use this command";
        private const string MemberNotInBotChannelError = "You must be in the same channel as the bot to use this command";

        public FonixTalkEngine TTSEngine { get; init; }

        public DECTalkCommands(FonixTalkEngine ttsEngine)
        {
            TTSEngine = ttsEngine;
        }

        //TODO move the join/leave commands into a seperate lifecycle command class to prevent excess allocation via the command object
        [SlashCommand("join", "Instruct the bot to join your voice channel. Not requred, but recommended")]
        public async Task JoinChannelCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await JoinChannel(ctx);
        }

        [SlashCommand("leave", "Instruct the bot to leave your voice channel")]
        public async Task LeaveChannelCommand(InteractionContext ctx/*, [Option("reason", "Optional reason for disconnecting the bot")] string reason = null*/)
        {
            await ctx.DeferAsync();

            if(!BotIsInMemberVoiceChannel(ctx, out VoiceNextConnection conn, out string error))
            {
                await EditResponseAsync(ctx, LeaveTitle, error, false);
                return;
            }

            conn.Disconnect();
            await EditResponseAsync(ctx, LeaveTitle, DisconnectSuccess);
        }

        [SlashCommand("say", "Instruct the bot to process and say the provided string")]
        public async Task SayStringComand(InteractionContext ctx, [Option("speak", "String to speak. Be creative here!")] string speak)
        {
            await ctx.DeferAsync();

            if(!BotIsInMemberVoiceChannel(ctx, out VoiceNextConnection conn, out string error))
            {
                conn = await JoinChannel(ctx);
            }

            var tts = TTSEngine.SpeakToMemory(speak);

            await EditResponseAsync(ctx, SpeakTitle, "Text synthesized and encoded, now speaking...");

            //jeebus mc fleebus this shit worked!
            MemoryStream tts2 = new();
            var wcs = new WaveFormatConversionStream(new WaveFormat(48000, 16, 2), new RawSourceWaveStream(tts, 0, tts.Length, new(11025, 16, 1)));
            WaveFileWriter.WriteWavFileToStream(tts2, wcs);

            await conn.GetTransmitSink().WriteAsync(tts2.ToArray());
        }

        //TODO any other commands that alter behavior such as global voice, volume, etc should be in a config commands class to mitigate allocations

#if DEBUG
        internal async Task<ReadOnlyMemory<byte>> DebugTranscodeStringAsync(string debugSpeak)
        {
            MemoryStream speakStream = new();
            TTSEngine.SpeakToStream(speakStream, debugSpeak);
            TTSEngine.Sync();

            ReadOnlyMemory<byte> opusSpeak = await TranscodeSpeakPCMAsync(speakStream);
            return opusSpeak;
        }
#endif
        private static async Task EditResponseAsync(InteractionContext ctx, string title, string content, bool success = true)
        {
            DiscordEmbedBuilder embed = new();
            embed.WithTitle(title);
            embed.WithDescription(content);
            embed.WithTimestamp(DateTime.Now);
            embed.WithAuthor(ctx);
            embed.WithColor(success ? DiscordColor.Green : DiscordColor.Red);
            embed.WithFooter(success ? "Operation Successful" : "An error occurred");

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
        private static async Task<VoiceNextConnection> EstablishOrVerifyVoiceNextConnectionAsync(InteractionContext ctx, DiscordChannel vc)
        {
            //TODO introduce logic to check that the voice channel is the same as the one provided,
            //or if the bot is already in a different voice channel
            //there will be some more congfiguration that will be required 
            return ctx.Client.GetVoiceNext().GetConnection(ctx.Guild) ?? await vc.ConnectAsync();
        }
        private static async Task<VoiceNextConnection> JoinChannel(InteractionContext ctx)
        {
            if(!MemberIsInVoiceChannel(ctx.Member, out DiscordChannel channel))
            {
                await EditResponseAsync(ctx, JoinTitle, MemberNotInChannelError, false);
                throw new InvalidOperationException(MemberNotInChannelError);
            }

            var conn = await EstablishOrVerifyVoiceNextConnectionAsync(ctx, channel);
            if(conn is not null)
            {
                await EditResponseAsync(ctx, JoinTitle, ConnectSuccess);
                return conn;
            }

            await EditResponseAsync(ctx, JoinTitle, VerifyJoinError, false);
            throw new InvalidOperationException(VerifyJoinError);
        }
        private static bool MemberIsInVoiceChannel(DiscordMember member, out DiscordChannel vc)
        {
            vc = null;
            return member.VoiceState is not null && (vc = member.VoiceState.Channel) is not null;
        }
        private static bool BotIsInMemberVoiceChannel(InteractionContext ctx, out VoiceNextConnection conn, out string specificError)
        {
            var memberVoice = ctx.Member.VoiceState?.Channel;
            conn = ctx.Client.GetVoiceNext().GetConnection(ctx.Guild);
            var sameChannel = memberVoice is not null && conn is not null && memberVoice == conn.TargetChannel;

            specificError = memberVoice is null ? MemberNotInChannelError : conn is null ? BotNotInChannelError : !sameChannel ? MemberNotInBotChannelError : string.Empty;

            return sameChannel;
        }
    }
}
