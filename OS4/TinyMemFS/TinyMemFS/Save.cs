using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TinyMemFS
{
    public partial class Save : Form
    {
        public string fileName;
        public string fileToAdd;
        public Save()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fileName = textBox1.Text;
            fileToAdd = textBox2.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
