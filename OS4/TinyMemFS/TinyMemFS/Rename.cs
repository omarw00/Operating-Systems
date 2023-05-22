using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TinyMemFS
{
    public partial class Rename : Form
    {
        public string fileName;
        public string newName;
        public Rename()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fileName = textBox1.Text;
            newName = textBox2.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
