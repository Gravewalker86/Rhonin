using System;
using System.Collections.Generic;

namespace Rhonin.RNG
{
    using System.Security.Cryptography;//Limiting exposure to this namespace.

    public class LookupDiceRoller
    {
        public void Start()
        {
            LookupDie D4 = new LookupDie(4);
            LookupDie D6 = new LookupDie(6);
            LookupDie D8 = new LookupDie(8);
            LookupDie D10 = new LookupDie(10);
            LookupDie D12 = new LookupDie(12);
            LookupDie D20 = new LookupDie(20);
            LookupDie D100 = new LookupDie(100);        
        }
    }

    public class LookupDie
    {

        private int _sides = -1;
        private int _iterations = 5000;//move iterations to roller class
        private UInt32 _rng = 0;
        private byte[] _cryptBuffer = new byte[4];
        private List<int> _lookupTable = new List<int>();
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
                    Array.Clear(_guid, 0, 16);//Clear array after each pass, may not be nessessary
                }
            }

            foreach(var entry in _sortTable) //converting sorted dictionary to list for faster lookup.
            {
                _lookupTable.Add(entry.Value);
            }
            _sortTable.Clear(); //clearing sorted dictionary for GC.
            _lookupTable.TrimExcess();//cleanup
        }

        public void Reinitialize()//used to regenerate the die.
        {
            _sortTable.Clear();
            _lookupTable.Clear();
            _generateTable();
        }

        public int Roll()
        {

            return 0;
        }
    }
}
