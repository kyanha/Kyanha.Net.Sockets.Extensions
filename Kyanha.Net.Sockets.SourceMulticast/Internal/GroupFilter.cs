using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
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
    /// <code>(sizeof(GroupFilter)-1) + (numberInList * sizeof(SockaddrStorage))</code>
    /// bytes, then creating SockaddrStorage structures from them and copying
    /// them into memory at
    /// <code>&gf_slist+(listindex * sizeof(SockaddrStorage)</code>.
    /// Do not forget to Marshal.FreeHGlobal this structure when complete.</remarks>
    internal struct GroupFilter
    {
        ulong gf_interface;
        SockaddrStorage gf_group;
        MulticastModeType gf_fmode;
        ulong gf_numsrc;
        byte gf_slist1;
    }
}
