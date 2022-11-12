using System.Collections.Generic;
using DSharpPlus.VoiceNext;

namespace Uoiea.Models
{
    /// <summary>
    /// Data that is representative of the current state of an arbitrary <see cref="VoiceNextConnection"/>
    /// </summary>
    internal struct SpeakConnectionData
    {
        /// <summary>
        /// Current state of the speak connection
        /// </summary>
        public SpeakState State { get; private set; } = SpeakState.Idle;
        /// <summary>
        /// Queue of speak strings which will be processed, resampled, and transmitted
        /// </summary>
        public Queue<string> SpeakQueue { get; } = new();
        /// <summary>
        /// Connection the contained data represents
        /// </summary>
        public VoiceNextConnection Connection { get; init; }

        public SpeakConnectionData(VoiceNextConnection connection) => Connection = connection;
    }
}
