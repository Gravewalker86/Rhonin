using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Rhonin.RNG;
using Rhonin.CommandLogic;

namespace Rhonin
{
    public class MyCommands
    {

        [Command("roll"), Aliases("Roll", "ROLL", "R", "r")]
        public async Task Roll(CommandContext ctx, string command)
        {
            await ctx.TriggerTypingAsync();//send client typing to discord.

            CommandDice dice = new CommandDice(ctx.User.Mention, ctx.RawArgumentString);

            if (!dice.Parse())
            {
                await ctx.RespondAsync($"{ctx.User.Mention} please roll in a valid format:" +
                    $"##D## +/- ##);");
                return;
            }
            else if (!dice.Good())
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, please limit yourself to {dice._MAXDIE} "+
                    $"and {dice._MAXSIZE}: {dice._MAXDIE}D{dice._MAXSIZE}.");
                return;
            }
            if (!dice.Roll())
            {
                await ctx.RespondAsync("Roll Error");
                return;
            }
            //Move all error checking to it's own class
            await ctx.RespondAsync(dice.GetOutput());
        }
    }
}


/*
        static SimpleDiceRoller DiceRoller = new SimpleDiceRoller();

        [Command("Testing"), Aliases("test", "Test", "testing", "TEST", "TESTING")]
        public async Task Testing(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();//send client typing to discord.
            const string SANITIZE = @"[^\dDd+-]";//simplified.
            const string VALIDATION = @"(\d+[Dd]\d+)";
            const string PARSE = @"(?<NUMDICE>\d+)[Dd](?<DIESIZE>\d+)(?<MODIFIER>[+-]\d+)*";
            /*can move validation into parse and utilize a single regex search for validation
            //  and pulling information.
            //  @"(?<VALIDATION>(?<NUMDICE>\d+)[Dd](?<DIESIZE>\d+))(?<MODIFIER>[+-]\d)*";
            //  should work
            int currentRoll = 0;
            int numberOfDice = 0;
            int dieSize = 0;
            int totalRoll = 0;
            int totalMod = 0;

            List<int> rolls = new List<int>();
            List<string> modifiers = new List<string>();

            string inputString = Regex.Replace(ctx.RawArgumentString, SANITIZE, "");
            //declared and sanitized in one line.
            Match regexMatch = Regex.Match(inputString, VALIDATION);//checks formatting
            //  can be eliminated by moving validation into the main regex string.
            if (!regexMatch.Success)
            //if (!Regex.Match(inputString, VALIDATION).Success) could also work.
            {
                await ctx.RespondAsync($"{ctx.User.Mention}: " +
                    "Please use correct formatting: ##d##. Examples, 1d20, 2d100, 12d8");
                Console.WriteLine("regex validation fail");
                return;
            }

            regexMatch = Regex.Match(inputString, PARSE);
            if ((regexMatch.Groups["NUMDICE"].Captures.Count < 1) || regexMatch.Groups["DIESIZE"].Captures.Count < 1)
            {
                await ctx.RespondAsync("Parse Error!");
                return;
            }

            numberOfDice = Convert.ToInt32(regexMatch.Groups["NUMDICE"].Captures[0].Value);
            dieSize = Convert.ToInt32(regexMatch.Groups["DIESIZE"].Captures[0].Value);

            foreach (Capture capture in regexMatch.Groups["MODIFIER"].Captures)
                modifiers.Add(capture.Value);

            foreach (string modifier in modifiers)
                totalMod += Convert.ToInt32(modifier);


            currentRoll = DiceRoller.Roll(dieSize);
            if (currentRoll == -1)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, Please enter a valid die size!" +
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
            string outputString = new string($"{ctx.User.Username} Rolled {numberOfDice}D{dieSize} :[" +
                $"{string.Join(", ", rolls)}] + " + totalMod + $" = {totalRoll + totalMod}");

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
                    if ((i % 1900 == 0) && i > 0)
                    {
                        await ctx.RespondAsync(stringBuffer);
                        stringBuffer = null;
                    }
                }
                if (stringBuffer.Length > 0)
                {
                    await ctx.RespondAsync(stringBuffer);
                }
            }
        }
*/
