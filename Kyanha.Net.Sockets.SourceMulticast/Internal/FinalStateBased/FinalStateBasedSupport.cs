using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kyanha.Net.Sockets.SourceMulticast.Internal;


namespace Kyanha.Net.Sockets.SourceMulticast.Internal.FinalStateBased
{
    public static class FinalStateBasedSupport
    {

        public static void AddListToMulticastGroup(this Socket socket, UInt32 InterfaceIndex, IPAddress GroupToFilter, IList<IPAddress> IPAddresses) =>
            PerformWSAIoctlForMulticastFilter(socket, InterfaceIndex, GroupToFilter, IPAddresses, Internal.MulticastModeType.McastInclude);
        
        internal static void PerformWSAIoctlForMulticastFilter(Socket socket, UInt32 InterfaceIndex, IPAddress GroupToFilter, IList<IPAddress> IPAddresses, Internal.MulticastModeType operation)
        {
            // First, figure out which structure we need.
            AddressFamily Family = (AddressFamily)GroupToFilter.AddressFamily;


            // Allocate the GroupFilter structure.
            GroupFilter groupFilter = new GroupFilter();

            // Set the members of the structure.
            groupFilter.InterfaceIndex = InterfaceIndex;
            groupFilter.Group = GroupToFilter;
            groupFilter.NumberOfSources = (uint)IPAddresses.Count;
            groupFilter.MulticastMode = operation;
            // The size of the above parameters is 140.

            int sizeToAllocate = 148 + (IPAddresses.Count * 128);

            byte[] WSAIoctlBuffer = new byte[sizeToAllocate];

            unsafe
            {
                IntPtr ptr = IntPtr.Zero;
                try
                {
                    ptr = Marshal.AllocHGlobal(sizeToAllocate);
                    int offset = 0;
                    byte* dest = (byte*)ptr.ToPointer();
                    byte* src = (byte*)&groupFilter.InterfaceIndex;
                    for (int i=0;i<148;i++)
                    {
                        dest[offset++] = src[i];
                    }

                    // Now, we need to convert the elements of IPAddresses to SockaddrStorage structures,
                    // and append them to the initialization that's occurred before.

                    for (int i = 0; i < IPAddresses.Count; i++)
                    {
                        Internal.SockaddrStorage6 sockaddrStorage = IPAddresses[i];
                        byte* sockaddrsrc = (byte*)&sockaddrStorage;
                        for (int j = 0; j<128; j++)
                        {
                            dest[offset++] = sockaddrsrc[j];
                        }
                    }

                    // So, the structure's complete. Now, we need to do the WSAioctl call.
                    SocketError ioctlreturn = Internal.Interop.WSAIoctl_Blocking(
                        socket.SafeHandle,
                        ioctlcodes.SIOCSMSFILTER,
                        ptr,
                        sizeToAllocate,
                        IntPtr.Zero,
                        0,
                        out int bytesTransferred,
                        IntPtr.Zero,
                        IntPtr.Zero
                        );
                    if(ioctlreturn!= 0)
                    {
                        throw new SocketException(Marshal.GetLastWin32Error());
                    }    
                    // No error, so we have success.
                } finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }
}
