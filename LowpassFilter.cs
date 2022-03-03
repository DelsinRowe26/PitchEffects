using CSCore.DSP;

namespace PitchShifter
{
    internal class LowpassFilter : BiQuad
    {
        private int sampleRate;
        private int topFreq;

        public LowpassFilter(int sampleRate, int topFreq)
        {
            this.sampleRate = sampleRate;
            this.topFreq = topFreq;
        }
    }
}