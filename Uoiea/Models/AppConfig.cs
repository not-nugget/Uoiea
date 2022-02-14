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

        public NamedPipeServerStream TTSPipe { get; init; }

        internal AppConfig(ILoggerFactory loggerFactory, string pipeName = "ttsPipe")
        {
            LoggerFactory = loggerFactory;
            Token = ConfigurationManager.AppSettings.Get("token");
            TTSPipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 2, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }

        public void Dispose()
        {
            TTSPipe.Dispose();
        }
    }
}
