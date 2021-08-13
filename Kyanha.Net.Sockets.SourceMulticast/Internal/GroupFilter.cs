using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    //// Definitions of MCAST_INCLUDE and MCAST_EXCLUDE for multicast source filter.
    //
    //typedef enum {
    //    MCAST_INCLUDE = 0,
    //    MCAST_EXCLUDE
    //}
    //MULTICAST_MODE_TYPE;

    //typedef struct group_filter
    //{
    //    ULONG gf_interface;
    //    SOCKADDR_STORAGE gf_group;
    //    MULTICAST_MODE_TYPE gf_fmode;
    //    ULONG gf_numsrc;
    //    SOCKADDR_STORAGE gf_slist[1];
    //}
    //GROUP_FILTER, * PGROUP_FILTER;

    /// <summary>
    /// Structure to support WSAioctl final-state-based multicast programming.
    /// </summary>
    /// <remarks>This is only part of the structure needed. The assembly of the full structure will
    /// necessarily involve finding the number of elements in the list of
    /// SocketAddresses submitted by the user, then allocating enough
    /// memory with Marshal.AllocHGlobal for
    /// <code>sizeof(GroupFilter) + (numberInList * sizeof(SockaddrStorage))</code>
    /// bytes, then creating SockaddrStorage structures from them and copying
    /// them into memory at the end of the structure.
    /// Do not forget to Marshal.FreeHGlobal this structure when complete.</remarks>
    internal struct GroupFilter6
    {
        internal UInt32 InterfaceIndex;                // 4 bytes
        internal SockaddrStorage6 Group;               // 128 bytes
        internal MulticastModeType MulticastMode;      // 4 bytes -- C enums are always int, 4 bytes.
        internal UInt32 NumberOfSources;               // 4 bytes
    }
}
