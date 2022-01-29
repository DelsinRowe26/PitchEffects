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
            ((System.ComponentModel.ISupportInitialize)(this.trackPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGain)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(527, 117);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(101, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(399, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Pitch";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // trackPitch
            // 
            this.trackPitch.Enabled = false;
            this.trackPitch.Location = new System.Drawing.Point(440, 15);
            this.trackPitch.Minimum = -10;
            this.trackPitch.Name = "trackPitch";
            this.trackPitch.Size = new System.Drawing.Size(300, 45);
            this.trackPitch.TabIndex = 5;
            this.trackPitch.Scroll += new System.EventHandler(this.trackPitch_Scroll);
            this.trackPitch.ValueChanged += new System.EventHandler(this.trackPitch_ValueChanged);
            // 
            // cmbInput
            // 
            this.cmbInput.FormattingEnabled = true;
            this.cmbInput.Location = new System.Drawing.Point(101, 12);
            this.cmbInput.Name = "cmbInput";
            this.cmbInput.Size = new System.Drawing.Size(290, 21);
            this.cmbInput.TabIndex = 7;
            // 
            // cmbOutput
            // 
            this.cmbOutput.FormattingEnabled = true;
            this.cmbOutput.Location = new System.Drawing.Point(101, 39);
            this.cmbOutput.Name = "cmbOutput";
            this.cmbOutput.Size = new System.Drawing.Size(290, 21);
            this.cmbOutput.TabIndex = 8;
            // 
            // lblMic
            // 
            this.lblMic.AutoSize = true;
            this.lblMic.Location = new System.Drawing.Point(21, 15);
            this.lblMic.Name = "lblMic";
            this.lblMic.Size = new System.Drawing.Size(63, 13);
            this.lblMic.TabIndex = 9;
            this.lblMic.Text = "Microphone";
            this.lblMic.Click += new System.EventHandler(this.lblMic_Click);
            // 
            // lblSpeaker
            // 
            this.lblSpeaker.AutoSize = true;
            this.lblSpeaker.Location = new System.Drawing.Point(21, 42);
            this.lblSpeaker.Name = "lblSpeaker";
            this.lblSpeaker.Size = new System.Drawing.Size(47, 13);
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
            this.label1.Location = new System.Drawing.Point(399, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Gain";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // trackGain
            // 
            this.trackGain.Enabled = false;
            this.trackGain.Location = new System.Drawing.Point(440, 66);
            this.trackGain.Maximum = 30;
            this.trackGain.Name = "trackGain";
            this.trackGain.Size = new System.Drawing.Size(300, 45);
            this.trackGain.TabIndex = 11;
            this.trackGain.Scroll += new System.EventHandler(this.trackGain_Scroll);
            this.trackGain.ValueChanged += new System.EventHandler(this.trackGain_ValueChanged);
            // 
            // chkAddMp3
            // 
            this.chkAddMp3.AutoSize = true;
            this.chkAddMp3.Enabled = false;
            this.chkAddMp3.Location = new System.Drawing.Point(402, 117);
            this.chkAddMp3.Name = "chkAddMp3";
            this.chkAddMp3.Size = new System.Drawing.Size(107, 17);
            this.chkAddMp3.TabIndex = 13;
            this.chkAddMp3.Text = "Add Sample Mp3";
            this.chkAddMp3.UseVisualStyleBackColor = true;
            this.chkAddMp3.CheckedChanged += new System.EventHandler(this.chkAddMp3_CheckedChanged);
            // 
            // bTnPlus
            // 
            this.bTnPlus.Location = new System.Drawing.Point(365, 92);
            this.bTnPlus.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bTnPlus.Name = "bTnPlus";
            this.bTnPlus.Size = new System.Drawing.Size(26, 19);
            this.bTnPlus.TabIndex = 14;
            this.bTnPlus.Text = "+";
            this.bTnPlus.UseVisualStyleBackColor = true;
            this.bTnPlus.Click += new System.EventHandler(this.bTnPlus_Click);
            // 
            // bTnMinus
            // 
            this.bTnMinus.Location = new System.Drawing.Point(365, 115);
            this.bTnMinus.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bTnMinus.Name = "bTnMinus";
            this.bTnMinus.Size = new System.Drawing.Size(26, 19);
            this.bTnMinus.TabIndex = 15;
            this.bTnMinus.Text = "-";
            this.bTnMinus.UseVisualStyleBackColor = true;
            this.bTnMinus.Click += new System.EventHandler(this.bTnMinus_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(783, 151);
            this.Controls.Add(this.bTnMinus);
            this.Controls.Add(this.lblSpeaker);
            this.Controls.Add(this.bTnPlus);
            this.Controls.Add(this.lblMic);
            this.Controls.Add(this.chkAddMp3);
            this.Controls.Add(this.cmbOutput);
            this.Controls.Add(this.cmbInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackGain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackPitch);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pitch Shifter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGain)).EndInit();
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
    }
}

