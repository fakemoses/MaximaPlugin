using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;

using SMath.Manager;
using System.Threading;

namespace MaximaPlugin.ControlObjects
{
    ///// <summary>
    ///// Access to Maxima for the Plot regions
    ///// </summary>
    //public static class MaximaPluginInterface
    //{
    //    /// <summary>
    //    /// This is never called.
    //    /// </summary>
    //    /// <returns></returns>
    //    public static bool PluginInit()
    //    {
    //        return true;
    //    }

    //    /// <summary>
    //    /// Access to Maxima for the Plot regions
    //    /// </summary>
    //    /// <param name="sendString"></param>
    //    /// <param name="optionCode"></param>
    //    /// <returns></returns>
    //    public static string SendAndReceive(string sendString, string optionCode)
    //    {
    //        return Translator.Ask(sendString, optionCode);
    //    }
    //}

    /// <summary>
    /// Modifiers for the translation process
    /// </summary>
    public static class TranslationModifiers
    {
        public static bool IsExpectedEquation = false; // reformat result matrix
        public static int TimeOut = 0; // timeout in ms, 0 means no time out

        public static void Reset()
        {
            IsExpectedEquation = false;
            TimeOut = 0;
        }
    }

    /// <summary>
    /// Strings for masking special characters
    /// </summary>
    public static class Replacement
    {
        public static string Sharp = "SXhXaXrXp";
        public static string LoopVar = "LXaXuXf";
        public static string Noun = "NXoXuXn";
    } 

    /// <summary>
    /// Universal Maxima access object
    /// </summary>
    class Translator
    {
        public static Stopwatch time = new Stopwatch();

        #region Maxima access
        private static MaximaSession maxima; 
        public static bool triedToCreate = false;
        public static int maximaStartStep = 0;
        public static string Log; // Translationlog

        /// <summary>
        /// Get the session object, if it does not exist, create it.
        /// </summary>
        /// <returns>Session object</returns>
        public static MaximaSession GetMaxima()
        {
            //thread race when using direct code instead of GUI?
            if (!triedToCreate)
            { triedToCreate = true;
              maxima = new MaximaSession();
            }
            return maxima;
        }

        /// <summary>
        /// Shut down Maxima session.
        /// </summary>
        public static void CloseMaxima()
        {
            if (maxima != null)
            {
                // delete gnuplot temp dir if only one SMath process is running.
                // TODO: Gnuplot temp dirs should be specific to Maximasessions (or even better for SMath documents and not to SMath instances.
                if (System.Diagnostics.Process.GetProcessesByName("SMathStudio_Desktop").Length <= 1)
                {
                    try
                    {
                        //TODO HERE for the 2023 new delete strategy
                        System.IO.Directory.Delete(maxima.gnuPlotImageFolder, true);
                    }
                    catch //(System.IO.IOException e)
                    {
                    }
                }

                maxima.CloseMaxima();
            }
        }
        #endregion

        #region Send and receive
        public static List<string> originalStrings;
        public static List<string> receivedStrings;
        public static bool foundError = false;
        public static bool timereset = false;
        public static bool isPartAnswer = false;
        public static int receiveTries = 14; // 14 = 10,5s
        // public static int timeout = 10000; // receive-timeout in milliseconds
        
        //logging event handler
        public static event EventHandler LogChanged;

        /// <summary>
        /// Issue a single command to Maxima and get the result
        /// </summary>
        /// <param name="sendString">command</param>
        public static string Ask(string sendString)
        {
            //init session if not available
            GetMaxima().StartSession();
            //check if maxima is there
            if (GetMaxima().GetState() == GetMaxima().GetMaximaStateRunning())
            {
                time.Reset();
                time.Start();
                sendString = TranslateToMaxima(sendString);
                maxima.SendSingleCommandToSocket(sendString);
                List<string> answers = Receive();
                string outputString = TranslateToSMath(answers);
                //event to tell the log something changed
                if(LogChanged != null)
                    LogChanged.Invoke(null, EventArgs.Empty);

                TranslationModifiers.Reset();
                return outputString;
            }
            else return maxima.GetState();
        }

        /// <summary>
        /// Try to get the response from Maxima. Catch special answers and generate replies if needed
        /// </summary>
        /// <returns>List of response strings</returns>
        public static List<string> Receive()
        {
            int i = 0;
            receivedStrings = new List<string>();
            string tempstring = "";
            Regex rxMxOut = new Regex(@"(\(.[i][0-9A-F]+\))");
            do
            {
                System.Threading.Thread.Sleep(50 * i);
                tempstring = maxima.ReceiveSingleCommandFromSocket();
                tempstring = tempstring.Substring(1, (tempstring.Length - 2)); // remove \" at beginning and end
                // postprocessing
                if (tempstring != "NoDataAvailable")
                {
                    // if something was found
                    Log = Log + "\n    Received from Maxima (try number: [" + Convert.ToString(i) + "] | elapsed time: [" + time.ElapsedMilliseconds + "ms]):\n" + tempstring.Replace("\n","");
                    // see if action has to be taken prior to back-translation
                    Filter(tempstring);
                }
                //if (timereset) { i = 0; timereset = false; }
                //i++;
                if (i < 20) i++;

                //} while (i < receiveTries && !rxMxOut.IsMatch(tempstring, 0) && !foundError);
                if (ControlObjects.TranslationModifiers.TimeOut != 0 && time.ElapsedMilliseconds > ControlObjects.TranslationModifiers.TimeOut) return receivedStrings;
            } while (! rxMxOut.IsMatch(tempstring, 0) && ! foundError);
            foundError = false;
            return receivedStrings;
        }
        #endregion
        
        #region Filter
        //Prefilter
        public static void Filter(string stringToFilter)
        {
            Regex rxPosNegQuest = new Regex(@"(Is\s)", RegexOptions.None);
            Regex rxMxStartOut = new Regex(@"(\(.[o][0-9A-F]+\))");
            Regex rxMxEndOut = new Regex(@"(\(.[i][0-9A-F]+\))"); // End Tag for response
            Regex rxError = new Regex(@"(\ssyntax\s)|(\serror\s)", RegexOptions.None);
            Regex rxRedundant = new Regex(@"redundant", RegexOptions.None); ;
            Regex rxDivergent = new Regex(@"divergent", RegexOptions.None); ;
            Regex rxUnit = new Regex(@"unit");
            Regex rxRedefine = new Regex(@"(\sredefining)|(\sredefined)");

            if(isPartAnswer)  // partial answer
            {
                if (receivedStrings.Count > 0) // responselist not empty? Then add string to last response
                    receivedStrings[receivedStrings.Count - 1] = receivedStrings[receivedStrings.Count - 1] + stringToFilter;
                
                if (rxMxEndOut.IsMatch(stringToFilter, 0)) isPartAnswer = false; // if end-tag found, response is complete.
            }
            else if (rxPosNegQuest.IsMatch(stringToFilter, 0)) // Does Maxima ask for pos/neg?
            {
                maxima.SendSingleCommandToSocket("positive"); // send default response, 
                timereset = true; //reset waiting time
                if (! rxUnit.IsMatch(stringToFilter, 0)) // if the question was not about a unit, add a message to the output.
                    PositivNegativZeroFilter(stringToFilter); 
            }
            else if (rxError.IsMatch(stringToFilter, 0)) // was the output an error message?
            {
                receivedStrings.Clear();
                receivedStrings.Add("error(\"[Maxima]: " + stringToFilter + "\")");
                foundError = true;
            }
            else if (rxDivergent.IsMatch(stringToFilter, 0)) // message cleanup
            {
                receivedStrings.Add("divergent");
            }
            else if (rxRedundant.IsMatch(stringToFilter, 0)) // message cleanup
            {
                receivedStrings.Add("redundant");
            }
            else if (rxMxStartOut.IsMatch(stringToFilter, 0)) // begin of output?
            {
                if (! rxMxEndOut.IsMatch(stringToFilter, 0)) // end of output missing?
                    isPartAnswer = true;
                InfoTermFilter(stringToFilter); // handle the in/out labels
            }
            else if(!rxRedefine.IsMatch(stringToFilter, 0)) // reformat eventual 'redefining' message
            {
                receivedStrings.Add(Symbols.StringChar + stringToFilter.Replace("\"", "").TrimStart('\r', '\n', ' ').TrimEnd('\r', '\n', ' ') + Symbols.StringChar);
            }

        }
        
        /// <summary>
        /// Get the output label from the string. 
        /// TODO: very old code, use regex to do the job
        /// </summary>
        /// <param name="stringToFilter"></param>
        public static void InfoTermFilter(string stringToFilter)
        {
            int startNum = 0, endNum = 0;
            for (int i = 0; i < stringToFilter.Length; i++)
            {
                if (i < stringToFilter.Length - 5 &&
                    stringToFilter[i + 0] == '(' &&
                    stringToFilter[i + 1] == '%' &&
                  (stringToFilter[i + 2] == 'o') &&
                    Char.IsDigit(stringToFilter[i + 3]))
                {
                    i+=3;
                    startNum = i;
                    while (i < stringToFilter.Length && Char.IsDigit(stringToFilter[i]))
                    {
                        i++;
                    }
                    endNum = i;
                    if (stringToFilter[i]==')')
                    {
                        if (i > 10) receivedStrings.Add("\"" + stringToFilter.Substring(0, startNum-3).Replace("\"", "").TrimStart('\r', '\n', ' ').TrimEnd('\r', '\n', ' ') + "\"");
                        receivedStrings.Add(stringToFilter.Substring(startNum - 3, stringToFilter.Length - (startNum - 3)));
                        maximaOutNum=Convert.ToInt32(stringToFilter.Substring(startNum,endNum-startNum));
                    }
                }
            }
        }

        /// <summary>
        /// Generate the message for automatic pos/neg/zero assumptions
        /// </summary>
        /// <param name="stringToFilter"></param>
        public static void PositivNegativZeroFilter(string stringToFilter)
        {
            string tempstring = "";
            int start = 0, end = 0;
            for (int i = 0; i < stringToFilter.Length; i++)
            {
                if (i < stringToFilter.Length - 4 &&
                    stringToFilter[i + 0] == 'I' &&
                    stringToFilter[i + 1] == 's' &&
                    stringToFilter[i + 2] == ' ')
                {
                    i = i + 3;
                    start = i;
                    do { i++; } while (stringToFilter[i] != ' ' && i < stringToFilter.Length);
                    i--;
                    end = i;
                    tempstring = stringToFilter.Substring(start, (end + 1) - start);
                    tempstring = "\"" + tempstring + " is assumed to be positive.\"";
                    receivedStrings.Add(tempstring);
                }
            }
        }
        #endregion

        #region Converting
        public static int maximaOutNum = 0;

        //STRINGHANDLE FUNCTIONS
        public static List<string> GetStringsOutAndReplaceThem(List<string> answers)
        {
            List<string> replacement = new List<string>();
            List<string> data = new List<string>();
            int start = 0, end = 0;
            //Step to every answers
            for (int i = 0; i < answers.Count; i++)
            {
                //First: get strings out
                start = 0;
                //all chars in answer
                for (int k = 0; k < answers[i].Length; k++)
                {
                    end = k;
                    if (answers[i][k] == '"' && k < answers[i].Length-1)
                    {
                        data.Add(answers[i].Substring(start, (end) - start));
                        start = k;
                        do
                        {
                            k++;
                        } while (answers[i][k] != '"' && k < answers[i].Length-1);
                        end = k;
                        originalStrings.Add(answers[i].Substring(start, (end+1) - start));
                        data.Add("PlaceForString");
                        start = k+1;
                    }

                    if (k == answers[i].Length-1)
                    {

                        data.Add(answers[i].Substring(start, (end+1) - start));
                    }
                }
                replacement.Add("");
                for (int j = 0; j < data.Count; j++)
                {
                    replacement[i] = replacement[i] + data[j];
                }
                data.Clear();
            }
            return replacement;
        }
        public static List<string> PutOriginalStringsIn(List<string> answers)
        {
            List<string> data = new List<string>();
            int start = 0, end = 0, counter=0;
            for (int i = 0; i < answers.Count; i++)
            {
                start = 0;
                for (int k = 0; k < answers[i].Length; k++)
                {
                    end = k;
                    if (k < answers[i].Length - 13 &&
                        answers[i][k + 0] == 'P' &&
                        answers[i][k + 1] == 'l' &&
                        answers[i][k + 2] == 'a' &&
                        answers[i][k + 3] == 'c' &&
                        answers[i][k + 4] == 'e' &&
                        answers[i][k + 5] == 'F' &&
                        answers[i][k + 6] == 'o' &&
                        answers[i][k + 7] == 'r' &&
                        answers[i][k + 8] == 'S' &&
                        answers[i][k + 9] == 't' &&
                        answers[i][k + 10] == 'r' &&
                        answers[i][k + 11] == 'i' &&
                        answers[i][k + 12] == 'n' &&
                        answers[i][k + 13] == 'g'
                        )
                    {
                        data.Add(answers[i].Substring(start, end - start));
                        data.Add(originalStrings[counter]);
                        counter++;
                        k = k + 13;
                        start = k+1;
                    }
                    else if (k == answers[i].Length - 1)
                    {
                        data.Add(answers[i].Substring(start, (k+1) - start));
                    }
                }
                answers[i] = "";
                for (int j = 0; j < data.Count; j++)
                {
                    answers[i] = answers[i] + data[j];
                }
                data.Clear();
            }
            return answers;
        }
        //TO SMATH
        public static List<string> PrepareStringsForSMath(List<string> answers)
        {
            for (int i = 0; i < answers.Count; i++)
                answers[i] = Converter.ConvertToSMath.PrepareStringsForSMath(answers[i].Trim());
            return answers;
        }
        public static List<string> PrepareTermsForSMath(List<string> answers)
        {
            for (int i = 0; i < answers.Count;i++)
                answers[i] = Converter.ConvertToSMath.PrepareTermsForSMath(answers[i].Trim().Replace("\"", "").Replace("\r\n", "").Replace("\n", ""));
            return answers;
        }

        /// <summary>
        /// Translation of received result to SMath
        /// </summary>
        /// <param name="answers"></param>
        /// <returns></returns>
        public static string TranslateToSMath(List<string> answers)
        {
            
            string outputString = "";
            originalStrings = new List<string>();

            string booleanMaximatoSMathPattern = @"if\s+([^)]+)\s+then\s+([^)]+)\s+else\s+([^\\\n(]+)";
            //deal with boolean expression
            for (int i = 0; i < answers.Count; i++)
            {

                answers[i] = Regex.Replace(answers[i], booleanMaximatoSMathPattern, "if($1, $2, $3)");
            }

            answers = GetStringsOutAndReplaceThem(answers);
            originalStrings = PrepareStringsForSMath(originalStrings);
            answers = PrepareTermsForSMath(answers);
            answers = PutOriginalStringsIn(answers);

            if (answers.Count > 1) // if more than one string, combine them into a SMath list.
            {
                //check if string has this vect message -> take only last answer
                bool isCrossProd = false;
                for(int i = 0; i < answers.Count; i++)
                {
                    if (answers[i].Contains("vect: warning: removing existing rule or rules for .."))
                        isCrossProd = true;
                }
                if (!isCrossProd)
                {
                    outputString = "sys(";
                    for (int i = 0; i < answers.Count; i++)
                    {
                        outputString = outputString + answers[i] + GlobalProfile.ArgumentsSeparatorStandard;
                    }
                    outputString = outputString + Convert.ToString(answers.Count) + GlobalProfile.ArgumentsSeparatorStandard + "1)";
                }
                else
                {
                    outputString = answers[answers.Count-1];
                }
            }
            else if (answers.Count == 1) outputString = answers[0]; // if a single string returned, just hand it over.

            // error condition seems to be that there is no answer string
            else outputString = ("error(\"[Maxima]: " + "No data available" + "\")");

            // translation log
            Log = Log + "\n    String given to SMath (elapsed time [" + time.ElapsedMilliseconds + "ms]):\n" 
                + outputString + "\n# End conversion (Maxima request number: [%" + maximaOutNum + "]) #";

            return outputString.Trim();
        }
        //TO MAXIMA
        public static List<string> PrepareStringsForMaxima(List<string> answers)
        {
            for (int i = 0; i < answers.Count; i++)
                answers[i] = Converter.ConvertToMaxima.PrepareStringsForMaxima(answers[i].Trim());
            return answers;
        }
        public static List<string> PrepareTermsForMaxima(List<string> answers)
        {
            for (int i = 0; i < answers.Count; i++)
            {
                answers[i] = Converter.ConvertToMaxima.PrepareTermsForMaxima(answers[i].Trim());
            }
            return answers;
        }
        /// <summary>
        /// Convert a string to Maxima and log the intermediate results
        /// Strings are extracted and converted separately
        /// </summary>
        /// <param name="termText"></param>
        /// 
        /// <returns></returns>
        public static string TranslateToMaxima(string termText)
        {
            //start session of not available - used in some forms which has no initial maxima session
            GetMaxima().StartSession();

            //// SMath if(condition,true,false) maybe use regex to find it
            //// in this case much easier than having to deal with 
            string pattern = @"if\(([^,\s]+)\s*,\s*([^,\s]+)\s*,\s*([^)]+)\)";
            Match match = Regex.Match(termText, pattern);

            if (match.Success)
            {
                // Extract the condition, true result, and false result
                string condition = match.Groups[1].Value.Trim();
                string trueResult = match.Groups[2].Value.Trim();
                string falseResult = match.Groups[3].Value.Trim();

                string combineResult = $"if({condition}) then {trueResult} else {falseResult}";

                termText = Regex.Replace(termText, pattern, combineResult);
            }

            originalStrings =new List<string>();
            List<string> termTextList = new List<string>() { termText };
            int x = termTextList.Count;
            Log = Log + "\n\n# Start conversion (Maxima request number: [%" + (maximaOutNum+1)  + "]) #\n    SMath request (elapsed time [" + time.ElapsedMilliseconds + "ms]):\n" + termText;
            originalStrings.Clear();
            termTextList = GetStringsOutAndReplaceThem(termTextList);
            originalStrings = PrepareStringsForMaxima(originalStrings);
            //prime extractor here?
            termTextList = PrepareTermsForMaxima(termTextList);
            termTextList = PutOriginalStringsIn(termTextList);
            int y = termTextList.Count;
            Log = Log + "\n    String given to Maxima (elapsed time [" + time.ElapsedMilliseconds + "ms]):\n" + termTextList[0];
            return termTextList[0];
        }
        #endregion
    }
}
