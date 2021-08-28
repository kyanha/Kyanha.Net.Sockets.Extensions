using System.Net;

namespace Kyanha.Net.Sockets.SourceMulticast
{
    /// <summary>
    /// Interface structure corresponding to GROUP_SOURCE_REQ for setsockopt.
    /// </summary>
    public class MulticastGroupSourceOption
    {
        private uint interfaceIndex = 0;
        // FF3E::4000:0000 is the invalid group.
        private IPAddress group = IPAddress.Parse("FF3E::4000:0000");

        // ::0 is the invalid address.
        private IPAddress source = IPAddress.Parse("::0");

        public uint InterfaceIndex
        {
            get => interfaceIndex;
            set => interfaceIndex = value;
        }
        public IPAddress Group { get => group; set => group = value; }
        public IPAddress Source { get => source; set => source = value; }

        private MulticastGroupSourceOption() { }

        public MulticastGroupSourceOption(IPAddress Source, IPAddress Group)
        {
            this.Source = Source; this.Group = Group;
        }

        public MulticastGroupSourceOption(IPAddress Source, IPAddress Group, uint InterfaceIndex)
        {
            this.Source = Source;
            this.Group = Group;
            this.InterfaceIndex = InterfaceIndex;
        }

        /// <summary>
        /// The copy constructor, primarily for internal use (but public because you might find a use for it).
        /// </summary>
        /// <param name="mgso"></param>
        public MulticastGroupSourceOption(MulticastGroupSourceOption mgso)
        {
            Source = mgso.Source;
            group = mgso.Group;
            InterfaceIndex = mgso.InterfaceIndex;
        }
    }
}
