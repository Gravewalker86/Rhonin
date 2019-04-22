using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace Rhonin
{
    class Program
    {
        static DiscordClient discord;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();//Async for maintaing server connection.
        }

        static async Task MainAsync(string[] args)//Main Program Loop
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = "KEY_GOES_HERE",//DiscordBot Authentication Token goes in the string.
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}