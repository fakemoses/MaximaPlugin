using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Security.Principal;
using System.Linq;

using SMath.Manager;
using MaximaPlugin.ControlObjects;

namespace MaximaPlugin
{
    class MaximaSession
    {
        public MaximaSocket socket = null;
        #region VARS
        // Path vars
        private string assemblyFolder = "";
        private string workingFolder = "";
        public string gnuPlotImageFolder = "";
        public string namedDrawImageFolder = "";
        private string ConfigFileName = "";

        string XMLname = "maxima.xml";
        string MAXIMAname = "maxima.bat";
        string pathToMAXIMA = "";
        string pathToMAXIMArel = "";
        int majorV = 0, minorV = 0, buildV = 0, revisionV = 0;
        //XML DATA
        public List<string> settings;
        public List<string> commands;
        public List<string> customCommands;
        public List<ExpressionStore> exprSMathToMaxima;
        public List<ExpressionStore> exprMaximaToSMath;
        //Process vars
        private Process maximaProcess = null;
        private bool maximaConnected = false;
        private List<Process> processesBeforeStart; // Use list for easy concatenation
        private List<Process> processesAfterStart;  // Use list for easy concatenation
        private Process newMaximaProcess;
        private string[] ProcessNames = { "maxima", "lisp", "sbcl" };

        //State Messages
        private string maximaState = "\"Initializing error. Please check path to Maxima under Insert> Maxima> Settings.\"";
        private string maximaStateClosed = "\"Maxima was closed.\"";
        private string maximaStateRunning = "\"Maxima started successfully.\"";
        private string maximaStateReInitialized = "\"Maxima was re-initialized\"";
        private string maximaStateRestart = "\"Restart complete.\"";
        private string maximaStateCleanup = "\"Cleanup complete.\"";
        //Other vars
        //private string receiveString="";
        private int timeToWait = 100;
        private bool firstCmd = false;
        //Version control init in ReadConfig()
        private int RequiredConfigFormatID = 13;
        private int FoundConfigFormatID = 0;

        #endregion

        /// <summary>
        /// Session constructor
        /// </summary>
        public MaximaSession()
        {
            settings = new List<string>();
            commands = new List<string>();
            customCommands = new List<string>();
            exprSMathToMaxima = new List<ExpressionStore>();
            exprMaximaToSMath = new List<ExpressionStore>();
            assemblyFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            workingFolder = GlobalProfile.SettingsDirectory + @"extensions\plugins\44011c1e-5d0d-4533-8e68-e32b5badce41";
            gnuPlotImageFolder = System.IO.Path.Combine(workingFolder, "GnuPlot");
            namedDrawImageFolder = System.IO.Path.Combine(workingFolder, "Images");
            ConfigFileName = System.IO.Path.Combine(workingFolder, XMLname);

        }

        #region Set and get
        /// <summary>
        /// Value of maximaState if session is running
        /// </summary>
        /// <returns></returns>
        public string GetMaximaStateRunning() { return maximaStateRunning; }
        public void SetRunState() { maximaState = maximaStateRunning; }
        /// <summary>
        /// Path to maxima.bat relative to SMath dir.
        /// </summary>
        /// <returns></returns>
        public string GetPathToMaximabat() { return pathToMAXIMArel; }
        public string GetAssemblyFolder() { return assemblyFolder; }
        public string WorkingFolderPath() { return workingFolder; }

        /// <summary>
        /// Executed at first Maxima interaction of a given SMath session
        /// in the constructor of AutoMaxima()
        /// </summary>
        public void StartSession()
        {
            // TODO: Why this conditional? Maxima can't be connected when StartSession() is called
            if (!maximaConnected)
            {
                firstCmd = true; // first interaction in a given SMath session
                // Read Config and start Maxima if the config is uptodate and the maxima path is valid
                if (File.Exists(ConfigFileName))
                {
                    ReadConfig();
                    if (IsConfigFileFromCurrentPluginVersion() && File.Exists(pathToMAXIMA))
                    {
                        StartAndConnectMaxima(0);
                        return;
                    }
                }
                // if we are here, either
                //    Config file does'nt exist or 
                //    Config file is older or 
                //    the path to maxima doesn't exist
                // Try to delete the old config files
                string oldCfgPath = assemblyFolder + "\\";
                List<string> files = new List<string>(){
                    oldCfgPath + "maxima.inf",
                    oldCfgPath + "smath.mac",
                    oldCfgPath + "smath.lisp",
                    oldCfgPath + "Maxima.log",
                    oldCfgPath + "load.mac",
                    oldCfgPath + "maxima.xml"
                };
                if (DeleteFiles(files))
                {
                    if (checkForXMLandMAXIMA())
                    {
                        ReadConfig();
                        SaveConfig();
                        //xmlRead(ConfigFileName);
                        StartAndConnectMaxima(1);
                        // if (! maximaConnected) return
                        MessageBox.Show("Found path: " + pathToMAXIMA + "\nYou can change this path under Insert->Maxima->Settings.\nMaxima Plugin is ready to use.",
                        "Found path",
                        MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        FoundNoPath();
                    }
                    Properties.Settings.Default.firstStart = false;
                }

            }

        }

        /// <summary>
        /// Dialog if path not found.
        /// If the Settings window isn't open, offer to open it.
        /// </summary>
        public void FoundNoPath()
        {
            // when using menu bar on GUI do nothing
            if (SharedFunctions.initializingOverMenue) return;

            string EnvironPath = Path.GetPathRoot(Environment.SystemDirectory);
            string MaximaPath = FindMaximaPath(EnvironPath);
            // make sure the path has a trailing directory separator
            if (!MaximaPath.EndsWith(Path.DirectorySeparatorChar.ToString())) MaximaPath += Path.DirectorySeparatorChar;

            MaximaSession m = Translator.GetMaxima();

            string res = m.SetNewPathToMaxima(MaximaPath);

            if (res != "CannotFind")
            {
                MessageBox.Show("Found Path to maxima: " + MaximaPath);
            }
            else
            {

                //if failed show this
                DialogResult result2 = MessageBox.Show(
                "Cannot find maxima.bat. Do you want to install it automatically? \n\nInstalling will require active internet connection and admin right.",
                "Cannot find maxima.bat",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification);

                if (result2 == DialogResult.Yes)
                {
                    // Based on suggestion by Davide Carpi
                    // Running the form in STAthread so that it can run independently
                    Thread staThread = new Thread(() =>
                    {
                        using (var installerForm = new MaximaPlugin.MForms.InstallerForm())
                        {
                            // Show the installer form modally
                            installerForm.ShowDialog();
                        }
                    });
                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    staThread.Join();

                }
            }

        }

        /// <summary>
        /// Try to get the path to Maxima
        /// </summary>
        /// <param name="StartPath"></param>
        /// <returns>Relative path or error message</returns>
        public string SetNewPathToMaxima(string StartPath)
        {
            StartPath = Path.GetDirectoryName(StartPath);
            if (StartPath != null)
            {
                string FoundPath = SharedFunctions.SearchFile(StartPath, MAXIMAname);
                if (FoundPath != null && File.Exists(FoundPath))
                {
                    ReadConfig();
                    pathToMAXIMA = FoundPath;
                    pathToMAXIMArel = SharedFunctions.GetRelativePath(FoundPath, GlobalProfile.ApplicationPath);
                    SaveConfig();
                    CloseMaxima();
                    StartSession();
                    //StartAndConnectMaxima(1);
                    return pathToMAXIMArel;
                }
            }
            return "CannotFind";
        }

        /// <summary>
        /// Get the value of maximaState
        /// </summary>
        /// <returns>value</returns>
        public string GetState() { return maximaState; }
        #endregion

        #region MaximaCommands


        /// <summary>
        /// Test if Maxima is responsive
        /// </summary>
        /// <returns>result of the check</returns>
        public bool IsAlive()
        {
            return (SendAndReceiveFromSocket("check", "check") != "-NoDataAvailable-");
        }

        public void CloseMaxima()
        {
            if (maximaConnected)
            {
                try
                {
                    //int processesBefore = System.Diagnostics.Process.GetProcessesByName("maxima").Length;
                    Thread.Sleep(timeToWait);
                    socket.SendSingleCommand("quit();");
                    Thread.Sleep(timeToWait * 10);
                    maximaConnected = false;
                    maximaState = maximaStateClosed;
                    //int processesAfter = System.Diagnostics.Process.GetProcessesByName("maxima").Length;
                    //Process[] procs = System.Diagnostics.Process.GetProcessesByName("maxima");
                    if (!(newMaximaProcess == null))
                    {
                        newMaximaProcess.Kill();
                        newMaximaProcess.WaitForExit();
                    }


                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Implementation of MaximaControl("cleanup").
        /// Maxima is reset without shutting down the session. 
        /// </summary>
        /// <returns></returns>
        public string CleanupMaxima()
        {
            if (maximaConnected)
            {
                SendAndReceiveFromSocket("reset(all)");
                SendAndReceiveFromSocket("kill(all)");
                socket.ClearFullLog("Log cleared by cleanup request\n");
                SendInitCmd();
                SetRunState();
            }
            if (maximaState == maximaStateRunning)
                return maximaStateCleanup;
            else return maximaState;
        }

        /// <summary>
        /// Implementation of MaximaControl("restart").
        /// The Maxima session is closed, the logs are reset and a new session is established.
        /// </summary>
        /// <returns></returns>
        public string RestartMaxima()
        {
            if (maximaConnected && firstCmd)
            {
                return maximaState;
            }
            else if (maximaConnected)
            {
                CloseMaxima();
                socket.ClearFullLog("Log cleared by restart request\n");
                MaximaSocket.wxmLog = "/* Log cleared by restart request */\n";
                StartAndConnectMaxima(0);
            }
            if (maximaState == maximaStateRunning)
                return maximaStateRestart;
            else return maximaState;
        }

        /// <summary>
        /// Submit configuration commands to a running Maxima session
        /// </summary>
        /// <returns></returns>
        public string SendInitCmd()
        {
            if (maximaConnected)
            {
                string tempstring = "";
                int i = 0;
                Regex rxMxOut = new Regex(@"(\(.[i][0-9A-F]+\))");
                foreach (string str in commands)
                {
                    MaximaPlugin.ControlObjects.Translator.maximaStartStep++;
                    socket.SendSingleCommand(str);
                    i = 0;
                    tempstring = socket.ReceiveSingleResponse();
                    while (!rxMxOut.IsMatch(tempstring, 0) && i < 50)
                    {
                        System.Threading.Thread.Sleep(timeToWait * 2);
                        tempstring = socket.ReceiveSingleResponse();
                        i++;
                    }
                }
                foreach (string str in customCommands)
                {
                    MaximaPlugin.ControlObjects.Translator.maximaStartStep++;
                    socket.SendSingleCommand(str);

                    i = 0;
                    tempstring = socket.ReceiveSingleResponse();
                    while (!rxMxOut.IsMatch(tempstring, 0) && i < 50)
                    {
                        System.Threading.Thread.Sleep(timeToWait * 2);
                        tempstring = socket.ReceiveSingleResponse();
                        i++;
                    }
                }
                //SharedFunctions.FullLog = SharedFunctions.FullLog + Environment.CurrentDirectory;
                maximaState = maximaStateReInitialized;
                //return socket.ReceiveSingleCommand();
                return "init complete";
            }
            return "Initializing error";
        }
        #endregion

        #region Send and Receive
        /// <summary>
        /// Sends a command to Maxima and gets the result if Maxima is running,
        /// otherwise the state description is returned
        /// </summary>
        /// <param name="sendString"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public string SendAndReceiveFromSocket(string sendString, string option = "none")
        {
            if (maximaConnected)
            {
                firstCmd = false;
                maximaState = maximaStateRunning;
                //receiveString = socket.SendAndReceive(sendString + ";", option);
                socket.SendSingleCommand(sendString + ";", option);
                return socket.ReceiveSingleResponse(option);

                //return receiveString;
            }
            else
            {
                return maximaState;
            }
        }

        /// <summary>
        /// Wrapper function
        /// </summary>
        /// <returns></returns>
        public string ReceiveSingleCommandFromSocket()
        {
            return socket.ReceiveSingleResponse();
        }

        /// <summary>
        /// Send and log
        /// </summary>
        /// <param name="data"></param>
        public void SendSingleCommandToSocket(string data)
        {
            firstCmd = false;
            socket.SendSingleCommand(data + ";"); // Actually send the string
        }
        #endregion

        #region Logging
        /// <summary>
        /// Get the last log entry from maximaSocket
        /// </summary>
        /// <returns></returns>
        public string GetLastLog() { return socket.GetLastLog(); }

        #endregion

        #region StartControl

        /// <summary>
        /// Gets the process objects of possible maxima processes.
        /// Possible process names are given in property ProcessNames.
        /// This was required, because different Maxima versions use different process names
        /// </summary>
        /// <returns></returns>
        private List<Process> GetProcesses()
        {
            List<Process> tmp = new List<Process> { };
            foreach (string name in ProcessNames)
            {
                if (System.Diagnostics.Process.GetProcessesByName(name).Length > 0)
                    tmp.AddRange(System.Diagnostics.Process.GetProcessesByName(name));
            }
            return tmp;
        }


        /// <summary>
        /// Establish connection and initialize Maxima (show a progress bar)
        /// Detect the process handle for the Maxima process for later closing
        /// </summary>
        /// <param name="option"></param>
        private void StartAndConnectMaxima(int option)
        {
            MaximaPlugin.ControlObjects.Translator.maximaStartStep = 1;
            if (!maximaConnected)
            {
                // determine, which maxima processes are running, before starting a new one.
                processesBeforeStart = GetProcesses();

                // start the progress bar
                ThreadWorkClass twc = new ThreadWorkClass(this);
                Thread trd = new Thread(new ThreadStart(twc.ThreadFunction));
                trd.Start();

                // start Maxima process
                socket = new MaximaSocket();
                System.Threading.Thread.Sleep(50);
                maximaProcess = new Process();
                maximaProcess.StartInfo.FileName = pathToMAXIMA;
                // "-l sbcl" enforces Steel bank common lisp (because it is unicode-proof)
                maximaProcess.StartInfo.Arguments = " -l sbcl -s " + Convert.ToString(socket.GetPort());
                maximaProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                maximaProcess.Start();
                System.Threading.Thread.Sleep(timeToWait);
                socket.ConnectClient();
                maximaConnected = true;

                //Wait for Maxima prompt
                string tempstring = "";
                int i = 0;
                Regex rxMxOut = new Regex(@"(\(.[i][0-9A-F]+\))");
                do
                {
                    System.Threading.Thread.Sleep(timeToWait * 2);
                    tempstring = socket.ReceiveSingleResponse();
                    i++;
                } while (!rxMxOut.IsMatch(tempstring, 0) && i < 50);

                // Send Maxima config commands
                tempstring = SendInitCmd();

                // Close the progress bar
                trd.Abort();

                // Set sessionstate to running
                System.Threading.Thread.Sleep(100);
                maximaState = maximaStateRunning;

                //Get the right maxima-process - the first process ("maximaProcess") is only the batch process which start maxima
                processesAfterStart = GetProcesses();

                if (processesAfterStart.Count == 1)
                {
                    //only one process is running
                    newMaximaProcess = processesAfterStart[0];
                }
                else if (processesAfterStart.Count > 1)
                {
                    //more than 1 process is running    
                    //the process which was not running before start, is the right one 
                    for (int j = 0; j < processesAfterStart.Count; j++)
                    {
                        if (!processesBeforeStart.Contains(processesAfterStart[j])) newMaximaProcess = processesAfterStart[j];
                    }
                }
                else
                {
                    // No process found.
                    newMaximaProcess = null;

                    DialogResult result = MessageBox.Show("No Maxima process found when searching for\n " + String.Join(", ", ProcessNames) +
                        "\nSMath won't be able to kill the process when shutting down or restarting.\n\nPlease identify the name of the Maxima process in your system \nand inform the developer in the SMath Forum.",
                        "No Maxima process found",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //maximaState = maximaStateCannotStart;
                    //maximaConnected = false;
                }


            }
        }

        /// <summary>
        /// Helper function for migration from old style config files
        /// Returns true if 
        ///    just one file was found, 
        ///    more files were found, the user confirmed deletion and the files were deleted successfully
        /// </summary>
        /// <param name="filePaths">list of file names</param>
        /// <returns>success flag</returns>
        public static bool DeleteFiles(List<string> filePaths)
        {
            string FilesFound = "";
            foreach (string path in filePaths)
            {
                if (File.Exists(path))
                {
                    FilesFound = FilesFound + path + "\n";
                }
            }
            if (FilesFound.Length > 1)
            {
                DialogResult result1 = MessageBox.Show("Old Config-Files:\n" + FilesFound + "will be deleted.",
                "Delete files",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (result1 == DialogResult.OK)
                {
                    if (IsAnAdministrator())
                    {
                        foreach (string path in filePaths)
                        {
                            if (File.Exists(path)) File.Delete(path);
                        }
                        return true;
                    }
                    else
                    {
                        DialogResult result2 = MessageBox.Show("Please restart with administrator privileges or delete files manually:\n" + FilesFound,
                        "Administrator privileges necessary",
                         MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return false;
                    }
                }
                else return false;
            }
            else return true;
        }

        /// <summary>
        /// Check the privileges
        /// </summary>
        /// <returns>true if the user is an admin</returns>
        public static bool IsAnAdministrator()
        {
            WindowsIdentity identity =
               WindowsIdentity.GetCurrent();
            WindowsPrincipal principal =
               new WindowsPrincipal(identity);
            return principal.IsInRole
               (WindowsBuiltInRole.Administrator);
        }





        /// <summary>
        /// Check if Config file was generated with the current plugin version
        /// </summary>
        /// <returns></returns>
        public bool IsConfigFileFromCurrentPluginVersion()
        {
            if (FoundConfigFormatID >= RequiredConfigFormatID)
            {
                if (majorV >= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major)
                {
                    if (majorV > System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major) return true;
                    if (minorV >= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor)
                    {
                        if (minorV > System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor) return true;
                        if (buildV >= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build)
                        {
                            if (buildV > System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build) return true;
                            if (revisionV >= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the config file exists and the path to maxima from therein is valid.
        /// </summary>
        /// <returns>test result</returns>
        public bool checkForXMLandMAXIMA()
        {
            if (File.Exists(ConfigFileName))
            {
                ReadConfig();
                if (File.Exists(pathToMAXIMA))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }
        #endregion

        #region XML handle

        /// <summary>
        /// Specify the config items and write them to the config file
        /// </summary>
        public void SaveConfig()
        {
            // Path to maxima.bat, ID and plugin-version
            settings = new List<string>(){
                pathToMAXIMArel,
                RequiredConfigFormatID.ToString(),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString(),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString(),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                };
            // default maxima init commands
            commands = new List<string>
                {
                "display2d:false$",
                "linel:700$",
                "simpsum:true$",
                "logsimp:true$",
                "keepfloat:true$",
                "ratprint:false$",
                "load(vect)$",
                "load(abs_integrate)$",
                "setup_autoload(mnewton, mnewton)$",
                "setup_autoload(draw, draw2d, draw3d)$",
                "load(lsquares)$",
                //"setup_autoload(lsquares, lsquares_mse, lsquares_estimates_approximate)$",  
                "logb(x,y):=log(x)/log(y)$",
                "ntewurzel(x,n):=x^(1/n)$", // helper function for representation of the nth root (nthroot is something different in Maxima)
                "el(x,y):=x[y]$",
                "el(x,[y]):=if equal(length(y),1) then x[y[1]] else x[y[1],y[2]]$",
                "Fit(Data,vars,fun,par,init):=lsquares_estimates_approximate(lsquares_mse(Data,vars,fun),par,initial=init,iprint=[-1,0])$",
                "Fit6(Data,vars,fun,par,init,t):=lsquares_estimates_approximate(lsquares_mse(Data,vars,fun),par,initial=init,tol=float(t),iprint=[-1,0])[1]$",
                "Residuals(Data,vars,fun,[par]):=float(transpose(matrix(lsquares_residuals(Data,vars,fun,par))))$",
                "MSE(Data,vars,fun,[par]):=float(lsquares_residual_mse(Data,vars,fun,par))$",
                "round2(x,n):=round(x*10^n)/10^n$",
                 //"CrossProduct(a,b):=transpose(matrix(express(args(transpose(a))[1] ~ args(transpose(b))[1])))$",
                };
            // add an example command if the list is empty
            if (customCommands.Count == 0)
            {
                customCommands.Add("CustomInitExample;");
            }
            // add some dummy contents if the list is empty
            if (exprSMathToMaxima.Count == 0)
            {
                exprSMathToMaxima.Add(new ExpressionStore("Your SMath functions for Maxima", "RegularExpression", "ReplaceExpression"));
                exprMaximaToSMath.Add(new ExpressionStore("Your Maxima functions for SMath", "RegularExpression", "ReplaceExpression"));
            }
            // actually write the config file
            XmlInterface.writeXml(ConfigFileName, settings, commands, customCommands, exprSMathToMaxima, exprMaximaToSMath);
        }

        /// <summary>
        /// Read the config data and report data to FullLog
        /// </summary>
        public void ReadConfig()
        {
            settings.Clear();
            commands.Clear();
            customCommands.Clear();
            exprSMathToMaxima.Clear();
            exprMaximaToSMath.Clear();

            if (File.Exists(ConfigFileName))
            {
                // Read config file
                XmlInterface.readXmlALL(ConfigFileName, settings, commands, customCommands, exprSMathToMaxima, exprMaximaToSMath);
                // Postprocessing
                if (settings.Count == 6)
                {
                    pathToMAXIMArel = settings[0];
                    FoundConfigFormatID = Convert.ToInt32(settings[1]);
                    majorV = Convert.ToInt32(settings[2]);
                    minorV = Convert.ToInt32(settings[3]);
                    buildV = Convert.ToInt32(settings[4]);
                    revisionV = Convert.ToInt32(settings[5]);
                    try
                    {
                        //string path = Environment.CurrentDirectory;
                        //Environment.CurrentDirectory = GlobalProfile.ApplicationPath;
                        pathToMAXIMA = Path.Combine(GlobalProfile.ApplicationPath, pathToMAXIMArel);
                        //Environment.CurrentDirectory = path;
                    }
                    catch { }
                }
                else FoundNoPath();
            }

        }

        public static string FindMaximaPath(string path)
        {
            List<string> files = GetDirectories(path);
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

            string fullPath = Path.Combine(path, foundMaximaPath) + "\\";

            return fullPath;
        }
        public static List<string> GetDirectories(string path, string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (searchOption == SearchOption.TopDirectoryOnly)
                return Directory.GetDirectories(path, searchPattern).ToList();

            var directories = new List<string>(GetDirectories(path, searchPattern));

            for (var i = 0; i < directories.Count; i++)
                directories.AddRange(GetDirectories(directories[i], searchPattern));

            return directories;
        }

        #endregion
        class ThreadWorkClass
        {
            MaximaSession am;
            MForms.LoadingForm ProgressBar;

            public ThreadWorkClass(MaximaSession am)
            {
                this.am = am;
            }
            public void ThreadFunction()
            {
                int maxsteps = am.commands.Count + am.customCommands.Count - 3; //+1
                double value = (double)MaximaPlugin.ControlObjects.Translator.maximaStartStep * 100 / (double)maxsteps;
                ProgressBar = new MForms.LoadingForm(maxsteps);
                ProgressBar.Show();
                while (value < 100)
                {
                    lock (am)
                    {
                        value = (double)MaximaPlugin.ControlObjects.Translator.maximaStartStep * 120 / (double)maxsteps;
                        if (value < 100) ProgressBar.progressControl(value);
                        else ProgressBar.progressControl(99);
                    }
                    System.Threading.Thread.Sleep(50);
                }
                ProgressBar.Close();
            }

        }
    }
}
