using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    /// <summary>
    /// Placeholder for both SockaddrStorage4 and SockaddrStorage6.
    /// </summary>
    internal unsafe struct SockaddrStorage
    {
        internal Int16 Family;
        internal fixed byte StructureData[126];

        internal static int Size { get => Marshal.SizeOf<SockaddrStorage>(); }

        public byte this[int offset]
        {
            get
            {
                if (offset < 0 || offset > 125)
                    throw new ArgumentOutOfRangeException("0 to 125, please");
                return StructureData[offset]; 
            }
            set
            {
                if (offset < 0 || offset > 125)
                {
                    throw new ArgumentOutOfRangeException("0 to 125, please");
                }
                StructureData[offset] = value;
            }
        }
        public SockaddrStorage(IPAddress address)
        {
            Family = (short)address.AddressFamily;

            if (Family == (short)AddressFamily.InterNetworkV6)
            {
                unsafe
                {
                    SockaddrStorage6 ss6 = new(address);
                    for (int i = 0; i < 125; i++)
                    {
                        StructureData[i] = ss6[i];
                    }
                }
            }
            else if (Family == (short)AddressFamily.InterNetwork)
            {
                unsafe
                {
                    SockaddrStorage4 ss4 = new(address);
                    for (int i = 0; i < 125; i++)
                    {
                        StructureData[i] = ss4[i];
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Unknown address family {address.AddressFamily}");
            }
        }

        public static implicit operator SockaddrStorage(IPAddress address) => new SockaddrStorage(address);

        public static implicit operator IPAddress(SockaddrStorage sas)
        {
            if (sas.Family == (short)AddressFamily.InterNetwork)
            {
                var sas4 = (SockaddrStorage4)sas;
                return new IPAddress(sas4.AddressData);
            }
            else if (sas.Family == (short)AddressFamily.InterNetworkV6)
            {
                var sas6 = (SockaddrStorage6)sas;
                return new IPAddress(sas6.AddressData);
            }
            else
            {
                throw new ArgumentException($"{nameof(SockaddrStorage)} was neither {(short)AddressFamily.InterNetwork} nor {(short)AddressFamily.InterNetworkV6}");
            }
        }
    }
}
