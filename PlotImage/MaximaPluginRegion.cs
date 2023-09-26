using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Globalization;

using SMath.Manager;
using SMath.Controls;
using SMath.Math;
using System.Runtime.CompilerServices;

namespace MaximaPlugin.PlotImage
{
    public class MaximaPluginRegion : RegionHolder<MaximaPluginCanvas>
    {
        public List<Definition> AvailableItems;
        //Mousepointer memorize
        public float mouseX = 0;
        public float mouseY = 0;
        //RedirectingVars
        public double dMouseX = 0.5;
        public double dMouseY = 0.5;
        public double dMouseW = 0;
        //Factors corressponding to the axis ranges
        // ToDo MK 2018 09 14: Rename and handle log plots
        //public double factorX;
        //public double factorY;
        //public double factorZ;
        public double changeValue;
        System.Diagnostics.Stopwatch dblclicktimer = new System.Diagnostics.Stopwatch();



        //Control Vars
        bool sizeChange = false;
        bool controlKeyDownAxis = false;
        bool mouseDown = false;
        public bool formOpen = false;
        PlotSettings psf = null;
        private const double zoomFactor = 30;

        bool mouseDoubleClick = false;
        bool firstEval = false;

        bool setFocusEvent = false;

        bool isShiftKeyDown = false;
        bool isCtrlKeyDown = false;


        //Constructors
        public MaximaPluginRegion(SessionProfile sessionProfile)
            : base(sessionProfile)
        {

        }
        /// <summary>
        /// Constructor of the Maxima plot region
        /// </summary>
        /// <param name="sessionProfile"></param>
        /// <param name="pt">2D or 3D</param>
        public MaximaPluginRegion(SessionProfile sessionProfile, PlotImage.PlotStore.PlotType pt)
            : base(sessionProfile)
        {
            canv.plotStore.plotType = pt;
            firstEval = true;

            //axis, grid and size for 3D plot 
            if (canv.plotStore.plotType == PlotStore.PlotType.plot3D)
            {
                canv.plotStore.enablexAxis = false;
                canv.plotStore.enableyAxis = false;

                canv.plotStore.width = 300;
                canv.plotStore.height = 300;

                this.SetSize(new Size(canv.plotStore.width, canv.plotStore.height)); //this method trigger the onSizeChanged event
                this.Resize();
            }

            // set default scaling states (see SS-198)
            // x automatic for 2D
            // x,y automatic for 3D
            canv.plotStore.xRangeS = PlotStore.State.Disable;
            if (canv.plotStore.plotType == PlotStore.PlotType.plot3D) canv.plotStore.yRangeS = PlotStore.State.Disable;
            canv.lastInput = "";
            canv.SetLastRequest();
            callRedraw();
            
            //add focus event handler
            base.FocusChanged += new RegionEventHandler(OnFocusChanged);
            setFocusEvent = true;
        }

        public MaximaPluginRegion(MaximaPluginRegion region)
            : base(region)
        {
            AvailableItems = region.AvailableItems;
            mouseX = region.mouseX;
            mouseY = region.mouseY;
            dMouseX = region.dMouseX;
            dMouseY = region.dMouseY;
            dMouseW = region.dMouseW;
            //            factorX = region.factorX;
            //            factorY = region.factorY;
            //            factorZ = region.factorZ;
            formOpen = region.formOpen;
            psf = region.psf;
            canv.borderOn = region.canv.borderOn;

            //add focus event handler
            base.FocusChanged += new RegionEventHandler(OnFocusChanged);
        }

        /// <summary>
        /// Doubleclick: open/activate settings dialog
        /// Click: update canvas variables
        /// 
        /// Doubleclick is identified with timer dblclicktimer.
        /// It is reset upon OnMouseUp and evaluated at OnMouseDown.
        /// If time between mouseup and next mousedown is less then whatever, 
        /// we assume a doubleclick.
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseDown(MouseEventOptions e)
        {
            base.OnMouseDown(e);

            // Store mouse location/state       
            mouseDown = true;
            mouseX = e.X;
            mouseY = e.Y;
            dMouseX = e.X / canv.Size.Width;
            dMouseY = e.Y / canv.Size.Height;
            mouseDoubleClick = false;

            // set focus event handler if not set yet -> only for copy
            // this only apply to copy. Copy has weird interaction with custom mouse event.
            // the custom event does not registered on copy action but on cloning it works.
            // perhaps if it is fixed in the future this can be removed
            if (!setFocusEvent)
            {
                this.FocusChanged += new RegionEventHandler(OnFocusChanged);
            }

            if (!dblclicktimer.IsRunning)
            {
                dblclicktimer.Start();
            }
            else
            {
                dblclicktimer.Stop();
                if (dblclicktimer.ElapsedMilliseconds < 500)
                {
                    if (formOpen)
                        psf.Focus();
                    else
                    {
                        mouseDoubleClick = true;
                        canv.oldPlotStore = canv.plotStore.Clone() as PlotStore;
                        psf = new PlotSettings(this);
                        psf.Show();
                        formOpen = true;
                    }
                    this.Invalidate();
                }
                dblclicktimer.Reset();
            }
        }
        /// <summary>
        /// Actions on mouse drag (pan, orbit)
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseMove(MouseEventOptions e)
        {
            //rotation and panning
            if (mouseDown && canv.mouseD && !sizeChange)
            {
                double dy = mouseY - e.Y;
                double dx = mouseX - e.X;
                dMouseX = e.X / canv.Size.Width;
                dMouseY = e.Y / canv.Size.Height;
                // Orbiting by mouse drag in 3D plot with interactive view setting
                if (canv.plotStore.view == PlotStore.State.Interactive && canv.plotStore.plotType == PlotStore.PlotType.plot3D && !isShiftKeyDown)
                {
                    // change zenith value based on y-move and make sure that it is between 0 and 180°
                    if (canv.plotStore.zenith + dy >= 180)
                        canv.plotStore.zenith = 180;// (canv.plotStore.zenith + dy) - 180;
                    else if (canv.plotStore.zenith + dy <= 0)
                        canv.plotStore.zenith = 0;//180 + (canv.plotStore.zenith + dy);
                    else
                        canv.plotStore.zenith += dy;

                    // change azimut value based on x-move and make sure that it is between 0 and 360°
                    if (canv.plotStore.azimuth + dx >= 360)
                        canv.plotStore.azimuth = (canv.plotStore.azimuth + dx) - 360;
                    else if (canv.plotStore.azimuth + dx <= 0)
                        canv.plotStore.azimuth = 360 + (canv.plotStore.azimuth + dx);
                    else
                        canv.plotStore.azimuth += dx;
                }
                // Panning by mouse drag in 2D-Plots and mapview of 3D-Plots just for those axes with interactive scaling
                else if (canv.plotStore.plotType == PlotStore.PlotType.plot2D || canv.plotStore.view == PlotStore.State.MapView)
                {
                    // x axis
                    if (canv.plotStore.xRangeS == PlotStore.State.Interactive)
                    {
                        if (canv.plotStore.xLogarithmic == PlotStore.State.Enable)
                        {
                            changeValue = Math.Exp((Math.Log(canv.plotStore.xMaxRange) - Math.Log(canv.plotStore.xMinRange)) * dx / canv.Size.Width);
                            canv.plotStore.xMinRange *= changeValue;
                            canv.plotStore.xMaxRange *= changeValue;
                        }
                        else
                        {
                            changeValue = (canv.plotStore.xMaxRange - canv.plotStore.xMinRange) * dx / canv.Size.Width;
                            canv.plotStore.xMinRange += changeValue;
                            canv.plotStore.xMaxRange += changeValue;
                        }
                    }
                    // y axis
                    if (canv.plotStore.yRangeS == PlotStore.State.Interactive)
                    {
                        if (canv.plotStore.yLogarithmic == PlotStore.State.Enable)
                        {
                            changeValue = Math.Exp((Math.Log(canv.plotStore.yMaxRange) - Math.Log(canv.plotStore.yMinRange)) * dy / canv.Size.Height);
                            canv.plotStore.yMinRange /= changeValue;
                            canv.plotStore.yMaxRange /= changeValue;
                        }
                        else
                        {
                            changeValue = (canv.plotStore.yMaxRange - canv.plotStore.yMinRange) * dy / canv.Size.Height;
                            canv.plotStore.yMinRange -= changeValue;
                            canv.plotStore.yMaxRange -= changeValue;
                        }
                    }
                }
                // pan the x and y axes of a 3D plot
                else if (canv.plotStore.plotType == PlotStore.PlotType.plot3D && isShiftKeyDown)
                {

                    if (canv.plotStore.xRangeS == PlotStore.State.Interactive)
                    {
                        double ddx = (dx * System.Math.Cos((canv.plotStore.azimuth / 180) * System.Math.PI)) + (dy * System.Math.Sin((canv.plotStore.azimuth / 180) * System.Math.PI));
                        changeValue = (canv.plotStore.xMaxRange - canv.plotStore.xMinRange) * ddx / canv.Size.Width;
                        canv.plotStore.xMaxRange += changeValue;
                        canv.plotStore.xMinRange += changeValue;
                    }
                    if (canv.plotStore.yRangeS == PlotStore.State.Interactive)
                    {
                        double ddy = dy = (dx * System.Math.Sin((canv.plotStore.azimuth / 180) * System.Math.PI)) - (dy * System.Math.Cos((canv.plotStore.azimuth / 180) * System.Math.PI));
                        changeValue = (canv.plotStore.yMaxRange - canv.plotStore.yMinRange) * ddy / canv.Size.Height;
                        canv.plotStore.yMinRange += changeValue;
                        canv.plotStore.yMaxRange += changeValue;
                    }
                } 
                //canv.plotApproval = true;
                if (!mouseDoubleClick)
                {
                    canv.SetLastRequest();
                    callRedraw();
                    mouseDoubleClick = false;
                }
                canv.Invalidate();

                mouseX = e.X;
                mouseY = e.Y;

            }
            base.OnMouseMove(e);
        }

        public override void OnMouseUp(MouseEventOptions e)
        {
            canv.mouseD = false;
            mouseDown = false;
            if (sizeChange)
            {
                sizeChange = false;
                callRedraw();
            }
            base.Invalidate();
            base.OnMouseUp(e);
            dblclicktimer.Start();

        }
        // Zoom
        protected override void OnMouseWheelAction(MouseEventOptions e, int delta)
        {
            double ddelta = delta;
            if (ddelta > 0)
                ddelta = 2;
            else
                ddelta = -2;
            dMouseW = dMouseW + delta;

            // Zoom x and y axis if not a 3D plot in z-only-mode
            if (!(canv.plotStore.plotType == PlotStore.PlotType.plot3D && (isShiftKeyDown || isCtrlKeyDown)))
            {
                // Zoom x axis if it is in interactive mode
                if (canv.plotStore.xRangeS == PlotStore.State.Interactive)
                {
                    if (canv.plotStore.xLogarithmic == PlotStore.State.Enable)
                    {
                        changeValue = Math.Exp((Math.Log(canv.plotStore.xMaxRange) - Math.Log(canv.plotStore.xMinRange)) * delta / zoomFactor);
                        canv.plotStore.xMinRange /= changeValue;
                        canv.plotStore.xMaxRange *= changeValue;
                    }
                    else
                    {
                        changeValue = (canv.plotStore.xMaxRange - canv.plotStore.xMinRange) * delta / zoomFactor;
                        canv.plotStore.xMaxRange -= changeValue;
                        canv.plotStore.xMinRange += changeValue;
                    }
                }

                // Zoom y axis if it is in interactive mode and not a 3D plot in z-only-mode
                if (canv.plotStore.yRangeS == PlotStore.State.Interactive)
                {
                    if (canv.plotStore.yLogarithmic == PlotStore.State.Enable)
                    {
                        changeValue = Math.Exp((Math.Log(canv.plotStore.yMaxRange) - Math.Log(canv.plotStore.yMinRange)) * delta / zoomFactor);
                        canv.plotStore.yMinRange /= changeValue;
                        canv.plotStore.yMaxRange *= changeValue;
                    }
                    else
                    {
                        changeValue = (canv.plotStore.yMaxRange - canv.plotStore.yMinRange) * delta / zoomFactor;
                        canv.plotStore.yMaxRange -= changeValue;
                        canv.plotStore.yMinRange += changeValue;
                    }
                }
            }
            // zoom of z if it is a 3D plot
            if (canv.plotStore.plotType == PlotStore.PlotType.plot3D && canv.plotStore.zRangeS == PlotStore.State.Interactive && !(isShiftKeyDown || isCtrlKeyDown))
            {
                if (canv.plotStore.zLogarithmic == PlotStore.State.Enable)
                {
                    changeValue = Math.Exp((Math.Log(canv.plotStore.zMaxRange) - Math.Log(canv.plotStore.zMinRange)) * delta / zoomFactor);
                    canv.plotStore.zMinRange /= changeValue;
                    canv.plotStore.zMaxRange *= changeValue;
                }
                else
                {
                    changeValue = (canv.plotStore.zMaxRange - canv.plotStore.zMinRange) * delta / zoomFactor;
                    canv.plotStore.zMaxRange += changeValue;
                    canv.plotStore.zMinRange -= changeValue;
                }
            }

            if(canv.plotStore.plotType == PlotStore.PlotType.plot3D && isShiftKeyDown)
            {
                //perform some calc with shiftkey
                canv.plotStore.scalAzimuth += (delta/zoomFactor);
                if(canv.plotStore.scalAzimuth < 0)
                {
                    canv.plotStore.scalAzimuth = 0;
                }
            } else if(canv.plotStore.plotType == PlotStore.PlotType.plot3D && isCtrlKeyDown)
            {
                //perform some calc with ctrlkey
                canv.plotStore.scalZenith += (delta / zoomFactor);
                if (canv.plotStore.scalZenith < 0)
                {
                    canv.plotStore.scalZenith = 0;
                }
            }
            //canv.plotApproval = true;
            canv.SetLastRequest();
            callRedraw();
            canv.Invalidate();
            base.OnMouseWheelAction(e, delta);
            if (canv.plotStore.viewRedirecting == PlotStore.State.Enable || canv.plotStore.mouseRedirecting == PlotStore.State.Enable)
            {
                canv.SetLastRequest();
                callRedraw();
                this.RequestEvaluation();
            }

        }

        //handles focus changed event
        public  void OnFocusChanged( RegionBase sender)
        {
            if(!this.Focused && formOpen)
            {
                CloseSettingForm();
            }
        }

        private void CloseSettingForm()
        {
            if (psf != null)
            {
                if (psf.InvokeRequired)
                {
                    psf.Invoke(new MethodInvoker(delegate ()
                    {
                        psf.Close();
                        formOpen = false;
                    }));
                }
                else
                {
                    psf.Close();
                    formOpen = false;
                }

            }
        }

        // handle the modifier keys
        public override void OnKeyDown(KeyEventOptions e)
        {
            if (e.KeyCode == (int)InputKeys.ShiftKey)
                    isShiftKeyDown = true;
            else if(e.KeyCode == (int)InputKeys.ControlKey)
                isCtrlKeyDown = true;
            base.OnKeyDown(e);

        }
        public override void OnKeyUp(KeyEventOptions e)
        {
            if (e.KeyCode == (int)InputKeys.ShiftKey)
                isShiftKeyDown = false;
            else if (e.KeyCode == (int)InputKeys.ControlKey)
                isCtrlKeyDown = false;

            base.OnKeyUp(e);
        }

        //SMATH
        public override RegionBase Clone()
        {
            // on clone close the settings dialog when open
            CloseSettingForm();

            //disable mouse events to prevent multiple draw calls
            mouseDown = false;
            canv.mouseD = false;

            //create clone and clone the plot store too
            var clone = new MaximaPluginRegion(this);
            clone.canv.plotStore = this.canv.plotStore.Clone() as PlotStore;
            clone.callRedraw();
            return clone;
        }

        // handler for region size changes
        protected override void OnSizeChanged(MouseEventOptions e)
        {
            
            if (canv.plotStore.pictureSizeState == PlotImage.PlotStore.State.Interactive && !firstEval)
            {
                sizeChange = true;
                canv.plotStore.width = canv.Size.Width;
                canv.plotStore.height = canv.Size.Height;
                canv.redrawCanvas = true;

                canv.ScalImg(canv.imageEo);
            }
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Interpretation of optionstring from xml file, taken from ImageEditRegion Plugin
        /// </summary>
        /// <param name="optionString"></param>
        public void OptionInterpreter(string optionString)
        {
            //Buffer vars
            string[] temp = new string[1] { "" };
            int tempArrayCounter = 0;
            //Loop vars
            int loopCounter = 0;
            int start = 0, end = 0;
            int sLength = optionString.Length;
            while (loopCounter < sLength && tempArrayCounter < temp.Length)
            {
                if (optionString[loopCounter] == ';')
                {
                    end = loopCounter;
                    temp[tempArrayCounter] = optionString.Substring(start, (end) - start);
                    tempArrayCounter++;
                    start = loopCounter + 1;
                }
                loopCounter++;
            }
            if (loopCounter > 0 && temp[0] == "border=true")
                canv.borderOn = true;
            else if (loopCounter > 0 && temp[0] == "border=false")
                canv.borderOn = false;
        }

        /// <summary>
        /// Generation of optionstring for xml file, taken from ImageEditRegion Plugin
        /// </summary>
        /// <returns></returns>
        public string OptionWriter()
        {
            string temp = "";
            if (canv.borderOn)
                temp = "border=true;";
            else
                temp = "border=false;";

            return temp;
        }


        // Serialize image and the contents of plotStore
        public override void ToXml(StorageWriter storage, FileParsingContext parsingContext)
        {
            FileInfo fileInfo;
            FileStream fileStream;
            BinaryReader binaryReader;
            XmlSerializer serializer = new XmlSerializer(typeof(PlotStore));
            int imageLength = 0;
            byte[] data;
            if (File.Exists(canv.imageFilePath))
            {
                fileInfo = new FileInfo(Path.ChangeExtension(canv.imageFilePath, "png"));
                fileStream = new FileStream(Path.ChangeExtension(canv.imageFilePath, "png"), FileMode.Open);
                binaryReader = new BinaryReader(fileStream);
                data = binaryReader.ReadBytes((int)fileInfo.Length);
                imageLength = data.Length;
            }
            else
                data = new byte[1] { 0 };

            var writer = storage.GetXmlWriter();
            writer.WriteStartElement("ImageFile");
            writer.WriteAttributeString("FileName", Path.GetFileName(canv.imageFilePath));
            writer.WriteAttributeString("DataLenght", Convert.ToString(imageLength));
            writer.WriteBase64(data, 0, data.Length);
            writer.WriteEndElement();
            canv.plotStore.option = OptionWriter();
            writer.WriteStartElement("plotstore");
            serializer.Serialize(writer, canv.plotStore);
            writer.WriteEndElement();
            base.ToXml(storage, parsingContext);
        }

        // Deserialize image and plotStore content
        public override void FromXml(StorageReader storage, FileParsingContext parsingContext)
        {
            var reader = storage.GetXmlReader();

            XmlSerializer serializer = new XmlSerializer(typeof(PlotStore));
            byte[] data = new byte[1];
            int dataLenght = 0;

            while (reader.Read())
            {
                if (reader.Name == "ImageFile")
                {
                    //canv.imageFilePath = Path.Combine(GlobalTools.workingFolder, reader.GetAttribute("FileName"));
                    dataLenght = Convert.ToInt32(reader.GetAttribute("DataLenght"));
                    if (dataLenght > 0)
                    {
                        data = new byte[dataLenght];
                        reader.ReadElementContentAsBase64(data, 0, dataLenght);

                        //only output in png format is supported
                        using (BinaryWriter writer = new BinaryWriter(File.Open(canv.imageFilePath, FileMode.Create)))
                        {
                            writer.Write(data);
                        }
                    }
                }

                if (reader.Name == "PlotStore")
                {
                    canv.plotStore = (PlotStore)serializer.Deserialize(reader);
                    OptionInterpreter(canv.plotStore.option);
                    canv.Size = new Size(canv.plotStore.width, canv.plotStore.height);
                }

                if (reader.Name == "input" || reader.Name == "maximaplugin")
                    break;
            }

            //manually override this
            using (FileStream stream = new FileStream(Path.ChangeExtension(canv.imageFilePath, "png"), FileMode.Open, FileAccess.Read))
            {
                canv.imageEo = System.Drawing.Image.FromStream(stream);
            }

            canv.ScalImg(canv.imageEo);

            base.FromXml(storage, parsingContext);
            this.Invalidate();
            this.RequestEvaluation();
        }

        private void callRedraw()
        {
            canv.Plot();
            canv.ScalImg(canv.imageEo);
            if (formOpen)
            {
                //some events calls from different thread causing cross thread exception and sometimes it's from the same thread
                if (psf.InvokeRequired)
                {
                    psf.Invoke(new MethodInvoker(delegate ()
                    {
                        psf.Restore();
                        psf.Refresh();
                    }));
                }
                else
                {
                    psf.Restore();
                    psf.Refresh();
                }
            }
        }


        public override void OnEvaluation(SMath.Math.Store store)
        {
            //IncludeDefs(ref store);
            string input = "";

            try
            {
                //boolean expression check
                string check = TermsConverter.ToString(Terms);
                string pattern = @"if\(([^,\s]+)\s*,\s*([^,\s]+)\s*,\s*([^)]+)\)";
                Match match = Regex.Match(check, pattern);

                if (!match.Success)
                {
                    var out1 = Computation.Preprocessing(Terms, ref store);
                    input = TermsConverter.ToString(out1);
                    AvailableItems = new List<Definition>();
                    for (int k = store.Count - 1; k > -1; k--)
                    {
                        AvailableItems[k] = store[k];
                    }
                }
                else
                    input = check;
            }
            catch
            {
            }
            if (!firstEval || canv.lastInput != input)
            {
                canv.lastInput = input;
                canv.SetLastRequest();

                if (canv.redrawCanvas)
                {
                    callRedraw();
                }
            }
            else
            {
                firstEval = false;
            }


            AddDefs(store);
            //dMouseX = 0;
            //dMouseY = 0;
            //dMouseW = 0;
            base.OnEvaluation(store);

        }

        // Redirection to context
        public void AddDefs(SMath.Math.Store store)
        {
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = GlobalProfile.ArgumentsSeparatorStandard.ToString()
            };

            //MOUSE
            if (canv.plotStore.mouseRedirecting == PlotStore.State.Enable)
            {
                store.AddDefinition(canv.plotStore.varNameMouseX, new Entry(dMouseX.ToString("0.0000", nfi)));
                store.AddDefinition(canv.plotStore.varNameMouseY, new Entry(dMouseY.ToString("0.0000", nfi)));
                store.AddDefinition(canv.plotStore.varNameMouseWheel, new Entry(dMouseW.ToString("0.0000", nfi)));
            }
            //VIEW
            if (canv.plotStore.viewRedirecting == PlotStore.State.Enable)
            {
                store.AddDefinition(canv.plotStore.varNameZenith, new Entry(canv.plotStore.zenith.ToString("0.0000", nfi)));
                store.AddDefinition(canv.plotStore.varNameAzimuth, new Entry(canv.plotStore.azimuth.ToString("0.0000", nfi)));
            }
            //AXIS
            if (canv.plotStore.xRedirecting == PlotStore.State.Enable)
            {
                store.AddDefinition(canv.plotStore.varNameXmin, new Entry(canv.plotStore.xMinRange.ToString("0.0000", nfi)));
                store.AddDefinition(canv.plotStore.varNameXmax, new Entry(canv.plotStore.xMaxRange.ToString("0.0000", nfi)));
            }
            if (canv.plotStore.yRedirecting == PlotStore.State.Enable)
            {
                store.AddDefinition(canv.plotStore.varNameYmin, new Entry(canv.plotStore.yMinRange.ToString("0.0000", nfi)));
                store.AddDefinition(canv.plotStore.varNameYmax, new Entry(canv.plotStore.yMaxRange.ToString("0.0000", nfi)));
            }
            if (canv.plotStore.zRedirecting == PlotStore.State.Enable)
            {
                store.AddDefinition(canv.plotStore.varNameZmin, new Entry(canv.plotStore.zMinRange.ToString("0.0000", nfi)));
                store.AddDefinition(canv.plotStore.varNameZmax, new Entry(canv.plotStore.zMaxRange.ToString("0.0000", nfi)));
            }


        }
    }
}
