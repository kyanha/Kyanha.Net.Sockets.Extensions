using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Kyanha.Net.Sockets.SourceMulticast.Internal;

namespace Kyanha.Net.Sockets.SourceMulticast
{
    [SupportedOSPlatform("windows")]
    public static class SocketExtensions
    {
        // MCAST_JOIN_SOURCE_GROUP
        [SupportedOSPlatform("windows")]
        public static void JoinMulticastGroupWithSource(this Socket sock, IPAddress Group, IPAddress ToListenFrom, uint Interface = 0)
            => DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode.MCAST_JOIN_SOURCE_GROUP, sock, Interface, Group, ToListenFrom);
        [SupportedOSPlatform("windows")]
        public static void JoinMulticastGroupWithSource(this Socket sock, MulticastGroupSourceOption mgso)
            => DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode.MCAST_JOIN_SOURCE_GROUP, sock, mgso.InterfaceIndex, mgso.Group, mgso.Source);

        // MCAST_LEAVE_SOURCE_GROUP
        [SupportedOSPlatform("windows")]
        public static void LeaveMulticastGroupWithSource(this Socket sock, IPAddress Group, IPAddress ToStopListeningFrom, uint Interface = 0)
            => DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode.MCAST_LEAVE_SOURCE_GROUP, sock, Interface, Group, ToStopListeningFrom);
        [SupportedOSPlatform("windows")]
        public static void LeaveMulticastGroupWithSource(this Socket sock, MulticastGroupSourceOption mgso)
            => DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode.MCAST_LEAVE_SOURCE_GROUP, sock, mgso.InterfaceIndex, mgso.Group, mgso.Source);

        private static void DoProtocolIndependentMcastSourceOperation(SourceMulticastOpcode opcode, Socket sock, uint Interface, IPAddress Group, IPAddress Source)
        {
            if (sock == null) throw new ArgumentNullException(nameof(sock));
            if (Group == null) throw new ArgumentNullException(nameof(Group));
            if (Source == null) throw new ArgumentNullException(nameof(Source));
            if (Group.AddressFamily != Source.AddressFamily) throw new ArgumentException($"Address family of {nameof(Group)} does not match address family of {nameof(Source)}");

            GroupSourceRequestData gsr = new GroupSourceRequestData()
            {
                gsr_group = new SockaddrStorage6(Group),
                gsr_source = new SockaddrStorage6(Source),
                gsr_interface = Interface
            };
            SocketError serror = Interop.setsockopt(sock.SafeHandle,
                Group.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6,
                (SocketOptionName)opcode,
                ref gsr,
                GroupSourceRequestData.Size);
            if (serror != SocketError.Success)
            {
                throw new SocketException(Marshal.GetLastWin32Error());
            }
        }

        // And now, for some IPAddress extensions.

        public static bool IsIPv6GroupMulticast(this IPAddress argument)
        {
            if (argument.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
                return false;
            //FF3x::8000:0000/97
            var bytes = argument.GetAddressBytes();
            // [0] must be 0xff
            // [1] must have the 0x30 bits set -- mandatory flags
            // [1] must not have the 0x0f bits equal to 0x0f -- 0x0f is an invalid scope
            return bytes![0] == 0xff && (bytes![1] & 0x30) == 0x30 && (bytes![1] & 0x0f) != 0x0f;
        }

        public static bool IsIPv4GroupMulticast(this IPAddress argument)
        {
            if (argument.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                return false;
            var bytes = argument.GetAddressBytes();
            return (bytes![0] == 232);
        }
    }
}
