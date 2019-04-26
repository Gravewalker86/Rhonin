using System;
using System.Numerics;

namespace Rhonin.RNG
{
    using System.Security.Cryptography;//Limiting exposure to this namespace.
    public class SimpleDiceRoller
    {
        private const int _DEFAULT_SIZE = 4;//default bytes of entropy
        private const int _MAX_SIZE = 10000; //max die size.

        private int _bytesOfEntrophy = _DEFAULT_SIZE;
        private int _previousDieSize = 0;
        private byte[] _byteArray = null;
        private BigInteger _maxRoll = 0;
        private static RNGCryptoServiceProvider _crypto = new RNGCryptoServiceProvider();

        SimpleDiceRoller()
        {
            SetEntrophy(_DEFAULT_SIZE);
        }

        SimpleDiceRoller(int bytes)
        {
            SetEntrophy(bytes);
        }

        public int GetEntrophy()
        {
            return _bytesOfEntrophy;
        }

        public void SetEntrophy(int bytes)
        {
            if (bytes > 0 && bytes <= 4096)
            {
                _bytesOfEntrophy = bytes;
            }
            else
            {
                _bytesOfEntrophy = _DEFAULT_SIZE;
            }
            Array.Resize(ref _byteArray, _bytesOfEntrophy + 1);//extra byte used to ensure positive BigInts.
            _previousDieSize = -1;
            //eliminates the need to store and check for _bytesOfEntrophy changes between rolls.
        }

        private void _calcMaxRoll(int size)//generates max roll to accept.
        {
            byte[] _maxArray = new byte[_bytesOfEntrophy + 1];

            for (int i = 0; i < _maxArray.Length; i++)//fills all but last byte with 0xFF.
            {
                _maxArray[i] = 0xFF;
            }

            _maxArray[_maxArray.GetUpperBound(0)] = 0x00;//forces positive result.
            _maxRoll = new BigInteger(_maxArray);
            _maxRoll -= (_maxRoll % size);//eliminates non-full period.            
        }

        public int Roll(int size)
        {
            if(size < 2 && size > 10000)
            {
                return -1;
            } //input validation.

            BigInteger _roll;

            if (size != _previousDieSize)
            {
                _calcMaxRoll(size);
            }

            do
            {
                _crypto.GetBytes(_byteArray);
                _byteArray[_byteArray.GetUpperBound(0)] = 0x00;//forces positive BigInt.
                _roll = new BigInteger(_byteArray);
            }
            while (_roll > _maxRoll);

            _previousDieSize = size;
            return ((int)(_roll % size));
        }
    }
    
    /*
    public class LookupDiceRoller
    {
        LookupDie D4;
        LookupDie D6;
        LookupDie D8;
        LookupDie D10;
        LookupDie D12;
        LookupDie D20;
        LookupDie D100;
        LookupDie D1000;

        public LookupDiceRoller()
        {
            D4 = new LookupDie(4);
            D6 = new LookupDie(6);
            D8 = new LookupDie(8);
            D10 = new LookupDie(10);
            D12 = new LookupDie(12);
            D20 = new LookupDie(20);
            D100 = new LookupDie(100);
            D1000 = new LookupDie(1000);
        }

        void SetIterations(int iterations)//reinitializes all dice with the new number of iterations.
        {
            D4.ResetIterations(iterations);
            D6.ResetIterations(iterations);
            D8.ResetIterations(iterations);
            D10.ResetIterations(iterations);
            D12.ResetIterations(iterations);
            D20.ResetIterations(iterations);
            D100.ResetIterations(iterations);
            D1000.ResetIterations(iterations);
        }

        public int Roll(int ds)
        {
            switch (ds)
            {
                case 4:
                    return D4.Roll();
                case 6:
                    return D6.Roll();
                case 8:
                    return D8.Roll();
                case 10:
                    return D10.Roll();
                case 12:
                    return D12.Roll();
                case 20:
                    return D20.Roll();
                case 100:
                    return D100.Roll();
                case 1000:
                    return D1000.Roll();

            }
            return -1;//invalid die size
        }

    }

    public class LookupDie
    {

        private int _sides = -1;
        private int _iterations = 500;//move iterations to roller class
        private ulong _rng = 0;
        private ulong _maxRoll = 0;
        //used for reducing roll-bias by ensuring equal coverage of the roll table by eliminating the 
        //  incomplete set at the top of the range.
        private const ulong _MAXVALUE = 4294967295UL; 
        //max 32bit Value, Used for calculating maxRoll which is used to reduce roll bias. 
        //  This is hardcoded for 4 bytes.
        private byte[] _cryptoBuffer = new byte[4];//hardcoded 4bytes of entropy
        private List<int> _lookupTable = new List<int>();
        private SortedDictionary<Guid, int> _sortTable = new SortedDictionary<Guid, int>();
        private static RNGCryptoServiceProvider _crypto = new RNGCryptoServiceProvider();

        

        public LookupDie(int inputSides)
        {
            _sides = inputSides;
            _setMaxRoll();
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
            _setMaxRoll();
            _generateTable();
        }

        private void _setMaxRoll()
        {
            ulong _maxCount = (uint)(_iterations * _sides);
            _maxRoll = (_MAXVALUE / _maxCount) * _maxCount;//Hardcoded for 4 bytes
        }


        public void ResetIterations(int _newIterations)//sets iterations and reinializes die
        {
            _iterations = _newIterations;
            Reinitialize();
        }

        public int Roll()
        {
            int _lookupKey = -1;
            Array.Clear(_cryptoBuffer, 0, 4);
            _rng = 0;
            do
            {
                _crypto.GetBytes(_cryptoBuffer);
                _rng = BitConverter.ToUInt32(_cryptoBuffer);//using ToUint32 to ensure the proper 4 bytes are set.
            }
            while (_rng > _maxRoll);

            _lookupKey = (int)(_rng % (ulong)_lookupTable.Count);

            if (_lookupKey >= 0 && _lookupKey < _lookupTable.Count)
            {
                return _lookupTable[_lookupKey];
            }
            else
            {
                throw new Exception("Invalid Lookup Key!");
            }
        }
    }
    */
}
