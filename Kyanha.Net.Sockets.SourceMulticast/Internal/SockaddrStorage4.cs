using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kyanha.Net.Sockets.SourceMulticast.Internal.Interop;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    /// <summary>
    /// SockaddrStorage for IPv4.
    /// </summary>
    /// <remarks>typedef struct in_addr {
    /// union {
    ///     struct { UCHAR s_b1, s_b2, s_b3, s_b4; } S_un_b;
    ///     struct { USHORT s_w1, s_w2; } S_un_w;
    ///     ULONG S_addr;
    /// } S_un;
    /// struct sockaddr_in {
    ///     short sin_family;
    ///     u_short sin_port;
    ///     struct in_addr sin_addr;
    ///     char sin_zero[8];
    /// };
    ///</remarks>

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct SockaddrStorage4
    {
        private Int16 family;                     // offset 0: C short is 2 bytes
        private UInt16 port;                      // offset 2  
        private UInt32 addressData;               // offset 4: C ULONG is 4 bytes
        private UInt64 zero;                      // offset 8  - 1 long
        private fixed ulong Padding[14];          // offset 16 - 2 longs

        internal static int Size { get => Marshal.SizeOf<SockaddrStorage4>(); }
        public short Family { get => family; set => family = value; }
        public ushort Port { get => Interop.NetworkToHostShort(port); set => port = HostToNetworkShort(value); }
        public UInt32 AddressData { get => NetworkToHostInt32(addressData); set => addressData = HostToNetworkInt32(value); }

        internal unsafe SockaddrStorage4(SocketAddress input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input.Family != AddressFamily.InterNetwork)
            {
                throw new ArgumentException($"Expected address family {AddressFamily.InterNetwork}, received address family {input.Family}");
            }
            // We have to zero all the other fields out, which is done by the byte[] constructors.
            family = (short)input.Family;
            port = 0;
            addressData = 0;
            zero = 0;
            for (int i = 0; i < 15; i++)
                Padding[i] = 0;

            // This not &family because SocketAddress doesn't put its Family in its access array.
            fixed (ushort* datashort = &port)
            {
                byte* data = (byte*)datashort;
                for (int i = 0; i < input.Size; i++)
                {
                    data[i] = input[i];
                }
            }
            return;
        }

        // Implements the same interface as SocketAddress, i.e. gives the entire
        // structure starting at offset 2 (because Family is read-only)
        public byte this[int offset]
        {
            get
            {
                if (offset < 0 || offset > 126)
                {
                    throw new IndexOutOfRangeException();
                }
                unsafe
                {
                    fixed (ushort* datashort = &port)
                    {
                        byte* data = (byte*)(void*)datashort;
                        return data[offset];
                    }
                }
            }
            set
            {
                if (offset < 0 || offset > 126)
                {
                    throw new IndexOutOfRangeException();
                }
                unsafe
                {
                    fixed (ushort* datashort = &port)
                    {
                        byte* data = (byte*)(void*)datashort;
                        data[offset] = value;
                    }
                }
            }
        }

        static public implicit operator SocketAddress(SockaddrStorage4 input)
        {
            SocketAddress sa = new SocketAddress((AddressFamily)input.Family);
            for (int i = 0; i < sa.Size; i++)
            {
                sa[i] = input[i];
            }
            return sa;
        }

        static public implicit operator SockaddrStorage4(SocketAddress sa)
        {
            return new SockaddrStorage4(sa);
        }
    }
}
