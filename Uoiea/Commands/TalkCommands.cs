using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using NAudio.Wave;
using SharpTalk;
using Uoiea.Models;

namespace Uoiea.Commands
{
    internal sealed partial class TalkCommands : UoieaCommands
    {
        private const string SpeakProcessSuccess = "Your phrase has been generated and enqueued! Enjoy!";
        private const string NoConnectionDataError = "Data does not exist for the provided connection";
        private static readonly WaveFormat FonixFormat = new(11025, 16, 1);
        private static readonly WaveFormat ResampleFormat = new(48000, 16, 2);

        public FonixTalkEngine TTSEngine { get; init; }

        public TalkCommands(FonixTalkEngine ttsEngine, SpeakConnectionContext context) : base(context)
        {
            TTSEngine = ttsEngine;
        }

        public partial async Task SayComand(InteractionContext ctx, string speak)
        {
            await ctx.DeferAsync();

            if(!InCallingMemberVC(ctx, out VoiceNextConnection conn, out _))
            {
                if(InVC(ctx, out _))
                {
                    await EditResponseAsync(ctx, SpeakTitle, SpeakJoinError, false);
                    return;
                }

                conn = await JoinChannel(ctx);
            }

            if(!GetConnectionData(conn, out SpeakConnectionData data, out string error))
            {
                await EditResponseAsync(ctx, SpeakTitle, error, false);
                return;
            }

            MemoryStream resampledSpeech = new();
            byte[] speechBytes = TTSEngine.SpeakToMemory(speak);
            ResampleTTSBytes(resampledSpeech, speechBytes);

            await EditResponseAsync(ctx, SpeakTitle, SpeakProcessSuccess);



            await conn.GetTransmitSink().WriteAsync(resampledSpeech.ToArray());
        }

        private bool GetConnectionData(VoiceNextConnection connection, out SpeakConnectionData data, out string error)
        {
            data = default;
            error = string.Empty;
            var nData = Context.GetConnectionData(connection);
            if(!nData.HasValue)
            {
                error = NoConnectionDataError;
                return false;
            }
            data = nData.Value;
            return true;
        }

        private static void ResampleTTSBytes(Stream destinationStream, byte[] ttsBytes) => WaveFileWriter.WriteWavFileToStream(destinationStream, new WaveFormatConversionStream(ResampleFormat, WaveStreamFromBytes(ttsBytes)));
        private static RawSourceWaveStream WaveStreamFromBytes(byte[] ttsBytes) => new(ttsBytes, 0, ttsBytes.Length, FonixFormat);
    }

    //Docs and Attributes
    [SlashCommandGroup(Constants.Uoiea, Constants.UoieaDescription), SlashCommandPermissions(Permissions.SendMessages)]
    internal sealed partial class TalkCommands
    {
        [SlashCommand("speak", "Instruct the bot to process and say the provided string")]
        public partial Task SayComand(InteractionContext ctx, [Option("speak", "Phrase to process and speak. Stay classy, but have fun!")] string speak);
    }
}
