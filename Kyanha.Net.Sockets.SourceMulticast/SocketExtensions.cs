using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Kyanha.Net.Sockets.SourceMulticast.Internal;

namespace Kyanha.Net.Sockets.SourceMulticast
{
    public static class SocketExtensions
    {
        // MCAST_JOIN_SOURCE_GROUP
        [SupportedOSPlatform("windows")]
        public static void JoinMulticastGroupWithSource(this Socket sock, SocketAddress Group, SocketAddress ToListenFrom, ulong Interface)
            => DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode.MCAST_JOIN_SOURCE_GROUP, sock, Interface, Group, ToListenFrom);
        [SupportedOSPlatform("windows")]
        public static void JoinMulticastGroupWithSource(this Socket sock, MulticastGroupSourceOption mgso)
            => DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode.MCAST_JOIN_SOURCE_GROUP, sock, mgso.InterfaceIndex, mgso.Group, mgso.Source);

        // MCAST_LEAVE_SOURCE_GROUP
        [SupportedOSPlatform("windows")]
        public static void LeaveMulticastGroupWithSource(this Socket sock, SocketAddress Group, SocketAddress ToStopListeningFrom, ulong Interface)
            => DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode.MCAST_LEAVE_SOURCE_GROUP, sock, Interface, Group, ToStopListeningFrom);
        [SupportedOSPlatform("windows")]
        public static void LeaveMulticastGroupWithSource(this Socket sock, MulticastGroupSourceOption mgso)
            => DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode.MCAST_LEAVE_SOURCE_GROUP, sock, mgso.InterfaceIndex, mgso.Group, mgso.Source);

        internal static void DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode opcode, Socket sock, ulong Interface, SocketAddress Group, SocketAddress Source)
        {
            if (sock == null) throw new ArgumentNullException(nameof(sock));
            if (Group == null) throw new ArgumentNullException(nameof(Group));
            if (Source == null) throw new ArgumentNullException(nameof(Source));
            if (Group.Family != Source.Family) throw new ArgumentException($"Address family of {nameof(Group)} does not match address family of {nameof(Source)}");

            GroupSourceRequestData gsr = new GroupSourceRequestData()
            {
                gsr_group = new MulticastSourceSocketAddress(Group),
                gsr_source = new MulticastSourceSocketAddress(Source),
                gsr_interface = Interface
            };
            SocketError serror = Interop.setsockopt(sock.SafeHandle,
                Group.Family == AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6,
                (SocketOptionName)opcode,
                ref gsr,
                GroupSourceRequestData.Size);
            if (serror != SocketError.Success)
            {
                throw new SocketException(Marshal.GetLastWin32Error());
            }
        }
    }
}
