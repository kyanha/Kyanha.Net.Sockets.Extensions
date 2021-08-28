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
        public static void JoinMulticastGroupWithSource(this Socket sock, IPAddress Group, IPAddress ToListenFrom, uint Interface = 0)
            => DoProtocolIndependentMcastSourceOperation(sock, SourceMulticastOpcode.MCAST_JOIN_SOURCE_GROUP, sock, Interface, Group, ToListenFrom);
        public static void JoinMulticastGroupWithSource(this Socket sock, MulticastGroupSourceOption mgso)
            => DoProtocolIndependentMcastSourceOperation(sock, SourceMulticastOpcode.MCAST_JOIN_SOURCE_GROUP, sock, mgso.InterfaceIndex, mgso.Group, mgso.Source);

        // MCAST_LEAVE_SOURCE_GROUP
        public static void LeaveMulticastGroupWithSource(this Socket sock, IPAddress Group, IPAddress ToStopListeningFrom, uint Interface = 0)
            => DoProtocolIndependentMcastSourceOperation(sock, SourceMulticastOpcode.MCAST_LEAVE_SOURCE_GROUP, sock, Interface, Group, ToStopListeningFrom);
        public static void LeaveMulticastGroupWithSource(this Socket sock, MulticastGroupSourceOption mgso)
            => DoProtocolIndependentMcastSourceOperation(sock, SourceMulticastOpcode.MCAST_LEAVE_SOURCE_GROUP, sock, mgso.InterfaceIndex, mgso.Group, mgso.Source);

        private unsafe static void DoProtocolIndependentMcastSourceOperation(this Socket socket, SourceMulticastOpcode opcode, Socket sock, uint Interface, IPAddress Group, IPAddress Source)
        {
            if (sock == null) throw new ArgumentNullException(nameof(sock));
            if (Group == null) throw new ArgumentNullException(nameof(Group));
            if (Source == null) throw new ArgumentNullException(nameof(Source));
            if (Group.AddressFamily != Source.AddressFamily) throw new ArgumentException($"Address family of {nameof(Group)} does not match address family of {nameof(Source)}");

            //// Okay, we have to be Clever[tm] here. Windows really does demand that a GROUP_SOURCE_REQ's
            //// SOCKADDR_STORAGE members be aligned on a 64-bit boundary.

            //GSRContainer container = new GSRContainer();

            //container.Align.CopyInFamily((short)Source.AddressFamily);
            //SockaddrStorage sas = new SockaddrStorage(Group);
            //container.Align.CopyInSockaddrStorageAtOffset(sas, 4);
            //sas = new SockaddrStorage(Source);
            //container.Align.CopyInSockaddrStorageAtOffset(sas, 132);

            //// Now, we need to get the address of the correct offset in the GSRContainer.Align.ment[] to pass as the parameter here.

            SocketError serror = SocketError.Success;

            //fixed (byte* gsr = &container.Align.ment[container.Offset()])
            //{
            //    serror = Interop.setsockopt(sock.SafeHandle,
            //        Group.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6,
            //        opcode,
            //        gsr,
            //        260);
            //}
            SockaddrStorage sasGroup = Group;
            SockaddrStorage sasSource = Source;
            GroupSourceReqStruct gsr = new GroupSourceReqStruct { gsr_interface = Interface, gsr_group = sasGroup, gsr_source = sasSource };

            int sas6size = SockaddrStorage6.Size;
            int sas4size = SockaddrStorage4.Size;

            serror = Interop.setsockopt(sock.SafeHandle,
                Group.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6,
                opcode,
                ref gsr,
                260);
//                GroupSourceReqStruct.Size);

            if (serror != SocketError.Success)
            {
                throw new SocketException(Marshal.GetLastWin32Error());
            }
        }


        // And now, for some IPAddress extensions.
        public static bool IsIPv6SourceGroupMulticast(this IPAddress argument)
        {
            if (argument.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
                return false;
            //FF3x::8000:0000/97
            var bytes = argument.GetAddressBytes();
            // [0] must be 0xff
            // [1] must have the 0x30 bits set -- mandatory flags (but 0x40 may eventually also be a valid flag)
            // [1] must not have the 0x0f bits equal to 0x0f -- 0x0f is an invalid scope
            return bytes![0] == 0xff && (bytes![1] & 0x30) == 0x30 && (bytes![1] & 0x0f) != 0x0f;
        }

        public static bool IsIPv4SourceGroupMulticast(this IPAddress argument)
        {
            if (argument.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                return false;
            var bytes = argument.GetAddressBytes();
            // 232.0.0.0/8
            return (bytes![0] == 232);
        }
    }
}

