using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpreadsheetApp
{
    public partial class search : Form
    {
        public string strToSearch;
        public search()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            strToSearch = textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
