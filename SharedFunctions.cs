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
        //Paths
        //public static string pathToSMath = "";
        //Logs
        //public static string FullLog = "";
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

        ///// <summary>
        ///// Add a text to FullLog
        ///// </summary>
        ///// <param name="text">Text to add</param>
        //public static void AddFullLog(string text)
        //{
        //    FullLog = FullLog + "\n\r LOGGING: \n\r" + text + "\n\r\n\r";
        //}

        /// <summary>
        /// Convert integer code to flag array
        /// TODO MK 2017 08 01: Merge this with unique caller 
        /// </summary>
        /// <param name="num">integer code</param>
        /// <returns>boolean array</returns>
        public static bool[] GetBooleFromInt(int num)
        {
            bool[] target = new bool[32];

            for (int i = 0; i < 32; i++)
            {
                target[i] = ((num >> i) & 1) == 1;
            }
            return target;
        }

        /// <summary>
        /// Convert boolean array to integer code
        /// TODO MK 2017 08 01: Merge this with unique caller 
        /// </summary>
        /// <param name="array">boolean array</param>
        /// <returns>Integer code</returns>
        public static int GetIntFromBool(bool[] array)
        {
            int num = 0;

            for (int i = 0; i < array.Length; ++i)
            {
                num = (num << 1) + (array[i] ? 1 : 0);
            }
            return num;
        }


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
            //findFunction = false;

            //// no preprocessing when cross product is involved (must be defined in the function's argument)
            //List<Term> xTerms = new List<Term>();
            //for (int j = 0; j < term.Length; j++)
            //{
            //    // Replace cross product symbol by a cross product marker string
            //    if (term[j].Type == TermType.Operator && term[j].ArgsCount == 2 && term[j].Text == Operators.VectorMultiplication)
            //    {
            //        xTerms.Add(new Term("VeCtOrMuLtIpLiCaTiOn", TermType.Function, 2));
            //    }
            //    else
            //    {
            //        xTerms.Add(term[j]);
            //    }
            //}
            //string text = TermsConverter.ToString(xTerms);

            //// Back-Conversion to terms representation
            //var termVar = TermsConverter.ToTerms(text);

            //// Actual preprocessing
            try
            {
                term = Computation.Preprocessing(term, ref contextHold);
                term = Computation.SymbolicCalculation(term, ref contextHold);
            }
            catch (NotDefinedException)
            {
                // Restore the un-preprocessed string, termVar turned out to be unreliable after the NotDefinedException)
                //termVar = TermsConverter.ToTerms(text);
            }
            catch (GeneralException)
            {
                // Restore the un-preprocessed string, termVar turned out to be unreliable after the NotDefinedException)
                //termVar = TermsConverter.ToTerms(text);
            }

            // Backsubstituation of the masked cross product
            //xTerms.Clear();
            //for (int j = 0; j < termVar.Length; j++)
            //{
            //    if (termVar[j].Type == TermType.Function && termVar[j].ArgsCount == 2 && termVar[j].Text == "VeCtOrMuLtIpLiCaTiOn")
            //    {
            //        xTerms.Add(new Term(Operators.VectorMultiplication, TermType.Operator, 2));
            //    }
            //    else
            //    {
            //        xTerms.Add(termVar[j]);
            //    }
            //}
            string text = TermsConverter.ToString(term);

            

            return text;
        }
        /// <summary>
        /// Preprocessing function, used by Maxima(), various direct functions and takeover functions
        /// The cross product is masked because it generates error messages if it doesn't recognize it's arguments as vectors.
        /// </summary>
        /// <param name="term">SMath expression</param>
        /// 
        /// <returns>String representation of pre-processed expression</returns>
        public static string Proprocessing_0(Term[] term)
        {
            findFunction = false;

            // no preprocessing when cross product is involved (must be defined in the function's argument)
            List<Term> xTerms = new List<Term>();
            for (int j = 0; j < term.Length; j++)
            {
                // Replace cross product symbol by a cross product marker string
                if (term[j].Type == TermType.Operator && term[j].ArgsCount == 2 && term[j].Text == Operators.VectorMultiplication)
                {
                    xTerms.Add(new Term("VeCtOrMuLtIpLiCaTiOn", TermType.Function, 2));
                }
                else
                {
                    xTerms.Add(term[j]);
                }
            }
            string text = TermsConverter.ToString(xTerms);

            // Back-Conversion to terms representation
            var termVar = TermsConverter.ToTerms(text);

            // Actual preprocessing
            try
            {
                termVar = Computation.Preprocessing(termVar, ref contextHold);
                //termVar = SMath.Math.Computation.SymbolicCalculation(termVar, ref contextHold);
            }
            catch (NotDefinedException)
            {
                // Restore the un-preprocessed string, termVar turned out to be unreliable after the NotDefinedException)
                //termVar = TermsConverter.ToTerms(text);
            }
            catch (GeneralException)
            {
                // Restore the un-preprocessed string, termVar turned out to be unreliable after the NotDefinedException)
                //termVar = TermsConverter.ToTerms(text);
            }

            // Backsubstituation of the masked cross product
            xTerms.Clear();
            for (int j = 0; j < termVar.Length; j++)
            {
                if (termVar[j].Type == TermType.Function && termVar[j].ArgsCount == 2 && termVar[j].Text == "VeCtOrMuLtIpLiCaTiOn")
                {
                    xTerms.Add(new Term(Operators.VectorMultiplication, TermType.Operator, 2));
                }
                else
                {
                    xTerms.Add(termVar[j]);
                }
            }
            text = TermsConverter.ToString(xTerms);

            return text;
        }

        #endregion

        #region File access
        static string pathToSearchFile = "";
        static bool searchSucces = false;
        /// <summary>
        /// File search function for AutoMaxima, function better goes to AutoMaxima.cs, because it is only used there.
        /// </summary>
        /// <param name="path">Path name</param>
        /// <param name="name">File name</param>
        /// <returns>Full filename if file exists, null otherwise</returns>
        public static string SearchFile(string path, string name)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            RecursiveSearch(di, name);
            if (searchSucces)
                return System.IO.Path.Combine(pathToSearchFile, name);
            else return null;
        }

        /// <summary>
        /// The actual recursive search
        /// </summary>
        /// <param name="root">Dirinfo object </param>
        /// <param name="filename">Filename</param>
        private static void RecursiveSearch(System.IO.DirectoryInfo root, string filename)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            try { files = root.GetFiles("*.*"); }
            catch (UnauthorizedAccessException) { }
            catch (System.IO.DirectoryNotFoundException) { }
            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    if (fi.Name == filename) { pathToSearchFile = fi.DirectoryName; searchSucces = true; }
                }
                subDirs = root.GetDirectories();
                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    RecursiveSearch(dirInfo, filename);
                }
            }
        }
        /// <summary>
        /// https://stackoverflow.com/questions/703281/getting-path-relative-to-the-current-working-directory
        /// </summary>
        /// <param name="filespec">Fullpath</param>
        /// <param name="folder">Reference dir</param>
        /// <returns></returns>
        public static string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        /// <summary>
        /// Generate a relative path name from the given current directory to a given absolute path name
        /// </summary>
        /// <param name="absolutPath">target path</param>
        /// <param name="currentPlace">reference location for relative path</param>
        /// <returns>relative path</returns>
        public static string MakePathRelativ(string absolutPath, string currentPlace)
        {
            string ECDpath = Environment.CurrentDirectory;
            try { absolutPath = absolutPath.Replace(currentPlace, "."); }
            catch { }
            try { absolutPath = absolutPath.Replace(Directory.GetParent(currentPlace).FullName, ".."); }
            catch { }
            try { absolutPath = absolutPath.Replace(Directory.GetParent(Directory.GetParent(currentPlace).FullName).FullName, "..."); }
            catch { }
            Environment.CurrentDirectory = ECDpath;
            return absolutPath;
        }


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
    }
}
