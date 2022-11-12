using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Uoiea.Models;

namespace Uoiea.Commands
{
    /// <summary>
    /// Base commands class that contains a plethora of helper methods, constants, and other doodads that a command module might benefit from
    /// </summary>
    internal abstract class UoieaCommands : ApplicationCommandModule
    {
        #region Constants
        protected const string JoinTitle = "Join Command";
        protected const string LeaveTitle = "Leave Command";
        protected const string SpeakTitle = "Speak Command";
        protected const string ConnectSuccess = "Joined! Use \"/tts speak <phrase>\" to get me to speak!";
        protected const string DisconnectSuccess = "Successfully disconnected";
        protected const string VerifyJoinError = "There was an error when verifying or creating a voice connection to your voice channel. I might not have the proper permissions to join, or I am already in another voice channel";
        protected const string SpeakJoinError = "An error occurred when trying to speak. I may be in a different voice channel";
        protected const string BotNotInChannelError = "The bot is not currently in a voice channel";
        protected const string MemberNotInChannelError = "You must be in a voice channel to use this command";
        protected const string MemberNotInBotChannelError = "You must be in the same channel as the bot to use this command";
        #endregion

        public SpeakConnectionContext Context { get; }

        public UoieaCommands(SpeakConnectionContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Joins the voice channel of the calling member if specific checks pass. Will throw if they fail
        /// </summary>
        protected async Task<VoiceNextConnection> JoinChannel(InteractionContext ctx, bool force = false)
        {
            if(!CallingMemberInVC(ctx, out DiscordChannel channel))
            {
                await EditResponseAsync(ctx, JoinTitle, MemberNotInChannelError, false);
                throw new InvalidOperationException(MemberNotInChannelError);
            }

            var conn = await GetOrConnectToVoiceChannelAsync(ctx, channel, force);
            if(conn is not null)
            {
                await EditResponseAsync(ctx, JoinTitle, ConnectSuccess);
                Context.ApplyConnection(conn, true);
                return conn;
            }

            await EditResponseAsync(ctx, JoinTitle, VerifyJoinError, false);
            throw new InvalidOperationException(VerifyJoinError);
        }

        /// <summary>
        /// Edit's the context's response with the provided information
        /// </summary>
        protected static async Task EditResponseAsync(InteractionContext ctx, string title, string content, bool success = true)
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

        /// <summary>
        /// Gets an existing connection to a voice channel or connects to the provided channel when not currently connected
        /// </summary>
        protected static async Task<VoiceNextConnection> GetOrConnectToVoiceChannelAsync(InteractionContext ctx, DiscordChannel connectIfDisconnected, bool force = false)
        {
            var conn = ctx.Client.GetVoiceNext().GetConnection(ctx.Guild);
            if(force || conn is not null) conn = await connectIfDisconnected.ConnectAsync();
            return conn;

        }
        
        /// <summary>
        /// Checks that the bot itself has a connection to a VC in the guild specified by <paramref name="ctx"/>
        /// </summary>
        protected static bool InVC(InteractionContext ctx, out string error)
        {
            bool connected = ctx.GetVoiceNextConnection() is not null;
            error = connected ? string.Empty : BotNotInChannelError;
            return connected;
        }
        /// <summary>
        /// Checks that the bot is within the member who executed the command's channel
        /// </summary>
        protected static bool InCallingMemberVC(InteractionContext ctx, out VoiceNextConnection conn, out string error)
        {
            var vc = ctx.Member.VoiceState?.Channel;
            conn = ctx.Client.GetVoiceNext().GetConnection(ctx.Guild);
            var sameChannel = vc is not null && conn is not null && vc == conn.TargetChannel;

            error = vc is null ? MemberNotInChannelError : conn is null ? BotNotInChannelError : !sameChannel ? MemberNotInBotChannelError : string.Empty;

            return sameChannel;
        }
        /// <summary>
        /// Checks that the calling member is within a voice channel
        /// </summary>
        protected static bool CallingMemberInVC(InteractionContext ctx, out DiscordChannel vc)
        {
            vc = null;
            return ctx.Member is not null && ctx.Member.VoiceState is not null && (vc = ctx.Member.VoiceState.Channel) is not null;
        }
    }
}
