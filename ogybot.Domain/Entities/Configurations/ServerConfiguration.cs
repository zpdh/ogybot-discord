﻿namespace ogybot.Domain.Entities.Configurations;

public class ServerConfiguration
{
    public Guid WynnGuildId { get; set; }
    public ulong DiscordGuildId { get; set; }
    public ulong TomeChannel { get; set; }
    public ulong LayoffsChannel { get; set; }
    public ulong RaidsChannel { get; set; }
    public ulong WarChannel { get; set; }
    public ICollection<ulong> PrivilegedRoles { get; set; } = [];
    public ICollection<ulong> ListeningChannels { get; set; } = [];
    public ICollection<ulong> BroadcastingChannels { get; set; } = [];

    public ServerConfiguration(
        Guid wynnGuildId,
        ulong discordGuildId,
        ulong tomeChannel,
        ulong layoffsChannel,
        ulong raidsChannel,
        ulong warChannel,
        ICollection<ulong> privilegedRoles,
        ICollection<ulong> listeningChannels,
        ICollection<ulong> broadcastingChannels)
    {
        WynnGuildId = wynnGuildId;
        DiscordGuildId = discordGuildId;
        TomeChannel = tomeChannel;
        LayoffsChannel = layoffsChannel;
        RaidsChannel = raidsChannel;
        WarChannel = warChannel;
        PrivilegedRoles = privilegedRoles;
        ListeningChannels = listeningChannels;
        BroadcastingChannels = broadcastingChannels;
    }

    private ServerConfiguration()
    {

    }
}