<h1 align="center"> ogybot </h1>

<h2 align="center"> About </h2>

OgyBot is a discord bot designed for easy guild management in the Wynncraft MMORPG & providing a real-time chat between the game & discord via WebSockets.
Some of the features included are:
- Automated queue management for guild invitations
- Queue management for guild rewards (tomes, aspects)
- Raid tracking across guild members
- Bidirectional real-time chatting between Discord and the in-game guild

<h2 align="center"> Technologies </h2>

- Domain-Driven Design Architecture
- .NET 8.0
- C#
- Discord.NET
- Docker
- WebSockets
- In-Memory Caching
- Socket.IO

 <a href="https://github.com/ezlixp/ico_server" target="_blank"> API implementation </a>. (Node.js)
 <a href="https://github.com/ezlixp/guild_api" target="_blank"> Minecraft Mod </a>. (Java)

<h2 align="center"> Commands </h2>

- /info - Displays info about the bot, such as this repository and it's authors.
> - /chiefs - Pings guild chiefs.
> - /raid - Pings either Light/Heavy Raid roles.
- /link - Links discord account to minecraft account
- /online - Shows current mod users online in-game.
- /tomelist \_\_\_
  - list - Displays players in the queue for a guild tome.
  - add - Adds a player to the queue.
  - remove - Removes a player(s) from the queue based on their username or index.
- /waitlist \_\_\_
  - list - Displays players in the queue for a guild invite.
  - add - Adds a to queue
  - remove - Removes a player(s) from the queue based on their username or index.
- /raidlist \_\_\_
  - list - Displays the raid information of each guild member (guild raids completed, aspects owed)
  - decrement - Decrements the aspects a user(s) based on their username of index.


<h2 align="center"> Contributions </h2>

Contributions are heavily appreciated, however, before you commit please make sure the code is readable and doesn't
break anything.

<h3> How to contribute </h3>

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/newFeature`)
3. Commit your Changes (`git commit -m 'feat: add newFeature'`)
4. Push to the Branch (`git push origin feature/newFeature`)
5. Open a Pull Request
