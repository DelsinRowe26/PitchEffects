using CSCore.DSP;

namespace PitchShifter
{
    internal class HighpassFilter : BiQuad
    {
        private int sampleRate;
        private int bottomFreq;

        public HighpassFilter(int sampleRate, int bottomFreq)
        {
            this.sampleRate = sampleRate;
            this.bottomFreq = bottomFreq;
        }
    }
}