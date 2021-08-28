using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    //    /// <summary>
    //    /// Managed implementation of native structure GROUP_SOURCE_REQ.
    //    /// </summary>
    //    /// <remarks>
    //    /// typedef struct group_source_req
    //    /// {
    //    ///     ULONG gsr_interface;        // Interface index.
    //    ///     SOCKADDR_STORAGE gsr_group; // Group address.
    //    ///     SOCKADDR_STORAGE gsr_source; // Source address.
    //    /// } GROUP_SOURCE_REQ, * PGROUP_SOURCE_REQ;
    //    /// </remarks>
 [StructLayout(LayoutKind.Sequential,Pack = 1, Size = 260)]
    internal struct GroupSourceReqStruct
    {
        /// <summary>
        /// The interface index (for the socket's protocol family) to operate upon.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        internal UInt32 gsr_interface;
        /// <summary>
        /// The multicast group to declare a source for.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray,ArraySubType = UnmanagedType.U1,SizeConst = 128)]
        internal sockaddr_storage_as_byte_array gsr_group;
        /// <summary>
        /// The unicast address of the source to operate with.
        /// </summary>
        /// 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        internal sockaddr_storage_as_byte_array gsr_source;

        internal static int Size { get => Marshal.SizeOf<GroupSourceReqStruct>(); }

        public static implicit operator GroupSourceReqStruct(MulticastGroupSourceOption mgso)
        {
            if (mgso.Source.AddressFamily != mgso.Group.AddressFamily)
            {
                throw new ArgumentException($"GSR: {nameof(MulticastGroupSourceOption)} source address family {mgso.Source.AddressFamily} != group address family {mgso.Group.AddressFamily}.");
            }
            if (!mgso.Group.IsIPv4SourceGroupMulticast() && !mgso.Group.IsIPv6SourceGroupMulticast())
            {
                throw new ArgumentException($"GSR: {nameof(MulticastGroupSourceOption)} group is not a source multicast group.");
            }
            var gsrd = new GroupSourceReqStruct { gsr_group = (SockaddrStorage)mgso.Group, gsr_source = (SockaddrStorage)mgso.Source, gsr_interface = mgso.InterfaceIndex };
            return gsrd;
        }

        public static implicit operator MulticastGroupSourceOption(GroupSourceReqStruct gsrd)
        {
            return new MulticastGroupSourceOption((SockaddrStorage)gsrd.gsr_source, (SockaddrStorage)gsrd.gsr_group, gsrd.gsr_interface);
        }
    }
}