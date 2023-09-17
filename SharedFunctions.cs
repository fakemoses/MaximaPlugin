using System;
using System.IO;
using System.Collections.Generic;

using SMath.Manager;
using SMath.Math;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MaximaPlugin
{
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
    }
}
