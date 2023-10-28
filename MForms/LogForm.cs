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
            toolTip1.SetToolTip(this.RefreshButton, "Update the contents");
            toolTip1.SetToolTip(this.ClearButton, "Clear the log");
            toolTip1.SetToolTip(this.SaveButton, "Save as .log file");
        }

        /// <summary>
        /// Updates the tbLog textbox whenever something is changed from the log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            tbLog_TextChanged(sender, e);
        }

        /// <summary>
        /// Reset variable on close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogForm_Close(object sender, FormClosingEventArgs e)
        {
            MForms.FormControl.ThreadLogProState = false;
        }

        /// <summary>
        /// Write the log as file and open it using default application when save button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (opWxm.Checked)
            {
                string pathToWxMaxima = ControlObjects.Translator.GetMaxima().GetPathToMaximaAbs().Replace("maxima.bat","wxmaxima.exe");
                await OpenFileWithProgramAsync(pathToWxMaxima, MaximaSocket.WriteWXM());
                SaveButton.Enabled = false;
            }
            else
            {
                SharedFunctions.WriteDataToFile(ControlObjects.Translator.GetMaxima().WorkingFolderPath() + "\\" + "Maxima.log", tbLog.Text);
                System.Diagnostics.Process.Start(ControlObjects.Translator.GetMaxima().WorkingFolderPath() + "\\" + "Maxima.log");
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (AlwaysOnTopCB.Checked)
                this.TopMost = true;
            else
                this.TopMost = false;
        }

        /// <summary>
        /// Clear the log and updates the tbLog textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            session.socket.ClearFullLog("Cleared by request from Log Viewer\n");
            MaximaPlugin.MaximaSocket.wxmLog = "";
            MaximaPlugin.ControlObjects.Translator.Log = "Cleared by request from Log Viewer\n";
            tbLog_TextChanged(sender, e);
        }

        /// <summary>
        /// Change the description of the controls when radio button is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void opWxm_CheckedChanged(object sender, EventArgs e)
        {
            if (opWxm.Checked)
            {
                SaveButton.Text = "Open in wxMaxima";
                toolTip1.SetToolTip(this.SaveButton, "Execute the complete session in wxMaxima");
            }
            else
            { 
                SaveButton.Text = "Save";
                toolTip1.SetToolTip(this.SaveButton, "Save as .log file");
            }
        }

        #region helper function

        /// <summary>
        /// Open wxm file using the defined programPath
        /// </summary>
        /// <param name="programPath"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Re-enable the Save button upon wxMaxima closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Process_Exited(object sender, EventArgs e)
        {
            // This code runs when the process exits
            Invoke(new Action(() => {
                SaveButton.Enabled = true;
            }));

            // Clean up event handler
            process.Exited -= Process_Exited;
            process.Dispose();
        }

        #endregion
    }
}
