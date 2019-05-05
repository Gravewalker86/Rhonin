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
        private const string _SANITIZE = @"[^d+D-\d]";//regex clean input
        private const string _PARSE = @"(?<VALIDATION>(?<NUMDICE>\d+)d(?<DIESIZE>\d+))(?<MOD>[+-]\d+)*";
        public readonly int _MAXDIE = 500;
        public readonly int _MAXSIZE = SimpleDiceRoller._MAX_SIZE;

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
            //string input = _rawCommand;
            //input.ToLower();
            //Regex.Replace(input, _SANITIZE, "");//Problem Line
            //Console.WriteLine(input);
            string command = Regex.Replace(_rawCommand, _SANITIZE, "");
            Match match = Regex.Match(_rawCommand, _PARSE);

            if (!match.Groups["VALIDATION"].Success)
                return false;

            _diceCount = Convert.ToInt32(match.Groups["NUMDICE"].Value);
            _dieSize = Convert.ToInt32(match.Groups["DIESIZE"].Value);
            _totalMod = CalcMods(match);
            return true;
        }

        public bool Good()
        {
            if (_diceCount < 1 || _diceCount > _MAXDIE || _dieSize < 2 || _dieSize > _MAXSIZE)
            {
                return false;
            }
            return true;
        }

        public bool Roll()
        {
            SimpleDiceRoller roller = new SimpleDiceRoller();
            int currentRoll = 0;
            for (int i = 1; i < _diceCount; i++)
            {
                currentRoll = roller.Roll(_dieSize);
                _rolls.Add(currentRoll);
                _totalRoll += currentRoll;
            }
            if (currentRoll == -1)
                return false;

            return true;
        }

        public string GetOutput()
        {
            string output = $"{_user} Rolled: [{string.Join(", ", _rolls)}] + {_totalMod} = {_totalMod + _totalRoll}";
            if (output.Length > 2000)
            {
                Console.WriteLine($"OUTPUT ERROR: {_rawCommand} produced {output.Length} characters, username: {_user} characters");
                return $"Error, Output is more than 2000 characters: {output.Length}. Try using less dice!";
            }//re-implement output segmentation and move errors to their own class.
            return output;
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