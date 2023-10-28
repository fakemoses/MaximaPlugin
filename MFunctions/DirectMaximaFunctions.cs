using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using SMath.Manager;
using SMath.Math;
using SMath.Math.Numeric;
using System.Linq;
using System.Net.NetworkInformation;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

namespace MaximaPlugin.MFunctions
{
    /// <summary>
    /// Contains methods to process mathematical expression and draw functions
    /// </summary>
    class DirectMaximaFunctions
    {
        public static int pictureCounter = 0;

        /// <summary>
        /// Implementation of the symbolic solve functions Solve(), LinSolve(), AlgSys()
        /// </summary>
        /// <param name="root">Name of the function (should be "Solve")</param>
        /// <param name="args">List of argument expressions</param>
        /// <param name="context">Context for preprocessing </param>
        /// <param name="result">result expression</param>
        /// <returns></returns>
        public static bool SymbSolve(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            var arg1 = SharedFunctions.Proprocessing(args[0]); // 
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            // both arguments can be provided as matrix or lists. Internally, they are lists
            arg1 = arg1.Replace("mat(", "sys(");
            arg2 = arg2.Replace("mat(", "sys(");
            // scalar arguments are converted to lists
            if (!arg1.Contains("sys(")) arg1 = "sys(" + arg1 + ",1,1)";
            if (!arg2.Contains("sys(")) arg2 = "sys(" + arg2 + ",1,1)";
            // send string to Maxima
            string stringToMaxima = root.Text.ToLower() + "(" + arg1 + "," + arg2 + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implementation of the ODE.2 function.
        /// </summary>
        /// <param name="root">Name of the function (should be "ODE.2")</param>
        /// <param name="args">List of argument expressions</param>
        /// <param name="context">Context for preprocessing </param>
        /// <param name="result">result expression</param>
        /// <returns></returns>
        public static bool Ode2M(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            var arg1 = TermsConverter.ToString(args[0]);
            var arg2 = TermsConverter.ToString(args[1]);
            var arg3 = TermsConverter.ToString(args[2]);
            string stringToMaxima = "ode2(" + arg1 + "," + arg2 + "," + arg3 + ")";
            // tag diff( as noun 
            stringToMaxima = stringToMaxima.Replace("diff(", ControlObjects.Replacement.Noun + "diff(");
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }


        /// <summary>
        /// Implements the function mNewton.
        /// </summary>
        /// <param name="root">Name of the function (should be "mNewton")</param>
        /// <param name="args">List of argument expressions</param>
        /// <param name="context">Context for preprocessing </param>
        /// <param name="result">result expression</param>
        /// <returns></returns>
        public static bool mNewton(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            // regex to find SMath list, sys(
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);

            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);

            arg1 = arg1.Replace("mat(", "sys(");
            arg2 = arg2.Replace("mat(", "sys(");
            arg3 = arg3.Replace("mat(", "sys(");

            // wrap scalar input with SMath list
            if (!rxSys.IsMatch(arg1, 0)) arg1 = "sys(" + arg1 + ",1,1)";
            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
            if (!rxSys.IsMatch(arg3, 0)) arg3 = "sys(" + arg3 + ",1,1)";

            // append all input as a single string
            string stringToMaxima = "mnewton(" + arg1 + "," + arg2 + "," + arg3 + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;

            // send to Maxima and retrieve result
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implements the function Fit.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Fit(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);
            var arg4 = SharedFunctions.Proprocessing(args[3]);
            var arg5 = SharedFunctions.Proprocessing(args[4]);

            // Make lists from matrices and scalars
            arg2 = arg2.Replace("mat(", "sys(");
            arg4 = arg4.Replace("mat(", "sys(");
            arg5 = arg5.Replace("mat(", "sys(");

            // wrap scalar input with SMath list
            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
            if (!rxSys.IsMatch(arg4, 0)) arg4 = "sys(" + arg4 + ",1,1)";
            if (!rxSys.IsMatch(arg5, 0)) arg5 = "sys(" + arg5 + ",1,1)";

            // append all input as a single string
            string stringToMaxima = "Fit(" + arg1 + "," + arg2 + "," + arg3 + "," + arg4 + "," + arg5 + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;

            // send to Maxima and retrieve result
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implements the function Fit(6).
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Fit6(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);
            var arg4 = SharedFunctions.Proprocessing(args[3]);
            var arg5 = SharedFunctions.Proprocessing(args[4]);
            var arg6 = SharedFunctions.Proprocessing(args[5]);

            // Make lists from matrices and scalars
            arg2 = arg2.Replace("mat(", "sys(");
            arg4 = arg4.Replace("mat(", "sys(");
            arg5 = arg5.Replace("mat(", "sys(");

            // wrap scalar input with SMath list
            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
            if (!rxSys.IsMatch(arg4, 0)) arg4 = "sys(" + arg4 + ",1,1)";
            if (!rxSys.IsMatch(arg5, 0)) arg5 = "sys(" + arg5 + ",1,1)";

            // append all input as a single string
            string stringToMaxima = "Fit6(" + arg1 + "," + arg2 + "," + arg3 + "," + arg4 + "," + arg5 + "," + arg6 + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;

            // send to Maxima and retrieve result
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implements the function MSE(3) and MSE(4).
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool MSE(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);

            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);
            // Make lists from matrices and scalars
            arg2 = arg2.Replace("mat(", "sys(");

            // wrap scalar input with SMath list
            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";

            // append all input as a single string
            string stringToMaxima = "MSE(" + arg1 + "," + arg2 + "," + arg3;

            if (root.ArgsCount == 4)
            {
                var arg4 = SharedFunctions.Proprocessing(args[3]);
                if (!rxSys.IsMatch(arg4, 0)) arg4 = "sys(" + arg4 + ",1,1)";
                stringToMaxima = stringToMaxima + "," + arg4;
            }

            stringToMaxima = stringToMaxima + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;

            // send to Maxima and retrieve result
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implements the function Residuals(3) and Residuals(4).
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Residuals(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);

            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);

            // Make lists from matrices and scalars
            arg2 = arg2.Replace("mat(", "sys(");

            // wrap scalar input with SMath list
            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";

            // append all input as a single string
            string stringToMaxima = "Residuals(" + arg1 + "," + arg2 + "," + arg3;

            if (root.ArgsCount == 4)
            {
                var arg4 = SharedFunctions.Proprocessing(args[3]);
                if (!rxSys.IsMatch(arg4, 0)) arg4 = "sys(" + arg4 + ",1,1)";
                stringToMaxima = stringToMaxima + "," + arg4;
            }

            stringToMaxima = stringToMaxima + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;

            // send to Maxima and retrieve result
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }


        /// <summary>
        /// Implements Cross product.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CrossProduct(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);

            // append all input as a single string of cross product command
            string stringtoMaxima = "(" + arg1 + ")" + "†" + "(" + arg2 + ");" + "express(%)";

            // send to Maxima and retrieve result
            string outputFromMaxima = ControlObjects.Translator.Ask(stringtoMaxima);

            //replace SMath list with mat
            outputFromMaxima = Regex.Replace(outputFromMaxima,@"sys", "mat");
            result = TermsConverter.ToTerms(outputFromMaxima);
            return true;
        }


        /// <summary>
        /// Implements Draw2D() and Draw3D()
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Draw(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            PlotImage.PlotStore plotStore = new PlotImage.PlotStore();

            // set some initial plot options
            plotStore.xRangeS = PlotImage.PlotStore.State.Disable;
            plotStore.yRangeS = PlotImage.PlotStore.State.Disable;
            plotStore.zRangeS = PlotImage.PlotStore.State.Disable;
            plotStore.gridState = PlotImage.PlotStore.State.Disable;
            plotStore.view = PlotImage.PlotStore.State.Disable;

            // set type of drawing
            if (root.Text == "Draw2D")
                plotStore.plotType = PlotImage.PlotStore.PlotType.plot2D;
            else
                plotStore.plotType = PlotImage.PlotStore.PlotType.plot3D;

            // boolean expression check
            string arg1 = SharedFunctions.Proprocessing(args[0]);

            // tag "polar, spherical, cylindrical" as noun
            arg1 = arg1.Replace("polar(", ControlObjects.Replacement.Noun + "polar(");
            arg1 = arg1.Replace("spherical(", ControlObjects.Replacement.Noun + "spherical(");
            arg1 = arg1.Replace("cylindrical(", ControlObjects.Replacement.Noun + "cylindrical(");

            // check if there is any user-inserted enhanced3d. Incase it is set to false
            Match isEnhanced3D = Regex.Match(arg1, @"enhanced3d≡(false|none)");
            if (isEnhanced3D.Success)
                plotStore.pm3d = PlotImage.PlotStore.State.Disable;


            // process first argument (draw commands and objects)
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
            Regex rxUnit = new Regex(@"(['][\w\d]+)", RegexOptions.None);
            Regex rxSize = new Regex(@"sys\((\d+),(\d+),2,1\)");
            
            
            // enclose argument in list if it is scalar
            if (!rxSys.IsMatch(arg1, 0)) arg1 = "sys(" + arg1 + ",1,1)";

            // user_preamble extraction here
            string preamble;
            arg1 = SharedFunctions.ExtractUserPreamble(arg1, out preamble);
            if (preamble != "")
                preamble = SharedFunctions.RemoveSysFromPreamble(preamble);

            // neutralize units by replacing with 1
            arg1 = rxUnit.Replace(arg1, "1");


            // Create gnuplot folder in SMath Working folder
            string TempPath = ControlObjects.Translator.GetMaxima().gnuPlotImageFolder;
            System.IO.Directory.CreateDirectory(TempPath);

            //create the Image folder permanent image if not exist
            string permPath = ControlObjects.Translator.GetMaxima().namedDrawImageFolder;
            if (!Directory.Exists(permPath))
            {
                Directory.CreateDirectory(permPath);
            }

            // by default this is the path unless name is given
            string RandomFileName = GenerateRandomString(8);
            string FilePath = Path.Combine(TempPath, RandomFileName);

            // prepare size and filename based on drawing type
            string arg2 = "";
            if (root.Text == "Draw3D")
            {
                plotStore.width = 300;
                plotStore.height = 300;
            }
            else
            {
                plotStore.width = 300;
                plotStore.height = 240;
            }

            // set default terminal
            string term = "pdf"; 
            string Target = "";
            string backupFile = "";


            // use random file name when only one arguments
            if (root.ArgsCount == 1)
            {
                plotStore.filename = FilePath;
                plotStore.termType = PlotImage.PlotStore.TermType.pdf;
                FilePath = FilePath + "." + term;
            }

            // Filename or size given
            if (root.ArgsCount == 2) 
            {
                // preprocess second argument
                arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
                Target = arg2.Replace("\"", "");

                //if given name is not just an extension name
                if (Target != "pdf" && Target != "png" && Target != "svg")
                {
                    if (!rxSys.IsMatch(arg2)) // argument is not a list, i.e. filename is given and not only file extension 
                    {

                        // Replace specific Unicode escape sequences with their corresponding characters
                        string convertedString = SharedFunctions.ReplaceUnicodeEscapeSequences(Target);

                        convertedString = convertedString.Replace("\\", "");

                        FilePath = Path.Combine(permPath, convertedString);

                        //set value in plotstore
                        plotStore.filename = Path.ChangeExtension(FilePath, null);

                        if (FilePath.EndsWith("png"))
                            plotStore.termType = PlotImage.PlotStore.TermType.png;
                        else if(FilePath.EndsWith("svg"))
                            plotStore.termType = PlotImage.PlotStore.TermType.svg;
                        else
                            plotStore.termType = PlotImage.PlotStore.TermType.pdf;

                        //check if file exists. If yes, convert to bck.
                        backupFile = CreateBackupFilePath(FilePath);
                    }
                    else // size is given
                    {
                        //some size extractor here
                        int[] size = GetSizeofImage(Target);

                        plotStore.width = size[0];
                        plotStore.height = size[1];

                        //set plotstore
                        plotStore.filename = FilePath;
                        plotStore.termType = PlotImage.PlotStore.TermType.pdf;

                        //file path
                        FilePath = FilePath + "." + term;
                    }
                }
                else
                {
                    //only extension is given as filename. Eg: "pdf"
                    term = Target;

                    //set value in plotstore
                    plotStore.filename = Path.ChangeExtension(FilePath, null);

                    FilePath = FilePath + "." + term;

                    if (FilePath.EndsWith("png"))
                        plotStore.termType = PlotImage.PlotStore.TermType.png;
                    else if (FilePath.EndsWith("svg"))
                        plotStore.termType = PlotImage.PlotStore.TermType.svg;
                    else
                        plotStore.termType = PlotImage.PlotStore.TermType.pdf;
                }
            }
            else if (root.ArgsCount == 3) // filename and size are given
            {
                // second argument is file name
                arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
                Target = arg2.Replace("\"", "");

                if (Target != "pdf" && Target != "svg" && Target != "png")
                {
                    // Replace specific Unicode escape sequences with their corresponding characters
                    string convertedString = SharedFunctions.ReplaceUnicodeEscapeSequences(Target);

                    convertedString = convertedString.Replace("\\", "");

                    FilePath = Path.Combine(permPath, convertedString);

                    //set value in plotstore
                    plotStore.filename = Path.ChangeExtension(FilePath, null);

                    if (FilePath.EndsWith("png"))
                        plotStore.termType = PlotImage.PlotStore.TermType.png;
                    else if (FilePath.EndsWith("svg"))
                        plotStore.termType = PlotImage.PlotStore.TermType.svg;
                    else
                        plotStore.termType = PlotImage.PlotStore.TermType.pdf;

                    //check if file exists. If yes, convert to bck.
                    backupFile = CreateBackupFilePath(FilePath);
                }
                else
                {
                    //only extension is given as filename
                    term = Target;

                    //set value in plotstore
                    plotStore.filename = Path.ChangeExtension(FilePath, null);

                    FilePath = FilePath + "." + term;

                    if (FilePath.EndsWith("png"))
                        plotStore.termType = PlotImage.PlotStore.TermType.png;
                    else if (FilePath.EndsWith("svg"))
                        plotStore.termType = PlotImage.PlotStore.TermType.svg;
                    else
                        plotStore.termType = PlotImage.PlotStore.TermType.pdf;
                }


                // third argument is size
                string sizeString = TermsConverter.ToString(Computation.Preprocessing(args[2], ref context));
                int[] size = GetSizeofImage(sizeString);

                plotStore.width = size[0];
                plotStore.height = size[1];

            }

            //for use later
            string fullFilePath = FilePath;


            //makelists
            plotStore.MakeLists();

            // Extract literal strings here!
            MaximaPlugin.ControlObjects.Translator.originalStrings = new List<string>();
            List<string> sl = MaximaPlugin.ControlObjects.Translator.GetStringsOutAndReplaceThem(new List<string>() { arg1 });

            // convert Matrices and Lists
            MaximaPlugin.Converter.ElementStoreManager esm = new MaximaPlugin.Converter.ElementStoreManager();
            esm = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.SMathListDataCollection(esm, sl[0]);

            // if preamble is given by user
            if (preamble != "")
            {
                // Check if the preamble contains more than one part
                int numSeparatorOccurrences = preamble.Split(new[] { GlobalProfile.ArgumentsSeparatorStandard }, StringSplitOptions.None).Length - 1;
                if (numSeparatorOccurrences > 0)
                {
                    // Split the preamble using the separator (',') into multiple parts
                    string[] preambleParts = preamble.Split(new[] { GlobalProfile.ArgumentsSeparatorStandard }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < preambleParts.Length; i++)
                    {
                        plotStore.prambleList.Add(preambleParts[i]);
                    }
                }
                else
                {
                    plotStore.prambleList.Add(preamble);
                }

            }

            // generate list of commands 
            MaximaPlugin.PlotImage.Draw.ListHandle(esm, plotStore.commandList, plotStore.prambleList);
            MaximaPlugin.PlotImage.Draw.terminate(esm, plotStore.commandList, plotStore.prambleList);
            
            // convert the generated list into a string
            string send = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.MakeTermString(esm, "", "");

            // put back literal string
            sl = MaximaPlugin.ControlObjects.Translator.PutOriginalStringsIn(new List<string>() { send });

            // send to Maxima
            ControlObjects.TranslationModifiers.TimeOut = 5000;
            string res = ControlObjects.Translator.Ask(root.Text.ToLower() + "(" + sl[0] + ")");

            //check if file exists, if yes check if it has content. No content means there is error.
            // Error image is generated using bitmap
            string errorImg = Path.ChangeExtension(fullFilePath, "png");
            string errormsg = "";

            if (!File.Exists(fullFilePath))
            {
                // file does not exists - return error image
                if (backupFile != "")
                    errormsg = "Error: " + res + "\n" + "Backup file can be found at: " + backupFile;
                else
                    errormsg = "Error: " + res;
                GenerateErrorMsgAsImage(errormsg, errorImg);
                result = TermsConverter.ToTerms(Symbols.StringChar + errorImg + Symbols.StringChar);
                return true;
            }
            else
            {
                long fileSize = new FileInfo(fullFilePath).Length;

                // if fileSize is smaller than 0, there is error. Return error image
                if (fileSize < 0)
                {
                    if (backupFile != "")
                        errormsg = "Error: " + res + "\n" + "Backup file can be found at: " + backupFile;
                    else
                        errormsg = "Error: " + res;
                    GenerateErrorMsgAsImage(errormsg, errorImg);
                    result = TermsConverter.ToTerms(Symbols.StringChar + errorImg + Symbols.StringChar);
                    return true;
                }
                result = TermsConverter.ToTerms(Symbols.StringChar + fullFilePath + Symbols.StringChar);
                return true;
            }
        }


        #region helper function
        /// <summary>
        /// Generates random file name for Draw method
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Extract size of image from the SMath list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static int[] GetSizeofImage(string input)
        {
            int[] numbers = new int[2];

            // Define a regular expression pattern to match numbers
            string pattern = @"\d+";

            // Use Regex.Match to find all matches of the pattern in the input string
            MatchCollection matches = Regex.Matches(input, pattern);

            // Convert the first two matched numbers to integers
            if (matches.Count >= 2)
            {
                numbers[0] = int.Parse(matches[0].Value);
                numbers[1] = int.Parse(matches[1].Value);
            }

            return numbers;
        }

        /// <summary>
        /// Generate error text as image and save it into the give filePath
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static void GenerateErrorMsgAsImage(string text, string filePath)
        {
            // Create a new Bitmap with the desired width and height based on the text size
            using (Bitmap bitmap = CreateBitmapFromText(text))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    // Set the background color and clear the bitmap
                    graphics.Clear(Color.White);

                    // Set the font and brush for the text
                    Font font = new Font("Arial", 8, FontStyle.Regular);
                    Brush brush = new SolidBrush(Color.Black);

                    // Draw the text on the bitmap
                    graphics.DrawString(text, font, brush, new PointF(10, 10));

                    // Save the bitmap as an image file
                    bitmap.Save(filePath, ImageFormat.Png);
                }
            }
        }

        /// <summary>
        /// Helper function to help GenerateErrorMsgAsImage() method to create an image
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static Bitmap CreateBitmapFromText(string text)
        {
            using (Font font = new Font("Arial", 8, FontStyle.Regular))
            {
                using (Bitmap tempBitmap = new Bitmap(1, 1))
                {
                    using (Graphics tempGraphics = Graphics.FromImage(tempBitmap))
                    {
                        // Measure the size of the text
                        SizeF textSize = tempGraphics.MeasureString(text, font);

                        // Adjust the bitmap size based on the text size
                        int width = (int)textSize.Width + 20; // Add extra padding
                        int height = (int)textSize.Height + 20; // Add extra padding

                        // Create the bitmap with the adjusted size
                        return new Bitmap(width, height);
                    }
                }
            }
        }

        /// <summary>
        /// Create a backup image from the existing image with "_bck" in the file name
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static string CreateBackupFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // File does not exist, return the original path
                return filePath;
            }

            // File exists, add "_bck" to the filename before the extension
            string directory = Path.GetDirectoryName(filePath);
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            string backupFileName = $"{filenameWithoutExtension}_bck{extension}";
            string backupFilePath = Path.Combine(directory, backupFileName);

            if (!File.Exists(backupFilePath))
            {
                // File exists, rename it to the backup file name
                File.Move(filePath, backupFilePath);
            }
            else
            {
                // previous backup file exists. Replace them
                // File exists, rename it to the backup file name
                File.Replace(filePath, backupFilePath, null);
            }

            return backupFilePath;
        }

        #endregion

    }
}
