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
        //public static AutoSocket anySocket;
        //public static bool anySocketConnected=false;

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
            while (i < root.ArgsCount)
            {
                //tempString = TermsConverter.ToString(Computation.Preprocessing(args[i], ref context));
                tempString = SharedFunctions.Proprocessing(args[i]);

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
        // This is never called
        //public static bool SocketInit(Term root, Term[][] args, ref Store context, ref Term[] result, MaximaSession maxima)
        //{
        //    anySocket = new AutoSocket();
        //    result = TermsConverter.ToTerms("\"SocketClientServer has started on port: "+ Convert.ToString(anySocket.GetAutoSocketPort()) + "\"");
        //    return true;
        //}
        //public static bool Socket(Term root, Term[][] args, ref Store context, ref Term[] result,MaximaSession maxima)
        //{
        //    string arg1 = TermsConverter.ToString(args[0]);
        //    //  anySocket.ConnectClient();
        //    result = TermsConverter.ToTerms(SMath.Manager.Symbols.StringChar + maxima.SendAndReceiveFromSocket(arg1) + SMath.Manager.Symbols.StringChar);
        //    return true;
        //}
    }
}
