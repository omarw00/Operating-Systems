using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinyMemFS
{
    public partial class Form1 : Form
    {
        TinyMemFS FS = new TinyMemFS();
        public Form1()
        {
            InitializeComponent();
            listBox1.Items.Add("File Name");
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            add addbutton = new add();
            if (addbutton.ShowDialog() == DialogResult.OK)
            {
                string fileName = addbutton.fileName;
                string fileToAdd = addbutton.fileToAdd;
                bool b = FS.add(fileName, fileToAdd);
                if (b == true)
                {
                    listBox1.Items.Add(fileName);
                }

            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Remove remove = new Remove();
            if (remove.ShowDialog() == DialogResult.OK)
            {
                string fileName = remove.fileName;
                bool b = FS.remove(fileName);
                if (b)
                {
                    listBox1.Items.Remove(fileName);
                }
            }
        }

        private void listFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<String> files = FS.listFiles();
            string str = "";
            foreach(string s in files) {
                str += s + "\n";
            }
            if (str == "")
            {
                MessageBox.Show("File System is empty!");
            }
            else
                MessageBox.Show(str);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save save = new Save();
            if (save.ShowDialog() == DialogResult.OK)
            {
                string fileName = save.fileName;
                string fileToAdd = save.fileToAdd;
                bool b = FS.save(fileName, fileToAdd);
            }
        }

        private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Encrypt encrypt = new Encrypt();
            if (encrypt.ShowDialog() == DialogResult.OK)
            {
                string key = encrypt.key;
                bool b = FS.encrypt(key);
                if (!b)
                {
                    MessageBox.Show("encryption failed!");
                }
            }
        }

        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Decrypt decrypt = new Decrypt();
            if (decrypt.ShowDialog() == DialogResult.OK)
            {
                string key = decrypt.key;
                bool b = FS.decrypt(key);
                if (!b)
                {
                    MessageBox.Show("decryption failed!");
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rename rename = new Rename();
            if (rename.ShowDialog() == DialogResult.OK)
            {
                string file1 = rename.fileName;
                string file2 = rename.newName;
                bool b = FS.rename(file1, file2);
                if (!b)
                    MessageBox.Show("rename operation failed");
                else
                {
                    listBox1.Items.Remove(file1);
                    listBox1.Items.Add(file2);
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy copy = new Copy();
            if (copy.ShowDialog() == DialogResult.OK)
            {
                string file1 = copy.fileName;
                string file2 = copy.newName;
                bool b = FS.copy(file1, file2);
                if (!b)
                    MessageBox.Show("Operation failed!");
                else
                    MessageBox.Show(file1 + " content has been copied to " + file2);
            }
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Compare comp = new Compare();
            if (comp.ShowDialog() == DialogResult.OK)
            {
                string file1 = comp.fileName;
                string file2 = comp.newName;
                bool b = FS.compare(file1, file2);
                if (b)
                {
                    MessageBox.Show("2 files are equal");
                }
                else
                    MessageBox.Show("2 files are not equal");
            }
        }
    }
}
