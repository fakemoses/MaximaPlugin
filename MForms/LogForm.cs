using System;
using System.Windows.Forms;

namespace MaximaPlugin.MForms
{
    public partial class LogForm : Form
    {
        MaximaSession session;
        public LogForm()
        {
            InitializeComponent();
            session = ControlObjects.Translator.GetMaxima();
        }
        private void tbLog_TextChanged(object sender, EventArgs e)
        {
            if (opFuLog.Checked)
            {
                tbLog.Text = ControlObjects.Translator.Log.Replace("\n", "\r\n");
            }
            else if (opMaLog.Checked)
            {
                tbLog.Text = session.socket.fullLog.Replace("\n", "\r\n");
            }else if(opWxm.Checked)
            {
                tbLog.Text = MaximaPlugin.MaximaSocket.wxmLog;
            }
            tbLog.SelectionStart = tbLog.Text.Length;
            tbLog.ScrollToCaret();
        }
        private void opFuLog_CheckedChanged(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void LogForm_Click(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void opMaLog_CheckedChanged(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void LogForm_Load(object sender, EventArgs e)
        {
            tbLog.ScrollBars = ScrollBars.Both;
            //maximaform = new AutoMaxima();
            tbLog_TextChanged(sender, e);
        }
        private void LogForm_Close(object sender, FormClosingEventArgs e)
        {
            MForms.FormControl.ThreadLogProState = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (opWxm.Checked)
            {        
                System.Diagnostics.Process.Start(MaximaSocket.WriteWXM());
            }
            else
            {
                SharedFunctions.WriteDataToFile(ControlObjects.Translator.GetMaxima().WorkingFolderPath() + "\\" + "Maxima.log", tbLog.Text);
                System.Diagnostics.Process.Start(ControlObjects.Translator.GetMaxima().WorkingFolderPath() + "\\" + "Maxima.log");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                this.TopMost = true;
            else
                this.TopMost = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            session.socket.ClearFullLog("Cleared by request from Log Viewer\n");
            MaximaPlugin.MaximaSocket.wxmLog = "";
            MaximaPlugin.ControlObjects.Translator.Log = "Cleared by request from Log Viewer\n";
            tbLog_TextChanged(sender, e);
        }
        private void opWxm_CheckedChanged(object sender, EventArgs e)
        {
            if (opWxm.Checked)
                button3.Text = "Save as wxm";
            else
                button3.Text = "Save as log";
        }
    }
}
