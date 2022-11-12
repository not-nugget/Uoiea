using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;

namespace Uoiea.Models
{
    internal static class Extensions
    {
        public static VoiceNextExtension GetVoiceNext(this InteractionContext ctx) => ctx.Client.GetVoiceNext();
        public static VoiceNextConnection GetVoiceNextConnection(this InteractionContext ctx) => ctx.GetVoiceNext().GetConnection(ctx.Guild);

        public static DiscordEmbedBuilder WithAuthor(this DiscordEmbedBuilder builder, BaseContext ctx) => builder.WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl);

        public static class Enumerable
        {
            public static IEnumerable<T> Collect<T>(T[][] collections)
            {
                List<T> collection = new();
                foreach(var enumerable in collections)
                    collection.AddRange(enumerable);
                return collection.AsEnumerable();
            }
        }
    }
}
