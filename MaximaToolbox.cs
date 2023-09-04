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
    // https://smath.com:8443/!/#public/view/head/plugins/CustomGlyphs/Toolbox.cs
    public partial class MainClass : IPluginToolboxGroups, IPluginHandleEvaluation
    {
        #region IPluginToolboxGroups
        public ToolboxGroup[] GetToolboxGroups(SessionProfile sessionProfile)
        {
            var groups = new ToolboxGroup[1];

            var buttons = new[]
            {
                new ButtonsMetaData(GetPngImage(Resource1.maxima))
                    { Size = new Size(36, 24), Description = "Process an expression in Maxima", Action = GetButtonAction("Maxima", 1, sessionProfile) },
                new ButtonsMetaData("Control")
                    { Size = new Size(54, 24), Description = "Control Maxima process", Action = GetButtonAction("MaximaControl", 1, sessionProfile) },
                new ButtonsMetaData("Define")
                    { Size = new Size(54, 24), Description = "Define in Maxima", Action = GetButtonAction("MaximaDefine", 1, sessionProfile) },
                new ButtonsMetaData("Takeover")
                    { Size = new Size(72, 24), Description = "Takeover by Maxima of SMath functions", Action = GetButtonAction("MaximaTakeover", 1, sessionProfile) },
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

        #endregion

        
    }
}
