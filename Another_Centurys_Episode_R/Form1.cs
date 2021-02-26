using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Another_Centurys_Episode_R
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }
        private Thread mth;

        private void button1_Click(object sender, EventArgs e)
        {
            mth = new Thread(new ThreadStart(prossfile));
            mth.IsBackground = true;
            mth.Start();
        }

        private void prossfile()
        {
            BDTFILE bdt = new BDTFILE();
            MODELS mod = new MODELS();
            TPFFILE tpf = new TPFFILE();
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                bool testmode = false;
                //testmode = true;
                if (testmode)
                {
                    try
                    {
                        string infile = listBox1.Items[i].ToString();
                        if (Path.GetExtension(infile) == ".bnd")
                            BNDFILE.loadBND(infile, textBox1.Text);
                        else if (Path.GetExtension(infile) == ".bdt")
                            bdt.init(infile, textBox1.Text, checkBox1.Checked);
                        else if (Path.GetExtension(infile) == ".flver")
                            mod.expModel(infile, textBox1.Text, checkBox1.Checked);
                        else if (Path.GetExtension(infile) == ".tpf")
                            tpf.init(infile, textBox1.Text,checkBox1.Checked);
                    }
                    catch
                    {
                        textBox2.Text = textBox2.Text + "\r\n" + ("error in ：" + listBox1.Items[i].ToString());
                        //MessageBox.Show("error in ：" + listBox1.Items[i].ToString());
                    }
                }
                else
                {
                    string infile = listBox1.Items[i].ToString();
                    if (Path.GetExtension(infile) == ".bnd")
                        BNDFILE.loadBND(infile, textBox1.Text);
                    else if (Path.GetExtension(infile) == ".bdt")
                        bdt.init(infile, textBox1.Text, checkBox1.Checked);
                    else if (Path.GetExtension(infile) == ".flver")
                        mod.expModel(infile, textBox1.Text, checkBox1.Checked);
                    else if (Path.GetExtension(infile) == ".tpf")
                        tpf.init(infile, textBox1.Text, checkBox1.Checked);
                }
                progressBar1.Value = i + 1;
                label1.Text = (i + 1).ToString() + "/" + progressBar1.Maximum;
                // Application.DoEvents();
            }
            button1.Enabled = true;
            MessageBox.Show("OK");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.AddRange(openFileDialog1.FileNames);
                label1.Text = "0/" + listBox1.Items.Count.ToString();
                progressBar1.Maximum = listBox1.Items.Count;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
                listBox1.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pallet t = new Pallet();
            t.Show();
        }
    }
}
