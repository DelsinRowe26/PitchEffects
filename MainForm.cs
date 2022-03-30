﻿using System;
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
using WinformsVisualization.Visualization;
using System.Drawing;

namespace PitchShifter
{
    public partial class MainForm : Form
    {
        //Глобальные переменныe
        private int strings = 0;
        private int tabIndex = 0;
        int[] min = new int[10];
        int[] max = new int[10];
        int[] minR = new int[10];
        int[] maxR = new int[10];
        int[] reverbTime = new int[10];
        int[] reverbHFRTR = new int[10];
        int plusclick = 0, plus = 0;

        private MMDeviceCollection mInputDevices;
        private MMDeviceCollection mOutputDevices;
        private WasapiCapture mSoundIn;
        private WasapiOut mSoundOut;
        private SampleDSP mDsp;
        private SimpleMixer mMixer;
        private ISampleSource mMp3;
        private IWaveSource mSource;
        private int SampleRate;
        private int SampleRate1 = 48000;

        private LineSpectrum mLineSpectrum;
        private VoicePrint3DSpectrum mVoicePrint;

        private readonly Bitmap mBitmap = new Bitmap(2000, 600);
        private int mXpos;
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public MainForm()
        {
            InitializeComponent();          
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
                mSoundIn = new WasapiCapture(/*false, AudioClientShareMode.Exclusive, 1*/);
                mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                mSoundIn.Initialize();
                

                if (cmbSelEff.SelectedIndex == 1)
                {
                    //Init DSP для смещения высоты тона
                    //var source = new SoundInSource(mSoundIn) { FillWithZeros = true };
                    for (int i = 0; i < plusclick; i++)
                    {
                        var source = BandPassFilter(mSoundIn, SampleRate1, min[i], max[i]);
                    
                        mDsp = new SampleDSP(source.ToStereo()/*ToSampleSource()*/);
                    
                        mDsp.GainDB = trackGain.Value;


                        SetPitchShiftValue();
                        SetupSampleSource(mDsp);
                        mSoundIn.Start();

                        Mixer();
                        //Добавляем наш источник звука в микшер

                        mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));//основная строка
                    }
                    SoundOut();

                    timer1.Start();
                    propertyGridBottom.SelectedObject = mVoicePrint;
                } 
                else if (cmbSelEff.SelectedIndex == 0)
                {
                    if (isDataValid(minR, maxR, reverbTime, reverbHFRTR, plusclick))
                    {
                        for (int i = 0; i < plusclick; i++)
                        {
                            var xsource = BandPassFilter(mSoundIn, SampleRate1, minR[i], maxR[i]);
                            if(reverbTime[i] != 0)
                            {
                                var reverb = new DmoWavesReverbEffect(xsource.ToWaveSource());
                                reverb.ReverbTime = reverbTime[i];
                                reverb.HighFrequencyRTRatio = ((float)reverbHFRTR[i]) / 1000;
                                xsource = reverb.ToSampleSource();
                            }
                            mDsp = new SampleDSP(xsource.ToStereo());
                            mDsp.GainDB = trackGain.Value;
                            SetPitchShiftValue();
                            SetupSampleSource(mDsp);
                            mSoundIn.Start();
                            Mixer();
                            mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));
                        }
                        
                        SoundOut();
                        timer1.Start();
                        propertyGridBottom.SelectedObject = mVoicePrint;
                    }
                }

                //Инициальный микшер
                
                //Запускает устройство воспроизведения звука с задержкой 1 мс.
                
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
                mSoundOut = new WasapiOut(/*false, AudioClientShareMode.Exclusive, 1*/);
                mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];
                mSoundOut.Initialize(mMixer.ToWaveSource(16));

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

        private void Mixer()
        {
            mMixer = new SimpleMixer(2, SampleRate) //стерео, 44,1 КГц
            {
                FillWithZeros = true,
                DivideResult = true, //Для этого установлено значение true, чтобы избежать звуков тиков из-за превышения -1 и 1.
            };
        }

        private void StopFullDuplex()//остановка всего
        {
            timer1.Stop();
            if (mSoundOut != null) mSoundOut.Dispose();
            if (mSoundIn != null) mSoundIn.Dispose();


        }

        private void SetupSampleSource(ISampleSource sampleSource)
        {
            const FftSize fftSize = FftSize.Fft4096;

            var spectrumProvider = new BasicSpectrumProvider(sampleSource.WaveFormat.Channels,
                sampleSource.WaveFormat.SampleRate, fftSize);

            mLineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                BarCount = 50,
                BarSpacing = 2,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt
            };

            mVoicePrint = new VoicePrint3DSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                PointCount = 200,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt
            };

            var notificationSource = new SingleBlockNotificationStream(sampleSource);

            notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);

            mSource = notificationSource.ToWaveSource(16);
        }

        private void GenerateVoice3DPrintSpectrum()
        {
            using (Graphics g = Graphics.FromImage(mBitmap))
            {
                pictureBoxBottom.Image = null;
                if (mVoicePrint.CreateVoicePrint3D(g, new RectangleF(0, 0, mBitmap.Width, mBitmap.Height),
                    mXpos, Color.Black, 3))
                {
                    mXpos += 3;
                    if (mXpos >= mBitmap.Width)
                        mXpos = 0;
                }
                pictureBoxBottom.Image = mBitmap;
            }
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
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopFullDuplex();
        }

        private void trackGain_Scroll(object sender, EventArgs e)
        {
            mDsp.GainDB = trackGain.Value;
            VolValue();
        }

        private void trackGain_ValueChanged(object sender, EventArgs e)
        {
            mDsp.GainDB = trackGain.Value;
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

        private void tbDiapPlus()//добавление текст боксов основная процедура
        {
            switch (plusclick)
            {
                case 0:
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
                    btnPitchVol1.Visible = true;
                    tbReverb1.Visible = true;
                    tbReverbHFRTR1.Visible = true;
                    lbReverb1.Visible = true;
                    tbFromReverb1.Visible =true;
                    lbFromReverb1.Visible = true;
                    lbToReverb1.Visible = true;
                    tbToReverb1.Visible = true;
                    lbReverb1.Visible = true;
                    plusclick++;
                    break;
                case 1:
                    lbFrom2.Visible = true;
                    tBxfrom2.Visible = true;
                    tBxto2.Visible = true;
                    lbTo2.Visible = true;
                    lbPitch2.Visible = true;
                    tbPitch2.Visible = true;
                    lbGain2.Visible = true;
                    tbGain2.Visible = true;
                    btnPitchVol2.Visible = true;
                    tbReverb2.Visible = true;
                    tbReverbHFRTR2.Visible = true;
                    tbFromReverb2.Visible = true;
                    lbFromReverb2.Visible = true;
                    lbToReverb2.Visible = true;
                    tbToReverb2.Visible = true;
                    lbReverb2.Visible = true;
                    plusclick++;
                    break;
                case 2:
                    lbFrom3.Visible = true;
                    tBxfrom3.Visible = true;
                    tBxto3.Visible = true;
                    lbTo3.Visible = true;
                    lbPitch3.Visible = true;
                    tbPitch3.Visible = true;
                    lbGain3.Visible = true;
                    tbGain3.Visible = true;
                    btnPitchVol3.Visible = true;
                    tbReverb3.Visible = true;
                    tbReverbHFRTR3.Visible = true;
                    tbFromReverb3.Visible = true;
                    lbFromReverb3.Visible = true;
                    lbToReverb3.Visible = true;
                    tbToReverb3.Visible = true;
                    lbReverb3.Visible = true;
                    plusclick++;
                    break;
                case 3:
                    lbFrom4.Visible = true;
                    tBxfrom4.Visible = true;
                    tBxto4.Visible = true;
                    lbTo4.Visible = true;
                    lbPitch4.Visible = true;
                    tbPitch4.Visible = true;
                    lbGain4.Visible = true;
                    tbGain4.Visible = true;
                    btnPitchVol4.Visible = true;
                    tbReverb4.Visible = true;
                    tbReverbHFRTR4.Visible = true;
                    tbFromReverb4.Visible = true;
                    lbFromReverb4.Visible = true;
                    lbToReverb4.Visible = true;
                    tbToReverb4.Visible = true;
                    lbReverb4.Visible = true;
                    plusclick++;
                    break;
                case 4:
                    lbFrom5.Visible = true;
                    tBxfrom5.Visible = true;
                    tBxto5.Visible = true;
                    lbTo5.Visible = true;
                    lbPitch5.Visible = true;
                    tbPitch5.Visible = true;
                    lbGain5.Visible = true;
                    tbGain5.Visible = true;
                    btnPitchVol5.Visible = true;
                    tbReverb5.Visible = true;
                    tbReverbHFRTR5.Visible = true;
                    tbFromReverb5.Visible = true;
                    lbFromReverb5.Visible = true;
                    lbToReverb5.Visible = true;
                    tbToReverb5.Visible = true;
                    lbReverb5.Visible = true;
                    plusclick++;
                    break;
                case 5:
                    lbFrom6.Visible = true;
                    tBxfrom6.Visible = true;
                    tBxto6.Visible = true;
                    lbTo6.Visible = true;
                    lbPitch6.Visible = true;
                    tbPitch6.Visible = true;
                    lbGain6.Visible = true;
                    tbGain6.Visible = true;
                    btnPitchVol6.Visible = true;
                    tbReverb6.Visible = true;
                    tbReverbHFRTR6.Visible = true;
                    tbFromReverb6.Visible = true;
                    lbFromReverb6.Visible = true;
                    lbToReverb6.Visible = true;
                    tbToReverb6.Visible = true;
                    lbReverb6.Visible = true;
                    plusclick++;
                    break;
                case 6:
                    lbFrom7.Visible = true;
                    tBxfrom7.Visible = true;
                    tBxto7.Visible = true;
                    lbTo7.Visible = true;
                    lbPitch7.Visible = true;
                    tbPitch7.Visible = true;
                    lbGain7.Visible = true;
                    tbGain7.Visible = true;
                    btnPitchVol7.Visible = true;
                    tbReverb7.Visible = true;
                    tbReverbHFRTR7.Visible = true;
                    tbFromReverb7.Visible = true;
                    lbFromReverb7.Visible = true;
                    lbToReverb7.Visible = true;
                    tbToReverb7.Visible = true;
                    lbReverb7.Visible = true;
                    plusclick++;
                    break;
                case 7:
                    lbFrom8.Visible = true;
                    tBxfrom8.Visible = true;
                    tBxto8.Visible = true;
                    lbTo8.Visible = true;
                    lbPitch8.Visible = true;
                    tbPitch8.Visible = true;
                    lbGain8.Visible = true;
                    tbGain8.Visible = true;
                    btnPitchVol8.Visible = true;
                    tbReverb8.Visible = true;
                    tbReverbHFRTR8.Visible = true;
                    tbFromReverb8.Visible = true;
                    lbFromReverb8.Visible = true;
                    lbToReverb8.Visible = true;
                    tbToReverb8.Visible = true;
                    lbReverb8.Visible = true;
                    plusclick++;
                    break;
                case 8:
                    lbFrom9.Visible = true;
                    tBxfrom9.Visible = true;
                    tBxto9.Visible = true;
                    lbTo9.Visible = true;
                    lbPitch9.Visible = true;
                    tbPitch9.Visible = true;
                    lbGain9.Visible = true;
                    tbGain9.Visible = true;
                    btnPitchVol9.Visible = true;
                    tbReverb9.Visible = true;
                    tbReverbHFRTR9.Visible = true;
                    tbFromReverb9.Visible = true;
                    lbFromReverb9.Visible = true;
                    lbToReverb9.Visible = true;
                    tbToReverb9.Visible = true;
                    lbReverb9.Visible = true;
                    plusclick++;
                    break;
                case 9:
                    lbFrom10.Visible = true;
                    tBxfrom10.Visible = true;
                    tBxto10.Visible = true;
                    lbTo10.Visible = true;
                    lbPitch10.Visible = true;
                    tbPitch10.Visible = true;
                    lbGain10.Visible = true;
                    tbGain10.Visible = true;
                    btnPitchVol10.Visible = true;
                    tbReverb10.Visible = true;
                    tbReverbHFRTR10.Visible = true;
                    tbFromReverb10.Visible = true;
                    lbFromReverb10.Visible = true;
                    lbToReverb10.Visible = true;
                    tbToReverb10.Visible = true;
                    lbReverb10.Visible = true;
                    bTnPlus.Enabled = false;
                    break;
            }
        }

        private void btnFix_Click(object sender, EventArgs e)//фиксация значений из текстбоксов
        {
                switch (plusclick)
                {
                    case 1:
                        reverbTime[0] = int.Parse(tbReverb1.Text);
                        reverbHFRTR[0] = int.Parse(tbReverbHFRTR1.Text);
                        min[0] = int.Parse(tBxfrom1.Text);
                        max[0] = int.Parse(tBxto1.Text);
                        minR[0] = int.Parse(tbFromReverb1.Text);
                        maxR[0] = int.Parse(tbToReverb1.Text);
                        PitchShifter.min[0] = int.Parse(tBxfrom1.Text);
                        PitchShifter.max[0] = int.Parse(tBxto1.Text);
                        PitchShifter.Vol[0] = int.Parse(tbGain1.Text);
                        break;
                    case 2:
                        reverbTime[1] = int.Parse(tbReverb2.Text);
                        reverbHFRTR[1] = int.Parse(tbReverbHFRTR2.Text);
                        min[1] = int.Parse(tBxfrom2.Text);
                        max[1] = int.Parse(tBxto2.Text);
                        minR[1] = int.Parse(tbFromReverb2.Text);
                        maxR[1] = int.Parse(tbToReverb2.Text);
                        PitchShifter.min[1] = int.Parse(tBxfrom2.Text);
                        PitchShifter.max[1] = int.Parse(tBxto2.Text);
                        PitchShifter.Vol[1] = int.Parse(tbGain2.Text);
                        break;
                    case 3:
                        reverbTime[2] = int.Parse(tbReverb3.Text);
                        reverbHFRTR[2] = int.Parse(tbReverbHFRTR3.Text);
                        min[2] = int.Parse(tBxfrom3.Text);
                        max[2] = int.Parse(tBxto3.Text);
                        minR[2] = int.Parse(tbFromReverb3.Text);
                        maxR[2] = int.Parse(tbToReverb3.Text);
                        PitchShifter.min[2] = int.Parse(tBxfrom3.Text);
                        PitchShifter.max[2] = int.Parse(tBxto3.Text);
                        PitchShifter.Vol[2] = int.Parse(tbGain3.Text);
                        break;
                    case 4:
                        reverbTime[3] = int.Parse(tbReverb4.Text);
                        reverbHFRTR[3] = int.Parse(tbReverbHFRTR4.Text);
                        min[3] = int.Parse(tBxfrom4.Text);
                        max[3] = int.Parse(tBxto4.Text);
                        minR[3] = int.Parse(tbFromReverb4.Text);
                        maxR[3] = int.Parse(tbToReverb4.Text);
                        PitchShifter.min[3] = int.Parse(tBxfrom4.Text);
                        PitchShifter.max[3] = int.Parse(tBxto4.Text);
                        PitchShifter.Vol[3] = int.Parse(tbGain4.Text);
                        break;
                    case 5:
                        reverbTime[4] = int.Parse(tbReverb5.Text);
                        reverbHFRTR[4] = int.Parse(tbReverbHFRTR5.Text);
                        min[4] = int.Parse(tBxfrom5.Text);
                        max[4] = int.Parse(tBxto5.Text);
                        minR[4] = int.Parse(tbFromReverb5.Text);
                        maxR[4] = int.Parse(tbToReverb5.Text);
                        PitchShifter.min[4] = int.Parse(tBxfrom5.Text);
                        PitchShifter.max[4] = int.Parse(tBxto5.Text);
                        PitchShifter.Vol[4] = int.Parse(tbGain5.Text);
                        break;
                    case 6:
                        reverbTime[5] = int.Parse(tbReverb6.Text);
                        reverbHFRTR[5] = int.Parse(tbReverbHFRTR6.Text);
                        min[5] = int.Parse(tBxfrom6.Text);
                        max[5] = int.Parse(tBxto6.Text);
                        minR[5] = int.Parse(tbFromReverb6.Text);
                        maxR[5] = int.Parse(tbToReverb6.Text);
                        PitchShifter.min[5] = int.Parse(tBxfrom6.Text);
                        PitchShifter.max[5] = int.Parse(tBxto6.Text);
                        PitchShifter.Vol[5] = int.Parse(tbGain6.Text);
                        break;
                    case 7:
                        reverbTime[6] = int.Parse(tbReverb7.Text);
                        reverbHFRTR[6] = int.Parse(tbReverbHFRTR7.Text);
                        min[6] = int.Parse(tBxfrom7.Text);
                        max[6] = int.Parse(tBxto7.Text);
                        minR[6] = int.Parse(tbFromReverb7.Text);
                        maxR[6] = int.Parse(tbToReverb7.Text);
                        PitchShifter.min[6] = int.Parse(tBxfrom7.Text);
                        PitchShifter.max[6] = int.Parse(tBxto7.Text);
                        PitchShifter.Vol[6] = int.Parse(tbGain7.Text);
                        break;
                    case 8:
                        reverbTime[7] = int.Parse(tbReverb8.Text);
                        reverbHFRTR[7] = int.Parse(tbReverbHFRTR8.Text);
                        min[7] = int.Parse(tBxfrom8.Text);
                        max[7] = int.Parse(tBxto8.Text);
                        minR[7] = int.Parse(tbFromReverb8.Text);
                        maxR[7] = int.Parse(tbToReverb8.Text);
                        PitchShifter.min[7] = int.Parse(tBxfrom8.Text);
                        PitchShifter.max[7] = int.Parse(tBxto8.Text);
                        PitchShifter.Vol[7] = int.Parse(tbGain8.Text);
                        break;
                    case 9 when plus == 0:
                        reverbTime[8] = int.Parse(tbReverb9.Text);
                        reverbHFRTR[8] = int.Parse(tbReverbHFRTR9.Text);
                        min[8] = int.Parse(tBxfrom9.Text);
                        max[8] = int.Parse(tBxto9.Text);
                        minR[8] = int.Parse(tbFromReverb9.Text);
                        maxR[8] = int.Parse(tbToReverb9.Text);
                        PitchShifter.min[8] = int.Parse(tBxfrom9.Text);
                        PitchShifter.max[8] = int.Parse(tBxto9.Text);
                        PitchShifter.Vol[8] = int.Parse(tbGain9.Text);
                        plus++;
                        break;
                    default:
                        if (plus == 1)
                        {
                            reverbTime[9] = int.Parse(tbReverb10.Text);
                            reverbHFRTR[9] = int.Parse(tbReverbHFRTR10.Text);
                            min[9] = int.Parse(tBxfrom10.Text);
                            max[9] = int.Parse(tBxto10.Text);
                            minR[9] = int.Parse(tbFromReverb10.Text);
                            maxR[9] = int.Parse(tbToReverb10.Text);
                            PitchShifter.min[9] = int.Parse(tBxfrom10.Text);
                            PitchShifter.max[9] = int.Parse(tBxto10.Text);
                            PitchShifter.Vol[9] = int.Parse(tbGain10.Text);
                            plus--;
                        }

                        break;
                }
        }

        private void bTnReset_Click(object sender, EventArgs e)//обнуление трэкбаров
        {
            trackGain.Value = 0;
            trackPitch.Value = 0;
            lbVolValue.Text = "0";
            lbPitchValue.Text = "0";
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
                tbReverb10.Visible = false;
                tbReverbHFRTR10.Visible = false;
                tbFromReverb10.Visible = false;
                lbFromReverb10.Visible = false;
                lbToReverb10.Visible = false;
                tbToReverb10.Visible = false;
                lbReverb10.Visible = false;
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
                tbReverb9.Visible = false;
                tbReverbHFRTR9.Visible = false;
                tbFromReverb9.Visible = false;
                lbFromReverb9.Visible = false;
                lbToReverb9.Visible = false;
                tbToReverb9.Visible = false;
                lbReverb9.Visible = false;
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
                tbReverb8.Visible = false;
                tbReverbHFRTR8.Visible = false;
                tbFromReverb8.Visible = false;
                lbFromReverb8.Visible = false;
                lbToReverb8.Visible = false;
                tbToReverb8.Visible = false;
                lbReverb8.Visible = false;
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
                tbReverb7.Visible = false;
                tbReverbHFRTR7.Visible = false;
                tbFromReverb7.Visible = false;
                lbFromReverb7.Visible = false;
                lbToReverb7.Visible = false;
                tbToReverb7.Visible = false;
                lbReverb7.Visible = false;
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
                tbReverb6.Visible = false;
                tbReverbHFRTR6.Visible = false;
                tbFromReverb6.Visible = false;
                lbFromReverb6.Visible = false;
                lbToReverb6.Visible = false;
                tbToReverb6.Visible = false;
                lbReverb6.Visible = false;
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
                tbReverb5.Visible = false;
                tbReverbHFRTR5.Visible = false;
                tbFromReverb5.Visible = false;
                lbFromReverb5.Visible = false;
                lbToReverb5.Visible = false;
                tbToReverb5.Visible = false;
                lbReverb5.Visible = false;
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
                tbReverb4.Visible = false;
                tbReverbHFRTR4.Visible = false;
                tbFromReverb4.Visible = false;
                lbFromReverb4.Visible = false;
                lbToReverb4.Visible = false;
                tbToReverb4.Visible = false;
                lbReverb4.Visible = false;
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
                tbReverb3.Visible = false;
                tbReverbHFRTR3.Visible = false;
                tbFromReverb3.Visible = false;
                lbFromReverb3.Visible = false;
                lbToReverb3.Visible = false;
                tbToReverb3.Visible = false;
                lbReverb3.Visible = false;
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
                tbReverb2.Visible = false;
                tbReverbHFRTR2.Visible = false;
                tbFromReverb2.Visible = false;
                lbFromReverb2.Visible = false;
                lbToReverb2.Visible = false;
                tbToReverb2.Visible = false;
                lbReverb2.Visible = false;
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
                tbReverb1.Visible = false;
                tbReverbHFRTR1.Visible = false;
                lbReverb1.Visible = false;
                tbFromReverb1.Visible = false;
                lbFromReverb1.Visible = false;
                lbToReverb1.Visible = false;
                tbToReverb1.Visible = false;
                lbReverb1.Visible = false;
                bTnMinus.Enabled = false;
                btnFix.Enabled = false;
                lbZnachPitch.Visible = false;
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

        /*private void PlusBtn_Click(object sender, EventArgs e)
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
        }*/

        private void cmbSampFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbSampFreq.SelectedIndex == 0)
            {
                SampleRate = 44100;
                PitchShifter.SampleRate2 = 22050;
                 //pitch.SampleRate = 44100;
            } else if(cmbSampFreq.SelectedIndex == 1)
            {
                SampleRate = 48000;
                PitchShifter.SampleRate2 = 24000;
                //pitch.SampleRate = 48000;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GenerateVoice3DPrintSpectrum();
        }

        /*private void AddString(string botFreq, string topFreq, string reverbTime, string reverbHFRTR)
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
        }*/

        private void Reverb()
        {
            if (plusclick != 0)
            {
                /*int[] botFreq = new int[strings];
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
                }*/

                
            }
            else
            {
                SoundOut();
                MessageBox.Show("Параметры не заданы");
            }
            if (isDataValid(minR, maxR, reverbTime, reverbHFRTR, plusclick))
            {
                Mixer();
                for (int i = 0; i < strings; i++)
                {
                    var x = BandPassFilter(mSoundIn, SampleRate, minR[i], maxR[i]);
                    if (reverbTime[i] != 0)
                    {
                        var reverb = new DmoWavesReverbEffect(x.ToWaveSource(16).ToStereo());
                        reverb.ReverbTime = reverbTime[i];
                        reverb.HighFrequencyRTRatio = ((float)reverbHFRTR[i]) / 1000;
                        x = reverb.ToSampleSource();
                    }
                    mDsp = new SampleDSP(x.ToStereo());//.ToWaveSource(16).ToSampleSource()
                    mDsp.GainDB = trackGain.Value;
                    mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));
                }
                SoundOut();
            }
        }
    }
}
