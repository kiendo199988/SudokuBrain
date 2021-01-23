using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuBrain
{
    class GridCell
    {
        private int value;
        public int GetValue()
        {
            return this.value;
        }
        public GridCell(int value)
        {
            this.value = value;
        }
    }
}
