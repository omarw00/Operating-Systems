using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpreadsheetApp
{
    public partial class setCell : Form
    {
        public int row;
        public int col;
        public string str;
        public setCell()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            row = Int32.Parse(textBox1.Text);
            col = Int32.Parse(textBox2.Text);
            str = textBox3.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
