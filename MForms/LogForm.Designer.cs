namespace MaximaPlugin.MForms
{
    partial class LogForm
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
            this.opWxm = new System.Windows.Forms.RadioButton();
            this.opFuLog = new System.Windows.Forms.RadioButton();
            this.opMaLog = new System.Windows.Forms.RadioButton();
            this.SaveButton = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.AlwaysOnTopCB = new System.Windows.Forms.CheckBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.opWxm);
            this.groupBox1.Controls.Add(this.opFuLog);
            this.groupBox1.Controls.Add(this.opMaLog);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 92);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log type";
            // 
            // opWxm
            // 
            this.opWxm.AutoSize = true;
            this.opWxm.Location = new System.Drawing.Point(6, 66);
            this.opWxm.Name = "opWxm";
            this.opWxm.Size = new System.Drawing.Size(46, 17);
            this.opWxm.TabIndex = 37;
            this.opWxm.TabStop = true;
            this.opWxm.Text = "wxm";
            this.opWxm.UseVisualStyleBackColor = true;
            this.opWxm.CheckedChanged += new System.EventHandler(this.opWxm_CheckedChanged);
            // 
            // opFuLog
            // 
            this.opFuLog.AutoSize = true;
            this.opFuLog.Location = new System.Drawing.Point(6, 43);
            this.opFuLog.Name = "opFuLog";
            this.opFuLog.Size = new System.Drawing.Size(96, 17);
            this.opFuLog.TabIndex = 36;
            this.opFuLog.Text = "Full conversion";
            this.opFuLog.UseVisualStyleBackColor = true;
            this.opFuLog.CheckedChanged += new System.EventHandler(this.opFuLog_CheckedChanged);
            // 
            // opMaLog
            // 
            this.opMaLog.AutoSize = true;
            this.opMaLog.Checked = true;
            this.opMaLog.Location = new System.Drawing.Point(6, 20);
            this.opMaLog.Name = "opMaLog";
            this.opMaLog.Size = new System.Drawing.Size(135, 17);
            this.opMaLog.TabIndex = 35;
            this.opMaLog.TabStop = true;
            this.opMaLog.Text = "Maxima communication";
            this.opMaLog.UseVisualStyleBackColor = true;
            this.opMaLog.CheckedChanged += new System.EventHandler(this.opMaLog_CheckedChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.Location = new System.Drawing.Point(594, 21);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(74, 83);
            this.SaveButton.TabIndex = 36;
            this.SaveButton.Text = "Save as log";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLog.Location = new System.Drawing.Point(12, 110);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(656, 460);
            this.tbLog.TabIndex = 35;
            this.tbLog.TextChanged += new System.EventHandler(this.tbLog_TextChanged);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RefreshButton.Location = new System.Drawing.Point(322, 21);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(266, 38);
            this.RefreshButton.TabIndex = 38;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // AlwaysOnTopCB
            // 
            this.AlwaysOnTopCB.AutoSize = true;
            this.AlwaysOnTopCB.Location = new System.Drawing.Point(6, 21);
            this.AlwaysOnTopCB.Name = "AlwaysOnTopCB";
            this.AlwaysOnTopCB.Size = new System.Drawing.Size(92, 17);
            this.AlwaysOnTopCB.TabIndex = 39;
            this.AlwaysOnTopCB.Text = "Always on top";
            this.AlwaysOnTopCB.UseVisualStyleBackColor = true;
            this.AlwaysOnTopCB.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ClearButton
            // 
            this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearButton.Location = new System.Drawing.Point(322, 66);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(266, 38);
            this.ClearButton.TabIndex = 40;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AlwaysOnTopCB);
            this.groupBox2.Location = new System.Drawing.Point(166, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(150, 92);
            this.groupBox2.TabIndex = 41;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 582);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SaveButton);
            this.Name = "LogForm";
            this.Text = "Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogForm_Close);
            this.Load += new System.EventHandler(this.LogForm_Load);
            this.Shown += new System.EventHandler(this.tbLog_TextChanged);
            this.Click += new System.EventHandler(this.LogForm_Click);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton opFuLog;
        private System.Windows.Forms.RadioButton opMaLog;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.CheckBox AlwaysOnTopCB;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton opWxm;
    }
}