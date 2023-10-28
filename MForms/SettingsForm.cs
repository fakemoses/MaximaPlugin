using MaximaPlugin.MInstaller;
using SMath.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace MaximaPlugin.MForms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            // check and get latest version here
            SetMaximaVersionInformation();
        }

        /// <summary>
        /// Create maxima session if not available and set the PathTB to the path of the maxima if available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // writeList();

            if (ControlObjects.Translator.GetMaxima() != null) // this tries to launch a maxima session
            {
                if (ControlObjects.Translator.GetMaxima().GetPathToMaximabat() != "")
                {
                    PathTB.Text = Path.GetDirectoryName(ControlObjects.Translator.GetMaxima().GetPathToMaximabat());
                }
                else
                {
                    PathTB.Text = SMath.Manager.GlobalProfile.ApplicationPath;
                }
            }

        }

        /// <summary>
        /// When form is closed, reset the variable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsForm_Close(object sender, FormClosingEventArgs e)
        {
            SharedFunctions.initializingOverMenue = false;
            MForms.FormControl.SettingsFormState = false;
        }

        /// <summary>
        /// Check if the path given by user contain a valid maxima.bat file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Verify existence of the path, either absolute or relative to the SMath install dir (ApplicationPath)
            string path = "";
            if (Directory.Exists(PathTB.Text)) path = PathTB.Text;
            else if (Directory.Exists(Path.Combine(SMath.Manager.GlobalProfile.ApplicationPath, PathTB.Text)))
                path = Path.Combine(SMath.Manager.GlobalProfile.ApplicationPath, PathTB.Text);
           
            if (path == "") // If no valid path found, go back
            {
                MessageBox.Show("Neither " + PathTB.Text 
                    +         "\nnor     " + Path.Combine(SMath.Manager.GlobalProfile.ApplicationPath, PathTB.Text)
                    +         "\nare valid paths.",
                                "Path to Maxima",
                                MessageBoxButtons.OK);
                return;
            }
            else // if path is a valid absolute path, search for maxima.bat
            {
                // make sure the path has a trailing directory separator
                if (! path.EndsWith(Path.DirectorySeparatorChar.ToString()) ) path += Path.DirectorySeparatorChar;
                // search for maxima.bat and save to config
                string fileInfo = ControlObjects.Translator.GetMaxima().SetNewPathToMaxima(path);
                if (fileInfo != "CannotFind") // maxima.bat was found
                {
                    PathTB.Text = Path.GetDirectoryName(ControlObjects.Translator.GetMaxima().GetPathToMaximabat());
                    DialogResult result1 = MessageBox.Show("Found \"" + fileInfo + "\".",
                    "Path to Maxima",
                    MessageBoxButtons.OK);
                    if (result1 == DialogResult.OK)
                        Close();
                }
                else
                {
                    DialogResult result3 = MessageBox.Show("Cannot find maxima.bat",
                    "Path to Maxima",
                    MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// Open a folder browser dialog to search for maxima.bat path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            // Define the start path for selection
            fbd.SelectedPath = SMath.Manager.GlobalProfile.ApplicationPath;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                PathTB.Text=fbd.SelectedPath;
            }

        }

        /// <summary>
        /// Opens installer form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void  InstallButton_Click(object sender, EventArgs e)
        {
            DialogResult result2 = MessageBox.Show(
                "Maxima installation require internet connection and admin right. Do you want to proceed?",
                "Installation require Internet connection",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1
                );

            if (result2 == DialogResult.Yes)
            {
                this.Close();
                Thread staThread = new Thread(() =>
                {
                    using (var installerForm = new MaximaPlugin.MForms.InstallerForm())
                    {
                        // Show the installer form modally
                        installerForm.ShowDialog();
                        installerForm.TopMost = true;
                    }
                });
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start();
                staThread.Join();

            }
        }

        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        #region Helper functions

        /// <summary>
        /// Set the Maxima version information from the label
        /// Get the latest Maxima version from internet 
        /// Find the current version installed - if not in C then find the xml file. If no XML file then None
        /// </summary>
        private async void SetMaximaVersionInformation()
        {
            string workingFolder = GlobalProfile.SettingsDirectory + @"extensions\plugins\44011c1e-5d0d-4533-8e68-e32b5badce41\maxima.xml";
            string r = CheckMaximaAvailable();
            string xml = FindPathToMaximaInXML(workingFolder);

            if (xml != "")
            {
                InstVerLabel.Text = ExtractMaximaVersion(xml);

            } else if (r != "")
            {
                InstVerLabel.Text = ExtractMaximaVersion(r);
            } else
            {
                InstVerLabel.Text = "None";
            }

            // need to deal with error -> try block for the await line
            string url = "https://sourceforge.net/projects/maxima/best_release.json";
            JsonDataFetcher dataFetcher = new JsonDataFetcher();
            string windowsUrl = await dataFetcher.GetWindowsReleaseUrl(url);

            LocVerLabel.Text = ExtractMaximaVersion(windowsUrl);
        }

        /// <summary>
        /// check for maxima in C drive
        /// </summary>
        /// <returns></returns>
        private static string CheckMaximaAvailable()
        {
            string EnvironPath = Path.GetPathRoot(Environment.SystemDirectory);
            List<string> files = MaximaSession.GetDirectories(EnvironPath);
            string foundMaximaPath = "";
            string pattern = @"\bmaxima-\d+(\.\d+){0,2}\b";

            foreach (string file in files)
            {
                MatchCollection matches = Regex.Matches(file, pattern);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        foundMaximaPath = match.Value;
                    }
                }
            }
            
            return foundMaximaPath;
        }

        /// <summary>
        /// Extract Maxima version from input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ExtractMaximaVersion(string input)
        {
            string pattern = @"\d+\.\d+\.\d+"; // Matches 3 sets of numbers separated by dots
            Match match = Regex.Match(input, pattern);
            return match.Success ? match.Value : null;
        }

        /// <summary>
        /// Check for Maxima path in config file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string FindPathToMaximaInXML (string filePath)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                XmlNodeList pathToMaximaNodes = xmlDoc.GetElementsByTagName("PathToMaxima.bat");
                if (pathToMaximaNodes.Count > 0)
                {
                    return pathToMaximaNodes[0].InnerText;
                }
            }
            catch (Exception ex)
            {
                
            }

            return string.Empty;
        }

        #endregion
    }
}
