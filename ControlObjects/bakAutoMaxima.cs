using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Sockets;
using SMath.Manager;
using SMath.Math;
using SMath.Math.Numeric;
using System.Reflection;

namespace MaximaPlugin
{
    class AutoMaxima
    {
        #region VARS
        AutoSocket maximaSocket = null;
        // Path vars
        private string assemblyFolder = "";
        private string pathToMaximaBat = "";
        private string nameOfMaximaBat = "maxima.bat";
        private string fullPathToMaximaBat = "";
        private string pathToMaximaConfigFiles = "";
        private string nameOfMaximaCfg = "maxima.inf";
        private string fullPathToMaximaCfg = "";
        private string oldfullPathToMaximaCfg = "";

        private string nameOfMaximaXml = "maxima.xml";
        private string fullPathToMaximaXml = "";
        private string oldfullPathToMaximaXml = "";

        private string nameOfSmathMac = "smath.mac";
        private string fullPathToSmathMac = "";
        private string nameOfSmathLisp = "smath.lisp";
        private string fullPathToSmathLisp = "";
        private string nameOfMaximaLogFile = "Maxima.log";
        private string fullPathToMaximaLogFile = "";
        private string pathsForMaxima = "";
        private string nameOfLoadFile = "load.mac";
        private string fullPathToLoadFile = "";
 
        //Process vars
        private Process maximaProcess = null;
        private bool maximaConnected = false;
        //States
        private string maximaState = "\"Initializing error. Please reinitialize under settigs.\"";
        private string maximaStateClosed = "\"Maxima was closed.\"";
        private string maximaStateRunning = "\"Maxima started successfully.\"";
        private string maximaStateReInitialized = "\"Maxima was re-initialized\"";
        private string maximaStateCannotStart = "\"Cannot start maxima.\")";
        private string maximaStateRestart = "\"Restart complete.\"";
        //Other vars
        private string receiveString="";
        private int timeToWait = 100;

        private int maximaInitComandsSize = 4;
        private string[] maximaInitComands = null;

        //Version control init in ReadConfigFile()
        private string lastPluginRev = "0";
        private string currentPluginRev = "0";
        private string lastPluginBuild = "0";
        private string currentPluginBuild = "0";

        private int fileReadArraySize = 10;
        private string[] fileReadArray = null;
        private string pathToSearchFile = "";
        private bool searchSucces = false;

        #endregion
        public AutoMaxima()
        {
            fileReadArray = new string[fileReadArraySize];
            for (int l = 0; l < fileReadArraySize; l++)
            {
                fileReadArray[l] = "0";
            }
            //Initialize paths
/*            if (!System.IO.Directory.Exists(assemblyFolder + "\\maximacfg"))
            {
                DirectoryInfo di = Directory.CreateDirectory(assemblyFolder + "\\maximacfg");
            }*/
            assemblyFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            pathToMaximaConfigFiles = assemblyFolder + "\\"; //maximacfg\\";  //System.Threading.Thread.GetDomain().BaseDirectory;
            fullPathToMaximaCfg = pathToMaximaConfigFiles + nameOfMaximaCfg;
            oldfullPathToMaximaCfg = searchConfigFile(fullPathToMaximaCfg, nameOfMaximaCfg);

            fullPathToMaximaXml = pathToMaximaConfigFiles + nameOfMaximaXml;
            oldfullPathToMaximaXml = searchConfigFile(fullPathToMaximaXml, nameOfMaximaXml);

            //Get version Info
            ReadConfigFile(oldfullPathToMaximaCfg);
            //---Read config for Path to maxima
            pathToMaximaBat = fileReadArray[0];
            fullPathToMaximaBat = pathToMaximaBat + "\\" + nameOfMaximaBat;
            fullPathToSmathMac = pathToMaximaConfigFiles + nameOfSmathMac;
            fullPathToSmathLisp = pathToMaximaConfigFiles + nameOfSmathLisp;
            fullPathToLoadFile = pathToMaximaConfigFiles + nameOfLoadFile;
            //--Log Files Path
            fullPathToMaximaLogFile = Directory.GetCurrentDirectory() + "\\" + nameOfMaximaLogFile;
            //----Prepare paths for send to maxima 
            pathsForMaxima = pathToMaximaConfigFiles;
            pathsForMaxima = pathsForMaxima.Replace(@"\", "/");

           
            maximaInitComands = new string[maximaInitComandsSize];
            maximaInitComands[0] = "load(\"" + pathsForMaxima + nameOfLoadFile + "\")$";
            maximaInitComands[1] = "future1$";//"load(\"" + pathsForMaxima + nameOfSmathLisp + "\")$";
            maximaInitComands[2] = "future2$";//"load(\"" + pathsForMaxima + nameOfSmathMac + "\")$";
            maximaInitComands[3] = "future3$";//"to_sm_display()$";
            
            
            
            maximaStateCannotStart = String.Format("error(\"Cannot start maxima. Please run MaximaControl(init{0}search path)\")", GlobalParams.CurrentArgumentsSeparator);
            //Try to start maxima from Path in config file
            StartWithPathFromConfigFile();
        }
        public string GetMaximaStateRunning() { return maximaStateRunning;}
        public string GetPathToMaximabat() { return fullPathToMaximaBat; }
        public void CloseMaxima()
        {
            if (maximaConnected)
            {
                System.Threading.Thread.Sleep(timeToWait);
                maximaSocket.SendSingleCommand("quit();");
                maximaConnected = false;
                maximaState = maximaStateClosed;
                //maximaProcess.Close();
                if (!maximaProcess.HasExited) maximaProcess.Kill();
            }
        }
        public string CleanupMaxima()
        {
            if (maximaConnected)
            {
                SendAndReceiveFromSocket("reset(all)");
                SendAndReceiveFromSocket("kill(all)");
                ClearFullLog();
                SendInitCmd();
                SetRunState();
            }
            if (maximaState == maximaStateRunning)
                return maximaStateRestart;
            else return maximaState;
        }
        public string RestartMaxima()
        {
            if (maximaConnected)
            {
                CloseMaxima();

                maximaSocket.ClearFullLog();
                StartAndConnectMaxima();
            }
            if (maximaState == maximaStateRunning)
                return maximaStateRestart;
            else return maximaState;
        }
        public void KillMaxima() { if (!maximaProcess.HasExited) maximaProcess.Kill(); }
        public string InitializeMaxima(string startSearchPath)
        {
            if (!maximaConnected)
            {
                if (startSearchPath != "NONE")
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(startSearchPath);
                    SearchFile(di, nameOfMaximaBat);
                    pathToMaximaBat = pathToSearchFile;
                }
                CreatConfigFile(fullPathToMaximaCfg, 
                    pathToMaximaBat +"\r\n" + 
                    currentPluginRev + "\r\n" + 
                    currentPluginBuild + "\r\n" + 
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                fullPathToMaximaBat = pathToMaximaBat + "\\" + nameOfMaximaBat;
                oldfullPathToMaximaCfg = fullPathToMaximaCfg;
                if (File.Exists(fullPathToMaximaCfg) && File.Exists(fullPathToMaximaBat) && File.Exists(fullPathToSmathMac) && File.Exists(fullPathToSmathLisp) && File.Exists(fullPathToLoadFile))
                StartWithPathFromConfigFile();
            }
            return maximaState;
        }
        public string setNewPathToMaxima(string startSearchPath) 
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(startSearchPath);
            SearchFile(di, nameOfMaximaBat);
            if ((File.Exists(pathToSearchFile + "\\" + nameOfMaximaBat)))
            {
                pathToMaximaBat = pathToSearchFile;
                fullPathToMaximaBat = pathToMaximaBat + "\\" + nameOfMaximaBat;
                CreatConfigFile(fullPathToMaximaCfg,
                pathToMaximaBat + "\r\n" +
                currentPluginRev + "\r\n" +
                currentPluginBuild + "\r\n" +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                StartWithPathFromConfigFile();
                InitializeMaxima(fullPathToMaximaCfg);
                return fullPathToMaximaCfg;
            }
            else return "CannotFind";
        }
        public string SendAndReceiveFromSocket(string sendString)
        {
            if (maximaConnected)
            {
                maximaState = maximaStateRunning;
                receiveString = maximaSocket.SendAndReceive(sendString + ";");
               // receiveString = receiveString.Replace("\"", "");
                return receiveString;
            }
            else
            {
                return maximaState;
            }
        }
        public string searchConfigFile(string filename, string fullPathFile)
        {
            if (File.Exists(fullPathFile))
            {
                return fullPathFile;
            }
            else
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(assemblyFolder);
                SearchFile(di.Parent, filename);
                if (searchSucces)
                {
                    string returns = pathToSearchFile + "\\" + filename;
                    searchSucces = false;
                    return returns;
                }
            }
            return fullPathFile;
        }
        public string ReceiveSingleCommandFromSocket()
        {
            return maximaSocket.ReceiveSingleCommand();
        }
        public void SendSingleCommandToSocket(string data)
        {
            maximaSocket.SendSingleCommand(data + ";");
        }
        public string SendInitCmd() 
        {
            if (maximaConnected)
            {
                for (int l = 0; l < maximaInitComandsSize; l++)
                {
                    maximaSocket.SendSingleCommand(maximaInitComands[l]);
                }
                maximaState = maximaStateReInitialized;
                return maximaSocket.ReceiveSingleCommand();
            }
            return "Initializing error";
        }
        public void SetRunState() { maximaState = maximaStateRunning; }
        public string GetLastLog() { return maximaSocket.GetLastLog(); }
        public string GetFullLog() { return maximaSocket.GetFullLog(); }
        public void ClearFullLog() 
        {
            if (maximaConnected)
            {
                maximaSocket.ClearFullLog();
            }
        }
        public string GetState() { return maximaState; }
        public void WriteAnything(string fileName, string data) { CreatConfigFile(fileName, data); }
        public string WriteLog() { CreatConfigFile(fullPathToMaximaLogFile, maximaSocket.GetFullLog()); return fullPathToMaximaLogFile; }
        private void StartWithPathFromConfigFile()
        {
            ReadConfigFile(oldfullPathToMaximaCfg);

            bool buildl = (Convert.ToInt64(lastPluginBuild) > Convert.ToInt64(currentPluginBuild));
            bool builde = (Convert.ToInt64(lastPluginBuild) == Convert.ToInt64(currentPluginBuild));
            bool rev = (Convert.ToInt64(lastPluginRev) >= Convert.ToInt64(currentPluginRev));
            bool aktuelleVersion = buildl || builde && rev;

            if (File.Exists(fullPathToMaximaCfg) && File.Exists(fullPathToSmathMac) && File.Exists(fullPathToSmathLisp) && File.Exists(fullPathToLoadFile) && aktuelleVersion)
            {
                if (File.Exists(fullPathToMaximaBat)) StartAndConnectMaxima();
                else maximaState = maximaStateCannotStart;
            }
            else InitializeMaxima("NONE");
        }
        private void StartAndConnectMaxima()
        {
            if (!maximaConnected)
            {
                string tempstring = "";
                int i = 0;
                Regex rxMxOut = new Regex(@"(\(.[soi][0-9]+\))");

                maximaSocket = new AutoSocket();
                System.Threading.Thread.Sleep(50);
                maximaProcess = new Process();
                maximaProcess.StartInfo.FileName = fullPathToMaximaBat;
                maximaProcess.StartInfo.Arguments = "-s " + Convert.ToString(maximaSocket.GetAutoSocketPort());
                maximaProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                maximaProcess.Start();
                System.Threading.Thread.Sleep(timeToWait);
                maximaSocket.ConnectClient();
                maximaConnected = true;
                do
                {
                    System.Threading.Thread.Sleep(timeToWait*2);
                    tempstring = maximaSocket.ReceiveSingleCommand();
                    i++;
                } while (!rxMxOut.IsMatch(tempstring, 0) && i < 50);
                tempstring = SendInitCmd();
                i = 0;
                while (!rxMxOut.IsMatch(tempstring, 0) && i < 50)
                {
                   System.Threading.Thread.Sleep(timeToWait*2);
                   tempstring = maximaSocket.ReceiveSingleCommand();
                   i++;
                }
                SendSingleCommandToSocket("check_command");
                tempstring = maximaSocket.ReceiveSingleCommand();
                while (!rxMxOut.IsMatch(tempstring, 0) && i < 50)
                {
                    System.Threading.Thread.Sleep(timeToWait * 2);
                    tempstring = maximaSocket.ReceiveSingleCommand();
                    i++;
                } 
                maximaState = maximaStateRunning;
            }
        }
        private void SearchFile(System.IO.DirectoryInfo root, string filename)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            try
            {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (System.IO.DirectoryNotFoundException)
            {
            }
            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    if (fi.Name == filename) { pathToSearchFile = fi.DirectoryName; searchSucces = true; }
                }
                subDirs = root.GetDirectories();
                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    SearchFile(dirInfo, filename);
                }
            }
        }
        private string ReadConfigFile(string fullPathToFile)
        {
            currentPluginRev = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();
            lastPluginRev = fileReadArray[1];
            currentPluginBuild = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
            lastPluginBuild = fileReadArray[2];

            if (File.Exists(fullPathToFile))
            {
                StreamReader myFileRead = new StreamReader(fullPathToFile, System.Text.Encoding.Default);
                int i = 0;
                while ((fileReadArray[i] = myFileRead.ReadLine()) != null && i < fileReadArraySize)
                {
                    i++;
                }
                myFileRead.Close();
                pathToMaximaBat = fileReadArray[0];
                lastPluginRev = fileReadArray[1];
                pathToMaximaBat = fileReadArray[0];
                lastPluginBuild = fileReadArray[2];

                return "FILE READ";
            }
            else
            {
                return "NO FILE";
            }
        }
        private void CreatConfigFile(string fullFilePath, string content)
        {
            StreamWriter myFileWrite = new StreamWriter(fullFilePath);
            myFileWrite.Write(content);
            myFileWrite.Close();
            DleteOldFiles();
            copyResources();
        }
        private void DleteOldFiles()
        {
            string[] pathsOld = new string[4];
            string oldCfgPath= System.Threading.Thread.GetDomain().BaseDirectory;
            pathsOld[0] = oldCfgPath + "maxima.inf";
            pathsOld[1] = oldCfgPath + "smath.mac";
            pathsOld[2] = oldCfgPath + "smath.lisp";
            pathsOld[3] = oldCfgPath + "Maxima.log";

            for (int l = 0;l<4;l++)
            {
                if (File.Exists(pathsOld[l]))
                {
                    File.Delete(pathsOld[l]);
                }
            }
        }
        private void copyResources()
        {
            System.IO.File.WriteAllBytes(fullPathToSmathMac, MaximaPlugin.Resource1.smathm);
            System.IO.File.WriteAllBytes(fullPathToSmathLisp, MaximaPlugin.Resource1.smathl);
            System.IO.File.WriteAllBytes(fullPathToLoadFile, MaximaPlugin.Resource1.load);
        }




        List<string> settings;
        List<string> commands;
        List<ExpressionStore> exprSMathToMaxima;
        List<ExpressionStore> exprMaximaToSMath;


        public void xmlGetPath(string pathToOldFile)
        {
            xmlRead(pathToOldFile);
            xmlWrite();
        }


        public void xmlCopy(string pathToOldFile)
        {
            xmlRead(pathToOldFile);
            xmlWrite();
        }
        public void xmlWrite()
        {

            settings = new List<string>(){
            pathToMaximaBat,
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString(),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString(),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };


            commands = new List<string>
            {
            "display2d:false$",
            "linel:700$",
            "load(abs_integrate)$",
            "load(vect)$",
            "load(mnewton)$",
            "simpsum:true$",
            "logsimp:true$",
            "logb(x,y):=log(x)/log(y)$el(x,y):=x[y]$",
            "el(x,[y]):=if equal(length(y),1) then x[y[1]] else x[y[1],y[2]]$",
            "keepfloat:true$"
            };

            if (exprSMathToMaxima.Count==0)
                exprSMathToMaxima.Add(new ExpressionStore("Your SMath functions for Maxima", "RegualrExpression", "ReplaceExpression"));

            if (exprSMathToMaxima.Count == 0)
                exprSMathToMaxima.Add(new ExpressionStore("Your Maxima functions for SMath", "RegualrExpression", "ReplaceExpression"));



            ControlObjects.XmlInterface.writeXml(settings, commands, exprSMathToMaxima, exprMaximaToSMath, 0);
            xmlRead("data.xml");
        }
        public void xmlRead(string pathToFile)
        {
            settings = new List<string>();
            commands = new List<string>();
            exprSMathToMaxima = new List<ExpressionStore>();
            exprMaximaToSMath = new List<ExpressionStore>();
            ControlObjects.ExternConverter.readXmlALL(pathToFile, settings, commands, exprSMathToMaxima, exprMaximaToSMath, 0);

            SharedFunctions.FullLog = SharedFunctions.FullLog + "\nSettings from xml file: ";
            foreach(string str in settings)
                SharedFunctions.FullLog = SharedFunctions.FullLog + "\nData " + str;

            SharedFunctions.FullLog = SharedFunctions.FullLog + "\nCommands from xml file: ";
            foreach (string str in commands)
                SharedFunctions.FullLog = SharedFunctions.FullLog + "\nData " + str;

            SharedFunctions.FullLog = SharedFunctions.FullLog + "\nExpressions from xml file: ";
            foreach (ExpressionStore str in exprSMathToMaxima)
                SharedFunctions.FullLog = SharedFunctions.FullLog + "\nData " + str.functionName + " " + str.regex + " " + str.replace ;

            foreach (ExpressionStore str in exprMaximaToSMath)
                SharedFunctions.FullLog = SharedFunctions.FullLog + "\nData " + str.functionName + " " + str.regex + " " + str.replace;


        }
    
    }
}
