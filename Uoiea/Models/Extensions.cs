using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Uoiea.Models
{
    internal static class Extensions
    {
        public static DiscordEmbedBuilder WithAuthor(this DiscordEmbedBuilder builder, BaseContext ctx)
        {
            return builder.WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl);
        }

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
