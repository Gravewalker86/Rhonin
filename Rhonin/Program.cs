using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;


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

            commands.RegisterCommands<MyCommands>();//registers command object MyCommands
        }

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();//starts async task for maintaining the bot.
        }

        static async Task MainAsync(string[] args)//Main Program Loop
        {
            BotInitialization(); //Boilerplate.
            await discord.ConnectAsync();
            await Task.Delay(-1);//Keeps bot alive.
        }

    }
}
