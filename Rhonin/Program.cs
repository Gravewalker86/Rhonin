using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Rhonin.RNG;

namespace Rhonin
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;


        static string Authentication()//Method could be eliminated through the use of JSON config file.
        {
            string authenticationKey = null;
            string keyName = "key.bin";//key stored in local file.

            if (!File.Exists(keyName))
            {
                Console.WriteLine("Authentication Token not found, enter authentication token: ");
                authenticationKey = Console.ReadLine();
                using (StreamWriter outFile = new StreamWriter(keyName))
                {
                    outFile.WriteLine(authenticationKey);
                }
                Console.WriteLine("Authentication Token written to file");
            }

            else
            {
                using (StreamReader inFile = new StreamReader(keyName))
                {
                    authenticationKey = inFile.ReadLine();
                }
            }

            return authenticationKey;
        }

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();//Async for maintaing server connection.
        }

        static async Task MainAsync(string[] args)//Main Program Loop
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Authentication(),
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = ";;"
            });

            commands.RegisterCommands<MyCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }

    public class MyCommands
    {
        [Command("Testing")]
        public async Task Testing(CommandContext ctx)
        {
            await ctx.RespondAsync($"Reading you loud and clear {ctx.User.Mention}!");
        }

        [Command("roll"), Aliases("Roll", "ROLL")]
        public async Task Roll(CommandContext ctx, int numDie, int dieSize)
        {
            var rnd = new Random();
            List<int> diceRolled = new List<int>();
            int totalRolled = 0;

            for (int i = 0; i < numDie; i++)
            {
                diceRolled.Add(rnd.Next(1, dieSize));
            }

            foreach (int roll in diceRolled)
            {
                totalRolled += roll;
            }
            await ctx.RespondAsync($"{ctx.User.Mention} rolled: [{string.Join(", ", diceRolled)}] = {totalRolled}");
        }

    }
}
