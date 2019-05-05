using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Rhonin;
using Rhonin.RNG;
using Rhonin.Utilities;
using System.Text;

namespace Rhonin.CommandLogic
{
    public class CommandDice
    {
        private const string _SANITIZE = @"[^\dDd+-]";//regex clean input
        private const string _PARSE = @"(?<VALIDATION>(?<NUMDICE>\d+)d(?<DIESIZE>\d+))(?<MODS>[+-]\d+)*";
        public readonly int _MAXDIE = 500;
        public readonly int _MAXSIZE = SimpleDiceRoller._MAX_SIZE;

        int _diceCount = 0;
        int _dieSize = 0;
        int _totalMod = 0;
        int _totalRoll = 0;
        string _input = null;
       
        readonly string _rawCommand;
        readonly string _user;

        List<int> _rolls = new List<int>();

        public CommandDice(string user, string context)
        {
            _user = user;
            _rawCommand = context;
        }

        public bool Parse()
        {
            _input = _rawCommand.ToLower();
            _input = Regex.Replace(_input, _SANITIZE, "");//LINE FIXED!
            Match match = Regex.Match(_input, _PARSE);

            if (!match.Groups["VALIDATION"].Success)
                return false;

            _diceCount = Convert.ToInt32(match.Groups["NUMDICE"].Value);
            _dieSize = Convert.ToInt32(match.Groups["DIESIZE"].Value);

            if (match.Groups["MODS"].Captures.Count >= 1)
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
            for (int i = 0; i < _diceCount; i++)
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
            string output = $"{_user} Rolled \"{_input}\": [{string.Join(", ", _rolls)}] ";
            if(!(_totalMod == 0))
            {
                if (_totalMod > 0)
                    output += $"+ {_totalMod} ";
                else
                    output += $"- { -_totalMod} ";
            }

            output += $"= {_totalMod + _totalRoll}";

            if (output.Length > 2000)
            {
                Console.WriteLine($"OUTPUT ERROR: {_rawCommand} produced {output.Length} characters, username: {_user} characters");
                return $"Error, Output is more than 2000 characters: {output.Length}. Try using less dice!";
            }//re-implement output segmentation and move errors to their own class.
            return output;
        }

        public static int CalcMods(Match match)
        {
            if (match.Groups["MODS"].Captures.Count < 1)
                return 0;

            int totalMod = 0;
            foreach (Capture cap in match.Groups["MODS"].Captures)
                totalMod += Convert.ToInt32(cap.Value);
                        
            return totalMod;
        }
    }

    static class Output//for segmenting output.
    {

    }
}