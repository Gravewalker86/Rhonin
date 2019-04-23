using System;
using System.Collections.Generic;
using System.Text;

namespace Rhonin.RNG
{
    using System.Security.Cryptography;//Limiting exposure.

    //private const double MACHINE_EPSILION = 0.000000000000001d; //needed for floor function to work in 0 < rand < 1 based RNG.

    class LookupDiceRoller
    {
        public void Start()
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            LookupDie D4 = new LookupDie(4);
            LookupDie D6 = new LookupDie(6);
            LookupDie D8 = new LookupDie(8);
            LookupDie D10 = new LookupDie(10);
            LookupDie D12 = new LookupDie(12);
            LookupDie D20 = new LookupDie(20);
            LookupDie D100 = new LookupDie(100);
            timer.Stop();

            D20._dumpTable();
            Console.WriteLine($"Tables generated in: {timer.Elapsed.TotalMilliseconds} Milliseconds.");

            
        }
    }

    public class LookupDie
    {

        private int _sides = -1;
        private int _iterations = 5000;
        private List<int> _rollTable = new List<int>();
        private SortedDictionary<Guid, int> _sortTable = new SortedDictionary<Guid, int>();
        private static RNGCryptoServiceProvider _crypto = new RNGCryptoServiceProvider();

        public LookupDie(int inputSides)
        {
            _sides = inputSides;
            _generateTable();
        }

        private void _generateTable()//generates the shuffled lookup table for the die.
        {
            for (int x = 1; x <= _sides; x++)//loop for each side of the die.
            {
                for(int y = 0; y < _iterations; y++)//loop for iterations.
                {
                    byte[] _guid = new byte[16];
                    _crypto.GetBytes(_guid);
                    _sortTable.Add(new Guid(_guid), x);
                }
            }

            foreach(var entry in _sortTable) //converting sorted dictionary to list for faster lookup.
            {
                _rollTable.Add(entry.Value);
            }

            _sortTable.Clear(); //clearing sorted dictionary for GC.
        }

        public void _dumpTable()
        {
            foreach(var entry in _rollTable)
            {
                Console.WriteLine($"Value: {entry}");
            }
        }
    }
}
