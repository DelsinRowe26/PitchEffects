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
            this.btnFix = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbRange = new System.Windows.Forms.TabPage();
            this.tbSettings = new System.Windows.Forms.TabPage();
            this.tBxfrom1 = new System.Windows.Forms.TextBox();
            this.tBxto1 = new System.Windows.Forms.TextBox();
            this.tBxfrom2 = new System.Windows.Forms.TextBox();
            this.tBxfrom3 = new System.Windows.Forms.TextBox();
            this.tBxfrom4 = new System.Windows.Forms.TextBox();
            this.tBxto2 = new System.Windows.Forms.TextBox();
            this.tBxto3 = new System.Windows.Forms.TextBox();
            this.tBxto4 = new System.Windows.Forms.TextBox();
            this.tBxfrom5 = new System.Windows.Forms.TextBox();
            this.tBxto5 = new System.Windows.Forms.TextBox();
            this.tBxfrom6 = new System.Windows.Forms.TextBox();
            this.tBxto6 = new System.Windows.Forms.TextBox();
            this.tBxfrom7 = new System.Windows.Forms.TextBox();
            this.tBxto7 = new System.Windows.Forms.TextBox();
            this.tBxfrom8 = new System.Windows.Forms.TextBox();
            this.tBxto8 = new System.Windows.Forms.TextBox();
            this.tBxfrom9 = new System.Windows.Forms.TextBox();
            this.tBxto9 = new System.Windows.Forms.TextBox();
            this.tBxfrom10 = new System.Windows.Forms.TextBox();
            this.tBxto10 = new System.Windows.Forms.TextBox();
            this.lbFrom1 = new System.Windows.Forms.Label();
            this.lbFrom2 = new System.Windows.Forms.Label();
            this.lbFrom3 = new System.Windows.Forms.Label();
            this.lbFrom4 = new System.Windows.Forms.Label();
            this.lbFrom5 = new System.Windows.Forms.Label();
            this.lbFrom6 = new System.Windows.Forms.Label();
            this.lbFrom7 = new System.Windows.Forms.Label();
            this.lbFrom8 = new System.Windows.Forms.Label();
            this.lbFrom9 = new System.Windows.Forms.Label();
            this.lbFrom10 = new System.Windows.Forms.Label();
            this.lbTo1 = new System.Windows.Forms.Label();
            this.lbTo2 = new System.Windows.Forms.Label();
            this.lbTo3 = new System.Windows.Forms.Label();
            this.lbTo4 = new System.Windows.Forms.Label();
            this.lbTo5 = new System.Windows.Forms.Label();
            this.lbTo6 = new System.Windows.Forms.Label();
            this.lbTo7 = new System.Windows.Forms.Label();
            this.lbTo8 = new System.Windows.Forms.Label();
            this.lbTo9 = new System.Windows.Forms.Label();
            this.lbTo10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGain)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tbRange.SuspendLayout();
            this.tbSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(691, 172);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.label2.Location = new System.Drawing.Point(532, 25);
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
            this.trackPitch.Location = new System.Drawing.Point(587, 18);
            this.trackPitch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.cmbInput.Location = new System.Drawing.Point(94, 11);
            this.cmbInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbInput.Name = "cmbInput";
            this.cmbInput.Size = new System.Drawing.Size(385, 24);
            this.cmbInput.TabIndex = 7;
            // 
            // cmbOutput
            // 
            this.cmbOutput.FormattingEnabled = true;
            this.cmbOutput.Location = new System.Drawing.Point(94, 45);
            this.cmbOutput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbOutput.Name = "cmbOutput";
            this.cmbOutput.Size = new System.Drawing.Size(385, 24);
            this.cmbOutput.TabIndex = 8;
            // 
            // lblMic
            // 
            this.lblMic.AutoSize = true;
            this.lblMic.Location = new System.Drawing.Point(4, 14);
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
            this.lblSpeaker.Location = new System.Drawing.Point(4, 52);
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
            this.label1.Location = new System.Drawing.Point(532, 81);
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
            this.trackGain.Location = new System.Drawing.Point(587, 81);
            this.trackGain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.chkAddMp3.Location = new System.Drawing.Point(536, 144);
            this.chkAddMp3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkAddMp3.Name = "chkAddMp3";
            this.chkAddMp3.Size = new System.Drawing.Size(137, 21);
            this.chkAddMp3.TabIndex = 13;
            this.chkAddMp3.Text = "Add Sample Mp3";
            this.chkAddMp3.UseVisualStyleBackColor = true;
            this.chkAddMp3.CheckedChanged += new System.EventHandler(this.chkAddMp3_CheckedChanged);
            // 
            // bTnPlus
            // 
            this.bTnPlus.Location = new System.Drawing.Point(427, 135);
            this.bTnPlus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bTnPlus.Name = "bTnPlus";
            this.bTnPlus.Size = new System.Drawing.Size(35, 23);
            this.bTnPlus.TabIndex = 14;
            this.bTnPlus.Text = "+";
            this.bTnPlus.UseVisualStyleBackColor = true;
            this.bTnPlus.Click += new System.EventHandler(this.bTnPlus_Click);
            // 
            // bTnMinus
            // 
            this.bTnMinus.Enabled = false;
            this.bTnMinus.Location = new System.Drawing.Point(427, 165);
            this.bTnMinus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bTnMinus.Name = "bTnMinus";
            this.bTnMinus.Size = new System.Drawing.Size(35, 23);
            this.bTnMinus.TabIndex = 15;
            this.bTnMinus.Text = "-";
            this.bTnMinus.UseVisualStyleBackColor = true;
            this.bTnMinus.Click += new System.EventHandler(this.bTnMinus_Click);
            // 
            // btnFix
            // 
            this.btnFix.Location = new System.Drawing.Point(691, 136);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(135, 29);
            this.btnFix.TabIndex = 16;
            this.btnFix.Text = "Fix";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbRange);
            this.tabControl1.Controls.Add(this.tbSettings);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(497, 220);
            this.tabControl1.TabIndex = 17;
            // 
            // tbRange
            // 
            this.tbRange.AutoScroll = true;
            this.tbRange.Controls.Add(this.lbTo10);
            this.tbRange.Controls.Add(this.lbTo9);
            this.tbRange.Controls.Add(this.lbTo8);
            this.tbRange.Controls.Add(this.lbTo7);
            this.tbRange.Controls.Add(this.lbTo6);
            this.tbRange.Controls.Add(this.lbTo5);
            this.tbRange.Controls.Add(this.lbTo4);
            this.tbRange.Controls.Add(this.lbTo3);
            this.tbRange.Controls.Add(this.bTnMinus);
            this.tbRange.Controls.Add(this.bTnPlus);
            this.tbRange.Controls.Add(this.lbTo2);
            this.tbRange.Controls.Add(this.lbTo1);
            this.tbRange.Controls.Add(this.lbFrom10);
            this.tbRange.Controls.Add(this.lbFrom9);
            this.tbRange.Controls.Add(this.lbFrom8);
            this.tbRange.Controls.Add(this.lbFrom7);
            this.tbRange.Controls.Add(this.lbFrom6);
            this.tbRange.Controls.Add(this.lbFrom5);
            this.tbRange.Controls.Add(this.lbFrom4);
            this.tbRange.Controls.Add(this.lbFrom3);
            this.tbRange.Controls.Add(this.lbFrom2);
            this.tbRange.Controls.Add(this.lbFrom1);
            this.tbRange.Controls.Add(this.tBxto10);
            this.tbRange.Controls.Add(this.tBxfrom10);
            this.tbRange.Controls.Add(this.tBxto9);
            this.tbRange.Controls.Add(this.tBxfrom9);
            this.tbRange.Controls.Add(this.tBxto8);
            this.tbRange.Controls.Add(this.tBxfrom8);
            this.tbRange.Controls.Add(this.tBxto7);
            this.tbRange.Controls.Add(this.tBxfrom7);
            this.tbRange.Controls.Add(this.tBxto6);
            this.tbRange.Controls.Add(this.tBxfrom6);
            this.tbRange.Controls.Add(this.tBxto5);
            this.tbRange.Controls.Add(this.tBxfrom5);
            this.tbRange.Controls.Add(this.tBxto4);
            this.tbRange.Controls.Add(this.tBxto3);
            this.tbRange.Controls.Add(this.tBxto2);
            this.tbRange.Controls.Add(this.tBxfrom4);
            this.tbRange.Controls.Add(this.tBxfrom3);
            this.tbRange.Controls.Add(this.tBxfrom2);
            this.tbRange.Controls.Add(this.tBxto1);
            this.tbRange.Controls.Add(this.tBxfrom1);
            this.tbRange.Location = new System.Drawing.Point(4, 25);
            this.tbRange.Name = "tbRange";
            this.tbRange.Padding = new System.Windows.Forms.Padding(3);
            this.tbRange.Size = new System.Drawing.Size(489, 191);
            this.tbRange.TabIndex = 0;
            this.tbRange.Text = "Range";
            this.tbRange.UseVisualStyleBackColor = true;
            // 
            // tbSettings
            // 
            this.tbSettings.Controls.Add(this.cmbOutput);
            this.tbSettings.Controls.Add(this.lblSpeaker);
            this.tbSettings.Controls.Add(this.lblMic);
            this.tbSettings.Controls.Add(this.cmbInput);
            this.tbSettings.Location = new System.Drawing.Point(4, 25);
            this.tbSettings.Name = "tbSettings";
            this.tbSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tbSettings.Size = new System.Drawing.Size(489, 191);
            this.tbSettings.TabIndex = 1;
            this.tbSettings.Text = "Settings";
            this.tbSettings.UseVisualStyleBackColor = true;
            // 
            // tBxfrom1
            // 
            this.tBxfrom1.Location = new System.Drawing.Point(79, 7);
            this.tBxfrom1.Name = "tBxfrom1";
            this.tBxfrom1.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom1.TabIndex = 16;
            this.tBxfrom1.Visible = false;
            // 
            // tBxto1
            // 
            this.tBxto1.Location = new System.Drawing.Point(241, 6);
            this.tBxto1.Name = "tBxto1";
            this.tBxto1.Size = new System.Drawing.Size(100, 22);
            this.tBxto1.TabIndex = 17;
            this.tBxto1.Visible = false;
            // 
            // tBxfrom2
            // 
            this.tBxfrom2.Location = new System.Drawing.Point(79, 35);
            this.tBxfrom2.Name = "tBxfrom2";
            this.tBxfrom2.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom2.TabIndex = 18;
            this.tBxfrom2.Visible = false;
            // 
            // tBxfrom3
            // 
            this.tBxfrom3.Location = new System.Drawing.Point(79, 63);
            this.tBxfrom3.Name = "tBxfrom3";
            this.tBxfrom3.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom3.TabIndex = 19;
            this.tBxfrom3.Visible = false;
            // 
            // tBxfrom4
            // 
            this.tBxfrom4.Location = new System.Drawing.Point(79, 91);
            this.tBxfrom4.Name = "tBxfrom4";
            this.tBxfrom4.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom4.TabIndex = 20;
            this.tBxfrom4.Visible = false;
            // 
            // tBxto2
            // 
            this.tBxto2.Location = new System.Drawing.Point(241, 34);
            this.tBxto2.Name = "tBxto2";
            this.tBxto2.Size = new System.Drawing.Size(100, 22);
            this.tBxto2.TabIndex = 21;
            this.tBxto2.Visible = false;
            // 
            // tBxto3
            // 
            this.tBxto3.Location = new System.Drawing.Point(241, 63);
            this.tBxto3.Name = "tBxto3";
            this.tBxto3.Size = new System.Drawing.Size(100, 22);
            this.tBxto3.TabIndex = 22;
            this.tBxto3.Visible = false;
            this.tBxto3.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // tBxto4
            // 
            this.tBxto4.Location = new System.Drawing.Point(241, 91);
            this.tBxto4.Name = "tBxto4";
            this.tBxto4.Size = new System.Drawing.Size(100, 22);
            this.tBxto4.TabIndex = 23;
            this.tBxto4.Visible = false;
            // 
            // tBxfrom5
            // 
            this.tBxfrom5.Location = new System.Drawing.Point(79, 119);
            this.tBxfrom5.Name = "tBxfrom5";
            this.tBxfrom5.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom5.TabIndex = 24;
            this.tBxfrom5.Visible = false;
            // 
            // tBxto5
            // 
            this.tBxto5.Location = new System.Drawing.Point(241, 119);
            this.tBxto5.Name = "tBxto5";
            this.tBxto5.Size = new System.Drawing.Size(100, 22);
            this.tBxto5.TabIndex = 25;
            this.tBxto5.Visible = false;
            // 
            // tBxfrom6
            // 
            this.tBxfrom6.Location = new System.Drawing.Point(79, 147);
            this.tBxfrom6.Name = "tBxfrom6";
            this.tBxfrom6.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom6.TabIndex = 26;
            this.tBxfrom6.Visible = false;
            // 
            // tBxto6
            // 
            this.tBxto6.Location = new System.Drawing.Point(241, 147);
            this.tBxto6.Name = "tBxto6";
            this.tBxto6.Size = new System.Drawing.Size(100, 22);
            this.tBxto6.TabIndex = 27;
            this.tBxto6.Visible = false;
            // 
            // tBxfrom7
            // 
            this.tBxfrom7.Location = new System.Drawing.Point(79, 175);
            this.tBxfrom7.Name = "tBxfrom7";
            this.tBxfrom7.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom7.TabIndex = 28;
            this.tBxfrom7.Visible = false;
            // 
            // tBxto7
            // 
            this.tBxto7.Location = new System.Drawing.Point(241, 175);
            this.tBxto7.Name = "tBxto7";
            this.tBxto7.Size = new System.Drawing.Size(100, 22);
            this.tBxto7.TabIndex = 29;
            this.tBxto7.Visible = false;
            // 
            // tBxfrom8
            // 
            this.tBxfrom8.Location = new System.Drawing.Point(79, 203);
            this.tBxfrom8.Name = "tBxfrom8";
            this.tBxfrom8.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom8.TabIndex = 30;
            this.tBxfrom8.Visible = false;
            // 
            // tBxto8
            // 
            this.tBxto8.Location = new System.Drawing.Point(241, 203);
            this.tBxto8.Name = "tBxto8";
            this.tBxto8.Size = new System.Drawing.Size(100, 22);
            this.tBxto8.TabIndex = 31;
            this.tBxto8.Visible = false;
            // 
            // tBxfrom9
            // 
            this.tBxfrom9.Location = new System.Drawing.Point(79, 231);
            this.tBxfrom9.Name = "tBxfrom9";
            this.tBxfrom9.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom9.TabIndex = 32;
            this.tBxfrom9.Visible = false;
            // 
            // tBxto9
            // 
            this.tBxto9.Location = new System.Drawing.Point(241, 231);
            this.tBxto9.Name = "tBxto9";
            this.tBxto9.Size = new System.Drawing.Size(100, 22);
            this.tBxto9.TabIndex = 33;
            this.tBxto9.Visible = false;
            // 
            // tBxfrom10
            // 
            this.tBxfrom10.Location = new System.Drawing.Point(79, 259);
            this.tBxfrom10.Name = "tBxfrom10";
            this.tBxfrom10.Size = new System.Drawing.Size(100, 22);
            this.tBxfrom10.TabIndex = 34;
            this.tBxfrom10.Visible = false;
            // 
            // tBxto10
            // 
            this.tBxto10.Location = new System.Drawing.Point(241, 259);
            this.tBxto10.Name = "tBxto10";
            this.tBxto10.Size = new System.Drawing.Size(100, 22);
            this.tBxto10.TabIndex = 35;
            this.tBxto10.Visible = false;
            // 
            // lbFrom1
            // 
            this.lbFrom1.AutoSize = true;
            this.lbFrom1.Location = new System.Drawing.Point(17, 11);
            this.lbFrom1.Name = "lbFrom1";
            this.lbFrom1.Size = new System.Drawing.Size(56, 17);
            this.lbFrom1.TabIndex = 36;
            this.lbFrom1.Text = "1. From";
            this.lbFrom1.Visible = false;
            // 
            // lbFrom2
            // 
            this.lbFrom2.AutoSize = true;
            this.lbFrom2.Location = new System.Drawing.Point(17, 37);
            this.lbFrom2.Name = "lbFrom2";
            this.lbFrom2.Size = new System.Drawing.Size(56, 17);
            this.lbFrom2.TabIndex = 37;
            this.lbFrom2.Text = "2. From";
            this.lbFrom2.Visible = false;
            // 
            // lbFrom3
            // 
            this.lbFrom3.AutoSize = true;
            this.lbFrom3.Location = new System.Drawing.Point(17, 66);
            this.lbFrom3.Name = "lbFrom3";
            this.lbFrom3.Size = new System.Drawing.Size(56, 17);
            this.lbFrom3.TabIndex = 38;
            this.lbFrom3.Text = "3. From";
            this.lbFrom3.Visible = false;
            // 
            // lbFrom4
            // 
            this.lbFrom4.AutoSize = true;
            this.lbFrom4.Location = new System.Drawing.Point(17, 94);
            this.lbFrom4.Name = "lbFrom4";
            this.lbFrom4.Size = new System.Drawing.Size(56, 17);
            this.lbFrom4.TabIndex = 39;
            this.lbFrom4.Text = "4. From";
            this.lbFrom4.Visible = false;
            // 
            // lbFrom5
            // 
            this.lbFrom5.AutoSize = true;
            this.lbFrom5.Location = new System.Drawing.Point(17, 122);
            this.lbFrom5.Name = "lbFrom5";
            this.lbFrom5.Size = new System.Drawing.Size(56, 17);
            this.lbFrom5.TabIndex = 40;
            this.lbFrom5.Text = "5. From";
            this.lbFrom5.Visible = false;
            // 
            // lbFrom6
            // 
            this.lbFrom6.AutoSize = true;
            this.lbFrom6.Location = new System.Drawing.Point(17, 150);
            this.lbFrom6.Name = "lbFrom6";
            this.lbFrom6.Size = new System.Drawing.Size(56, 17);
            this.lbFrom6.TabIndex = 41;
            this.lbFrom6.Text = "6. From";
            this.lbFrom6.Visible = false;
            // 
            // lbFrom7
            // 
            this.lbFrom7.AutoSize = true;
            this.lbFrom7.Location = new System.Drawing.Point(17, 178);
            this.lbFrom7.Name = "lbFrom7";
            this.lbFrom7.Size = new System.Drawing.Size(56, 17);
            this.lbFrom7.TabIndex = 42;
            this.lbFrom7.Text = "7. From";
            this.lbFrom7.Visible = false;
            // 
            // lbFrom8
            // 
            this.lbFrom8.AutoSize = true;
            this.lbFrom8.Location = new System.Drawing.Point(17, 206);
            this.lbFrom8.Name = "lbFrom8";
            this.lbFrom8.Size = new System.Drawing.Size(56, 17);
            this.lbFrom8.TabIndex = 43;
            this.lbFrom8.Text = "8. From";
            this.lbFrom8.Visible = false;
            // 
            // lbFrom9
            // 
            this.lbFrom9.AutoSize = true;
            this.lbFrom9.Location = new System.Drawing.Point(17, 234);
            this.lbFrom9.Name = "lbFrom9";
            this.lbFrom9.Size = new System.Drawing.Size(56, 17);
            this.lbFrom9.TabIndex = 44;
            this.lbFrom9.Text = "9. From";
            this.lbFrom9.Visible = false;
            // 
            // lbFrom10
            // 
            this.lbFrom10.AutoSize = true;
            this.lbFrom10.Location = new System.Drawing.Point(17, 262);
            this.lbFrom10.Name = "lbFrom10";
            this.lbFrom10.Size = new System.Drawing.Size(64, 17);
            this.lbFrom10.TabIndex = 45;
            this.lbFrom10.Text = "10. From";
            this.lbFrom10.Visible = false;
            // 
            // lbTo1
            // 
            this.lbTo1.AutoSize = true;
            this.lbTo1.Location = new System.Drawing.Point(199, 11);
            this.lbTo1.Name = "lbTo1";
            this.lbTo1.Size = new System.Drawing.Size(20, 17);
            this.lbTo1.TabIndex = 46;
            this.lbTo1.Text = "to";
            this.lbTo1.Visible = false;
            // 
            // lbTo2
            // 
            this.lbTo2.AutoSize = true;
            this.lbTo2.Location = new System.Drawing.Point(199, 38);
            this.lbTo2.Name = "lbTo2";
            this.lbTo2.Size = new System.Drawing.Size(20, 17);
            this.lbTo2.TabIndex = 47;
            this.lbTo2.Text = "to";
            this.lbTo2.Visible = false;
            // 
            // lbTo3
            // 
            this.lbTo3.AutoSize = true;
            this.lbTo3.Location = new System.Drawing.Point(199, 66);
            this.lbTo3.Name = "lbTo3";
            this.lbTo3.Size = new System.Drawing.Size(20, 17);
            this.lbTo3.TabIndex = 48;
            this.lbTo3.Text = "to";
            this.lbTo3.Visible = false;
            // 
            // lbTo4
            // 
            this.lbTo4.AutoSize = true;
            this.lbTo4.Location = new System.Drawing.Point(199, 94);
            this.lbTo4.Name = "lbTo4";
            this.lbTo4.Size = new System.Drawing.Size(20, 17);
            this.lbTo4.TabIndex = 49;
            this.lbTo4.Text = "to";
            this.lbTo4.Visible = false;
            // 
            // lbTo5
            // 
            this.lbTo5.AutoSize = true;
            this.lbTo5.Location = new System.Drawing.Point(199, 122);
            this.lbTo5.Name = "lbTo5";
            this.lbTo5.Size = new System.Drawing.Size(20, 17);
            this.lbTo5.TabIndex = 50;
            this.lbTo5.Text = "to";
            this.lbTo5.Visible = false;
            // 
            // lbTo6
            // 
            this.lbTo6.AutoSize = true;
            this.lbTo6.Location = new System.Drawing.Point(199, 150);
            this.lbTo6.Name = "lbTo6";
            this.lbTo6.Size = new System.Drawing.Size(20, 17);
            this.lbTo6.TabIndex = 51;
            this.lbTo6.Text = "to";
            this.lbTo6.Visible = false;
            // 
            // lbTo7
            // 
            this.lbTo7.AutoSize = true;
            this.lbTo7.Location = new System.Drawing.Point(199, 178);
            this.lbTo7.Name = "lbTo7";
            this.lbTo7.Size = new System.Drawing.Size(20, 17);
            this.lbTo7.TabIndex = 52;
            this.lbTo7.Text = "to";
            this.lbTo7.Visible = false;
            // 
            // lbTo8
            // 
            this.lbTo8.AutoSize = true;
            this.lbTo8.Location = new System.Drawing.Point(199, 206);
            this.lbTo8.Name = "lbTo8";
            this.lbTo8.Size = new System.Drawing.Size(20, 17);
            this.lbTo8.TabIndex = 53;
            this.lbTo8.Text = "to";
            this.lbTo8.Visible = false;
            // 
            // lbTo9
            // 
            this.lbTo9.AutoSize = true;
            this.lbTo9.Location = new System.Drawing.Point(199, 234);
            this.lbTo9.Name = "lbTo9";
            this.lbTo9.Size = new System.Drawing.Size(20, 17);
            this.lbTo9.TabIndex = 54;
            this.lbTo9.Text = "to";
            this.lbTo9.Visible = false;
            // 
            // lbTo10
            // 
            this.lbTo10.AutoSize = true;
            this.lbTo10.Location = new System.Drawing.Point(199, 262);
            this.lbTo10.Name = "lbTo10";
            this.lbTo10.Size = new System.Drawing.Size(20, 17);
            this.lbTo10.TabIndex = 55;
            this.lbTo10.Text = "to";
            this.lbTo10.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1044, 236);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnFix);
            this.Controls.Add(this.chkAddMp3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackGain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackPitch);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pitch Shifter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackGain)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tbRange.ResumeLayout(false);
            this.tbRange.PerformLayout();
            this.tbSettings.ResumeLayout(false);
            this.tbSettings.PerformLayout();
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
        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbRange;
        private System.Windows.Forms.TextBox tBxto1;
        private System.Windows.Forms.TextBox tBxfrom1;
        private System.Windows.Forms.TabPage tbSettings;
        private System.Windows.Forms.TextBox tBxto4;
        private System.Windows.Forms.TextBox tBxto3;
        private System.Windows.Forms.TextBox tBxto2;
        private System.Windows.Forms.TextBox tBxfrom4;
        private System.Windows.Forms.TextBox tBxfrom3;
        private System.Windows.Forms.TextBox tBxfrom2;
        private System.Windows.Forms.TextBox tBxto10;
        private System.Windows.Forms.TextBox tBxfrom10;
        private System.Windows.Forms.TextBox tBxto9;
        private System.Windows.Forms.TextBox tBxfrom9;
        private System.Windows.Forms.TextBox tBxto8;
        private System.Windows.Forms.TextBox tBxfrom8;
        private System.Windows.Forms.TextBox tBxto7;
        private System.Windows.Forms.TextBox tBxfrom7;
        private System.Windows.Forms.TextBox tBxto6;
        private System.Windows.Forms.TextBox tBxfrom6;
        private System.Windows.Forms.TextBox tBxto5;
        private System.Windows.Forms.TextBox tBxfrom5;
        private System.Windows.Forms.Label lbTo10;
        private System.Windows.Forms.Label lbTo9;
        private System.Windows.Forms.Label lbTo8;
        private System.Windows.Forms.Label lbTo7;
        private System.Windows.Forms.Label lbTo6;
        private System.Windows.Forms.Label lbTo5;
        private System.Windows.Forms.Label lbTo4;
        private System.Windows.Forms.Label lbTo3;
        private System.Windows.Forms.Label lbTo2;
        private System.Windows.Forms.Label lbTo1;
        private System.Windows.Forms.Label lbFrom10;
        private System.Windows.Forms.Label lbFrom9;
        private System.Windows.Forms.Label lbFrom8;
        private System.Windows.Forms.Label lbFrom7;
        private System.Windows.Forms.Label lbFrom6;
        private System.Windows.Forms.Label lbFrom5;
        private System.Windows.Forms.Label lbFrom4;
        private System.Windows.Forms.Label lbFrom3;
        private System.Windows.Forms.Label lbFrom2;
        private System.Windows.Forms.Label lbFrom1;
    }
}

