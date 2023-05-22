using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TinyMemFS
{
    public partial class add : Form
    {
        public string fileName;
        public string fileToAdd;
        public add()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            fileName = textBox1.Text;
            fileToAdd = textBox2.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
