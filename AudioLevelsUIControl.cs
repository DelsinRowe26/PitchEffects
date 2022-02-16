// Copyright (C) 2017 by David W. Jeske
// Released to the Public Domain

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PitchShifter
{
    class AudioLevelsUIControl : Control
    {
        AudioLevelMonitor _audioMonitor;
        List<Pen> pens = new List<Pen>();
        Dictionary<string, Pen> _sessionIdToPen = new Dictionary<string, Pen>();
        Timer dispatcherTimer;
        Pen greenPen = new Pen(Brushes.Green, 0.5f);


        public AudioLevelsUIControl() {
            DoubleBuffered = true;
            dispatcherTimer = new Timer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = 100;            
            dispatcherTimer.Start();

            // populate pens                        
            pens.Add(new Pen(Brushes.Crimson, 1.0f));
            pens.Add(new Pen(Brushes.DarkKhaki, 1.0f));
            pens.Add(new Pen(Brushes.FloralWhite, 1.0f));
            pens.Add(new Pen(Brushes.HotPink, 1.0f));
            pens.Add(new Pen(Brushes.Yellow, 1.0f));
            pens.Add(new Pen(Brushes.Lavender, 1.0f));
            pens.Add(new Pen(Brushes.Cyan, 1.0f));
            pens.Add(new Pen(Brushes.Maroon, 1.0f));


            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e) {
            dispatcherTimer.Stop();
            this.Invalidate();
        }

        public AudioLevelMonitor AudioMonitor {
            get { return _audioMonitor; }
            set {
                _audioMonitor = value; 
                if (_audioMonitor != null) {
                }
            }
        }

        private void _audioMonitor_NewAudioSamplesEventListeners(AudioLevelMonitor monitor) {
        }      

        private void RenderVUMeterGrid(Graphics g, double maxSample) {
            // сделать его похожим на волюметр
            // g.FillRectangle(Brushes.Black,this.Bounds);   
            g.FillRectangle(Brushes.Black,0,0,Size.Width,Size.Height);

            double gridLine_step = 0.01;
            double gridLine_log = gridLine_step * 2;

            // рисовать линии сетки каждые 0,1
            for (double x = 0.0; x < maxSample; x += gridLine_step) {
                int y = (int)(Size.Height - (Size.Height * (x / maxSample)));
                g.DrawLine(greenPen,
                    new Point(0, y),
                    new Point(Size.Width, y));

                // логарифмические линии сетки
                if (x >= gridLine_log) {
                    gridLine_step *= 2;
                    gridLine_log = gridLine_step * 2;
                }
            }

        }

        // это кажется дорогим, но я не уверен, как еще это сделать
        private double computeMaxSampleLastN(IDictionary<string, AudioLevelMonitor.SampleInfo> sampleMap, int lastN)
        {//вычислить Max Sample Last N
            double maxSample = 0.0;
            foreach (var kvp in sampleMap) {
                var samples = kvp.Value.samples;
                for (int i = 1; i <= samples.Length; i++) {
                    if (i > lastN) {
                        goto next_process;
                    }

                    var val = samples[samples.Length - i];
                    maxSample = Math.Max(maxSample, val);
                    if (maxSample > 0.9) {
                        return 1.0; // экономить время
                    }
                    next_process:;
                }

            }
            return maxSample;
        }
        int nextPenToAllocate = -1;
        private Pen penForSessionId(string sessionId) {
            if (nextPenToAllocate < 0) {
                nextPenToAllocate = Math.Abs((int)DateTime.Now.Ticks) % (pens.Count - 1);
            }

            if (_sessionIdToPen.ContainsKey(sessionId)) {
                return _sessionIdToPen[sessionId];
            }
            else {
                // allocate a new pen //выделить новую ручку
                var allocatedPen = _sessionIdToPen[sessionId] = pens[nextPenToAllocate];
                nextPenToAllocate = (nextPenToAllocate + 1) % (pens.Count - 1);
                return allocatedPen;
            }
        }      

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            var g = pe.Graphics;

            // if we have no AudioMonitor draw a blank grid // если у нас нет AudioMonitor рисуем пустую сетку
            if (AudioMonitor == null) {
                RenderVUMeterGrid(g, 1.0);
                return;
            }
            // otherwise get samples, and draw a scaled rgid // в противном случае получите образцы и нарисуйте масштабированный rgid       
            var activeSamples = AudioMonitor.GetActiveSamples();
            double maxSample = computeMaxSampleLastN(activeSamples, this.Size.Width);           
            maxSample = Math.Max(maxSample, 0.05); // make sure we don't divide by zero
            RenderVUMeterGrid(g, maxSample);

            // now draw the individual sample lines // Теперь нарисуйте отдельные линии образца                        
            foreach (var kvp in activeSamples) {
                Pen audioLevelPen = penForSessionId(kvp.Value.sessionId);
                string name = kvp.Value.SessionName;
                double[] samples = kvp.Value.samples;

                double last_sample = samples[samples.Length - 1];
                for (int x = 0; x < samples.Length - 1; x++) {
                    if (x > Size.Width) {
                        goto next_process;
                    }
                    var sample = samples[samples.Length - (x + 1)];
                    g.DrawLine(audioLevelPen,
                        new Point(Size.Width - x, (int)(Size.Height - (Size.Height * (last_sample / maxSample)))),
                        new Point(Size.Width - (x + 1), (int)(Size.Height - (Size.Height * (sample / maxSample)))));
                    last_sample = sample;
                }
                next_process:;
            }



            // and finally draw the legend // и, наконец, нарисовать легенду
            // http://csharphelper.com/blog/2015/05/get-font-metrics-in-a-wpf-program-using-c/
            // http://csharphelper.com/blog/2015/04/render-text-easily-in-a-wpf-program-using-c/
            List<string> sessionIdList = activeSamples.Keys.ToList();
            sessionIdList.Sort();
            var font = SystemFonts.DefaultFont;
            // first time is to measure the height to draw the legend box // первый раз - измерить высоту, чтобы нарисовать поле легенды
            {
                float y_start = 5;
                float max_label_width = 0;

                foreach (var sessionId in sessionIdList) {
                    string name = activeSamples[sessionId].SessionName;            
                    var measure = g.MeasureString(name, font);
                    y_start += measure.Height;             
                    max_label_width = Math.Max(max_label_width,measure.Width);
                    y_start += 10; // vertical padding // вертикальное заполнение
                }
                // draw the legend box // нарисуйте поле легенды
                g.FillRectangle(Brushes.Black,5,10,max_label_width + 10,y_start);
                g.DrawRectangle(greenPen,5,10,max_label_width + 10,y_start);
            }


            // now draw the legend labels // теперь нарисуйте метки легенды
            {
                float y_start = 5;

                foreach (var sessionId in sessionIdList) {
                    string name = activeSamples[sessionId].SessionName;
                    Pen pen = this.penForSessionId(sessionId);
                    var brush = pen.Brush;

                    var measure = g.MeasureString(name,font);
                    y_start += measure.Height;
                    g.DrawString(name, font, brush, new PointF(10, y_start));

                    y_start += 10; // вертикальное заполнение
                }
            }

            dispatcherTimer.Start();
        }

    }
}
