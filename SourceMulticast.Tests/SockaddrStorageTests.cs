using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using Kyanha.Net.Sockets.SourceMulticast.Internal;

namespace SourceMulticast.Tests
{
    [TestClass]
    public class SockaddrStorageTests
    {
        [TestMethod]
        public void SockaddrStorage4IsCorrectLength()
        {
            Assert.AreEqual(128, SockaddrStorage4.Size);
        }

        [TestMethod]
        public void SockaddrStorage6IsCorrectLength()
        {
            Assert.AreEqual(128, SockaddrStorage6.Size);
        }

        [TestMethod]
        public void SockaddrStorage4Ordering()
        {
            SockaddrStorage4 ss4 = new();
            ss4.Family = (short)Kyanha.Net.Sockets.SourceMulticast.Internal.AddressFamily.InterNetwork;
            Assert.AreEqual((short)Kyanha.Net.Sockets.SourceMulticast.Internal.AddressFamily.InterNetwork, ss4.Family) ;
            ss4.Port = 0x1234;
            Assert.AreEqual(0x1234,ss4.Port);
            ss4.AddressData = 0xf3f719a0;
            Assert.AreEqual(0xf3f719a0,ss4.AddressData);
        }

        [TestMethod]
        public void SockaddrStorage6Ordering()
        {
            SockaddrStorage6 sockaddrStorage6 = new();
            sockaddrStorage6.Family = (short)Kyanha.Net.Sockets.SourceMulticast.Internal.AddressFamily.InterNetworkV6;
            Assert.AreEqual((short)Kyanha.Net.Sockets.SourceMulticast.Internal.AddressFamily.InterNetworkV6, sockaddrStorage6.Family);
            sockaddrStorage6.FlowInformation = 0x12345678;
            Assert.AreEqual((uint)0x12345678, sockaddrStorage6.FlowInformation);
            sockaddrStorage6.Port = 0x6543;
            Assert.AreEqual(0x6543, sockaddrStorage6.Port);
            byte[] testarray = new byte[16];
            for(int i = 0; i < testarray.Length; i++)
            {
                testarray[i] = (byte)(60 + i);
            }
            sockaddrStorage6.AddressData = testarray;
            Assert.AreEqual(testarray[0], (byte)60);
            byte[] test2array = sockaddrStorage6.AddressData;
            Assert.AreNotSame(testarray, test2array);
            CollectionAssert.AreEqual(testarray, test2array);
        }
    }
}
