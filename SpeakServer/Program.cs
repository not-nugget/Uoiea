using SharpTalk;
using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using OpusDotNet;
using System.IO;
using System.Collections.Generic;

namespace SpeakServer
{
    public static class Program
    {
        static void Main(string[] args) => Runner(args).GetAwaiter().GetResult();

        static async Task Runner(string[] args)
        {
            Debug("starting speak process");
            if (args == null || args.Length == 0) return;

            OpusEncoder encoder = new OpusEncoder(Application.VoIP, 12000, 1);

            //using (AnonymousPipeClientStream writePipe = new AnonymousPipeClientStream(args[0]))
            //using (AnonymousPipeClientStream readPipe = new AnonymousPipeClientStream(args[1]))
            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", args[0], PipeDirection.InOut, PipeOptions.Asynchronous))
            {
                Debug("connecting to pipe server");
                await pipeStream.ConnectAsync();
                Debug("pipes connected");
                try
                {
                    Debug("creating TTS engine");
                    using (FonixTalkEngine ttsEngine = new FonixTalkEngine())
                    //using (StreamReader reader = new StreamReader(pipeStream))
                    //using (StreamWriter writer = new StreamWriter(writeClient))
                    {
                        while (pipeStream.IsConnected)
                        {
                            await pipeStream.FlushAsync();

                            try
                            {
                                Debug("waiting for data...");
                                byte[] buffer = new byte[512];
                                int read = await pipeStream.ReadAsync(buffer, 0, 512);
                                string text = Encoding.UTF8.GetString(buffer);
                                Debug($"data received... content:\n      {text}");
                                Debug($"resending incoming data");
                                MemoryStream resBuf = new MemoryStream(Encoding.UTF8.GetBytes(text));
                                await resBuf.CopyToAsync(pipeStream);

                                //byte[] buffer = new byte[1024];
                                //int read = await pipeStream.ReadAsync(buffer, 0, 1024);
                                //string text = Encoding.UTF8.GetString(buffer);
                                //Debug($"data received... content:\n      {text}");

                                //byte[] opusBlocks = new byte[0];
                                //MemoryStream opusStream;
                                //int index = 0, stride = 20;
                                //byte[] pcm = ttsEngine.SpeakToMemory(text);
                                //while (true)
                                //{
                                //    if (index > (pcm.Length + stride)) break;

                                //    byte[] pcmBlock = new byte[stride];
                                //    byte[] opusBlock = new byte[stride];
                                //    int write = encoder.Encode(pcmBlock, stride, opusBlock, stride);

                                //    Array.Copy(opusBlock, 0, opusBlocks, opusBlocks.Length - 1, write);

                                //    index += stride;
                                //}

                                //opusStream = new MemoryStream(opusBlocks);
                                //await opusStream.CopyToAsync(pipeStream);

                                //Debug("sent TTS response");
                            }
                            catch (Exception e)
                            {
                                Debug("reading/writing exception");
                                Debug(e.Message);
                            }
                        }
                    }
                }
                finally { }
            }
        }

        static void Debug(string message) => Console.WriteLine(message);
    }
}
