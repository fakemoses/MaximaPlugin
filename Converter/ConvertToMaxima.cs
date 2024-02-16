using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using SMath.Manager;

namespace MaximaPlugin.Converter
{
    static class ConvertToMaxima
    {
        public static bool convertingmx = true;
        #region Dictionarys

        static Dictionary<string, string> constantsToMaxima = new Dictionary<string, string>
        {
            { @"(?<=\W)(e)(?=\W)", "%e" },       // euler
            { @"(?<=\W)(i)(?=\W)", "%i" },       // complex
            { @"(?<=\W)(#)(?=\W)", "" },         // empty placeholder
            { @"(\w)#", "$1" + ControlObjects.Replacement.Sharp}, // variable name with  #
            { @"#(\w)", ControlObjects.Replacement.Sharp+"$1"},  // variable name with #
            { @"π", "%pi" },                     // pi 
            { @"(\p{L}\w*)(\.)", "$1_%_" },  // Textindex 
        };
        static Dictionary<string, string> symbolsToMaxima = new Dictionary<string, string> 
        {
                { @"(^|\W)(∞)($|\W)","$1inf$3" },                                                                   // infinity
                { @"(^|\W)(\-∞)($|\W)","$1minf$3"  },                                                               // negativ infinity
                { @"±([^\+\-*/])+($|[\+\-*/\)\(]+)", "plusminus($1)$2" },                                           // plusminus
                { @"≡", "=" },                                                                                      // ≡ becomes =
                { @"≤", " <= " },  
                { @"≥", " >= " },
                { @"≠", " # " },
                { @"¬", " not " },
                { @"&", " and " },
                { @"\|", " or " },
                { @"¤", " not or " },
                { @"†", " ~ " },                                                                                      // crossproduct
        };
        static Dictionary<string, string> seperatorsToMaxima = new Dictionary<string, string> 
        {
                { @"'diff\(", "diff("},
                { @"'([^\+\-*/])", "%unit$1" },                                                                     // units
                { @"{", "(" },                                                                                      // brackets
                { @"}", ")" },                                                                                      // brackets

        };

        static Dictionary<string, string> primeToRandom = new Dictionary<string, string>
        {
            { @"([a-zA-Z]+)\s*'", "$1PRIME" },
        };

        // MK 2018 09 03 \b marks the start of a word
        static Dictionary<string, string> functionNamesToMaxima = new Dictionary<string, string> 
        {
                { @"\bint\(", "integrate(" },            
                { @"\blog\(", "logb(" },
                { @"\bln\(", "log(" },
                { @"\blim\(", "limit(" },
                { @"\bsign\(", "signum(" },
                { @"\bdet\(", "determinant(" },
                { @"\bRe\(", "realpart(" },
                { @"\bIm\(", "imagpart(" },
                { @"\bnthroot\(", "ntewurzel(" }, // helper function for representation of the nth root (nthroot is something different in Maxima)
                { @"\bround\(", "round2(" }, // helper function for representation of round(2) in Maxima
        };
        #endregion


        /// <summary>
        /// Replaces string constants to Maxima
        /// </summary>
        /// <param name="text">string to translate</param>
        /// 
        /// <returns>Translated string</returns>
        public static string PrepareStringsForMaxima(string text)
        {        
			text = TermsConverter.DecodeText(text);
            text = text.Replace("\"$", "");
            text = text.Replace("$\"", "");
            text = text.Replace("\\", "\\\\"); // convert backslash
            return text;
        }

        /// <summary>
        /// Translate Smath string to Maxima string
        /// </summary>
        /// <param name="text">Smath expression</param>
        /// <returns>SMath expression</returns>
        public static string PrepareTermsForMaxima(string text)
        {
            text = text.Replace(Symbols.StringChar, "");

            text = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.MatrixConvert(text);
            text = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.ListConvert(text);

            text = DiffConverterToMaxima.DataCollection(text);

            foreach (var pair in primeToRandom) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            foreach (var pair in constantsToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            foreach (var pair in seperatorsToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            foreach (var pair in symbolsToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            foreach (var pair in functionNamesToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            // custom conversions from config file
            foreach (ExpressionStore exp in ControlObjects.Translator.GetMaxima().exprSMathToMaxima)
                text = (new Regex(exp.regex)).Replace(text, exp.replace);
            // convert tagged items to noun form 
            text = text.Replace(ControlObjects.Replacement.Noun, "'");

            return text;
        }
    }

    /// <summary>
    /// Conversion of derivatives to Maxima.
    /// </summary>
    public static class DiffConverterToMaxima
    {
        public static List<string> diffVarNumDCTM = new List<string>();
        public static int startDiff = 0, endDiff = 0;

        /// <summary>
        /// Generate the SMath expression string from the term structure
        /// </summary>
        /// <param name="diffVarNumSTM">Term structure</param>
        /// <returns>SMath expression </returns>
        public static string MakeTermString(List<string> diffVarNumSTM)
        {
            string tempstring = "";
            for (int k = 1; k < diffVarNumSTM.Count; k++)
            {
                tempstring = tempstring + "diff(" + diffVarNumSTM[0] + GlobalProfile.ArgumentsSeparatorStandard + diffVarNumSTM[k] + GlobalProfile.ArgumentsSeparatorStandard + "1)";
                if (k != diffVarNumSTM.Count - 1) tempstring = tempstring + "+";
            }
            return tempstring;
        }

        /// <summary>
        /// Search for diff(... structures in expressions
        /// </summary>
        /// <param name="text">Maxima string</param>
        /// <returns>term structure</returns>
        public static string DataCollection(string text)
        {
            int bracketOpen = 0, bracketClose = 0;
            int charCountStart = 0, charCounter = 0;
            bool flipStringChar = false;
            int i = -1;
            while (i < text.Length)
            {
                i++;
                if (i < (text.Length - 4) &&
                    text[i + 0] == 'd' &&
                    text[i + 1] == 'i' &&
                    text[i + 2] == 'f' &&
                    text[i + 3] == 'f' &&
                    text[i + 4] == '(')
                {
                    startDiff = i;
                    i += 5;
                    bracketOpen++;
                    while (i < text.Length && bracketOpen - bracketClose > 0)
                    {
                        charCountStart = i;
                        while (i < text.Length && !(bracketOpen - bracketClose == 1 && text[i] == ')') && (text[i] != GlobalProfile.ArgumentsSeparatorStandard || bracketOpen - bracketClose > 1 || flipStringChar))
                        {
                            if (text[i] == '"' && flipStringChar)
                                flipStringChar = false;
                            else if (text[i] == '"')
                                flipStringChar = true;
                            else if (text[i] == '(')
                                bracketOpen++;
                            else if (text[i] == ')')
                                bracketClose++;
                            charCounter++;
                            i++;
                        }
                        diffVarNumDCTM.Add(text.Substring(charCountStart, charCounter));
                        charCounter = 0;

                        if (i < text.Length && text[i] == '(')
                            bracketOpen++;
                        else if (i < text.Length && text[i] == ')')
                            bracketClose++;
                        i++;
                    }
                    endDiff = i;
                    if (diffVarNumDCTM.Count == 1)
                    {
                        string temp = MakeDiffOne(diffVarNumDCTM[0]);
                        text = MatrixAndListFromSMathToMaxima.MakeString(text, temp, startDiff, endDiff);
                        i = startDiff + temp.Length;
                    }
                    bracketOpen = 0; bracketClose = 0; charCountStart = 0; charCounter = 0;
                    diffVarNumDCTM.Clear(); startDiff = 0; endDiff = 0;
                }
            }
            return text;
        }

        /// <summary>
        /// Helper function in DataCollection
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MakeDiffOne(string text)
        {
            int charCountStart = 0, charCounter = 0;
            int i = -1;
            List<string> vars = new List<string>();
            bool foundLetter = false, foundFunction = false, alreadyStored = false; ;
            vars.Add(text);
            while (i < text.Length)
            {
                i++;
                if (i < (text.Length) && (Char.IsLetter(text[i]) || Char.IsDigit(text[i])))
                {
                    charCountStart = i;
                    while (i < text.Length && (Char.IsLetter(text[i]) || Char.IsDigit(text[i]) || text[i] == '('))
                    {

                        if (text[i] == '(')
                        {
                            foundFunction = true;
                            break;
                        }
                        if (Char.IsLetter(text[i]))
                            foundLetter = true;

                        charCounter++;
                        i++;
                    }
                    if (charCounter > 0)
                    {
                        string tmp = text.Substring(charCountStart, charCounter);

                        for (int l = 1; l < vars.Count; l++)
                                if (vars[l] == tmp) alreadyStored = true;

                        if (foundLetter && !foundFunction && !alreadyStored) vars.Add(tmp);
                        foundFunction = false;
                        foundLetter = false;
                        alreadyStored = false;
                        charCounter = 0;
                    }
                }
            }
            return MakeTermString(vars);
        }
    }

}
