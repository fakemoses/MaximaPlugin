using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaximaPlugin.PlotImage
{
    public partial class PlotSettings : Form
    {
        public PlotStore plotStore;
        public MaximaPluginRegion region;
        public MaximaPluginCanvas regionC;
        public bool abort = false;
        public bool firstLocationSet = true;
        public bool initComplete = false;

        #region FORM CONTROL
        public PlotSettings(MaximaPlugin.PlotImage.MaximaPluginRegion region)
        {
            InitializeComponent();
            this.region = region;
            this.regionC = region.GetCanvas();

            this.plotStore = region.GetCanvas().plotStore;
            if (plotStore.plotType == PlotImage.PlotStore.PlotType.plot3D)
            {
                groupBox2.Visible = true;
                groupBox1.Visible = true;
                groupBox30.Visible = true;
                groupBox4.Visible = true;
                groupBox6.Visible = true;
            }
            Restore();
            initComplete = true;
            this.Size = new System.Drawing.Size(310, 525);

        }
        private void PlotSettings_Load(object sender, EventArgs e)
        {
            SetLocation();
            ToolTip toolTip1 = new ToolTip();
            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;
            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.textTitle, "Plot title (special characters allowed)");
            toolTip1.SetToolTip(this.textXtitle, "label for the x axis (special characters allowed)");
            toolTip1.SetToolTip(this.textYtitle, "label for the y axis (special characters allowed)");
            toolTip1.SetToolTip(this.textZtitle, "label for the z axis (special characters allowed)");
            toolTip1.SetToolTip(this.radioXrangeDisable, "Automatic limits by Gnuplot");
            toolTip1.SetToolTip(this.radioYrangeDisable, "Automatic limits by Gnuplot");
            toolTip1.SetToolTip(this.radioZrangeDisable, "Automatic limits by Gnuplot");
            toolTip1.SetToolTip(this.radioXrangeCustom, "Specify the values in the entry fields");
            toolTip1.SetToolTip(this.radioYrangeCustom, "Specify the values in the entry fields");
            toolTip1.SetToolTip(this.radioZrangeCustom, "Specify the values in the entry fields");
            toolTip1.SetToolTip(this.radioXrangeInteractive, "Enable mouse zoom and pan");
            toolTip1.SetToolTip(this.radioYrangeInteractive, "Enable mouse zoom and pan");
            toolTip1.SetToolTip(this.radioZrangeInteractive, "Enable mouse zoom and pan");
            toolTip1.SetToolTip(this.textXrangeMin, "Lower limit, must be < than upper limit, must be > 0 in log plots");
            toolTip1.SetToolTip(this.textYrangeMin, "Lower limit, must be < than upper limit, must be > 0 in log plots");
            toolTip1.SetToolTip(this.textZrangeMin, "Lower limit, must be < than upper limit, must be > 0 in log plots");
            toolTip1.SetToolTip(this.textXrangeMax, "Upper limit, must be > than lower limit");
            toolTip1.SetToolTip(this.textYrangeMin, "Upper limit, must be > than lower limit");
            toolTip1.SetToolTip(this.textZrangeMin, "Upper limit, must be > than lower limit");
            toolTip1.SetToolTip(this.checkXgrid, "Enable grid lines");
            toolTip1.SetToolTip(this.checkYgrid, "Enable grid lines");
            toolTip1.SetToolTip(this.checkZgrid, "Enable grid lines");
            toolTip1.SetToolTip(this.checkXlog, "Enable logarithmic axis");
            toolTip1.SetToolTip(this.checkYlog, "Enable logarithmic axis");
            toolTip1.SetToolTip(this.checkZlog, "Enable logarithmic axis");
            toolTip1.SetToolTip(this.textXbase, "Base of the logarithm");
            toolTip1.SetToolTip(this.textYbase, "Base of the logarithm");
            toolTip1.SetToolTip(this.textZbase, "Base of the logarithm");
            toolTip1.SetToolTip(this.checkBorder, "Check to set custom border code");
            toolTip1.SetToolTip(this.textBorderVal, "Sum of 1:left, 2:right, 4: bottom, 8: top");
            toolTip1.SetToolTip(this.checkTextSizeCustom, "Use a custom text size");
            toolTip1.SetToolTip(this.textTextSize, "Text size for tics and labels (pt)");
            toolTip1.SetToolTip(this.checkGrid, "Use custom grid for surfaces");
            toolTip1.SetToolTip(this.textGridXu, "Grid points in x or u direction");
            toolTip1.SetToolTip(this.textGridYv, "Grid points in y or v direction");
            toolTip1.SetToolTip(this.checkContour, "Use custom contour settings");
            toolTip1.SetToolTip(this.textContourLevels, "# of contour lines");
            toolTip1.SetToolTip(this.comboBoxContour, "Location of contour lines");
            toolTip1.SetToolTip(this.checkPm3d, "Enable enhanded colouring mode (PM3D)");
            toolTip1.SetToolTip(this.textPm3dPalette, "PM3D palette, e.g. [red, green, blue]");
            toolTip1.SetToolTip(this.textWidth, "Width of the plot (pixel)");
            toolTip1.SetToolTip(this.textHeight, "Height of the plot (pixel)");
            toolTip1.SetToolTip(this.comboTerm, "Image format (gnuplot terminal)");
            toolTip1.SetToolTip(this.radioSizeAuto, "Image size controlled by region size");
            toolTip1.SetToolTip(this.radioSizeCustom, "Image size controlled by entry fields");
            toolTip1.SetToolTip(this.textAzimut, "Azimut angle (°)");
            toolTip1.SetToolTip(this.textZenit, "Zenit angle (°)");
            toolTip1.SetToolTip(this.textScalE, "Scaling in all directions");
            toolTip1.SetToolTip(this.textScalA, "Scaling in z only");
            toolTip1.SetToolTip(this.radioViewCustom, "View control by entry fields");
            toolTip1.SetToolTip(this.radioViewInteractiv, "View control by mouse (drag for orbit, wheel for zoom)");
            toolTip1.SetToolTip(this.radioViewXY, "Map view (xy-plane)");
            toolTip1.SetToolTip(this.radioViewDisable, "Default view");
            toolTip1.SetToolTip(this.tabAxes, "Range, log, grid");
            toolTip1.SetToolTip(this.tabVariables, "Bind plot properties to canvas variables");
            toolTip1.SetToolTip(this.checkMouseContext, "Bind mouse position to canvas variables");
            toolTip1.SetToolTip(this.checkXContext, "Bind x limits to canvas variables");
            toolTip1.SetToolTip(this.checkYContext, "Bind y limits to canvas variables");
            toolTip1.SetToolTip(this.checkZContext, "Bind z limits to canvas variables");
            toolTip1.SetToolTip(this.checkOrbitContext, "Bind view angles to canvas variables");
            toolTip1.SetToolTip(this.textAzimuthRedirecting, "Variable name for azimut angle");
            toolTip1.SetToolTip(this.textZenithRedirecting, "Variable name for zenit angle");
            toolTip1.SetToolTip(this.textVarNameMw, "Variable name for mouse wheel (sum of increments");
            toolTip1.SetToolTip(this.textVarNameMx, "Variable name for x relative to the image (0...1)");
            toolTip1.SetToolTip(this.textVarNameMy, "Variable name for y relative to the image (0...1)");
            toolTip1.SetToolTip(this.textVarNameXmin, "Variable name for lower x limit");
            toolTip1.SetToolTip(this.textVarNameXmax, "Variable name for upper x limit");
            toolTip1.SetToolTip(this.textVarNameYmin, "Variable name for lower y limit");
            toolTip1.SetToolTip(this.textVarNameYmax, "Variable name for upper y limit");
            toolTip1.SetToolTip(this.textVarNameZmin, "Variable name for lower z limit");
            toolTip1.SetToolTip(this.textVarNameZmax, "Variable name for upper z limit");
            toolTip1.SetToolTip(this.button_Refresh, "Apply changes to plot. You might need to re-calculate (F9)");
            toolTip1.SetToolTip(this.button_Cancel_all, "Cancel all changes made in the settings dialog");
            toolTip1.SetToolTip(this.button_ShowCommandlist, "Toggle display of generated Gnuplot and Maxima Draw commands");


 

        }
        public void SetLocation()
        {
            var screen = Screen.FromPoint(this.Location);
            if (this.Location.X + this.Width > screen.WorkingArea.Right || firstLocationSet)
                this.Location = new Point(screen.WorkingArea.Right - this.Width, ((screen.WorkingArea.Bottom - screen.WorkingArea.Top) / 2) - (this.Height / 2));
            firstLocationSet = false;
        }
        private void PlotSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (abort)
                region.GetCanvas().plotStore = region.GetCanvas().oldPlotStore;
            else
                RefreshStore();
            region.formOpen = false;
            region.GetCanvas().plotApproval = true;
            region.Invalidate();
        }
        #endregion

        #region DATA HANDLE
        /// <summary>
        /// Adjust displayed region to the contents of PlotStore
        /// </summary>
        public void Restore()
        {
            #region PICTURE
            if (plotStore.titleState == PlotImage.PlotStore.State.Custom)
            {
                //radioTitleCustom.Checked = true;
                textTitle.Text = plotStore.title;
                textTitle.ReadOnly = false;
            }
            else
            {
                //radioTitleDisable.Checked = true;
                textTitle.Text = "";
                textTitle.ReadOnly = false;
            }



            #endregion

            #region GENERAL DIAGRAM STYLE
            //BORDER
            textBorderVal.Text = Convert.ToString(plotStore.borderVal);
            if (plotStore.border == PlotImage.PlotStore.State.Enable)
            {
                checkBorder.Checked = true;
                textBorderVal.ReadOnly = false;
            }
            else
            {
                checkBorder.Checked = false;
                textBorderVal.ReadOnly = true;
            }
            //TEXT
            textTextSize.Text = plotStore.textSize.ToString();
            textTextSize.ReadOnly = false;
            //textTextFont.Text = plotStore.textFont;
            if (plotStore.textSizeState == PlotImage.PlotStore.State.Custom)
            {
                checkTextSizeCustom.Checked = true;
                textTextSize.ReadOnly = false;
                //   textTextFont.ReadOnly = false;
            }
            else
            {
                textTextSize.ReadOnly = true;
                // textTextFont.ReadOnly = true;

            }
            //PM3D
            textPm3dPalette.Text = plotStore.pm3dpalette;
            if (plotStore.pm3d == PlotStore.State.Enable)
            {
                checkPm3d.Checked = true;
                textPm3dPalette.ReadOnly = false;
            }
            else
            {
                textPm3dPalette.ReadOnly = true;
            }
            //CONTOUR
            textContourLevels.Text = plotStore.contourLevels.ToString();
            comboBoxContour.SelectedIndex = 0;
            if (plotStore.contour == PlotStore.State.Enable)
            {
                checkContour.Checked = true;
                textContourLevels.ReadOnly = false;
                comboBoxContour.Enabled = true;
            }
            else
            {
                textContourLevels.ReadOnly = true;
                comboBoxContour.Enabled = false;
            }
            //GRID
            textGridXu.Text = plotStore.gridXu.ToString();
            textGridYv.Text = plotStore.gridYv.ToString();
            if (plotStore.gridState == PlotImage.PlotStore.State.Custom)
            {
                checkGrid.Checked = true;
                textGridXu.ReadOnly = false;
                textGridYv.ReadOnly = false;
            }
            else
            {
                textGridXu.ReadOnly = true;
                textGridYv.ReadOnly = true;
            }
            #endregion

            #region VIEW
            if (plotStore.view == PlotImage.PlotStore.State.Custom)
            {
                radioViewCustom.Checked = true;
                textZenit.Text = Convert.ToString(plotStore.zenith);
                textZenit.ReadOnly = false;
                textAzimut.Text = Convert.ToString(plotStore.azimuth);
                textAzimut.ReadOnly = false;
                textScalE.Text = Convert.ToString(plotStore.scalZenith);
                textScalE.ReadOnly = false;
                textScalA.Text = Convert.ToString(plotStore.scalAzimuth);
                textScalA.ReadOnly = false;
            }
            else if (plotStore.view == PlotImage.PlotStore.State.Interactive)
            {
                radioViewInteractiv.Checked = true;
                textZenit.Text = Convert.ToString(plotStore.zenith);
                textZenit.ReadOnly = false;
                textAzimut.Text = Convert.ToString(plotStore.azimuth);
                textAzimut.ReadOnly = false;
                textScalE.Text = Convert.ToString(plotStore.scalZenith);
                textScalE.ReadOnly = false;
                textScalA.Text = Convert.ToString(plotStore.scalAzimuth);
                textScalA.ReadOnly = false;
            }
            else if (plotStore.view == PlotImage.PlotStore.State.MapView)
            {
                radioViewXY.Checked = true;
                textZenit.Text = "";
                textZenit.ReadOnly = true;
                textAzimut.Text = "";
                textAzimut.ReadOnly = true;
                textScalE.Text = "";
                textScalE.ReadOnly = true;
                textScalA.Text = "";
                textScalA.ReadOnly = true;
            } else
            {
                radioViewDisable.Checked = true;
                textZenit.Text = "";
                textZenit.ReadOnly = true;
                textAzimut.Text = "";
                textAzimut.ReadOnly = true;
                textScalE.Text = "";
                textScalE.ReadOnly = true;
                textScalA.Text = "";
                textScalA.ReadOnly = true;
            }
            #endregion

            #region PICTURE
            if (plotStore.pictureSizeState == PlotImage.PlotStore.State.Custom)
            {
                radioSizeCustom.Checked = true;
                textWidth.ReadOnly = false;
                textHeight.ReadOnly = false;

            }
            else if (plotStore.pictureSizeState == PlotImage.PlotStore.State.Interactive)
            {
                radioSizeAuto.Checked = true;
                textWidth.ReadOnly = true;
                textHeight.ReadOnly = true;
            }
            textWidth.Text = Convert.ToString(plotStore.width);
            textHeight.Text = Convert.ToString(plotStore.height);

            if (plotStore.termType == MaximaPlugin.PlotImage.PlotStore.TermType.svg)
            {
                comboTerm.SelectedIndex = 0;
            }
            else if (plotStore.termType == MaximaPlugin.PlotImage.PlotStore.TermType.png)
            {
                comboTerm.SelectedIndex = 1;
            }
            #endregion

            #region AXIS NAMES
            textXtitle.Text = plotStore.xName;
            textXtitle.ReadOnly = false;
            //if (plotStore.xNameS == PlotImage.PlotStore.State.Enable)
            //{
            //    radioXtitleCustom.Checked = true;
            //    textXtitle.Text = plotStore.xName;
            //    textXtitle.ReadOnly = false;
            //}
            //else if (plotStore.xNameS == PlotImage.PlotStore.State.Default)
            //{
            //    radioXtitleDefault.Checked = true;
            //    textXtitle.Text = SharedFunctions.defaultPlotValues.xName;
            //    textXtitle.ReadOnly = true;
            //}
            //else
            //{
            //    radioXtitleDisable.Checked = true;
            //    textXtitle.Text = "";
            //    textXtitle.ReadOnly = true;
            //}
            textYtitle.Text = plotStore.yName;
            textYtitle.ReadOnly = false;
            //if (plotStore.yNameS == PlotImage.PlotStore.State.Enable)
            //{
            //    radioYtitleCustom.Checked = true;
            //    textYtitle.Text = plotStore.yName;
            //    textYtitle.ReadOnly = false;
            //}
            //else if (plotStore.yNameS == PlotImage.PlotStore.State.Default)
            //{
            //    radioYtitleDefault.Checked = true;
            //    textYtitle.Text = SharedFunctions.defaultPlotValues.yName;
            //    textYtitle.ReadOnly = true;

            //}
            //else
            //{
            //    radioYtitleDisable.Checked = true;
            //    textYtitle.Text = "";
            //    textYtitle.ReadOnly = true;
            //}
            textZtitle.Text = plotStore.zName;
            textZtitle.ReadOnly = false;
            //if (plotStore.zNameS == PlotImage.PlotStore.State.Enable)
            //{
            //    radioZtitleCustom.Checked = true;
            //    textZtitle.Text = plotStore.zName;
            //    textZtitle.ReadOnly = false;
            //}
            //else if (plotStore.zNameS == PlotImage.PlotStore.State.Default)
            //{
            //    radioZtitleDefault.Checked = true;
            //    textZtitle.Text = SharedFunctions.defaultPlotValues.zName;
            //    textZtitle.ReadOnly = true;
            //}
            //else
            //{
            //    radioZtitleDisable.Checked = true;
            //    textZtitle.Text = "";
            //    textZtitle.ReadOnly = true;
            //}
            #endregion

            #region AXIS RANGE
            if (plotStore.xRangeS == PlotImage.PlotStore.State.Custom)
            {
                radioXrangeCustom.Checked = true;
                textXrangeMin.Text = Convert.ToString(plotStore.xMinRange);
                textXrangeMax.Text = Convert.ToString(plotStore.xMaxRange);
                textXrangeMin.ReadOnly = false;
                textXrangeMax.ReadOnly = false;

            }
            else if (plotStore.xRangeS == PlotImage.PlotStore.State.Interactive)
            {
                radioXrangeInteractive.Checked = true;
                textXrangeMin.Text = Convert.ToString(plotStore.xMinRange);
                textXrangeMax.Text = Convert.ToString(plotStore.xMaxRange);
                textXrangeMin.ReadOnly = false;
                textXrangeMax.ReadOnly = false;
            }
            else
            {
                radioXrangeDisable.Checked = true;
                textXrangeMin.Text = "";
                textXrangeMax.Text = ""; 
                textXrangeMin.ReadOnly = true;
                textXrangeMax.ReadOnly = true;
            }

            if (plotStore.yRangeS == PlotImage.PlotStore.State.Custom)
            {
                radioYrangeCustom.Checked = true;
                textYrangeMin.Text = Convert.ToString(plotStore.yMinRange);
                textYrangeMax.Text = Convert.ToString(plotStore.yMaxRange);
                textYrangeMin.ReadOnly = false;
                textYrangeMax.ReadOnly = false;
            }
            else if (plotStore.yRangeS == PlotImage.PlotStore.State.Interactive)
            {
                radioYrangeInteractive.Checked = true;
                textYrangeMin.Text = Convert.ToString(plotStore.yMinRange);
                textYrangeMax.Text = Convert.ToString(plotStore.yMaxRange);
                textYrangeMin.ReadOnly = false;
                textYrangeMax.ReadOnly = false;
            }
            else
            {
                radioYrangeDisable.Checked = true;
                textYrangeMin.Text = "";
                textYrangeMax.Text = "";
                textYrangeMin.ReadOnly = true;
                textYrangeMax.ReadOnly = true;
            }

            if (plotStore.zRangeS == PlotImage.PlotStore.State.Custom)
            {
                radioZrangeCustom.Checked = true;
                textZrangeMin.Text = Convert.ToString(plotStore.zMinRange);
                textZrangeMax.Text = Convert.ToString(plotStore.zMaxRange);
                textZrangeMin.ReadOnly = false;
                textZrangeMax.ReadOnly = false;
            }
            else if (plotStore.zRangeS == PlotImage.PlotStore.State.Interactive)
            {
                radioZrangeInteractive.Checked = true;
                textZrangeMin.Text = Convert.ToString(plotStore.zMinRange);
                textZrangeMax.Text = Convert.ToString(plotStore.zMaxRange);
                textZrangeMin.ReadOnly = false;
                textZrangeMax.ReadOnly = false;
            }
            else
            {
                radioZrangeDisable.Checked = true;
                textZrangeMin.Text = "";
                textZrangeMax.Text = "";
                textZrangeMin.ReadOnly = true;
                textZrangeMax.ReadOnly = true;
            }
            #endregion

            #region GRID
            if (plotStore.xGrid == PlotImage.PlotStore.State.Enable)
                checkXgrid.Checked = true;
            else
                checkXgrid.Checked = false;

            if (plotStore.yGrid == PlotImage.PlotStore.State.Enable)
                checkYgrid.Checked = true;
            else
                checkYgrid.Checked = false;

            if (plotStore.zGrid == PlotImage.PlotStore.State.Enable)
                checkZgrid.Checked = true;
            else
                checkZgrid.Checked = false;
            #endregion

            #region LOG
            textXbase.Text = Convert.ToString(plotStore.xLogBase);
            textYbase.Text = Convert.ToString(plotStore.yLogBase);
            textZbase.Text = Convert.ToString(plotStore.zLogBase);
            if (plotStore.xLogarithmic == PlotImage.PlotStore.State.Enable)
                checkXlog.Checked = true;
            else
                checkXlog.Checked = false;

            if (plotStore.yLogarithmic == PlotImage.PlotStore.State.Enable)
                checkYlog.Checked = true;
            else
                checkYlog.Checked = false;

            if (plotStore.zLogarithmic == PlotImage.PlotStore.State.Enable)
                checkZlog.Checked = true;
            else
                checkZlog.Checked = false;
            #endregion

            listBox1.Items.Clear();
            foreach (string str in plotStore.commandList)
                listBox1.Items.Add(str);

            listBox2.Items.Clear();
            foreach (string str in plotStore.prambleList)
                listBox2.Items.Add(str);

            #region REDIRECTING
            //MOUSE
            if (plotStore.mouseRedirecting == PlotImage.PlotStore.State.Enable)
            {
                checkMouseContext.Checked = true;
                textVarNameMx.Text = plotStore.varNameMouseX;
                textVarNameMx.ReadOnly = false;
                textVarNameMy.Text = plotStore.varNameMouseY;
                textVarNameMy.ReadOnly = false;
                textVarNameMw.Text = plotStore.varNameMouseWheel;
                textVarNameMw.ReadOnly = false;
            }
            else
            {
                checkMouseContext.Checked = false;
                textVarNameMx.Text = plotStore.varNameMouseX;
                textVarNameMx.ReadOnly = true;
                textVarNameMy.Text = plotStore.varNameMouseY;
                textVarNameMy.ReadOnly = true;
                textVarNameMw.Text = plotStore.varNameMouseWheel;
                textVarNameMw.ReadOnly = true;
            }
            //VIEW
            if (plotStore.viewRedirecting == PlotImage.PlotStore.State.Enable)
            {
                checkOrbitContext.Checked = true;
                textZenithRedirecting.Text = plotStore.varNameZenith;
                textZenithRedirecting.ReadOnly = false;
                textAzimuthRedirecting.Text = plotStore.varNameAzimuth;
                textAzimuthRedirecting.ReadOnly = false;
            }
            else
            {
                checkOrbitContext.Checked = false;
                textZenithRedirecting.Text = plotStore.varNameZenith;
                textZenithRedirecting.ReadOnly = true;
                textAzimuthRedirecting.Text = plotStore.varNameAzimuth;
                textAzimuthRedirecting.ReadOnly = true;
            }
            //AXIS
            textVarNameXmin.Text = plotStore.varNameXmin;
            textVarNameXmax.Text = plotStore.varNameXmax;
            if (plotStore.xRedirecting == PlotImage.PlotStore.State.Enable)
            {
                checkXContext.Checked = true;
                textVarNameXmin.ReadOnly = false;
                textVarNameXmax.ReadOnly = false;
            }
            else
            {
                checkXContext.Checked = false;
                textVarNameXmin.ReadOnly = true;
                textVarNameXmax.ReadOnly = true;
            }
            textVarNameYmin.Text = plotStore.varNameYmin;
            textVarNameYmax.Text = plotStore.varNameYmax;
            if (plotStore.yRedirecting == PlotImage.PlotStore.State.Enable)
            {
                checkYContext.Checked = true;
                textVarNameYmin.ReadOnly = false;
                textVarNameYmax.ReadOnly = false;
            }
            else
            {
                checkYContext.Checked = false;
                textVarNameYmin.ReadOnly = true;
                textVarNameYmax.ReadOnly = true;
            }
            textVarNameZmin.Text = plotStore.varNameZmin;
            textVarNameZmax.Text = plotStore.varNameZmax;
            if (plotStore.zRedirecting == PlotImage.PlotStore.State.Enable)
            {
                checkZContext.Checked = true;
                textVarNameZmin.ReadOnly = false;
                textVarNameZmax.ReadOnly = false;
            }
            else
            {
                checkZContext.Checked = false;
                textVarNameZmin.ReadOnly = true;
                textVarNameZmax.ReadOnly = true;
            }
            #endregion
        }
        public void Store()
        {
            
            #region PICTURE
            plotStore.titleState = PlotImage.PlotStore.State.Custom;
            plotStore.title = textTitle.Text;
            //if (radioTitleCustom.Checked == true)
            //{
            //    plotStore.titleState = PlotImage.PlotStore.State.Custom;
            //    if (!textTitle.ReadOnly) plotStore.title = textTitle.Text;
            //}
            //else 
            //{
            //    plotStore.titleState = PlotImage.PlotStore.State.Disable;
            //}

            #endregion 

            #region GENERAL DIAGRAM STYLE
            //TEXT
            if (checkTextSizeCustom.Checked)
            {
                plotStore.textSizeState = PlotImage.PlotStore.State.Custom;
                //if (!textTextFont.ReadOnly) plotStore.textFont = textTextFont.Text;
                //if (!getInteger(textTextSize, ref number)) return;
                //plotStore.textSize = number;
                try
                {
                    if (!textTextSize.ReadOnly) plotStore.textSize = Convert.ToInt32(textTextSize.Text);
                    //plotStore.textSize = Convert.ToInt32(textTextSize.Text);
                    //textTextSize.ForeColor = Color.Black;

                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                    //textTextSize.ForeColor = Color.Red;
                }
            }
            else
            {
                plotStore.textSizeState = PlotImage.PlotStore.State.Disable;
            }

            //PM3D
            if (checkPm3d.Checked)
            {
                plotStore.pm3d = PlotStore.State.Enable;
                if (!textPm3dPalette.ReadOnly) plotStore.pm3dpalette = textPm3dPalette.Text;
            }
            else
            {
                plotStore.pm3d = PlotStore.State.Disable;
            }


            //CONTOUR
            if (checkContour.Checked)
            {
                plotStore.contour = PlotStore.State.Enable;
                plotStore.contourType = comboBoxContour.Text;
                try
                {

                    if (!textContourLevels.ReadOnly) plotStore.contourLevels = Convert.ToInt32(textContourLevels.Text);

                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else
            {
                plotStore.contour = PlotStore.State.Disable;
            }



            if (checkGrid.Checked)
            {
                plotStore.gridState = PlotImage.PlotStore.State.Custom;
                try
                {
                    if (!textGridXu.ReadOnly) plotStore.gridXu = Convert.ToInt32(textGridXu.Text);
                    if (!textGridYv.ReadOnly) plotStore.gridYv = Convert.ToInt32(textGridYv.Text);

                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else
            {
                plotStore.gridState = PlotImage.PlotStore.State.Disable;
            }


            if (checkBorder.Checked)
            {
                plotStore.border = PlotImage.PlotStore.State.Enable;
                try
                {
                    if (!textBorderVal.ReadOnly) plotStore.borderVal = Convert.ToInt32(textBorderVal.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else
            {
                plotStore.border = PlotImage.PlotStore.State.Disable;
            }
            #endregion

            #region VIEW
            if (radioViewCustom.Checked == true)
            {
                try
                {
                    plotStore.view = PlotImage.PlotStore.State.Custom;
                    if (!textZenit.ReadOnly) plotStore.zenith = Convert.ToDouble(textZenit.Text);
                    if (!textAzimut.ReadOnly) plotStore.azimuth = Convert.ToDouble(textAzimut.Text);
                    if (!textScalE.ReadOnly) plotStore.scalZenith = Convert.ToDouble(textScalE.Text);
                    if (!textScalA.ReadOnly) plotStore.scalAzimuth = Convert.ToDouble(textScalA.Text);
                }
                catch 
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else if (radioViewInteractiv.Checked == true)
            {
                try
                {
                    plotStore.view = PlotImage.PlotStore.State.Interactive;
                    if (!textZenit.ReadOnly) plotStore.zenith = Convert.ToDouble(textZenit.Text);
                    if (!textAzimut.ReadOnly) plotStore.azimuth = Convert.ToDouble(textAzimut.Text);
                    if (!textScalE.ReadOnly) plotStore.scalZenith = Convert.ToDouble(textScalE.Text);
                    if (!textScalA.ReadOnly) plotStore.scalAzimuth = Convert.ToDouble(textScalA.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else if (radioViewDisable.Checked == true)
            {
                plotStore.view = PlotImage.PlotStore.State.Disable;
            }
            else if(radioViewXY.Checked == true)
            {
                plotStore.view = PlotImage.PlotStore.State.MapView;
            }
            #endregion

            #region PICTURE
            if (radioSizeAuto.Checked)
            {
                plotStore.pictureSizeState = PlotImage.PlotStore.State.Interactive;
            }
            else if (radioSizeCustom.Checked)
            {
                plotStore.pictureSizeState = PlotImage.PlotStore.State.Custom;
                try
                {
                    if (!textWidth.ReadOnly) plotStore.width = Convert.ToInt32(textWidth.Text);
                    if (!textHeight.ReadOnly) plotStore.height = Convert.ToInt32(textHeight.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }

            if (comboTerm.SelectedIndex == 0)
                plotStore.termType = MaximaPlugin.PlotImage.PlotStore.TermType.svg;
            else if (comboTerm.SelectedIndex == 1)
                plotStore.termType = MaximaPlugin.PlotImage.PlotStore.TermType.png;
            #endregion

            #region AXIS NAMES
            plotStore.xNameS = PlotImage.PlotStore.State.Enable;
            plotStore.xName = textXtitle.Text;

            //if (radioXtitleCustom.Checked == true)
            //{
            //    plotStore.xNameS = PlotImage.PlotStore.State.Enable;
            //    if (!textXtitle.ReadOnly) plotStore.xName = textXtitle.Text;
            //}
            //else if (radioXtitleDefault.Checked == true)
            //    plotStore.xNameS = PlotImage.PlotStore.State.Default;
            //else
            //    plotStore.xNameS = PlotImage.PlotStore.State.Disable;

            plotStore.yNameS = PlotImage.PlotStore.State.Enable;
            plotStore.yName = textYtitle.Text;
            //if (radioYtitleCustom.Checked == true)
            //{
            //    plotStore.yNameS = PlotImage.PlotStore.State.Enable;
            //    if (!textYtitle.ReadOnly) plotStore.yName = textYtitle.Text;
            //}
            //else if (radioYtitleDefault.Checked == true)
            //    plotStore.yNameS = PlotImage.PlotStore.State.Default;
            //else
            //    plotStore.yNameS = PlotImage.PlotStore.State.Disable;

            plotStore.zNameS = PlotImage.PlotStore.State.Enable;
            plotStore.zName = textZtitle.Text;
            //if (radioZtitleCustom.Checked == true)
            //{
            //    plotStore.zNameS = PlotImage.PlotStore.State.Enable;
            //    if (!textZtitle.ReadOnly) plotStore.zName = textZtitle.Text;
            //}
            //else if (radioZtitleDefault.Checked == true)
            //    plotStore.zNameS = PlotImage.PlotStore.State.Default;
            //else
            //    plotStore.zNameS = PlotImage.PlotStore.State.Disable;
            #endregion

            #region AXIS RANGES
            if (  radioXrangeCustom.Checked == true)
            {
                plotStore.xRangeS = PlotImage.PlotStore.State.Custom;
                try
                {
                    if (!textXrangeMin.ReadOnly) plotStore.xMinRange = Convert.ToDouble(textXrangeMin.Text);
                    if (!textXrangeMax.ReadOnly) plotStore.xMaxRange = Convert.ToDouble(textXrangeMax.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else if (radioXrangeInteractive.Checked == true)
            {
                plotStore.xRangeS = PlotImage.PlotStore.State.Interactive;
                try
                {
                    if (!textXrangeMin.ReadOnly) plotStore.xMinRange = Convert.ToDouble(textXrangeMin.Text);
                    if (!textXrangeMax.ReadOnly) plotStore.xMaxRange = Convert.ToDouble(textXrangeMax.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else
                plotStore.xRangeS = PlotImage.PlotStore.State.Disable;
            if (radioYrangeCustom.Checked == true)
            {
                plotStore.yRangeS = PlotImage.PlotStore.State.Custom;
                try
                {
                    if (!textYrangeMin.ReadOnly) plotStore.yMinRange = Convert.ToDouble(textYrangeMin.Text);
                    if (!textYrangeMax.ReadOnly) plotStore.yMaxRange = Convert.ToDouble(textYrangeMax.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else if (radioYrangeInteractive.Checked == true)
            {
                plotStore.yRangeS = PlotImage.PlotStore.State.Interactive;
                try
                {
                    if (!textYrangeMin.ReadOnly) plotStore.yMinRange = Convert.ToDouble(textYrangeMin.Text);
                    if (!textYrangeMax.ReadOnly) plotStore.yMaxRange = Convert.ToDouble(textYrangeMax.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else
                plotStore.yRangeS = PlotImage.PlotStore.State.Disable;

            if (radioZrangeCustom.Checked == true)
            {
                plotStore.zRangeS = PlotImage.PlotStore.State.Custom;
                try 
                {
                    if (!textZrangeMin.ReadOnly) plotStore.zMinRange = Convert.ToDouble(textZrangeMin.Text);
                    if (!textZrangeMax.ReadOnly) plotStore.zMaxRange = Convert.ToDouble(textZrangeMax.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else if (radioZrangeInteractive.Checked == true)
            {
                plotStore.zRangeS = PlotImage.PlotStore.State.Interactive;
                try
                {
                    if (!textZrangeMin.ReadOnly) plotStore.zMinRange = Convert.ToDouble(textZrangeMin.Text);
                    if (!textZrangeMax.ReadOnly) plotStore.zMaxRange = Convert.ToDouble(textZrangeMax.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter numbers.");
                }
            }
            else
                plotStore.zRangeS = PlotImage.PlotStore.State.Disable;
            #endregion

            #region GRID
            if (checkXgrid.Checked)
                plotStore.xGrid = PlotImage.PlotStore.State.Enable;
            else
                plotStore.xGrid = PlotImage.PlotStore.State.Disable;
            if (checkYgrid.Checked)
                plotStore.yGrid = PlotImage.PlotStore.State.Enable;
            else
                plotStore.yGrid = PlotImage.PlotStore.State.Disable;
            if (checkZgrid.Checked)
                plotStore.zGrid = PlotImage.PlotStore.State.Enable;
            else
                plotStore.zGrid = PlotImage.PlotStore.State.Disable;
            #endregion

            #region LOG
            // if log axes are enabled, then check if the limits are positive.
            // if both limits are negative, then reset them to defaults.
            // if lower limit is negative, reset it to upper limit divided by default span
            // if upper limit is negative, reset it to lower limit times default span

            double span = PlotStore.logMaxDfault / PlotStore.logMinDefault;
            if (checkXlog.Checked)
            {
                plotStore.xLogarithmic = PlotImage.PlotStore.State.Enable;
                try { plotStore.xLogBase = Convert.ToDouble(textXbase.Text); }
                catch { plotStore.xLogBase = 10; }
                if (plotStore.xMaxRange <= 0.0 && plotStore.xMinRange <= 0.0)
                {
                    plotStore.xMinRange = PlotStore.logMinDefault;
                    plotStore.xMaxRange = PlotStore.logMaxDfault;
                }
                if (plotStore.xMinRange <= 0.0) plotStore.xMinRange = plotStore.xMaxRange / span;
                if (plotStore.xMaxRange <= 0.0) plotStore.xMaxRange = plotStore.xMinRange * span;
            }
            else
            {
                plotStore.xLogarithmic = PlotImage.PlotStore.State.Disable;
            }

            if (checkYlog.Checked)
            {
                plotStore.yLogarithmic = PlotImage.PlotStore.State.Enable;
                try { plotStore.yLogBase = Convert.ToDouble(textYbase.Text); }
                catch { plotStore.yLogBase = 10; }
                if (plotStore.yMaxRange <= 0.0 && plotStore.yMinRange <= 0.0)
                {
                    plotStore.yMinRange = PlotStore.logMinDefault;
                    plotStore.yMaxRange = PlotStore.logMaxDfault;
                }
                if (plotStore.yMinRange <= 0.0) plotStore.yMinRange = plotStore.yMaxRange / span;
                if (plotStore.yMaxRange <= 0.0) plotStore.yMaxRange = plotStore.yMinRange * span;
            }
            else
            {
                plotStore.yLogarithmic = PlotImage.PlotStore.State.Disable;

            }
            if (checkZlog.Checked)
            {
                plotStore.zLogarithmic = PlotImage.PlotStore.State.Enable;
                try { plotStore.zLogBase = Convert.ToDouble(textZbase.Text); }
                catch { plotStore.zLogBase = 10; }
                if (plotStore.zMaxRange <= 0.0 && plotStore.zMinRange <= 0.0)
                {
                    plotStore.zMinRange = PlotStore.logMinDefault;
                    plotStore.zMaxRange = PlotStore.logMaxDfault;
                }
                if (plotStore.zMinRange <= 0.0) plotStore.zMinRange = plotStore.zMaxRange / span;
                if (plotStore.zMaxRange <= 0.0) plotStore.zMaxRange = plotStore.zMinRange * span;

            }
            else
            {
                plotStore.zLogarithmic = PlotImage.PlotStore.State.Disable;
            }
            #endregion

            #region REDIRECTING
            // MOUSE
            if (checkMouseContext.Checked)
            {
                plotStore.mouseRedirecting = PlotImage.PlotStore.State.Enable;
                plotStore.varNameMouseX = textVarNameMx.Text;
                plotStore.varNameMouseY = textVarNameMy.Text;
                plotStore.varNameMouseWheel = textVarNameMw.Text;
            }
            else
            {
                plotStore.mouseRedirecting = PlotImage.PlotStore.State.Disable;
            }
            // VIEW
            if (checkOrbitContext.Checked)
            {
                plotStore.viewRedirecting = PlotImage.PlotStore.State.Enable;
                plotStore.varNameZenith = textZenithRedirecting.Text;
                plotStore.varNameAzimuth = textAzimuthRedirecting.Text;
            }
            else
            {
                plotStore.viewRedirecting = PlotImage.PlotStore.State.Disable;
            }
            // AXIS
            if (checkXContext.Checked)
            {
                plotStore.xRedirecting = PlotImage.PlotStore.State.Enable;
                plotStore.varNameXmin = textVarNameXmin.Text;
                plotStore.varNameXmax = textVarNameXmax.Text;
            }
            else
            {
                plotStore.xRedirecting = PlotImage.PlotStore.State.Disable;
            }
            if (checkYContext.Checked)
            {
                plotStore.yRedirecting = PlotImage.PlotStore.State.Enable;
                plotStore.varNameYmin = textVarNameYmin.Text;
                plotStore.varNameYmax = textVarNameYmax.Text;
            }
            else
            {
                plotStore.yRedirecting = PlotImage.PlotStore.State.Disable;
            }
            if (checkZContext.Checked)
            {
                plotStore.zRedirecting = PlotImage.PlotStore.State.Enable;
                plotStore.varNameZmin = textVarNameZmin.Text;
                plotStore.varNameZmax = textVarNameZmax.Text;
            }
            else
            {
                plotStore.zRedirecting = PlotImage.PlotStore.State.Disable;
            }
            #endregion
           
        }
        public void RefreshStore()
        {
            Store();
            plotStore.MakeLists();
            Restore();
            //if (valid)
            //{
                
            //}
            
        }
        #endregion

        #region INPUT EVENTS

        


        private void AnyChanges(object sender, EventArgs e)
        {
            if (initComplete)
            {
                RefreshStore();
            }
              
        }
        private void EnterKey(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                RefreshStore();
                region.GetCanvas().plotApproval = true;
                region.Invalidate();
            }
        }
        #endregion

        #region BUTTONS
        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshStore();
            region.formOpen = false;
            region.GetCanvas().plotApproval = true;
            region.Invalidate();
        }
        private void Abort_Click(object sender, EventArgs e)
        {
            abort = true;
            this.Close();
        }
        private void ShowCommandlist_Click(object sender, EventArgs e)
        {
            if (!tabControl2.Visible)
            {
                button_ShowCommandlist.Text = "Hide commands";
                this.Size = new System.Drawing.Size(700, 525);
                tabControl2.Visible = true;

            }
            else
            {
                this.Size = new System.Drawing.Size(310, 525);
                button_ShowCommandlist.Text = "Show commands";
                tabControl2.Visible = false;

            }
            SetLocation();
        }
        #endregion

        private void groupBox28_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox10_Enter(object sender, EventArgs e)
        {

        }
    }
}
