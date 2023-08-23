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
            this.button3 = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
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
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(611, 21);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(57, 83);
            this.button3.TabIndex = 36;
            this.button3.Text = "Save as log";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
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
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(322, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(283, 38);
            this.button1.TabIndex = 38;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 21);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(92, 17);
            this.checkBox1.TabIndex = 39;
            this.checkBox1.Text = "Always on top";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(322, 66);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(283, 38);
            this.button2.TabIndex = 40;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox1);
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
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
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
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton opWxm;
    }
}