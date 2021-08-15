using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast
{
    /// <summary>
    /// Manages source multicast membership for a given Socket.
    /// </summary>
    class SourceMulticastManager
    {
        private readonly Socket socket;
        // Holds the list of interfaceIndexes that this Socket should listen to sourcegroups on
        private ICollection<uint> interfaceIndexes = new HashSet<uint>();
        private IList<SourceAndGroup> sourceAndGroups = new List<SourceAndGroup>();
        private IList<SourceGroupInterface> sourceGroupInterfaces = new List<SourceGroupInterface>();

        public Socket Socket => socket;

        public SourceMulticastManager(Socket socket)
        {
            this.socket = socket;
        }

        #region Interface indexes to join new sourcegroups on
        /// <summary>
        /// Gets a collection of all interface indexes this class will join new sourcegroups on.
        /// </summary>
        /// <returns>A ReadOnlyCollection&lt;uint&gt; of the current list of interfaces this class will join new sourcegroups on.</returns>
        public IReadOnlyCollection<uint> GetInterfaceIndexes() => ImmutableHashSet<uint>.Empty.Union(interfaceIndexes);

        /// <summary>
        /// Add an interface index to the list that new sourcegroups will be joined on.
        /// </summary>
        /// <param name="arg"></param>
        public void AddInterfaceIndex(uint arg) => interfaceIndexes.Add(arg);

        /// <summary>
        /// Removes an interface index from the list that new sourcegroups will be added on.
        /// </summary>
        /// <param name="arg"></param>
        public void RemoveInterfaceIndex(uint arg) => interfaceIndexes.Remove(arg);
        #endregion

        //TODO: Methods to join and leave sourcegroups in SourceAndGroup format, and in Source,Group format.
        // Remember AddressGroup verification!
    }

    public class SourceAndGroup
    {
        public readonly IPAddress Source;
        public readonly IPAddress Group;
        public SourceAndGroup(IPAddress Source, IPAddress Group)
        {
            if (Source.AddressFamily != Group.AddressFamily)
                throw new ArgumentException($"Source.Addressfamily ({Source.AddressFamily}) not same as Group.Addressfamily ({Group.AddressFamily})");
            this.Source = Source;
            this.Group = Group;
        }

    }
    public class SourceGroupInterface
    {
        private readonly SourceAndGroup sgi;
        public readonly IList<uint> InterfaceIndexes = new List<uint>();

        internal SourceAndGroup Sgi => sgi;

        private SourceGroupInterface() { }

        public SourceGroupInterface(IPAddress Source, IPAddress Group)
        {
            sgi = new SourceAndGroup(Source, Group);
        }
    }
}
