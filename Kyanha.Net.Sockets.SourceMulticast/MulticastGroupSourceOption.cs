using System.Net;

namespace Kyanha.Net.Sockets.SourceMulticast
{
    /// <summary>
    /// Interface structure corresponding to GROUP_SOURCE_REQ for setsockopt.
    /// </summary>
    public class MulticastGroupSourceOption
    {
        private ulong interfaceIndex;
        private SocketAddress group;
        private SocketAddress source;

        public ulong InterfaceIndex { get => interfaceIndex; set => interfaceIndex = value; }
        public SocketAddress Group { get => group; set => group = value; }
        public SocketAddress Source { get => source; set => source = value; }
    }
}
