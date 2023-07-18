using System;
using System.Windows.Forms;

namespace MaximaPlugin.MForms
{

    public partial class LoadingForm : Form
    {
        public delegate void ProgressCallback();


        public LoadingForm(int maxsteps)
        {
            InitializeComponent();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {


        }


        public void progressControl(double step)
        {
            progressBar1.Value=(int)step;
        }

        /*
        public void progressControl()
        {
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
            if (this.progressBar1.InvokeRequired)
			{
                ProgressCallback d = new ProgressCallback(progressControl);
				this.Invoke(d);
			}
			else
			{
                this.progressBar1.PerformStep();
			}
		}*/

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
