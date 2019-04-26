using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;

using Rhonin.RNG;
using System.Threading;

/*
 * Should potentially move all regex functionality and error checking to it's own method / class
 * 
 * Ask Gahder for help cleaning up the regex.
 */

namespace Rhonin
{
    public class MyCommands
    {
        LookupDiceRoller DiceRoller = new LookupDiceRoller();

        [Command("Testing")]
        public async Task Testing(CommandContext ctx)
        {
            await ctx.RespondAsync($"Reading you loud and clear {ctx.User.Mention}!");
        }

        //Carbon copy of roll using sorted lists. TEMPORARY SOLUTION FOR IMMEDIATE USE!
        [Command("sortedroll"), Aliases("SortedRoll", "SORTEDROLL", "SR", "sr")]
        public async Task SortedRoll(CommandContext ctx, string command)
        {
            await ctx.TriggerTypingAsync();//send client typing to discord.

            int currentRoll = -1;
            int totalRoll = 0;
            int dieSize = 20;
            int numberOfDice = 1;
            string inputString = ctx.RawArgumentString;
            List<int> rolls = new List<int>();

            inputString = Regex.Replace(inputString, @"[A-CE-Za-ce-z\W]", "");//stripps excess
            Match regexMatch = Regex.Match(inputString, @"(\d+[Dd]\d+)");//checks formatting

            if (!regexMatch.Success)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}: Please use correct formatting: ##d##. Examples, 1d20, 2d100, 12d8");
                return;//Not sure if this is proper usage.
            }

            regexMatch = Regex.Match(inputString, @"(\d+)[Dd](\d+)");
            numberOfDice = Convert.ToInt32(regexMatch.Groups[1].Value);
            dieSize = Convert.ToInt32(regexMatch.Groups[2].Value);

            if (numberOfDice > 500)
            {
                await ctx.RespondAsync($"Let's try to keep it under 500 dice at a time {ctx.User.Mention}");
                return;
            }

            currentRoll = DiceRoller.Roll(dieSize);
            if (currentRoll == -1)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Please enter a valid die size! Valid sizes are: D4, D6, D8, D10, D12, D20, D100 and, D1000");
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


            rolls.Sort();//only difference between this and roll
            rolls.Reverse();//only other difference.
            string outputString = new string($"{ctx.User.Username} Rolled {numberOfDice}D{dieSize} :[{string.Join(", ", rolls)}] = {totalRoll}");

            if (outputString.Length > 1990)
            {
                await ctx.RespondAsync($"Roll less dice! {ctx.User.Mention}");
            }

            await ctx.RespondAsync(outputString);
        }


        [Command("roll"), Aliases("Roll", "ROLL", "R", "r")]
        public async Task Roll(CommandContext ctx, string command)
        {
            await ctx.TriggerTypingAsync();//send client typing to discord.

            int currentRoll = -1;
            int totalRoll = 0;
            int dieSize = 20;
            int numberOfDice = 1;
            string inputString = ctx.RawArgumentString;
            List<int> rolls = new List<int>();

            inputString = Regex.Replace(inputString, @"[A-CE-Za-ce-z\W]", "");//stripps excess
            Match regexMatch = Regex.Match(inputString, @"(\d+[Dd]\d+)");//checks formatting

            if (!regexMatch.Success)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}: Please use correct formatting: ##d##. Examples, 1d20, 2d100, 12d8");
                return;//Not sure if this is proper usage.
            }

            regexMatch = Regex.Match(inputString, @"(\d+)[Dd](\d+)");
            numberOfDice = Convert.ToInt32(regexMatch.Groups[1].Value);
            dieSize = Convert.ToInt32(regexMatch.Groups[2].Value);

            if(numberOfDice > 500)
            {
                await ctx.RespondAsync($"Let's try to keep it under 500 dice at a time {ctx.User.Mention}");
                return;
            }

            currentRoll = DiceRoller.Roll(dieSize);
            if (currentRoll == -1)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Please enter a valid die size! Valid sizes are: D4, D6, D8, D10, D12, D20, D100 and, D1000");
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

            string outputString = new string($"{ctx.User.Username} Rolled {numberOfDice}D{dieSize} :[{string.Join(", ",rolls)}] = {totalRoll}");

            if(outputString.Length > 1990)
            {
                await ctx.RespondAsync($"Roll less dice! {ctx.User.Mention}");
            }

            await ctx.RespondAsync(outputString);
            
            /* Fix segmentation
            
            if(outputString.Length > 1990)
            {
                int i = 0;
                string outputSegment = null;

                for (i = 0; i < outputString.Length / 1990; i++)
                {
                    outputSegment = (outputString.Substring(i * 1990, 1990));
                    await ctx.RespondAsync(outputSegment);
                    Thread.Sleep(1000);
                }

                if ((outputString.Length % 1990) != 0)
                {
                    outputSegment = (outputString.Substring(i));
                    await ctx.RespondAsync(outputSegment);
                    Thread.Sleep(1000);
                }
                
            }
            else
            {
                await ctx.RespondAsync(outputString);
            }
            
             */
        }

        [Command("roll2"), Aliases("Rollz", "ROLLz")]
        public async Task Roll2(CommandContext ctx, int numDie, int dieSize)
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
