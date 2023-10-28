using System;
using System.IO;
using System.Collections.Generic;

using SMath.Manager;
using SMath.Math;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MaximaPlugin
{
    /// <summary>
    /// Contain functions that are used by multiple classes throughout the codebase
    /// </summary>
    public class SharedFunctions
    {
        #region GlobalVars

        public static string lastSMathGet = "";
        //SMath object holder
        public static Term rootHold = null;
        public static Term[][] argsHold = null;
        public static Store contextHold = null;
        public static Term[] resultHold = null;
        //Other control vars
        public static bool initializingOverMenue = false;
        //Pot region
        public static PlotImage.PlotStore defaultPlotValues = new PlotImage.PlotStore();

        #endregion

        #region Preprocessing
        public static bool findFunction = false;

        /// <summary>
        /// Preprocessing function, used by Maxima(), various direct functions and takeover functions
        /// The cross product is masked because it generates error messages if it doesn't recognize it's arguments as vectors.
        /// </summary>
        /// <param name="term">SMath expression</param>
        /// 
        /// <returns>String representation of pre-processed expression</returns>
        public static string Proprocessing(Term[] term)
        {

            //// Actual preprocessing
            try
            {
                term = Computation.Preprocessing(term, ref contextHold);
                term = Computation.SymbolicCalculation(term, ref contextHold);
            }
            catch (NotDefinedException)
            {
                // do nothing
            }
            catch (GeneralException)
            {
                // do nothing
            }

            string text = TermsConverter.ToString(term);

            

            return text;
        }

        #endregion

        #region File access
        /// <summary>
        /// Create a file from a string
        /// </summary>
        /// <param name="fullFilePath">file name</param>
        /// <param name="content">contents</param>
        public static void WriteDataToFile(string fullFilePath, string content)
        {
            StreamWriter myFileWrite = new StreamWriter(fullFilePath);
            myFileWrite.Write(content);
            myFileWrite.Close();
        }

        #endregion

        #region Sreen DPI
        /// <summary>
        /// Get the scale factor of the current screen for windows form scaling
        /// </summary>
        /// <returns>Float of scale factor. 100% scaling returns 1.0, 125% returns 1.25 and so on</returns>
        public static float getScreenScaleFactor()
        {
            var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            float scaleFactor = g.DpiX / GlobalProfile.ContentDpi;
            return scaleFactor;
        }

        #endregion

        #region Unicode Manipulation

        /// <summary>
        /// Replace Unicode escape sequence into it's original form
        /// </summary>
        /// <param name="input">file name</param>
        /// <returns>String with replaced unicode characters.</returns>
        public static string ReplaceUnicodeEscapeSequences(string input)
        {
            // Replace specific Unicode escape sequences with their corresponding characters
            // add more as needed
            input = input.Replace("\\005F", "_");
            input = input.Replace("\\002E", ".");
            input = input.Replace("\\0020\\", " ");
            return input;
        }
        #endregion


        #region User Preamble 

        /// <summary> 
        /// Extract user preamble from string, save ít into a variable userPreamble.
        /// The user_preamble is removed from the input string
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="userPreamble">variable to save extracted user_preamble</param>
        /// <returns>Input string without user_preamble and extracted user_preamble</returns>
        public static string ExtractUserPreamble(string input, out string userPreamble)
        {
            // Regular expression pattern to match user_preamble components
            string pattern = @"user_preamble\s*≡\s*(sys\s*\([^)]+\)|\""[^""]+\""),?";

            // Find the first match using regex
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                // Extract the user_preamble component from the match
                userPreamble = match.Groups[1].Value;

                // Remove the user_preamble from the input string along with the trailing comma
                string postProcessedInput = Regex.Replace(input, pattern, "");

                return postProcessedInput;
            }

            // If no user_preamble found, return the original input string
            userPreamble = string.Empty;
            return input;
        }

        /// <summary> 
        /// Converts SMath list, sys, to a string where preambles are separated by comma
        /// </summary>
        /// <param name="preamble">input string</param>
        /// <returns>string where preamble is separated by comma</returns>
        public static string RemoveSysFromPreamble(string preamble)
        {
            // Check if the preamble contains sys(...)
            int sysStartIndex = preamble.IndexOf("sys(", StringComparison.Ordinal);
            if (sysStartIndex >= 0)
            {
                // Find the position of the closing bracket of sys(...)
                int sysEndIndex = preamble.IndexOf(')', sysStartIndex);

                // Find the position of the second last comma
                int secondLastCommaIndex = FindCommaBeforeSecondLastIndex(preamble, sysEndIndex);

                // Remove the content between the second last comma and sys(...), including the comma
                preamble = preamble.Substring(0, secondLastCommaIndex + 2);
            }

            // Remove the entire sys(...) from the preamble
            preamble = preamble.Replace("sys(", "").TrimEnd(')').TrimEnd(',');

            // Add a quotation mark at the end if it's not already there
            if (!preamble.EndsWith("\""))
            {
                preamble += "\"";
            }

            return preamble;
        }

        /// <summary> 
        /// Converts SMath list, sys, to a string where preambles are separated by comma
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="startIndex">start index of "sys("</param>
        /// <returns>Position of the comma just before the second-to-last item</returns>
        static int FindCommaBeforeSecondLastIndex(string input, int startIndex)
        {
            int count = 0;
            int index = startIndex;

            while (count < 2 && index >= 0)
            {
                if (input[index] == ',')
                {
                    count++;
                    if (count == 2)
                    {
                        return index - 1; // Return the position of the comma just before the second-to-last item
                    }
                }

                index--;
            }

            return -1; // If there's no second-to-last item, return -1
        }
        #endregion
    }
}
