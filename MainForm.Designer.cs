namespace PitchShifter
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.trackPitch = new System.Windows.Forms.TrackBar();
            this.cmbInput = new System.Windows.Forms.ComboBox();
            this.cmbOutput = new System.Windows.Forms.ComboBox();
            this.lblMic = new System.Windows.Forms.Label();
            this.lblSpeaker = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.trackGain = new System.Windows.Forms.TrackBar();
            this.chkAddMp3 = new System.Windows.Forms.CheckBox();
            this.bTnPlus = new System.Windows.Forms.Button();
            this.bTnMinus = new System.Windows.Forms.Button();
            this.tbContr = new System.Windows.Forms.TabControl();
            this.tbPgDiap = new System.Windows.Forms.TabPage();
            this.tbPg = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.trackPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGain)).BeginInit();
            this.tbContr.SuspendLayout();
            this.tbPgDiap.SuspendLayout();
            this.tbPg.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(199, 366);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(135, 28);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 238);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Pitch";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // trackPitch
            // 
            this.trackPitch.Enabled = false;
            this.trackPitch.Location = new System.Drawing.Point(117, 238);
            this.trackPitch.Margin = new System.Windows.Forms.Padding(4);
            this.trackPitch.Minimum = -10;
            this.trackPitch.Name = "trackPitch";
            this.trackPitch.Size = new System.Drawing.Size(400, 56);
            this.trackPitch.TabIndex = 5;
            this.trackPitch.Scroll += new System.EventHandler(this.trackPitch_Scroll);
            this.trackPitch.ValueChanged += new System.EventHandler(this.trackPitch_ValueChanged);
            // 
            // cmbInput
            // 
            this.cmbInput.FormattingEnabled = true;
            this.cmbInput.Location = new System.Drawing.Point(97, 17);
            this.cmbInput.Margin = new System.Windows.Forms.Padding(4);
            this.cmbInput.Name = "cmbInput";
            this.cmbInput.Size = new System.Drawing.Size(385, 24);
            this.cmbInput.TabIndex = 7;
            // 
            // cmbOutput
            // 
            this.cmbOutput.FormattingEnabled = true;
            this.cmbOutput.Location = new System.Drawing.Point(97, 48);
            this.cmbOutput.Margin = new System.Windows.Forms.Padding(4);
            this.cmbOutput.Name = "cmbOutput";
            this.cmbOutput.Size = new System.Drawing.Size(385, 24);
            this.cmbOutput.TabIndex = 8;
            // 
            // lblMic
            // 
            this.lblMic.AutoSize = true;
            this.lblMic.Location = new System.Drawing.Point(7, 17);
            this.lblMic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMic.Name = "lblMic";
            this.lblMic.Size = new System.Drawing.Size(82, 17);
            this.lblMic.TabIndex = 9;
            this.lblMic.Text = "Microphone";
            this.lblMic.Click += new System.EventHandler(this.lblMic_Click);
            // 
            // lblSpeaker
            // 
            this.lblSpeaker.AutoSize = true;
            this.lblSpeaker.Location = new System.Drawing.Point(7, 51);
            this.lblSpeaker.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpeaker.Name = "lblSpeaker";
            this.lblSpeaker.Size = new System.Drawing.Size(61, 17);
            this.lblSpeaker.TabIndex = 10;
            this.lblSpeaker.Text = "Speaker";
            // 
            // timer1
            // 
            this.timer1.Interval = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 302);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Gain";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // trackGain
            // 
            this.trackGain.Enabled = false;
            this.trackGain.Location = new System.Drawing.Point(117, 302);
            this.trackGain.Margin = new System.Windows.Forms.Padding(4);
            this.trackGain.Maximum = 30;
            this.trackGain.Name = "trackGain";
            this.trackGain.Size = new System.Drawing.Size(400, 56);
            this.trackGain.TabIndex = 11;
            this.trackGain.Scroll += new System.EventHandler(this.trackGain_Scroll);
            this.trackGain.ValueChanged += new System.EventHandler(this.trackGain_ValueChanged);
            // 
            // chkAddMp3
            // 
            this.chkAddMp3.AutoSize = true;
            this.chkAddMp3.Enabled = false;
            this.chkAddMp3.Location = new System.Drawing.Point(13, 371);
            this.chkAddMp3.Margin = new System.Windows.Forms.Padding(4);
            this.chkAddMp3.Name = "chkAddMp3";
            this.chkAddMp3.Size = new System.Drawing.Size(137, 21);
            this.chkAddMp3.TabIndex = 13;
            this.chkAddMp3.Text = "Add Sample Mp3";
            this.chkAddMp3.UseVisualStyleBackColor = true;
            this.chkAddMp3.CheckedChanged += new System.EventHandler(this.chkAddMp3_CheckedChanged);
            // 
            // bTnPlus
            // 
            this.bTnPlus.Location = new System.Drawing.Point(463, 132);
            this.bTnPlus.Name = "bTnPlus";
            this.bTnPlus.Size = new System.Drawing.Size(34, 23);
            this.bTnPlus.TabIndex = 14;
            this.bTnPlus.Text = "+";
            this.bTnPlus.UseVisualStyleBackColor = true;
            this.bTnPlus.Click += new System.EventHandler(this.bTnPlus_Click);
            // 
            // bTnMinus
            // 
            this.bTnMinus.Location = new System.Drawing.Point(463, 161);
            this.bTnMinus.Name = "bTnMinus";
            this.bTnMinus.Size = new System.Drawing.Size(34, 23);
            this.bTnMinus.TabIndex = 15;
            this.bTnMinus.Text = "-";
            this.bTnMinus.UseVisualStyleBackColor = true;
            this.bTnMinus.Click += new System.EventHandler(this.bTnMinus_Click);
            // 
            // tbContr
            // 
            this.tbContr.Controls.Add(this.tbPgDiap);
            this.tbContr.Controls.Add(this.tbPg);
            this.tbContr.Location = new System.Drawing.Point(12, 12);
            this.tbContr.Name = "tbContr";
            this.tbContr.SelectedIndex = 0;
            this.tbContr.Size = new System.Drawing.Size(511, 219);
            this.tbContr.TabIndex = 16;
            this.tbContr.BindingContextChanged += new System.EventHandler(this.bTnPlus_Click);
            // 
            // tbPgDiap
            // 
            this.tbPgDiap.AutoScroll = true;
            this.tbPgDiap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPgDiap.Controls.Add(this.bTnMinus);
            this.tbPgDiap.Controls.Add(this.bTnPlus);
            this.tbPgDiap.Location = new System.Drawing.Point(4, 25);
            this.tbPgDiap.Name = "tbPgDiap";
            this.tbPgDiap.Padding = new System.Windows.Forms.Padding(3);
            this.tbPgDiap.Size = new System.Drawing.Size(503, 190);
            this.tbPgDiap.TabIndex = 1;
            this.tbPgDiap.Text = "Ranges";
            this.tbPgDiap.UseVisualStyleBackColor = true;
            this.tbPgDiap.BindingContextChanged += new System.EventHandler(this.bTnPlus_Click);
            this.tbPgDiap.Click += new System.EventHandler(this.tbPgDiap_Click);
            // 
            // tbPg
            // 
            this.tbPg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPg.Controls.Add(this.lblSpeaker);
            this.tbPg.Controls.Add(this.lblMic);
            this.tbPg.Controls.Add(this.cmbInput);
            this.tbPg.Controls.Add(this.cmbOutput);
            this.tbPg.Location = new System.Drawing.Point(4, 25);
            this.tbPg.Name = "tbPg";
            this.tbPg.Padding = new System.Windows.Forms.Padding(3);
            this.tbPg.Size = new System.Drawing.Size(503, 190);
            this.tbPg.TabIndex = 0;
            this.tbPg.Text = "Settings";
            this.tbPg.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 424);
            this.Controls.Add(this.tbContr);
            this.Controls.Add(this.chkAddMp3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackGain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackPitch);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pitch Shifter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGain)).EndInit();
            this.tbContr.ResumeLayout(false);
            this.tbPgDiap.ResumeLayout(false);
            this.tbPg.ResumeLayout(false);
            this.tbPg.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackPitch;
        private System.Windows.Forms.ComboBox cmbInput;
        private System.Windows.Forms.ComboBox cmbOutput;
        private System.Windows.Forms.Label lblMic;
        private System.Windows.Forms.Label lblSpeaker;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackGain;
        private System.Windows.Forms.CheckBox chkAddMp3;
        private System.Windows.Forms.Button bTnPlus;
        private System.Windows.Forms.Button bTnMinus;
        private System.Windows.Forms.TabControl tbContr;
        private System.Windows.Forms.TabPage tbPg;
        private System.Windows.Forms.TabPage tbPgDiap;
    }
}

