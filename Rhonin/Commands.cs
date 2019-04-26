using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Rhonin.RNG;

/*
 * Should potentially move all regex functionality and error checking to it's own method / class
 * 
 * Ask Gahder for help cleaning up the regex.
 */

/*
 * Move error strings into enum.
 */

namespace Rhonin
{
    public class MyCommands
    {
        static SimpleDiceRoller DiceRoller = new SimpleDiceRoller();

        [Command("Testing"), Aliases("test","Test","testing","TEST","TESTING")]
        public async Task Testing(CommandContext ctx)
        {
            await ctx.RespondAsync($"Reading you loud and clear {ctx.User.Mention}!");
        }

        [Command("roll"), Aliases("Roll", "ROLL", "R", "r")]
        public async Task Roll(CommandContext ctx, string command)
        {
            await ctx.TriggerTypingAsync();//send client typing to discord.

            int currentRoll = 0;
            int numberOfDice = 0;
            int dieSize = 0;
            int totalRoll = 0;

            string inputString = ctx.RawArgumentString;
            List<int> rolls = new List<int>();

            inputString = Regex.Replace(inputString, @"[A-CE-Za-ce-z\W]", "");//sanitizes formatting
            Match regexMatch = Regex.Match(inputString, @"(\d+[Dd]+\d+)");//checks formatting

            if (!regexMatch.Success)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}: "+
                    "Please use correct formatting: ##d##. Examples, 1d20, 2d100, 12d8");
                return;
            }

            regexMatch = Regex.Match(inputString, @"(\d+)[Dd]+(\d+)");
            numberOfDice = Convert.ToInt32(regexMatch.Groups[1].Value);
            dieSize = Convert.ToInt32(regexMatch.Groups[2].Value);

            currentRoll = DiceRoller.Roll(dieSize);
            if (currentRoll == -1)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Please enter a valid die size!"+
                    $"Valid sizes are D2 through D{DiceRoller.GetMax()}.");
                return;
            }

            rolls.Add(currentRoll);
            totalRoll += currentRoll;

            for (int i = 1; i < numberOfDice; i++)
            {
                currentRoll = DiceRoller.Roll(dieSize);
                rolls.Add(currentRoll);
                totalRoll += currentRoll;
            }

            string outputString = new string($"{ctx.User.Username} Rolled {numberOfDice}D{dieSize} :["+
                $"{string.Join(", ", rolls)}] = {totalRoll}");

            if (outputString.Length <= 1990)
            {
                await ctx.RespondAsync(outputString);
            }
            else
            {
                string stringBuffer = null;
                for (int i = 0; i < outputString.Length; i++)
                {
                    stringBuffer += outputString[i];
                    if((i % 1900 == 0) && i > 0)
                    {
                        await ctx.RespondAsync(stringBuffer);
                        stringBuffer = null;
                    }
                }
                if(stringBuffer.Length > 0)
                {
                    await ctx.RespondAsync(stringBuffer);
                }
            }
        }
    }
}
