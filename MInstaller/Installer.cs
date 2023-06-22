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

namespace MaximaPlugin.MInstaller
{
    class Installer
    {
        public static void DownloadInstaller(string url, string path)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, path);
                    MessageBox.Show("Installer downloaded successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error downloading the installer: " + ex.Message);
                }
            }
        }

        public static void RequestAdminPrivileges(string installerPath)
        {
            // Check if the current user has administrative privileges
            if (IsUserAdmin())
            {
                MessageBox.Show("The installation process has started."); // Show a message box
                InstallSilently(installerPath);

            }
            else
            {
                // Restart the application with administrative privileges
                RestartWithAdminPrivileges(installerPath);
            }

        }

        static bool IsUserAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void RestartWithAdminPrivileges(string installerPath)
        {
            string scriptPath = Path.Combine(Path.GetTempPath(), "InstallScript.ps1");

            // Create a PowerShell script file that will be executed with admin rights
            string scriptContent = $@"
            Start-Process '{installerPath}' -ArgumentList '/S' -Verb RunAs -Wait
            Remove-Item -Path '{scriptPath}' -Force";

            File.WriteAllText(scriptPath, scriptContent);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "powershell.exe";
            startInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\" -WindowStyle hidden";

            try
            {
                //some input for user when installing begins

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        static void InstallSilently(string installerPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = installerPath;
            startInfo.Arguments = "/S /D=C:\\";

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
