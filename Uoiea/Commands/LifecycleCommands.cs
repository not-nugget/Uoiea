using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Uoiea.Models;

namespace Uoiea.Commands
{
    internal sealed partial class LifecycleCommands : UoieaCommands
    {
        private const string MoveTitle = "Move command";
        private const string MoveSuccess = "Successfully moved to the target destination";
        private const string NoChannelMoveTargetError = "Please specify a channel I should move to when you are not in a channel yourself";

        public LifecycleCommands(SpeakConnectionContext context) : base(context) { }

        public partial async Task JoinChannelCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await JoinChannel(ctx);
        }

        public partial async Task LeaveChannelCommand(InteractionContext ctx, string reason)
        {
            await ctx.DeferAsync();

            if(!InCallingMemberVC(ctx, out VoiceNextConnection conn, out string error))
            {
                await EditResponseAsync(ctx, LeaveTitle, error, false);
                return;
            }

            conn.Disconnect();
            await EditResponseAsync(ctx, LeaveTitle, DisconnectSuccess);
        }

        public partial async Task ForceJoinChannelCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await JoinChannel(ctx, true);
        }

        public partial async Task ForceLeaveChannelCommand(InteractionContext ctx, string reason)
        {
            await ctx.DeferAsync();
            ctx.GetVoiceNextConnection().Disconnect();
            await EditResponseAsync(ctx, LeaveTitle, DisconnectSuccess);
        }

        public partial async Task ForceMoveChannelCommand(InteractionContext ctx, DiscordChannel targetChannel, string reason)
        {
            await ctx.DeferAsync();

            if(!CallingMemberInVC(ctx, out DiscordChannel conn) && targetChannel is null)
            {
                await EditResponseAsync(ctx, LeaveTitle, NoChannelMoveTargetError, false);
                return;
            }

            ctx.GetVoiceNextConnection()?.Disconnect();
            await (conn ?? targetChannel).ConnectAsync();
            await EditResponseAsync(ctx, MoveTitle, MoveSuccess);
        }
    }

    // Docs and Attributes
    [SlashCommandGroup(Constants.Uoiea, Constants.UoieaDescription), SlashCommandPermissions(Permissions.SendMessages)]
    internal sealed partial class LifecycleCommands
    {
        [SlashCommand("join", "Instruct the bot to join your voice channel. Not requred, but recommended")]
        public partial Task JoinChannelCommand(InteractionContext ctx);

        [SlashCommand("leave", "Instruct the bot to leave your voice channel")]
        public partial Task LeaveChannelCommand(InteractionContext ctx, [Option("reason", "Optional reason for disconnecting the bot")] string reason = null);

        [SlashCommand("fjoin", "Force the bot to join your voice channel. Caller must have move members permission"), SlashCommandPermissions(Permissions.MoveMembers)]
        public partial Task ForceJoinChannelCommand(InteractionContext ctx);

        [SlashCommand("fleave", "Force the bot to leave your voice channel. Caller must have move members permission"), SlashCommandPermissions(Permissions.MoveMembers)]
        public partial Task ForceLeaveChannelCommand(InteractionContext ctx, [Option("reason", "Optional reason for disconnecting the bot")] string reason = null);

        [SlashCommand("fmove", "Force the bot to leave your voice channel. Caller must have move members permission"), SlashCommandPermissions(Permissions.MoveMembers)]
        public partial Task ForceMoveChannelCommand(InteractionContext ctx, [Option("target channel", "Optional specific channel to move the bot to")] DiscordChannel targetChannel = null, [Option("reason", "Optional reason for disconnecting the bot")] string reason = null);
    }
}
