namespace MaximaPlugin.MForms
{
    partial class SettingsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PathTB = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.InstallGroupBox = new System.Windows.Forms.GroupBox();
            this.LocVerLabel = new System.Windows.Forms.Label();
            this.InstVerLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.InstallButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.InstallGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.checkedListBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(597, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 132);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select functions to takeover";
            this.groupBox1.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 104);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "15";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "int()",
            "diff()",
            "det()",
            "lim()",
            "sum()"});
            this.checkedListBox1.Location = new System.Drawing.Point(6, 19);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(100, 79);
            this.checkedListBox1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(112, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 91);
            this.label1.TabIndex = 7;
            this.label1.Text = "Select here which functions \r\ndo you want to takeover.\r\nThe number you get must \r" +
    "\nbe the twice argument e.g \r\nMaximaControl(takeover;\"number\")\r\nIf there is no ar" +
    "gument \r\nthe value is set to 15.";
            // 
            // PathTB
            // 
            this.PathTB.Location = new System.Drawing.Point(6, 19);
            this.PathTB.Name = "PathTB";
            this.PathTB.Size = new System.Drawing.Size(409, 20);
            this.PathTB.TabIndex = 2;
            this.PathTB.Text = "Cannot find maxima.bat. Please enter the path to maxima.bat or any start search p" +
    "ath.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SearchButton);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.PathTB);
            this.groupBox2.Location = new System.Drawing.Point(11, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(453, 78);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Path to Maxima";
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(421, 17);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(26, 23);
            this.SearchButton.TabIndex = 4;
            this.SearchButton.Text = "...";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(267, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Specify a directory from where to search for maxima.bat";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(314, 226);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 4;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(395, 226);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 5;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // InstallGroupBox
            // 
            this.InstallGroupBox.Controls.Add(this.LocVerLabel);
            this.InstallGroupBox.Controls.Add(this.InstVerLabel);
            this.InstallGroupBox.Controls.Add(this.label4);
            this.InstallGroupBox.Controls.Add(this.label3);
            this.InstallGroupBox.Controls.Add(this.label5);
            this.InstallGroupBox.Controls.Add(this.InstallButton);
            this.InstallGroupBox.Location = new System.Drawing.Point(11, 12);
            this.InstallGroupBox.Name = "InstallGroupBox";
            this.InstallGroupBox.Size = new System.Drawing.Size(453, 110);
            this.InstallGroupBox.TabIndex = 6;
            this.InstallGroupBox.TabStop = false;
            this.InstallGroupBox.Text = "Install Maxima";
            // 
            // LocVerLabel
            // 
            this.LocVerLabel.AutoSize = true;
            this.LocVerLabel.Location = new System.Drawing.Point(164, 56);
            this.LocVerLabel.Name = "LocVerLabel";
            this.LocVerLabel.Size = new System.Drawing.Size(58, 13);
            this.LocVerLabel.TabIndex = 9;
            this.LocVerLabel.Text = "Checking..";
            // 
            // InstVerLabel
            // 
            this.InstVerLabel.AutoSize = true;
            this.InstVerLabel.Location = new System.Drawing.Point(164, 34);
            this.InstVerLabel.Name = "InstVerLabel";
            this.InstVerLabel.Size = new System.Drawing.Size(33, 13);
            this.InstVerLabel.TabIndex = 8;
            this.InstVerLabel.Text = "None";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Latest available version online:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Installed Version: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(341, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Maxima installation requires active internet connection and admin rights";
            // 
            // InstallButton
            // 
            this.InstallButton.Location = new System.Drawing.Point(363, 46);
            this.InstallButton.Name = "InstallButton";
            this.InstallButton.Size = new System.Drawing.Size(75, 23);
            this.InstallButton.TabIndex = 0;
            this.InstallButton.Text = "Install";
            this.InstallButton.UseVisualStyleBackColor = true;
            this.InstallButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 261);
            this.Controls.Add(this.InstallGroupBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_Close);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.InstallGroupBox.ResumeLayout(false);
            this.InstallGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox PathTB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.GroupBox InstallGroupBox;
        private System.Windows.Forms.Button InstallButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label LocVerLabel;
        private System.Windows.Forms.Label InstVerLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}