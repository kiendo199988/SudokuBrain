using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace SudokuBrain
{
    public partial class Form1 : Form
    {
        private SudokuGrid sudokuGrid;
        private FourCube fc;
        private int nrOfSteps=0;
        private Thread thread;
        private TimeSpan timeSpan;
        //bool inCriticalSection = true;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Kien's Sudoku Brain";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timeSpan = TimeSpan.FromMilliseconds(trackBarSudoku.Value);         
        }


        private void EnterPuzzle()
        {
            int[] oneDimensionalArray = new int[81];
            for (int i = 1; i <= 81; i++)
            {
                if (this.Controls.ContainsKey("tb" + i.ToString()))
                {
                    TextBox tb = this.Controls["tb" + i.ToString()] as TextBox;
                    int result;
                    if (tb.Text != "" && Int32.TryParse(tb.Text, out result))
                    {
                        oneDimensionalArray[i - 1] = result;
                    }
                    else
                    {
                        oneDimensionalArray[i - 1] = 0;
                    }
                }
            }

            int[,] twoDimensionalArray = new int[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    twoDimensionalArray[x, y] = oneDimensionalArray[9 * x + y];
                }
            }
            sudokuGrid = new SudokuGrid(twoDimensionalArray);
            fc = new FourCube(sudokuGrid);
        }
        private void btnEnterPuzzle_Click(object sender, EventArgs e)
        {
            EnterPuzzle();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            

         
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void Step()
        {
            Rrr rrr = new Rrr(fc);
            Rrr intermediate = rrr.GetRrrByANumberOfCycles(rrr, nrOfSteps);
            if (intermediate.IsReady() == false)
            {
                lbDigitSelection.Items.Clear();
                lbRowSelection.Items.Clear();
                lbColumnSelection.Items.Clear();
                lbBlockSelection.Items.Clear();

                FourCube equalized4Cube = intermediate.GetFourCubeAfter1RRRCycle().Equalizer();
                FourCube selector4Cube = equalized4Cube.Selector();
                int[,] azimuth2D = new int[9, 9];
                int[,] row2D = new int[9, 9];
                int[,] column2D = new int[9, 9];
                int[,] block2D = new int[9, 9];

                double[,] confidences2D = new double[9, 9];
                int[] azimuth1D = new int[81];
                double[] confidences1D = new double[81];
                int confidenceSmaller02 = 0;
                int confidenceSmaller04 = 0;
                int confidenceSmaller06 = 0;
                int confidenceSmaller08 = 0;
                int confidenceSmaller10 = 0;

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        for (int azimuth = 0; azimuth < 9; azimuth++)
                        {
                            if (selector4Cube.GetCubeCells()[3, row, col, azimuth].GetConfidence() == 1)
                            {
                                azimuth2D[row, col] = azimuth + 1;
                                confidences2D[row, col] = equalized4Cube.GetCubeCells()[3, row, col, azimuth].GetConfidence();
                            }
                            if (selector4Cube.GetCubeCells()[0, row, col, azimuth].GetConfidence() == 1)
                            {
                                column2D[row, col] = azimuth + 1;
                            }
                            if (selector4Cube.GetCubeCells()[1, row, col, azimuth].GetConfidence() == 1)
                            {
                                row2D[row, col] = azimuth + 1;
                            }
                            if (selector4Cube.GetCubeCells()[2, row, col, azimuth].GetConfidence() == 1)
                            {
                                block2D[row, col] = azimuth + 1;
                            }
                        }
                    }
                }

                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        azimuth1D[9 * x + y] = azimuth2D[x, y];
                        confidences1D[9 * x + y] = confidences2D[x, y];
                    }
                }

                for (int i = 1; i <= 81; i++)
                {
                    if (this.Controls.ContainsKey("tb" + (i).ToString()))
                    {
                        TextBox tb = this.Controls["tb" + i.ToString()] as TextBox;
                        tb.Text = azimuth1D[i - 1].ToString();
                        if (confidences1D[i - 1] > 0 && confidences1D[i - 1] <= 0.2)
                        { tb.ForeColor = Color.Pink; confidenceSmaller02++; }
                        else if (confidences1D[i - 1] > 0.2 && confidences1D[i - 1] <= 0.4)
                        { tb.ForeColor = Color.Red; confidenceSmaller04++; }
                        else if (confidences1D[i - 1] > 0.4 && confidences1D[i - 1] <= 0.6)
                        { tb.ForeColor = Color.Green; confidenceSmaller06++; }
                        else if (confidences1D[i - 1] > 0.6 && confidences1D[i - 1] <= 0.8)
                        { tb.ForeColor = Color.Blue; confidenceSmaller08++; }
                        else if (confidences1D[i - 1] > 0.8 && confidences1D[i - 1] <= 1)
                        { tb.ForeColor = Color.Black; confidenceSmaller10++; }
                    }
                }


                //chart
                chartDifference.Series["Differences from the previous step"].Points.Add(intermediate.DifferenceCellsFromLastStep());
                //chartDifference.Series["Differences from the solution"].Points.Add(intermediate.DifferenceCellsFromSolution());

                chartConfidence.Series["0-0.2"].Points.Add(confidenceSmaller02);
                chartConfidence.Series["0.2-0.4"].Points.Add(confidenceSmaller04);
                chartConfidence.Series["0.4-0.6"].Points.Add(confidenceSmaller06);
                chartConfidence.Series["0.6-0.8"].Points.Add(confidenceSmaller08);
                chartConfidence.Series["0.8-1"].Points.Add(confidenceSmaller10);

                //tables
                for (int x = 0; x < 9; x++)
                {
                    string s = ""; string row = ""; string col = ""; string block = "";
                    for (int y = 0; y < 9; y++)
                    {
                        s += azimuth2D[x, y] + ", ";
                        row += row2D[x, y] + ", ";
                        col += column2D[x, y] + ", ";
                        block += block2D[x, y] + ", ";
                    }
                    lbDigitSelection.Items.Add(s);
                    lbRowSelection.Items.Add(row);
                    lbColumnSelection.Items.Add(col);
                    lbBlockSelection.Items.Add(block);
                }
            }
        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {
            if(fc!=null || sudokuGrid != null)
            {
                nrOfSteps++;
                Step();
            }
            else
            {
                MessageBox.Show("Please enter a puzzle!");
            }
           
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            thread = new Thread(Start);
            thread.Start();           
        }

        private void Start()
        {
            bool isDone = false;
            while (!isDone)
            {
                if (fc != null && sudokuGrid != null)
                {
                    //inCriticalSection = true;
                    nrOfSteps++;
                    Rrr rrr = new Rrr(fc);
                    Rrr intermediate = rrr.GetRrrByANumberOfCycles(rrr, nrOfSteps);
                    if (intermediate.IsReady() == false)
                    {
                        SetEmptyListBox(lbDigitSelection);
                        SetEmptyListBox(lbRowSelection);
                        SetEmptyListBox(lbColumnSelection);
                        SetEmptyListBox(lbBlockSelection);

                        FourCube equalized4Cube = intermediate.GetFourCubeAfter1RRRCycle().Equalizer();
                        FourCube selector4Cube = equalized4Cube.Selector();
                        int[,] azimuth2D = new int[9, 9];
                        int[,] row2D = new int[9, 9];
                        int[,] column2D = new int[9, 9];
                        int[,] block2D = new int[9, 9];

                        double[,] confidences2D = new double[9, 9];
                        int[] azimuth1D = new int[81];
                        double[] confidences1D = new double[81];
                        int confidenceSmaller02 = 0;
                        int confidenceSmaller04 = 0;
                        int confidenceSmaller06 = 0;
                        int confidenceSmaller08 = 0;
                        int confidenceSmaller10 = 0;

                        for (int row = 0; row < 9; row++)
                        {
                            for (int col = 0; col < 9; col++)
                            {
                                for (int azimuth = 0; azimuth < 9; azimuth++)
                                {
                                    if (selector4Cube.GetCubeCells()[3, row, col, azimuth].GetConfidence() == 1)
                                    {
                                        azimuth2D[row, col] = azimuth + 1;
                                        confidences2D[row, col] = equalized4Cube.GetCubeCells()[3, row, col, azimuth].GetConfidence();
                                    }
                                    if (selector4Cube.GetCubeCells()[0, row, col, azimuth].GetConfidence() == 1)
                                    {
                                        column2D[row, col] = azimuth + 1;
                                    }
                                    if (selector4Cube.GetCubeCells()[1, row, col, azimuth].GetConfidence() == 1)
                                    {
                                        row2D[row, col] = azimuth + 1;
                                    }
                                    if (selector4Cube.GetCubeCells()[2, row, col, azimuth].GetConfidence() == 1)
                                    {
                                        block2D[row, col] = azimuth + 1;
                                    }
                                }
                            }
                        }

                        for (int x = 0; x < 9; x++)
                        {
                            for (int y = 0; y < 9; y++)
                            {
                                azimuth1D[9 * x + y] = azimuth2D[x, y];
                                confidences1D[9 * x + y] = confidences2D[x, y];
                            }
                        }

                        for (int i = 1; i <= 81; i++)
                        {
                            if (this.Controls.ContainsKey("tb" + (i).ToString()))
                            {
                                TextBox tb = this.Controls["tb" + i.ToString()] as TextBox;
                                SetText(tb, azimuth1D[i - 1].ToString());
                                if (confidences1D[i - 1] > 0 && confidences1D[i - 1] <= 0.2)
                                { SetColor(tb, Color.Pink); confidenceSmaller02++; }
                                else if (confidences1D[i - 1] > 0.2 && confidences1D[i - 1] <= 0.4)
                                { SetColor(tb, Color.Red); confidenceSmaller04++; }
                                else if (confidences1D[i - 1] > 0.4 && confidences1D[i - 1] <= 0.6)
                                { SetColor(tb, Color.Green); confidenceSmaller06++; }
                                else if (confidences1D[i - 1] > 0.6 && confidences1D[i - 1] <= 0.8)
                                { SetColor(tb, Color.Blue); confidenceSmaller08++; }
                                else if (confidences1D[i - 1] > 0.8 && confidences1D[i - 1] <= 1)
                                { SetColor(tb, Color.Black); confidenceSmaller10++; }
                            }
                        }


                        //chart
                        SetChart(chartDifference, "Differences from the previous step", intermediate.DifferenceCellsFromLastStep());
                        //chartLine.Series["Differences from the solution"].Points.Add(intermediate.DifferenceCellsFromSolution());

                        SetChart(chartConfidence, "0-0.2", confidenceSmaller02);
                        SetChart(chartConfidence, "0.2-0.4", confidenceSmaller04);
                        SetChart(chartConfidence, "0.4-0.6", confidenceSmaller06);
                        SetChart(chartConfidence, "0.6-0.8", confidenceSmaller08);
                        SetChart(chartConfidence, "0.8-1", confidenceSmaller10);

                        //tables
                        for (int x = 0; x < 9; x++)
                        {
                            string s = ""; string row = ""; string col = ""; string block = "";
                            for (int y = 0; y < 9; y++)
                            {
                                s += azimuth2D[x, y] + ", ";
                                row += row2D[x, y] + ", ";
                                col += column2D[x, y] + ", ";
                                block += block2D[x, y] + ", ";
                            }
                            SetListBox(lbDigitSelection, s);
                            SetListBox(lbRowSelection, row);
                            SetListBox(lbColumnSelection,col);
                            SetListBox(lbBlockSelection, block);
                        }
                        isDone = intermediate.IsReady();
                        Thread.Sleep(timeSpan);
                    }
                    //inCriticalSection = false;
                }
                else
                {
                    MessageBox.Show("Please enter a puzzle");
                }
            }
           
        }

        private void btnPreset1_Click(object sender, EventArgs e)
        {
            ClearSudokuGrid();
            tb2.Text = "1"; tb3.Text = "2";tb13.Text = "3"; tb15.Text = "6";tb16.Text = "5";
            tb17.Text = "4";tb21.Text = "4";
            tb24.Text = "7"; tb26.Text = "3";
            tb29.Text = "5";tb33.Text = "8";tb34.Text = "1";tb35.Text = "2";
            tb39.Text = "6";tb40.Text = "7";
            tb51.Text = "1";tb52.Text = "6";tb53.Text = "8";
            tb56.Text = "3";tb57.Text = "7";tb60.Text = "9";tb61.Text = "2"; ;tb62.Text = "6";
            tb66.Text = "5";tb71.Text = "1";
            tb74.Text = "9";tb75.Text = "1";tb76.Text = "5";tb78.Text = "2";tb79.Text = "3";
            EnterPuzzle();
        }

        private void ClearSudokuGrid()
        {
            for (int i = 1; i <= 81; i++)
            {
                if (this.Controls.ContainsKey("tb" + i.ToString()))
                {
                    TextBox tb = this.Controls["tb" + i.ToString()] as TextBox;
                    tb.Text = "";
                }
            }
            nrOfSteps = 0;
            sudokuGrid = null;
            fc = null;
            foreach (var series in chartDifference.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chartConfidence.Series)
            {
                series.Points.Clear();
            }
            lbDigitSelection.Items.Clear();
            lbRowSelection.Items.Clear();
            lbColumnSelection.Items.Clear();
            lbBlockSelection.Items.Clear();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearSudokuGrid();        
        }

        private void btnPreset2_Click(object sender, EventArgs e)
        {
            ClearSudokuGrid();
            //inkala
            tb1.Text = "8";
            tb12.Text = "3"; tb13.Text = "6";
            tb20.Text = "7";tb23.Text = "9";tb25.Text = "2";
            tb29.Text = "5"; tb33.Text="7";
            tb41.Text = "4";tb42.Text = "5";tb43.Text = "7";
            tb49.Text = "1";tb53.Text = "3";
            tb57.Text = "1";tb62.Text = "6";tb63.Text = "8";
            tb66.Text = "8";tb67.Text = "5";tb71.Text = "1";
            tb74.Text = "9";tb79.Text = "4";
            EnterPuzzle();
        }

        private void btnPreset3_Click(object sender, EventArgs e)
        {
            ClearSudokuGrid();
            tb2.Text = "9"; tb3.Text = "7";tb6.Text = "8";tb8.Text = "6";
            tb14.Text = "1";
            tb21.Text = "6";tb22.Text = "4";tb23.Text = "7";
            tb28.Text = "6";tb29.Text = "8";
            tb39.Text = "2";tb41.Text = "5";tb43.Text = "6";
            tb49.Text = "8";tb53.Text = "7";tb54.Text = "5";
            tb59.Text = "9";tb60.Text = "3";tb61.Text = "8";
            tb68.Text = "2";
            tb74.Text = "4"; tb76.Text = "1";tb79.Text = "9";tb80.Text = "2";
            EnterPuzzle();
        }

        private void btnPreviousStep_Click(object sender, EventArgs e)
        {
            if (fc != null || sudokuGrid != null)
            {
               
                if (nrOfSteps > 0)
                {
                    nrOfSteps--;

                    lbDigitSelection.Items.Clear();
                    lbRowSelection.Items.Clear();
                    lbColumnSelection.Items.Clear();
                    lbBlockSelection.Items.Clear();

                    Rrr rrr = new Rrr(fc);
                    Rrr intermediate = rrr.GetRrrByANumberOfCycles(rrr, nrOfSteps);
                    FourCube equalized4Cube = intermediate.GetFourCubeAfter1RRRCycle().Equalizer();
                    FourCube selector4Cube = equalized4Cube.Selector();
                    int[,] row2D = new int[9, 9];
                    int[,] column2D = new int[9, 9];
                    int[,] block2D = new int[9, 9];
                    int[,] azimuth2D = new int[9, 9];
                    double[,] confidences2D = new double[9, 9];
                    int[] azimuth1D = new int[81];
                    double[] confidences1D = new double[81];

                    for (int row = 0; row < 9; row++)
                    {
                        for (int col = 0; col < 9; col++)
                        {
                            for (int azimuth = 0; azimuth < 9; azimuth++)
                            {
                                if (selector4Cube.GetCubeCells()[3, row, col, azimuth].GetConfidence() == 1)
                                {
                                    azimuth2D[row, col] = azimuth + 1;
                                    confidences2D[row, col] = equalized4Cube.GetCubeCells()[3, row, col, azimuth].GetConfidence();
                                }
                                if (selector4Cube.GetCubeCells()[0, row, col, azimuth].GetConfidence() == 1)
                                {
                                    column2D[row, col] = azimuth + 1;
                                }
                                if (selector4Cube.GetCubeCells()[1, row, col, azimuth].GetConfidence() == 1)
                                {
                                    row2D[row, col] = azimuth + 1;
                                }
                                if (selector4Cube.GetCubeCells()[2, row, col, azimuth].GetConfidence() == 1)
                                {
                                    block2D[row, col] = azimuth + 1;
                                }
                            }
                        }
                    }



                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            azimuth1D[9 * x + y] = azimuth2D[x, y];
                            confidences1D[9 * x + y] = confidences2D[x, y];
                        }
                    }

                    for (int i = 1; i <= 81; i++)
                    {
                        if (this.Controls.ContainsKey("tb" + (i).ToString()))
                        {
                            TextBox tb = this.Controls["tb" + i.ToString()] as TextBox;
                            tb.Text = azimuth1D[i - 1].ToString();
                            if (confidences1D[i - 1] > 0 && confidences1D[i - 1] <= 0.2) { tb.ForeColor = Color.Pink; }
                            else if (confidences1D[i - 1] > 0.2 && confidences1D[i - 1] <= 0.4) { tb.ForeColor = Color.Red; }
                            else if (confidences1D[i - 1] > 0.4 && confidences1D[i - 1] <= 0.6) { tb.ForeColor = Color.Green; }
                            else if (confidences1D[i - 1] > 0.6 && confidences1D[i - 1] <= 0.8) { tb.ForeColor = Color.Blue; }
                            else if (confidences1D[i - 1] > 0.8 && confidences1D[i - 1] <= 1) { tb.ForeColor = Color.Black; }

                        }
                    }

                    if (nrOfSteps >= chartDifference.Series["Differences from the previous step"].Points.Count)
                    {
                        chartDifference.Series["Differences from the previous step"].Points.RemoveAt(nrOfSteps - 1);
                        chartConfidence.Series["0-0.2"].Points.RemoveAt(nrOfSteps - 1);
                        chartConfidence.Series["0.2-0.4"].Points.RemoveAt(nrOfSteps - 1);
                        chartConfidence.Series["0.4-0.6"].Points.RemoveAt(nrOfSteps - 1);
                        chartConfidence.Series["0.6-0.8"].Points.RemoveAt(nrOfSteps - 1);
                        chartConfidence.Series["0.8-1"].Points.RemoveAt(nrOfSteps - 1);
                    }
                    else
                    {
                        chartDifference.Series["Differences from the previous step"].Points.RemoveAt(nrOfSteps);
                        chartConfidence.Series["0-0.2"].Points.RemoveAt(nrOfSteps);
                        chartConfidence.Series["0.2-0.4"].Points.RemoveAt(nrOfSteps);
                        chartConfidence.Series["0.4-0.6"].Points.RemoveAt(nrOfSteps);
                        chartConfidence.Series["0.6-0.8"].Points.RemoveAt(nrOfSteps);
                        chartConfidence.Series["0.8-1"].Points.RemoveAt(nrOfSteps);
                   }
               
                    //tables
                    for (int x = 0; x < 9; x++)
                    {
                        string s = ""; string row = ""; string col = ""; string block = "";
                        for (int y = 0; y < 9; y++)
                        {
                            s += azimuth2D[x, y] + ", ";
                            row += row2D[x, y] + ", ";
                            col += column2D[x, y] + ", ";
                            block += block2D[x, y] + ", ";
                        }
                        lbDigitSelection.Items.Add(s);
                        lbRowSelection.Items.Add(row);
                        lbColumnSelection.Items.Add(col);
                        lbBlockSelection.Items.Add(block);
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Please enter a puzzle");
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void lbColumnSelection_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        delegate void SetTextCallback(TextBox tb,string text);

        private void SetText(TextBox tb, string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (tb.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { tb, text });
            }
            else
            {
                tb.Text = text;
            }
        }

        delegate void SetColorCallback(TextBox tb, Color c);

        private void SetColor(TextBox tb, Color c)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (tb.InvokeRequired)
            {
                SetColorCallback d = new SetColorCallback(SetColor);
                this.Invoke(d, new object[] { tb, c });
            }
            else
            {
                tb.ForeColor = c;
            }
        }


        delegate void SetListBoxCallBack(ListBox lb, string s);

        private void SetListBox(ListBox lb, string s)
        {      
            if (lb.InvokeRequired)
            {
                SetListBoxCallBack d = new SetListBoxCallBack(SetListBox);
                this.Invoke(d, new object[] { lb, s });
            }
            else
            {
                lb.Items.Add(s);
            }
        }

        delegate void SetEmptyListBoxCallBack(ListBox lb);

        private void SetEmptyListBox(ListBox lb)
        {
            if (lb.InvokeRequired)
            {
                SetEmptyListBoxCallBack d = new SetEmptyListBoxCallBack(SetEmptyListBox);
                this.Invoke(d, new object[] { lb });
            }
            else
            {
                lb.Items.Clear();
            }
        }

        delegate void SetChartCallBack(Chart c,string s,int i);

        private void SetChart(Chart c,string s, int i)
        {
            if (c.InvokeRequired)
            {
                SetChartCallBack d = new SetChartCallBack(SetChart);
                this.Invoke(d, new object[] { c,s,i });
            }
            else
            {
                c.Series[s].Points.Add(i);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            thread.Abort();           
        }

        private void trackBarSudoku_Scroll(object sender, EventArgs e)
        {
            timeSpan = TimeSpan.FromMilliseconds(trackBarSudoku.Value);
        }
     
    }
}
