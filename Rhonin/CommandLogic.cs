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
        private const string _PARSE = @"(?<VALIDATION>(?<NUMDICE>\d+)d(?<DIESIZE>\d+))(?<MOD>[+-]\d+)*";
        //regex parse input.
     
        public Match ParseRegex(string input)
        {
            input.ToLower();
            Regex.Replace(input, _SANITIZE, "");
            return Regex.Match(input, _PARSE);
        }

        public bool IsValid (Match parse)
        {
            return parse.Groups["VALIDATION"].Success;
        }

        public int DiceCount (Match parse)
        {
            return Convert.ToInt32(parse.Groups["NUMDICE"].Value);
        }

        public int DieSize (Match parse)
        {
            return Convert.ToInt32(parse.Groups["DIESIZE"].Value);
        }

        public int Mods (Match parse)
        {
            if (parse.Groups["MOD"].Captures.Count < 1)
                return 0;

            int totalMod = 0;
            foreach (Capture cap in parse.Groups["MOD"].Captures)
            {
                totalMod += Convert.ToInt32(cap.Value);
            }

            return totalMod;
        }
    }
}