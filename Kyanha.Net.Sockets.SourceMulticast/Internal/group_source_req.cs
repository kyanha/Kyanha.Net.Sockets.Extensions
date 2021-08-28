using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{

    ///<summary>Internal struct to cast fixed memory to</summary>
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    internal class group_source_req
    {
        [MarshalAs(UnmanagedType.U4)]
        internal UInt32 Interface;
        [MarshalAs(UnmanagedType.ByValArray,ArraySubType = UnmanagedType.U1,SizeConst = 128)]
        internal SockaddrStorage Group;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        internal SockaddrStorage Source;
    }
}
