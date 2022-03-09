using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using CSCore;
using CSCore.SoundIn;//Вход звука
using CSCore.SoundOut;//Выход звука
using CSCore.CoreAudioAPI;
using CSCore.Streams;
using CSCore.Codecs;
using CSCore.Streams.Effects;
using CSCore.DSP;

namespace PitchShifter
{
    public partial class MainForm : Form
    {
        //Глобальные переменныe
        private int strings = 0;
        private int tabIndex = 0;
        private static float[] fftBuffer = new float[32000];
        int[] Pitch = new int[10];
        int[] Gain = new int[10];
        int[] min = new int[10];
        int[] max = new int[10];
        int plusclick = 0, plus = 0;


        private MMDeviceCollection mInputDevices;
        private MMDeviceCollection mOutputDevices;
        private WasapiCapture mSoundIn;
        private WasapiOut mSoundOut;
        private SampleDSP mDsp;
        private SimpleMixer mMixer;
        private ISampleSource mMp3;
        private readonly IWaveSource primarySource;
        private int SampleRate;
        private int SampleRate1 = 48000;

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public MainForm()
        {
            InitializeComponent();

            //Initialize WasapiCapture for recording
            mSoundIn = new WasapiCapture(true, AudioClientShareMode.Shared, 0);
            mSoundIn.Initialize();

            //Initialize soundout
            primarySource = new SoundInSource(mSoundIn) { FillWithZeros = true }
                        .ChangeSampleRate(SampleRate1).ToStereo();
            mSoundOut = new WasapiOut() { Latency = 1 };
            mSoundOut.Initialize(primarySource);
            
        }

        private void MainForm_Load(object sender, EventArgs e)//загрузка и определение микрофона и колонок
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
        private bool StartFullDuplex()//запуск пича и громкости
        {
            try
            {
                //Запускает устройство захвата звука с задержкой 1 мс.
                mSoundIn = new WasapiCapture(false, AudioClientShareMode.Exclusive, 1);
                mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                mSoundIn.Initialize();
                mSoundIn.Start();
                
                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };

                
                else if (cmbSelEff.SelectedIndex == 1)
                {
                    //Init DSP для смещения высоты тона
                    mDsp = new SampleDSP(source.ToSampleSource().ToStereo());
                    
                    mDsp.GainDB = trackGain.Value + 20;
                    //SetPitchShiftValue();
                    //Reverb();
                    SetPitchShiftValue();
                }

                //Инициальный микшер
                if (cmbSelEff.SelectedIndex == 1)
                {
                    mMixer = new SimpleMixer(2, SampleRate) //стерео, 44,1 КГц
                    {
                        FillWithZeros = false,
                        DivideResult = true, //Для этого установлено значение true, чтобы избежать звуков тиков из-за превышения -1 и 1.
                    };

                    //Добавляем наш источник звука в микшер
                
                    mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));//основная строка
                }

                //Запускает устройство воспроизведения звука с задержкой 1 мс.
                SoundOut();
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

        private void SoundOut()
        {
            if (cmbSelEff.SelectedIndex == 0)
            {
                mSoundOut = new WasapiOut() { Latency = 50 };
                mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];
                mSoundOut.Initialize(mMixer.ToWaveSource());

                mSoundOut.Play();
            }
            else if (cmbSelEff.SelectedIndex == 1)
            {
                mSoundOut = new WasapiOut(false, AudioClientShareMode.Exclusive, 1);
                mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];
                mSoundOut.Initialize(mMixer.ToWaveSource(16));

                //Start rolling!
                mSoundOut.Play();
            }
        }

        private void StopFullDuplex()//остановка всего
        {
            if (mSoundOut != null) mSoundOut.Dispose();
            if (mSoundIn != null) mSoundIn.Dispose();

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StopFullDuplex();
            if (StartFullDuplex())
            {
                mSoundIn.Start();
                mSoundOut.Play();
                trackGain.Enabled = true;
                trackPitch.Enabled = true;
                chkAddMp3.Enabled = true;
                bTnReset.Enabled = true;
            }
        }

        private void SetPitchShiftValue()//рассчеты и значения пича
        {
            mDsp.PitchShift = (float)Math.Pow(2.0F, trackPitch.Value / 13.0F);
            //await Task.Run(() => Reverb());
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopFullDuplex();
        }

        private void trackGain_Scroll(object sender, EventArgs e)
        {
            mDsp.GainDB = trackGain.Value + 20;
            VolValue();
        }

        private void trackGain_ValueChanged(object sender, EventArgs e)
        {
            mDsp.GainDB = trackGain.Value + 20;
        }

        private void trackPitch_Scroll(object sender, EventArgs e)
        {
            if (cmbSelEff.SelectedIndex == 1)
            {
                SetPitchShiftValue();
            }
            PitchValue();
        }

        private void trackPitch_ValueChanged(object sender, EventArgs e)
        {
            if (cmbSelEff.SelectedIndex == 1)
            {
                SetPitchShiftValue();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            trackGain.Value = 0;            
        }

        private void label2_Click(object sender, EventArgs e)
        {
            trackPitch.Value = 0;
        }

        private void chkAddMp3_CheckedChanged(object sender, EventArgs e)//чекбокс для включения музыки на фоне
        {
            if (mMixer != null)
            {
                if (chkAddMp3.Checked)
                {
                    mMp3 = CodecFactory.Instance.GetCodec("test.mp3").ToStereo().ToSampleSource();
                    mMixer.AddSource(mMp3.ChangeSampleRate(mMixer.WaveFormat.SampleRate));
                }
                else
                {
                    mMixer.RemoveSource(mMp3);
                }
            } 
        }

        private void bTnPlus_Click(object sender, EventArgs e)// Кнопка для создание текстбоксов диапазонов
        {
            tbDiapPlus();
        }

        private void bTnMinus_Click(object sender, EventArgs e)//удаление текстбоксов
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
        private void tbDiapPlus()//добавление текст боксов основная процедура
        {
            if (plusclick == 0)
            {
                bTnMinus.Enabled = true;
                tBxfrom1.Visible = true;
                tBxto1.Visible = true;
                lbFrom1.Visible = true;
                lbTo1.Visible = true;
                lbPitch1.Visible = true;
                tbPitch1.Visible = true;
                lbGain1.Visible = true;
                tbGain1.Visible = true;
                btnFix.Enabled = true;
                lbZnachPitch.Visible = true;
                ZnachVol.Visible = true;
                btnPitchVol1.Visible = true;
                plusclick++;
            }
            else if (plusclick == 1)
            {
                lbFrom2.Visible = true;
                tBxfrom2.Visible = true;
                tBxto2.Visible = true;
                lbTo2.Visible = true;
                lbPitch2.Visible = true;
                tbPitch2.Visible = true;
                lbGain2.Visible = true;
                tbGain2.Visible = true;
                btnPitchVol2.Visible = true;
                plusclick++;
            }
            else if (plusclick == 2)
            {
                lbFrom3.Visible = true;
                tBxfrom3.Visible = true;
                tBxto3.Visible = true;
                lbTo3.Visible = true;
                lbPitch3.Visible = true;
                tbPitch3.Visible = true;
                lbGain3.Visible = true;
                tbGain3.Visible = true;
                btnPitchVol3.Visible = true;
                plusclick++;
            }
            else if (plusclick == 3)
            {
                lbFrom4.Visible = true;
                tBxfrom4.Visible = true;
                tBxto4.Visible = true;
                lbTo4.Visible = true;
                lbPitch4.Visible = true;
                tbPitch4.Visible = true;
                lbGain4.Visible = true;
                tbGain4.Visible = true;
                btnPitchVol4.Visible = true;
                plusclick++;
            }
            else if (plusclick == 4)
            {
                lbFrom5.Visible = true;
                tBxfrom5.Visible = true;
                tBxto5.Visible = true;
                lbTo5.Visible = true;
                lbPitch5.Visible = true;
                tbPitch5.Visible = true;
                lbGain5.Visible = true;
                tbGain5.Visible = true;
                btnPitchVol5.Visible = true;
                plusclick++;
            }
            else if (plusclick == 5)
            {
                lbFrom6.Visible = true;
                tBxfrom6.Visible = true;
                tBxto6.Visible = true;
                lbTo6.Visible = true;
                lbPitch6.Visible = true;
                tbPitch6.Visible = true;
                lbGain6.Visible = true;
                tbGain6.Visible = true;
                btnPitchVol6.Visible = true;
                plusclick++;
            }
            else if (plusclick == 6)
            {
                lbFrom7.Visible = true;
                tBxfrom7.Visible = true;
                tBxto7.Visible = true;
                lbTo7.Visible = true;
                lbPitch7.Visible = true;
                tbPitch7.Visible = true;
                lbGain7.Visible = true;
                tbGain7.Visible = true;
                btnPitchVol7.Visible = true;
                plusclick++;
            }
            else if (plusclick == 7)
            {
                lbFrom8.Visible = true;
                tBxfrom8.Visible = true;
                tBxto8.Visible = true;
                lbTo8.Visible = true;
                lbPitch8.Visible = true;
                tbPitch8.Visible = true;
                lbGain8.Visible = true;
                tbGain8.Visible = true;
                btnPitchVol8.Visible = true;
                plusclick++;
            }
            else if (plusclick == 8)
            {
                lbFrom9.Visible = true;
                tBxfrom9.Visible = true;
                tBxto9.Visible = true;
                lbTo9.Visible = true;
                lbPitch9.Visible = true;
                tbPitch9.Visible = true;
                lbGain9.Visible = true;
                tbGain9.Visible = true;
                btnPitchVol9.Visible = true;
                plusclick++;
            }
            else if (plusclick == 9)
            {
                lbFrom10.Visible = true;
                tBxfrom10.Visible = true;
                tBxto10.Visible = true;
                lbTo10.Visible = true;
                lbPitch10.Visible = true;
                tbPitch10.Visible = true;
                lbGain10.Visible = true;
                tbGain10.Visible = true;
                btnPitchVol10.Visible = true;
                bTnPlus.Enabled = false;
            }
        }

        private void btnFix_Click(object sender, EventArgs e)//фиксация значений из текстбоксов
        {
            for (int i = 0; i < min.Length; i++)
            {
                switch (plusclick)
                {
                    case 1:
                        min[0] = int.Parse(tBxfrom1.Text);
                        max[0] = int.Parse(tBxto1.Text);
                        Pitch[0] = int.Parse(tbPitch1.Text);
                        Gain[0] = int.Parse(tbGain1.Text);
                        break;
                    case 2:
                        min[1] = int.Parse(tBxfrom2.Text);
                        max[1] = int.Parse(tBxto2.Text);
                        Pitch[1] = int.Parse(tbPitch2.Text);
                        Gain[1] = int.Parse(tbGain2.Text);
                        break;
                    case 3:
                        min[2] = int.Parse(tBxfrom3.Text);
                        max[2] = int.Parse(tBxto3.Text);
                        Pitch[2] = int.Parse(tbPitch3.Text);
                        Gain[2] = int.Parse(tbGain3.Text);
                        break;
                    case 4:
                        min[3] = int.Parse(tBxfrom4.Text);
                        max[3] = int.Parse(tBxto4.Text);
                        Pitch[3] = int.Parse(tbPitch4.Text);
                        Gain[3] = int.Parse(tbGain4.Text);
                        break;
                    case 5:
                        min[4] = int.Parse(tBxfrom5.Text);
                        max[4] = int.Parse(tBxto5.Text);
                        Pitch[4] = int.Parse(tbPitch5.Text);
                        Gain[4] = int.Parse(tbGain5.Text);
                        break;
                    case 6:
                        min[5] = int.Parse(tBxfrom6.Text);
                        max[5] = int.Parse(tBxto6.Text);
                        Pitch[5] = int.Parse(tbPitch6.Text);
                        Gain[5] = int.Parse(tbGain6.Text);
                        break;
                    case 7:
                        min[6] = int.Parse(tBxfrom7.Text);
                        max[6] = int.Parse(tBxto7.Text);
                        Pitch[6] = int.Parse(tbPitch7.Text);
                        Gain[6] = int.Parse(tbGain7.Text);
                        break;
                    case 8:
                        min[7] = int.Parse(tBxfrom8.Text);
                        max[7] = int.Parse(tBxto8.Text);
                        Pitch[7] = int.Parse(tbPitch8.Text);
                        Gain[7] = int.Parse(tbGain8.Text);
                        break;
                    case 9 when plus == 0:
                        min[8] = int.Parse(tBxfrom9.Text);
                        max[8] = int.Parse(tBxto9.Text);
                        Pitch[8] = int.Parse(tbPitch9.Text);
                        Gain[8] = int.Parse(tbGain9.Text);
                        plus++;
                        break;
                    default:
                        if (plus == 1)
                        {
                            min[9] = int.Parse(tBxfrom10.Text);
                            max[9] = int.Parse(tBxto10.Text);
                            Pitch[9] = int.Parse(tbPitch10.Text);
                            Gain[9] = int.Parse(tbGain10.Text);
                            plus--;
                        }

                        break;
                }
            }
        }

        private void bTnReset_Click(object sender, EventArgs e)//обнуление трэкбаров
        {
            trackGain.Value = 0;
            trackPitch.Value = 0;
            lbVolValue.Text = "0";
            lbPitchValue.Text = "0";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //int.Parse(mSoundIn.WaveFormat.SampleRate);
            //wave = (int)Math.Pow(2.0F, mSoundIn.WaveFormat.SampleRate / 13.0F);
            //textBox1.Text = audio.GetFrequencyNative(out long source).ToString();
            //textBox1.Text = mMixer.WaveFormat.ToString();
            //textBox1.Text = mDsp.Length.ToString();
            //textBox1.Text = fftBuffer.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trackGain.Value = 1;
            trackPitch.Value = 6;
            lbVolValue.Text = "1";
            lbPitchValue.Text = "6";
        }

        private void PitchValue()
        {
            lbPitchValue.Text = trackPitch.Value.ToString();
        }

        private void VolValue()
        {
            lbVolValue.Text = trackGain.Value.ToString();
        }

        private void btnPitchVol1_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch1.Text);
            lbPitchValue.Text = tbPitch1.Text;
            trackGain.Value = int.Parse(tbGain1.Text);
            lbVolValue.Text = tbGain1.Text;
        }

        private void btnPitchVol2_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch2.Text);
            lbPitchValue.Text = tbPitch2.Text;
            trackGain.Value = int.Parse(tbGain2.Text);
            lbVolValue.Text = tbGain2.Text;
        }

        private void btnPitchVol3_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch3.Text);
            lbPitchValue.Text = tbPitch3.Text;
            trackGain.Value = int.Parse(tbGain3.Text);
            lbVolValue.Text = tbGain3.Text;
        }

        private void btnPitchVol4_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch4.Text);
            lbPitchValue.Text = tbPitch4.Text;
            trackGain.Value = int.Parse(tbGain4.Text);
            lbVolValue.Text = tbGain4.Text;
        }

        private void btnPitchVol5_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch5.Text);
            lbPitchValue.Text = tbPitch5.Text;
            trackGain.Value = int.Parse(tbGain5.Text);
            lbVolValue.Text = tbGain5.Text;
        }

        private void btnPitchVol6_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch6.Text);
            lbPitchValue.Text = tbPitch6.Text;
            trackGain.Value = int.Parse(tbGain6.Text);
            lbVolValue.Text = tbGain6.Text;
        }

        private void btnPitchVol7_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch7.Text);
            lbPitchValue.Text = tbPitch7.Text;
            trackGain.Value = int.Parse(tbGain7.Text);
            lbVolValue.Text = tbGain7.Text;
        }

        private void btnPitchVol8_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch8.Text);
            lbPitchValue.Text = tbPitch8.Text;
            trackGain.Value = int.Parse(tbGain8.Text);
            lbVolValue.Text = tbGain8.Text;
        }

        private void btnPitchVol9_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch9.Text);
            lbPitchValue.Text = tbPitch9.Text;
            trackGain.Value = int.Parse(tbGain9.Text);
            lbVolValue.Text = tbGain9.Text;
        }

        private void btnPitchVol10_Click(object sender, EventArgs e)
        {
            trackPitch.Value = int.Parse(tbPitch10.Text);
            lbPitchValue.Text = tbPitch10.Text;
            trackGain.Value = int.Parse(tbGain10.Text);
            lbVolValue.Text = tbGain10.Text;
        }

        private void tbDiapMinus()//процедура удаления текстбоксов
        {
            if (plusclick == 9)
            {
                lbFrom10.Visible = false;
                tBxfrom10.Visible = false;
                tBxto10.Visible = false;
                lbTo10.Visible = false;
                lbPitch10.Visible = false;
                tbPitch10.Visible = false;
                lbGain10.Visible = false;
                tbGain10.Visible = false;
                bTnPlus.Enabled = true;
                btnPitchVol10.Visible = false;
                plusclick--;
            }
            else if(plusclick == 8)
            {
                lbFrom9.Visible = false;
                tBxfrom9.Visible = false;
                tBxto9.Visible = false;
                lbTo9.Visible = false;
                lbPitch9.Visible = false;
                tbPitch9.Visible = false;
                lbGain9.Visible = false;
                tbGain9.Visible = false;
                btnPitchVol9.Visible = false;
                plusclick--;
            }
            else if (plusclick == 7)
            {
                lbFrom8.Visible = false;
                tBxfrom8.Visible = false;
                tBxto8.Visible = false;
                lbTo8.Visible = false;
                lbPitch8.Visible = false;
                tbPitch8.Visible = false;
                lbGain8.Visible = false;
                tbGain8.Visible = false;
                btnPitchVol8.Visible = false;
                plusclick--;
            }
            else if (plusclick == 6)
            {
                lbFrom7.Visible = false;
                tBxfrom7.Visible = false;
                tBxto7.Visible = false;
                lbTo7.Visible = false;
                lbPitch7.Visible = false;
                tbPitch7.Visible = false;
                lbGain7.Visible = false;
                tbGain7.Visible = false;
                btnPitchVol7.Visible = false;
                plusclick--;
            }
            else if (plusclick == 5)
            {
                lbFrom6.Visible = false;
                tBxfrom6.Visible = false;
                tBxto6.Visible = false;
                lbTo6.Visible = false;
                lbPitch6.Visible = false;
                tbPitch6.Visible = false;
                lbGain6.Visible = false;
                tbGain6.Visible = false;
                btnPitchVol6.Visible = false;
                plusclick--;
            }
            else if (plusclick == 4)
            {
                lbFrom5.Visible = false;
                tBxfrom5.Visible = false;
                tBxto5.Visible = false;
                lbTo5.Visible = false;
                lbPitch5.Visible = false;
                tbPitch5.Visible = false;
                lbGain5.Visible = false;
                tbGain5.Visible = false;
                btnPitchVol5.Visible = false;
                plusclick--;
            }
            else if (plusclick == 3)
            {
                lbFrom4.Visible = false;
                tBxfrom4.Visible = false;
                tBxto4.Visible = false;
                lbTo4.Visible = false;
                lbPitch4.Visible = false;
                tbPitch4.Visible = false;
                lbGain4.Visible = false;
                tbGain4.Visible = false;
                btnPitchVol4.Visible = false;
                plusclick--;
            }
            else if (plusclick == 2)
            {
                lbFrom3.Visible = false;
                tBxfrom3.Visible = false;
                tBxto3.Visible = false;
                lbTo3.Visible = false;
                lbPitch3.Visible = false;
                tbPitch3.Visible = false;
                lbGain3.Visible = false;
                tbGain3.Visible = false;
                btnPitchVol3.Visible = false;
                plusclick--;
            }
            else if (plusclick == 1)
            {
                lbFrom2.Visible = false;
                tBxfrom2.Visible = false;
                tBxto2.Visible = false;
                lbTo2.Visible = false;
                lbPitch2.Visible = false;
                tbPitch2.Visible = false;
                lbGain2.Visible = false;
                tbGain2.Visible = false;
                btnPitchVol2.Visible = false;
                plusclick--;
            }
            else if (plusclick == 0)
            {
                lbFrom1.Visible = false;
                tBxfrom1.Visible = false;
                tBxto1.Visible = false;
                lbTo1.Visible = false;
                lbPitch1.Visible = false;
                tbPitch1.Visible = false;
                lbGain1.Visible = false;
                tbGain1.Visible = false;
                btnPitchVol1.Visible = false;
                bTnMinus.Enabled = false;
                btnFix.Enabled = false;
                lbZnachPitch.Visible = false;
                ZnachVol.Visible = false;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)//просто кнопка остановки
        {
            StopFullDuplex();
            trackGain.Enabled = false;
            trackPitch.Enabled = false;
            chkAddMp3.Enabled = false;
            bTnReset.Enabled = false;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Reverb();
        }

        public ISampleSource BandPassFilter(WasapiCapture mSoundIn, int sampleRate, int bottomFreq, int topFreq)
        {
            var sampleSource = new SoundInSource(mSoundIn) { FillWithZeros = true }
                    .ChangeSampleRate(sampleRate).ToStereo().ToSampleSource();

            var tempFilter = sampleSource.AppendSource(x => new BiQuadFilterSource(x));
            tempFilter.Filter = new HighpassFilter(sampleRate, bottomFreq);
            var filteredSource = tempFilter.AppendSource(x => new BiQuadFilterSource(x));
            filteredSource.Filter = new LowpassFilter(sampleRate, topFreq);

            return filteredSource;
        }

        public bool isDataValid(int[] botFreq, int[] topFreq, int[] reverbTime, int[] reverbHFRTR, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (botFreq[i] > SampleRate / 2 || topFreq[i] > SampleRate / 2)
                {
                    MessageBox.Show("Частоты полосового фильтра не могут быть больше половины частоты дискретизации ("
                        + Convert.ToString(SampleRate / 2) + ")");
                    return false;
                }
                if (reverbTime[i] > 3000)
                {
                    MessageBox.Show("Время реверберации не может быть выше 3000мс");
                    return false;
                }
                if (botFreq[i] >= topFreq[i])
                {
                    MessageBox.Show("Нижняя частота полосового фильтра должна быть меньше верхней");
                    return false;
                }
                if (reverbHFRTR[i] > 999 || reverbHFRTR[i] < 1)
                {
                    MessageBox.Show("Не может быть выше 999 и ниже 1");
                    return false;
                }
            }
            return true;
        }

        private void PlusBtn_Click(object sender, EventArgs e)
        {
            AddString("0", "0", "0", "1");
        }

        private void MinusBtn_Click(object sender, EventArgs e)
        {
            if (strings != 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    //tabPage1.Controls[tabPage1.Controls.Count - 1].Dispose();
                    tabPage1.Controls.RemoveAt(tabPage1.Controls.Count - 1);
                }
                strings--;
                tabIndex -= 8;
            }
        }

        private void cmbSampFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbSampFreq.SelectedIndex == 0)
            {
                SampleRate = 44100;
            } else if(cmbSampFreq.SelectedIndex == 1)
            {
                SampleRate = 48000;
            }
        }

        private void AddString(string botFreq, string topFreq, string reverbTime, string reverbHFRTR)
        {
            Label lbNumber = new Label();
            lbNumber.AutoSize = false;
            lbNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            lbNumber.Location = new System.Drawing.Point(5, 5 + (strings * 25));
            lbNumber.Name = "lbNumber" + Convert.ToString(strings + 1);
            lbNumber.Size = new System.Drawing.Size(25, 15);
            lbNumber.TabIndex = tabIndex++;
            lbNumber.Text = Convert.ToString(strings + 1) + ".";
            tabPage1.Controls.Add(lbNumber);

            Label lbFrom = new Label();
            lbFrom.AutoSize = false;
            lbFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            lbFrom.Location = new System.Drawing.Point(35, 5 + (strings * 25));
            lbFrom.Name = "lbFrom" + Convert.ToString(strings + 1);
            lbFrom.Size = new System.Drawing.Size(40, 15);
            lbFrom.TabIndex = tabIndex++;
            lbFrom.Text = "From";
            tabPage1.Controls.Add(lbFrom);

            TextBox tbBotFreq = new TextBox();
            tbBotFreq.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            tbBotFreq.Location = new System.Drawing.Point(80, 5 + (strings * 25));
            tbBotFreq.Name = "tbBotFreq" + Convert.ToString(strings + 1);
            tbBotFreq.Size = new System.Drawing.Size(45, 15);
            tbBotFreq.TabIndex = tabIndex++;
            tbBotFreq.Text = botFreq;
            tabPage1.Controls.Add(tbBotFreq);

            Label lbTo = new Label();
            lbTo.AutoSize = false;
            lbTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            lbTo.Location = new System.Drawing.Point(130, 5 + (strings * 25));
            lbTo.Name = "lbTo" + Convert.ToString(strings + 1);
            lbTo.Size = new System.Drawing.Size(25, 15);
            lbTo.TabIndex = tabIndex++;
            lbTo.Text = "To";
            tabPage1.Controls.Add(lbTo);

            TextBox tbTopFreq = new TextBox();
            tbTopFreq.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            tbTopFreq.Location = new System.Drawing.Point(165, 5 + (strings * 25));
            tbTopFreq.Name = "tbTopFreq" + Convert.ToString(strings + 1);
            tbTopFreq.Size = new System.Drawing.Size(45, 15);
            tbTopFreq.TabIndex = tabIndex++;
            tbTopFreq.Text = topFreq;
            tabPage1.Controls.Add(tbTopFreq);

            Label lbReverb = new Label();
            lbReverb.AutoSize = false;
            lbReverb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            lbReverb.Location = new System.Drawing.Point(215, 5 + (strings * 25));
            lbReverb.Name = "lbReverb" + Convert.ToString(strings + 1);
            lbReverb.Size = new System.Drawing.Size(55, 15);
            lbReverb.TabIndex = tabIndex++;
            lbReverb.Text = "Reverb";
            tabPage1.Controls.Add(lbReverb);

            TextBox tbReverb = new TextBox();
            tbReverb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            tbReverb.Location = new System.Drawing.Point(275, 5 + (strings * 25));
            tbReverb.Name = "tbReverb" + Convert.ToString(strings + 1);
            tbReverb.Size = new System.Drawing.Size(45, 15);
            tbReverb.TabIndex = tabIndex++;
            tbReverb.Text = reverbTime;
            tabPage1.Controls.Add(tbReverb);

            TextBox tbReverbHFRTR = new TextBox();
            tbReverbHFRTR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            tbReverbHFRTR.Location = new System.Drawing.Point(335, 5 + (strings * 25));
            tbReverbHFRTR.Name = "tbReverbHFRTR" + Convert.ToString(strings + 1);
            tbReverbHFRTR.Size = new System.Drawing.Size(45, 15);
            tbReverbHFRTR.TabIndex = tabIndex++;
            tbReverbHFRTR.Text = reverbHFRTR;
            tabPage1.Controls.Add(tbReverbHFRTR);

            strings++;
        }

        private void Reverb()
        {
            if (strings != 0)
            {
                int[] botFreq = new int[strings];
                int[] topFreq = new int[strings];
                int[] reverbTime = new int[strings];
                int[] reverbHFRTR = new int[strings];
                int j = 0, c = 0;
                for (int i = 0; i < tabPage1.Controls.Count; i++)
                {
                    if (tabPage1.Controls[i] is TextBox)
                    {
                        uint n;
                        if (uint.TryParse(tabPage1.Controls[i].Text, out n))
                        {
                            c++;
                            switch (c % 4)
                            {
                                case 1:
                                    botFreq[j] = (int)n;
                                    break;
                                case 2:
                                    topFreq[j] = (int)n;
                                    break;
                                case 3:
                                    reverbTime[j] = (int)n;
                                    break;
                                default:
                                    reverbHFRTR[j] = (int)n;
                                    j++;
                                    break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверные данные");
                        }
                    }
                }

                if (isDataValid(botFreq, topFreq, reverbTime, reverbHFRTR, strings))
                {
                    mMixer = new SimpleMixer(2, SampleRate) //стерео, 44,1 КГц
                    {
                        FillWithZeros = true,
                        DivideResult = true, //Для этого установлено значение true, чтобы избежать звуков тиков из-за превышения -1 и 1.
                    };
                    mMixer.Dispose();
                    for (int i = 0; i < strings; i++)
                    {
                        var x = BandPassFilter(mSoundIn, SampleRate, botFreq[i], topFreq[i]);
                        if (reverbTime[i] != 0)
                        {
                            var reverb = new DmoWavesReverbEffect(x.ToWaveSource());
                            reverb.ReverbTime = reverbTime[i];
                            reverb.HighFrequencyRTRatio = ((float)reverbHFRTR[i]) / 1000;
                            x = reverb.ToSampleSource();
                        }
                        mMixer.AddSource(x);
                    }
                    SoundOut();
                }
            }
            else
            {
                SoundOut();
                MessageBox.Show("Параметры не заданы");
            }
        }
    }
}
