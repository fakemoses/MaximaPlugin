using System;
using System.Text.RegularExpressions;
using SMath.Manager;
using SMath.Math;

namespace MaximaPlugin.MFunctions
{
    class MaximaAllround
    {

        /// <summary>
        /// Implements Maxima()
        /// </summary>
        /// <param name="root">Function name</param>
        /// <param name="args">Args list</param>
        /// <param name="context">context</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool Maxima(Term root, Term[][] args, ref Store context, ref Term[] result)
        {

            // check if debug window has to be used
            if (root.ArgsCount > 1 && args[1][0].Text.Contains("debug")) 
            {
                // use debugger
                //MForms.FormControl.formDataRePre = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
                MForms.FormControl.formDataRe = TermsConverter.ToString(args[0]);
                MForms.FormControl.formDataRePre = SharedFunctions.Proprocessing(args[0]);
                MForms.FormControl.OpenForm("ThreadDebuggerPro");
                MForms.FormControl.formSMathCalc = true;
                do { System.Threading.Thread.Sleep(1000);} while (!MForms.FormControl.formReadyState && MForms.FormControl.formWaitFor);
                result = TermsConverter.ToTerms(MForms.FormControl.formDataAn);
                MForms.FormControl.formDataAn = "\"Debugger was used\"";
                MForms.FormControl.formSMathCalc = false;
                MForms.FormControl.formReadyState = false;
                return true;
            }
            else
            {
                // don't use debugger
                // produce the input for maxima
                string stringToMaxima = SharedFunctions.Proprocessing(args[0]);
                bool isCrossProd = false;
                if(stringToMaxima.Contains("†"))
                {
                    isCrossProd = true;
                    string[] delim = stringToMaxima.Split('†');

                    string tempString = "load(vect);";

                    for (int i = 0; i < delim.Length; i++)
                    {
                        delim[i] = Regex.Replace(delim[i], @"mat\(", "[");
                        delim[i] = Regex.Replace(delim[i], @"\],\[", ",");
                        delim[i] = Regex.Replace(delim[i], @",\d+,\d+\)", "]");

                        if (i < delim.Length-1)
                            tempString += "(" + delim[i] + ")" + "†";
                        else
                            tempString += "(" + delim[i] + ")";
                    }

                    tempString += ";express(%)";

                    stringToMaxima = tempString;
                }

                string pattern = @"if\(([^,\s]+)\s*,\s*([^,\s]+)\s*,\s*([^)]+)\)";
                Match match = Regex.Match(stringToMaxima, pattern);
                if(!match.Success && !isCrossProd)
                // send input and get result
                    result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
                else if (isCrossProd)
                {
                    string outputFromMaxima = ControlObjects.Translator.Ask(stringToMaxima);

                    outputFromMaxima = Regex.Replace(outputFromMaxima, @"sys", "mat");
                    result = TermsConverter.ToTerms(outputFromMaxima);
                }
                else
                    result = TermsConverter.ToTerms(Symbols.StringChar + ControlObjects.Translator.Ask(stringToMaxima) + Symbols.StringChar);

                return true;
            }
        }
        
        /// <summary>
        /// Control functions. 
        /// Restart
        /// Cleanup
        /// Path info
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool MaximaControlF(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            ControlObjects.Translator.GetMaxima().StartSession();

            // Restart
            string arg1 = TermsConverter.ToString(args[0]);

            // helper variables for Takeover call
            Term[] tmp = TermsConverter.ToTerms("");
            Term[][] TakeoverNone = new Term[][] { TermsConverter.ToTerms("none") };

            if (arg1 == "restart" || arg1 == Symbols.StringChar + "restart" + Symbols.StringChar)
            {
                result = TermsConverter.ToTerms(ControlObjects.Translator.GetMaxima().RestartMaxima());
                // Reset the takeover state
                MaximaTakeoverF( TakeoverNone, ref context, ref tmp);
                return true;
            }
            else if (arg1 == "init" || arg1 == Symbols.StringChar + "init" + Symbols.StringChar)
            {
                result = TermsConverter.ToTerms("\"This option is obsolete. Please use Settings button in the control window.\"");
                return true;  
            }
            else if (arg1 == "cleanup" || arg1 == Symbols.StringChar + "cleanup" + Symbols.StringChar)
            {
                result = TermsConverter.ToTerms(ControlObjects.Translator.GetMaxima().CleanupMaxima());
                // Reset the takeover state
                MaximaTakeoverF(TakeoverNone, ref context, ref tmp);
                return true;
            }
            else if (arg1 == "settings" || arg1 == Symbols.StringChar + "settings" + Symbols.StringChar)
            {
                result = TermsConverter.ToTerms("\"This option is obsolete. Please use Settings button in the control window.\"");
                return true;
            }
            else if (arg1 == "surrender" || arg1 == Symbols.StringChar + "surrender" + Symbols.StringChar)
            {
                result = TermsConverter.ToTerms("\"This option is obsolete. Please use MaximaTakeover().\"");
                return true;
            }
            else if (arg1 == "takeover" || arg1 == Symbols.StringChar + "takeover" + Symbols.StringChar)
            {
                result = TermsConverter.ToTerms("\"This option is obsolete. Please use MaximaTakeover().\"");
                return true;
            } if(arg1 == "cleanupFolder" || arg1 == Symbols.StringChar + "cleanupFolder" + Symbols.StringChar)
            {
                //folder deletion
                try
                {
                    System.IO.Directory.Delete(ControlObjects.Translator.GetMaxima().namedDrawImageFolder, true);
                    result = TermsConverter.ToTerms("\"Deletetion successful.\"");
                    return true;
                }
                catch //(System.IO.IOException e)
                {
                    result = TermsConverter.ToTerms("\"Error: Unable to delete the folder. Please close any application that uses the files in the folder.\"");
                    return true;
                }
            }
            else
            {
                if (ControlObjects.Translator.GetMaxima().GetState() == ControlObjects.Translator.GetMaxima().GetMaximaStateRunning())
                    result = TermsConverter.ToTerms(Symbols.StringChar + ControlObjects.Translator.GetMaxima().GetPathToMaximabat() + Symbols.StringChar);
                else
                    result = TermsConverter.ToTerms(ControlObjects.Translator.GetMaxima().GetState());

                return true;
            }
        }

        /// <summary>
        /// >Implements MaximaDefine.
        /// If called with a single argument, define the symbol in Maxima using it's value in SMath.
        /// If called with two arguments, define the symbol in Maxima using the second argument.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool MaximaDefine(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            string Name = TermsConverter.ToString(args[0]);
            string Value = SharedFunctions.Proprocessing(args[root.ArgsCount - 1]);
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(Name + ":" + Value));
            return true;
        }


        /// <summary>
        /// Implements MaximaTakeover(). If a function is to be taken over, an appropriate definition is added to the context, if it is released, the 
        /// the definition is removed.
        /// TODO:
        /// - return the unevaluated function call if the arguments aren't strings
        /// - make the return message nice
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">message</param>
        /// <returns>true</returns>
        public static bool MaximaTakeoverF(Term[][] args, ref Store context, ref Term[] result)
         {
            // List of available definitions
            String[] Functions = new String[]
            {
                "Diff(f__)",
                "Diff(f__,x__)",
                "Diff(f__,x__,n__)",
                "Int(f__,x__)",
                "Int(f__,x__,a__,b__)",
                "Lim(f__,x__,x0__)",
                "Sum(f__,var__,a__,b__)",
                "Det(m__)",
            };

            // Try to ensure maxima is running
            ControlObjects.Translator.GetMaxima().StartSession();
            //m.StartSession();
            // Check if Maxima is running and complain otherwise
            if (ControlObjects.Translator.GetMaxima().GetState() != ControlObjects.Translator.GetMaxima().GetMaximaStateRunning())
            {
                result = TermsConverter.ToTerms(ControlObjects.Translator.GetMaxima().GetState());
                return true;
            }

            // Generate a string with all args concatenated
            var ArgString = ""; 
            foreach (Term[] arg in args) ArgString = ArgString + TermsConverter.ToString((Computation.Preprocessing(arg, ref context))); 

            // Loop over the available functions
            string message = "";
            foreach (String Function in Functions)
            {
                string LhsString = Function.ToLower();
                Entry lhs = Entry.Create(TermsConverter.ToTerms(LhsString));

                if (ArgString.Contains(LhsString.Substring(0, 3)) || ArgString.Contains("all"))
                {
                        // add definition
                        Entry rhs = Entry.Create(TermsConverter.ToTerms(Function));
                        context.AddDefinition(lhs, rhs);
                    // add description (doesn't work)
                    //context[context.Count-1].Description =  "description of "+ LhsString ;
                    // augment message if needed
                    string MsgPart = LhsString.Split('(')[0] + "(), ";
                    if (!message.Contains(MsgPart)) message += MsgPart;             
                } 
                else
                {
                    // remove definition
                    context.Clear(lhs);
                }
            }

            // Make result string
            if (message.Length == 0) message = "All functions handled by SMath"; 
            else message = message.Substring(0, message.Length - 2) + " handled by Maxima";

            result = TermsConverter.ToTerms(Symbols.StringChar + message + Symbols.StringChar);
            return true;
        }


        /// <summary>
        /// Implements MaximaLog()
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool MaximaLog(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            string arg1 = TermsConverter.ToString(args[0]);
            string data = "";
            MaximaSession session = ControlObjects.Translator.GetMaxima();

            if (arg1 == "all" || arg1 == Symbols.StringChar + "all" + Symbols.StringChar)
            {
                // FullLog, this is collected in MaximaSocket class.
                data = session.socket.fullLog.Replace("\"", "$");
                result = TermsConverter.ToTerms(Symbols.StringChar + data +  Symbols.StringChar);
                return true;
            }
            else if (arg1 == "clear" || arg1 == Symbols.StringChar + "clear" + Symbols.StringChar)
            {
                session.socket.ClearFullLog("Cleared by MaximaLog(clear)");
                MaximaSocket.wxmLog = "";
                result = TermsConverter.ToTerms(Symbols.StringChar + "Logdata cleared" + Symbols.StringChar);
                return true;
            }
            else if (arg1 == "write" || arg1 == Symbols.StringChar + "write" + Symbols.StringChar)
            {
                result = TermsConverter.ToTerms("\"This command is obsolete. Please use buttons in Log-window.\"");
                return true;
            }
            else if (arg1 == "big" || arg1 == Symbols.StringChar + "big" + Symbols.StringChar)
            {
                MForms.FormControl.OpenForm("ThreadLogPro");
                result = TermsConverter.ToTerms(Symbols.StringChar + "Log window opened" + Symbols.StringChar);
                return true;
            }
            else if (arg1 == "saveWXM" || arg1 == Symbols.StringChar + "saveWXM" + Symbols.StringChar)
            {
                MaximaSocket.WriteWXM();
                result = TermsConverter.ToTerms("\"File saved as: commands.wxm\"");
                return true;
            }
            else
            {
                data = session.GetLastLog().Replace("\"", "$");
                //data = data + "\nSMath get: " + SharedFunctions.lastSMathGet;
                //data = data.Replace("\"", "$");
                result = TermsConverter.ToTerms(Symbols.StringChar + data + Symbols.StringChar);
                return true;
            }
        }
    }
}
