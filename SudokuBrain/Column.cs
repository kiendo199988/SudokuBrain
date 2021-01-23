using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuBrain
{
    class Column
    {
        //fields
        private CubeCell[] cubeCells;
        
        //get, set
        public CubeCell[] GetCubeCells()
        {
            return this.cubeCells;
        }
        public void SetCubeCells(CubeCell[] cubeCells)
        {
            this.cubeCells = cubeCells;
        }
        //constructor
        public Column(CubeCell[] cubeCells)
        {
            this.cubeCells = cubeCells;
        }

        //selector
        public Column Selector()
        {
            CubeCell[] newCells = new CubeCell[9];
            CubeCell winner = cubeCells[0];
            for (int i = 1; i < 10; i++)
            {
                if (winner.GetIsClue() == true && winner.GetConfidence() == 1)
                {
                    newCells[i-1] = new CubeCell(1, true);
                    if (i < 9)
                    {
                        for (int j = i; j < 9; j++)
                        {
                            newCells[j] = cubeCells[j].Copy();

                        }
                    }
                   
                    break;
                }
                else if (winner.GetIsClue() == true && winner.GetConfidence() == 0)
                {
                    newCells[i - 1] = winner.Copy();
                    winner = cubeCells[i];
                }
                else if (winner.GetIsClue() != true)
                {
                    int nrOfNonClueCellWithBiggerConfidence = 0;
                    for (int j = i; j < 9; j++)
                    {
                        if (cubeCells[j].GetIsClue() == false && winner.GetConfidence() < cubeCells[j].GetConfidence())
                        {
                            nrOfNonClueCellWithBiggerConfidence++;
                        }
                    }

                    if (nrOfNonClueCellWithBiggerConfidence == 0)
                    {
                        newCells[i - 1] = new CubeCell(1, false);
                        for (int j = i; j < 9; j++)
                        {
                            if (cubeCells[j].GetIsClue() == false)
                            {
                                newCells[j] = new CubeCell(0, false);
                            }
                            else
                            {
                                newCells[j] = cubeCells[j].Copy();
                            }
                        }
                        break;
                    }
                    else
                    {
                        newCells[i - 1] = new CubeCell(0, false);
                        winner = cubeCells[i];
                    }

                }
            }        
            return new Column(newCells);
        }

        //equalizer
        public Column Equalizer()
        {
            double sum = 0;
            CubeCell[] newCells = new CubeCell[4];
            for (int i = 0; i < cubeCells.Length; i++)
            {
                sum += cubeCells[i].GetConfidence();
            }

            for(int i = 0; i < cubeCells.Length; i++)
            {
                if(cubeCells[i].GetIsClue() == true)
                {
                    newCells[i] = new CubeCell(sum / 4, true);
                }
                else
                {
                    newCells[i] = new CubeCell(sum/4,false);
                }
            }
            return new Column(newCells);
        }
    }
}
