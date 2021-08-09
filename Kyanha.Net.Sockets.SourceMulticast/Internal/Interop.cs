using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    partial class Interop
    {
        [DllImport("Ws2_32.dll", SetLastError = true)]
        internal static unsafe extern SocketError setsockopt(
       [In] SafeSocketHandle socketHandle,
       [In] SocketOptionLevel optionLevel,
       [In] SocketOptionName optionName,
       [In] ref GroupSourceRequestData optionValue,
       [In] int optionLength);
    }
}
