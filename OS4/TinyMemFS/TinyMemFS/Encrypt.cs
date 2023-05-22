using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TinyMemFS
{
    public partial class Encrypt : Form
    {
        public string key;
        public Encrypt()
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
