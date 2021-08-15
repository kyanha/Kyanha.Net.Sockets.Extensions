using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal.FinalStateBased
{
    internal enum ioctlcodes : uint
    {
        SIOCSMSFILTER = 0x8004747E,
        SIOCGMSFILTER = 0x8004747F
    }
}
