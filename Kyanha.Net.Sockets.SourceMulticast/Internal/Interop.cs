using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    internal partial class Interop
    {
        #region setsockopt
        [DllImport("Ws2_32.dll", SetLastError = true)]
        internal static extern SocketError setsockopt(
            [In] SafeSocketHandle socketHandle,
            [In] SocketOptionLevel optionLevel,
            [In] SourceMulticastOpcode optionName,
            [In] ref group_source_req optionValue,
            [In] int optionLength);
        [DllImport("Ws2_32.dll", SetLastError = true)]
        internal unsafe static extern SocketError setsockopt(
            [In] SafeSocketHandle socketHandle,
            [In] SocketOptionLevel optionLevel,
            [In] SourceMulticastOpcode optionName,
            [In] byte* optionValue,
            [In] int optionLength);
        [DllImport("Ws2_32.dll", SetLastError = true)]
        internal unsafe static extern SocketError setsockopt(
            [In] SafeSocketHandle socketHandle,
            [In] SocketOptionLevel optionLevel,
            [In] SourceMulticastOpcode optionName,
            [In] ref GroupSourceReqStruct optionValue,
            [In] int optionLength);
        #endregion
        #region host to network byte order (managed)
        internal static ulong HostToNetworkLong(ulong value) => WinsockSwapUInt64(value);
        internal static ulong NetworkToHostLong(ulong value) => WinsockSwapUInt64(value);
        internal static UInt32 HostToNetworkInt32(uint value) => WinsockSwapUInt32(value);
        internal static UInt32 NetworkToHostInt32(uint value) => WinsockSwapUInt32(value);
        internal static ushort HostToNetworkShort(ushort value) => WinsockSwapUInt16(value);
        internal static ushort NetworkToHostShort(ushort value) => WinsockSwapUInt16(value);
        #endregion

        #region Utility functions that were #defines
        private static ushort WinsockSwapUInt16(ushort value)
        {
            return
               (ushort)
               (((value >> 8) & 0x00ff) |
                 (value << 8) & 0xff00);
        }
        private static uint WinsockSwapUInt32(uint value)
        {
            uint retval;
            retval = 
             ((value >> 24) & 0x000000ff) |
             ((value >> 8)  & 0x0000ff00) |
             ((value << 8)  & 0x00ff0000) |
             ((value << 24) & 0xff000000);
            return retval;
        }
        private static ulong WinsockSwapUInt64(ulong value)
        {
            ulong retval;
            retval =
              (((value) >> 56) & 0x00000000000000FF) |
              (((value) >> 40) & 0x000000000000FF00) |
              (((value) >> 24) & 0x0000000000FF0000) |
              (((value) >> 8)  & 0x00000000FF000000) |
              (((value) << 8)  & 0x000000FF00000000) |
              (((value) << 24) & 0x0000FF0000000000) |
              (((value) << 40) & 0x00FF000000000000) |
              (((value) << 56) & 0xFF00000000000000);
            return retval;
        }
        #endregion
    }
}
