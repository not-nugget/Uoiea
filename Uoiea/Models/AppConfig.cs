using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.IO;
using System.IO.Pipes;

namespace Uoiea.Models
{
    public sealed class AppConfig : IDisposable
    {
        public ILoggerFactory LoggerFactory { get; init; }
        public string Token { get; init; }

        public AnonymousPipeServerStream TTSRead { get; init; }
        public AnonymousPipeServerStream TTSWrite { get; init; }

        internal AppConfig(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Token = ConfigurationManager.AppSettings.Get("token");
            TTSRead = new(PipeDirection.In, HandleInheritability.Inheritable);
            TTSWrite = new(PipeDirection.Out, HandleInheritability.Inheritable);
        }

        public void Dispose()
        {
            TTSRead.Dispose();
            TTSWrite.Dispose();
        }
    }
}
