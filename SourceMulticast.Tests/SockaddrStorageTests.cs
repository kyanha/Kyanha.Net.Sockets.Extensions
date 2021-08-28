using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Sockets;
using Kyanha.Net.Sockets.SourceMulticast.Internal;

namespace SourceMulticast.Tests
{
    [TestClass]
    public class SockaddrStorageTests
    {
        [TestCategory("SockaddrStorage4 length")]
        [TestMethod]
        public void SockaddrStorage4IsCorrectLength()
        {
            Assert.AreEqual(128, SockaddrStorage4.Size);
        }

        [TestCategory("SockaddrStorage6 length")]
        [TestMethod]
        public void SockaddrStorage6IsCorrectLength()
        {
            Assert.AreEqual(128, SockaddrStorage6.Size);
        }

        [TestCategory("SockaddrStorage length")]
        [TestMethod]
        public void SockaddrStorageIsCorrectLength()
        {
            Assert.AreEqual(128, SockaddrStorage.Size);
        }

        [TestCategory("SockaddrStorage4")]
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

        [TestCategory("SockaddrStorage6")]
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

        [TestCategory("SockaddrStorage6")]
        [TestMethod]
        public void SockaddrStorage6ToIPAddressAndBack()
        {
            IPAddress ip = IPAddress.Parse("123:4567:89ab:cdef:3210:7654:ba98:fedc");
            SockaddrStorage6 sas6 = ip;
            IPAddress ip2 = sas6;
            Assert.AreEqual<IPAddress>(ip, ip2);
        }

        [TestCategory("SockaddrStorage4")]
        [TestMethod]
        public void SockaddrStorage4ToIPAddressAndBack()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            SockaddrStorage4 sas4 = ip;
            IPAddress ip2 = sas4;
            Assert.AreEqual<IPAddress>(ip, ip2);
        }

        [TestCategory("SockaddrStorage6")]
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void SockaddrStorage6RejectsIPv4Address()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            SockaddrStorage6 sas6 = ip; // should throw
            IPAddress ip2 = sas6;
            Assert.AreEqual<IPAddress>(ip, ip2);
        }

        [TestCategory("SockaddrStorage4")]
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void SockaddrStorage4RejectsIPv6Address()
        {
            IPAddress ip = IPAddress.Parse("::1");
            SockaddrStorage4 sas4 = ip; // should throw
            IPAddress ip2 = sas4;
            Assert.AreEqual<IPAddress>(ip, ip2);
        }

        [TestCategory("IPAddress behavior verification")]
        [TestMethod]
        public void IPAddressDataV6WorksLikeIExpect()
        {
            IPAddress ip = IPAddress.Parse("::1");
            byte[] addrbytes = ip.GetAddressBytes();
            IPAddress ip2 = new IPAddress(addrbytes);
            Assert.AreEqual<IPAddress>(ip, ip2);
        }

        [TestCategory("IPAddress behavior verification")]
        [TestMethod]
        public void IPAddressDataV4WorksLikeIExpect()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            byte[] addrbytes = ip.GetAddressBytes();
            IPAddress ip2 = new IPAddress(addrbytes);
            Assert.AreEqual<IPAddress>(ip, ip2);
        }

        [TestCategory("SockaddrStorage")]
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void SaS6CannotGoToSaS4()
        {
            IPAddress ip = IPAddress.Parse("::1");
            SockaddrStorage6 sas6 = ip;
            SockaddrStorage sas = sas6;
            SockaddrStorage4 sas4 = sas;
            Assert.AreEqual(sas6, sas4);
        }

        [TestCategory("SockaddrStorage")]
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void SaS4CannotGoToSaS6()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            SockaddrStorage4 sas4 = ip;
            SockaddrStorage sas = sas4;
            SockaddrStorage6 sas6 = sas;
            Assert.AreEqual(sas4, sas6);
        }
    }
}
