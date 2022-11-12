namespace Uoiea.Models
{
    /// <summary>
    /// Current speaking state as represetned by a <see cref="byte"/>
    /// </summary>
    internal enum SpeakState : byte
    {
        /// <summary>
        /// An error has occurred and thus has caused speaking to stop
        /// </summary>
        Error = 0,
        /// <summary>
        /// The speak queue is empty and the connection is idle
        /// </summary>
        Idle = 1,
        /// <summary>
        /// The speak queue is empty and the connection is idle while an input is being processed
        /// </summary>
        Processing = 2,
        /// <summary>
        /// The speak queue may be empty and the connection is transmitting a speak stream
        /// </summary>
        Speaking = 3,
        /// <summary>
        /// The speak queue may be empty and the connection is paused while transmitting a speak stream
        /// </summary>
        Paused = 4,
        /// <summary>
        /// The state of the connection cannot currently be resolved and should likely be handled as an error
        /// </summary>
        Unknown = byte.MaxValue,
    }
}
