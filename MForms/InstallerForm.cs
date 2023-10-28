using MaximaPlugin.MInstaller;
using SMath.Manager;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaximaPlugin.MForms
{
    public partial class InstallerForm : Form
    {
        private int installerProcessId;
        private Timer timer;
        private double scaleFactor = 1.0;

        /// <summary>
        /// constructor
        /// </summary>
        public InstallerForm()
        {
            InitializeComponent();

            scaleFactor = SharedFunctions.getScreenScaleFactor();

            // form panel visibilitsy
            StatusPanel.Visible = false;
            InstTypePanel.Visible = true;

            SilentInstRB.Checked = true;
            ManualInstRB.Checked = false;

            // Attach event handlers for radio buttons and Next button
            SilentInstRB.CheckedChanged += RadioButtonOption_CheckedChanged;
            ManualInstRB.CheckedChanged += RadioButtonOption_CheckedChanged;

            // windows scaling thing
            if (scaleFactor > 1.0 && scaleFactor < 1.5)
                this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(260 * scaleFactor));
            else if (scaleFactor >= 1.5)
                this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(260 * scaleFactor));
            else
                this.Size = new System.Drawing.Size(520, 260);

            //move Installer type panel to status panel location
            InstTypePanel.Location = StatusPanel.Location;
        }

        /// <summary>
        /// Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Initiate maxima session and close the form after installation is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishButton_Click(object sender, EventArgs e)
        {
            ControlObjects.Translator.GetMaxima().FoundNoPath();
            Close();
        }

        /// <summary>
        /// Event to set the radio buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonOption_CheckedChanged(object sender, EventArgs e)
        {
            // Only one radio button allowed
            if (sender == SilentInstRB && SilentInstRB.Checked)
            {
                ManualInstRB.Checked = false;
            }
            else if (sender == ManualInstRB && ManualInstRB.Checked)
            {
                SilentInstRB.Checked = false;
            }
        }

        /// <summary>
        /// When next button is click, move the controls to its respective positions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextButton_Click(object sender, EventArgs e)
        {
            InstProgressBar.Maximum = 100; // Set the maximum value of the progress bar
            InstProgressBar.Value = 0;
            timer = new Timer();
            timer.Interval = 1000; // Check every 1 second 
            timer.Tick += Timer_Tick;


            // Handle radio button changes here
            if (SilentInstRB.Checked)
            {
                if (scaleFactor > 1.0 && scaleFactor < 1.5)
                    this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(260 * scaleFactor));
                else if (scaleFactor >= 1.5)
                    this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(260 * scaleFactor));
                else
                    this.Size = new System.Drawing.Size(520, 260);
            }
            else if (ManualInstRB.Checked)
            {
                if (scaleFactor > 1.0 && scaleFactor < 1.5)
                    this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(200 * scaleFactor));
                else if (scaleFactor >= 1.5)
                    this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(200 * scaleFactor));
                else
                    this.Size = new System.Drawing.Size(520, 200);

                label3.Location = new System.Drawing.Point(0, 76);
                StatusLabel.Location = new System.Drawing.Point(46, 123);
                label5.Location = new System.Drawing.Point(0, 123);
                CancelButton.Location = new System.Drawing.Point(302, 103);
                FinishButton.Location = new System.Drawing.Point(383, 103);

                label2.Visible = false;
                InstProgressBar.Visible = false;
            }

            // form panel related init
            StatusPanel.Visible = true;
            InstTypePanel.Visible = false;

            ExtractUrlFromJson();
            FinishButton.Enabled = false;
        }

        #region helper functions
        /// <summary>
        /// extract url from Json
        /// </summary>
        public async void ExtractUrlFromJson()
        {
            StatusLabel.Text = "Downloading installer..";
            string url = "https://sourceforge.net/projects/maxima/best_release.json";
            string randomFileName = Path.GetRandomFileName();
            string installerFileName = $"Maxima_installer_{randomFileName}.exe";
            string installerPath = Path.Combine(Path.GetTempPath(), installerFileName);

            JsonDataFetcher dataFetcher = new JsonDataFetcher();
            string windowsUrl = await dataFetcher.GetWindowsReleaseUrl(url);

            if (!string.IsNullOrEmpty(windowsUrl))
            {
                await InstallMaxima(windowsUrl, installerPath);
            }
            else
            {
                MessageBox.Show("Unable to extract the release URL from the JSON. Check your internet connection");
                Close();
            }
        }

        /// <summary>
        /// Download and install maxima.
        /// </summary>
        /// <param name="windowsUrl"></param>
        /// <param name="installerPath"></param>
        /// <returns></returns>
        public async Task InstallMaxima(string windowsUrl, string installerPath)
        {

            //async download
            await Installer.DownloadInstaller(windowsUrl, installerPath, DlProgressBar);

            //check for silent installation
            bool silentInstall;
            if (SilentInstRB.Checked == true)
                silentInstall = true;
            else
                silentInstall = false;

            //start installation
            installerProcessId = Installer.RequestAdminPrivileges(installerPath, silentInstall);

            //disable cancel button at this point
            CancelButton.Enabled = false;

            if (installerProcessId > 0)
            {
                //start timer to track progress
                timer.Start();
                StatusLabel.Text = "The installation is in progress...";
            }
            else
            {
                MessageBox.Show("Failed to start the installation.");
            }
        }

        /// <summary>
        /// Timer tick event handler to check if the installer process has exited.
        /// Updates the installation progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Process installerProcess = Process.GetProcessById(installerProcessId);
                if (installerProcess.HasExited)
                {
                    // The installer process has exited
                    // Perform actions or display a message to indicate completion
                    StatusLabel.Text = "The installation has completed.";
                    InstProgressBar.Value = 100;
                    FinishButton.Enabled = true;

                    // Stop the timer
                    Timer timer = (Timer)sender;
                    timer.Stop();

                }
                else
                {
                    // The installer process is still running, update the progress bar
                    // based on the time elapsed (assuming a maximum of 1 minutes)
                    double elapsedTime = (DateTime.Now - installerProcess.StartTime).TotalSeconds;
                    int progressValue = (int)(elapsedTime / 60 * 100); // Calculate the progress percentage
                    InstProgressBar.Value = Math.Min(progressValue, 80); // Cap at 80% if still running after 2 minutes
                }
            }
            catch (ArgumentException)
            {
                // most of the time the exit of the PID is not really trackable due to the timer
                // hence it will catch exception but the exit is still fine

                StatusLabel.Text = "The installation has completed.";
                InstProgressBar.Value = 100;
                FinishButton.Enabled = true;

                // Stop the timer
                Timer timer = (Timer)sender;
                timer.Stop();
            }
        }

        #endregion

    }


}
