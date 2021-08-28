using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 128)]
    internal unsafe struct sockaddr_storage_as_byte_array
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 128)]
        internal fixed byte data[128];

        public unsafe static implicit operator sockaddr_storage_as_byte_array(SockaddrStorage sas)
        {
            sockaddr_storage_as_byte_array ssba = new sockaddr_storage_as_byte_array();

            byte* ptrsas = (byte*)&sas.Family;
            for (int i = 0; i < 128; i++)
            {
                ssba.data[i] = ptrsas[i];
            }
            return ssba;
        }

        public unsafe static implicit operator SockaddrStorage(sockaddr_storage_as_byte_array sasba)
        {
            SockaddrStorage sas = new SockaddrStorage();
            byte* ptrsas = (byte*)&sasba.data;
            sas.Family = Marshal.ReadInt16((IntPtr)ptrsas);
            for(int i = 2; i<128; i++)
            {
                sas[i - 2] = ptrsas[i];
            }
            return sas;
        }
    }
}
