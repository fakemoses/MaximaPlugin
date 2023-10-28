using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MaximaPlugin.MInstaller
{
    /// <summary>
    /// Responsible to download file from URL, create PowerShell script and install the downloaded file.
    /// </summary>
    class Installer
    {
        /// <summary>
        /// Download installer and update the download progress bar in the installer form
        /// </summary>
        /// <param name="url">Installer URL</param>
        /// <param name="path">Download path</param>
        /// <param name="pb">Progress bar of installer form</param>
        /// <returns></returns>
        public static async Task DownloadInstaller(string url, string path, System.Windows.Forms.ProgressBar pb)
        {
            using (var client = new WebClient())
            {
                // Update the progress bar value based on the percentage downloaded
                client.DownloadProgressChanged += (sender, e) =>
                {
                    pb.Value = e.ProgressPercentage;
                };

                client.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Error == null)
                    {

                    }
                    else
                    {
                        MessageBox.Show("Error downloading the installer: " + e.Error.Message);
                    }
                };

                // download file
                try
                {
                    await client.DownloadFileTaskAsync(new Uri(url), path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error downloading the installer: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Check for admin privilege. 
        /// Run installer directly when admin right is available.
        /// Create powershell script and execute if no admin rights is available.
        /// </summary>
        /// <param name="installerPath"></param>
        /// <param name="silentInstall"></param>
        /// <returns></returns>
        public static int RequestAdminPrivileges(string installerPath, bool silentInstall)
        {
            int pid = 0; // process ID
            // Check if the current user has administrative privileges
            if (IsUserAdmin())
            {

                pid = InstallSilently(installerPath);

            }
            else
            {

                pid = RestartWithAdminPrivileges(installerPath, silentInstall);

            }
            return pid;
        }

        /// <summary>
        /// Check if user is admin
        /// </summary>
        /// <returns></returns>
        static bool IsUserAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Create PowerShell script and run the installer via the script.
        /// </summary>
        /// <param name="installerPath"></param>
        /// <param name="silentInstall"></param>
        /// <returns></returns>
        static int RestartWithAdminPrivileges(string installerPath, bool silentInstall)
        {
            string scriptPath = Path.Combine(Path.GetTempPath(), "InstallScript.ps1");
            string scriptContent = "";

            // Create a PowerShell script file that will be executed with admin rights
            if (silentInstall)
            {
                scriptContent = $@"
            Start-Process '{installerPath}' -ArgumentList '/S' -Verb RunAs -Wait
            Remove-Item -Path '{scriptPath}' -Force";
            }
            else
            {
                scriptContent = $@"
            Start-Process '{installerPath}' -Verb RunAs -Wait
            Remove-Item -Path '{scriptPath}' -Force";
            }


            File.WriteAllText(scriptPath, scriptContent);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "powershell.exe";
            startInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                // Start the PowerShell process
                Process process = Process.Start(startInfo);

                // Return the process ID
                return process.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Execute directly the installer.
        /// </summary>
        /// <param name="installerPath"></param>
        /// <returns></returns>
        static int InstallSilently(string installerPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = installerPath;
            startInfo.Arguments = "/S /D=C:\\";

            try
            {
                // Start the PowerShell process
                Process process = Process.Start(startInfo);

                // Return the process ID
                return process.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return -1;
            }
        }
    }
}
