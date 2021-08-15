using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using static Kyanha.Net.Sockets.SourceMulticast.Internal.Interop;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    /// <summary>
    /// Managed implementation of native struct SOCKADDR_STORAGE for IPv6.
    /// </summary>
    /// <remarks>For use with the following setsockopt opcodes:
    /// MCAST_BLOCK_SOURCE
    /// MCAST_JOIN_SOURCE_GROUP
    /// MCAST_LEAVE_SOURCE_GROUP
    /// MCAST_UNBLOCK_SOURCE
    /// 
    /// Implements the following structures inside a 128-byte container block:
    /// typedef struct in6_addr {
    ///     union {
    ///         UCHAR Byte[16];
    ///         USHORT Word[8];
    ///     } u;
    /// } IN6_ADDR, *PIN6_ADDR, FAR* LPIN6_ADDR;
    /// typedef struct sockaddr_in6_w2ksp1 {
    ///    short sin6_family;        /* AF_INET6 */
    ///   USHORT sin6_port;          /* Transport level port number */
    ///    ULONG sin6_flowinfo;      /* IPv6 flow information */
    ///    struct in6_addr sin6_addr;  /* IPv6 address */
    ///    ULONG sin6_scope_id;       /* set of interfaces for a scope */
    /// } SOCKADDR_IN6_W2KSP1, *PSOCKADDR_IN6_W2KSP1, FAR* LPSOCKADDR_IN6_W2KSP1;
    ///</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct SockaddrStorage6
    {
        private Int16 family;                   // offset 0   - 0 longs
        private ushort port;                    // offset 2
        private UInt32 flowInformation;         // offset 4
        private fixed byte addressData[16];     // offset 8   - 1 long
        private ulong scopeID;                  // offset 24  - 3 longs
        private  fixed UInt64 Padding[12];       // offset 32  - 4 longs

        internal static int Size { get => Marshal.SizeOf<SockaddrStorage6>(); }
        public short Family
        {
            get => family;
            set
            {
                if (value != (short)AddressFamily.InterNetworkV6)
                    throw new ArgumentException("Requires AddressFamily.InterNetworkV6");
                family = value;
            }
        }

        public ushort Port { get => NetworkToHostShort(port); set => port = HostToNetworkShort(value); }
        public UInt32 FlowInformation { get => NetworkToHostInt32(flowInformation); set => flowInformation = HostToNetworkInt32(value); }
        public ulong ScopeID { get => NetworkToHostLong(scopeID); set => scopeID = HostToNetworkLong(value); }
        public byte[] AddressData
        {
            get
            {
                var retval = new byte[16];
                fixed (byte* data = addressData)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        retval[i] = data[i];
                    }
                }
                Array.Reverse(retval);
                return retval;
            }
            set
            {
                if (value.Length != 16)
                    throw new ArgumentException($"Expected a byte[16], got a byte[{value.Length}]");
                // Should not have side effects on the array passed as 'value', so we copy to a new one
                var tmp = new byte[value.Length];
                value.CopyTo(tmp, 0);
                Array.Reverse(tmp);
                fixed (byte* data = addressData)
                {
                    for(int i = 0; i < tmp.Length; i++)
                    {
                        data[i] = tmp[i];
                    }
                }
            }
        }

        internal unsafe SockaddrStorage6(IPAddress input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                throw new ArgumentException($"Expected address family {AddressFamily.InterNetworkV6}, received address family {input.AddressFamily}");
            }

            family = (short)input.AddressFamily;
            // We have to zero all the other fields out before we can bytecopy
            port = 0;
            scopeID = 0;
            flowInformation = 0;
            for (int i = 0; i < 12; i++)
                Padding[i] = 0;

            // Now, we copy the address bytes over.
            {
                byte[] inputAddress = input.GetAddressBytes();
                for (int i = 0; i < 16; i++)
                {
                    addressData[i] = inputAddress[i];
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
                if (offset < 0 || offset > 125)
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

        static public implicit operator IPAddress(SockaddrStorage6 input)
        {
            return new IPAddress(input.AddressData);
        }

        static public implicit operator SockaddrStorage6(IPAddress sa)
        {
            return new SockaddrStorage6(sa);
        }

        static public explicit operator SockaddrStorage6(SockaddrStorage sas)
        {
            if(sas.Family != (short)AddressFamily.InterNetworkV6)
            { 
                throw new ArgumentException($"Expected address family {(Int16)AddressFamily.InterNetworkV6}, received address family {sas.Family}");
            }
            SockaddrStorage6 sas6 = new SockaddrStorage6
            {
                Family = sas.Family
            };
            for (int i=0; i<126; i++)
            {
                sas6[i] = sas.StructureData[i];
            }
            return sas6;
        }

        static public implicit operator SockaddrStorage(SockaddrStorage6 sas6)
        {
            SockaddrStorage sas = new SockaddrStorage();
            sas.Family = sas6.Family;
            for (int i=0; i < 126; i++)
            {
                sas.StructureData[i] = sas6[i];
            }
            return sas;
        }
    }
}
