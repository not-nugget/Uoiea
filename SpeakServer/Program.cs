using SharpTalk;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace SpeakServer
{
    public static class Program
    {
        static void Main(string[] args) => Runner(args).GetAwaiter().GetResult();

        static async Task Runner(string[] args)
        {
            Debug("starting speak process");
            Debug($"args[0]: {args[0]} args[1]: {args[1]}");
            if (args == null || args.Length == 0 || args.Length != 2) return;

            using (AnonymousPipeClientStream writePipe = new AnonymousPipeClientStream(PipeDirection.Out, args[0]))
            using (AnonymousPipeClientStream readPipe = new AnonymousPipeClientStream(PipeDirection.In, args[1]))
            {
                Debug("pipes connected");
                try
                {
                    Debug("creating TTS engine");
                    using (FonixTalkEngine ttsEngine = new FonixTalkEngine())
                    //using (StreamReader reader = new StreamReader(readPipe))
                    //using (StreamWriter writer = new StreamWriter(writeClient))
                    {
                        while (readPipe.IsConnected && writePipe.IsConnected)
                        {
                            Debug("waiting for data...");
                            byte[] buffer = new byte[512];
                            await readPipe.ReadAsync(buffer, 0, 512);
                            string text = Encoding.UTF8.GetString(buffer);
                            Debug($"data received... content:\n      {text}");
                            Debug($"resending incoming data");
                            MemoryStream resBuf = new MemoryStream(Encoding.UTF8.GetBytes(text));
                            await resBuf.CopyToAsync(writePipe);
                        }
                    }
                }
                finally { Console.ReadLine(); }
            }
        }

        static void Debug(string message) => Console.WriteLine(message);
    }
}
