using SMath.Controls;
using SMath.Drawing;
using SMath.Drawing.Svg;
using SMath.Manager;

using System;
using System.Drawing;
using System.IO;

namespace MaximaPlugin
{
    // partial implementation of main class for readability
    // code is from CustomGlyphs by Davide Carpi
    // https://smath.com:8443/!/#public/view/head/plugins/CustomGlyphs
    // For IPluginRegionDrawing, manual import of MathRegion from Smath installation folder in the References might be needed
    public partial class MainClass : IPluginToolboxGroups, IPluginHandleEvaluation, IPluginRegionDrawing
    {
        #region IPluginToolboxGroups
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

            groups[0] = new ToolboxGroup { Title = "Maxima", Buttons = buttons, Index = 1};

            return groups;
        }

        private IBitmap GetImage(string image)
        {
            return SessionsManager.Current.Specifics.BitmapFromResources(GlobalProfile.GetAssembly<MainClass>(), "MaximaPlugin.Resources1." + image + ".png");
        }

        private IBitmap GetPngImage(Bitmap image)
        {
            var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return SessionsManager.Current.Specifics.StreamToBitmap(ms);
        }

        private string GetButtonAction(string name, int argsCount, SessionProfile sessionProfile)
        {
            var term = new Term(name, TermType.Function, argsCount);

            TermInfo.ChangeTermByNamingType(sessionProfile, term, sessionProfile.NamingType, true);
            return term.Text + Brackets.LeftVisible + (argsCount > 1 ? new String(sessionProfile.ArgumentsSeparator, argsCount - 1) : String.Empty);
        }

        #region IPluginRegionDrawing
        public bool PrepareDrawing(Term[] terms, DrawInfo[] finish, MathPainter math, PaintContext e)
        {
            if (math == null || terms[math.Inc].Type != TermType.Function)
                return false;

            var term = terms[math.Inc];

            float fontScaleFactor = Math.Max(e.NormalFont.Height / 10, 1); // FIXME: not good if FontHeight < 10

            using (var graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
            {
                var font = new Font(e.NormalFont.Name, e.NormalFont.Size, (FontStyle)e.NormalFont.Style);

                if (term.Text == "Maxima")
                {
                    using (Font LogoFont = new Font(FontFamily.GenericSerif, 16 * fontScaleFactor))
                    {
                        Size LogoSize = graphics.MeasureString("ξΣ", LogoFont).ToSize();

                        int childCount = term.ArgsCount;
                        float width = LogoSize.Width;
                        float center = (LogoSize.Height - 8) / 2;
                        float center2bottom = (LogoSize.Height - 8) / 2 + 3;

                        for (int i = 1; i <= childCount; i++)
                        {
                            width += finish[math.finishNum - i].Size.Width;
                            center = Math.Max(center, finish[math.finishNum - i].Center);
                            center2bottom = Math.Max(center2bottom, finish[math.finishNum - i].Size.Height - finish[math.finishNum - i].Center);
                        }

                        int sepWidth = (int)graphics.MeasureString(e.SessionProfile.ArgumentsSeparator.ToString(), font).Width;
                        width += (childCount - 1) * sepWidth + 7 * 2 - 1;

                        var points = new PointF[childCount];
                        float posX = LogoSize.Width + 6;

                        for (int i = 0; i < childCount; i++)
                        {
                            points[i] = new PointF(posX, center - finish[math.finishNum - (childCount - i)].Center + 1);
                            posX += finish[math.finishNum - (childCount - i)].Size.Width + sepWidth;
                        }

                        math.DrawWizard(terms, width, center + center2bottom + 3, points, math.Inc, center + 1);
                    }

                    return true;
                }
            }

            if (term.ArgsCount == 2 && term.Text.Equals("Cross"))
            {
                Prepare_InlineInfix_SVG(11, 16, terms, finish, math, e);

                return true;
            }

            return false;
        }

        public bool ProceedDrawing(DrawMemory[] mem, MathPainter math, PaintContext e)
        {
            if (math == null || mem[math.Inc].Type != TermType.Function)
                return false;

            float fontScaleFactor = Math.Max(e.NormalFont.Height / 10, 1); // FIXME: not good if FontHeight < 10
            var dm = mem[math.Inc];

            if (dm.Text.Equals("Maxima"))
            {
                // the user can delete placeholders
                dm.IsCommonFunction = true;

                ColorBrush backLogoBrush = new ColorBrush(e.CurFontClr != 0 ? Color.White : Color.FromArgb(255, 12, 50, 180));
                ColorBrush foreLogoBrush = new ColorBrush(Color.FromArgb(255, 252, 40, 40));
                FontInfo backLogoFont = new FontInfo(SMath.Drawing.Graphics.Specifics.GenericSerif, 16 * fontScaleFactor, FontfaceStyle.Bold);
                FontInfo foreLogoFont = new FontInfo(SMath.Drawing.Graphics.Specifics.GenericSerif, 12 * fontScaleFactor, FontfaceStyle.Bold);
                {
                    Size LogoSize = e.Graphics.MeasureString("ξΣ", backLogoFont, float.MaxValue, StringOptions.GenericDefault).ToSize();
                    float posX = e.ClipRectangle.Location.X + dm.Location.X - (int)fontScaleFactor;
                    float posY = e.ClipRectangle.Location.Y + dm.Location.Y + dm.Center - LogoSize.Height / 2 - 1 * (int)fontScaleFactor + 2;
                    e.Graphics.DrawString("ξΣ", backLogoFont, backLogoBrush, posX, posY, float.MaxValue, StringOptions.GenericDefault);
                    e.Graphics.DrawString("Μ", foreLogoFont, foreLogoBrush, posX + 5 * fontScaleFactor, posY + 5 * (int)fontScaleFactor - 2, float.MaxValue, StringOptions.GenericDefault);

                    //dm.FunctionTextBounds = new Rectangle(0, posY, 1, 40);dm.

                    Size sepSize = e.Graphics.MeasureString(e.SessionProfile.ArgumentsSeparator.ToString(), e.NormalFont, float.MaxValue, StringOptions.GenericDefault).ToSize();
                    float bracketPnt2Y = dm.Center + (LogoSize.Height - 8) / 2;

                    for (int i = 0; i < dm.pm.Length; i++)
                    {
                        bracketPnt2Y = Math.Max(bracketPnt2Y, mem[dm.pm[i]].Size.Height - mem[dm.pm[i]].Center + dm.Center);

                        if (i > 0)
                            e.CustomStringDrawing(e.SessionProfile.ArgumentsSeparator.ToString(), e.CurFontClr, mem[dm.pm[i]].Location.X - sepSize.Width - dm.Location.X, dm.Center - sepSize.Height / 2 + 1, dm);
                    }

                    e.BracketPnt1 = new Point(LogoSize.Width - 1 - 2 * (int)fontScaleFactor, 0);
                    // to have a full-hight bracket: bracketPnt2Y = mathRegion.mem[mathRegion.Inc].Size.Height
                    e.BracketPnt2 = new PointF(dm.Size.Width - 1, bracketPnt2Y + 1);
                    e.Parenthesis = true;
                }

                return true;
            }

            if (dm.ArgsCount == 2 && dm.Text.Equals("Cross"))
            {
                // ×
                Proceed_InlineInfix_SVG(SvgPaths.CrossProduct, 11, 16, 0.4f, mem, math, e, true);

                return true;
            }

            return false;
        }

        private void Prepare_InlineInfix_SVG(float w, float h, Term[] terms, DrawInfo[] finish, MathPainter math, PaintContext e)
        {
            var fontScaleFactor = e.NormalFont.Height / 10f; // font 10pt is reference
            var symbolSize = new SizeF(w * fontScaleFactor, h * fontScaleFactor);

            var numChilds = terms[math.Inc].ArgsCount;
            var pts = new PointF[numChilds];

            int ptsOffsetX = 0;
            int ptsOffsetY = 0;

            float width = ptsOffsetX + symbolSize.Width * (numChilds - 1);
            float height = 0;
            float center = ptsOffsetY + symbolSize.Height / 2f;

            float maxCenter = center;
            float maxDescent = symbolSize.Height / 2f;

            for (int i = numChilds; i > 0; i--)
            {
                var term = finish[math.finishNum - i];
                width += term.Size.Width;
                maxCenter = Math.Max(maxCenter, term.Center);
                maxDescent = Math.Max(maxDescent, term.Size.Height - term.Center);
            }

            height = ptsOffsetY + maxCenter + maxDescent;

            float x = ptsOffsetX;
            float y = ptsOffsetY + maxCenter;

            for (int i = numChilds; i > 0; i--)
            {
                var term = finish[math.finishNum - i];
                pts[numChilds - i] = new PointF(x, y - term.Center);
                x += symbolSize.Width + term.Size.Width;
            }

            math.DrawWizard(terms, width, height, pts, math.Inc, maxCenter);
        }


        private void Proceed_InlineInfix_SVG(GraphicsFormatter gf, float width, float heigth, float svgScale, DrawMemory[] mem, MathPainter math, PaintContext e, bool isCOmmonFunction = false)
        {
            Proceed_InlineInfix_SVG(new[] { gf }, width, heigth, svgScale, mem, math, e, isCOmmonFunction);
        }

        private void Proceed_InlineInfix_SVG(GraphicsFormatter[] gfs, float width, float heigth, float svgScale, DrawMemory[] mem, MathPainter math, PaintContext e, bool isCOmmonFunction = false)
        {
            // the user can delete placeholders
            mem[math.Inc].IsCommonFunction = isCOmmonFunction;

            var function = mem[math.Inc];
            var pmLength = function.pm.Length;

            var fontScaleFactor = e.NormalFont.Height / 10f; // font 10pt is reference
            var symbolSize = new SizeF(width * fontScaleFactor, heigth * fontScaleFactor);

            var posX = function.Location.X;
            var posY = function.Location.Y + (int)function.Center - 1;

            var symbolScale = fontScaleFactor * svgScale;
            var reverseScale = 1f / symbolScale;

            e.Graphics.Translate(e.ClipRectangle.X, e.ClipRectangle.Y);

            for (int i = 0; i < pmLength - 1; i++)
            {
                var halfWidth = symbolSize.Width / 2f;
                posX += mem[function.pm[i]].Size.Width + halfWidth - 0.5f;

                e.Graphics.Translate(posX, posY);
                e.Graphics.Scale(symbolScale, symbolScale);

                foreach (var gf in gfs)
                    e.Graphics.FillPath(new ColorBrush(e.CurLineClr == 0 ? SMathColors.RegularText : SMathColors.HighlightedText), gf.Steps.ToArray());

                e.Graphics.Scale(reverseScale, reverseScale);
                e.Graphics.Translate(-posX, -posY);

                posX += halfWidth + 0.5f;
            }

            e.Graphics.Translate(-e.ClipRectangle.X, -e.ClipRectangle.Y);
        }
        #endregion

        #endregion
    }

    #region svgPaths
    internal static class SvgPaths
    {
        // operations
        public static GraphicsFormatter CrossProduct = new GraphicsFormatter();

        public static void Init()
        {
            // all glyphs are centered to 0,0
            // online tool to preview/edit: https://yqnn.github.io/svg-path-editor/

            // operations
            SvgPathParser.Parse("M -5 6 L -7 4 L -2 -1 L -7 -6 L -5 -8 L 0 -3 L 5 -8 L 7 -6 L 2 -1 L 7 4 L 5 6 L 0 1 Z", CrossProduct);
        }
    }
    #endregion

}
