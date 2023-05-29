using System;
using System.Collections.Generic;
using System.Windows.Forms;

using SMath.Manager;

namespace MaximaPlugin.MForms
{
    public partial class DebugForm : Form
    {
        static string returnString = "\"Debugger was used\"";
        static MaximaSession maximaform;
        public DebugForm()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            maximaform=ControlObjects.Translator.GetMaxima();
            //start session of not available
            ControlObjects.Translator.GetMaxima().StartSession();

            checkBox3.Text = "SMath preprocessing (only for Maxima(expr"+GlobalProfile.ArgumentsSeparatorStandard+"\"debug\"))";
            label1.Text = "Please note:\r\nTo get a direct term string from SMath use Maxima(expr" + GlobalProfile.ArgumentsSeparatorStandard + "\"debug\").\r\nIf SMath does not continue, push „Continue SMath calculation“ or „Do all“ again.";
            if (FormControl.formSMathCalc)
            {
                if (checkBox3.Checked) tbSmIn.Text = FormControl.formDataRePre;
                else tbSmIn.Text = FormControl.formDataRe;
            }

        }
        private void Form1_Click(object sender, EventArgs e)
        {
            if (FormControl.formSMathCalc)
            {
                if (checkBox3.Checked) tbSmIn.Text = FormControl.formDataRePre;
                else tbSmIn.Text = FormControl.formDataRe;
            }
        }
        public void Form1_Close(object sender, FormClosingEventArgs e)
        {
            if (tbSmOut.Text != "String to SMath")
                FormControl.formDataAn = tbSmOut.Text;
            else FormControl.formDataAn = returnString;
            FormControl.formReadyState = true;
            FormControl.formSMathCalc = false;
            MForms.FormControl.ThreadDebuggerProState = false;
        }
        private void bCoToMa_Click(object sender, EventArgs e)
        {
            ControlObjects.Translator.Log = ControlObjects.Translator.Log + "\n###   Debugger   ###";
            tbMaIn.Text = ControlObjects.Translator.TranslateToMaxima(tbSmIn.Text);
        }
        private void bCalcInMa_Click(object sender, EventArgs e)
        {
            string tempstring = maximaform.SendAndReceiveFromSocket(tbMaIn.Text);
            tbMaOut.Text = tempstring.Substring(1, (tempstring.Length - 2));
            if (cbAutoRe.Checked)
            {
                System.Threading.Thread.Sleep(Convert.ToInt32(tbTiToWa.Text));
                tbBu.Text = maximaform.ReceiveSingleCommandFromSocket();
            }
        }
        private void bCoToSm_Click(object sender, EventArgs e)
        {
            tbSmOut.Text = ControlObjects.Translator.TranslateToSMath(new List<string> { tbMaOut.Text });
        }
        private void bDoAll_Click(object sender, EventArgs e)
        {
            string tempstring = "";
            if (FormControl.formSMathCalc)
            {
                if (checkBox3.Checked) tbSmIn.Text = FormControl.formDataRePre;
                else tbSmIn.Text = FormControl.formDataRe;

            }
            ControlObjects.Translator.Log = ControlObjects.Translator.Log + "\n###   Debugger   ###";
            tbMaIn.Text = ControlObjects.Translator.TranslateToMaxima(tbSmIn.Text);
            
            tempstring = maximaform.SendAndReceiveFromSocket(tbMaIn.Text);
            tbMaOut.Text = tempstring.Substring(1, (tempstring.Length - 2));
            tbSmOut.Text = ControlObjects.Translator.TranslateToSMath(new List<string> { tbMaOut.Text });
            if (cbAutoRe.Checked)
            {
                System.Threading.Thread.Sleep(Convert.ToInt32(tbTiToWa.Text));
                tbBu.Text = maximaform.ReceiveSingleCommandFromSocket();
            }
            if (tbSmOut.Text != "String to SMath")
                FormControl.formDataAn = tbSmOut.Text;
            else FormControl.formDataAn = returnString;
            FormControl.formReadyState = true;
        }
        private void bMaRe_Click(object sender, EventArgs e)
        {
            string tempstring = maximaform.ReceiveSingleCommandFromSocket();
            tbBu.Text = tempstring.Substring(1, (tempstring.Length - 2));

        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (tbSmOut.Text != "String to SMath")
                FormControl.formDataAn = tbSmOut.Text;
            else FormControl.formDataAn = returnString;
            FormControl.formReadyState = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            maximaform.SendSingleCommandToSocket(tbSend.Text);
            string tempstring = maximaform.ReceiveSingleCommandFromSocket();
            tbBu.Text = tempstring.Substring(1, (tempstring.Length - 2));
        }
        private void bMaSe_Click(object sender, EventArgs e)
        {
            maximaform.SendSingleCommandToSocket(tbSend.Text);
        }
        private void bDisMa_Click(object sender, EventArgs e)
        {
            MaximaPlugin.MainClass.regularEnable = false;
        }
        private void bEnMa_Click(object sender, EventArgs e)
        {
            MaximaPlugin.MainClass.regularEnable = false;
        }
        private void tbLog_TextChanged(object sender, EventArgs e)
        {
        }
        private void opMaLog_CheckedChanged(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void opFuLog_CheckedChanged(object sender, EventArgs e)
        {
            tbLog_TextChanged(sender, e);
        }
        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            MaximaPlugin.MainClass.regularEnable = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            MaximaPlugin.MainClass.regularEnable = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.tbBu.Visible = true;
            this.bMaRe.Visible = true;
            this.tbSend.Visible = true;
            this.bMaSe.Visible = true;
            this.button2.Visible = true;
            this.groupBox2.Visible = true;
            this.groupBox3.Visible = true;
            this.Size = new System.Drawing.Size(700, 570);
        }

        private void tbSmIn_TextChanged(object sender, EventArgs e)
        {
            if (FormControl.formSMathCalc) button1.Visible = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                this.TopMost = true;
            else
                this.TopMost = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void DebugForm_Deactivate(object sender, EventArgs e)
        {
        }

        private void bactterm_Click(object sender, EventArgs e)
        {
            if (FormControl.formSMathCalc)
            {
                if (checkBox3.Checked) tbSmIn.Text = FormControl.formDataRePre;
                else tbSmIn.Text = FormControl.formDataRe;

            }
            else
            {
                DialogResult result2 = MessageBox.Show("Only possible while SMath calculate the function Maxima(expr" + GlobalProfile.ArgumentsSeparatorStandard + "\"debug\")",
                "No data",
                MessageBoxButtons.OK);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            bDoAll_Click(sender, e);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                FormControl.formWaitFor = true;
            else
                FormControl.formWaitFor = false;
        }
    }
}
