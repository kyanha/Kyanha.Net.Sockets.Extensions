using System.Net;

namespace Kyanha.Net.Sockets.SourceMulticast
{
    /// <summary>
    /// Interface structure corresponding to GROUP_SOURCE_REQ for setsockopt.
    /// </summary>
    public class MulticastGroupSourceOption
    {
        private uint interfaceIndex = 0;
        private IPAddress group = IPAddress.Parse("FF3E::0000:0001");
        private IPAddress source = IPAddress.Parse("::1");

        public uint InterfaceIndex { get => interfaceIndex; set => interfaceIndex = value; }
        public IPAddress Group { get => group; set => group = value; }
        public IPAddress Source { get => source; set => source = value; }
    }
}
