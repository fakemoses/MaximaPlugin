namespace MaximaPlugin.MForms
{
    partial class DebugForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbSmIn = new System.Windows.Forms.TextBox();
            this.bCoToMa = new System.Windows.Forms.Button();
            this.tbMaIn = new System.Windows.Forms.TextBox();
            this.bCalcInMa = new System.Windows.Forms.Button();
            this.bCoToSm = new System.Windows.Forms.Button();
            this.tbSmOut = new System.Windows.Forms.TextBox();
            this.tbMaOut = new System.Windows.Forms.TextBox();
            this.bDoAll = new System.Windows.Forms.Button();
            this.tbBu = new System.Windows.Forms.TextBox();
            this.cbAutoRe = new System.Windows.Forms.CheckBox();
            this.tbTiToWa = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bMaRe = new System.Windows.Forms.Button();
            this.tbSend = new System.Windows.Forms.TextBox();
            this.bMaSe = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.bactterm = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSmIn
            // 
            this.tbSmIn.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.tbSmIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSmIn.Location = new System.Drawing.Point(12, 63);
            this.tbSmIn.Name = "tbSmIn";
            this.tbSmIn.Size = new System.Drawing.Size(486, 20);
            this.tbSmIn.TabIndex = 0;
            this.tbSmIn.Text = "String from SMath";
            this.tbSmIn.TextChanged += new System.EventHandler(this.tbSmIn_TextChanged);
            // 
            // bCoToMa
            // 
            this.bCoToMa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bCoToMa.Location = new System.Drawing.Point(12, 89);
            this.bCoToMa.Name = "bCoToMa";
            this.bCoToMa.Size = new System.Drawing.Size(486, 22);
            this.bCoToMa.TabIndex = 1;
            this.bCoToMa.Text = "↓ Convert to Maxima ↓";
            this.bCoToMa.UseVisualStyleBackColor = true;
            this.bCoToMa.Click += new System.EventHandler(this.bCoToMa_Click);
            // 
            // tbMaIn
            // 
            this.tbMaIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMaIn.Location = new System.Drawing.Point(12, 117);
            this.tbMaIn.Name = "tbMaIn";
            this.tbMaIn.Size = new System.Drawing.Size(486, 20);
            this.tbMaIn.TabIndex = 2;
            this.tbMaIn.Text = "String to Maxima";
            // 
            // bCalcInMa
            // 
            this.bCalcInMa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bCalcInMa.Location = new System.Drawing.Point(12, 143);
            this.bCalcInMa.Name = "bCalcInMa";
            this.bCalcInMa.Size = new System.Drawing.Size(486, 22);
            this.bCalcInMa.TabIndex = 3;
            this.bCalcInMa.Text = "↓ Calculate in Maxima ↓ ";
            this.bCalcInMa.UseVisualStyleBackColor = true;
            this.bCalcInMa.Click += new System.EventHandler(this.bCalcInMa_Click);
            // 
            // bCoToSm
            // 
            this.bCoToSm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bCoToSm.Location = new System.Drawing.Point(12, 200);
            this.bCoToSm.Name = "bCoToSm";
            this.bCoToSm.Size = new System.Drawing.Size(486, 22);
            this.bCoToSm.TabIndex = 4;
            this.bCoToSm.Text = "↓  Convert to SMath ↓ ";
            this.bCoToSm.UseVisualStyleBackColor = true;
            this.bCoToSm.Click += new System.EventHandler(this.bCoToSm_Click);
            // 
            // tbSmOut
            // 
            this.tbSmOut.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.tbSmOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSmOut.Location = new System.Drawing.Point(12, 228);
            this.tbSmOut.Name = "tbSmOut";
            this.tbSmOut.Size = new System.Drawing.Size(486, 20);
            this.tbSmOut.TabIndex = 5;
            this.tbSmOut.Text = "String to SMath";
            // 
            // tbMaOut
            // 
            this.tbMaOut.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.tbMaOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMaOut.Location = new System.Drawing.Point(12, 174);
            this.tbMaOut.Name = "tbMaOut";
            this.tbMaOut.Size = new System.Drawing.Size(486, 20);
            this.tbMaOut.TabIndex = 6;
            this.tbMaOut.Text = "String from Maxima";
            // 
            // bDoAll
            // 
            this.bDoAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bDoAll.Location = new System.Drawing.Point(504, 35);
            this.bDoAll.Name = "bDoAll";
            this.bDoAll.Size = new System.Drawing.Size(56, 241);
            this.bDoAll.TabIndex = 7;
            this.bDoAll.Text = "Do all";
            this.bDoAll.UseVisualStyleBackColor = true;
            this.bDoAll.Click += new System.EventHandler(this.bDoAll_Click);
            // 
            // tbBu
            // 
            this.tbBu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBu.Location = new System.Drawing.Point(12, 457);
            this.tbBu.Name = "tbBu";
            this.tbBu.Size = new System.Drawing.Size(486, 20);
            this.tbBu.TabIndex = 15;
            this.tbBu.Text = "String from Maxima";
            this.tbBu.Visible = false;
            // 
            // cbAutoRe
            // 
            this.cbAutoRe.AutoSize = true;
            this.cbAutoRe.Location = new System.Drawing.Point(5, 19);
            this.cbAutoRe.Name = "cbAutoRe";
            this.cbAutoRe.Size = new System.Drawing.Size(66, 17);
            this.cbAutoRe.TabIndex = 17;
            this.cbAutoRe.Text = "Receive";
            this.cbAutoRe.UseVisualStyleBackColor = true;
            // 
            // tbTiToWa
            // 
            this.tbTiToWa.Location = new System.Drawing.Point(71, 17);
            this.tbTiToWa.Name = "tbTiToWa";
            this.tbTiToWa.Size = new System.Drawing.Size(44, 20);
            this.tbTiToWa.TabIndex = 18;
            this.tbTiToWa.Text = "1000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(115, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "ms after calculating in Maxima";
            // 
            // bMaRe
            // 
            this.bMaRe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bMaRe.Location = new System.Drawing.Point(12, 429);
            this.bMaRe.Name = "bMaRe";
            this.bMaRe.Size = new System.Drawing.Size(486, 22);
            this.bMaRe.TabIndex = 20;
            this.bMaRe.Text = "Receive only";
            this.bMaRe.UseVisualStyleBackColor = true;
            this.bMaRe.Visible = false;
            this.bMaRe.Click += new System.EventHandler(this.bMaRe_Click);
            // 
            // tbSend
            // 
            this.tbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSend.Location = new System.Drawing.Point(12, 403);
            this.tbSend.Name = "tbSend";
            this.tbSend.Size = new System.Drawing.Size(486, 20);
            this.tbSend.TabIndex = 24;
            this.tbSend.Text = "String to Maxima";
            this.tbSend.Visible = false;
            // 
            // bMaSe
            // 
            this.bMaSe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bMaSe.Location = new System.Drawing.Point(12, 377);
            this.bMaSe.Name = "bMaSe";
            this.bMaSe.Size = new System.Drawing.Size(486, 22);
            this.bMaSe.TabIndex = 25;
            this.bMaSe.Text = "Send only";
            this.bMaSe.UseVisualStyleBackColor = true;
            this.bMaSe.Visible = false;
            this.bMaSe.Click += new System.EventHandler(this.bMaSe_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(12, 254);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(486, 22);
            this.button1.TabIndex = 28;
            this.button1.Text = "Continue SMath calculation ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(504, 377);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 100);
            this.button2.TabIndex = 29;
            this.button2.Text = "Do both!";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 279);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(469, 39);
            this.label1.TabIndex = 31;
            this.label1.Text = "Please note:\r\nTo get a direct term string from SMath use Maxima(expr;\"debug\").\r\nI" +
    "f SMath does not continue, push „Continue SMath calculation“ or „Do all“ again.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(12, 483);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(152, 49);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Maxima functions in SMath";
            this.groupBox2.Visible = false;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(70, 19);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(60, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Disable";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(58, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Enable";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbAutoRe);
            this.groupBox3.Controls.Add(this.tbTiToWa);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(170, 483);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(265, 49);
            this.groupBox3.TabIndex = 36;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Automatic socket buffer check";
            this.groupBox3.Visible = false;
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(12, 349);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(548, 22);
            this.button4.TabIndex = 37;
            this.button4.Text = "Expert mode";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(21, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(92, 17);
            this.checkBox1.TabIndex = 38;
            this.checkBox1.Text = "Always on top";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // bactterm
            // 
            this.bactterm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bactterm.Location = new System.Drawing.Point(12, 35);
            this.bactterm.Name = "bactterm";
            this.bactterm.Size = new System.Drawing.Size(486, 22);
            this.bactterm.TabIndex = 40;
            this.bactterm.Text = "↓ Update SMath input string ↓";
            this.bactterm.UseVisualStyleBackColor = true;
            this.bactterm.Click += new System.EventHandler(this.bactterm_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(121, 12);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(192, 17);
            this.checkBox2.TabIndex = 42;
            this.checkBox2.Text = "Wait for return string from debugger";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(319, 12);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(277, 17);
            this.checkBox3.TabIndex = 43;
            this.checkBox3.Text = "SMath preprocessing (only for Maxima(expr;\"debug\"))";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(640, 331);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.bactterm);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bMaSe);
            this.Controls.Add(this.tbSend);
            this.Controls.Add(this.bMaRe);
            this.Controls.Add(this.tbBu);
            this.Controls.Add(this.bDoAll);
            this.Controls.Add(this.tbMaOut);
            this.Controls.Add(this.tbSmOut);
            this.Controls.Add(this.bCoToSm);
            this.Controls.Add(this.bCalcInMa);
            this.Controls.Add(this.tbMaIn);
            this.Controls.Add(this.bCoToMa);
            this.Controls.Add(this.tbSmIn);
            this.Name = "DebugForm";
            this.Text = "Debugger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_Close);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Click += new System.EventHandler(this.Form1_Click);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSmIn;
        private System.Windows.Forms.Button bCoToMa;
        private System.Windows.Forms.TextBox tbMaIn;
        private System.Windows.Forms.Button bCalcInMa;
        private System.Windows.Forms.Button bCoToSm;
        private System.Windows.Forms.TextBox tbSmOut;
        private System.Windows.Forms.TextBox tbMaOut;
        private System.Windows.Forms.Button bDoAll;
        private System.Windows.Forms.TextBox tbBu;
        private System.Windows.Forms.CheckBox cbAutoRe;
        private System.Windows.Forms.TextBox tbTiToWa;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bMaRe;
        private System.Windows.Forms.TextBox tbSend;
        private System.Windows.Forms.Button bMaSe;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button bactterm;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
    }
}

