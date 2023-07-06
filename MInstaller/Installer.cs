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
    class Installer
    {
        public static async Task DownloadInstaller(string url, string path, System.Windows.Forms.ProgressBar pb, Form form)
        {
            int maxRetryAttempts = 3; // Set the maximum number of retry attempts
            int retryDelaySeconds = 5; // Set the delay in seconds between retries

            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (sender, e) =>
                {
                    // Update the progress bar value based on the percentage downloaded
                    pb.Value = e.ProgressPercentage;
                };

                client.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Error == null)
                    {
                        MessageBox.Show("Installer downloaded successfully.");
                    }
                    else
                    {
                         MessageBox.Show("Error downloading the installer: " + e.Error.Message);
                    }
                };

                int retryCount = 0;

                while (retryCount < maxRetryAttempts)
                {
                    try
                    {
                        await client.DownloadFileTaskAsync(new Uri(url), path);
                        // Download completed successfully, exit the loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        MessageBox.Show($"Error downloading the installer (Attempt {retryCount}): {ex.Message}");
                        await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                    }
                }

                if (retryCount >= maxRetryAttempts)
                {
                    MessageBox.Show($"Failed to download the installer after {maxRetryAttempts} attempts.");
                    form.Close();
                }
            }
        }

        public static int RequestAdminPrivileges(string installerPath)
        {
            int pid = 0; // process ID
            // Check if the current user has administrative privileges
            if (IsUserAdmin())
            {
                //delete this later - replace with something else
                MessageBox.Show("The installation process has started."); // Show a message box
                // need to know when the installer is finished
                pid = InstallSilently(installerPath);
                
            }
            else
            {
                // Restart the application with administrative privileges
                pid = RestartWithAdminPrivileges(installerPath);
            }
            return pid;
        }

        static bool IsUserAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static int RestartWithAdminPrivileges(string installerPath)
        {
            string scriptPath = Path.Combine(Path.GetTempPath(), "InstallScript.ps1");

            // Create a PowerShell script file that will be executed with admin rights
            string scriptContent = $@"
            Start-Process '{installerPath}' -ArgumentList '/S' -Verb RunAs -Wait
            Remove-Item -Path '{scriptPath}' -Force";

            File.WriteAllText(scriptPath, scriptContent);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "powershell.exe";
            startInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"";
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

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
