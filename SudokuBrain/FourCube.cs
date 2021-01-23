using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuBrain
{
    class FourCube
    {
        //fields
        private CubeCell[,,,] cubeCells;
        private SudokuGrid originalSudokuGrid;
        //get, set methods
        public CubeCell[,,,] GetCubeCells()
        {
            return this.cubeCells;
        }
        public SudokuGrid GetSudokuGrid()
        {
            return this.originalSudokuGrid;
        }

        //constructor
        public FourCube()
        {
            cubeCells = new CubeCell[4, 9, 9, 9];
            for (int row=0; row < 9; row++)
            {
                for(int col = 0; col < 9; col++)
                {
                    for(int azimuth = 0; azimuth < 9; azimuth++)
                    {
                        for(int cube = 0; cube < 4; cube++)
                        {
                            cubeCells[cube, row, col, azimuth] = new CubeCell();
                        }
                    }
                }
            }
        }
        public FourCube(CubeCell[,,,] cells)
        {
            this.cubeCells = cells;
        }

        public FourCube(SudokuGrid sudokuGrid)
        {
            this.originalSudokuGrid = sudokuGrid;
            cubeCells = new CubeCell[4, 9, 9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    for (int azimuth = 0; azimuth < 9; azimuth++)
                    {
                        for (int cube = 0; cube < 4; cube++)
                        {
                            cubeCells[cube, row, col, azimuth] = new CubeCell();
                        }
                    }
                }
            }
            for (int row=0; row < 9; row++)
            {
                for (int col = 0; col<9;col++)
                {
                    if (originalSudokuGrid.GetCells()[row, col] != 0)
                    {
                        int azimuthalIndex = originalSudokuGrid.GetCells()[row, col] - 1;
                        for (int cube = 0; cube < 4; cube++)
                        {
                            cubeCells[cube, row, col, azimuthalIndex] = new CubeCell(1, true);
                            for(int i = 0; i < 9; i++)
                            {
                                int baseRowIndex = (row / 3) * 3;
                                int offsetRow = i / 3;
                                int baseColIndex = (col / 3) * 3;
                                int offsetCol = i % 3;
                                //block
                                if (baseRowIndex + offsetRow != row && baseColIndex + offsetCol != col)
                                {
                                    cubeCells[cube, baseRowIndex + offsetRow, baseColIndex + offsetCol, azimuthalIndex] = new CubeCell(0, true);
                                }
                                //row
                                if (i != row)
                                {
                                    cubeCells[cube, i, col, azimuthalIndex] = new CubeCell(0, true);
                                }
                                //col
                                if (i != col)
                                {
                                    cubeCells[cube, row, i, azimuthalIndex] = new CubeCell(0, true);
                                }
                                //azimuth
                                if (i != azimuthalIndex)
                                {
                                    cubeCells[cube, row, col, i] = new CubeCell(0, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int cube = 0; cube < 4; cube++)
                        {
                            for (int azimuthalIndex = 0; azimuthalIndex < 9; azimuthalIndex++)
                            {
                                //cubeCells[cube, row, col, azimuthalIndex].GetIsClue() != true
                                if (cubeCells[cube, row, col, azimuthalIndex].GetIsClue() != true)
                                {
                                    cubeCells[cube, row, col, azimuthalIndex] = new CubeCell();
                                }
                            }
                        }
                    }

                }
            }
        }

        //methods
        //plus 2 four-cube
        public FourCube Plus(FourCube fc)
        {
            CubeCell[,,,] newCubeCells = new CubeCell[4, 9, 9, 9];
            
            for (int cube = 0; cube < 4; cube++)
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        for (int azimuth = 0; azimuth < 9; azimuth++)
                        {
                            if(cubeCells[cube,row,col,azimuth].GetIsClue()!=true && fc.GetCubeCells()[cube, row, col, azimuth].GetIsClue() != true)
                            {
                                newCubeCells[cube, row, col, azimuth] = new CubeCell();
                                newCubeCells[cube, row, col, azimuth].SetConfidence(cubeCells[cube, row, col, azimuth].GetConfidence() + fc.GetCubeCells()[cube, row, col, azimuth].GetConfidence());
                            }
                            else
                            {
                                newCubeCells[cube, row, col, azimuth] = new CubeCell(cubeCells[cube, row, col, azimuth].GetConfidence(), true);
                            }
                        }
                    }
                }
            }
            return new FourCube(newCubeCells);
        }

        //minus 2 four-cube
        public FourCube Minus(FourCube fc)
        {
            CubeCell[,,,] newCubeCells = new CubeCell[4, 9, 9, 9];
            for (int cube = 0; cube < 4; cube++)
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        for (int azimuth = 0; azimuth < 9; azimuth++)
                        {
                            if (cubeCells[cube, row, col, azimuth].GetIsClue() != true && fc.GetCubeCells()[cube, row, col, azimuth].GetIsClue() != true)
                            {
                                newCubeCells[cube, row, col, azimuth] = new CubeCell();
                                newCubeCells[cube, row, col, azimuth].SetConfidence(cubeCells[cube, row, col, azimuth].GetConfidence() - fc.GetCubeCells()[cube, row, col, azimuth].GetConfidence());
                            }
                            else
                            {
                                newCubeCells[cube, row, col, azimuth] = new CubeCell(cubeCells[cube, row, col, azimuth].GetConfidence(), true);
                            }
                        }
                    }
                }
            }
            return new FourCube(newCubeCells);
        }

        //multiply
        public FourCube Multiply(double v)
        {
            CubeCell[,,,] newCubeCells = new CubeCell[4, 9, 9, 9];
            for (int cube = 0; cube < 4; cube++)
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        for (int azimuth = 0; azimuth < 9; azimuth++)
                        {
                            if (cubeCells[cube, row, col, azimuth].GetIsClue() != true)
                            {
                                newCubeCells[cube, row, col, azimuth] = new CubeCell(v * cubeCells[cube, row, col, azimuth].GetConfidence(),false);
                            }
                            else
                            {
                                newCubeCells[cube, row, col, azimuth] = new CubeCell(cubeCells[cube, row, col, azimuth].GetConfidence(), true);
                            }
                        }
                    }
                }
            }
            return new FourCube(newCubeCells);
        }
             

        //equalizer
        public FourCube Equalizer()
        {
            Column[,,] equalizerColumns = new Column[9,9,9];
            //extract and perform equalizer on cloumns
            for(int row = 0; row < 9; row++)
            {
                for(int col = 0; col < 9; col++)
                {
                    for(int azimuth = 0; azimuth < 9; azimuth++)
                    {
                        CubeCell[] equalizedCubeCells = new CubeCell[4];
                        for (int cube = 0; cube < 4; cube++)
                        {
                            equalizedCubeCells[cube] = cubeCells[cube, row, col, azimuth];
                        }
                        Column c = new Column(equalizedCubeCells);
                        equalizerColumns[row, col, azimuth] = c.Equalizer();
                    }
                }
            }

            //create a blank fourcube
            CubeCell[,,,] newCubeCells = new CubeCell[4, 9, 9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    for (int azimuth = 0; azimuth < 9; azimuth++)
                    {
                        for (int cube = 0; cube < 4; cube++)
                        {
                            newCubeCells[cube, row, col, azimuth] = cubeCells[cube, row, col, azimuth].Copy();
                        }
                    }
                }
            }

            //replace in a new fourcube
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    for (int azimuth = 0; azimuth < 9; azimuth++)
                    {
                        for (int cube = 0; cube < 4; cube++)
                        {
                            newCubeCells[cube, row, col, azimuth] = equalizerColumns[row, col, azimuth].GetCubeCells()[cube];
                        }                     
                    }
                }
            }

            return new FourCube(newCubeCells);
        }


        ////selector
        public FourCube Selector()
        {
            CubeCell[,,,] newCubeCells = new CubeCell[4, 9, 9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    for (int azimuth = 0; azimuth < 9; azimuth++)
                    {
                        for (int cube = 0; cube < 4; cube++)
                        {
                            newCubeCells[cube, row, col, azimuth] = cubeCells[cube, row, col, azimuth].Copy();
                        }
                    }
                }
            }

            //row 
            Column[,] rowColumns = new Column[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int azimuth = 0; azimuth < 9; azimuth++)
                {
                    CubeCell[] cellsForSelect = new CubeCell[9];
                    for (int col = 0; col < 9; col++)
                    {
                        cellsForSelect[col] = cubeCells[0, row, col, azimuth];
                    }
                    Column c = new Column(cellsForSelect);
                    rowColumns[row, azimuth] = c.Selector();
                }
            }

            //replace rows in a new 4-dimensional array
            for (int row = 0; row < 9; row++)
            {
                for (int azimuth = 0; azimuth < 9; azimuth++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        newCubeCells[0, row, col, azimuth] = rowColumns[row, azimuth].GetCubeCells()[col];
                    }

                }
            }

            //column 
            Column[,] columnColumns = new Column[9, 9];
            for (int col = 0; col < 9; col++)
            {
                for (int azimuth = 0; azimuth < 9; azimuth++)
                {
                    CubeCell[] cellsForSelect = new CubeCell[9];
                    for (int row = 0; row < 9; row++)
                    {
                        cellsForSelect[row] = cubeCells[1, row, col, azimuth];
                    }
                    Column c = new Column(cellsForSelect);
                    columnColumns[col, azimuth] = c.Selector();
                }
            }

            //replace columns in a new 4-dimensional array
            for (int col = 0; col < 9; col++)
            {
                for (int azimuth = 0; azimuth < 9; azimuth++)
                {
                    for (int row = 0; row < 9; row++)
                    {
                        newCubeCells[1, row, col, azimuth] = columnColumns[col, azimuth].GetCubeCells()[row];
                    }

                }
            }

            //block
            Column[,,] blockColumn = new Column[9, 3, 3];
            for (int azimuth = 0; azimuth < 9; azimuth++)
            {
                for (int row = 0; row < 7; row += 3)
                {
                    for (int col = 0; col < 7; col += 3)
                    {
                        CubeCell[] cellsForSelect = new CubeCell[9];
                        for (int i = 0; i < 9; i++)
                        {
                            int baseRowIndex = (row / 3) * 3;
                            int offsetRow = i / 3;
                            int baseColIndex = (col / 3) * 3;
                            int offsetCol = i % 3;

                            cellsForSelect[i] = cubeCells[2, baseRowIndex + offsetRow, baseColIndex + offsetCol, azimuth];
                        }
                        Column c = new Column(cellsForSelect);
                        blockColumn[azimuth, row / 3, col / 3] = c.Selector();
                    }
                }
            }

            //replace the block-column in a new fourcube
            for (int azimuth = 0; azimuth < 9; azimuth++)
            {
                for (int row = 0; row < 7; row += 3)
                {
                    for (int col = 0; col < 7; col += 3)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            int baseRowIndex = (row / 3) * 3;
                            int offsetRow = i / 3;
                            int baseColIndex = (col / 3) * 3;
                            int offsetCol = i % 3;
                            newCubeCells[2, baseRowIndex + offsetRow, baseColIndex + offsetCol, azimuth]
                                = blockColumn[azimuth, row / 3, col / 3].GetCubeCells()[i];
                        }
                    }
                }
            }

            //azimuthal
            Column[,] azimuthalColumns = new Column[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    CubeCell[] cellsForSelect = new CubeCell[9];
                    for (int azimuth = 0; azimuth < 9; azimuth++)
                    {
                        cellsForSelect[azimuth] = cubeCells[3, row, col, azimuth];
                    }
                    Column c = new Column(cellsForSelect);
                    azimuthalColumns[row, col] = c.Selector();
                }
            }

            //replace azimuthal in a new 4-dimensional array
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    for (int azimuth = 0; azimuth < 9; azimuth++)
                    {
                        newCubeCells[3, row, col, azimuth] = azimuthalColumns[row, col].GetCubeCells()[azimuth];
                    }

                }
            }

            return new FourCube(newCubeCells);
        }

        public int GetNrOfTwoCellsWithDifferentConfidence(FourCube fc)
        {
            int nrOfCellsWithDifferentConfidence = 0;
            for (int cube = 0; cube < 4; cube++)
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        for (int azimuth = 0; azimuth < 9; azimuth++)
                        {
                            if(fc.GetCubeCells()[cube,row,col,azimuth].GetConfidence() != this.cubeCells[cube, row, col, azimuth].GetConfidence())
                            {
                                nrOfCellsWithDifferentConfidence++;
                            }
                        }
                    }
                }
            }
            return nrOfCellsWithDifferentConfidence;
        }

        //compare if two 4-cubes are equal
        public bool Equals(FourCube fc)
        {
            if (this.GetNrOfTwoCellsWithDifferentConfidence(fc) != 0) { return false; }
            else { return true; }
        }

        ////RRR-dynamical
        //public FourCube OneRRRCycle()
        //{
        //   return this.Plus(this.Equalizer().Multiply(2).Minus(this).Selector2()).Minus(this.Equalizer());
        //}

        ////rrrdynamical for 100 times
        //public FourCube OneHundredCycles(FourCube fc, int i)
        //{
        //    if (i >= 0)
        //    {
        //        i--;
        //        return fc.OneRRRCycle().OneHundredCycles(fc.OneRRRCycle(), i);
        //    }
        //    else { return fc.OneRRRCycle(); }
        //}
        //public FourCube Selector()
        //{
        //    //create a blank fourcube
        //    CubeCell[,,,] newCubeCells = new CubeCell[4, 9, 9, 9];
        //    for (int row = 0; row < 9; row++)
        //    {
        //        for (int col = 0; col < 9; col++)
        //        {
        //            for (int azimuth = 0; azimuth < 9; azimuth++)
        //            {
        //                for (int cube = 0; cube < 4; cube++)
        //                {
        //                    newCubeCells[cube, row, col, azimuth] = cubeCells[cube, row, col, azimuth].Copy();
        //                }
        //            }
        //        }
        //    }

        //    //extract the columns
        //    Column[,] rowColumns = new Column[9, 9];
        //    Column[,] columnColumns = new Column[9, 9];
        //    Column[,] azimuthalColumns = new Column[9, 9];
        //    Column[,,] blockColumn = new Column[9, 3, 3];
        //    for (int cube = 0; cube < 4; cube++)
        //    {
        //        for (int i = 0; i < 9; i++)
        //        {
        //            if(cube == 2)
        //            {
        //                for (int row = 0; row < 7; row += 3)
        //                {
        //                    for (int col = 0; col < 7; col += 3)
        //                    {
        //                        CubeCell[] cellsForSelect = new CubeCell[9];
        //                        for (int q = 0; q < 9; q++)
        //                        {
        //                            int baseRowIndex = (row / 3) * 3;
        //                            int offsetRow = q / 3;
        //                            int baseColIndex = (col / 3) * 3;
        //                            int offsetCol = q % 3;
        //                            cellsForSelect[q] = cubeCells[2, baseRowIndex + offsetRow, baseColIndex + offsetCol, i];
        //                        }
        //                        Column c = new Column(cellsForSelect);
        //                        blockColumn[i, row / 3, col / 3] = c.Selector();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                for (int j = 0; j < 9; j++)
        //                {
        //                    CubeCell[] cellsForSelect = new CubeCell[9];
        //                    for (int k = 0; k < 9; k++)
        //                    {
        //                        if (cube == 0) { cellsForSelect[k] = cubeCells[cube, i, k, j]; }
        //                        else if (cube == 1) { cellsForSelect[k] = cubeCells[cube, k, i, j]; }
        //                        else if (cube == 3) { cellsForSelect[k] = cubeCells[cube, i, j, k]; }
        //                    }
        //                    Column c = new Column(cellsForSelect);
        //                    if (cube == 0) { rowColumns[i, j] = c.Selector(); }
        //                    else if (cube == 1) { columnColumns[i, j] = c.Selector(); }
        //                    else if (cube == 3) { azimuthalColumns[i, j] = c.Selector(); }
        //                }
        //            }

        //        }
        //    }

        //    //replace in the blank fourcube
        //    for(int cube = 0; cube < 4; cube++)
        //    {
        //        for (int i = 0; i < 9; i++)
        //        {
        //            if(cube == 2)
        //            {
        //                for (int row = 0; row < 7; row += 3)
        //                {
        //                    for (int col = 0; col < 7; col += 3)
        //                    {
        //                        for (int q = 0; q < 9; q++)
        //                        {
        //                            int baseRowIndex = (row / 3) * 3;
        //                            int offsetRow = q / 3;
        //                            int baseColIndex = (col / 3) * 3;
        //                            int offsetCol = q % 3;
        //                            newCubeCells[cube, baseRowIndex + offsetRow, baseColIndex + offsetCol, i]
        //                                = blockColumn[i, row / 3, col / 3].GetCubeCells()[q];
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                for (int j = 0; j < 9; j++)
        //                {
        //                    for (int k = 0; k < 9; k++)
        //                    {
        //                        if (cube == 0) { newCubeCells[cube, i, k, j] = rowColumns[i, j].GetCubeCells()[k]; }
        //                        if (cube == 1) { newCubeCells[cube, k, i, j] = columnColumns[i, j].GetCubeCells()[k]; }
        //                        if (cube == 3) { newCubeCells[cube, i, j, k] = columnColumns[i, j].GetCubeCells()[k]; }
        //                    }

        //                }
        //            }                  
        //        }
        //    }

        //    //return new fourcube
        //    return new FourCube(newCubeCells);
        //}
    }
}
