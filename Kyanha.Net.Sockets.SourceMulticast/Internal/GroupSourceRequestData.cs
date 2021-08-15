using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    /// <summary>
    /// Managed implementation of native structure GROUP_SOURCE_REQ.
    /// </summary>
    /// <remarks>
    /// typedef struct group_source_req
    /// {
    ///     ULONG gsr_interface;        // Interface index.
    ///     SOCKADDR_STORAGE gsr_group; // Group address.
    ///     SOCKADDR_STORAGE gsr_source; // Source address.
    /// } GROUP_SOURCE_REQ, * PGROUP_SOURCE_REQ;
    /// </remarks>
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    internal struct GroupSourceRequestData
    {
        /// <summary>
        /// The interface index (for the socket's protocol family) to operate upon.
        /// </summary>
        internal UInt32 gsr_interface;
        /// <summary>
        /// The multicast group to declare a source for.
        /// </summary>
        internal SockaddrStorage6 gsr_group;
        /// <summary>
        /// The unicast address of the source to operate with.
        /// </summary>
        internal SockaddrStorage6 gsr_source;

        internal static readonly int Size = Marshal.SizeOf<GroupSourceRequestData>();

        //public static implicit operator MulticastGroupSourceOption(GroupSourceRequestData gsrd)
        //{
        //    MulticastGroupSourceOption mgso = new MulticastGroupSourceOption();
        //    mgso.InterfaceIndex = gsrd.gsr_interface;
        //    mgso.Group = gsrd.gsr_group;
        //    mgso.Source = gsrd.gsr_source;
        //    return mgso;
        //}

        //public static implicit operator GroupSourceRequestData(MulticastGroupSourceOption mgso)
        //{
        //    GroupSourceRequestData gsrd = new GroupSourceRequestData();
        //    gsrd.gsr_interface = mgso.InterfaceIndex;
        //    gsrd.gsr_group = mgso.Group;
        //    gsrd.gsr_source = mgso.Source;
        //    return gsrd;
        //}
    }
}
