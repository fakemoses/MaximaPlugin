using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;

using SMath.Manager;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace MaximaPlugin.ControlObjects
{
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
        public static string Log = ""; // Translationlog

        /// <summary>
        /// Get the session object, if it does not exist, create it.
        /// </summary>
        /// <returns>Session object</returns>
        public static MaximaSession GetMaxima()
        {
            //thread race when using direct code instead of GUI?
            if (!triedToCreate)
            {
                triedToCreate = true;
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
        public static int receiveTries = 14; 

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
                if (LogChanged != null)
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
                    Log = Log + "\n    Received from Maxima (try number: [" + Convert.ToString(i) + "] | elapsed time: [" + time.ElapsedMilliseconds + "ms]):\n" + tempstring.Replace("\n", "");
                    // see if action has to be taken prior to back-translation
                    Filter(tempstring);
                }

                if (tempstring.Contains("NoDataAvailable") && GetMaxima().GetLastLog().Contains("incorrect syntax"))
                {
                    Thread staThread = new Thread(() =>
                    {
                        MessageBox.Show("Syntax error: Maxima hangs due to incorrect input. Restart the session by clicking Insert > Maxima > Restart from the menu bar.",
                            "Maxima Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    });
                    staThread.SetApartmentState(ApartmentState.STA);
                    staThread.Start();
                    break;
                }

                if (i < receiveTries) i++;

                //} while (i < receiveTries && !rxMxOut.IsMatch(tempstring, 0) && !foundError);
                if (ControlObjects.TranslationModifiers.TimeOut != 0 && time.ElapsedMilliseconds > ControlObjects.TranslationModifiers.TimeOut) return receivedStrings;
            } while (!rxMxOut.IsMatch(tempstring, 0) && !foundError && i < receiveTries);
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

            if (isPartAnswer)  // partial answer
            {
                if (receivedStrings.Count > 0) // responselist not empty? Then add string to last response
                    receivedStrings[receivedStrings.Count - 1] = receivedStrings[receivedStrings.Count - 1] + stringToFilter;

                if (rxMxEndOut.IsMatch(stringToFilter, 0)) isPartAnswer = false; // if end-tag found, response is complete.
            }
            else if (rxPosNegQuest.IsMatch(stringToFilter, 0)) // Does Maxima ask for pos/neg?
            {
                maxima.SendSingleCommandToSocket("positive"); // send default response, 
                timereset = true; //reset waiting time
                if (!rxUnit.IsMatch(stringToFilter, 0)) // if the question was not about a unit, add a message to the output.
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
                if (!rxMxEndOut.IsMatch(stringToFilter, 0)) // end of output missing?
                    isPartAnswer = true;
                InfoTermFilter(stringToFilter); // handle the in/out labels
            }
            else if (!rxRedefine.IsMatch(stringToFilter, 0)) // reformat eventual 'redefining' message
            {
                receivedStrings.Add(Symbols.StringChar + stringToFilter.Replace("\"", "").TrimStart('\r', '\n', ' ').TrimEnd('\r', '\n', ' ') + Symbols.StringChar);
            }

        }

        /// <summary>
        /// Get the output label from the string. 
        /// </summary>
        /// <param name="stringToFilter"></param>

        public static void InfoTermFilter(string stringToFilter)
        {
            // remove leading spaces
            stringToFilter = stringToFilter.TrimStart('\r', '\n', ' ');

            // grouping the results into few groups
            // First group is to check if the string exists
            // Second group is to check if there is any leading text before (%o
            // Third group is to extract the number after (%o
            Match match = Regex.Match(stringToFilter, @"(.*)\(%o(\d+)\)", RegexOptions.Singleline);
            if (match.Success)
            {
                if (match.Groups[1].Value != "")
                {
                    receivedStrings.Add("\"" + match.Groups[1].Value.TrimStart('\r', '\n', ' ').TrimEnd('\r', '\n', ' ') + "\"");
                    stringToFilter = stringToFilter.Replace(match.Groups[1].Value, "");
                }
                receivedStrings.Add(stringToFilter);
                maximaOutNum = Convert.ToInt32(match.Groups[2].Value);
            }

        }

        /// <summary>
        /// Generate the message for automatic pos/neg/zero assumptions
        /// </summary>
        /// <param name="stringToFilter"></param>

        public static void PositivNegativZeroFilter(string stringToFilter)
        {
            string tempstring = "";
            // Example string: Is a*b*(a*b-1) positive, negative or zero?
            // First group is to check if the string starts with "Is"
            // Second group is to extract anything between spaces after "Is"
            Match match = Regex.Match(stringToFilter, @"^Is\s+([^ ]+)");
            if (match.Success)
            {
                if (match.Groups[0].Value.Contains("Is"))
                {
                    tempstring = "\"" + match.Groups[1].Value + " is assumed to be positive.\"";
                    receivedStrings.Add(tempstring);
                }
            }
        }
        #endregion
        #region Converting
        public static int maximaOutNum = 0;

        ////STRINGHANDLE FUNCTIONS

        public static List<string> GetStringsOutAndReplaceThem(List<string> answers)
        {
            List<string> replacement = new List<string>();

            for (int i = 0; i < answers.Count; i++)
            {
                string tmp = answers[i];

                // Regex searches for anything that is between two double quotes within the string
                // and replaces it with "PlaceForString"
                // MatchCollection is used because there are multiple matches in one string
                MatchCollection matches = Regex.Matches(answers[i], @"""(.*?)""");
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        originalStrings.Add("\"" + match.Groups[1].Value + "\"");
                        tmp = tmp.Replace("\"" + match.Groups[1].Value + "\"", "PlaceForString");
                    }
                }
                if(matches.Count == 0)
                    replacement.Add(answers[i]);
                else replacement.Add(tmp);
            }

            return replacement;
        }

        public static List<string> PutOriginalStringsIn(List<string> answers)
        {
            //using regex replace to replace the "PlaceForString" with the original string one by one
            var regex = new Regex(@"PlaceForString");
            for (int i = 0; i < answers.Count; i++)
            {
                for(int j = 0; j < originalStrings.Count; j++)
                {
                    answers[i] = regex.Replace(answers[i],originalStrings[j],1);
                }
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
            for (int i = 0; i < answers.Count; i++)
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

            //check if this is a case where Maxima sends Cross results one by one and got appended by the filter
            // if case to handle multiple outputs but appended in a single line due to the Filter (only for cross case)
            if (answers.Count == 1 && answers[0].Contains("~"))
            {
                MatchCollection matches = Regex.Matches(answers[0], @"\(%o");
                if (matches.Count > 1)
                {
                    int lastIndex = answers[0].LastIndexOf("(%o");
                    if (lastIndex != -1)
                    {
                        answers[0] = answers[0].Substring(lastIndex);
                    }
                }
            }

            answers = GetStringsOutAndReplaceThem(answers);
            originalStrings = PrepareStringsForSMath(originalStrings);
            answers = PrepareTermsForSMath(answers);
            answers = PutOriginalStringsIn(answers);

            if (answers.Count > 1) // if more than one string, combine them into a SMath list.
            {
                //check if string is  vect. It will give 2 answers and one of it contains the ~ sign which is cross
                bool isCrossProd = false;
                for (int i = 0; i < answers.Count; i++)
                {
                    // this might change in the future but this is the only thing at the moment that can differentiate the type of the problem it solves.
                    if (answers[i].Contains("~"))
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
                    outputString = answers[answers.Count - 1];
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

            originalStrings = new List<string>();
            List<string> termTextList = new List<string>() { termText };
            int x = termTextList.Count;
            Log = Log + "\n\n# Start conversion (Maxima request number: [%" + (maximaOutNum + 1) + "]) #\n    SMath request (elapsed time [" + time.ElapsedMilliseconds + "ms]):\n" + termText;
            originalStrings.Clear();
            termTextList = GetStringsOutAndReplaceThem(termTextList);
            originalStrings = PrepareStringsForMaxima(originalStrings);
            termTextList = PrepareTermsForMaxima(termTextList);
            termTextList = PutOriginalStringsIn(termTextList);
            int y = termTextList.Count;
            Log = Log + "\n    String given to Maxima (elapsed time [" + time.ElapsedMilliseconds + "ms]):\n" + termTextList[0];
            return termTextList[0];
        }
        #endregion
    }
}
