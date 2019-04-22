using System;
using System.Collections.Generic;
using System.Text;

namespace Rhonin.RNG
{

    class DiceRoller
    {
    }

    public class Dice
    {
        private const double MACHINE_EPSILION = 0.000000000000001d; //needed for floor function to work in 0 < rand < 1 based RNG.
        private int _sides = -1;
        private List<int> _rollTable = new List<int>();
        private SortedDictionary<Guid, int> _sortTable = new SortedDictionary<Guid, int>();

        public Dice(int inputSides)
        {
            _sides = inputSides;
            _generateTable();
        }

        private void _generateTable()
        {
            for (int i = 1; i <= _sides; i++)
            {

            }
        }


    }
}
