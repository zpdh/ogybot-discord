namespace ogybot.Domain.Entities.Configurations;

public class ServerConfiguration
{

    public Guid WynnGuildId { get; set; }
    public ulong DiscordGuildId { get; set; }
    public ulong TomeChannel { get; set; }
    public ulong LayoffsChannel { get; set; }
    public ulong RaidsChannel { get; set; }
    public ulong WarChannel { get; set; }
    public ulong ListeningChannel { get; set; }
    public ulong BroadcastingChannel { get; set; }
    public ICollection<ulong> PrivilegedRoles { get; set; } = [];

    public ServerConfiguration(
        Guid wynnGuildId,
        ulong discordGuildId,
        ulong tomeChannel,
        ulong layoffsChannel,
        ulong raidsChannel,
        ulong warChannel,
        ulong listeningChannel,
        ulong broadcastingChannel,
        ICollection<ulong> privilegedRoles)
    {
        WynnGuildId = wynnGuildId;
        DiscordGuildId = discordGuildId;
        TomeChannel = tomeChannel;
        LayoffsChannel = layoffsChannel;
        RaidsChannel = raidsChannel;
        WarChannel = warChannel;
        PrivilegedRoles = privilegedRoles;
        ListeningChannel = listeningChannel;
        BroadcastingChannel = broadcastingChannel;
    }

    private ServerConfiguration()
    {

    }
}