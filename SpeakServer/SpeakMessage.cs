using System;

namespace SpeakServer
{
    //IMPL TODO this will be a more advanced messaging system so the bot can do more things like change the voice, pause, or completely cancel everything
    [Serializable]
    public class SpeakMessage
    {
        public int id;
        public string message;
        public byte[] tts;
    }
}
