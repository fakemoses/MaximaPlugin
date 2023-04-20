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

namespace MaximaPlugin.MFunctions
{
    class SingleRohling
    {
        static public string functionName = "Maxima";
        static public TermInfo GetTermInfos()
		{
            return new TermInfo("Maxima", TermType.Function, "[maxima] Send any expression to Maxima", FunctionSections.Unknown, true);
		}
        static public bool IsSupport(Term root)
        {
            if (root.Text == functionName) return true;
            return false;
        }
        static public bool LowLevelEvaluation(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            string stringToMaxima = "";
            string tempString = "";
            string arg1 = TermsConverter.ToString(args[0]);
            int i = 0;

            if (arg1 == "future_option" || arg1 == Symbols.StringChar + "future_option" + Symbols.StringChar)
            {
                result = TermsConverter.ToTerms(Symbols.StringChar + MaximaControl.RestartMaxima() + Symbols.StringChar);
                return true;
            }
            else
            {
                while (i < root.ArgsCount)
                {
                    tempString = TermsConverter.ToString(Computation.Preprocessing(args[i], ref context));
                    Match found = Regex.Match(tempString, "\".+\"", RegexOptions.None);
                    //       GlobalProfile.ArgumentsSeparatorStandard;
                    if (found.Success)
                    {
                        tempString = tempString.Replace(Symbols.StringChar, "");
                    }
                    else
                    {
                        tempString = ConvertStrings.PrepareForMaxima(tempString, 1);
                    }
                    if (i == 0)
                        stringToMaxima = tempString;
                    else
                        stringToMaxima = stringToMaxima + "," + tempString;
                    i++;
                }
                string answerFromMaxima = SharedFunctions.CheckedSendAndReceive(stringToMaxima + ";");
                return SharedFunctions.ResultOutput(answerFromMaxima, ref context, ref result);
            }
        }
    }
}
