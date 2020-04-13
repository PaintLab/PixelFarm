using System;

namespace BitMiracle.LibJpeg
{
    /// <summary>
    /// Represents a row of image - collection of samples.
    /// </summary>
#if EXPOSE_LIBJPEG
    public
#endif
    class SampleRow
    {

        readonly short[] _lineBuffer16;
        readonly byte[] _lineBuffer8;

        readonly int _columnCount;
        readonly int _componentsPerSample;

        /// <summary>
        /// Creates a row from raw samples data.
        /// </summary>
        /// <param name="row">Raw description of samples.<br/>
        /// You can pass collection with more than sampleCount samples - only sampleCount samples 
        /// will be parsed and all remaining bytes will be ignored.</param>
        /// <param name="columnCount">The number of samples in row.</param>
        /// <param name="bitsPerComponent">The number of bits per component.</param>
        /// <param name="componentsPerSample">The number of components per sample.</param>
        public SampleRow(byte[] row, int columnCount, byte bitsPerComponent, byte componentsPerSample)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (row.Length == 0)
                throw new ArgumentException("row is empty");

            if (columnCount <= 0)
                throw new ArgumentOutOfRangeException("sampleCount");

            if (bitsPerComponent <= 0 || bitsPerComponent > 16)
                throw new ArgumentOutOfRangeException("bitsPerComponent");

            if (componentsPerSample <= 0 || componentsPerSample > 5) //1,2,3,4
                throw new ArgumentOutOfRangeException("componentsPerSample");

            _componentsPerSample = componentsPerSample;


            _columnCount = columnCount;

            using (BitStream bitStream = new BitStream(row))
            {
                //create long buffer for a single line                
                _lineBuffer16 = new short[columnCount * componentsPerSample];
                int byteIndex = 0;
                for (int i = 0; i < columnCount; ++i)
                {
                    //each component
                    //eg. 1,2,3,4 
                    switch (componentsPerSample)
                    {
                        case 1:
                            _lineBuffer16[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                            byteIndex++;
                            break;
                        case 2:
                            _lineBuffer16[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                            _lineBuffer16[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                            byteIndex += 2;
                            break;
                        case 3:
                            _lineBuffer16[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                            _lineBuffer16[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                            _lineBuffer16[byteIndex + 2] = (short)bitStream.Read(bitsPerComponent);
                            byteIndex += 3;
                            break;
                        case 4:
                            _lineBuffer16[byteIndex] = (short)bitStream.Read(bitsPerComponent);
                            _lineBuffer16[byteIndex + 1] = (short)bitStream.Read(bitsPerComponent);
                            _lineBuffer16[byteIndex + 2] = (short)bitStream.Read(bitsPerComponent);
                            _lineBuffer16[byteIndex + 4] = (short)bitStream.Read(bitsPerComponent);
                            byteIndex += 4;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

            }
        }

        public int ComponentsPerSample => _componentsPerSample;


        public void GetComponentsAt(int column, out byte r, out byte g, out byte b)
        {
            //no alpha channel for jpeg 
            switch (_componentsPerSample)
            {
                case 1:
                    {
                        r = g = b = (byte)_lineBuffer16[column];
                    }
                    break;
                case 2:
                    {
                        //2 byte per sample?                        
                        throw new NotSupportedException(); //?
                    }
                case 3:
                    {
                        int pos = column * 3;
                        r = (byte)_lineBuffer16[pos];
                        g = (byte)_lineBuffer16[pos + 1];
                        b = (byte)_lineBuffer16[pos + 2];
                    }
                    break;
                case 4:
                    {
                        //should not occurs?
                        throw new NotSupportedException(); //?
                    }
                default:
                    throw new NotSupportedException();
            }
        }
        public void WriteToList(System.Collections.Generic.List<byte> outputBytes)
        {
            //write bytes of this row to output bytes 
            for (int i = 0; i < _columnCount; ++i)
            {
                switch (_componentsPerSample)
                {
                    case 1:
                        {
                            outputBytes.Add((byte)_lineBuffer16[i]);
                        }
                        break;
                    case 2:
                        {
                            //2 byte per sample?                        
                            throw new NotSupportedException(); //?
                        }
                    case 3:
                        {
                            int pos = i * 3;
                            outputBytes.Add((byte)_lineBuffer16[pos]);
                            outputBytes.Add((byte)_lineBuffer16[pos + 1]);
                            outputBytes.Add((byte)_lineBuffer16[pos + 2]);
                        }
                        break;
                    case 4:
                        {
                            //should not occurs?
                            throw new NotSupportedException(); //?
                        }
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Gets the number of samples in this row.
        /// </summary>
        /// <value>The number of samples.</value>
        public int Length => _columnCount;

    }
}
