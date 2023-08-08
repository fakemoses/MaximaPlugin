using SMath.Manager;
using SMath.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaximaPlugin.MForms
{
    public partial class MaximaSettingsForm : Form
    {
        public MaximaSettingsForm()
        {
            InitializeComponent();
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(button5, "Restart the maxima session.");

            ToolTip toolTip2 = new ToolTip();
            toolTip2.SetToolTip(button6, "Cleanup current Maxima session (reset and kill all).");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //restart maxima
            ControlObjects.Translator.GetMaxima().RestartMaxima();

            // how to reset SMath entry? 

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //cleanup maxima
            ControlObjects.Translator.GetMaxima().CleanupMaxima();

            // how to reset the SMath entry?
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //delete image folder if exists
        }
    }
}
