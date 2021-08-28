using System;

namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    /// <summary>
    /// This is copied directly from System.Net.Sockets:AddressFamily.cs (via github:dotnet/runtime)
    /// but fit into an Int16 to avoid casts from that version's extended values beyond Winsock.
    /// </summary>
    public enum AddressFamily : Int16
    {
        Unknown = -1,           // Unknown
        Unspecified = 0,        // Unspecified
        Unix = 1,               // Local to host (pipes, portals)
        InterNetwork = 2,       // Internetwork: UDP, TCP, etc.
        ImpLink = 3,            // ARPAnet imp addresses
        Pup = 4,                // pup protocols: e.g. BSP
        Chaos = 5,              // MIT CHAOS protocols
        NS = 6,                 // XEROX NS protocols
        Ipx = NS,               // IPX and SPX
        Iso = 7,                // ISO protocols
        Osi = Iso,              // OSI is ISO
        Ecma = 8,               // European Computer Manufacturers
        DataKit = 9,            // DataKit protocols
        Ccitt = 10,             // CCITT protocols, X.25 etc
        Sna = 11,               // IBM SNA
        DecNet = 12,            // DECnet
        DataLink = 13,          // Direct data link interface
        Lat = 14,               // LAT
        HyperChannel = 15,      // NSC Hyperchannel
        AppleTalk = 16,         // AppleTalk
        NetBios = 17,           // NetBios-style addresses
        VoiceView = 18,         // VoiceView
        FireFox = 19,           // FireFox
        Banyan = 21,            // Banyan
        Atm = 22,               // Native ATM Services
        InterNetworkV6 = 23,    // Internetwork Version 6
        Cluster = 24,           // Microsoft Wolfpack
        Ieee12844 = 25,         // IEEE 1284.4 WG AF
        Irda = 26,              // IrDA
        NetworkDesigners = 28,  // Network Designers OSI & gateway enabled protocols
        Max = 29,               // Max
    }
}
