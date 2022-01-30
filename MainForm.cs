using System;
using System.Windows.Forms;
using System.Diagnostics;
using CSCore;
using CSCore.SoundIn;//Вход звука
using CSCore.SoundOut;//Выход звука
using CSCore.CoreAudioAPI;
using CSCore.Streams;
using CSCore.Codecs;
using System.Drawing;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.Linq;

namespace PitchShifter
{
    public partial class MainForm : Form
    {
        //Глобальные переменные
        int[] min = new int[10];
        int[] max = new int[10];
        int plusclick = 0;
        public static List<TextBox> TextBoxes = new List<TextBox>();
        public static List<Label> labels = new List<Label>();
        public static List<Label> nums = new List<Label>();
        private MMDeviceCollection mInputDevices;
        private MMDeviceCollection mOutputDevices;
        private WasapiCapture mSoundIn;
        private WasapiOut mSoundOut;
        private SampleDSP mDsp;
        //private VolumeSource vSab;
        private SimpleMixer mMixer;
        private ISampleSource mMp3;
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Находит устройства для захвата звука и заполнияет комбобокс
            MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
            mInputDevices = deviceEnum.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
            MMDevice activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
            foreach (MMDevice device in mInputDevices)
            {
                cmbInput.Items.Add(device.FriendlyName);
                if (device.DeviceID == activeDevice.DeviceID) cmbInput.SelectedIndex = cmbInput.Items.Count - 1;
            }

            //Находит устройства для вывода звука и заполняет комбобокс
            activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            mOutputDevices = deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
            foreach (MMDevice device in mOutputDevices)
            {
                cmbOutput.Items.Add(device.FriendlyName);
                if (device.DeviceID == activeDevice.DeviceID) cmbOutput.SelectedIndex = cmbOutput.Items.Count - 1;
            }

        }

        private bool StartFullDuplex()
        {
            try
            {
                //Запускает устройство захвата звука с задержкой 5 мс.
                mSoundIn = new WasapiCapture(false, AudioClientShareMode.Exclusive, 1);
                mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex]; 
                mSoundIn.Initialize();
                mSoundIn.Start();

                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };

                //Init DSP для смещения высоты тона
                mDsp = new SampleDSP(source.ToSampleSource().ToMono());
                mDsp.GainDB = trackGain.Value;
                SetPitchShiftValue();

                //Инициальный микшер
                mMixer = new SimpleMixer(1, 44100) //моно, 44,1 КГц
                {
                    FillWithZeros = false,
                    DivideResult = true //Для этого установлено значение true, чтобы избежать звуков тиков из-за превышения -1 и 1.
                };

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 5 мс.
                mSoundOut = new WasapiOut(false, AudioClientShareMode.Exclusive, 1);
                mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];
                mSoundOut.Initialize(mMixer.ToWaveSource(16));
                
                //Start rolling!
                mSoundOut.Play();
                return true;
            }
            catch (Exception ex)
            {
                string msg = "Error in StartFullDuplex: \r\n" + ex.Message;
                MessageBox.Show(msg);
                Debug.WriteLine(msg);
            }
            return false;
        }

        private void StopFullDuplex()
        {
            if (mSoundOut != null) mSoundOut.Dispose();
            if (mSoundIn != null) mSoundIn.Dispose();
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            StopFullDuplex();
            if (StartFullDuplex())
            {
                trackGain.Enabled = true;
                trackPitch.Enabled = true;
                chkAddMp3.Enabled = true;
            }
        }

        private void SetPitchShiftValue()
        {
            mDsp.PitchShift = (float)Math.Pow(4.0F, trackPitch.Value / 13.5F);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopFullDuplex();
        }

        private void trackGain_Scroll(object sender, EventArgs e)
        {
            mDsp.GainDB = trackGain.Value;
        }

        private void trackGain_ValueChanged(object sender, EventArgs e)
        {
            mDsp.GainDB = trackGain.Value;
        }

        private void trackPitch_Scroll(object sender, EventArgs e)
        {
            SetPitchShiftValue();
        }

        private void trackPitch_ValueChanged(object sender, EventArgs e)
        {
            SetPitchShiftValue();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            trackGain.Value = 0;            
        }

        private void label2_Click(object sender, EventArgs e)
        {
            trackPitch.Value = 0;
        }

        private void chkAddMp3_CheckedChanged(object sender, EventArgs e)
        {
            if (mMixer != null)
            {
                if (chkAddMp3.Checked)
                {
                    mMp3 = CodecFactory.Instance.GetCodec("test.mp3").ToMono().ToSampleSource();
                    mMixer.AddSource(mMp3.ChangeSampleRate(mMixer.WaveFormat.SampleRate));
                }
                else
                {
                    mMixer.RemoveSource(mMp3);
                }
            } 
        }

        private void lblMic_Click(object sender, EventArgs e)
        {

        }

        private void bTnPlus_Click(object sender, EventArgs e)// Создание диапазонов
        {
            tbDiapPlus();
        }

        private void bTnMinus_Click(object sender, EventArgs e)
        {
            tbDiapMinus();
        }
        /*private void Diap()
        {
            Label num = new Label();
            Label lastnum = nums.LastOrDefault();
            Label label = new Label();
            Label label2 = new Label();
            Label lastLabel = labels.LastOrDefault();
            Label lastLabel2 = labels.LastOrDefault();
            TextBox newTextBox = new TextBox();
            TextBox newTextBox2 = new TextBox();
            TextBox lastOldTextBox = TextBoxes.LastOrDefault();
            TextBox lastOldTextBox2 = TextBoxes.LastOrDefault();
            label.AutoSize = true;
            label2.AutoSize = true;
            num.AutoSize = true;
            if (lastOldTextBox == null && lastOldTextBox2 == null && lastLabel == null && lastLabel2 == null && lastnum == null)
            {
                newTextBox.Location = new Point(100, 70);
                newTextBox2.Location = new Point(250, 70);
                label.Location = new Point(70, 70);
                label2.Location = new Point(220, 70);
                num.Location = new Point(50, 70);
                num.Text = "1.";
                label.Text = "from";
                label2.Text = "to";
            }
            else
            {
                newTextBox.Location = new Point(lastOldTextBox.Location.X - 150, lastOldTextBox.Location.Y + 30);
                newTextBox2.Location = new Point(lastOldTextBox2.Location.X, lastOldTextBox2.Location.Y + 30);
                label.Location = new Point(lastLabel.Location.X - 150, lastLabel.Location.Y + 30);
                label2.Location = new Point(lastLabel2.Location.X, lastLabel2.Location.Y + 30);
                label.Text = "from";
                label2.Text = "to";
                num.Location = new Point(lastnum.Location.X - 170, lastnum.Location.Y + 30);
                num.Text = $"{nums.Count}.";
            }
            //tbContr.SelectedIndex[0].Add(label);
            nums.Add(num);
            this.Controls.Add(num);
            labels.Add(label);
            this.Controls.Add(label);
            labels.Add(label2);
            this.Controls.Add(label2);
            TextBoxes.Add(newTextBox);
            Control.ControlCollection controls = this.Controls;
            controls.Add(newTextBox);
            TextBoxes.Add(newTextBox2);
            controls.Add(newTextBox2);
        }*/
        private void tbDiapPlus()
        {
            if (plusclick == 0)
            {
                bTnMinus.Enabled = true;
                tBxfrom1.Visible = true;
                tBxto1.Visible = true;
                lbFrom1.Visible = true;
                lbTo1.Visible = true;
                plusclick++;
            }
            else if (plusclick == 1)
            {
                lbFrom2.Visible = true;
                tBxfrom2.Visible = true;
                tBxto2.Visible = true;
                lbTo2.Visible = true;
                plusclick++;
            }
            else if (plusclick == 2)
            {
                lbFrom3.Visible = true;
                tBxfrom3.Visible = true;
                tBxto3.Visible = true;
                lbTo3.Visible = true;
                plusclick++;
            }
            else if (plusclick == 3)
            {
                lbFrom4.Visible = true;
                tBxfrom4.Visible = true;
                tBxto4.Visible = true;
                lbTo4.Visible = true;
                plusclick++;
            }
            else if (plusclick == 4)
            {
                lbFrom5.Visible = true;
                tBxfrom5.Visible = true;
                tBxto5.Visible = true;
                lbTo5.Visible = true;
                plusclick++;
            }
            else if (plusclick == 5)
            {
                lbFrom6.Visible = true;
                tBxfrom6.Visible = true;
                tBxto6.Visible = true;
                lbTo6.Visible = true;
                plusclick++;
            }
            else if (plusclick == 6)
            {
                lbFrom7.Visible = true;
                tBxfrom7.Visible = true;
                tBxto7.Visible = true;
                lbTo7.Visible = true;
                plusclick++;
            }
            else if (plusclick == 7)
            {
                lbFrom8.Visible = true;
                tBxfrom8.Visible = true;
                tBxto8.Visible = true;
                lbTo8.Visible = true;
                plusclick++;
            }
            else if (plusclick == 8)
            {
                lbFrom9.Visible = true;
                tBxfrom9.Visible = true;
                tBxto9.Visible = true;
                lbTo9.Visible = true;
                plusclick++;
            }
            else if (plusclick == 9)
            {
                lbFrom10.Visible = true;
                tBxfrom10.Visible = true;
                tBxto10.Visible = true;
                lbTo10.Visible = true;
                bTnPlus.Enabled = false;
            }
        }

        private void btnFix_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbDiapMinus()
        {
            if (plusclick == 9)
            {
                lbFrom10.Visible = false;
                tBxfrom10.Visible = false;
                tBxto10.Visible = false;
                lbTo10.Visible = false;
                bTnPlus.Enabled = true;
                plusclick--;
            }
            else if(plusclick == 8)
            {
                lbFrom9.Visible = false;
                tBxfrom9.Visible = false;
                tBxto9.Visible = false;
                lbTo9.Visible = false;
                plusclick--;
            }
            else if (plusclick == 7)
            {
                lbFrom8.Visible = false;
                tBxfrom8.Visible = false;
                tBxto8.Visible = false;
                lbTo8.Visible = false;
                plusclick--;
            }
            else if (plusclick == 6)
            {
                lbFrom7.Visible = false;
                tBxfrom7.Visible = false;
                tBxto7.Visible = false;
                lbTo7.Visible = false;
                plusclick--;
            }
            else if (plusclick == 5)
            {
                lbFrom6.Visible = false;
                tBxfrom6.Visible = false;
                tBxto6.Visible = false;
                lbTo6.Visible = false;
                plusclick--;
            }
            else if (plusclick == 4)
            {
                lbFrom5.Visible = false;
                tBxfrom5.Visible = false;
                tBxto5.Visible = false;
                lbTo5.Visible = false;
                plusclick--;
            }
            else if (plusclick == 3)
            {
                lbFrom4.Visible = false;
                tBxfrom4.Visible = false;
                tBxto4.Visible = false;
                lbTo4.Visible = false;
                plusclick--;
            }
            else if (plusclick == 2)
            {
                lbFrom3.Visible = false;
                tBxfrom3.Visible = false;
                tBxto3.Visible = false;
                lbTo3.Visible = false;
                plusclick--;
            }
            else if (plusclick == 1)
            {
                lbFrom2.Visible = false;
                tBxfrom2.Visible = false;
                tBxto2.Visible = false;
                lbTo2.Visible = false;
                plusclick--;
            }
            else if (plusclick == 0)
            {
                lbFrom1.Visible = false;
                tBxfrom1.Visible = false;
                tBxto1.Visible = false;
                lbTo1.Visible = false;
                bTnMinus.Enabled = false;
            }
        }
    }
}
