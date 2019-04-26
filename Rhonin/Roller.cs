using System;
using System.Numerics;

namespace Rhonin.RNG
{
    using System.Security.Cryptography;//Limiting exposure to this namespace.

    public class SimpleDiceRoller
    {
        private const int _DEFAULT_SIZE = 4;//default bytes of entropy
        public const int _MAX_SIZE = 10000; //max die size.

        private int _bytesOfEntrophy = _DEFAULT_SIZE;
        private int _previousDieSize = 0;
        private byte[] _byteArray = null;
        private BigInteger _maxRoll = 0;
        private static RNGCryptoServiceProvider _crypto = new RNGCryptoServiceProvider();

        public SimpleDiceRoller()
        {
            SetEntrophy(_DEFAULT_SIZE);
        }

        public SimpleDiceRoller(int bytes)
        {
            SetEntrophy(bytes);
        }

        public int GetMax()
        {
            return _MAX_SIZE;
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
                return -1; //input validation.

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
            return ((int)(_roll % size) + 1);
        }
    }
}