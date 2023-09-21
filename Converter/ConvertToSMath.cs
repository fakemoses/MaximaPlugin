using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using SMath.Manager;

namespace MaximaPlugin.Converter
{
    /// <summary>
    /// Data and Functions for conversion from Maxima to SMath
    /// </summary>
    static class ConvertToSMath
    {
        public static bool skipConverting = false;
        #region KeyValueLists
        // TODO MK 2017 07 24: Use a common set of lists for ConvertToSMath and ConvertToMaxima

        // Function names which must be translated. 
        // TODO MK 2017 07 24: Check if the normalized names still need translation
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
                { @"π", "%pi" },
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
        //// Special characters for german language
        //static KeyValueList<string, string> letters = new KeyValueList<string, string> 
        //{
        //        { @"ä", "%ae" },
        //        { @"ö", "%oe" },
        //        { @"ü", "%ue" },
        //        { @"ß", "%ss" },
        //        { @"Ä", "%Ae" },
        //        { @"Ö", "%Oe" },
        //        { @"Ü", "%Ue" },
        //        { @"%DegreeChar", "°" },
        //};
        //// Euler e and imaginary unit i, pi (constants with special names in Maxima
        static KeyValueList<string, string> constantsToSMath = new KeyValueList<string, string> 
        {
            { @"(^|\W)(%e)($|\W)", "$1e$3" },     // euler
            { @"(^|\W)(%i)($|\W)", "$1i$3" },     // complex
            { @"%pi", "π"},                       // pi 
            { ControlObjects.Replacement.Sharp + @"(\w)", "#$1" },   // names starting with #
            { @"(\w)" + ControlObjects.Replacement.Sharp, "$1#" },   // names ending with #
            { @"(?<=\W)(" + ControlObjects.Replacement.Sharp + @")(?=\W)", "#"}, // replacementstring as name
            { @"([,\(])(?=[,\)])", "$1#"}, // missing operands


        };

        // further special symbols
        static KeyValueList<string, string> symbolsToSMath = new KeyValueList<string, string> 
        {
                { @"(^|\W)(inf)($|\W)","$1∞$3" },               // positiv infinity
                { @"(^|\W)(minf)($|\W)","$1-∞$3"  },            // negativ infinity
                { @"(^|\W)(infinity)($|\W)","$1∞$3"  },                                                             // complex infinity
              //  { @"(.*)(plusminus\()(\d*)(\))(.*)", "$1±$3$5" },                                                   // plusminus     
               // { String.Format(@"(\d+{0}\d+)[bE](\+\d+|\-\d+|\d+)",'.'), "$1*10^$2" },// floats
                { @"(\d+\.\d+)[bBeE](\+\d+|\-\d+|\d+)", "$1*10^$2" },     // floats
        };
        // logical operators.
        // TODO MK 2017 07 24: Check what is with = (equal)
        static KeyValueList<string, string> logicToSMath = new KeyValueList<string, string> 
        {
                { @"(\s)<=(\s)", "$1≤$2" },  
                { @"(\s)>=(\s)", "$1≥$2" },
                { @"(\s)#(\s)", "$1≠$2" },
                { @"(\s)not(\s)", "$1¬$2" },
                { @"(\s)and(\s)", "$1&$2" },
                { @"(\s)or(\s)", "$1|$2" },
               // { @"not or", "¤" },
        };

        // Other conversions
        static KeyValueList<string, string> seperatorsToSMath = new KeyValueList<string, string> 
        {
            // What is this? Replace ?% by nothing?
            { @"\?\%", "" },
            // Suppression of leading '?        // request is equal to result
            { @"'([^\+\-*/])", "$1" },
            // units 
            // TODO: MK 2017 07 25: Better do that in term representation?
            { @"%unit([^\+\-*/])", "'$1" },  
            // Remove % signs (markup for constants in Maxima)
            { @"%([^\+\-*/])", "$1" },
            // Array indices 
            { String.Format(@"(^|[\+\-*/\(\)])([^()\[\]{0}]+)\[([^()\[\]{0}]+)\{0}([^()\[\]{0}]+)\]($|[\+\-*/\(\)])",GlobalProfile.ArgumentsSeparatorStandard), String.Format("$1el($2{0}$3{0}$4)$5",GlobalProfile.ArgumentsSeparatorStandard) }, 
            { String.Format(@"(^|[\+\-*/\(\)])([^()\[\]{0}]+)\[([^()\[\]{0}]+)\]($|[\+\-*/\(\)])",GlobalProfile.ArgumentsSeparatorStandard), String.Format("$1el($2{0}$3)$4",GlobalProfile.ArgumentsSeparatorStandard) },
            // = becomes ≡
            { @"(=)", "≡" },
            // Cross product                                                                                    
            { @"~", "†" },        
        };

        // TODO MK 2017 07 26: Check if that can be joined with the other functions lists.
        // MK 2018 09 03 \b marks the start of a word
        static KeyValueList<string, string> functionNamesToSMath = new KeyValueList<string, string> 
        {
                { @"\bintegrate\(", "int(" },
                { @"\blog\(", "ln(" },
                { @"\blimit\(", "lim(" },
                { @"\bsignum\(", "sign(" },
                { @"\bdeterminant\(", "det(" },
                { @"\brealpart\(", "Re(" },
                { @"\bimagpart\(", "Im(" },
          //      { String.Format(@"diff\(([^{0}]+|(\w+)\(([^{0}][{0}]*)+\))[{0}]([^{0}]+)[{0}]([^{0}]+)[{0}]([^{0}]+)[{0}]([^{0}]+)\)",GlobalProfile.ArgumentsSeparatorStandard), String.Format("diff(diff($1{0}$4{0}$5){0}$6{0}$7)",GlobalProfile.ArgumentsSeparatorStandard) }, // array indices   },
                { @"\?at\(", "at(" },
                { @"\bntewurzel\(", "nthroot(" },
                { @"\bround\((.+)\)", "round($1,0)" },

        };
        #endregion

        /// <summary>
        /// Replaces string constants to SMath
        /// </summary>
        /// <param name="text">string to translate</param>
        /// 
        /// <returns>Translated string</returns>
        public static string PrepareStringsForSMath(string text)
        {
            // Remove "$ and $" 
            text = text.Replace("\"$", "").Replace("$\"", "");
            // Translate greek and special symbols
            //foreach (var pair in CharactersToAscii) text = (new Regex(pair.Value).Replace(text, pair.Key));
            //foreach (var pair in symbolsToSMath) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            //foreach (var pair in letters) text = (new Regex(pair.Value)).Replace(text, pair.Key);
            text = text.Replace("\\\\", "\\"); // convert backslash
            return text;
        }

        /// <summary>
        /// Generates an SMath expression string from a PowerDataContainer object
        /// </summary>
        /// <param name="container">PowerDataContainer object</param>
        /// <returns>SMath expression string</returns>
        public static string ConvertPowerStringBuilder(PowerDataContainer container)
        {
            string tmpString;
            if (container.firstElement)
                tmpString = "";
            else
                tmpString = "^{";

            for (int j = 0; j < container.itemData.Count; j++)
            {
                if (container.itemData[j] != container.layerMsg)
                {
                    tmpString = tmpString + container.itemData[j];
                }
                else
                {
                    tmpString = tmpString + ConvertPowerStringBuilder(container.getNextPowerDataContainer());
                }
                if (j == container.itemData.Count - 1 && !container.firstElement)
                    tmpString = tmpString + "}";
            }
            return tmpString;
        }

        /// <summary>
        /// Adds appropriate curly braces in expressions with powers
        /// </summary>
        /// <param name="text">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertPower(string text)
        {
            int charCounter = 0;
            int layer = 0;
            //if(start!=0) bracketOpen =1;
            PowerDataContainer container = new PowerDataContainer(null);
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == '^')
                {
                    if (charCounter > 0)
                    {
                        container.addData(text.Substring(i - charCounter, charCounter));
                        charCounter = 0;
                    }
                    layer++;
                    container = container.getNewPowerDataContainer(container);
                }
//                else if (!(Char.IsLetter(text[i]) || Char.IsDigit(text[i]) || text[i] == GlobalParams.CurrentDecimalSymbol || text[i] == '(') && container.charcounter > 0)
                else if (!(Char.IsLetter(text[i]) || Char.IsDigit(text[i]) || text[i] == '.' || text[i] == '(') && container.charcounter > 0)
                {
                    if (container.bracketOpen - container.bracketClose == 0 && text[i] == ')')
                    {
                        container.addData(text.Substring(i - charCounter, charCounter));
                        charCounter = 0;
                        if (!container.firstElement)
                        {
                            container = container.getPrevPowerDataContainer();
                            layer--;
                            i--;
                        }
                    }
                    else if (container.bracketOpen - container.bracketClose == 1 && text[i] == ')')
                    {
                        container.bracketClose++;
                        charCounter++;
                        container.addData(text.Substring((i + 1) - charCounter, charCounter));
                        charCounter = 0;
                        if (!container.firstElement)
                        {
                            container = container.getPrevPowerDataContainer();
                            layer--;
                        }
                    }
                    else if (container.bracketOpen - container.bracketClose == 0)
                    {
                        container.addData(text.Substring(i - charCounter, charCounter));
                        charCounter = 1;
                        if (!container.firstElement)
                        {
                            container = container.getPrevPowerDataContainer();
                            layer--;
                        }
                    }
                    else
                    {
                        if (text[i] == ')') { container.bracketClose++; }
                        charCounter++;
                        container.charcounter++;
                    }
                }
                else
                {
                    charCounter++;
                    container.charcounter++;
                    if (text[i] == ')') { container.bracketClose++; }
                    else if (text[i] == '(') { container.bracketOpen++; }
                }
                if (charCounter > 0 && i == text.Length - 1)
                {
                    container.addData(text.Substring((i + 1) - charCounter, charCounter));
                    charCounter = 0;
                }
                i++;
            }
            while (container.getPrevPowerDataContainer() != null)
                container = container.getPrevPowerDataContainer();
            return ConvertPowerStringBuilder(container);
        }


        /// <summary>
        /// Translate Maxima string to SMath string
        /// </summary>
        /// <param name="text">Maxima expression</param>
        /// <returns>SMath expression</returns>
        public static string PrepareTermsForSMath(string text)
        {
            // Remove input and output numbers from Maxima response
            Regex rxMxOut = new Regex(@"(\(.[soi][0-9A-F]+\))");
            text = rxMxOut.Replace(text, "");

            // replace masked i
            //TODO MK 2017 07 26: can this be done in term form?         
            text = text.Replace("VXAXRi","i");

            // translate logical operators
            foreach (var pair in logicToSMath) text = (new Regex(pair.Key)).Replace(text, pair.Value);

            // remove spaces
            text = text.Replace(" ", "");

            // replace empty list by a string (SMath can't display empty strings)
            Regex rxEmtyList = new Regex(@"\[\]");
            text = rxEmtyList.Replace(text, "\"no result\"");

            // set the matrix converter flag
            if (ControlObjects.TranslationModifiers.IsExpectedEquation)
                MaximaPlugin.Converter.MatrixAndListFromMaximaToSMath.solveFunction = true;

            // convert matrix objects (lists of lists)
            text = MaximaPlugin.Converter.MatrixAndListFromMaximaToSMath.MatrixConvert(text);

            // convert nested lists if [[[ is found somewhere in the string
            Regex rxMulti = new Regex(@"\[\[\[");
            if (rxMulti.IsMatch(text,0))
                text = MaximaPlugin.Converter.MatrixAndListFromMaximaToSMath.MultiListConvert(text);

            //replace any variable with subscript a[1] a[2] with random text 
            text= Regex.Replace(text, @"([a-zA-Z]+)\[(\d+)\]", "$1INDEX$2");

            // convert lists
            text = MaximaPlugin.Converter.MatrixAndListFromMaximaToSMath.ListConvert(text);

            //put back the indexes
            text = Regex.Replace(text, @"([a-zA-Z]+)INDEX(\d+)", "$1[$2]");

            //convert into smath -> a[1] => el(a,1)
            text = Regex.Replace(text, @"(\w+)\[(\d+)\]", "el($1,$2)");

            // reset the matrix converter flag
            MaximaPlugin.Converter.MatrixAndListFromMaximaToSMath.solveFunction = false;

            // translate derivatives
            text=DiffConverterToSMath.DataCollection(text);

            // translate greek characters and e and i
            //foreach (var pair in CharactersToAscii) text = (new Regex(pair.Value).Replace(text, pair.Key));
            foreach (var pair in constantsToSMath) text = (new Regex(pair.Key)).Replace(text, pair.Value);

            // translate subscipt tag
            text = text.Replace("_%_", "."); 

            // do more translations
            foreach (var pair in seperatorsToSMath) text = (new Regex(pair.Key)).Replace(text, pair.Value);
            
            // translate more symbols
            foreach (var pair in symbolsToSMath) text = (new Regex(pair.Key)).Replace(text, pair.Value);

            // translate non-trig function names
            foreach (var pair in functionNamesToSMath) text = (new Regex(pair.Key)).Replace(text, pair.Value);

            // convert powers 
            text = ConvertPower(text);

            // Apply custom translations stored in maxima.xml
            string eRx, eRe;
            foreach (ExpressionStore exp in ControlObjects.Translator.GetMaxima().exprMaximaToSMath)
            {
                eRx = exp.regex; 
                eRe = exp.replace;
                // protect eventual dots in the regular expressions.
                if (GlobalProfile.ArgumentsSeparatorStandard == '.') { eRx = eRx.Replace("{0}", "\\{0}"); }
                if (GlobalProfile.DecimalSymbolStandard == '.') { eRe = eRe.Replace("{1}", "\\{1}"); }
               
                text = (new Regex(String.Format(eRx, GlobalProfile.ArgumentsSeparatorStandard, GlobalProfile.DecimalSymbolStandard))).Replace(text, (String.Format(eRe, GlobalProfile.ArgumentsSeparatorStandard, GlobalProfile.DecimalSymbolStandard)));
            }

            text = text.Replace("PRIME", "'");

            return text;
        }
    }

    /// <summary>
    /// Conversion of power expressions (care for different operator priority in maxima and smath)
    /// </summary>
    public class PowerDataContainer
    {
        public bool firstElement = false;
        int pointerToRefs = 0;
        public int charcounter = 0;
        public int bracketOpen = 0;
        public int bracketClose = 0;
        //Msg to layerUp 
        public string layerMsg = "!+!+!+LAYER+!+!+!";
        //Data
        public List<string> itemData = new List<string>();
        List<PowerDataContainer> refs = new List<PowerDataContainer>();

        /// <summary>
        /// Append an item tom the refs list
        /// </summary>
        /// <param name="prev">item to append</param>
        public PowerDataContainer(PowerDataContainer prev) { refs.Add(prev); if (prev == null) firstElement = true; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldContainer"></param>
        /// <returns></returns>
        public PowerDataContainer getNewPowerDataContainer(PowerDataContainer oldContainer)
        {
            itemData.Add(layerMsg); // add string to itemData list
            PowerDataContainer newC = new PowerDataContainer(oldContainer); 
            refs.Add(newC);
            return newC;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PowerDataContainer getNextPowerDataContainer()
        {
            pointerToRefs++;
            return refs[pointerToRefs];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PowerDataContainer getPrevPowerDataContainer()
        {
            return refs[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void addData(string text)
        {
            itemData.Add(text);
        }
    }

    /// <summary>
    /// Conversion of derivatives to SMath. The task is to convert diff(f,x,n,y,m) to diff(diff(f,x,n),y,m)
    /// </summary>
    public static class DiffConverterToSMath
    {
        public static List<string> diffVarNumDCTS = new List<string>(); // storage list for arguments of diff()
        public static int startDiff = 0, endDiff = 0;


        /// <summary>
        /// Generate the SMath expression string from the term structure
        /// </summary>
        /// <param name="diffVarNumMTS">Term structure</param>
        /// <returns>SMath expression </returns>
        public static string MakeTermString(List<string> diffVarNumMTS)
        {
            string tempstring = "";
 
            for (int k = 0; k < ((diffVarNumMTS.Count - 1) / 2); k++)
            {
                tempstring = tempstring + "diff(";
            }
            tempstring = tempstring + diffVarNumMTS[0];
            for (int k = diffVarNumMTS.Count - 1; k > 0; k--)
            {
                if (k % 2 == 0)
                    tempstring =  tempstring + GlobalProfile.ArgumentsSeparatorStandard + diffVarNumMTS[k - 1];
                else
                {
                    if (Convert.ToInt32(diffVarNumMTS[k + 1]) > 1)
                        tempstring = tempstring + GlobalProfile.ArgumentsSeparatorStandard + diffVarNumMTS[k + 1] + ")";
                    else
                        tempstring = tempstring + ")";
                }
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

            // scanning loop
            int i = -1;
            while (i < text.Length)
            {
                i++;

                // check for diff( beginning at i
                if (i < (text.Length - 4) &&
                    text[i + 0] == 'd' &&
                    text[i + 1] == 'i' &&
                    text[i + 2] == 'f' &&
                    text[i + 3] == 'f' &&
                    text[i + 4] == '(')
                {
                    //  diff( was found
                    startDiff = i; // mark start of diff command 
                    i += 5; // advance scanning pointer
                    bracketOpen++; // increase level counter
                    
                    // scan the remainder of the currently found diff() command
                    while (i < text.Length && bracketOpen - bracketClose > 0)
                    {
                        charCountStart = i;
                        // as long as the last bracket isn't closed by the text and either the following text isn't an arg sep or more than one bracket level is open or flipstringchar is true
                        while (i < text.Length && !(bracketOpen - bracketClose == 1 && text[i] == ')') && (text[i] != GlobalProfile.ArgumentsSeparatorStandard || bracketOpen - bracketClose > 1 || flipStringChar))
                        {
                            if (text[i] == '"' && flipStringChar) // string marker
                                flipStringChar = false;
                            else if (text[i] == '"')  // string marker
                                flipStringChar = true;
                            else if (text[i] == '(') // register opening bracket
                                bracketOpen++;
                            else if (text[i] == ')')  // register closing bracket
                                bracketClose++; 
                            charCounter++; 
                            i++;
                        }

                        // Append found contents to list
                        diffVarNumDCTS.Add(text.Substring(charCountStart, charCounter));
                        charCounter = 0;


                        if (i < text.Length && text[i] == '(') // register opening bracket
                            bracketOpen++;
                        else if (i < text.Length && text[i] == ')') // register closing bracket
                            bracketClose++;

                        i++;

                    }
                    endDiff = i;

                    // Convert diff() with more than 2 arguments
                    if (diffVarNumDCTS.Count > 2)
                    {
                        // build translated expression based on arguments
                        string temp = MakeTermString(diffVarNumDCTS);
                        // insert translated string instead of original expression
                        // TODO MK 2017 07 27: Rename function to ReplaceAt and put it to some logical location
                        text = MatrixAndListFromSMathToMaxima.MakeString(text, temp, startDiff, endDiff);
                        i = startDiff + temp.Length;
                    }
                    bracketOpen = 0; bracketClose = 0; charCountStart = 0; charCounter = 0;
                    diffVarNumDCTS.Clear(); startDiff = 0; endDiff = 0;
                }
            }
            return text;
        }
    }

}
