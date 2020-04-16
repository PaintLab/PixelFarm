using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BitMiracle.LibJpeg
{
    public class BitStream : IDisposable
    {
        const int BITS_IN_BYTE = 8;
        bool _alreadyDisposed;
        Stream _stream;
        int _positionInByte;
        int _sizeInBits;

        public BitStream()
        {

        }
        public void ResetInput(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (_stream == null)
            {
                _stream = new MemoryStream(buffer);
                _sizeInBits = bitsAllocated();
            }
            else
            {
                if (_stream.Length != buffer.Length)
                {
                    //alloc new
                    _stream.Dispose();
                    _stream = new MemoryStream(buffer);
                    _sizeInBits = bitsAllocated();
                }
                else
                {
                    _stream.Position = 0;
                    _stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_alreadyDisposed)
            {
                if (disposing)
                {
                    if (_stream != null)
                        _stream.Dispose();
                }

                _stream = null;
                _alreadyDisposed = true;
            }
        }

        /// <summary>
        /// size in bits 
        /// </summary>
        /// <returns></returns>
        public int SizeInBits() => _sizeInBits;

        internal Stream UnderlyingStream => _stream;

        public virtual int Read(int bitCount)
        {
            if (Tell() + bitCount > bitsAllocated())
                throw new ArgumentOutOfRangeException("bitCount");

            return read(bitCount);
        }

        public int Write(int bitStorage, int bitCount)
        {
            if (bitCount == 0)
                return 0;

            const int maxBitsInStorage = sizeof(int) * BITS_IN_BYTE;
            if (bitCount > maxBitsInStorage)
                throw new ArgumentOutOfRangeException("bitCount");

            for (int i = 0; i < bitCount; ++i)
            {
                byte bit = (byte)((bitStorage << (maxBitsInStorage - (bitCount - i))) >> (maxBitsInStorage - 1));
                if (!writeBit(bit))
                    return i;
            }

            return bitCount;
        }

        public void Seek(int pos, SeekOrigin mode)
        {
            switch (mode)
            {
                case SeekOrigin.Begin:
                    seekSet(pos);
                    break;

                case SeekOrigin.Current:
                    seekCurrent(pos);
                    break;

                case SeekOrigin.End:
                    seekSet(SizeInBits() + pos);
                    break;
            }
        }

        public int Tell()
        {
            return (int)_stream.Position * BITS_IN_BYTE + _positionInByte;
        }

        private int bitsAllocated()
        {
            return (int)_stream.Length * BITS_IN_BYTE;
        }

        byte[] _byte_buffer1 = new byte[1];

 
        private int read(int bitsCount)
        {
            //Codes are packed into a continuous bit stream, high-order bit first. 
            //This stream is then divided into 8-bit bytes, high-order bit first. 
            //Thus, codes can straddle byte boundaries arbitrarily. After the EOD marker (code value 257), 
            //any leftover bits in the final byte are set to 0.

            if (bitsCount < 0 || bitsCount > 32)
                throw new ArgumentOutOfRangeException("bitsCount");

            if (bitsCount == 0)
                return 0;

            int bitsRead = 0;
            int result = 0;

            while (bitsRead == 0 || (bitsRead - _positionInByte < bitsCount))
            {
                _stream.Read(_byte_buffer1, 0, 1);

                result = (result << BITS_IN_BYTE);
                result += _byte_buffer1[0];

                bitsRead += 8;
            }

            _positionInByte = (_positionInByte + bitsCount) % 8;
            if (_positionInByte != 0)
            {
                result = (result >> (BITS_IN_BYTE - _positionInByte));

                _stream.Seek(-1, SeekOrigin.Current);
            }

            if (bitsCount < 32)
            {
                int mask = ((1 << bitsCount) - 1);
                result = result & mask;
            }

            return result;
        }

        private bool writeBit(byte bit)
        {
            if (_stream.Position == _stream.Length)
            {
                byte[] bt = { (byte)(bit << (BITS_IN_BYTE - 1)) };
                _stream.Write(bt, 0, 1);
                _stream.Seek(-1, SeekOrigin.Current);
            }
            else
            {
                byte[] bt = { 0 };
                _stream.Read(bt, 0, 1);
                _stream.Seek(-1, SeekOrigin.Current);

                int shift = (BITS_IN_BYTE - _positionInByte - 1) % BITS_IN_BYTE;
                byte maskByte = (byte)(bit << shift);

                bt[0] |= maskByte;
                _stream.Write(bt, 0, 1);
                _stream.Seek(-1, SeekOrigin.Current);
            }

            Seek(1, SeekOrigin.Current);

            int currentPosition = Tell();
            if (currentPosition > _sizeInBits)
                _sizeInBits = currentPosition;

            return true;
        }

        private void seekSet(int pos)
        {
            if (pos < 0)
                throw new ArgumentOutOfRangeException("pos");

            int byteDisplacement = pos / BITS_IN_BYTE;
            _stream.Seek(byteDisplacement, SeekOrigin.Begin);

            int shiftInByte = pos - byteDisplacement * BITS_IN_BYTE;
            _positionInByte = shiftInByte;
        }

        private void seekCurrent(int pos)
        {
            int result = Tell() + pos;
            if (result < 0 || result > bitsAllocated())
                throw new ArgumentOutOfRangeException("pos");

            seekSet(result);
        }
    }
}
