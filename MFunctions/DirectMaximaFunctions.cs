﻿using System;
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



        ///// <summary>
        ///// Implementation of the Solve function
        ///// </summary>
        ///// <param name="root">Name of the function (should be "Solve")</param>
        ///// <param name="args">List of argument expressions</param>
        ///// <param name="context">Context for preprocessing </param>
        ///// <param name="result">result expression</param>
        ///// <returns></returns>
        //public static bool SolveM(Term root, Term[][] args, ref Store context, ref Term[] result)
        //{
        //    Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
        //    /*
        //    var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
        //    var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));*/
        //    var arg1 = SharedFunctions.Proprocessing(args[0]);
        //    var arg2 = SharedFunctions.Proprocessing(args[1]);
        //    // both arguments can be provided as matrix or lists. Internally, they are lists
        //    arg1 = arg1.Replace("mat(", "sys(");
        //    arg2 = arg2.Replace("mat(", "sys(");
        //    // scalar arguments are converted to lists
        //    if (!rxSys.IsMatch(arg1, 0)) arg1 = "sys(" + arg1 + ",1,1)";
        //    if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
        //    // send string to Maxima
        //    string stringToMaxima = "solve(" + arg1 + "," + arg2 + ")";
        //    ControlObjects.TranslationModifiers.IsExpectedEquation = true;
        //    result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
        //    return true;
        //}

        //// TODO: MK 2017-07-16 Collect the solver functions into a single one

        ///// <summary>
        ///// Implementation of the LinSolve function
        ///// </summary>
        ///// <param name="root">Name of the function (should be "LinSolve")</param>
        ///// <param name="args">List of argument expressions</param>
        ///// <param name="context">Context for preprocessing </param>
        ///// <param name="result">result expression</param>
        ///// <returns></returns>
        //public static bool LinSolveM(Term root, Term[][] args, ref Store context, ref Term[] result)
        //{
        //    Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
        //    /*
        //    var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
        //    var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));*/
        //    var arg1 = SharedFunctions.Proprocessing(args[0]);
        //    var arg2 = SharedFunctions.Proprocessing(args[1]);
        //    // both arguments can be provided as matrix or lists. Internally, they are lists
        //    arg1 = arg1.Replace("mat(", "sys(");
        //    arg2 = arg2.Replace("mat(", "sys(");
        //    // scalar arguments are converted to lists
        //    if (!rxSys.IsMatch(arg1, 0)) arg1 = "sys(" + arg1 + ",1,1)";
        //    if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
        //    // send string to Maxima
        //    string stringToMaxima = "linsolve(" + arg1 + "," + arg2 + ")";
        //    ControlObjects.TranslationModifiers.IsExpectedEquation = true;
        //    result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
        //    return true;
        //}

        ///// <summary>
        ///// Implementation of the AlgSys function
        ///// </summary>
        ///// <param name="root">Name of the function (should be "AlgSys")</param>
        ///// <param name="args">List of argument expressions</param>
        ///// <param name="context">Context for preprocessing </param>
        ///// <param name="result">result expression</param>
        ///// <returns></returns>
        //public static bool AlgsysM(Term root, Term[][] args, ref Store context, ref Term[] result)
        //{
        //    Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
        //    /*
        //    var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
        //    var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));*/
        //    var arg1 = SharedFunctions.Proprocessing(args[0]);
        //    var arg2 = SharedFunctions.Proprocessing(args[1]);
        //    // both arguments can be provided as matrix or lists. Internally, they are lists
        //    arg1 = arg1.Replace("mat(", "sys(");
        //    arg2 = arg2.Replace("mat(", "sys(");
        //    // scalar arguments are converted to lists
        //    if (!rxSys.IsMatch(arg1, 0)) arg1 = "sys(" + arg1 + ",1,1)";
        //    if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
        //    // send string to Maxima
        //    string stringToMaxima = "algsys(" + arg1 + "," + arg2 + ")";
        //    ControlObjects.TranslationModifiers.IsExpectedEquation = true;
        //    result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
        //    return true;
        //}

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
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
            /*
            var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
            var arg3 = TermsConverter.ToString(Computation.Preprocessing(args[2], ref context));*/
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);
            arg1 = arg1.Replace("mat(", "sys(");
            arg2 = arg2.Replace("mat(", "sys(");
            arg3 = arg3.Replace("mat(", "sys(");
            if (!rxSys.IsMatch(arg1, 0)) arg1 = "sys(" + arg1 + ",1,1)";
            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
            if (!rxSys.IsMatch(arg3, 0)) arg3 = "sys(" + arg3 + ",1,1)";
            string stringToMaxima = "mnewton(" + arg1 + "," + arg2 + "," + arg3 + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;
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
            /*
            var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
            var arg3 = TermsConverter.ToString(Computation.Preprocessing(args[2], ref context));*/
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);
            var arg4 = SharedFunctions.Proprocessing(args[3]);
            var arg5 = SharedFunctions.Proprocessing(args[4]);

            // Make lists from matrices and scalars
            arg2 = arg2.Replace("mat(", "sys(");
            arg4 = arg4.Replace("mat(", "sys(");
            arg5 = arg5.Replace("mat(", "sys(");

            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
            if (!rxSys.IsMatch(arg4, 0)) arg4 = "sys(" + arg4 + ",1,1)";
            if (!rxSys.IsMatch(arg5, 0)) arg5 = "sys(" + arg5 + ",1,1)";

            string stringToMaxima = "Fit(" + arg1 + "," + arg2 + "," + arg3 + "," + arg4 + "," + arg5 + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;
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
            /*
            var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
            var arg3 = TermsConverter.ToString(Computation.Preprocessing(args[2], ref context));*/
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

            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
            if (!rxSys.IsMatch(arg4, 0)) arg4 = "sys(" + arg4 + ",1,1)";
            if (!rxSys.IsMatch(arg5, 0)) arg5 = "sys(" + arg5 + ",1,1)";

            string stringToMaxima = "Fit6(" + arg1 + "," + arg2 + "," + arg3 + "," + arg4 + "," + arg5 + "," + arg6 + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;
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
            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
            string stringToMaxima = "MSE(" + arg1 + "," + arg2 + "," + arg3;

            if (root.ArgsCount == 4)
            {
                var arg4 = SharedFunctions.Proprocessing(args[3]);
                if (!rxSys.IsMatch(arg4, 0)) arg4 = "sys(" + arg4 + ",1,1)";
                stringToMaxima = stringToMaxima + "," + arg4;
            }

            stringToMaxima = stringToMaxima + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;
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
            if (!rxSys.IsMatch(arg2, 0)) arg2 = "sys(" + arg2 + ",1,1)";
            string stringToMaxima = "Residuals(" + arg1 + "," + arg2 + "," + arg3;

            if (root.ArgsCount == 4)
            {
                var arg4 = SharedFunctions.Proprocessing(args[3]);
                if (!rxSys.IsMatch(arg4, 0)) arg4 = "sys(" + arg4 + ",1,1)";
                stringToMaxima = stringToMaxima + "," + arg4;
            }

            stringToMaxima = stringToMaxima + ")";
            ControlObjects.TranslationModifiers.IsExpectedEquation = true;
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }




        ////Plotting
        //public static bool findPre = false;

        ///// <summary>
        ///// Combine the standard and the user preamble in draw commands.
        ///// </summary>
        ///// <param name="text"></param>
        ///// <param name="ipre"></param>
        ///// <returns></returns>
        //public static string PreambleReplace(string text, string ipre)
        //{
        //    findPre = false;
        //    int bracketO = 0, bracketC = 0;
        //    List<string> element = new List<string>();
        //    int i = -1, start = 0, charCounter = 0;
        //    int completeStart = 0;
        //    bool flip = false;
        //    int completeEnd = 0;
        //    while (i < text.Length - 17)
        //    {
        //        i++;
        //        //text.IndexOf("user_preamble≡sys(");
        //        // TODO MK 2017 07 18 Check if that can't be reduced to a single comparison
        //        if (
        //            text[i + 0] == 'u' &&
        //            text[i + 1] == 's' &&
        //            text[i + 2] == 'e' &&
        //            text[i + 3] == 'r' &&
        //            text[i + 4] == '_' &&
        //            text[i + 5] == 'p' &&
        //            text[i + 6] == 'r' &&
        //            text[i + 7] == 'e' &&
        //            text[i + 8] == 'a' &&
        //            text[i + 9] == 'm' &&
        //            text[i + 10] == 'b' &&
        //            text[i + 11] == 'l' &&
        //            text[i + 12] == 'e' &&
        //            text[i + 13] == '≡' &&
        //            text[i + 14] == 's' &&
        //            text[i + 15] == 'y' &&
        //            text[i + 16] == 's' &&
        //            text[i + 17] == '(')
        //        {
        //            //sys((user_preamble≡sys("set term svg noenhanced size 400,400 dynamic","set style fill transparent solid 0.5 border","set view 60, 30, 1.2, 1.2",3,1))
        //            completeStart = i;
        //            i += 17; 
        //            bracketO++;
        //            findPre = true;
        //            while (i < text.Length && bracketO - bracketC > 0)
        //            {
        //                i++;
        //                start = i;
        //                charCounter = 0;
        //                while (((text[i] != GlobalProfile.ArgumentsSeparatorStandard && text[i] != ')') || flip || bracketO - bracketC > 1) && i < text.Length)
        //                {
        //                    if (text[i] == '(')
        //                        bracketO++;
        //                    else if (text[i] == ')')
        //                        bracketC++;
        //                    if (text[i] == '"' && flip)
        //                        flip = false;
        //                    else if (text[i] == '"')
        //                        flip = true;

        //                    if (bracketO - bracketC > 0) charCounter++;
        //                    i++;
        //                }
        //                if (charCounter > 0)
        //                    element.Add(text.Substring(start, charCounter));

        //                if (i < text.Length && text[i] == '(')
        //                    bracketO++;
        //                else if (i < text.Length && text[i] == ')')
        //                    bracketC++;
        //            }
        //            completeEnd = i + 1;
        //        }
        //    }



        //    if (findPre && element.Count > 0)
        //    {
        //        string tmp = "user_preamble≡sys(" + ipre + GlobalProfile.ArgumentsSeparatorStandard;
        //        for (int k = 0; k < element.Count - 2; k++)
        //        {
        //            tmp = tmp + element[k];
        //            if (k == element.Count - 3) tmp = tmp + GlobalProfile.ArgumentsSeparatorStandard + Convert.ToString(k + 3) + GlobalProfile.ArgumentsSeparatorStandard + "1)";
        //            else tmp = tmp + GlobalProfile.ArgumentsSeparatorStandard;
        //        }
        //        return MakeString(text, tmp, completeStart, completeEnd);
        //    }
        //    else if (!findPre)
        //    {
        //        i = -1;
        //        while (i < text.Length - 13)
        //        {
        //            i++;
        //            if (
        //                text[i + 0] == 'u' &&
        //                text[i + 1] == 's' &&
        //                text[i + 2] == 'e' &&
        //                text[i + 3] == 'r' &&
        //                text[i + 4] == '_' &&
        //                text[i + 5] == 'p' &&
        //                text[i + 6] == 'r' &&
        //                text[i + 7] == 'e' &&
        //                text[i + 8] == 'a' &&
        //                text[i + 9] == 'm' &&
        //                text[i + 10] == 'b' &&
        //                text[i + 11] == 'l' &&
        //                text[i + 12] == 'e' &&
        //                text[i + 13] == '≡')
        //            {
        //                //sys((user_preamble≡sys("set term svg noenhanced size 400,400 dynamic","set style fill transparent solid 0.5 border","set view 60, 30, 1.2, 1.2",3,1))
        //                completeStart = i;
        //                i += 14;
        //                findPre = true;
        //                start = i;

        //                while (((text[i] != GlobalProfile.ArgumentsSeparatorStandard) || flip || bracketO - bracketC > 0) && i < text.Length)
        //                {
        //                    if (text[i] == '(')
        //                        bracketO++;
        //                    else if (text[i] == ')')
        //                        bracketC++;
        //                    if (text[i] == '"' && flip)
        //                        flip = false;
        //                    else if (text[i] == '"')
        //                        flip = true;

        //                    charCounter++;
        //                    i++;
        //                }


        //                if (charCounter > 0)
        //                {
        //                    if (bracketO - bracketC == -1) charCounter--;
        //                    element.Add(text.Substring(start, charCounter));
        //                    completeEnd = i;
        //                }
        //            }
        //        }

        //        if (findPre && element.Count == 1)
        //        {
        //            string tmp = "user_preamble≡sys(" + ipre + GlobalProfile.ArgumentsSeparatorStandard + element[0] + GlobalProfile.ArgumentsSeparatorStandard + "3" + GlobalProfile.ArgumentsSeparatorStandard + "1)";
        //            return MakeString(text, tmp, completeStart, completeEnd).Replace("sys((user_preamble≡", "sys(user_preamble≡").Replace((GlobalProfile.ArgumentsSeparatorStandard + "(user_preamble≡"), (GlobalProfile.ArgumentsSeparatorStandard + "user_preamble≡"));
        //        }
        //    }

        //    return text;
        //}
        //public static string MakeString(string input, string newArrayString, int arrayStart, int arrayEnd)
        //{
        //    string tmp1 = input.Substring(0, arrayStart);
        //    string tmp2 = input.Substring(arrayEnd, input.Length - arrayEnd);
        //    return tmp1 + newArrayString + tmp2;
        //}



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

            //set some plot options
            plotStore.xRangeS = PlotImage.PlotStore.State.Disable;
            plotStore.yRangeS = PlotImage.PlotStore.State.Disable;
            plotStore.zRangeS = PlotImage.PlotStore.State.Disable;
            plotStore.gridState = PlotImage.PlotStore.State.Disable;
            plotStore.view = PlotImage.PlotStore.State.Disable;

            //set type of drawing
            if (root.Text == "Draw2D")
                plotStore.plotType = PlotImage.PlotStore.PlotType.plot2D;
            else
                plotStore.plotType = PlotImage.PlotStore.PlotType.plot3D;

            // boolean expression check
            string arg1 = SharedFunctions.Proprocessing(args[0]);

            //use regex to check if it contain the SMath if(condition,true,false) condition
            string pattern = @"if\(([^,\s]+)\s*,\s*([^,\s]+)\s*,\s*([^)]+)\)";
            Match match = Regex.Match(arg1, pattern);
            if (!match.Success)
            {
                // tag "polar, spherical, cylindrical" as noun
                arg1 = arg1.Replace("polar(", ControlObjects.Replacement.Noun + "polar(");
                arg1 = arg1.Replace("spherical(", ControlObjects.Replacement.Noun + "spherical(");
                arg1 = arg1.Replace("cylindrical(", ControlObjects.Replacement.Noun + "cylindrical(");
            }

            //check if there is any user-inserted enhanced3d. Incase it is set to false
            Match isEnhanced3D = Regex.Match(arg1, @"enhanced3d≡(false|none)");
            if (isEnhanced3D.Success)
                plotStore.enhanced3dState = PlotImage.PlotStore.State.Disable;


            // process first argument (draw commands and objects)
            Regex rxSys = new Regex(@"sys\(", RegexOptions.None);
            Regex rxUnit = new Regex(@"(['][\w\d]+)", RegexOptions.None);
            Regex rxSize = new Regex(@"sys\((\d+),(\d+),2,1\)");
            
            
            // enclose argument in list if it is scalar
            if (!rxSys.IsMatch(arg1, 0)) arg1 = "sys(" + arg1 + ",1,1)";

            //user_preamble extraction here
            string preamble;
            arg1 = ExtractUserPreamble(arg1, out preamble);
            if (preamble != "")
                preamble = RemoveSysFromPreamble(preamble);

            // neutralize units by replacing with 1
            arg1 = rxUnit.Replace(arg1, "1");


            // Create Temp path in SMath Working folder
            string TempPath = ControlObjects.Translator.GetMaxima().gnuPlotImageFolder;
            System.IO.Directory.CreateDirectory(TempPath);

            //create the image path if not exist
            string permPath = ControlObjects.Translator.GetMaxima().namedDrawImageFolder;
            if (!Directory.Exists(permPath))
            {
                Directory.CreateDirectory(permPath);
            }

            //by default this is the path unless name is given
            string RandomFileName = GenerateRandomString(8);
            string FilePath = Path.Combine(TempPath, RandomFileName);

            // prepare size and filename
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
            
            string term = "pdf"; // default terminal
            string Target = "";
            string backupFile = "";


            // use random file name when only one arguments -> only lists of settings
            if (root.ArgsCount == 1)
            {
                plotStore.filename = FilePath;
                plotStore.termType = PlotImage.PlotStore.TermType.pdf;
                FilePath = FilePath + "." + term;
            }

            if (root.ArgsCount == 2) // Filename or size given
            {
                // preprocess second argument
                arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
                Target = arg2.Replace("\"", "");
                if (Target != "pdf" && Target != "png" && Target != "svg")
                {
                    if (!rxSys.IsMatch(arg2)) // argument is not a list, i.e. filename is given and not only file extension 
                    {

                        // Replace specific Unicode escape sequences with their corresponding characters
                        string convertedString = ReplaceUnicodeEscapeSequences(Target);

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
            }
            else if (root.ArgsCount == 3) // filename and size are given
            {
                // second argument is file name
                arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
                Target = arg2.Replace("\"", "");

                if (Target != "pdf" && Target != "svg" && Target != "png")
                {
                    // Replace specific Unicode escape sequences with their corresponding characters
                    string convertedString = ReplaceUnicodeEscapeSequences(Target);

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


            //makelists()
            plotStore.MakeLists();

            // Extract Strings here!
            MaximaPlugin.ControlObjects.Translator.originalStrings = new List<string>();
            List<string> sl = MaximaPlugin.ControlObjects.Translator.GetStringsOutAndReplaceThem(new List<string>() { arg1 });

            // Matrices and Lists
            MaximaPlugin.Converter.ElementStoreManager esm = new MaximaPlugin.Converter.ElementStoreManager();
            esm = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.SMathListDataCollection(esm, sl[0]);

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


            ListHandle(esm, plotStore.commandList, plotStore.prambleList);
            terminate(esm, plotStore.commandList, plotStore.prambleList);
            
            // convert to Maxima
            string send = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.MakeTermString(esm, "", "");


            sl = MaximaPlugin.ControlObjects.Translator.PutOriginalStringsIn(new List<string>() { send });
            // send to Maxima
            ControlObjects.TranslationModifiers.TimeOut = 5000;
            string res = ControlObjects.Translator.Ask(root.Text.ToLower() + "(" + sl[0] + ")");

            //check if file exists, if yes check if it has content. No content means there is error.

            string errorImg = Path.ChangeExtension(fullFilePath, "png");
            string errormsg = "";

            if (!File.Exists(fullFilePath))
            {
                //error
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


        public static bool findPreL = false;

        /// <summary>
        /// List manipulation
        /// </summary>
        /// <param name="esm"></param>
        /// <param name="pres"></param>
        /// <param name="term"></param>
        /// <param name="name"></param>
        /// <param name="bgColor"></param>
        public static void ListHandle(MaximaPlugin.Converter.ElementStoreManager esm, List<string> commands, List<string> preamble)
        {
            Regex rxPreamble = new Regex(@"preamble");
            for (int i = 0; i < esm.currentStore.items; i++)
            {
                if (findPreL) break;
                for (int j = 0; j < esm.currentStore.itemData[i].Count; j++)
                {
                    if (findPreL) break;
                    else if (esm.currentStore.itemData[i][j] == esm.layerMsg)
                    {
                        esm.nextElementStore();
                        ListHandle(esm, commands, preamble);
                        esm.prevElementStore();
                    }
                    else if (rxPreamble.Match(esm.currentStore.itemData[i][0], 0).Success)
                    {
                        findPreL = true;
                        if (esm.currentStore.itemData[i].Count > 1 && esm.currentStore.itemData[i][1] == esm.layerMsg)
                        {
                            bool tmp = false;
                            if (esm.currentStore.itemData[i][0][0] == '(')
                            {
                                esm.currentStore.itemData[i][0] = "user_preamble≡";
                                tmp = true;
                            }
                            esm.nextElementStore();
                            string temp = "";
                            for (int o = 0; o < preamble.Count; o++)
                            {
                                if (o == preamble.Count - 1)
                                    temp = temp + preamble[o];
                                else
                                    temp = temp + preamble[o] + GlobalProfile.ArgumentsSeparatorStandard + " ";
                            }
                            esm.currentStore.itemData.Insert(0, (new List<string>() { temp }));
                            esm.currentStore.items++;
                            esm.currentStore.rows = esm.currentStore.items;
                            esm.currentStore.cols = esm.currentStore.items;
                            esm.currentStore.refPointer = 0;
                            esm.prevElementStore();
                            if (tmp && esm.currentStore.itemData[i].Count > 1)
                            {
                                esm.currentStore.itemData[i].RemoveAt(2);
                            }
                        }
                        else
                        {
                            string temp = esm.currentStore.itemData[i][0].Substring(14);
                            if (temp[0] == '≡') temp = temp.Substring(1);
                            int open = 0, close = 0;
                            for (int z = 0; z < temp.Length; z++)
                            {
                                if (temp[z] == '(') { open++; }
                                else if (temp[z] == ')') { close++; }
                            }
                            if (open - close == -1) temp = temp.Substring(0, temp.Length - 1);
                            esm.currentStore.itemData.RemoveAt(i);
                            string temp2 = "[";
                            for (int o = 0; o < preamble.Count; o++)
                            {
                                temp2 = temp2 + preamble[o] + GlobalProfile.ArgumentsSeparatorStandard + " ";
                            }
                            temp2 = temp2 + temp + "]";
                            esm.currentStore.itemData.Insert(0, (new List<string>() { "user_preamble≡" + temp2 }));
                        }
                    }
                }
            }
            esm.currentStore.refPointer = 0;
        }
        public static void terminate(MaximaPlugin.Converter.ElementStoreManager esm, List<string> commands, List<string> preamble)
        {
            string temp = "[";

            if (!findPreL)
            {
                temp = "[";
                for (int o = 0; o < preamble.Count; o++)
                {
                    if (o == preamble.Count - 1)
                        temp = temp + preamble[o];
                    else
                        temp = temp + preamble[o] + GlobalProfile.ArgumentsSeparatorStandard + " ";
                }
                temp = temp + "]";
                esm.currentStore.itemData.Insert(0, (new List<string>() { "user_preamble≡" + temp }));

                esm.currentStore.items++;
                esm.currentStore.rows = esm.currentStore.items;
                esm.currentStore.cols = esm.currentStore.items;

            }



            temp = "";
            for (int o = 0; o < commands.Count; o++)
            {
                if (o == commands.Count - 1)
                    temp = temp + commands[o];
                else
                    temp = temp + commands[o] + GlobalProfile.ArgumentsSeparatorStandard + " ";
            }

            esm.currentStore.itemData.Insert(0, (new List<string>() { temp }));

            esm.currentStore.items++;
            esm.currentStore.rows = esm.currentStore.items;
            esm.currentStore.cols = esm.currentStore.items;
            esm.currentStore.refPointer = 0;
            findPreL = false;
            esm.gotoFirstElementStore();

        }


        //get string from argument
        static int[] GetSizeofImage(string input)
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

        static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ReplaceUnicodeEscapeSequences(string input)
        {
            // Replace specific Unicode escape sequences with their corresponding characters
            input = input.Replace("\\005F", "_");
            input = input.Replace("\\002E", ".");
            return input;
        }

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

        // error text as image to show to the user
        static void GenerateErrorMsgAsImage(string text, string filePath)
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

        static string CreateBackupFilePath(string filePath)
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

    }
}
