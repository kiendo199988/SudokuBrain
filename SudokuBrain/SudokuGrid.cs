using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuBrain
{
    class SudokuGrid
    {
        private static int size = 9;
        private int[,] cells = new int[size, size];

        public int[,] GetCells()
        {
            return this.cells;
        }
        public SudokuGrid(int[,] cells)
        {
            this.cells = cells;
        }
    }
}
