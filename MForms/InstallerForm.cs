using MaximaPlugin.MInstaller;
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

        public InstallerForm()
        {
            InitializeComponent();
            ExtractUrlFromJson();
            button1.Enabled = false;
        }

        public async void ExtractUrlFromJson()
        {
            label4.Text = "Downloading installer..";
            string url = "https://sourceforge.net/projects/maxima/best_release.json"; 
            string installerPath = Path.Combine(Path.GetTempPath(), "installer.exe");

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
            await Installer.DownloadInstaller(windowsUrl, installerPath, progressBar1);
            installerProcessId = Installer.RequestAdminPrivileges(installerPath);
            
            if(installerProcessId > 0)
            {
                // Start a timer to periodically check if the installer process has exited
                Timer timer = new Timer();
                timer.Interval = 1000; // Check every 1 second 
                timer.Tick += Timer_Tick;
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
            //this part still not working as intended
            try
            {
                Process installerProcess = Process.GetProcessById(installerProcessId);
                if (installerProcess.HasExited)
                {
                    // The installer process has exited
                    // Perform actions or display a message to indicate completion
                    label4.Text = "The installation has completed.";
                    button1.Enabled = true;
                    MessageBox.Show("The installation has completed.");

                    // Stop the timer
                    Timer timer = (Timer)sender;
                    timer.Stop();

                }
            }
            catch (ArgumentException)
            {
  
                label4.Text = "The installation has completed.";
                button1.Enabled = true;

                // Stop the timer
                Timer timer = (Timer)sender;
                timer.Stop();
            }
        }
    }

    
}
