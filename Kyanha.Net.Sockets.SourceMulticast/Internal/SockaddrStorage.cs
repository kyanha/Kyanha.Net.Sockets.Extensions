using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    /// <summary>
    /// Managed implementation of native struct SOCKADDR_STORAGE.
    /// </summary>
    /// <remarks>For use with the following setsockopt opcodes:
    /// MCAST_BLOCK_SOURCE
    /// MCAST_JOIN_SOURCE_GROUP
    /// MCAST_LEAVE_SOURCE_GROUP
    /// MCAST_UNBLOCK_SOURCE</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SockaddrStorage
    {
        [MarshalAs(UnmanagedType.I2)]
        internal short Family;      // formally defined as ADDRESS_FAMILY, but it's a C short (2 bytes)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        byte[] AddressData;
        [MarshalAs(UnmanagedType.U8)]
        UInt64 AlignmentAid;        // 8 bytes for alignment
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 112)]
        byte[] PaddingBlock2;

        internal static int Size { get => Marshal.SizeOf<SockaddrStorage>(); }

        internal unsafe SockaddrStorage(SocketAddress input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            this.Family = (short)input.Family;
            // We have to zero all the other fields out, which is done by the byte[] constructors.
            this.AddressData = new byte[6];
            this.AlignmentAid = 0;
            this.PaddingBlock2 = new byte[112];

            // This is an unsafe block because of the definition of MSSA and the fact that we can have more
            // bytes to deal with than the size of the padding would allow us to directly address.
            unsafe
            {
                fixed (byte* addrbyte = &this.AddressData[0])
                {
                    for (int i = 0; i < input.Size; i++)
                    {
                        addrbyte[i] = input[i];
                    }
                }
            }
            return;
        }

        static public implicit operator SocketAddress(SockaddrStorage input)
        {
            SocketAddress sa = new SocketAddress((AddressFamily)input.Family);
            for (int i = 0; i < sa.Size; i++)
            {
                sa[i] = input.AddressData[i];
            }
            return sa;
        }

        static public implicit operator SockaddrStorage(SocketAddress sa)
        {
            return new SockaddrStorage(sa);
        }
    }
}
