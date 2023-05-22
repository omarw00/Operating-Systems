using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TinyMemFS
{
    public partial class Decrypt : Form
    {
        public string key;
        public Decrypt()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            key = textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
