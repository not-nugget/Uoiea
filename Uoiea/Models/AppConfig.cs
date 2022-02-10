using Microsoft.Extensions.Logging;
using System.Configuration;

namespace Uoiea.Models
{
    public class AppConfig
    {
        public ILoggerFactory LoggerFactory { get; init; }
        public string Token { get; init; }

        internal AppConfig(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Token = ConfigurationManager.AppSettings.Get("token");
        }
    }
}
