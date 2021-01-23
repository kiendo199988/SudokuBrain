using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuBrain
{
    class CubeCell
    {
        //fields
        private double confidence;
        private bool isClue;

        //get, set
        public double GetConfidence()
        {
            return this.confidence;
        }
        public void SetConfidence(double confi)
        {
            this.confidence = confi;
        }
        public bool GetIsClue()
        {
            return this.isClue;
        }
        public void SetIsClue(bool clue)
        {
            this.isClue = clue;
        }

        //constructor
        public CubeCell(double confidence, bool clue)
        {
            this.confidence = confidence;
            this.isClue = clue;
        }
        public CubeCell()
        {
            this.confidence = 0;
            this.isClue = false;
        }
        //method
        public CubeCell Copy()
        {
            return new CubeCell(confidence,isClue);
        }
    }
}
