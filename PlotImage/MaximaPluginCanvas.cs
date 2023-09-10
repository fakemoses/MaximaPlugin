using System.Drawing;
using System.IO;

//using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Forms;
using SharpVectors.Renderers.Gdi;

using SMath.Manager;
using SMath.Controls;
using SMath.Drawing;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Reflection;
using ImageMagick;
using System.Runtime.InteropServices.ComTypes;
using Svg;

namespace MaximaPlugin.PlotImage
{
    public class MaximaPluginCanvas : RegionEvaluable
    {
        public PlotStore plotStore;
        public PlotStore oldPlotStore;
        //CONTROL VARS
        public static int counter = 0;
        public string imageFilePath = "";
        public PlotStore.State tempState = PlotStore.State.Default;
        //PLOT
        public string lastPlotRequest = "";
        public string lastInput = "";
        public bool plotApproval = false;
        public bool isError = false;
        public string errorText = "";
        //PICTURE
        public System.Drawing.Image imageEs;
        public System.Drawing.Image imageEo;
        public SvgPictureBoxWindow svgWindow;
        public GdiGraphicsRenderer svgRenderer;
        public bool mouseD = false;
        public bool borderOn = true;
        public bool redrawCanvas = false;
        string lastGivenEquation = "";
        bool firstTimeDraw = true;

        
        public int newWidth = 0;
        public int newHeight = 0;
        public int oldHeight = 0;
        public int oldWidth = 0;

        // image container method
        internal ThreadSafeImageContainer ImageContainer { get; set; }

        public MaximaPluginCanvas(SessionProfile sessionProfile)
            : base(sessionProfile)
        {
            Init();

            // image container method
            this.ImageContainer = new ThreadSafeImageContainer();

            base.Size = new Size(plotStore.width, plotStore.height);
            this.Invalidate();
        }
        public MaximaPluginCanvas(MaximaPluginCanvas region)
            : base(region)
        {
            this.Init();
            // image container method
            this.ImageContainer = new ThreadSafeImageContainer(region.ImageContainer);
            this.imageEo = region.imageEo;
            this.plotStore = region.plotStore;
            this.lastPlotRequest = region.lastPlotRequest;
            this.errorText = region.errorText;
        }
        ~MaximaPluginCanvas()
        {
            counter--;
        }
        public void Init()
        {
            counter++;
            plotStore = new PlotStore();
            //VARNAME EXTENSION 
            plotStore.varNameMouseX = plotStore.varNameMouseX + counter.ToString();
            plotStore.varNameMouseY = plotStore.varNameMouseY + counter.ToString();
            plotStore.varNameMouseWheel = plotStore.varNameMouseWheel + counter.ToString();
            plotStore.varNameZenith = plotStore.varNameZenith + counter.ToString();
            plotStore.varNameAzimuth = plotStore.varNameAzimuth + counter.ToString();
            plotStore.varNameXmin = plotStore.varNameXmin + counter.ToString();
            plotStore.varNameXmax = plotStore.varNameXmax + counter.ToString();
            plotStore.varNameYmin = plotStore.varNameYmin + counter.ToString();
            plotStore.varNameYmax = plotStore.varNameYmax + counter.ToString();
            plotStore.varNameZmin = plotStore.varNameZmin + counter.ToString();
            plotStore.varNameZmax = plotStore.varNameZmax + counter.ToString();
            //SVG
            svgRenderer = new GdiGraphicsRenderer();
            svgWindow = new SvgPictureBoxWindow(Size.Width, Size.Height, svgRenderer);
            svgWindow.CreateEmptySvgDocument();
            //PATHS
            Directory.CreateDirectory(ControlObjects.Translator.GetMaxima().gnuPlotImageFolder);

            string fileExt = "";

            if (plotStore.termType == PlotStore.TermType.pdf)
                fileExt = ".pdf";
            else if (plotStore.termType == PlotStore.TermType.svg)
                fileExt = ".svg";
            else
                fileExt = ".png";

            imageFilePath = System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName(), fileExt);
            imageFilePath = Path.Combine(ControlObjects.Translator.GetMaxima().gnuPlotImageFolder, imageFilePath);
            plotStore.filename = System.IO.Path.ChangeExtension(imageFilePath, null).Replace("\\", "/");
            //IMAGE
            imageEo = new Bitmap(plotStore.width, plotStore.height);
            imageEo.Save(imageFilePath);
            lastInput = "";
            //plotApproval = true;
        }
        public void ScalImg(System.Drawing.Image img)
        {

            double widthScale = 0, heightScale = 0;

            if (img.Width != 0)
                widthScale = (double)base.Size.Width / (double)img.Width;
            if (img.Height != 0)
                heightScale = (double)base.Size.Height / (double)img.Height;

            double scale = Math.Min(widthScale, heightScale);

            newWidth = (int)(img.Width * scale);
            newHeight = (int)(img.Height * scale);

            imageEs = new Bitmap(newWidth, newHeight);
            using (var g = System.Drawing.Graphics.FromImage(imageEs))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, new System.Drawing.Rectangle(0, 0, newWidth, newHeight));
            }
        }
        public void Plot()
        {
            string returnV = MaximaPlugin.PlotImage.Draw.RegionDraw(this);
            if (returnV == null)
            {
                isError = false;
            }
            else
            {
                isError = true;
                errorText = returnV;
            }

            //plotApproval = false;
            redrawCanvas = false;
            LoadImage();
        }
        public bool LoadImage()
        {
            System.Drawing.Image loadedImage = new Bitmap(Size.Width, Size.Height);

            string fileExt = "";
            if (plotStore.termType == PlotStore.TermType.pdf)
                fileExt = ".pdf";
            else if (plotStore.termType == PlotStore.TermType.svg)
                fileExt = ".svg";
            else
                fileExt = ".png";

            imageFilePath = Path.ChangeExtension(imageFilePath, fileExt);

            if (!File.Exists(imageFilePath)) return false;
            try
            {
                if (plotStore.termType == PlotStore.TermType.svg)
                {

                    //new renderer based on : https://smath.com:8443/svn/public/plugins/ImageEditRegion
                    // this renderer gives a sharper output image

                    var SVGdoc = Svg.SvgDocument.Open(Path.ChangeExtension(imageFilePath,"svg"));
                    if (SVGdoc.OwnerDocument.ViewBox.Width == 0 || SVGdoc.OwnerDocument.ViewBox.Height == 0)
                    {
                        SVGdoc.Height = 1;
                        SVGdoc.Width = 1;
                        SVGdoc.Height = SVGdoc.Height.ToPercentage();
                        SVGdoc.Width = SVGdoc.Width.ToPercentage();
                        SVGdoc.OwnerDocument.ViewBox = new SvgViewBox(SVGdoc.OwnerDocument.Bounds.Left, SVGdoc.OwnerDocument.Bounds.Top, Convert.ToInt32(SVGdoc.OwnerDocument.GetDimensions().Width), Convert.ToInt32(SVGdoc.OwnerDocument.GetDimensions().Height));
                        SVGdoc.Height = Convert.ToInt32(SVGdoc.OwnerDocument.GetDimensions().Height);
                        SVGdoc.Width = Convert.ToInt32(SVGdoc.OwnerDocument.GetDimensions().Width);
                    }
                    oldWidth = Convert.ToInt32(SVGdoc.Width);
                    oldHeight = Convert.ToInt32(SVGdoc.Height);

                    SVGdoc.Height = Convert.ToInt32(Size.Height * (300 / Math.Max(GlobalProfile.ContentDpi, GlobalProfile.ContentDpi)));
                    SVGdoc.Width = Convert.ToInt32(Size.Width * (300/ Math.Max(GlobalProfile.ContentDpi, GlobalProfile.ContentDpi)));

                    using (MemoryStream stream = new MemoryStream())
                    {
                        SVGdoc.Draw().Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        loadedImage = System.Drawing.Image.FromStream(stream);
                        imageEo = loadedImage;
                    }

                    return true;

                }
                else if (plotStore.termType == PlotStore.TermType.pdf)
                {
                    // pdf reader here
                    // idea of implementation based on https://smath.com:8443/!/#public/view/head/plugins/ImageEditRegion/ImageCanvas.cs
                    try
                    {
                        //read and convert directly from memory

                        //byte[] pdfBytes = System.IO.File.ReadAllBytes(imageFilePath);
                        //byte[] pngByte = Freeware.Pdf2Png.Convert(pdfBytes, 1);

                        //using (MemoryStream pngStream = new MemoryStream(pngByte))
                        //{
                        //    System.Drawing.Image PDFimg = System.Drawing.Image.FromStream(pngStream);
                        //    var PDFBitmap = new Bitmap(PDFimg);
                        //    loadedImage = PDFimg;
    
                        //}

                        //MagickNET thing
                        string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                        MagickNET.Initialize();
                        MagickNET.SetGhostscriptDirectory(assemblyPath);

                        var settings = new MagickReadSettings
                        {
                            Density = new Density(300, 300, DensityUnit.PixelsPerCentimeter),
                            Depth = 1,
                            Compression = CompressionMethod.NoCompression,
                        };
                        string ImginPng = Path.ChangeExtension(imageFilePath, "png");

                        using(var images = new MagickImageCollection())
                        {
                            images.Read(imageFilePath, settings);
                            images.Write(ImginPng);
                        }

                        //ouput image is in 32bit. The standard output from Gnuplot is 24 bit
                        // probably not even needed to convert to 24 bit png
                        using (var imgMagick = new MagickImage(ImginPng))
                        {
                            imgMagick.Format = MagickFormat.Png24;
                            imgMagick.HasAlpha = false;

                            imgMagick.Write(ImginPng);
                        }

                        using (FileStream stream = new FileStream(ImginPng, FileMode.Open, FileAccess.Read))
                        {
                            imageEo = System.Drawing.Image.FromStream(stream);
                        }

                        File.Delete(ImginPng);

                    }
                    catch
                    {

                    }

                    return true;
                }
                else
                {
                    using (FileStream stream = new FileStream(Path.ChangeExtension(imageFilePath, fileExt), FileMode.Open, FileAccess.Read))
                    {
                        imageEo = System.Drawing.Image.FromStream(stream);
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Set the SMath input
        /// </summary>
        public void SetLastRequest()
        {
            if (this.lastInput != "#" && this.lastInput != "")
            {
                if (plotStore.titleState != PlotStore.State.Custom) plotStore.titleState = PlotStore.State.Disable;
                if (firstTimeDraw == true)
                {
                    lastPlotRequest = this.lastInput;
                    lastGivenEquation = lastPlotRequest;
                    redrawCanvas = true;
                    firstTimeDraw = false;
                } else if (!string.Equals(lastGivenEquation, this.lastInput, StringComparison.OrdinalIgnoreCase) || this.CheckEvaluability())
                {
                    lastPlotRequest = this.lastInput;
                    lastGivenEquation = lastPlotRequest;
                    redrawCanvas = true;
                }
                else
                {
                    lastPlotRequest = lastGivenEquation;
                    redrawCanvas = false;
                }
            }
            else if (plotStore.plotType == PlotStore.PlotType.plot2D)
            {
                tempState = plotStore.titleState;
                if (plotStore.title != "")
                    plotStore.titleState = PlotStore.State.Custom;
                else
                    plotStore.titleState = PlotStore.State.Default;
                //plotStore.xRangeS = PlotStore.State.Disable;
                lastPlotRequest = "explicit(sin(x),x,-5,5)";
                redrawCanvas = true;
            }
            else if (plotStore.plotType == PlotStore.PlotType.plot3D)
            {
                tempState = plotStore.titleState;
                if (plotStore.title != "")
                    plotStore.titleState = PlotStore.State.Custom;
                else
                    plotStore.titleState = PlotStore.State.Default;
                //plotStore.xRangeS = PlotStore.State.Disable;
                //plotStore.yRangeS = PlotStore.State.Disable;
                lastPlotRequest = "explicit(2*sin(x)*cos(y),x,-5,5,y,-5,5)";
                redrawCanvas = true;
            }
        }
        public int GetInstanceNum()
        {
            return counter;
        }
        public override void OnPaint(PaintEventOptions e)
        {

            if (borderOn) this.Border = true;
            else this.Border = false;
            base.OnPaint(e);

            if (imageEs == null) return;
            if (!isError)
            {
                IBitmap bmp = SMath.Drawing.Graphics.Specifics.BitmapFromNativeImage(new Bitmap(imageEo));

                //using imagecontainer method as implemented by https://smath.com:8443/!/#public/view/head/plugins/ImageEditRegion/ImageCanvas.cs
                this.ImageContainer.SetImage(bmp);
                this.ImageContainer.DrawImage(e.Graphics, new System.Drawing.Rectangle((int)e.ClipRectangle.X, (int)e.ClipRectangle.Y, newWidth, newHeight), true);
                bmp.Dispose();
            }
            else
            {
                this.Border = true;
                e.Graphics.Clear(System.Drawing.Color.White);
                e.Graphics.DrawString("Maxima draw error:\n\r" + errorText, new FontInfo("Tahoma", 12, FontfaceStyle.Regular), ColorBrushes.Black, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, StringOptions.GenericDefault);
            }
        }
        public override RegionBase Clone()
        {
            return new MaximaPluginCanvas(this);
        }
        public override void OnMouseDown(MouseEventOptions e)
        {
            mouseD = true;
            base.OnMouseDown(e);
        }
        public override void OnEvaluation(SMath.Math.Store store)
        {
        }

    }
}
