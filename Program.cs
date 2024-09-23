﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ogybot.Builders;
using ogybot.DataAccess.Sockets;

namespace ogybot;

public static class Program
{
    public static async Task Main()
    {
        var services = ServiceBuilder.Build();
        var config = services.GetRequiredService<IConfiguration>();
        var webSocketServer = services.GetRequiredService<ChatServer>();
        var discordClient = services.GetRequiredService<DiscordSocketClient>();
        var interactionService = services.GetRequiredService<InteractionService>();

        await webSocketServer.StartServerAsync();
        discordClient.SetupInteraction(config, services, interactionService);

        // Delay the task until program is closed
        await Task.Delay(-1);
    }
}