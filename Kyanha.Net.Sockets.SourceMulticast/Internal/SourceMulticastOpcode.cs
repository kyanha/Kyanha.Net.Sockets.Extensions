using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    enum SourceMulticastOpcode
    {
        MCAST_BLOCK_SOURCE = 43,    // Block IP group/source.
        MCAST_UNBLOCK_SOURCE = 44,  // Unblock IP group/source.
        MCAST_JOIN_SOURCE_GROUP = 45,   // Join IP group/source.
        MCAST_LEAVE_SOURCE_GROUP = 46,  // Leave IP group/source.
    }
}
