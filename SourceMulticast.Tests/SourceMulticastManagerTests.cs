using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Sockets;
using Kyanha.Net.Sockets.SourceMulticast;
using Kyanha.Net.Sockets.SourceMulticast.Internal;

namespace SourceMulticast.Tests
{
    [TestClass]
    public class SourceMulticastManagerTests
    {
        Socket TestSocket6;
        IPAddress SMMTestIPAddress6 = IPAddress.Parse("::1");
        Socket TestSocket4;
        IPAddress SMMTestIPAddress4 = IPAddress.Parse("127.0.0.1");

        int portToBind = 32765;

        IPAddress SMMTestMCastAddr6 = IPAddress.Parse("ff3e::ffff:eeee");
        IPAddress SMMTestMCastAddr4 = IPAddress.Parse("232.2.3.4");



        [TestInitialize]
        public void InitializeSMMTests()
        {
            TestSocket6 = new Socket(SocketType.Dgram, ProtocolType.Udp);
            TestSocket6.Bind(new IPEndPoint(SMMTestIPAddress6, portToBind));

            TestSocket4 = new Socket(SocketType.Dgram, ProtocolType.Udp);
            TestSocket4.Bind(new IPEndPoint(SMMTestIPAddress4, portToBind));
        }

        [TestCleanup]
        public void CleanupSMMTests()
        {
            TestSocket6.Close();
            TestSocket4.Close();
        }

        [TestMethod]
        public void InstantiateASMMOnANewSocket6()
        {
            IPAddress ip = IPAddress.Parse("::1");
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            SourceMulticastManager smm = new SourceMulticastManager(socket);
            MulticastGroupSourceOption mgso = new MulticastGroupSourceOption(SMMTestIPAddress6, SMMTestMCastAddr6);
            smm.AddInterfaceIndex(0);
            smm.JoinSourceGroup(mgso);
            smm.LeaveSourceGroup(mgso);
            smm.Socket.Close();
        }

        [TestMethod]
        public void InstantiateASMMOnANewSocket4()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            SourceMulticastManager smm = new SourceMulticastManager(socket);
            MulticastGroupSourceOption mgso = new MulticastGroupSourceOption(SMMTestIPAddress4, SMMTestMCastAddr4);
            smm.AddInterfaceIndex(0);
            smm.JoinSourceGroup(mgso);
            smm.LeaveSourceGroup(mgso);
            smm.Socket.Close();
        }

    }
}
