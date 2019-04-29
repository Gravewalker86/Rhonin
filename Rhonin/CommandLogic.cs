using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Rhonin;
using Rhonin.RNG;
using Rhonin.Utilities;

namespace Rhonin.Commands
{
    class Dice
    {
        private const string _SANITIZE = @"[^d+-\d]";//regex clean input
        private const string _PARSE = @"(?<VALIDATION>(?<NUMDICE>\d+)[Dd](?<DIESIZE>\d+))(?<MODIFIER>[+-]\d+)*";
        //regex parse input.

        int currentRoll = 0;
        int numberOfDice = 0;
        int dieSize = 0;
        int totalRoll = 0;
        int totalMod = 0;

        List<int> rolls = new List<int>();
        List<string> modifiers = new List<string>();
     
        public Match ParseRegex(string input)
        {
            input.ToLower();
            Regex.Replace(input, _SANITIZE, "");
            return Regex.Match(input, _PARSE);
        }
    }
}