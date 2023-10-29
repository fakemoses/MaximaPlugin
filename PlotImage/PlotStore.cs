using System;
using System.Collections.Generic;
using System.Globalization;

using SMath.Manager;

namespace MaximaPlugin.PlotImage
{
    /// <summary>
    /// Contain all the plot options, create preamble and command lists for draw region and draw functions
    /// </summary>
    [Serializable]
    public class PlotStore
    {
        public List<string> commandList;
        public List<string> prambleList;

        public const double logMinDefault = 0.001;
        public const double logMaxDfault = 10;

        /// <summary>
        /// Constructor. Set all plot options.
        /// </summary>
        public PlotStore()
        {
            #region TITLE AND LOOK
            plotType = PlotType.plot2D;
            title = "SAMPLE PLOT";
            titleDefault = "SAMPLE PLOT";
            titleState = State.Disable;

            mapView = State.Disable;
            #endregion

            #region GENERAL DIAGRAM STYLE
            //FORMAT
            border = State.Default;
            borderVal = 4095;
            textSizeState = State.Default;
            textSize = 8;
            textFont = "Arial";
            //3D
            pm3d = State.Enable;
            pm3dpalette = "color";

            //BACKGROUND COLOR
            bgColor = "#fefefe";

            //CONTOUR
            contour = State.Custom;
            contourLevels = 5;
            contourType = "";

            //AXIS
            enablexAxis = true;
            enableyAxis = true;
            enablezAxis = false;

            //SURFACE GRID
            gridState = State.Custom;
            gridXu = 40;
            gridYv = 40;
            #endregion

            #region PICTURE
            width = 300;
            height = 240;
            pictureSizeState = State.Interactive;
            termType = TermType.png;
            filename = System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName(), null);
            #endregion

            //propotional axes
            propAxes = State.Disable;

            #region AXIS NAMES
            xName = "x";
            yName = "y";
            zName = "z";
            #endregion

            #region AXIS TYPES
            xAxisType = "solid";
            yAxisType = "solid";
            #endregion

            #region AXIS CONFIG
            xAxisColor = "black";
            yAxisColor = "black";

            xaxisWidth = 1;
            yaxisWidth = 1;
            #endregion

            #region AXIS NAMES STATE
            xNameS = State.Default;
            yNameS = State.Default;
            zNameS = State.Default;
            #endregion

            #region AXIS RANGES
            xMinRange = -5;
            xMaxRange = 5;
            yMinRange = -5;
            yMaxRange = 5;
            zMinRange = -5;
            zMaxRange = 5;

            #endregion

            #region AXIS RANGE STATE
            xRangeS = State.Interactive;
            yRangeS = State.Interactive;
            zRangeS = State.Interactive;
            #endregion

            #region AXIS GRID STATE
            xGrid = State.Enable;
            yGrid = State.Enable;
            zGrid = State.Disable;
            #endregion

            #region AXIS LOG STATE
            xLogarithmic = State.Disable;
            yLogarithmic = State.Disable;
            zLogarithmic = State.Disable;
            #endregion

            #region LOG BASE
            xLogBase = 10;
            yLogBase = 10;
            zLogBase = 10;
            #endregion

            #region VIEW
            azimuth = 60;
            zenith = 60;
            scalAzimuth = 1.0;
            scalZenith = 1.0;
            view = State.Interactive;
            #endregion

            #region REDIRECTING
            //MOUSE
            mouseRedirecting = State.Disable;
            varNameMouseX = "MouseX";
            varNameMouseY = "MouseY";
            varNameMouseWheel = "MouseW";
            //VIEW
            viewRedirecting = State.Disable;
            varNameAzimuth = "Azimut";
            varNameZenith = "Zenit";
            //AXIS X
            xRedirecting = State.Disable;
            varNameXmin = "MinX";
            varNameXmax = "MaxX";
            //AXIS Y
            yRedirecting = State.Disable;
            varNameYmin = "MinY";
            varNameYmax = "MaxY";
            //AXIS Z
            zRedirecting = State.Disable;
            varNameZmin = "MinZ";
            varNameZmax = "MaxZ";
            #endregion

            #region FUTURE OPTION PLACEHOLDER
            option = "none;";
            #endregion

            commandList = new List<string>();
            prambleList = new List<string>();
        }

        /// <summary>
        /// Create list of commands and preamble by populating the List based on the plot options
        /// </summary>
        public void MakeLists()
        {
            commandList.Clear();
            prambleList.Clear();
            //float dpi = GlobalProfile.ContentDpi;
            #region PICTURE AND SETTINGS

            if (textSizeState == State.Disable)
                textSize = 8;

            double convertedW = Math.Round(width / GlobalProfile.ContentDpi, 2,MidpointRounding.ToEven);
            string convertedWidthforPDF = convertedW.ToString("0.##").Replace(",", ".");

            double convertedH = Math.Round(height / GlobalProfile.ContentDpi, 2, MidpointRounding.ToEven);
            string convertedHeightforPDF = convertedH.ToString("0.##").Replace(",", ".");

            string convertedWidth = width.ToString();
            string convertedHeight = height.ToString();

            if (termType == TermType.svg)
            {
                commandList.Add("terminal=svg");
                prambleList.Add("\"set term svg noenhanced size " + convertedWidth + ", " + convertedHeight + Symbols.StringChar);

            }
            else if (termType == TermType.png)
            {
                commandList.Add("terminal=pngcairo");
                prambleList.Add("\"set term pngcairo enhanced size " + convertedWidth + ", " + convertedHeight + Symbols.StringChar);

            }
            else
            {
                commandList.Add("terminal=pdfcairo");
                prambleList.Add("\"set term pdfcairo enhanced size " + convertedWidthforPDF + ", " + convertedHeightforPDF + Symbols.StringChar);
            }

            commandList.Add("file_name=\"" + filename + "\"");
            commandList.Add("font=\"" + textFont + "\"");
            if (termType == TermType.pdf && textSizeState != State.Custom)
            {
                textSize = 12; //on random points, 14 is too large
            }
            commandList.Add("font_size=" + textSize);

            prambleList.Add("\"set encoding utf8\"");
            if (titleState == State.Custom)
                commandList.Add("title=\"" + title + "\"");
            if (titleState == State.Default)
                commandList.Add("title=\"" + titleDefault + "\"");
            #endregion

            #region DIAGRAM STYLE
            //CONTOUR
            if (contour == State.Enable)
            {
                commandList.Add("contour_levels=" + Convert.ToString(contourLevels));
                commandList.Add("contour=" + contourType);
            }

            //BACKGROUND STYLE
            commandList.Add("background_color=\"" + bgColor + "\"");

            //AXIS SYTLES AND GRID for 2d
            if (plotType == PlotType.plot2D)
            {
                if (enablexAxis)
                {
                    commandList.Add("xaxis=true");
                    commandList.Add("xaxis_type=" + xAxisType);
                    commandList.Add("xaxis_color=" + xAxisColor);
                    commandList.Add("xaxis_width=" + xaxisWidth);
                } 
                if (enableyAxis)
                {
                    commandList.Add("yaxis=true");
                    commandList.Add("yaxis_type=" + yAxisType);
                    commandList.Add("yaxis_color=" + yAxisColor);
                    commandList.Add("yaxis_width=" + yaxisWidth);
                }
            }

            if(termType == TermType.svg || termType == TermType.pdf)
                prambleList.Add("\"set style line 100 lc rgb 'grey' lt -1 lw 0.1\"");
            else if(termType == TermType.png)
                prambleList.Add("\"set style line 100 lc rgb 'black' lt -1 lw 0.1\"");

            #region AXIS GRID

            if (xGrid == State.Enable || xGrid == State.Default)
                prambleList.Add("\"set grid xtics ls 100\"");
            if (yGrid == State.Enable || yGrid == State.Default)
                prambleList.Add("\"set grid ytics ls 100\"");
            if ((zGrid == State.Enable || zGrid == State.Default) && plotType == PlotType.plot3D)
                prambleList.Add("\"set grid ztics ls 100\"");
            #endregion

            //PM3D
            if (pm3d == State.Enable || pm3d == State.Custom)
            {
                commandList.Add("enhanced3d = true");
                prambleList.Add("\"set pm3d lighting depthorder base\"");
                if(pm3d == State.Custom)
                    commandList.Add("palette =" + pm3dpalette.Replace(',', GlobalProfile.ArgumentsSeparatorStandard));
            }

            //SURFACE GRID
            if (gridState == State.Custom)
            {
                commandList.Add("xu_grid=" + gridXu);
                commandList.Add("yv_grid=" + gridYv);
            }
            //BORDER
            if (border == State.Enable)
                prambleList.Add("\"set border " + borderVal.ToString() + "\"");
            if (view == State.MapView && plotType == PlotType.plot3D)
                prambleList.Add("\"set view map\"");
            #endregion

            #region AXIS NAMES
            //X
            if (xNameS == State.Enable)
                commandList.Add("xlabel=\"" + xName + "\"");
            else if (xNameS == State.Default)
                commandList.Add("xlabel=\"" + SharedFunctions.defaultPlotValues.xName + "\"");
            //Y
            if (yNameS == State.Enable)
                commandList.Add("ylabel=\"" + yName + "\"");
            else if (yNameS == State.Default)
                commandList.Add("ylabel=\"" + SharedFunctions.defaultPlotValues.yName + "\"");
            //Z
            if (zNameS == State.Enable)
                commandList.Add("zlabel=\"" + zName + "\"");
            else if (zNameS == State.Default)
                commandList.Add("zlabel=\"" + SharedFunctions.defaultPlotValues.zName + "\"");
            #endregion

            // propotional axes

            if(propAxes == State.Enable)
            {
                if (plotType == PlotType.plot2D)
                {
                    commandList.Add("proportional_axes=xy");
                }
                else
                {
                    commandList.Add("proportional_axes=xyz");
                }
            }

            #region AXIS RANGES
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };

            if (xRangeS == State.Custom || xRangeS == State.Interactive)
                commandList.Add("xrange=[" + xMinRange.ToString(nfi) + GlobalProfile.ArgumentsSeparatorStandard + xMaxRange.ToString(nfi) + "]");

            if (yRangeS == State.Custom || yRangeS == State.Interactive)
                commandList.Add("yrange=[" + yMinRange.ToString(nfi) + GlobalProfile.ArgumentsSeparatorStandard + yMaxRange.ToString(nfi) + "]");

            if (zRangeS == State.Custom || zRangeS == State.Interactive)
                commandList.Add("zrange=[" + zMinRange.ToString(nfi) + GlobalProfile.ArgumentsSeparatorStandard + zMaxRange.ToString(nfi) + "]");
            #endregion


            #region AXIS LOGARITHMIC
            if (xLogarithmic == State.Enable || xLogarithmic == State.Default)
                prambleList.Add("\"set logscale x " + Convert.ToString(xLogBase) + "\"");
            if (yLogarithmic == State.Enable || yLogarithmic == State.Default)
                prambleList.Add("\"set logscale y " + Convert.ToString(yLogBase) + "\"");
            if ((zLogarithmic == State.Enable || zLogarithmic == State.Default) && plotType == PlotType.plot3D)
                prambleList.Add("\"set logscale z " + Convert.ToString(zLogBase) + "\"");
            #endregion

            if ((view == State.Interactive || view == State.Default || view == State.Custom) && plotType == PlotType.plot3D)
                prambleList.Add("\"set view " + zenith.ToString(nfi) + ", " + azimuth.ToString(nfi) + ", " + scalZenith.ToString(nfi) + ", " + scalAzimuth.ToString(nfi) + Symbols.StringChar);
        }
        
        /// <summary>
        /// Cloning event handler
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        # region ENUMS
        public enum TermType
        {
            svg,
            png,
            pdf
        };

        public enum PlotType
        {
            plot2D,
            plot3D
        }
        public enum State
        {
            Enable,
            Default,
            Disable,
            Custom,
            Interactive,
            MapView
        }


        #endregion

        #region TITLE AND LOOK
        private PlotType _plotType;
        public PlotType plotType
        {
            set { _plotType = value; }
            get { return _plotType; }
        }
        private string _titleDefault;
        public string titleDefault
        {
            set { _titleDefault = value; }
            get { return _titleDefault; }
        }
        private string _title;
        public string title
        {
            set { _title = value; }
            get { return _title; }
        }
        private State _titleState;
        public State titleState
        {
            set { _titleState = value; }
            get { return _titleState; }
        }

        #endregion

        #region GENERAL DIAGRAM STYLE
        //BORDER
        private State _border;
        public State border
        {
            set { _border = value; }
            get { return _border; }
        }
        private int _borderVal;
        public int borderVal
        {
            set { _borderVal = value; }
            get { return _borderVal; }
        }
        //TEXT
        private int _textSize;
        public int textSize
        {
            set { _textSize = value; }
            get { return _textSize; }
        }
        private State _textSizeState;
        public State textSizeState
        {
            set { _textSizeState = value; }
            get { return _textSizeState; }
        }
        private string _textFont;
        public string textFont
        {
            set { _textFont = value; }
            get { return _textFont; }
        }
        //CONTOUR
        private State _contour;
        public State contour
        {
            set { _contour = value; }
            get { return _contour; }
        }
        private string _contourType;
        public string contourType
        {
            set { _contourType = value; }
            get { return _contourType; }
        }
        private int _contourLevels;
        public int contourLevels
        {
            set { _contourLevels = value; }
            get { return _contourLevels; }
        }

        //BACKGROUND COLOR
        private string _bgColor;
        public string bgColor
        {
            set { _bgColor = value; }
            get { return _bgColor; }
        }

        //Enhanced3D
        private State _enhanced3dState;
        public State enhanced3dState
        {
            set { _enhanced3dState = value; }
            get { return _enhanced3dState; }
        }

        //PM3D
        private State _pm3d;
        public State pm3d
        {
            set { _pm3d = value; }
            get { return _pm3d; }
        }
        private string _pm3dpalette;
        public string pm3dpalette
        {
            set { _pm3dpalette = value; }
            get { return _pm3dpalette; }
        }
        //SURFACE GRID    
        private int _gridXu;
        public int gridXu
        {
            set { _gridXu = value; }
            get { return _gridXu; }
        }
        private int _gridYv;
        public int gridYv
        {
            set { _gridYv = value; }
            get { return _gridYv; }
        }
        private State _gridState;
        public State gridState
        {
            set { _gridState = value; }
            get { return _gridState; }
        }
        #endregion

        #region PICTURE
        private int _widht;
        public int width
        {
            set { _widht = value; }
            get { return _widht; }
        }
        private int _height;
        public int height
        {
            set { _height = value; }
            get { return _height; }
        }
        private State _pictureSizeState;
        public State pictureSizeState
        {
            set { _pictureSizeState = value; }
            get { return _pictureSizeState; }
        }
        private string _filename;
        public string filename
        {
            set { _filename = value; }
            get { return _filename; }
        }
        private TermType _termType;
        public TermType termType
        {
            set { _termType = value; }
            get { return _termType; }
        }


        private State _mapView;
        public State mapView
        {
            set { _mapView = value; }
            get { return _mapView; }
        }



        #endregion

        #region AXIS NAMES
        private string _xName;
        public string xName
        {
            set { _xName = value; }
            get { return _xName; }
        }
        private string _yName;
        public string yName
        {
            set { _yName = value; }
            get { return _yName; }
        }
        private string _zName;
        public string zName
        {
            set { _zName = value; }
            get { return _zName; }
        }
        #endregion

        #region AXIS TYPES
        private string _xaxisType;
        public string xAxisType
        {
            set { _xaxisType = value; }
            get { return _xaxisType; }
        }

        private string _yaxisType;
        public string yAxisType
        {
            set { _yaxisType = value; }
            get { return _yaxisType; }
        }

        #endregion

        #region AXIS CONFIG

        private string _xaxisColor;
        public string xAxisColor
        {
            set { _xaxisColor = value; }
            get { return _xaxisColor; }
        }

        private string _yaxisColor;
        public string yAxisColor
        {
            set { _yaxisColor = value; }
            get { return _yaxisColor; }
        }

        private int _xaxisWidth;
        public int xaxisWidth
        {
            set { _xaxisWidth = value; }
            get { return _xaxisWidth; }
        }

        private int _yaxisWidth;
        public int yaxisWidth
        {
            set { _yaxisWidth = value; }
            get { return _yaxisWidth; }
        }

        #endregion

        #region AXIS NAMES STATE
        private State _xNameS;
        public State xNameS
        {
            set { _xNameS = value; }
            get { return _xNameS; }
        }
        private State _yNameS;
        public State yNameS
        {
            set { _yNameS = value; }
            get { return _yNameS; }
        }
        private State _zNameS;
        public State zNameS
        {
            set { _zNameS = value; }
            get { return _zNameS; }
        }
        #endregion

        #region AXIS RANGES
        private double _xMinRange;
        public double xMinRange
        {
            set { _xMinRange = value; }
            get { return _xMinRange; }
        }

        private double _xMaxRange;
        public double xMaxRange
        {
            set { _xMaxRange = value; }
            get { return _xMaxRange; }
        }

        private double _yMinRange;
        public double yMinRange
        {
            set { _yMinRange = value; }
            get { return _yMinRange; }
        }

        private double _yMaxRange;
        public double yMaxRange
        {
            set { _yMaxRange = value; }
            get { return _yMaxRange; }
        }

        private double _zMinRange;
        public double zMinRange
        {
            set { _zMinRange = value; }
            get { return _zMinRange; }
        }

        private double _zMaxRange;
        public double zMaxRange
        {
            set { _zMaxRange = value; }
            get { return _zMaxRange; }
        }
        #endregion

        #region AXIS RANGES STATE 
        private State _xRangeS;
        public State xRangeS
        {
            set { _xRangeS = value; }
            get { return _xRangeS; }
        }
        private State _yRangeS;
        public State yRangeS
        {
            set { _yRangeS = value; }
            get { return _yRangeS; }
        }
        private State _zRangeS;
        public State zRangeS
        {
            set { _zRangeS = value; }
            get { return _zRangeS; }
        }
        #endregion

        #region AXIS GRID STATE
        private State _xGrid;
        public State xGrid
        {
            set { _xGrid = value; }
            get { return _xGrid; }
        }
        private State _yGrid;
        public State yGrid
        {
            set { _yGrid = value; }
            get { return _yGrid; }
        }
        private State _zGrid;
        public State zGrid
        {
            set { _zGrid = value; }
            get { return _zGrid; }
        }
        #endregion

        #region AXIS LOGARITHMIC
        private State _xLogarithmic;
        public State xLogarithmic
        {
            set { _xLogarithmic = value; }
            get { return _xLogarithmic; }
        }
        private State _yLogarithmic;
        public State yLogarithmic
        {
            set { _yLogarithmic = value; }
            get { return _yLogarithmic; }
        }
        private State _zLogarithmic;
        public State zLogarithmic
        {
            set { _zLogarithmic = value; }
            get { return _zLogarithmic; }
        }
        #endregion

        #region LOG BASE

        private double _xLogBase;
        public double xLogBase
        {
            set { _xLogBase = value; }
            get { return _xLogBase; }
        }
        private double _yLogBase;
        public double yLogBase
        {
            set { _yLogBase = value; }
            get { return _yLogBase; }
        }
        private double _zLogBase;
        public double zLogBase
        {
            set { _zLogBase = value; }
            get { return _zLogBase; }
        }



        #endregion

        #region VIEW
        private double _azimuth;
        public double azimuth
        {
            set { _azimuth = value; }
            get { return _azimuth; }
        }
        private double _zenith;
        public double zenith
        {
            set { _zenith = value; }
            get { return _zenith; }
        }
        private double _scalAzimuth;
        public double scalAzimuth
        {
            set { _scalAzimuth = value; }
            get { return _scalAzimuth; }
        }
        private double _scalZenith;
        public double scalZenith
        {
            set { _scalZenith = value; }
            get { return _scalZenith; }
        }
        private State _view;
        public State view
        {
            set { _view = value; }
            get { return _view; }
        }
        #endregion

        #region REDIRECTING
        //MOUSE
        private State _mouseRedirecting;
        public State mouseRedirecting
        {
            set { _mouseRedirecting = value; }
            get { return _mouseRedirecting; }
        }
        private string _varNameMouseX;
        public string varNameMouseX
        {
            set { _varNameMouseX = value; }
            get { return _varNameMouseX; }
        }
        private string _varNameMouseY;
        public string varNameMouseY
        {
            set { _varNameMouseY = value; }
            get { return _varNameMouseY; }
        }
        private string _varNameMouseWheel;
        public string varNameMouseWheel
        {
            set { _varNameMouseWheel = value; }
            get { return _varNameMouseWheel; }
        }
        //VIEW
        private State _viewRedirecting;
        public State viewRedirecting
        {
            set { _viewRedirecting = value; }
            get { return _viewRedirecting; }
        }
        private string _varNameAzimuth;
        public string varNameAzimuth
        {
            set { _varNameAzimuth = value; }
            get { return _varNameAzimuth; }
        }
        private string _varNameZenith;
        public string varNameZenith
        {
            set { _varNameZenith = value; }
            get { return _varNameZenith; }
        }
        //AXIS X
        private State _xRedirecting;
        public State xRedirecting
        {
            set { _xRedirecting = value; }
            get { return _xRedirecting; }
        }
        private string _varNameXmin;
        public string varNameXmin
        {
            set { _varNameXmin = value; }
            get { return _varNameXmin; }
        }
        private string _varNameXmax;
        public string varNameXmax
        {
            set { _varNameXmax = value; }
            get { return _varNameXmax; }
        }
        //AXIS Y
        private State _yRedirecting;
        public State yRedirecting
        {
            set { _yRedirecting = value; }
            get { return _yRedirecting; }
        }
        private string _varNameYmin;
        public string varNameYmin
        {
            set { _varNameYmin = value; }
            get { return _varNameYmin; }
        }
        private string _varNameYmax;
        public string varNameYmax
        {
            set { _varNameYmax = value; }
            get { return _varNameYmax; }
        }
        //AXIS Z
        private State _zRedirecting;
        public State zRedirecting
        {
            set { _zRedirecting = value; }
            get { return _zRedirecting; }
        }
        private string _varNameZmin;
        public string varNameZmin
        {
            set { _varNameZmin = value; }
            get { return _varNameZmin; }
        }
        private string _varNameZmax;
        public string varNameZmax
        {
            set { _varNameZmax = value; }
            get { return _varNameZmax; }
        }
        #endregion

        #region FUTURE OPTION PLACEHOLDER
        private string _option;
        public string option
        {
            set { _option = value; }
            get { return _option; }
        }
        #endregion

        #region AXIS
        private bool _enablexAxis;
        private bool _enableyAxis;
        private bool _enablezAxis;

        public bool enablexAxis
        {
            set { _enablexAxis = value; }
            get { return _enablexAxis; }
        }

        public bool enableyAxis
        {
            set { _enableyAxis = value; }
            get { return _enableyAxis; }
        }

        public bool enablezAxis
        {
            set { _enablezAxis = value; }
            get { return _enablezAxis; }
        }

        // custom palette
        private State _customPalatte;
        public State customPalatte
        {
            set { _customPalatte = value; }
            get { return _customPalatte; }
        }

        // Propotional Axes
        private State _propAxes;
        public State propAxes
        {
            set { _propAxes = value; }
            get { return _propAxes; }
        }

        #endregion
    }
}