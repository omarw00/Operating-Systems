using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpreadsheetApp
{
    public partial class getCell : Form
    {
        public int row;
        public int col;
        public getCell()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            row = Int32.Parse(textBox1.Text);
            col = Int32.Parse(textBox2.Text);
            this.DialogResult = DialogResult.OK;
        }
    }
}
