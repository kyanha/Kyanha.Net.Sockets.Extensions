# Kyanha.Net.Sockets.Extensions

Copyright (c) 2021 Kyle A Hamilton. Licensed under the MIT license.

## What this is

This aims to be a useful library of extension methods for `System.Net.Sockets.Socket`.  As of right now, it contains only one extension class.

Currently, these methods are supported only on Windows. There's nothing preventing support for Unix, except that the `setsockopt` arguments are completely different, and the build system to also support Unix likewise has to be completely different.

### Kyanha.Net.Sockets.SourceMulticast

RFC 8815 deprecated Any Source Multicast (ASM) for inter-domain communication, in favor of Source Specific Multicast (SSM). While Windows supports SSM, the base class libraries of .Net don't (currently, as of .Net 5.0). This issue is tracked at [Dotnet Runtime issue 36170](https://github.com/dotnet/runtime/issues/36170).

The SSM specification, RFC 4607, has section 4.3 which states that there should exist an API to allocate SSM group addresses. Unfortunately, the author has not been able to find any such API on Windows or on Linux. (This includes the suggested API from RFC 2771.) If you have multiple multicast-sending apps or services on your machine and want to have reasonable certainty that you won't accidentally collide with another app's choice of multicast group, you should generate 24 bits (for IPv4) or 31 bits (for IPv6) of randomness, and use that in the appropriate variable range of the SSM multicast group address range.

Windows has a "Protocol-Independent Multicast" architecture, which makes it possible for these `setsockopt` calls to work on both IPv4 and IPv6 addresses and sockets, as long as all Source and Group addresses provided are the same address family as the `Socket` they're issued on. Theoretically, this wouldn't be limited to IPv4 and IPv6, but those are the only two address families that the rest of the .Net BCL supports at this time.

Windows also has a "final-state-based multicast" configuration system which uses `WSAioctl` instead of `setsockopt` to specify the multicast sources interested in, but this library does not implement this interface at this time. This means that you will need to provide your Sources one at a time.

So, here's my humble contribution to make the world a slightly better place.

#### Note the following aspects of SSM:

1. Multicast addresses are within specific ranges (IPv4) or formed with specific bit patterns (IPv6). Explaining these is beyond the scope of this document. It is assumed that you know what you're doing. (If you don't, see RFC 1112, RFC 4604, and RFC 5771 for IPv4; see RFC 3513, RFC 4291, and RFC 7371 for IPv6.  Also see RFC 4607 section 1 for a summary.) Suffice it to say that for IPv4, your SSM group must fall within 232.0.0.0/8 (except not within 232.0.0.x/24), and for IPv6 your dynamically-generated group address must be within FF3x\:\:8000\:0000/97. Your statically-assigned group address must be within FF3x\:\:/98. (FF3x\:\:4000\:0000/98 is reserved for allocation by IANA.)

2. **_All of these calls impact not only the system, but also the network._** On IPv4, the system uses IGMPv3 (Internet Group Management Protocol version 3) to tell the router that you want to listen for a particular Group from a particular Source, and the router propagates those calls outward. On IPv6, the support for this is built into the protocol itself under the moniker MLDv2 (Multicast Listener Discovery version 2), and it behaves the same way as IGMPv3. **Be kind with this shared resource**, and tell the system when you want to leave the multicast groups you're listening for. (If other Sockets have joined the same Group, the system will keep track of that and not leave until all of them are closed. So, it won't hurt the system or other processes to clean up after yourself.)

3. There are 2 operations you can ask the system (and the network) to do for you.
    1. You can state that you want to join (that is, listen to all packets sent to) the Group, and nominate a particular Source which might be sending them.
    2. You can state that you want to leave (that is, stop listening to packets addressed to) the Group from that Source anymore (and free up the system and network resources used by that route).

4. An Any Source Multicast group can have multiple Sources, and is identified solely by { group }. Source Specific Multicast binds a channel identified by the tuple {sender, group}. With these calls, you tell the system (and the network) what Sources you are actually interested in (and, potentially, which ones you're not). This impacts packet propagation for multicast packets from those sources.

5. Your Socket can listen for packets from multiple { source, group } tuples simultaneously.

6. Once you join a source group, even though RFC 4607 suggests otherwise, you might receive all packets sent to that group that your network receives, whether or not they're from the sources that you specifically requested. If another process on your system or on your network requests the same Group with different Sources, you might receive packets from those Sources as well. You should be prepared for the possibility that your request to join a Group with a particular Source might *not* tell the system to prevent you from receiving packets from other Sources, or limit you only to those Sources you've whitelisted.  (While this may not be the case on Windows, it may be the case on any given Unix that dotnet runs on, and it's best to be prepared for the possibility.)

7. TCP cannot be used with multicast, because it would effectively DDOS the Sources when the receivers sent the TCP acknowledgment packets. UDP only, please.

8. All of these extension methods need the Index of the interface you're working with. See the notes in the Structures section about how to find this.

#### Use:
To use it, include the `Kyanha.Net.Sockets.SourceMulticast` assembly, then put
```cs
using static Kyanha.Net.Sockets.SourceMulticast.SocketExtensions;
```
at the top of your C# source file which needs to use them.

##### Structures:

```cs
    public class MulticastGroupSourceOption
    {
        public ulong InterfaceIndex { get; set; }
        public IPAddress Source { get; set; }
        public IPAddress Group { get; set; }
    }
```

This is a managed wrapper around the native `GROUP_SOURCE_REQ` structure, which is used for all of the Source Specific Multicast `setsockopt` operations.

`InterfaceIndex`: This is the index of the interface to operate on, based on the interface numbering for the protocol you're using. To get this, you need to use the `System.Net.NetworkInformation` assembly, then iterate through the IEnumerable returned by `NetworkInterface.GetAllNetworkInterfaces()` until you find the one you want.  (This is the interface that you will be receiving multicast packets on.) Ideally, you'd then check each one for e.g.
```cs
    netif.Supports(NetworkInterfaceComponent.IP)
```
or
```cs
    netif.Supports(NetworkInterfaceComponent.IPv6)
```
and only if they return true for an interface then get its index with
```cs
    netif.GetIPProperties().GetIPv4Properties().Index
```
or
```cs
    netif.GetIPProperties().GetIPv6Properties().Index
```
and add it to the list of interfaces you're going to be listening on. It's best to act as though interface numbers are different between the different protocols (they often are on Windows, even if they're typically not on Unix.)

`Source`: This is the source of the multicast packets that you're telling the system you want to deal with, whether you want to listen or stop listening to it.

`Group`: This is the multicast group that you want to join, leave, or block or unblock particular Sources on.

If you don't want to instantiate this struct, you can provide each of these parameters to the implementing functions directly.

##### Extension Methods:

```cs
public static void JoinMulticastGroupWithSource(this Socket sock,
    IPAddress Group, IPAddress ToListenFrom, ulong Interface)
public static void JoinMulticastGroupWithSource(this Socket sock,
    MulticastGroupSourceOption mgso)
```
These methods issue the setsockopt `MCAST_JOIN_SOURCE_GROUP`. As stated, this tells the system to tell the network that you want to listen to the multicast `Group` from source `ToListenFrom`.

```cs
public static void LeaveMulticastGroupWithSource(this Socket sock, IPAddress Group,
    IPAddress ToStopListeningFrom, ulong Interface)
public static void LeaveMulticastGroupWithSource(this Socket sock,
    MulticastGroupSourceOption mgso)
```
These methods issue the setsockopt `MCAST_LEAVE_SOURCE_GROUP`. This tells the system to tell the network that you aren't interested in packets sent to `Group` from `ToStopListeningFrom` anymore. 

### Final Notes:

Multicast technology has been sadly underutilized. As of now, the main users of it have been cable companies multicasting their television lineups. There's no reason that you can't use the same technology for your own purposes, though, and there's no reason that the only use must be media streams. Use your imagination to find ways you can leverage single-packet-send/multi-recipient-delivery in your applications -- you might be surprised at what you figure out you can do.

I hope this helps!

-Kyle Hamilton
August 8, 2021
