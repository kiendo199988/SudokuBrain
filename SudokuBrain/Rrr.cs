using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuBrain
{
    class Rrr
    {
        private FourCube fc;
        private FourCube fc2;
        //private FourCube solution;
        
        public FourCube GetFourCubeAfter1RRRCycle()
        {
            return fc2;
        }

        //public FourCube GetSolution()
        //{
        //    return solution;
        //}
        public Rrr(FourCube fc)
        {
            this.fc = fc;
            this.fc2 = fc.Plus(fc.Equalizer().Multiply(2).Minus(fc).Selector()).Minus(fc.Equalizer());
        }

        ////step
        //public void Step()
        //{
        //    bool isDone = false;
        //    Rrr rrr = new Rrr(fc);
        //    while (!isDone)
        //    {              
        //        nrOfSteps++;
        //        Rrr intermediate = rrr.GetRrrByANumberOfCycles(rrr, nrOfSteps);
        //        if (intermediate.IsReady())
        //        {
        //            solution = intermediate.GetFourCubeAfter1RRRCycle();
        //        }
        //    }
            
        //}
        //is ready
        public bool IsReady()
        {
            if (fc.Equals(fc2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int DifferenceCellsFromLastStep()
        {
            return fc.GetNrOfTwoCellsWithDifferentConfidence(fc2);
        }

       

        public Rrr GetRrrByANumberOfCycles(Rrr newRrr,int i)
        {
            if (i > 0)
            {
                i--;
                Rrr r = new Rrr(newRrr.GetFourCubeAfter1RRRCycle());
                return GetRrrByANumberOfCycles(r, i);
            }
            else
            {
                return newRrr;
            }
        }
       
    }
}
