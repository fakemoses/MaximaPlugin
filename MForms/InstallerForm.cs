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

        public InstallerForm()
        {
            InitializeComponent();

            scaleFactor = SharedFunctions.getScreenScaleFactor();

            // form panel thing
            panel1.Visible = false;
            panel2.Visible = true;

            radioButton1.Checked = true;
            radioButton2.Checked = false;

            // Attach event handlers for radio buttons and Next button
            radioButton1.CheckedChanged += RadioButtonOption_CheckedChanged;
            radioButton2.CheckedChanged += RadioButtonOption_CheckedChanged;

            if (scaleFactor > 1.0 && scaleFactor < 1.5)
                this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(260 * scaleFactor));
            else if (scaleFactor >= 1.5)
                this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(260 * scaleFactor));
            else
                this.Size = new System.Drawing.Size(520, 260);

            //move panel 2 to panel 1 location
            panel2.Location = panel1.Location;
        }

        public async void ExtractUrlFromJson()
        {
            label4.Text = "Downloading installer..";
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

        public async Task InstallMaxima(string windowsUrl, string installerPath)
        {

            //async download
            await Installer.DownloadInstaller(windowsUrl, installerPath, progressBar1);

            //check for silent installation
            bool silentInstall;
            if (radioButton1.Checked == true)
                silentInstall = true;
            else
                silentInstall = false;

            //start installation
            installerProcessId = Installer.RequestAdminPrivileges(installerPath, silentInstall);

            //disable cancel button at this point
            button2.Enabled = false;

            if (installerProcessId > 0)
            {
                //start timer to track progress
                timer.Start();
                label4.Text = "The installation is in progress...";
            }
            else
            {
                MessageBox.Show("Failed to start the installation.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ControlObjects.Translator.GetMaxima().FoundNoPath();
            Close();
        }

        // Timer tick event handler to check if the installer process has exited
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Process installerProcess = Process.GetProcessById(installerProcessId);
                if (installerProcess.HasExited)
                {
                    // The installer process has exited
                    // Perform actions or display a message to indicate completion
                    label4.Text = "The installation has completed.";
                    progressBar2.Value = 100;
                    button1.Enabled = true;

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
                    progressBar2.Value = Math.Min(progressValue, 80); // Cap at 80% if still running after 2 minutes
                }
            }
            catch (ArgumentException)
            {
                // most of the time the exit of the PID is not really trackable due to the timer
                // hence it will catch exception but the exit is still fine

                label4.Text = "The installation has completed.";
                progressBar2.Value = 100;
                button1.Enabled = true;

                // Stop the timer
                Timer timer = (Timer)sender;
                timer.Stop();
            }
        }
        private void RadioButtonOption_CheckedChanged(object sender, EventArgs e)
        {
            // Only one radio button allowed
            if (sender == radioButton1 && radioButton1.Checked)
            {
                radioButton2.Checked = false;
            }
            else if (sender == radioButton2 && radioButton2.Checked)
            {
                radioButton1.Checked = false;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            progressBar2.Maximum = 100; // Set the maximum value of the progress bar
            progressBar2.Value = 0;
            timer = new Timer();
            timer.Interval = 1000; // Check every 1 second 
            timer.Tick += Timer_Tick;


            // Handle radio button changes here
            if (radioButton1.Checked)
            {
                if (scaleFactor > 1.0 && scaleFactor < 1.5)
                    this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(260 * scaleFactor));
                else if (scaleFactor >= 1.5)
                    this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(260 * scaleFactor));
                else
                    this.Size = new System.Drawing.Size(520, 260);
            }
            else if (radioButton2.Checked)
            {
                if (scaleFactor > 1.0 && scaleFactor < 1.5)
                    this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(200 * scaleFactor));
                else if (scaleFactor >= 1.5)
                    this.Size = new System.Drawing.Size((int)(520 * scaleFactor), (int)(200 * scaleFactor));
                else
                    this.Size = new System.Drawing.Size(520, 200);

                label3.Location = new System.Drawing.Point(0, 76);
                label4.Location = new System.Drawing.Point(46, 123);
                label5.Location = new System.Drawing.Point(0, 123);
                button2.Location = new System.Drawing.Point(302, 103);
                button1.Location = new System.Drawing.Point(383, 103);

                label2.Visible = false;
                progressBar2.Visible = false;
            }

            // form panel related init
            panel1.Visible = true;
            panel2.Visible = false;

            ExtractUrlFromJson();
            button1.Enabled = false;
        }
    }


}
