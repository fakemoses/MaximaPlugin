using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using SMath.Manager;

namespace MaximaPlugin.Converter
{
    static class ConvertToMaxima
    {
        public static bool convertingmx = true;
        #region KeyValueLists
        static KeyValueList<string, string> euro = new KeyValueList<string, string> 
        {
            { SMath.Manager.Functions.Arccosec, "acsc" },
            { SMath.Manager.Functions.Arcctg, "acot" },
            { SMath.Manager.Functions.Arctg, "atan" },
            { SMath.Manager.Functions.Cosec, "csc" },
            { SMath.Manager.Functions.Ctg, "cot" },
            { SMath.Manager.Functions.Tg, "tan" },
            { SMath.Manager.Functions.Arch, "acosh" },
            { SMath.Manager.Functions.Arcth, "acoth" },
            { SMath.Manager.Functions.Arsh, "asinh" },
            { SMath.Manager.Functions.Arth, "atanh" },
            { SMath.Manager.Functions.Ch, "cosh" },
            { SMath.Manager.Functions.CosecH, "csch" },
            { SMath.Manager.Functions.Cth, "coth" },
            { SMath.Manager.Functions.Sh, "sinh" },
            { SMath.Manager.Functions.Th, "tanh" }
        };
        static KeyValueList<string, string> world = new KeyValueList<string, string> 
        {
            { SMath.Manager.Functions.Arccos, "acos" },
            { SMath.Manager.Functions.Arcctg, "acot" },
            { SMath.Manager.Functions.Arccosec, "acsc" },
            { SMath.Manager.Functions.Arcsec, "asec" },
            { SMath.Manager.Functions.Arcsin, "asin" },
            { SMath.Manager.Functions.Arctg, "atan" },
            { SMath.Manager.Functions.Arch, "acosh" },
            { SMath.Manager.Functions.Arcth, "acoth" },
            { SMath.Manager.Functions.Arsh, "asinh" },
            { SMath.Manager.Functions.Arth, "atanh" }
        };
        // Special characters to ASCII (required if socket uses ASCII encoding)
        static KeyValueList<string, string> CharactersToAscii = new KeyValueList<string, string>
        {
            //lowercase greek (except pi, it is in Constants)
                { @"α", "%alpha" },
                { @"β", "%beta" },
                { @"γ", "%gamma" },
                { @"δ", "%delta" },
                { @"ε", "%epsilon" },
                { @"ζ", "%zeta" },
                { @"η", "%eta" },
                { @"θ", "%theta" },
                { @"ι", "%iota" },
                { @"κ", "%kappa" },
                { @"λ", "%lambda" },
                { @"μ", "%mu" },
                { @"ν", "%nu" },
                { @"ξ", "%xi" },
                { @"ο", "%omicron" },
                { @"ρ", "%rho" },
                { @"σ", "%sigma" },
                { @"τ", "%tau" },
                { @"υ", "%upsilon" },
                { @"φ", "%varphi" },
                { @"ϑ", "%vartheta" },
                { @"χ", "%chi" },
                { @"ψ", "%psi" },
                { @"ω", "%omega" },
           //uppercase greek
                { @"Α", "%Alpha" },
                { @"Β", "%Beta" },
                { @"Γ", "%Gamma" },
                { @"Δ", "%Delta" },
                { @"Ε", "%Epsilon" },
                { @"Ζ", "%Zeta" },
                { @"Η", "%Eta" },
                { @"Θ", "%Theta" },
                { @"Ι", "%Iota" },
                { @"Κ", "%Kappa" },
                { @"Λ", "%Lambda" },
                { @"Μ", "%Mu" },
                { @"Ν", "%Nu" },
                { @"Ξ", "%Xi" },
                { @"Ο", "%Omicron" },
                { @"Π", "%Pi" },
                { @"Ρ", "%Rho" },
                { @"Σ", "%Sigma" },
                { @"Τ", "%Tau" },
                { @"Υ", "%Upsilon" },
                { @"Φ", "%Phi" },
                { @"Χ", "%Chi" },
                { @"Ψ", "%Psi" },
                { @"Ω", "%Omega" },
            // other characters
                { @"ä", "%ae" },
                { @"ö", "%oe" },
                { @"ü", "%ue" },
                { @"ß", "%ss" },
                { @"Ä", "%Ae" },
                { @"Ö", "%Oe" },
                { @"Ü", "%Ue" },
                { @"°", "%DegreeChar" },

        };
        //
        //static KeyValueList<string, string> letters = new KeyValueList<string, string> 
        //{
        //        { @"ä", "%ae" },
        //        { @"ö", "%oe" },
        //        { @"ü", "%ue" },
        //        { @"ß", "%ss" },
        //        { @"Ä", "%Ae" },
        //        { @"Ö", "%Oe" },
        //        { @"Ü", "%Ue" },
        //};
        static KeyValueList<string, string> constantsToMaxima = new KeyValueList<string, string>
        {
            { @"(?<=\W)(e)(?=\W)", "%e" },       // euler
            { @"(?<=\W)(i)(?=\W)", "%i" },       // complex
            { @"(?<=\W)(#)(?=\W)", "" },         // empty placeholder
            { @"(\w)#", "$1" + ControlObjects.Replacement.Sharp}, // variable name with  #
            { @"#(\w)", ControlObjects.Replacement.Sharp+"$1"},  // variable name with #
            { @"π", "%pi" },                     // pi 
            //{ @"([0-9a-zA-Z]*[a-zA-Z][0-9a-zA-Z]*)([.])", "$1_%_" },                                            // Textindex 
            { @"(\p{L}\w*)(\.)", "$1_%_" },  // Textindex 
            // { @"([0-9a-zA-Z]*[a-zA-Z][0-9a-zA-Z]*)(["+GlobalParams.CurrentDecimalSymbol + @"])", "$1_%_" },     // Textindex 
        };
        static KeyValueList<string, string> symbolsToMaxima = new KeyValueList<string, string> 
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
        static KeyValueList<string, string> seperatorsToMaxima = new KeyValueList<string, string> 
        {
                { @"'diff\(", "diff("},
                { @"'([^\+\-*/])", "%unit$1" },                                                                     // units
                { @"{", "(" },                                                                                      // brackets
                { @"}", ")" },                                                                                      // brackets

        };
        // MK 2018 09 03 \b marks the start of a word
        static KeyValueList<string, string> functionNamesToMaxima = new KeyValueList<string, string> 
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
        public static string PrepareStringsForMaxima(string text)
        {        
       //     text = ConvertTrigonometricFunctionsFromSMathToMaxima(text);
			text = TermsConverter.DecodeText(text);
            text = text.Replace("\"$", "");
            text = text.Replace("$\"", "");
            //foreach (var pair in CharactersToAscii) text = (new Regex(pair.Key).Replace(text, pair.Value));
            //foreach (var pair in letters) text = (new Regex(pair.Key).Replace(text, pair.Value));
            //foreach (var pair in symbolsToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            text = text.Replace("\\", "\\\\"); // convert backslash
            return text;
        }
        public static string putCharfun(string input)
        {
            Term[] termsIn = TermsConverter.ToTerms(input);
            List<Term> mylist = new List<Term>();
            string code = String.Empty;
            for (int i = 0; i < termsIn.Length; i++)
            {
                mylist.Add(termsIn[i]);
                if (termsIn[i].Text == Operators.BooleanAnd ||
                    //termsIn[i].Text == Operators.BooleanEqual || 
                    termsIn[i].Text == Operators.BooleanLess ||
                    termsIn[i].Text == Operators.BooleanLessOrEqual ||
                    termsIn[i].Text == Operators.BooleanMore ||
                    termsIn[i].Text == Operators.BooleanMoreOrEqual ||
                    termsIn[i].Text == Operators.BooleanNot ||
                    termsIn[i].Text == Operators.BooleanNotEqual ||
                    termsIn[i].Text == Operators.BooleanOr ||
                    termsIn[i].Text == Operators.BooleanXor)
                {
                    mylist.Add(new Term("charfun", TermType.Function, 1));
                }
            }
            Term[] termsOut = mylist.ToArray();
            return TermsConverter.ToString(termsOut);
        }

        public static string PrepareTermsForMaxima(string text)
        {
            //Regex rxassume = new Regex(@"assume");
            //if(!rxassume.Match(text,0).Success)
                // text = putCharfun(text);
            text = text.Replace(Symbols.StringChar, "");

            text = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.MatrixConvert(text);
            text = MaximaPlugin.Converter.MatrixAndListFromSMathToMaxima.ListConvert(text);

            text = DiffConverterToMaxima.DataCollection(text);

            //foreach (var pair in CharactersToAscii) text = (new Regex(pair.Key).Replace(text, pair.Value));
            //foreach (var pair in letters) text = (new Regex(pair.Key).Replace(text, pair.Value));
            foreach (var pair in constantsToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            foreach (var pair in seperatorsToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            foreach (var pair in symbolsToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            foreach (var pair in functionNamesToMaxima) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            // custom conversions from config file
            foreach (ExpressionStore exp in ControlObjects.Translator.GetMaxima().exprSMathToMaxima)
                text = (new Regex(exp.regex)).Replace(text, exp.replace);
            // convert tagged items to noun form 
            text = text.Replace(ControlObjects.Replacement.Noun, "'");
            // if (ControlObjects.TranslationModifiers.IsDiffAsNoun) text = text.Replace("diff(", "'diff(");

            return text;
        }
    }   
    public static class DiffConverterToMaxima
    {
        public static List<string> diffVarNumDCTM = new List<string>();
        public static int startDiff = 0, endDiff = 0;
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
