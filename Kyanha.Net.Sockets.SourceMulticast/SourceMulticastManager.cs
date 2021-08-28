using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Immutable;
using static Kyanha.Net.Sockets.SourceMulticast.SocketExtensions;

namespace Kyanha.Net.Sockets.SourceMulticast
{
    /// <summary>
    /// Manages source multicast memberships for a given Socket.
    /// </summary>
    /// <remarks>This class tracks multicast group memberships that are made through this class.
    /// It does not examine the legitimate InterfaceIndexes of the system, it leaves that
    /// to the client.
    /// 
    /// <see cref="AddInterfaceIndex(uint)"/>, <see cref="RemoveInterfaceIndex(uint)"/> to set up
    /// the list of interfaces the next group join or leave operation will operate on.
    /// 
    /// <see cref="GetInterfaceIndexes"/> gets the current list of interfaces the next group join
    /// or leave will operate on.
    /// 
    /// Each successful individual {Source,Group} join on the associated Socket will be tracked in a
    /// <see cref="MulticastGroupSourceOption"/> object. The collection of these can be accessed via
    /// <see cref="GetSourceGroupMemberships"/>.
    /// 
    /// A source group may be joined by providing a <see cref="MulticastGroupSourceOption"/> to
    /// JoinSourceGroup or LeaveSourceGroup. If its <see cref="MulticastGroupSourceOption.InterfaceIndex"/>
    /// is 0, SourceGroupManager will join that source group on each interface in the InterfaceIndex
    /// collection. Otherwise, it will join that source group only on that InterfaceIndex. Assuming
    /// it succeeds, this class will track it, regardless.</remarks>
    public class SourceMulticastManager
    {
        private readonly Socket socket;

        // Holds the list of interfaceIndexes that this Socket should listen to sourcegroups on
        private ICollection<uint> interfaceIndexes = new HashSet<uint>();
        private ICollection<MulticastGroupSourceOption> MulticastGroupSourceOptions = new List<MulticastGroupSourceOption>();

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

        #region Existing Source Group Memberships for this socket
        public IReadOnlyCollection<MulticastGroupSourceOption> GetSourceGroupMemberships() => ImmutableHashSet<MulticastGroupSourceOption>.Empty.Union(MulticastGroupSourceOptions);

        #endregion
        public Socket Socket => socket;

        public SourceMulticastManager(Socket socket)
        {
            if (socket.SocketType != SocketType.Dgram && socket.SocketType != SocketType.Seqpacket)
                throw new ArgumentException("Multicast socket type must be datagram or seqpacket");
            if (socket.ProtocolType != ProtocolType.Udp && socket.ProtocolType != ProtocolType.Raw)
                throw new ArgumentException("Multicast socket protocol must be UDP or RAW");
            this.socket = socket;
        }

        // We do not want to enable changing the Socket after construction, so prevent the creation of a
        // public parameterless constructor.
        private SourceMulticastManager() { }



        public bool JoinSourceGroup(MulticastGroupSourceOption mgso)
        {
            bool ReturnValue = true;
            if (mgso.Group.AddressFamily != mgso.Source.AddressFamily)
                throw new ArgumentException($"SMM: AddressFamilies do not match ({mgso.Group.AddressFamily} != {mgso.Source.AddressFamily})");
            if (!mgso.Group.IsIPv4SourceGroupMulticast() && !mgso.Group.IsIPv6SourceGroupMulticast())
                throw new ArgumentException($"SMM: {nameof(mgso.Group)} is not a sourcegroup multicast");

            // Now, we get into the meat of the operation.

            foreach (uint index in interfaceIndexes)
            {
                MulticastGroupSourceOption tmpmgso = new MulticastGroupSourceOption(mgso);
                tmpmgso.InterfaceIndex = index;
                try
                {
                    socket.JoinMulticastGroupWithSource(tmpmgso);
                    MulticastGroupSourceOptions.Add(tmpmgso);
                }
                catch (Exception)
                {
                    //// Try to clean up. The setsockopt could have succeeded, so we have to undo it; however, if it didn't succeed,
                    //// we don't need to pass up the error.
                    //try
                    //{
                    //    MulticastGroupSourceOptions.Remove(tmpmgso);
                    //    socket.LeaveMulticastGroupWithSource(tmpmgso);
                    //}
                    //catch { }
                    ReturnValue = false;
                    throw;
                }
            }

            return ReturnValue;
        }

        public void LeaveSourceGroup(MulticastGroupSourceOption mgso)
        {
            throw new NotImplementedException();
        }


    }
}
