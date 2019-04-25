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

        //static public Rhonin.RNG.LookupDiceRoller DiceRoller;


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

        static void BotInitialization()//Used to store DSharpPlus Boilerplate.
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
                StringPrefix = ";;",
                CaseSensitive = false,
                EnableDms = false,
                EnableDefaultHelp = false
            });

            commands.RegisterCommands<MyCommands>();
        }

        static void RhoninInitialization()
        {
            //DiceRoller = new LookupDiceRoller();
        }



        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();//Async for maintaing server connection.
        }

        static async Task MainAsync(string[] args)//Main Program Loop
        {
            BotInitialization(); //Boilerplate.
            //RhoninInitialization(); //actual initialization.

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

    }
}
