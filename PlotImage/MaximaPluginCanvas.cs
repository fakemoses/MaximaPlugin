using System.Drawing;
using System.IO;

using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Forms;
using SharpVectors.Renderers.Gdi;

using SMath.Manager;
using SMath.Controls;
using SMath.Drawing;
using System;

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

        public int oldHeight = 0;
        public int oldWidth = 0;

        private double dpiSCR = GlobalProfile.ContentDpi, dpiPrint = 300;

        public MaximaPluginCanvas(SessionProfile sessionProfile)
            : base(sessionProfile)
        {
            Init();
            base.Size = new Size(plotStore.width, plotStore.height);
            plotApproval = true;
            this.Invalidate();
        }
        public MaximaPluginCanvas(MaximaPluginCanvas region)
            : base(region)
        {
            this.Init();
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

            //problem lies here - stuffs are hardcoded
            string fileExt = "";

            if (plotStore.termType == PlotStore.TermType.pdf)
                fileExt = ".pdf";
            else
                fileExt = ".png";

            imageFilePath = System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName(), fileExt);
            imageFilePath = Path.Combine(ControlObjects.Translator.GetMaxima().gnuPlotImageFolder, imageFilePath);
            plotStore.filename = System.IO.Path.ChangeExtension(imageFilePath, null).Replace("\\", "/");
            //IMAGE
            imageEo = new Bitmap(plotStore.width, plotStore.height);
            imageEo.Save(imageFilePath);
            lastInput = "";
            SetLastRequest();
            plotApproval = true;
        }
        public void ScalImg(System.Drawing.Image img)
        {

            double widthScale = 0, heightScale = 0;

            if (img.Width != 0)
                widthScale = (double)base.Size.Width / (double)img.Width;
            if (img.Height != 0)
                heightScale = (double)base.Size.Height / (double)img.Height;

            double scale = Math.Min(widthScale, heightScale);

            int newWidth = (int)(img.Width * scale);
            int newHeight = (int)(img.Height * scale);

            imageEs = new Bitmap(newWidth, newHeight);
            using (var g = System.Drawing.Graphics.FromImage(imageEs))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, new System.Drawing.Rectangle(0, 0, newWidth, newHeight));
            }

        }
        public bool Plot()
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
            System.Threading.Thread.Sleep(10);
            plotApproval = false;
            return LoadImage();
        }
        public bool LoadImage()
        {
            System.Drawing.Image loadedImage = new Bitmap(Size.Width, Size.Height);

            string fileExt = "";
            if (plotStore.termType == PlotStore.TermType.pdf)
                fileExt = ".pdf";
            else
                fileExt = ".png";

            imageFilePath = Path.ChangeExtension(imageFilePath, fileExt);

            if (!File.Exists(imageFilePath)) return false;
            try
            {
                if (plotStore.termType == PlotStore.TermType.svg)
                {
                    svgWindow.Source = Path.ChangeExtension(imageFilePath, "svg");
                    svgWindow.Resize(Size.Width, Size.Height);
                    svgRenderer.Render(svgWindow.Document as SvgDocument);
                    imageEo = svgRenderer.RasterImage;
                    svgRenderer.ClearMap();
                    return true;

                }
                else if (plotStore.termType == PlotStore.TermType.pdf)
                {
                    // pdf reader here
                    // implementation based on https://smath.com:8443/!/#public/view/head/plugins/ImageEditRegion/ImageCanvas.cs
                    try
                    {
                        //using (PdfReader reader = new PdfReader(imageFilePath))
                        //{
                        //    oldWidth = Convert.ToInt32(reader.GetPageSize(1).Width / 72 * dpiSCR);
                        //    oldHeight = Convert.ToInt32(reader.GetPageSize(1).Height / 72 * dpiSCR);
                        //}

                        //GhostscriptSettings PDFsettings = new GhostscriptSettings();
                        //PDFsettings.Page.AllPages = false;
                        //PDFsettings.Page.Start = 1;
                        //PDFsettings.Page.End = 1;
                        //PDFsettings.Size.Manual = new Size(oldWidth, oldHeight);
                        //PDFsettings.Device = GhostscriptSharp.Settings.GhostscriptDevices.jpeg;
                        //int dpiXPDF = Convert.ToInt32(oldWidth / (Size.Width / dpiSCR));
                        //int dpiYPDF = Convert.ToInt32(oldHeight / (Size.Height / dpiSCR));
                        //double dpiPDFxFactor = Math.Max(1, dpiPrint / Math.Max(dpiXPDF, dpiYPDF));
                        //PDFsettings.Resolution = new Size(Convert.ToInt32(dpiSCR * dpiPDFxFactor), Convert.ToInt32(dpiSCR * dpiPDFxFactor));
                        //string ConvertedPDFtoPNG = System.IO.Path.ChangeExtension(imageFilePath, ".jpeg");
                        //GhostscriptWrapper.GenerateOutput(imageFilePath, ConvertedPDFtoPNG, PDFsettings);

                        string ConvertedPDFtoPNG = System.IO.Path.ChangeExtension(imageFilePath, ".png");

                        //using (var document = new Document(imageFilePath))
                        //{
                        //    var renderer = new Aspose.Pdf.Devices.PngDevice();
                        //    renderer.Process(document.Pages[1], ConvertedPDFtoPNG);
                        //} 

                        var dd = System.IO.File.ReadAllBytes(imageFilePath);
                        byte[] pngByte = Freeware.Pdf2Png.Convert(dd, 1);
                        System.IO.File.WriteAllBytes(ConvertedPDFtoPNG, pngByte);


                        using (Stream PDFbmpstrm = System.IO.File.Open(ConvertedPDFtoPNG, System.IO.FileMode.Open, FileAccess.Read))
                        {
                            
                            System.Drawing.Image PDFimg = System.Drawing.Image.FromStream(PDFbmpstrm);
                            var PDFBitmap = new Bitmap(PDFimg);
                            loadedImage = PDFimg;
                        }

                        imageEo = loadedImage;

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
                lastPlotRequest = this.lastInput;
            }
            else if (plotStore.plotType == PlotStore.PlotType.plot2D)
            {
                tempState = plotStore.titleState;
                plotStore.titleState = PlotStore.State.Default;
                //plotStore.xRangeS = PlotStore.State.Disable;
                lastPlotRequest = "explicit(sin(x),x,-5,5)";
            }
            else if (plotStore.plotType == PlotStore.PlotType.plot3D)
            {
                tempState = plotStore.titleState;
                plotStore.titleState = PlotStore.State.Default;
                //plotStore.xRangeS = PlotStore.State.Disable;
                //plotStore.yRangeS = PlotStore.State.Disable;
                lastPlotRequest = "explicit(2*sin(x)*cos(y),x,-5,5,y,-5,5)";
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
            if (plotApproval)
            {
                SetLastRequest();
                Plot();
                ScalImg(imageEo);
            }
            if (imageEs == null) return;
            if (!isError)
            {
                var bmp = SMath.Drawing.Graphics.Specifics.BitmapFromNativeImage(imageEs);
                GraphicsExtensions.DrawImage(e.Graphics, bmp, e.ClipRectangle.Location.X, e.ClipRectangle.Location.Y);
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
