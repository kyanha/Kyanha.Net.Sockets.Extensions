namespace Kyanha.Net.Sockets.SourceMulticast.Internal
{
    enum SourceMulticastOpcode
    {
        MCAST_JOIN_SOURCE_GROUP = 45,       // Join IP group/source.
        MCAST_LEAVE_SOURCE_GROUP = 46,      // Leave IP group/source.
    }
}
