using SharpTalk;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace SpeakServer
{
    public static class Program
    {
        static void Main(string[] args) => Runner(args);

        static void Runner(string[] args)
        {
            Debug("starting speak process");
            Debug($"args[0]: {args[0]} args[1]: {args[1]}");
            if (args == null || args.Length == 0 || args.Length != 2) return;

            using (AnonymousPipeClientStream writePipe = new AnonymousPipeClientStream(args[0]))
            using (AnonymousPipeClientStream readPipe = new AnonymousPipeClientStream(args[1]))
            {
                Debug("pipes connected");
                try
                {
                    Debug("creating TTS engine");
                    using (FonixTalkEngine ttsEngine = new FonixTalkEngine())
                    using (StreamReader reader = new StreamReader(readPipe))
                    //using (StreamWriter writer = new StreamWriter(writeClient))
                    {
                        while (readPipe.IsConnected && writePipe.IsConnected)
                        {
                            Debug("waiting for data...");
                            string content = reader.ReadLine();
                            Debug($"data received... content:\n      {content}");
                            ttsEngine.SpeakToStream(writePipe, content);
                            writePipe.WaitForPipeDrain();
                            Debug("sent TTS response");
                        }
                    }
                }
                finally { Console.ReadLine(); }
            }
        }

        static void Debug(string message) => Console.WriteLine(message);
    }
}
