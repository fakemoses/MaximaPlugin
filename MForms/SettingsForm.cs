using System;
using System.Windows.Forms;
using System.IO;
using MaximaPlugin.ControlObjects;
using MaximaPlugin.MInstaller;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SMath.Manager;

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
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void writeNumber()
        {
            bool[] check = new bool[checkedListBox1.Items.Count];
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                check[i] = checkedListBox1.GetItemChecked(4 - i);

            }
            textBox1.Text = Convert.ToString(SharedFunctions.GetIntFromBool(check));
        }
        private void writeList()
        {
            bool[] tmp = SharedFunctions.GetBooleFromInt(Convert.ToInt32(textBox1.Text));

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, tmp[i]);
            }
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // writeList();

            if (ControlObjects.Translator.GetMaxima() != null) // this tries to launch a maxima session
            {
                if (ControlObjects.Translator.GetMaxima().GetPathToMaximabat() != "")
                {
                    textBox2.Text = Path.GetDirectoryName(ControlObjects.Translator.GetMaxima().GetPathToMaximabat());
                }
                else
                {
                    textBox2.Text = SMath.Manager.GlobalProfile.ApplicationPath;
                }
            }

        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            writeNumber();
        }
        private void SettingsForm_Close(object sender, FormClosingEventArgs e)
        {
            SharedFunctions.initializingOverMenue = false;
            MForms.FormControl.SettingsFormState = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text!="")
               writeList();
        }
        public void PathMsgBox()
        {
            if (ControlObjects.Translator.GetMaxima().GetPathToMaximabat() != "")
            {
                DialogResult result1 = MessageBox.Show("Found \"" + ControlObjects.Translator.GetMaxima().GetPathToMaximabat() + "\".",
                "Path to Maxima",
                MessageBoxButtons.OK);
            }
            else
            {
                DialogResult result2 = MessageBox.Show("Cannot find maxima.bat",
                "Path to Maxima",
                MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// "Search for maxima.bat" pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            // Verify existence of the path, either absolute or relative to the SMath install dir (ApplicationPath)
            string path = "";
            if (Directory.Exists(textBox2.Text)) path = textBox2.Text;
            else if (Directory.Exists(Path.Combine(SMath.Manager.GlobalProfile.ApplicationPath, textBox2.Text)))
                path = Path.Combine(SMath.Manager.GlobalProfile.ApplicationPath, textBox2.Text);
           
            if (path == "") // If no valid path found, go back
            {
                MessageBox.Show("Neither " + textBox2.Text 
                    +         "\nnor     " + Path.Combine(SMath.Manager.GlobalProfile.ApplicationPath, textBox2.Text)
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
                    textBox2.Text = Path.GetDirectoryName(ControlObjects.Translator.GetMaxima().GetPathToMaximabat());
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

        private void button1_Click_1(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            // Define the start path for selection
            fbd.SelectedPath = SMath.Manager.GlobalProfile.ApplicationPath;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text=fbd.SelectedPath;
                //button3_Click(sender, e);
            }

        }

        //install button
        private void  button2_Click(object sender, EventArgs e)
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

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        // get the latest version from internet 
        // find the current version installed - if not in C then find the xml file. If no XML file then None

        private async void SetMaximaVersionInformation()
        {
            string workingFolder = GlobalProfile.SettingsDirectory + @"extensions\plugins\44011c1e-5d0d-4533-8e68-e32b5badce41";
            string r = CheckMaximaAvailable();

            //current version
            if (r != "")
            {
                label6.Text = ExtractMaximaVersion(r);
            } else
            {
                string xml = FindPathToMaximaInXML(workingFolder);
                if (xml != "")
                    label6.Text = xml;
                else
                    label6.Text = "None";
            }

            // need to deal with error -> try block for the await line
            string url = "https://sourceforge.net/projects/maxima/best_release.json";
            JsonDataFetcher dataFetcher = new JsonDataFetcher();
            string windowsUrl = await dataFetcher.GetWindowsReleaseUrl(url);

            label7.Text = ExtractMaximaVersion(windowsUrl);
        }

        //check for maxima in C drive
        static string CheckMaximaAvailable()
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

        static string ExtractMaximaVersion(string input)
        {
            string pattern = @"\d+\.\d+\.\d+"; // Matches 3 sets of numbers separated by dots
            Match match = Regex.Match(input, pattern);
            return match.Success ? match.Value : null;
        }

        static string FindPathToMaximaInXML (string filePath)
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
    }
}
