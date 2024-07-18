using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThunderbirdFixMailsHeaders
{
    public partial class Form1 : Form
    {
        private DateTime dtstart;
        private List<string> files = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbFile.Text = openFileDialog1.FileName;
                rbFile.Checked = true;
            }
        }

        private void btFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbFolder.Text = folderBrowserDialog1.SelectedPath;
                rbFolder.Checked = true;
            }
        }

        private void SL(string message, bool newline = false, bool datetime = false)
        {
            if (message == "")
            {
                return;
            }
            if (datetime)
            {
                tbStatus.Invoke((MethodInvoker)delegate
                {
                    tbStatus.AppendText(DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" +
                        DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" +
                        DateTime.Now.Second.ToString().PadLeft(2, '0') + " ");
                });
            }
            tbStatus.Invoke((MethodInvoker)delegate
            {
                tbStatus.AppendText(message);
            });
            if (newline)
            {
                tbStatus.Invoke((MethodInvoker)delegate
                {
                    tbStatus.AppendText(Environment.NewLine);
                });
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //fix
            if (backgroundWorker1.CancellationPending == false)
            {
                SL("Start fix:", true, true);
                backgroundWorker1.ReportProgress(0);
                long count, size;
                for (int i = 0; i < files.Count; i++)
                {
                    if (backgroundWorker1.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        groupBox1.Invoke((MethodInvoker)delegate
                        {
                            groupBox1.Text = "Status (file: " + (i + 1) + "/" + files.Count + "):";
                        });
                        SL("- file: " + files[i], true);
                        if (File.Exists(files[i] + ".original"))
                        {
                            SL("Skip file: " + files[i] + " (.original exist!)", true);
                            continue;
                        }
                        else
                        {
                            File.Move(files[i], files[i] + ".original");
                        }
                        using (StreamReader sr = new StreamReader(files[i] + ".original"))
                        {
                            count = 1;
                            size = sr.BaseStream.Length;
                            using (StreamWriter sw = new StreamWriter(files[i]))
                            {
                                while (!sr.EndOfStream)
                                {
                                    int c = sr.Read();
                                    if (c == '\r' && sr.Peek() == '\r')
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        sw.Write((char)c);
                                    }
                                    count++;
                                    if (count % 1000000 == 0)
                                    {
                                        backgroundWorker1.ReportProgress((int)(count * 100 / size));
                                    }
                                }
                            }
                        }
                        if (File.Exists(files[i] + ".msf"))
                        {
                            File.Delete(files[i] + ".msf");
                        }
                        File.Delete(files[i] + ".original");
                    }
                }
                SL("Time progress: " + TimeSpan.FromSeconds((DateTime.Now - this.dtstart).TotalSeconds).ToString(@"hh\:mm\:ss"), true, true);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btStart.Text = "Start";
            groupBox1.Text = "Status:";
            btStart.Enabled = true;
            progressBar1.Value = 100;
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false)
            {
                if (MessageBox.Show("Before you start backup your files! Continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
                this.files = new List<string>();
                //file
                if (rbFile.Checked)
                {
                    if (tbFile.Text == "")
                    {
                        MessageBox.Show("Please select file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        files.Add(tbFile.Text);
                    }
                }
                //folder
                if (rbFolder.Checked)
                {
                    if (tbFolder.Text == "")
                    {
                        MessageBox.Show("Please select folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        foreach (string file0 in Directory.GetFiles(tbFolder.Text).Select(Path.GetFileName))
                        {
                            if (file0.Contains(".") == false)
                            {
                                files.Add(tbFolder.Text + "\\" + file0);
                            }
                        }
                    }
                }
                //program is running?
                if (Process.GetProcessesByName("Thunderbird").Length > 0)
                {
                    MessageBox.Show("Please close Thunderbird.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //list files
                SL("Selected files:", true, true);
                foreach (string file0 in files)
                {
                    SL("- " + file0, true);
                }
                dtstart = DateTime.Now;
                progressBar1.Value = 0;
                tbStatus.Text = string.Empty;
                btStart.Text = "Cancel";
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                backgroundWorker1.CancelAsync();
                btStart.Enabled = false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:kamil@kbejt.pl?subject=Thunderbird%20Fix%20Mails%20Headers");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/kamilmroczkowski/ThunderbirdFixMailsHeaders");
        }
    }
}
