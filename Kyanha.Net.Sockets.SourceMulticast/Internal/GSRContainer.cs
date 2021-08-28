using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    /// <summary>
    /// Needed to allow fixed() expressions on the inner fixed byte[].
    /// </summary>
    internal unsafe struct GSRAlignment
    {
        internal fixed byte ment[267];

        /// <summary>
        /// Returns the number of bytes to offset for the buffer to be aligned at 0x04 or 0x0c.
        /// </summary>
        internal uint Offset()
        {
            fixed (byte *b = ment)
            {
                switch ((ulong)b % 8)
                {
                    case 0:
                        return 4;
                    case 1:
                        return 3;
                    case 2:
                        return 2;
                    case 3:
                        return 1;
                    case 4:
                        return 0;
                    case 5:
                        return 7;
                    case 6:
                        return 6;
                    case 7:
                        return 5;
                }
            }
            throw new OverflowException("a mod 8 operation returned a value outside 0 through 7?!");
        }

        internal void CopyInSockaddrStorageAtOffset(SockaddrStorage sockaddrStorage, uint offset)
        {
            if (offset != 4 && offset != 132)
                throw new ArgumentException("Offset must be either 4 or 132");
            byte* sas = (byte*)&sockaddrStorage;
            for (int i = 0; i < 128; i++)
            {
                ment[Offset() + offset + i] = sas[i];
            }
        }

        internal void CopyInFamily(short Family)
        {
            byte* family = (byte*)&Family;
            ment[Offset() + 0] = family[0];
            ment[Offset() + 1] = family[1];
        }
    }

    internal unsafe class GSRContainer
    {
        internal GSRAlignment Align;

        internal uint Offset()
        {
            return Align.Offset();
        }
    }
}
