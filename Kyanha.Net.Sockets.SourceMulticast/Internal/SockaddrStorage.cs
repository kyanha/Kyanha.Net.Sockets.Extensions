using System;
using System.Net;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    /// <summary>
    /// Placeholder for both SockaddrStorage4 and SockaddrStorage6.
    /// </summary>
    internal unsafe struct SockaddrStorage
    {
        internal short Family;
        internal fixed byte StructureData[126];

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
        public static implicit operator SockaddrStorage(IPAddress address)
        {
            SockaddrStorage ss = new();
            ss.Family = (short)address.AddressFamily;

            if (ss.Family == (short)AddressFamily.InterNetworkV6)
            {
                unsafe
                {
                    SockaddrStorage6 ss6 = new(address);
                    for (int i = 0; i < 125; i++)
                    {
                        ss.StructureData[i] = ss6[i];
                    }
                }
            } 
            else if (ss.Family == (short)AddressFamily.InterNetwork)
            {
                unsafe
                {
                    SockaddrStorage4 ss4 = new(address);
                    for (int i = 0; i < 125; i++)
                    {
                        ss.StructureData[i] = ss4[i];
                    }
                }
            } else
            {
                throw new ArgumentException($"Unknown address family {address.AddressFamily}");
            }
            return ss;
        }
    }
}
