using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Rhonin;
using Rhonin.RNG;
using Rhonin.Utilities;

namespace Rhonin.CommandLogic
{
    public class CommandDice
    {
        private const string _SANITIZE = @"[^d+-\d]";//regex clean input
        private const string _PARSE = @"(?<VALIDATION>(?<NUMDICE>\d+)d(?<DIESIZE>\d+))(?<MOD>[+-]\d+)*";

        bool _valid = false;
        bool _sorted = false;
        int _diceCount = 0;
        int _dieSize = 0;
        int _totalMod = 0;
        int _totalRoll = 0;
        readonly string _rawCommand;
        readonly string _user;

        List<int> _rolls = new List<int>();
        List<string> _output = new List<string>();

        public CommandDice(string user, string context)
        {
            _user = user;
            _rawCommand = context;
        }


        public bool Parse()
        {
            string input = _rawCommand;
            input.ToLower();
            Regex.Replace(input, _SANITIZE, "");

            Match match = Regex.Match(input, _PARSE);

            if (!match.Groups["VALIDATION"].Success)
                return false;

            _valid = true;
            _diceCount = Convert.ToInt32(match.Groups["NUMDICE"].Value);
            _dieSize = Convert.ToInt32(match.Groups["DIESIZE"].Value);
            _totalMod = CalcMods(match);
            return true;
        }

        public Roll()
        {
            SimpleDiceRoller roller = new SimpleDiceRoller();
            int currentRoll = 0;
            for (int i = 1; i < _diceCount; i++)
            {
                currentRoll = roller.Roll(_dieSize);
                _rolls.Add(currentRoll);
                _totalRoll += currentRoll;
            }
        }

        public static int CalcMods (Match match)
        {
            if (match.Groups["MOD"].Captures.Count < 1)
                return 0;

            int totalMod = 0;
            foreach (Capture cap in match.Groups["MOD"].Captures)
            {
                totalMod += Convert.ToInt32(cap.Value);
            }

            return totalMod;
        }
    }

    static class Output
    {

    }
}