using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Rhonin.RNG;

/*
 * Should potentially move all regex functionality and error checking to it's own method / class
 */

namespace Rhonin
{
    public class MyCommands
    {
        [Command("Testing")]
        public async Task Testing(CommandContext ctx)
        {
            await ctx.RespondAsync($"Reading you loud and clear {ctx.User.Mention}!");
        }

        [Command("roll"), Aliases("Roll", "ROLL")]
        public async Task Roll(CommandContext ctx, string command)
        {
            int dieSize = 20;
            int numberOfDice = 1;
            string inputString = ctx.RawArgumentString;
            inputString = Regex.Replace(inputString, @"[A-CE-Za-ce-z\W]", "");//stripps excess
            //await ctx.RespondAsync($"Stripped input string: {inputString}");

            Match regexMatch = Regex.Match(inputString, @"(\d+[Dd]\d+)");//checks formatting
            if (!regexMatch.Success)
            {
                await ctx.RespondAsync($"{ctx.User}: Please use correct formatting: (Number of Dice)d(Size of die). Examples, 1d20, 2d100, 12d8");
            }
            await ctx.RespondAsync(regexMatch.Value);
            /*

            if (!regexMatch.Success)
            {
                await ctx.RespondAsync($"{ctx.User}: Please use correct formatting: (Number of Dice)d(Size of die). Examples, 1d20, 2d100, 12d8");
            }
            else
            {
                regexMatch = Regex.Match(inputString, @"(\d*)d");
                numberOfDice = Convert.ToInt32(regexMatch.Value);
                regexMatch = Regex.Match(inputString, @".*d(\d*)");
                dieSize = Convert.ToInt32(regexMatch);

                await ctx.RespondAsync($"number of dice: {numberOfDice}{Environment.NewLine}die size: {dieSize}");
            }
            
            await ctx.RespondAsync($"{inputString}");
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
