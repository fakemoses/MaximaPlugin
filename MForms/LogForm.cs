using MaximaPlugin.ControlObjects;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaximaPlugin.MForms
{
    public partial class LogForm : Form
    {
        MaximaSession session;
        private Process process;
        ToolTip toolTip1;
        public LogForm()
        {
            InitializeComponent();
            session = ControlObjects.Translator.GetMaxima();
            ControlObjects.Translator.GetMaxima().StartSession();

            // add new event to update everytime changes occurs instead of relying on isFocused event
            Translator.LogChanged += tbLog_TextChanged;

            toolTip1 = new ToolTip();
            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;
            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.opMaLog, "Input and output strings of the Maxima session");
            toolTip1.SetToolTip(this.opFuLog, "Step by step translation and processing");
            toolTip1.SetToolTip(this.opWxm, "wxMaxima file with all input sent to Maxima");
            toolTip1.SetToolTip(this.button1, "Update the contents");
            toolTip1.SetToolTip(this.button2, "Clear the log");
            toolTip1.SetToolTip(this.button3, "Save as .log file");
        }
        private void tbLog_TextChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                Invoke((MethodInvoker)delegate
                {
                    if (opFuLog.Checked)
                    {
                        tbLog.Text = ControlObjects.Translator.Log.Replace("\n", "\r\n");
                    }
                    else if (opMaLog.Checked)
                    {
                        tbLog.Text = session.socket.fullLog.Replace("\n", "\r\n");
                    }
                    else if (opWxm.Checked)
                    {
                        tbLog.Text = MaximaPlugin.MaximaSocket.wxmLog;
                    }
                    tbLog.SelectionStart = tbLog.Text.Length;
                    tbLog.ScrollToCaret();
                });
            }
        }
        private void opFuLog_CheckedChanged(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void LogForm_Click(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void opMaLog_CheckedChanged(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void LogForm_Load(object sender, EventArgs e)
        {
            tbLog.ScrollBars = ScrollBars.Both;
            //maximaform = new AutoMaxima();
            tbLog_TextChanged(sender, e);
        }
        private void LogForm_Close(object sender, FormClosingEventArgs e)
        {
            MForms.FormControl.ThreadLogProState = false;
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            if (opWxm.Checked)
            {
                string pathToWxMaxima = ControlObjects.Translator.GetMaxima().GetPathToMaximaAbs().Replace("maxima.bat","wxmaxima.exe");
                await OpenFileWithProgramAsync(pathToWxMaxima, MaximaSocket.WriteWXM());
                button3.Enabled = false;
            }
            else
            {
                SharedFunctions.WriteDataToFile(ControlObjects.Translator.GetMaxima().WorkingFolderPath() + "\\" + "Maxima.log", tbLog.Text);
                System.Diagnostics.Process.Start(ControlObjects.Translator.GetMaxima().WorkingFolderPath() + "\\" + "Maxima.log");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                this.TopMost = true;
            else
                this.TopMost = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            session.socket.ClearFullLog("Cleared by request from Log Viewer\n");
            MaximaPlugin.MaximaSocket.wxmLog = "";
            MaximaPlugin.ControlObjects.Translator.Log = "Cleared by request from Log Viewer\n";
            tbLog_TextChanged(sender, e);
        }
        private void opWxm_CheckedChanged(object sender, EventArgs e)
        {
            if (opWxm.Checked)
            {
                button3.Text = "Open in wxMaxima";
                toolTip1.SetToolTip(this.button3, "Execute the complete session in wxMaxima");
            }
            else
            { 
                button3.Text = "Save";
                toolTip1.SetToolTip(this.button3, "Save as .log file");
            }
        }

        private Task OpenFileWithProgramAsync(string programPath, string filePath)
        {
            try
            {
                process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = programPath,
                    Arguments = filePath,
                    UseShellExecute = true
                };
                process.StartInfo = startInfo;

                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;

                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            // This code runs when the process exits
            Invoke(new Action(() => {
                button3.Enabled = true;
            }));

            // Clean up event handler
            process.Exited -= Process_Exited;
            process.Dispose();
        }
    }
}
