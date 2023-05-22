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
using System.Threading;

namespace ThrashingGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        Thread th;
        Boolean flag;
        private void button2_Click(object sender, EventArgs e)
        {
            flag = false;
            th.Abort();
        }

        private void openfunc()
        {
            while (flag)
            {
                int[,] matrix = new int[20000, 15000];
                for (int i = 10; i < 15000 && flag == true; i++)
                {
                    for (int j = 0; j < 20000 && flag == true; j++)
                    {
                        matrix[j, i] = i;
                        if (matrix[j, i] == 0)
                        {
                            matrix[j, i] = 20001;
                        }
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            flag = true;
            th = new Thread(openfunc);
            th.Start();
        }
    }
}
