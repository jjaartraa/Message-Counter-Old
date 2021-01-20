using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Globalization;
using System.IO;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        String ConvName;
        int ConvCount;
        string[] directories;


        private void CountB_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "")
            {
                try
                {
                    directories = Directory.GetDirectories(folderBrowserDialog1.SelectedPath + "\\messages\\inbox");
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid Directory");
                }

                CountB.Visible = false;
                CountB.Enabled = false;
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                progressBar1.Maximum = directories.Count();
                label1.Visible = true;
                label1.BringToFront();

                if (backgroundWorker1.IsBusy == false)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void LocateMessagesInHTML(string path)
        {
            string[] messages = Directory.GetFiles(path,"message_*.html");
            try
            {
                ConvName = null;
                ConvCount = 0;

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(path);
                ConvName = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
                byte[] bytes = Encoding.Default.GetBytes(ConvName);
                ConvName = Encoding.UTF8.GetString(bytes);
                this.Invoke((MethodInvoker)delegate
                { label1.Text = "Loading:  " + ConvName; });
                var CountNode = doc.DocumentNode.SelectNodes("//body/div/div/div");
                FindNode(CountNode);
                AddStats(ConvName, ConvCount);
            }
            catch (Exception) { }
        }

        private void Copyright_Click(object sender, EventArgs e)
        {
            if (Copyright.ForeColor == SystemColors.Control)
                Copyright.ForeColor = SystemColors.ControlText;
            else
                Copyright.ForeColor = SystemColors.Control;
        }

        private void AddStats(string name = "Name not found", int count = 0)
        {
            string ForCount = count.ToString("### ###");

            Button btn = new Button
            {
                Font = new Font("Arial Unicode MS", 8.25f),
                Name = name,
                Tag = count,
                Text = name + Environment.NewLine + "Messages:" + Environment.NewLine + ForCount,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5, 5, 5, 5),
                Height = 75,
                Width = 165,
                ForeColor = Color.Black,
                BackColor = Color.White,
                Padding = new Padding(15, 0, 0, 0)
            };
            this.Invoke((MethodInvoker)delegate
            {
                label1.Text = "Loading:  " + ConvName;
                flowLayoutPanel1.Controls.Add(btn);
                if (flowLayoutPanel1.VerticalScroll.Visible)
                {
                    flowLayoutPanel1.Width = 710 + 15;
                    flowLayoutPanel1.Padding = new Padding(4, 0, 4, 0);
                }
                else
                    flowLayoutPanel1.Width = 710;
                var mySortedList = flowLayoutPanel1.Controls.OfType<Button>().OrderBy(x => -Convert.ToInt32(x.Tag)).ToList();
                flowLayoutPanel1.Controls.AddRange(mySortedList.ToArray());
                flowLayoutPanel1.Refresh();
            });
        }

        private void FindNode(HtmlNodeCollection nodes)
        {
            foreach (var node in nodes)
            {
                try
                {
                    if (node.Attributes["role"].Value == "main")
                        ConvCount = node.Elements("div").Count();
                    break;
                }
                catch { }
                if (node.HasChildNodes)
                    FindNode(node.ChildNodes);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var folder in directories)
            {
                LocateMessagesInHTML(folder + "\\message.html");
                backgroundWorker1.ReportProgress(ConvCount);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = progressBar1.Value + 1;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Visible = false;
            label1.Visible = false;
            CountB.Visible = true;
            CountB.Enabled = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://paypal.me/jjaartraa");
        }
    }
}
