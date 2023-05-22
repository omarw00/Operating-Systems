using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetApp
{
    public partial class Form1 : Form
    {
        private SharableSpreadSheet spreadSheet;
        private OpenFileDialog openFileDialog;
        public Form1()
        {
            InitializeComponent();
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            dataGridView1.Visible = false;


        }
        //
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        // start spread sheet button
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = false;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;

        }
        // load spread sheet
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName != null)
                {
                    spreadSheet = new SharableSpreadSheet(5, 5);
                    spreadSheet.load(openFileDialog.FileName);
                }
                else
                {
                    spreadSheet = new SharableSpreadSheet(5, 5);
                }
            }
            showSheet();
            
            //
        }

        private void showSheet()
        {
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            dataGridView1.Visible = true;
            menuStrip1.Visible = true;
            editToolStripMenuItem.Visible = true;


            Tuple<int, int> size = spreadSheet.getSize();
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = size.Item2;
            for (int i = 0; i<size.Item2; i++)
            {
                dataGridView1.Columns[i].Name = i.ToString();

            }
            for (int i =0;i< size.Item1; i++)
            {
                dataGridView1.Rows.Add();
                for (int j = 0; j< size.Item2; j++)
                {
                    dataGridView1[j,i].Value = spreadSheet.getCell(i, j);
                }
            }
        }
        // start 
        private void button3_Click(object sender, EventArgs e)
        {
            int nRow = Int32.Parse(textBox1.Text);
            int nCol = Int32.Parse(textBox2.Text);
            int nUsers;
            if (textBox3.Text == "")
            {
                nUsers = -1;
            }
            else
                nUsers = Int32.Parse(textBox3.Text);
            if (nUsers <= 0 )
            {
                spreadSheet = new SharableSpreadSheet(nRow, nCol);
            }
            else
            {
                spreadSheet = new SharableSpreadSheet(nRow, nCol, nUsers);

            }
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            dataGridView1.Visible = true;
            editToolStripMenuItem.Visible = true;
            showSheet();

        }
        //load
        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName != null)
                {
                    spreadSheet = new SharableSpreadSheet(5, 5);
                    spreadSheet.load(openFileDialog.FileName);
                }
                else
                {
                    spreadSheet = new SharableSpreadSheet(5, 5);
                }
            }
            showSheet();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            if (save.ShowDialog() == DialogResult.OK)
            {
                spreadSheet.save(save.FileName);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName != null)
                {
                    spreadSheet = new SharableSpreadSheet(5, 5);
                    spreadSheet.load(openFileDialog.FileName);
                }
                else
                {
                    spreadSheet = new SharableSpreadSheet(5, 5);
                }
            }
            showSheet();
        }

        private void getCellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCell getCell = new getCell();
            if(getCell.ShowDialog() == DialogResult.OK)
            {
                int row = getCell.row;
                int col = getCell.col;
                string ret = spreadSheet.getCell(row, col);
                if (ret == null || ret == "") 
                {
                    MessageBox.Show("The cell: [" + row + "," + col + "] is empty!");
                }
                else
                {
                    MessageBox.Show("The cell: [" + row + "," + col + "] is: " + ret);
                }
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            search search = new search();
            if(search.ShowDialog() == DialogResult.OK)
            {
                String str = search.strToSearch;
                Tuple<int, int> tuple = spreadSheet.searchString(str);
                if(tuple == null)
                {
                    MessageBox.Show("the String: " + str + "is not found!");
                }
                else
                {
                    MessageBox.Show("the String: '" + str + "' is found in cell: [" + tuple.Item1 + "," + tuple.Item2 + "]" );
                    dataGridView1.CurrentCell = dataGridView1.Rows[tuple.Item1].Cells[tuple.Item2];
                }
            }
        }

        private void getSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tuple<int, int> size = spreadSheet.getSize();
            MessageBox.Show("The shareable spread sheet size is: Rows = " + size.Item1 + ", Columns = " + size.Item2);

        }

        private void setCellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setCell set = new setCell();
            if (set.ShowDialog() == DialogResult.OK)
            {
                spreadSheet.setCell(set.row, set.col, set.str);
            }
            showSheet();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                spreadSheet.setCell(e.RowIndex, e.ColumnIndex, dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString());

            }
            catch(Exception ex)
            {
                spreadSheet.setCell(e.RowIndex, e.ColumnIndex, "");

            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (spreadSheet != null)
            {
                DialogResult dr = MessageBox.Show("Do you want to save the Shared Spread Sheet?", " before Closing ", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    SaveFileDialog save = new SaveFileDialog();
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        spreadSheet.save(save.FileName);
                    }
                }
            }
        }

        private void dataGridView1_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                spreadSheet.setCell(e.RowIndex, e.ColumnIndex, dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString());

            }
            catch (Exception ex)
            {
                spreadSheet.setCell(e.RowIndex, e.ColumnIndex, "");

            }
        }
    }
}
