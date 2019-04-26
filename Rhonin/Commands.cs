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
            await ctx.TriggerTypingAsync();//send client typing to discord.
            const string SANITIZE = @"[A-CE-Za-ce-z\W]";
            const string VALIDATION = @"(d+[Dd]\d+";
            const string PARSE = @"(?<NUMDICE>\d+)[Dd](?<DIESIZE>\d+)(?<MODIFIER>[+-]\d)*";

            int currentRoll = 0;
            int numberOfDice = 0;
            int dieSize = 0;
            int totalRoll = 0;
            int mod = 0;
            List<int> rolls = new List<int>();

            string inputString = Regex.Replace(ctx.RawArgumentString, @"[A-CE-Za-ce-z\W]", "");
            //declared and sanitized in one line.
            Match regexMatch = Regex.Match(inputString, @"(\d+[Dd]\d+)");//checks formatting

            if (!regexMatch.Success)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}: " +
                    "Please use correct formatting: ##d##. Examples, 1d20, 2d100, 12d8");
                return;
            }

            Regex regex = new Regex(@"(?<NUMDICE>\d+)[Dd](?<DIESIZE>\d+)(?<MODIFIER>[+-]\d)*");
            MatchCollection matches = regex.Matches(inputString);

            foreach(Match match in matches)
            {

            }

        }

        [Command("roll"), Aliases("Roll", "ROLL", "R", "r")]
        public async Task Roll(CommandContext ctx, string command)
        {
            await ctx.TriggerTypingAsync();//send client typing to discord.

            int currentRoll = 0;
            int numberOfDice = 0;
            int dieSize = 0;
            int totalRoll = 0;
            int mod = 0;
            List<int> rolls = new List<int>();

            string inputString = Regex.Replace(ctx.RawArgumentString, @"[A-CE-Za-ce-z\W]", "");
            //declared and sanitized in one line.
            Match regexMatch = Regex.Match(inputString, @"(\d+[Dd]\d+)");//checks formatting

            if (!regexMatch.Success)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}: " +
                    "Please use correct formatting: ##d##. Examples, 1d20, 2d100, 12d8");
                return;
            }

            Regex regex = new Regex(@"(\d+)[Dd](\d+)([+-]\d)*");
            MatchCollection matches = regex.Matches(inputString);

            // 15d4+10 2d8 
            //Match regexMatch = Regex.Match(inputString, @"(\d+[Dd]+\d+\+*\d*)");

            //this line should be unnessecary as you just did a match on line 47
            //regexMatch = Regex.Match(inputString, @"(\d+)[Dd]+(\d+)");
            numberOfDice = Convert.ToInt32(regexMatch.Groups[1].Value);
            dieSize = Convert.ToInt32(regexMatch.Groups[2].Value);

            if (inputString.Contains("+"))
            {
                totalRoll = Convert.ToInt32(regexMatch.Groups[3].Value);
                mod = totalRoll;
            }
 


            //if (dieSize < 2 || dieSize > DiceRoller.GetMax())
            //{
            //    await ctx.RespondAsync($"{ctx.User.Mention}, Please enter a valid die size!" +
            //        $"Valid sizes are D2 through D{DiceRoller.GetMax()}.");
            //    return;
            //}

            //using the above if block will allow you to not need to call DiceRoller.Roll twice, and begin the loop at 0
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

            //modified the output string to contain the modifier text
            string outputString = new string($"{ctx.User.Username} Rolled {numberOfDice}D{dieSize} :["+
                $"{string.Join(", ", rolls)}] + " + mod + "= {totalRoll}");

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
