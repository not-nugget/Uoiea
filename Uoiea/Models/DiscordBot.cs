using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoiea.Models
{
    public class DiscordBot
    {
        private DiscordClient Client { get; init; }

        public DiscordBot(AppConfig config)
        {
            Client = new(new DiscordConfiguration()
            {
                Intents = DiscordIntents.AllUnprivileged,
                LoggerFactory = config.LoggerFactory,
                Token = config.Token,
            });
        }
    }
}
