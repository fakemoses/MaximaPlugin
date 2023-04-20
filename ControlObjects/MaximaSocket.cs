using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;


namespace MaximaPlugin
{   
    class MaximaSocket
    {
        #region VARS
        //socket vars
        private Socket server = null;
        private Socket client = null;
        private int port = 4555;
        private bool connected = false;
        //Encoding SocketEncoding = Encoding.ASCII;
        //Encoding SocketEncoding = Encoding.GetEncoding("iso-8859-1");
        private Encoding encoding = new UTF8Encoding();
        //communication vars
        private int timeToWait=25;
        private int receiveTries = 5;
        // log containers
        private string lastLog = "";
        private string lastPrompt = "";
        public string fullLog = "";
        public static string wxmLog = "";
        #endregion

        #region wxmStuff
        // constant strings for WXM logging
        public static string wxmFileStart = "/* [wxMaxima batch file version 1] [ DO NOT EDIT BY HAND! ]*/\r\n";
        public static string wxmStartCmd = "\r\n/* [wxMaxima: input   start ] */\r\n";
        public static string wxmEndCmd = "\r\n/* [wxMaxima: input   end   ] */\r\n";
        public static string wxmFileEnd = "\r\n\"Created with SMath\"$";

        /// <summary>
        /// Write session log as wxMaxima file (e.g. for further examination in Maxima). This consists of three parts:
        /// 1. Standard startup commands
        /// 2. Custom startup commands
        /// 3. Log of the interactive commands
        /// </summary>
        public static string WriteWXM()
        {
            string tmp = wxmFileStart;
            for (int k = 1; k < MaximaPlugin.ControlObjects.Translator.GetMaxima().commands.Count; k++)
            {
                tmp = tmp + wxmStartCmd + MaximaPlugin.ControlObjects.Translator.GetMaxima().commands[k] + wxmEndCmd;
            }
            for (int k = 0; k < MaximaPlugin.ControlObjects.Translator.GetMaxima().customCommands.Count; k++)
            {
                tmp = tmp + wxmStartCmd + MaximaPlugin.ControlObjects.Translator.GetMaxima().customCommands[k] + wxmEndCmd;
            }
            string destination = Path.Combine(ControlObjects.Translator.GetMaxima().WorkingFolderPath(), "commands.wxm");
            SharedFunctions.WriteDataToFile(destination, tmp + wxmLog + wxmFileEnd);
            return destination;
        }
        #endregion

        #region CONTROL
        /// <summary>
        /// Constructor
        /// </summary>
        public MaximaSocket() 
        {
            //instanceCounter = instanceCounter + 1;
            IPAddress autoSocketIp = new IPAddress(new byte[] { 127, 0, 0, 1 });
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(autoSocketIp, 0));
            server.Listen(1);
            port = ((IPEndPoint)server.LocalEndPoint).Port;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MaximaSocket() 
        {
            connected = false;
            server.Close();
        }

        /// <summary>
        /// Start socket server
        /// </summary>
        //private void StartSocketServer()
        //{
        //    IPAddress autoSocketIp = new IPAddress(new byte[] { 127, 0, 0, 1 });
        //    autoSocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    autoSocketServer.Bind(new IPEndPoint(autoSocketIp, 0));
        //    autoSocketServer.Listen(1);
        //    autoSocketPort = ((IPEndPoint)autoSocketServer.LocalEndPoint).Port;
        //}

        /// <summary>
        /// Connect client
        /// </summary>
        public void ConnectClient() 
        {
            if (connected == false || !client.Connected)
            {
                client = server.Accept();
                connected = true;
            }
        }

        /// <summary>
        /// Expose socket port number
        /// </summary>
        /// <returns>Port</returns>
        public int GetPort() 
        {
            return port;
        }
        #endregion

        #region LOGS
        /// <summary>
        /// Expose log entry for last command
        /// </summary>
        /// <returns>lastlog</returns>
        public string GetLastLog() 
        {
            return lastLog;
        }

        

        /// <summary>
        /// Clears the full log
        /// </summary>
        /// <param name="message">String to write to the beginning of the cleared log</param>
        public void ClearFullLog(string message)
        {
            fullLog = message + "\n";
        }
        #endregion

        #region SEND AND RECEIVING

        /// <summary>
        /// Send and log a command to Maxima. 
        /// </summary>
        /// <param name="sendString">String to send</param>
        /// <param name="option">Set this to "check" to bypass logging of the command.</param>
        public void SendSingleCommand(string sendString, string option = "none")
        {
            // Log command
            if (option != "check")
            {
                fullLog = fullLog + sendString;
                lastLog = lastPrompt + sendString;
                wxmLog = wxmLog + wxmStartCmd + sendString + wxmEndCmd;
            }
            // Send command
            //Byte[] sendBytes = Encoding.Default.GetBytes(sendString);
            Byte[] sendBytes = encoding.GetBytes(sendString);
            if (client.Connected) client.Send(sendBytes, sendBytes.Length, SocketFlags.None);
            else
            {
                //Do something usefull. Restart?
            }
        }

        /// <summary>
        /// Receive and log a single response from Maxima. 
        /// </summary>
        /// <param name="option">Set this to "check" to bypass logging to fulllog.</param>
        /// <returns>Response string</returns>
        public string ReceiveSingleResponse(string option = "none")
        {
            int  attempt = 0;
            while (client.Available < 2 && attempt < receiveTries)
            {
                System.Threading.Thread.Sleep(timeToWait);
                attempt++;
            }
            if (client.Available > 2)
            {
                Byte[] rawBytes = new Byte[client.Available];
                int numberOfBytes = client.Receive(rawBytes);
                string ResponseText = encoding.GetString(rawBytes);
                // cut away the new input promt and keep it for the next lastlog
                int index = ResponseText.LastIndexOf("\n(%i");
                string toLastLog = ResponseText;
                if (index != -1)
                {
                    lastPrompt = ResponseText.Substring(index);
                    toLastLog = ResponseText.Remove(index);
                }
                // logging (response isn't logged to WXM)
                if (option != "check")
                {
                    fullLog = fullLog + ResponseText;
                    lastLog = "Received Bytes: " + Convert.ToString(numberOfBytes) + lastLog + toLastLog;
                }
                // make response string   
                if (numberOfBytes > 2) return "\"" + ResponseText + "\"";
            }
            return ("-NoDataAvailable-");
        }

        /// <summary>
        /// Send a command to Maxima and receive the response.
        /// Log both
        /// </summary>
        /// <param name="sendString">Command</param>
        /// <param name="option">Set this to "check" to bypass the log</param>
        /// <returns>response</returns>
        //public string SendAndReceive(string sendString, string option = "none")
        //{
        //    SendSingleCommand(sendString, option);
        //    return ReceiveSingleResponse(option);
        //}
        #endregion
    } 
}
