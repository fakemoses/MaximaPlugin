﻿using System.Windows.Forms;
using System.Threading;
using System;

namespace MaximaPlugin.MForms
{
    class FormControl
    {
        public static bool ThreadDebuggerProState = false;
        public static bool ThreadLogProState = false;
        public static bool SettingsFormState = false;
        public static bool MaximaFormState = false;
        public static bool InstallerFormState = false;


        public static string formDataRe = "String from SMath";
        public static string formDataRePre = "String from SMath";
        public static string formDataAn = "\"Debugger was used\"";
        public static bool formReadyState = false;
        public static bool formWaitFor = true;
        public static bool formSMathCalc = false;
        public static bool formOpenState = false;



        /// <summary>
        /// Controller to open Debug and log form
        /// </summary>
        /// <param name="formName"></param>
        public static void OpenForm(string formName) 
        {

            ControlObjects.Translator.GetMaxima().StartSession();
            if (!ThreadDebuggerProState && formName == "ThreadDebuggerPro" && ControlObjects.Translator.GetMaxima().GetState() == ControlObjects.Translator.GetMaxima().GetMaximaStateRunning())
            {

                Thread trd = new Thread(new ThreadStart(MForms.FormControl.ThreadDebuggerPro));
                trd.IsBackground = true;
                trd.Start();
            }
            else if (!ThreadLogProState && formName == "ThreadLogPro" && ControlObjects.Translator.GetMaxima().GetState() == ControlObjects.Translator.GetMaxima().GetMaximaStateRunning())
            {

                Thread trd = new Thread(new ThreadStart(MForms.FormControl.ThreadLogPro));
                trd.IsBackground = true;
                trd.Start();
            }
        }

        /// <summary>
        /// Configure and run the form
        /// </summary>
        public static void InstallerForm()
        {
            InstallerFormState = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MaximaPlugin.MForms.InstallerForm());
        }

        public static void SettingsForm()
        {
            SettingsFormState = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MaximaPlugin.MForms.SettingsForm());
        }

        public static void ThreadDebuggerPro()
        {
            ThreadDebuggerProState = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MaximaPlugin.MForms.DebugForm());
        }

        public static void ThreadLogPro()
        {
            ThreadLogProState = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MaximaPlugin.MForms.LogForm());
        }


    }
}