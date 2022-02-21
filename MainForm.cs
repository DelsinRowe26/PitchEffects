using System;
using System.Windows.Forms;
using System.Diagnostics;
using CSCore;
using CSCore.SoundIn;//Вход звука
using CSCore.SoundOut;//Выход звука
using CSCore.CoreAudioAPI;
using CSCore.Streams;
using CSCore.Streams.Effects;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.Utils;
//using System.Numerics;
using WinformsVisualization.Visualization;

/*using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;*/

using CSCore.SoundOut.MMInterop;
using System.Drawing;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.Linq;

namespace PitchShifter
{
    public partial class MainForm : Form
    {
        AudioLevelMonitor audioMonitor;
        //Глобальные переменныe
        private static float[] fftBuffer = new float[32000];
        private static long gRover, gInit;
        int[] Pitch = new int[10];
        int[] Gain = new int[10];
        int[] min = new int[10];
        int[] max = new int[10];
        int plusclick = 0, plus = 0;
        private MMDeviceCollection mInputDevices;
        private MMDeviceCollection mOutputDevices;
        private WasapiCapture mSoundIn;
        private CSCore.SoundOut.WasapiOut mSoundOut;
        private WaveIn waveIn;
        private SampleDSP mDsp;
        private Complex[] data;
        private FftSize fftSize;
        private SoundInSource sound;
        private Equalizer filter;
        private AudioClock audio;
        private FftProvider fftProvider;
        //private CSCore.WaveFormat format;
        //private MusicPlayer vSab = new MusicPlayer();
        private SimpleMixer mMixer;
        private ISampleSource mMp3;
        private readonly Bitmap _bitmap = new Bitmap(2000, 600);
        private int _xpos;
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public MainForm()
        {
            InitializeComponent();
            audioMonitor = new AudioLevelMonitor();
        }

        /*public static void Main(string[] args)
        {
            int sampleRate = 44100;
            short[] data = new short[sampleRate];
            double frequency = Math.PI * 2 * 20000.0 / sampleRate;
            for(int index = 0; index < sampleRate; index++)
            {
                data[index] = (short)(Sine(index, frequency) * Length(-0.0015, frequency, index, 1.0, sampleRate) * short.MaxValue);
            }
        }*/

        public static double Length(double compressor, double frequency, double position, double length, int sampleRate)//какая-то хрень из интернета
        {
            return Math.Exp(((compressor / sampleRate) * frequency * sampleRate * (position / sampleRate)) / (length / sampleRate));
        }

        public static double Sine(int index, double frequency)//!!!!!!!!!!!!!!!!!!!!!!!!!Хрень понадобится лезь, не понадобится не лезь.
        {
            return Math.Sin(index * frequency);
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
            long offset = 1, sampleCount = 1, osamp, i, k, qpd, index, inFifoLatency, stepSize, fftFrameSize2;
            float[] indata = new float[4096];
            double magn, phase, tmp, window, real, imag;
            double freqPerBin, expct;

            float[] outdata = indata;
            fftFrameSize2 = 2048;
            stepSize = 4096 / 48000;
            freqPerBin = 48000 / (double)4096;
            expct = 2.0 * Math.PI * (double)stepSize / (double)4096;
            inFifoLatency = 4096 - stepSize;
            if (gRover == 0) gRover = inFifoLatency;

            try
            {
                //Запускает устройство захвата звука с задержкой 1 мс.
                mSoundIn = new WasapiCapture(false, AudioClientShareMode.Exclusive, 1);
                mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                mSoundIn.Initialize();
                mSoundIn.Start();
                
                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };

                for(i = offset; i < sampleCount; i++)
                {

                }
                ShortTimeFourierTransform(fftBuffer, 4096, -1);

                

                ShortTimeFourierTransform(fftBuffer, 4096, 1);

                //Init DSP для смещения высоты тона
                mDsp = new SampleDSP(source.ToSampleSource().ToStereo());
                mDsp.GainDB = trackGain.Value + 20;
                SetPitchShiftValue();

                //Инициальный микшер
                mMixer = new SimpleMixer(2, 48000) //стерео, 44,1 КГц
                {
                    FillWithZeros = false,
                    DivideResult = true, //Для этого установлено значение true, чтобы избежать звуков тиков из-за превышения -1 и 1.
                };

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 1 мс.
                mSoundOut = new WasapiOut(false, AudioClientShareMode.Exclusive, 1);
                mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];
                mSoundOut.Initialize(mMixer.ToWaveSource(16));

                /*int sampleRate = 48000;
                short[] data = new short[sampleRate];
                double frequency = Math.PI * 2 * 440.0 / mSoundIn.WaveFormat.SampleRate;
                for (int index = 0; index < sampleRate; index++)
                {
                    data[index] = (short)(Sine(index, frequency) * Length(-0.0015, frequency, index, 1.0, sampleRate) * short.MaxValue);
                }*/

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
                trackGain.Enabled = true;
                trackPitch.Enabled = true;
                chkAddMp3.Enabled = true;
                bTnReset.Enabled = true;
            }
        }

        private void SetPitchShiftValue()//рассчеты и значения пича
        {
            mDsp.PitchShift = (float)Math.Pow(2.0F, trackPitch.Value / 13.0F);
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
            SetPitchShiftValue();
            PitchValue();
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
            textBox1.Text = fftBuffer.ToString();
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

        /*public static Complex[] nfft(Complex[] X)//Взятое из интернета БПФ 
        {
            int N = X.Length;
            Complex[] X_n = new Complex[N];
            for (int i = 0; i < N / 2; i++)
            {
                X_n[i] = X[N / 2 + i];
                X_n[N / 2 + i] = X[i];
            }
            return X_n;
        }

        private static Complex w(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }
        public static Complex[] fft(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = fft(x_even);
                Complex[] X_odd = fft(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + w(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                }
            }
            return X;
        }*/

        public static void ShortTimeFourierTransform(float[] fftBuffer, long fftFrameSize, long sign)
        {
            float wr, wi, arg, temp;
            float tr, ti, ur, ui;
            long i, bitm, j, le, le2, k;

            for (i = 2; i < 2 * fftFrameSize - 2; i += 2)
            {
                for (bitm = 2, j = 0; bitm < 2 * fftFrameSize; bitm <<= 1)
                {
                    if ((i & bitm) != 0) j++;
                    j <<= 1;
                }
                if (i < j)
                {
                    temp = fftBuffer[i];
                    fftBuffer[i] = fftBuffer[j];
                    fftBuffer[j] = temp;
                    temp = fftBuffer[i + 1];
                    fftBuffer[i + 1] = fftBuffer[j + 1];
                    fftBuffer[j + 1] = temp;
                }
            }
            long max = (long)(Math.Log(fftFrameSize) / Math.Log(2.0) + .5);
            for (k = 0, le = 2; k < max; k++)
            {
                le <<= 1;
                le2 = le >> 1;
                ur = 1.0F;
                ui = 0.0F;
                arg = (float)Math.PI / (le2 >> 1);
                wr = (float)Math.Cos(arg);
                wi = (float)(sign * Math.Sin(arg));
                for (j = 0; j < le2; j += 2)
                {

                    for (i = j; i < 2 * fftFrameSize; i += le)
                    {
                        tr = fftBuffer[i + le2] * ur - fftBuffer[i + le2 + 1] * ui;
                        ti = fftBuffer[i + le2] * ui + fftBuffer[i + le2 + 1] * ur;
                        fftBuffer[i + le2] = fftBuffer[i] - tr;
                        fftBuffer[i + le2 + 1] = fftBuffer[i + 1] - ti;
                        fftBuffer[i] += tr;
                        fftBuffer[i + 1] += ti;

                    }
                    tr = ur * wr - ui * wi;
                    ui = ur * wi + ui * wr;
                    ur = tr;
                }
            }
        }
        /*public static Complex[] FftResultBuffer(int k, int N)
        {
            
        }*/
    }
}
