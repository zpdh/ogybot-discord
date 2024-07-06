using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Loggers;

namespace test.Login
{
    public class Client
    {
        //private string _token = "MTI1MjQ2MzAyODAyNTQyNjAzMQ.GgbYj2.ItzY4IqZxVmZ2ol3BLoPZkuhifWlj-S1u9oR0k";
        // ^^^^^ ogybot token

        private string _token = "MTI1NTUwNDYyNTQyMzg3NjE3OA.G4Jg5o.K7e54OtKLTmGcKEa0E7mt4VZz5N36Cel42cSQw";
        // ^^^^^ testing bot token

        public DiscordSocketClient client {  get; set; }

        public Client(DiscordSocketClient client2)
        {
            client = client2;
        }

        public async Task Login()
        {
            await client.LoginAsync(TokenType.Bot, _token);
            await client.StartAsync();
        }

        public void Log()
        {
            client.Log += LoggerService.Log;
        }

        public DiscordSocketClient GetClient()
        {
            return client;
        }
    }
}
