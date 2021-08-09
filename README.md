# Kyanha.Net.Sockets.Extensions

Copyright (c) 2021 Kyle A Hamilton. Licensed under the MIT license.

## What this is

This aims to be a useful library of extension methods for `System.Net.Sockets.Socket`.  As of right now, there's only one extension class.

Currently, these methods are supported only on Windows. There's nothing preventing support for Unix, except that the `setsockopt` arguments are completely different, and the build system to also support Unix likewise has to be completely different.

### Kyanha.Net.Sockets.SourceMulticast

RFC 8815 deprecated Any Source Multicast (ASM) for inter-domain communication, in favor of Source Specific Multicast (SSM). While Windows supports SSM, the base class libraries of .Net don't (currently, as of .Net 5.0). This issue is tracked at [Dotnet Runtime issue 36170](https://github.com/dotnet/runtime/issues/36170).

Windows has a "Protocol-Independent Multicast" architecture, which makes it possible for these `setsockopt` calls to work on both IPv4 and IPv6 addresses and sockets, as long as all Source and Group addresses provided are the same address family as the `Socket` they're issued on.

(Windows also has an "end-state multicast" configuration system which uses `ioctl` instead of `setsockopt`, but this library does not implement this interface at this time. This means that you will need to provide your Sources one at a time.)

So, here's my humble contribution to make the world a slightly better place.

#### Note the following aspects of SSM:

1. Each and every one of these calls impacts not only the system, but also the network. On IPv4, the system uses IGMPv3 (Internet Group Management Protocol version 3) to tell the router that you want to listen for a particular Group from a particular Source, and it propagates those calls outward. On IPv6, the support for this is built into the protocol itself under the moniker MLDv2 (Multicast Listener Discovery version 2), and it propagates those calls outward as well. Be kind with this shared resource, and tell the system when you want to leave the multicast groups you're listening for. (If other Sockets have joined the same Group, the system will keep track of that and not leave until all of them are closed. So, clean up after yourself.)

2. There are 4 operations you can ask the system (and the network) to do for you.
    1. You can state that you want to temporarily block a particular Source on a Group.
    2. You can state that you want to listen to all packets sent to the Group by a particular Source.
    3. You can state that you want to stop listening to packets addressed to the Group from that Source anymore (and free up the system and network resources used by that request).
    4. You can state that you want to unblock a particular Source that's previously been blocked.

3. A multicast group can have multiple Sources. With these calls, what you're doing is telling the system (and the network) what Sources you're actually interested in (and which ones you're not). This impacts packet routing for multicast packets from those sources.

4. Once you join a group, you will receive all packets sent to that group that your network receives, whether or not they're from the senders that you specifically requested. If another process on your system or on your network requests the same Group with different Sources, you will receive packets from those Sources as well. Your request to join a Group with a particular Source does *not* tell the system to prevent you from receiving packets from other Sources, or firewall you only to those Sources you've whitelisted.

5. If you are receiving packets from Sources that you did not request, you can try to block the offending Source. (This author has not yet determined if this causes the system to firewall those packets away from your Socket, while still allowing other Sockets to receive them.)

6. You cannot use TCP with multicast, because it would effectively DDOS the Sources. UDP only, please.

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
        public SocketAddress Source { get; set; }
        public SocketAddress Group { get; set; }
    }
```

This is basically a managed wrapper around the native `GROUP_SOURCE_REQ` structure, which is used for all of the Source Specific Multicast `setsockopt` operations.

`InterfaceIndex`: This is the index of the interface to operate on, based on the interface numbering for the protocol you're using. To get this, you need to use the `System.Net.NetworkInformation` package, then iterate through the IEnumerable returned by `NetworkInterface.GetAllNetworkInterfaces()` until you find the one you want.  (This is the interface that you will be receiving multicast packets on.) Ideally, you'd check each one for e.g.
```cs
    netif.Supports(NetworkInterfaceComponent.IP)
```
or
```cs
    netif.Supports(NetworkInterfaceComponent.IPv6)
```
and only then get its index with
```cs
    netif.GetIPProperties().GetIPv4Properties().Index
```
or
```cs
    netif.GetIPProperties().GetIPv6Properties().Index
```
and add it to the list of interfaces you're going to be listening on. Interface numbers are different between the different protocols.

`Source`: This is the remote IP that you're working with, the source of the multicast packets that you're telling the system to deal with.

`Group`: This is the multicast group that you want to work with.

If you don't want to instantiate this struct, you can provide each of these parameters to the implementing functions directly.



##### Extension Methods:
```cs
public static void BlockMulticastGroupFromSource(this Socket sock,
    SocketAddress Group, SocketAddress ToBlock, ulong Interface)
public static void BlockMulticastGroupFromSource(this Socket sock,
    MulticastGroupSourceOption mgso)
```
These methods issue the setsockopt `MCAST_BLOCK_SOURCE`.

```cs
public static void JoinMulticastGroupWithSource(this Socket sock,
    SocketAddress Group, SocketAddress ToListenFrom, ulong Interface)
public static void JoinMulticastGroupWithSource(this Socket sock,
    MulticastGroupSourceOption mgso)
```
These methods issue the setsockopt `MCAST_JOIN_SOURCE_GROUP`.

```cs
public static void LeaveMulticastGroupWithSource(this Socket sock, SocketAddress Group,
    SocketAddress ToStopListeningFrom, ulong Interface)
public static void LeaveMulticastGroupWithSource(this Socket sock,
    MulticastGroupSourceOption mgso)
```
These methods issue the setsockopt `MCAST_LEAVE_SOURCE_GROUP`.

```cs
public static void UnblockMulticastGroupFromSource(this Socket sock,
    SocketAddress Group, SocketAddress ToUnblock, ulong Interface)
public static void UnblockMulticastGroupFromSource(this Socket sock,
    MulticastGroupSourceOption mgso)
```
These methods issue the setsockopt `MCAST_UNBLOCK_SOURCE`.

### Final Notes:

Multicast technology has been sadly underutilized. As of now, the main users of it have been cable companies multicasting their television lineups. There's no reason that you can't use the same technology for your own purposes, and there's no reason that the only use must be media streams. Use your imagination to find ways you can leverage single-packet-send/multi-recipient-delivery in your applications -- you might be surprised at what you figure out you can do.

I hope this helps!

-Kyle Hamilton
August 8, 2021
