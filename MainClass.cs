﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MaximaPlugin.ControlObjects;
using SMath.Controls;
using SMath.Drawing;
using SMath.Manager;
using SMath.Math;

namespace MaximaPlugin
{

    public partial class MainClass : IPluginHandleEvaluation, IPluginLowLevelEvaluation, IPluginCustomRegion, IPluginToolboxGroups
    {
        #region Private fields

        DragAndDropFileType[] dragAndDropFileTypes;

        #endregion

        #region IPluginHandleEvaluation Members
        /// <summary>
        /// Define functions that will be evaluated by the plugin
        /// </summary>
        /// <param name="sessionProfile">SessionProfile</param>
        /// <returns></returns>
        TermInfo[] IPluginHandleEvaluation.GetTermsHandled(SessionProfile sessionProfile) {
            return new TermInfo[]
            {
                //direct equivalent for internal functions, 
                new TermInfo ("Int", TermType.Function, "Int(expr" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " lower limit" + sessionProfile.ArgumentsSeparator + " upper limit) calculates the definite integral with respect to var" , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("Int", TermType.Function, "Int(expr" + sessionProfile.ArgumentsSeparator + " var) calculates the indefinite integral with respect to var" , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("Lim", TermType.Function, "Limit(expr" + sessionProfile.ArgumentsSeparator + " var, value) calculates the limit of expr as var approaches value. Add +'0 or -'0 (zero marked as unit) for single sided limit." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("Diff", TermType.Function, "Diff(expr) derives expr with respect to x" , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("Diff", TermType.Function, "Diff(expr" + sessionProfile.ArgumentsSeparator + " var) derives expr with respect to var" , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("Diff", TermType.Function, "Diff(expr" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " n) derivates expr n times with respect to var" , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("Sum", TermType.Function, "[Maxima] Sum(expr" + sessionProfile.ArgumentsSeparator + " index" + sessionProfile.ArgumentsSeparator + " n" + sessionProfile.ArgumentsSeparator + " m) calculates the sum of expr for index starting at n and ending at m" , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("Det", TermType.Function, "Det(expr) calculates the determinant of expr" , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },

                //solve functions
                new TermInfo ("Solve", TermType.Function,"Solve(eqn" + sessionProfile.ArgumentsSeparator + " var) solves eqn for var. eqn is a boolean equation or a list of equations, var is a variable name or a list of names to solve for. Returns solutions as boolean equations var=value. Multiple solutions are given as a row vector. Use Assign() in order to apply solutions as assignments to var." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("LinSolve", TermType.Function,"LinSolve(eqn" + sessionProfile.ArgumentsSeparator + " var) solves eqn for var. eqn is a boolean equation or a list of equations, var is a variable name or a list of names to solve for. Returns solutions as boolean equations var=value. Multiple solutions are given as a row vector. Use Assign() in order to apply solutions as assignments to var." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("Algsys", TermType.Function,"Algsys(eqn" + sessionProfile.ArgumentsSeparator + " var) solves eqn for var. eqn is a boolean equation or a list of equations, var is a variable name or a list of names to solve for. Returns solutions as boolean equations var=value. Multiple solutions are given as a row vector. Use Assign() in order to apply solutions as assignments to var." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },
                new TermInfo ("mNewton", TermType.Function,"mNewton(eqn" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " init) solves (numeric) eqn for var. eqn is a boolean equation or a list of equations, var is a variable name or a list of names to solve for, init is a guessed startvalue or a list of guessed startvalues. Returns solutions as boolean equations var=value. Multiple solutions are given as a row vector. Use Assign() in order to apply solutions as assignments to var." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression),new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)),
                new TermInfo ("Fit", TermType.Function,"Fit(Data" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " function" + sessionProfile.ArgumentsSeparator + " params" + sessionProfile.ArgumentsSeparator + " init) Returns values for params  which minimize the mean square error between data and function. Data columns correspond to entries in var list. params is a list of parameter names with initial values init (both given als list). Values are returned as list of equations var=value and can be assigned using Assign()." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression),new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)),
                new TermInfo ("Fit", TermType.Function,"Fit(Data" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " function" + sessionProfile.ArgumentsSeparator + " params" + sessionProfile.ArgumentsSeparator + " init" + sessionProfile.ArgumentsSeparator + " tol)  Returns values for params  which minimize the mean square error between data and function. Data columns correspond to entries in var list. params is a list of parameter names with initial values init (both given als list). tol is a tolerance for the fit. Values are returned as list of equations var=value and can be assigned using Assign()." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Matrix), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.RealNumber)),
                new TermInfo ("Assign", TermType.Function,"Assign(eqn) converts the boolean equations var=expr into assignments var:=expr. Thus you can use the output of Fit(), Solve(), ODE2() directly for assigning the solution to the unknowns." , FunctionSections.Unknown , true),
                // MSE(3)
                new TermInfo ("MSE", TermType.Function,"MSE(Data" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " function) Returns the mean square error for the given data and model. Data columns correspond to entries in var list." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Matrix),new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)),
                // MSE(4)
                new TermInfo ("MSE", TermType.Function,"MSE(Data" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " function" + sessionProfile.ArgumentsSeparator + " params) Returns the mean square error for the given data and model. Data columns correspond to entries in var list. params is a list of name=value pairs to substitute any undefined variables beyond the data variables in the model function. " , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Matrix),new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)),
                // Residuals(3)
                new TermInfo ("Residuals", TermType.Function,"Residuals(Data" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " function) Returns the residuals between the given data and model function. Data columns correspond to entries in var list." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Matrix),new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)),
                // Residuals(4)
                new TermInfo ("Residuals", TermType.Function,"Residuals(Data" + sessionProfile.ArgumentsSeparator + " var" + sessionProfile.ArgumentsSeparator + " function" + sessionProfile.ArgumentsSeparator + " params) Returns the residuals between the given data and model function. Data columns correspond to entries in var list. params is a list of name=value pairs to substitute any undefined variables beyond the data variables in the model function. " , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Matrix),new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)),

                // Cross Product
                new TermInfo ("Cross", TermType.Function,"Cross(expr" + sessionProfile.ArgumentsSeparator + " expr" + sessionProfile.ArgumentsSeparator + ") Returns the result of cross product. Inputs are in form of vector. Also support 2D-Vectors. " , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)),

                //differential equations
                new TermInfo ("ODE"+GlobalProfile.DecimalSymbolStandard+"2", TermType.Function,"ODE.2(ode" + sessionProfile.ArgumentsSeparator + " f(x)" + sessionProfile.ArgumentsSeparator + " x) solves an ordinary differential equation (max second order) for function f(x) with independent variable x. Returns a boolean equation f(x)=expr, which can be converted into an assignment using Assign()." , FunctionSections.Unknown , true, new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression), new ArgumentInfo(ArgumentSections.Expression)) { IsUndefinedArgumentsSupported = true },

                //Plotting
                new TermInfo ("Draw2D", TermType.Function, "Draw2D(obj [" + sessionProfile.ArgumentsSeparator + " name] [" + sessionProfile.ArgumentsSeparator + " size]) creates a 2D plot using Maxima draw2d(). obj is a list of graphics objects (functions) or options (boolean equations option=value). Returns the file name for display in an Image region. If name is given, it is relative to the current document directory. Format is specified by file name extension: PNG (default), PDF or SVG. If no name (or just the extension for format specification) is given, a unique random temporary name is used. size is an optional list with (xpix, ypix). Size defaults to 300 x 240 pixels.", FunctionSections.Unknown , true),
                new TermInfo ("Draw3D", TermType.Function, String.Format("Draw3D(obj [" + sessionProfile.ArgumentsSeparator + " name] [" + sessionProfile.ArgumentsSeparator + " size]) creates a 3D plot using Maxima draw3d(). obj is a list of graphics objects (functions) or options (boolean equations option=value).  Returns the file name for display in an Image region. If name is given, it is relative to the current document directory. Format is specified by file name extension: PNG (default), PDF or SVG. If no name (or just the extension for format specification) is given, a unique random temporary name is used. size is an optional list with (xpix, ypix). Size defaults to 300 x 240 pixels.",sessionProfile.DecimalSymbol) , FunctionSections.Unknown , true),

                //maxima control
                new TermInfo ("Maxima", TermType.Function,"Maxima(expr [" + sessionProfile.ArgumentsSeparator + "debug]): Process expr in Maxima. If the flag  debug is given, the processing steps (SMath preprocessing, translation to Maxima, Maxima result, translation to SMath) can be inspected and modified  in the debug window. Alternatively, use MaximaLog() to see what is done on Maxima side." , FunctionSections.Unknown , true){ IsUndefinedArgumentsSupported = true },
                new TermInfo ("MaximaDefine", TermType.Function,"MaximaDefine(name [" + sessionProfile.ArgumentsSeparator + " expr]) Definition of name using expr in Maxima or define name in Maxima using it's current SMath value." , FunctionSections.Unknown , true){ IsUndefinedArgumentsSupported = true },
                new TermInfo ("MaximaControl", TermType.Function,"MaximaControl(command) („restart“): restart Maxima; („cleanup“): cleanup current Maxima session (reset and kill all);  („settings“): open settings window", FunctionSections.Unknown , true),
                new TermInfo ("MaximaTakeover", TermType.Function,"MaximaTakeover(command): Let Maxima handle the functions int(), diff(), det(), lim and sum(). Possible commands: 'all', 'none' or the function names (at least the first three characters like 'int', 'dif', 'sum', 'lim' or 'det')" , FunctionSections.Unknown , true),
                new TermInfo ("MaximaLog", TermType.Function,"MaximaLog(command): \"all\" show full log, \"clear\" clear log, get Maxima logdata from last request; („big“) open log window" , FunctionSections.Unknown , true),
            };
        }
        #endregion

        #region IPluginLowLevelEvaluation
        /// <summary>
        /// Redirect the plugin function to it's specific method
        /// </summary>
        /// <param name="root">Name of the function</param>
        /// <param name="args">List of arguments</param>
        /// <param name="context">Store</param>
        /// <param name="result">Result that will be shown on worksheet</param>
        /// <returns></returns>
        bool IPluginLowLevelEvaluation.ExpressionEvaluation(Term root, Term[][] args, ref Store context, ref Term[] result)
        {
            if (regularEnable || root.Text == "MaximaControl")
            {
                // save the arguments in Shared function
                SharedFunctions.rootHold = root;
                SharedFunctions.argsHold = args;
                SharedFunctions.contextHold = context;
                SharedFunctions.resultHold = result;

                if (root.Type == TermType.Function && root.Text == "Maxima")
                {
                    return MFunctions.MaximaAllround.Maxima(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "MaximaDefine")
                {
                    return MFunctions.MaximaAllround.MaximaDefine(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "MaximaControl")
                {
                    return MFunctions.MaximaAllround.MaximaControlF(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "MaximaTakeover")
                {
                    return MFunctions.MaximaAllround.MaximaTakeoverF(args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "MaximaLog")
                {
                    return MFunctions.MaximaAllround.MaximaLog(root, args, ref context, ref result);
                }
                // replacement functions
                else if (root.Text == "Int" && root.Type == TermType.Function && root.ArgsCount == 4)
                {
                    return MFunctions.InternOverwrite.Integrate(root, args, ref context, ref result);
                }
                else if (root.Text == "Int" && root.Type == TermType.Function && root.ArgsCount == 2)
                {
                    return MFunctions.InternOverwrite.IntegrateUnknown(root, args, ref context, ref result);
                }
                else if (root.Text == "Lim" && root.Type == TermType.Function && root.ArgsCount == 3)
                {
                    return MFunctions.InternOverwrite.Limit(root, args, ref context, ref result);
                }
                else if (root.Text == "Diff" && root.Type == TermType.Function && root.ArgsCount == 1)
                {
                    return MFunctions.InternOverwrite.Derivatex(root, args, ref context, ref result);
                }
                else if (root.Text == "Diff" && root.Type == TermType.Function && root.ArgsCount == 2)
                {
                    return MFunctions.InternOverwrite.Derivate(root, args, ref context, ref result);
                }
                else if (root.Text == "Diff" && root.Type == TermType.Function && root.ArgsCount == 3)
                {
                    return MFunctions.InternOverwrite.DerivateN(root, args, ref context, ref result);
                }
                else if (root.Text == "Sum" && root.Type == TermType.Function && root.ArgsCount == 4)
                {
                    return MFunctions.InternOverwrite.Sum(root, args, ref context, ref result);
                }
                else if (root.Text == "Det" && root.Type == TermType.Function && root.ArgsCount == 1)
                {
                    return MFunctions.InternOverwrite.Det(root, args, ref context, ref result);
                }
                //solve Functions
                else if (root.Type == TermType.Function && root.Text == "Solve" && root.ArgsCount == 2)
                {
                    return MFunctions.DirectMaximaFunctions.SymbSolve(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "LinSolve" && root.ArgsCount == 2)
                {
                    return MFunctions.DirectMaximaFunctions.SymbSolve(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "Algsys" && root.ArgsCount == 2)
                {
                    return MFunctions.DirectMaximaFunctions.SymbSolve(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == ("ODE.2") && root.ArgsCount == 3)
                {
                    return MFunctions.DirectMaximaFunctions.Ode2M(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "mNewton" && root.ArgsCount == 3)
                {
                    return MFunctions.DirectMaximaFunctions.mNewton(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "MSE" && (root.ArgsCount == 3 || root.ArgsCount == 4))
                {
                    return MFunctions.DirectMaximaFunctions.MSE(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "Residuals" && (root.ArgsCount == 3 || root.ArgsCount == 4))
                {
                    return MFunctions.DirectMaximaFunctions.Residuals(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "Fit" && root.ArgsCount == 5)
                {
                    return MFunctions.DirectMaximaFunctions.Fit(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "Fit" && root.ArgsCount == 6)
                {
                    return MFunctions.DirectMaximaFunctions.Fit6(root, args, ref context, ref result);
                }
                else if (root.Type == TermType.Function && root.Text == "Assign")
                {
                    return MFunctions.OtherUsefull.Assign(root, args, ref context, ref result);
                } else if (root.Type == TermType.Function && root.Text == "Cross")
                {
                    return MFunctions.DirectMaximaFunctions.CrossProduct(root, args, ref context, ref result);
                }
                //Plotting
                else if (root.Type == TermType.Function && (root.Text == "Draw2D" || root.Text == "Draw3D"))
                {
                    return MFunctions.DirectMaximaFunctions.Draw(root, args, ref context, ref result);
                }
                {
                    return false;
                }
            }
            return false;
        }

        #endregion

        #region IPluginToolboxGroups
        /// <summary>
        /// Define members of toolbox
        /// </summary>
        /// <param name="sessionProfile">SessionProfile</param>
        /// <returns></returns>
        public ToolboxGroup[] GetToolboxGroups(SessionProfile sessionProfile)
        {
            SvgPaths.Init();

            var groups = new ToolboxGroup[1];

            var buttons = new[]
            {
                new ButtonsMetaData(GetPngImage(Resource1.maxima))
                    { Size = new Size(36, 24), Description = "Maxima(): Execute expression in Maxima", Action = GetButtonAction("Maxima", 1, sessionProfile) },
                new ButtonsMetaData("Control")
                    { Size = new Size(54, 24), Description = "Control Maxima process. E.g. Restarting the session by using MaximaControl(\"restart\")", Action = GetButtonAction("MaximaControl", 1, sessionProfile) },
                new ButtonsMetaData("Define")
                    { Size = new Size(54, 24), Description = "Define in Maxima", Action = GetButtonAction("MaximaDefine", 1, sessionProfile) },
                new ButtonsMetaData("Takeover")
                    { Size = new Size(72, 24), Description = "Takeover by Maxima of SMath functions. Use \"all\", \"none\", \"int\", \"lim\", \"diff\", \"det\" or \"sum\".", Action = GetButtonAction("MaximaTakeover", 1, sessionProfile) },
                new ButtonsMetaData("Log")
                    { Size = new Size(36, 24), Description = "Maxima process log overview", Action = GetButtonAction("MaximaLog", 1, sessionProfile) },
                new ButtonsMetaData(new[]{
                    "M 7 14 L 6 13 L 9 10 L 6 7 L 7 6 L 10 9 L 13 6 L 14 7 L 11 10 L 14 13 L 13 14 L 10 11 Z" },
                                    new Size(20, 20))
                    { Size = new Size(32, 24), Description = "Cross product of vectors", Action = GetButtonAction("Cross", 2, sessionProfile)},
            };

            groups[0] = new ToolboxGroup { Title = "Maxima", Buttons = buttons, Index = 1 };

            return groups;
        }
        /// <summary>
        /// Get the image of the toolbox member
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns></returns>
        private IBitmap GetPngImage(Bitmap image)
        {
            var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return SessionsManager.Current.Specifics.StreamToBitmap(ms);
        }

        /// <summary>
        /// Button action when toolbox member is clicked
        /// </summary>
        /// <param name="name">Name of the function</param>
        /// <param name="argsCount">Number of arguments of the function</param>
        /// <param name="sessionProfile">SessionProfile</param>
        /// <returns></returns>
        private string GetButtonAction(string name, int argsCount, SessionProfile sessionProfile)
        {
            var term = new Term(name, TermType.Function, argsCount);

            TermInfo.ChangeTermByNamingType(sessionProfile, term, sessionProfile.NamingType, true);
            return term.Text + Brackets.LeftVisible + (argsCount > 1 ? new String(sessionProfile.ArgumentsSeparator, argsCount - 1) : String.Empty);
        }

        #endregion

        #region IPluginCustomRegion Members
        RegionBase IPluginCustomRegion.CreateNew(SessionProfile sessionProfile)
        {
            return new MaximaPlugin.PlotImage.MaximaPluginRegion(sessionProfile);
        }

        public string[] SupportedClipboardFormats
        {
            get { return null; }
        }

        DragAndDropFileType[] IPluginCustomRegion.DragAndDropFileTypes
        {
            get { return this.dragAndDropFileTypes; }
        }

        /// <summary>
        /// Context menu entries
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        MenuButton[] IPluginCustomRegion.GetContextMenuItems(MenuContext context)
        {
            MenuButton SaveButton = new MenuButton("Save...", delegate(MenuButtonArgs args)
            {
                string path = ((MaximaPlugin.PlotImage.MaximaPluginRegion)args.CurrentRegion).GetCanvas().imageFilePath;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                if (Path.GetExtension(path).Equals(".svg"))
                    saveFileDialog1.Filter = "svg files (*.svg)|*.svg|All files (*.*)|*.*";
                else if (Path.GetExtension(path).Equals(".pdf"))
                    saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                else
                    saveFileDialog1.Filter = "png files (*.png)|*.png|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(path, saveFileDialog1.FileName, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not save file to disk. Original error: " + ex.Message);
                    }
                }
            });
            MenuButton BorderButton = new MenuButton("Switch border on/off", delegate (MenuButtonArgs args)
            {
                bool temp = ((MaximaPlugin.PlotImage.MaximaPluginRegion)args.CurrentRegion).GetCanvas().borderOn;
                if (temp)
                    ((MaximaPlugin.PlotImage.MaximaPluginRegion)args.CurrentRegion).GetCanvas().borderOn = false;
                else
                    ((MaximaPlugin.PlotImage.MaximaPluginRegion)args.CurrentRegion).GetCanvas().borderOn = true;
            });
            MenuButton[] contextMenuItems = new[]
            {
                BorderButton,
                SaveButton

            };
            return contextMenuItems;
        }

        /// <summary>
        /// Custom Insert menu entries
        /// </summary>
        /// <param name="sessionProfile"></param>
        /// <returns></returns>
        MenuButton[] IPluginCustomRegion.GetMenuItems(SessionProfile sessionProfile)
        {
            MenuButton menubutton = new MenuButton("Maxima");
            menubutton.Icon = SMath.Drawing.Graphics.Specifics.BitmapFromNativeImage(Resource1.maxima16c);
            menubutton.Behavior = MenuButtonBehavior.AlwaysEnabled;
            menubutton.AppendChild("Log", MLog);
            menubutton.AppendChild("Debug", MDebug);
            menubutton.AppendChild("Settings", MSettings);
            menubutton.AppendChild(new MenuButton("Draw2D", delegate (MenuButtonArgs args)
            {
                args.CurrentRegion = new PlotImage.MaximaPluginRegion(sessionProfile, PlotImage.PlotStore.PlotType.plot2D);
            }  ));
            menubutton.AppendChild(new MenuButton("Draw3D", delegate (MenuButtonArgs args)
            {
                args.CurrentRegion = new PlotImage.MaximaPluginRegion(sessionProfile, PlotImage.PlotStore.PlotType.plot3D);
            }  ));
            menubutton.AppendChild("Restart", MRestart);
            menubutton.AppendChild("Help", MHelp);
            return new MenuButton[] { menubutton };
        }
        string IPluginCustomRegion.TagName
        {
            get { return "maximaplugin"; }
        }

        public Type RegionType
        {
            get
            {
                return typeof(MaximaPlugin.PlotImage.MaximaPluginRegion);
            }
        }
        #endregion

        #region IPlugin Members

        public static bool regularEnable = true;

        void IPlugin.Initialize()
        {
            // No Drag'n'Drop files supported
            this.dragAndDropFileTypes = new DragAndDropFileType[0];
        }

        void IDisposable.Dispose()
        {
            ControlObjects.Translator.CloseMaxima();
        }
        #endregion

        #region Buttons
        void MLog(MenuButtonArgs args)
        {
            MForms.FormControl.OpenForm("ThreadLogPro");

			args.CurrentRegions = new RegionBase[0];
        }
        void MDebug(MenuButtonArgs args)
        {
            MForms.FormControl.OpenForm("ThreadDebuggerPro");

			args.CurrentRegions = new RegionBase[0];
        }
        void MSettings(MenuButtonArgs args)
        {
            SharedFunctions.initializingOverMenue = true;
            MForms.SettingsForm sf = new MForms.SettingsForm();

            sf.Show();

            args.CurrentRegions = new RegionBase[0];
        }

        void MHelp(MenuButtonArgs args) 
        {
            Translator.GetMaxima().StartSession();

            // get absolute path of Maxima
            string maximaPath = Translator.GetMaxima().GetPathToMaximaAbs();

            //Find and extract the maxima version
            Match match = Regex.Match(maximaPath, @"^(.*?\\maxima-\d+\.\d+\.\d+)");

            Match maximaVersion = Regex.Match(maximaPath, @"\d+\.\d+\.\d+");

            if (match.Success)
            {
                maximaPath = match.Value.Replace("\\", "/");
                string fileUrl = "file:///" + maximaPath +"/share/maxima/" + maximaVersion.Value + "/doc/html/maxima_toc.html";

                // execute the URL using process
                // the URL will be opened using default browser
                string localFilePath = new Uri(fileUrl).LocalPath;
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = localFilePath,
                        UseShellExecute = true
                    });
                } catch { }
            }
        }

        void MRestart(MenuButtonArgs args)
        {
            ControlObjects.Translator.GetMaxima().RestartMaxima();
            args.CurrentRegions = new RegionBase[0];
            args.Worksheet.StartEvaluation();
        }
        #endregion
    }
}