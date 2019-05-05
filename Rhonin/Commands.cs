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