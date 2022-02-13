using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;
using Uoiea.Models;

namespace Uoiea
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            ILoggerFactory factory = new LoggerFactory().AddSerilog();

            DiscordBot bot = new(new(factory));

            await bot.StartAsync();

            await Task.Delay(-1);
        }
    }
}
