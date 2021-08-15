using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Net;
using Kyanha.Net.Sockets.SourceMulticast.Internal;
using Kyanha.Net.Sockets.SourceMulticast;
using Kyanha.Net.Sockets.SourceMulticast.Internal.FinalStateBased;

namespace SourceMulticast.Tests
{

    public class FinalStateBasedTests
    {
        [TestMethod]
        public void PassAListInToListenToIPv6()
        {
            IList<IPAddress> Addresses = new List<IPAddress>
            {
                IPAddress.IPv6Loopback,
                IPAddress.IPv6Any,
            };
            IoctlDispatch.PrepareIoctlStructureFromList(Addresses);
            throw new NotImplementedException();
        }

        [TestMethod]
        public void PassAListInToListenToIPv4()
        {
            IList<IPAddress> Addresses = new List<IPAddress> 
            {
                IPAddress.Loopback,
                IPAddress.Any,
            };
            IoctlDispatch.PrepareIoctlStructureFromList(Addresses);
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void PassAListInToListenToIPvMixed()
        {
            IList<IPAddress> Addresses = new List<IPAddress>
            { 
                IPAddress.Loopback, 
                IPAddress.IPv6Loopback, 
                IPAddress.Any,
                IPAddress.IPv6Any,
            };
            IoctlDispatch.PrepareIoctlStructureFromList(Addresses);
            throw new NotImplementedException();
        }
    }
}
