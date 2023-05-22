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

namespace ProcessManagmentApp
{
    public partial class Form1 : Form
    {
        String Details = "{0,-20}{1,-20}";
        public Form1()
        {
            InitializeComponent();
        }
        Process[] CurrProcesses = Process.GetProcesses();

        private void GetCurrProccesses()
        {
            String ID;
            String PName;
            listBox1.Items.Clear();
            foreach (Process p in CurrProcesses)
            {

                ID = "" + p.Id + "";
                PName = p.ProcessName;

                listBox1.Items.Add(String.Format(Details, ID, PName));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            String ID;
            String PName;
            foreach (Process p in CurrProcesses)
            {
                
                ID = "" + p.Id + "";
                PName = p.ProcessName;

                listBox1.Items.Add(String.Format(Details,ID,PName));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Add(String.Format(Details, "Process ID ", "Process Name"));
            GetCurrProccesses();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex < 1)
            {
                MessageBox.Show("Please select a Process to kill before clicking the 'Kill' button!");
            }
            else
            {
                CurrProcesses[listBox1.SelectedIndex].Kill();
                GetCurrProccesses();

           
            }
        }
    }
}
