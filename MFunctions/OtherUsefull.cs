using System;

using SMath.Manager;
using SMath.Math;

namespace MaximaPlugin.MFunctions
{
    /// <summary>
    /// Functions which do not use Maxima.
    /// </summary>
    class OtherUsefull
    {

        /// <summary>
        /// Implements Assign(). 
        /// TODO: Remove useless strings (Maxima messages) from the output, might be a good point to get used 
        /// to Entry class.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Assign(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            string text = "";
            string tempString = "";
            int i = 0;
            string term = root.Text;
            while (i < root.ArgsCount)
            {
                //tempString = TermsConverter.ToString(Computation.Preprocessing(args[i], ref context));
                tempString = SharedFunctions.Proprocessing(args[i]);

                // Solve has to be precomputed if exists?

                if (i == 0)
                    text = tempString;
                else
                    text = text + "," + tempString;
                i++;
            }
            text = text.Replace("≡", ":");
            result = TermsConverter.ToTerms(text);
            return true;
        }
    }
}
