namespace MaximaPlugin.MForms
{
    partial class InstallerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FinishButton = new System.Windows.Forms.Button();
            this.DlProgressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.InstProgressBar = new System.Windows.Forms.ProgressBar();
            this.StatusPanel = new System.Windows.Forms.Panel();
            this.InstTypePanel = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.NextButton = new System.Windows.Forms.Button();
            this.ManualInstRB = new System.Windows.Forms.RadioButton();
            this.SilentInstRB = new System.Windows.Forms.RadioButton();
            this.StatusPanel.SuspendLayout();
            this.InstTypePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // FinishButton
            // 
            this.FinishButton.Location = new System.Drawing.Point(383, 163);
            this.FinishButton.Name = "FinishButton";
            this.FinishButton.Size = new System.Drawing.Size(75, 23);
            this.FinishButton.TabIndex = 0;
            this.FinishButton.Text = "Finish";
            this.FinishButton.UseVisualStyleBackColor = true;
            this.FinishButton.Click += new System.EventHandler(this.FinishButton_Click);
            // 
            // DlProgressBar
            // 
            this.DlProgressBar.Location = new System.Drawing.Point(3, 23);
            this.DlProgressBar.Name = "DlProgressBar";
            this.DlProgressBar.Size = new System.Drawing.Size(302, 23);
            this.DlProgressBar.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Downloading Maxima";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Installing Maxima";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(316, 26);
            this.label3.TabIndex = 5;
            this.label3.Text = "Please allow admin access when requested to install the maxima. \r\nInstallation ma" +
    "y takes few minutes.";
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(46, 163);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(63, 13);
            this.StatusLabel.TabIndex = 6;
            this.StatusLabel.Text = "StatusLabel";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Status:";
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(302, 163);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 8;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // InstProgressBar
            // 
            this.InstProgressBar.Location = new System.Drawing.Point(3, 76);
            this.InstProgressBar.Name = "InstProgressBar";
            this.InstProgressBar.Size = new System.Drawing.Size(302, 23);
            this.InstProgressBar.TabIndex = 9;
            // 
            // StatusPanel
            // 
            this.StatusPanel.Controls.Add(this.DlProgressBar);
            this.StatusPanel.Controls.Add(this.InstProgressBar);
            this.StatusPanel.Controls.Add(this.FinishButton);
            this.StatusPanel.Controls.Add(this.CancelButton);
            this.StatusPanel.Controls.Add(this.label1);
            this.StatusPanel.Controls.Add(this.label5);
            this.StatusPanel.Controls.Add(this.label2);
            this.StatusPanel.Controls.Add(this.StatusLabel);
            this.StatusPanel.Controls.Add(this.label3);
            this.StatusPanel.Location = new System.Drawing.Point(12, 12);
            this.StatusPanel.Name = "StatusPanel";
            this.StatusPanel.Size = new System.Drawing.Size(470, 200);
            this.StatusPanel.TabIndex = 10;
            // 
            // InstTypePanel
            // 
            this.InstTypePanel.Controls.Add(this.label8);
            this.InstTypePanel.Controls.Add(this.label7);
            this.InstTypePanel.Controls.Add(this.label6);
            this.InstTypePanel.Controls.Add(this.NextButton);
            this.InstTypePanel.Controls.Add(this.ManualInstRB);
            this.InstTypePanel.Controls.Add(this.SilentInstRB);
            this.InstTypePanel.Location = new System.Drawing.Point(12, 230);
            this.InstTypePanel.Name = "InstTypePanel";
            this.InstTypePanel.Size = new System.Drawing.Size(470, 200);
            this.InstTypePanel.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 124);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(344, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "User-customizable installation. Default installation path is recommended.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(205, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Automatic installation with default settings.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(195, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Select the type of the installation";
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(383, 157);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 23);
            this.NextButton.TabIndex = 2;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // ManualInstRB
            // 
            this.ManualInstRB.AutoSize = true;
            this.ManualInstRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ManualInstRB.Location = new System.Drawing.Point(14, 104);
            this.ManualInstRB.Name = "ManualInstRB";
            this.ManualInstRB.Size = new System.Drawing.Size(131, 17);
            this.ManualInstRB.TabIndex = 1;
            this.ManualInstRB.TabStop = true;
            this.ManualInstRB.Text = "Manual installation";
            this.ManualInstRB.UseVisualStyleBackColor = true;
            // 
            // SilentInstRB
            // 
            this.SilentInstRB.AutoSize = true;
            this.SilentInstRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SilentInstRB.Location = new System.Drawing.Point(14, 47);
            this.SilentInstRB.Name = "SilentInstRB";
            this.SilentInstRB.Size = new System.Drawing.Size(122, 17);
            this.SilentInstRB.TabIndex = 0;
            this.SilentInstRB.TabStop = true;
            this.SilentInstRB.Text = "Silent installation";
            this.SilentInstRB.UseVisualStyleBackColor = true;
            // 
            // InstallerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 444);
            this.Controls.Add(this.InstTypePanel);
            this.Controls.Add(this.StatusPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "InstallerForm";
            this.Text = "Maxima Installation";
            this.StatusPanel.ResumeLayout(false);
            this.StatusPanel.PerformLayout();
            this.InstTypePanel.ResumeLayout(false);
            this.InstTypePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button FinishButton;
        private System.Windows.Forms.ProgressBar DlProgressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.ProgressBar InstProgressBar;
        private System.Windows.Forms.Panel StatusPanel;
        private System.Windows.Forms.Panel InstTypePanel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.RadioButton ManualInstRB;
        private System.Windows.Forms.RadioButton SilentInstRB;
    }
}