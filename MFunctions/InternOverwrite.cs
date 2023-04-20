using System;
using System.Text.RegularExpressions;

using SMath.Manager;
using SMath.Math;

namespace MaximaPlugin.MFunctions
{
    class InternOverwrite
    {
        /// <summary>
        /// Implements Int(4), second argument (integration variable) is excluded from preprocessing.
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool Integrate(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            /*
            var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg2 = TermsConverter.ToString(args[1]);
            var arg3 = TermsConverter.ToString(Computation.Preprocessing(args[2], ref context));
            var arg4 = TermsConverter.ToString(Computation.Preprocessing(args[3], ref context));
            */
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            //var arg2 = TermsConverter.ToString(args[1]);
            var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
            var arg3 = SharedFunctions.Proprocessing(args[2]);
            var arg4 = SharedFunctions.Proprocessing(args[3]);
            string stringToMaxima = "integrate(" + arg1 + GlobalProfile.ArgumentsSeparatorStandard + arg2 + GlobalProfile.ArgumentsSeparatorStandard + arg3 + GlobalProfile.ArgumentsSeparatorStandard + arg4 + ")";
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }
        /// <summary>
        /// Implements Int(2), second argument (integration variable) is excluded from preprocessing.
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool IntegrateUnknown(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            /*
            var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg2 = TermsConverter.ToString(args[1]);*/
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
            string stringToMaxima = "integrate(" + arg1 + GlobalProfile.ArgumentsSeparatorStandard + arg2 + ")";
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implements Lim(3) with special evaluation of the third argument to distinguish left and right limits
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool Limit(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            /*
            var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
            var arg3 = TermsConverter.ToString(Computation.Preprocessing(args[2], ref context));*/
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);

            string stringToMaxima;
            arg3 = (new Regex(@"(.*)\+0$")).Replace(arg3, "$1" + ", plus");
            arg3 = (new Regex(@"(.*)\-0$")).Replace(arg3, "$1" + ", minus");
            stringToMaxima = "limit(" + arg1 + "," + arg2 + "," + arg3 + ")";

            string answerFromMaxima = ControlObjects.Translator.Ask(stringToMaxima);
 
            // TODO MK 2017 07 28:This seems to handle limit(4) in results but it doesn't
            if ((new Regex(String.Format(@"(limit\([^{0}]+{0}[^{0}]+{0}[^{0}]+{0}[^{0}]+\))", GlobalProfile.ArgumentsSeparatorStandard))).IsMatch(answerFromMaxima))
            {
                result = TermsConverter.ToTerms("expr");
                return true;
            }
            result = TermsConverter.ToTerms(answerFromMaxima);
            return true;
        }

        /// <summary>
        /// Implements Diff(1) 
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool Derivatex(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            //var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            string stringToMaxima = "diff(" + arg1 + ")";
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implements Diff(2)
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool Derivate(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            /*
            var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));*/
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            string stringToMaxima = "diff(" + arg1 + GlobalProfile.ArgumentsSeparatorStandard + arg2 + ")";
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implements Diff(3)
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool DerivateN(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            /*
            var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg2 = TermsConverter.ToString(Computation.Preprocessing(args[1], ref context));
            var arg3 = TermsConverter.ToString(Computation.Preprocessing(args[2], ref context));*/
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            var arg2 = SharedFunctions.Proprocessing(args[1]);
            var arg3 = SharedFunctions.Proprocessing(args[2]);
            string stringToMaxima = "diff(" + arg1 + GlobalProfile.ArgumentsSeparatorStandard + arg2 + GlobalProfile.ArgumentsSeparatorStandard + arg3 + ")";
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }

        /// <summary>
        /// Implements Sum(4), the loop variable is anonymized.
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool Sum(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            // Do preprocessing to get the actual argument values
            for (int i = 0; i < args.GetLength(0); i++)
            {
                args[i] = Computation.Preprocessing(args[i], ref context);
            }
            // replace the name of the loop variable with something strange to avoid misunderstanding as i or e or whatever in
            var loopVar = args[1][0].Text;
            var loopVarMod = ControlObjects.Replacement.LoopVar+loopVar;
            args[1][0].Text = loopVarMod;
            for (int k = 0; k < args[0].Length; k++)
            {
                args[0][k].Text = args[0][k].Text.Replace(loopVar, loopVarMod);
            }
            // convert to string
            string stringToMaxima = "sum("
                + TermsConverter.ToString(args[0]) + ','
                + TermsConverter.ToString(args[1]) + ','
                + TermsConverter.ToString(args[2]) + ','
                + TermsConverter.ToString(args[3]) + ")";
            // ready for standard processing
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            // back-substitution of the loopvar in the result (should't appear there, but symbolic evaluation might fail, then
            // we get the verbatim sum back.
            for (int k = 0; k < result.Length; k++)
            {
                result[k].Text = result[k].Text.Replace(loopVarMod, loopVar);
            }

            return true;
        }

        /// <summary>
        /// Implements Det()
        /// </summary>
        /// <param name="root">unused</param>
        /// <param name="args">arguments</param>
        /// <param name="context">context for preprocessing</param>
        /// <param name="result">result</param>
        /// <returns>true</returns>
        public static bool Det(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
           // var arg1 = TermsConverter.ToString(Computation.Preprocessing(args[0], ref context));
            var arg1 = SharedFunctions.Proprocessing(args[0]);
            string stringToMaxima = "determinant(" + arg1 + ")";
            result = TermsConverter.ToTerms(ControlObjects.Translator.Ask(stringToMaxima));
            return true;
        }
    }
}