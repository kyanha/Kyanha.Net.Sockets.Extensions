using System;
using System.Net;
using System.Runtime.InteropServices;
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
        public ushort Port { get => port; set => port = value; }
        public UInt32 AddressData { get => addressData; set => addressData = value; }

        internal unsafe SockaddrStorage4(IPAddress input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new ArgumentException($"Expected address family {AddressFamily.InterNetwork}, received address family {input.AddressFamily}");
            }
            // We have to zero all the other fields out before we can set the addressData.
            family = (short)input.AddressFamily;
            port = 0;
            addressData = 0;
            zero = 0;
            for (int i = 0; i < 15; i++)
                Padding[i] = 0;

            // This, because we can't have unions in C#.
            fixed (uint* datashort = &addressData)
            {
                byte* data = (byte*)datashort;
                for (int i = 0; i < input.GetAddressBytes().Length; i++)
                {
                    data[i] = input.GetAddressBytes()[i];
                }
            }
            return;
        }

        // Implements the same interface as IPAddress, i.e. gives the entire
        // structure starting at offset 2 (because Family is read-only)
        public byte this[int offset]
        {
            get
            {
                if (offset < 0 || offset > 125)
                {
                    throw new IndexOutOfRangeException("0 to 125, please");
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
                if (offset < 0 || offset > 125)
                {
                    throw new IndexOutOfRangeException("0 to 125, please");
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

        static public implicit operator IPAddress(SockaddrStorage4 input)
        {
            return new IPAddress(input.AddressData);
        }

        static public implicit operator SockaddrStorage4(IPAddress sa)
        {
            return new SockaddrStorage4(sa);
        }

        static public implicit operator SockaddrStorage4(SockaddrStorage sa)
        {
            if (sa.Family != (short)AddressFamily.InterNetwork)
            {
                throw new ArgumentException($"Expected address family {AddressFamily.InterNetwork}, received address family {sa.Family}");
            }
            SockaddrStorage4 ss4 = new();
            for(int i = 0; i < 126; i++)
            {
                ss4[i] = sa[i];
            }
            return ss4;
        }

        static public implicit operator SockaddrStorage(SockaddrStorage4 sas4)
        {
            SockaddrStorage sas = new SockaddrStorage();
            sas.Family = sas4.Family;
            for (int i = 0; i < 126; i++)
            {
                sas.StructureData[i] = sas4[i];
            }
            return sas;
        }
    }
}
