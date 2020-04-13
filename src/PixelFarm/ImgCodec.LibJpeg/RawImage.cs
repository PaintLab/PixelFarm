using System.Collections.Generic;
using System.Diagnostics;

namespace BitMiracle.LibJpeg
{
    class RawImage : IRawImage
    {
        List<SampleRow> _samples;
        Colorspace _colorspace;

        int m_currentRow = -1;
        internal RawImage(List<SampleRow> samples, Colorspace colorspace)
        {
            Debug.Assert(samples != null);
            Debug.Assert(samples.Count > 0);
            Debug.Assert(colorspace != Colorspace.Unknown);


            _samples = samples;
            _colorspace = colorspace;
        }

        public int Width => _samples[0].Length;

        public int Height => _samples.Count;

        public Colorspace Colorspace => _colorspace;

        public int ComponentsPerPixel => _samples[0].ComponentsPerSample;

        public void BeginRead()
        {
            m_currentRow = 0;
        }

        public byte[] GetPixelRow()
        {
            SampleRow row = _samples[m_currentRow];
            List<byte> result = new List<byte>();
            for (int i = 0; i < row.Length; ++i)
            {
                //Sample sample = row[i];
                row.WriteToList(result);

                //for (int j = 0; j < sample.ComponentCount; ++j)
                //    result.Add((byte)sample[j]);
            }
            ++m_currentRow;
            return result.ToArray();
        }

        public void EndRead()
        {
        }
    }
}
