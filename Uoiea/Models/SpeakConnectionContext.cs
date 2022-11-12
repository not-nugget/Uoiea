using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;

namespace Uoiea.Models
{
    /// <summary>
    /// Context which stores and tracks different voice connections, their current state, speak queue and other important
    /// metadata, as well as some actionable models alongside stored data
    /// </summary>
    internal sealed class SpeakConnectionContext
    {
        private ConcurrentDictionary<VoiceNextConnection, SpeakConnectionData> ConnectionData { get; } = new();

        /// <summary>
        /// Apply a connection to the context, creating an instance of <see cref="SpeakConnectionData"/> to represent the state of the connection.
        /// <paramref name="overwriteExisting"/> controls the behavior of the method if <paramref name="connection"/> is already accounted for
        /// </summary>
        /// <param name="connection"><see cref="VoiceNextConnection"/> to apply to the context</param>
        /// <param name="overwriteExisting">When true, the data represented by <paramref name="connection"/> will be overwritten if it exists, when false existing data will be left alone</param>
        /// <returns>true if <paramref name="connection"/>'s data was created or overwritten, false if no changes were made</returns>
        public bool ApplyConnection(VoiceNextConnection connection, bool overwriteExisting = true)
        {
            if(ConnectionData.ContainsKey(connection))
            {
                if(overwriteExisting) ConnectionData[connection] = new();
                return overwriteExisting;
            }

            return ConnectionData.TryAdd(connection, new());
        }

        /// <summary>
        /// Clear <paramref name="connection"/>'s data fron the context if it exists
        /// </summary>
        /// <param name="connection"><see cref="VoiceNextConnection"/>'s data to clear</param>
        /// <returns>true if the data existed and was cleared, false otherwise</returns>
        public bool ClearConnection(VoiceNextConnection connection)
        {
            return ConnectionData.TryRemove(connection, out _);
        }

        public SpeakConnectionData? GetConnectionData(VoiceNextConnection connection) => ConnectionData.ContainsKey(connection) ? ConnectionData[connection] : null;
    }
}
