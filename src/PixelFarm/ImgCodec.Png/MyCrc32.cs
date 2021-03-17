﻿using System;
// Copyright (c) 2006-2009 Dino Chiesa and Microsoft Corporation.
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License.
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs):
// Time-stamp: <2010-January-16 13:16:27>
//
// ------------------------------------------------------------------
//
// Implements the CRC algorithm, which is used in zip files.  The zip format calls for
// the zipfile to contain a CRC for the unencrypted byte stream of each file.
//
// It is based on example source code published at
//    http://www.vbaccelerator.com/home/net/code/libraries/CRC32/Crc32_zip_CRC32_CRC32_cs.asp
//
// This implementation adds a tweak of that code for use within zip creation.  While
// computing the CRC we also compress the byte stream, in the same read loop. This
// avoids the need to read through the uncompressed stream twice - once to compute CRC
// and another time to compress.
//
// ------------------------------------------------------------------
namespace ImageTools
{
    ref struct CRC32Calculator
    {
        long _totalBytesRead;
        uint _runningCrc32Result;
#if DEBUG
        bool _dbugFirstReset;
#endif
        /// <summary>
        /// indicates the total number of bytes read on the CRC stream.
        /// This is used when writing the ZipDirEntry when compressing files.
        /// </summary>
        public long TotalBytesRead => _totalBytesRead;
        /// <summary>
        /// Indicates the current CRC for all blocks slurped in.
        /// </summary>
        public int Crc32Result
        {
            get
            {
                // return one's complement of the running result
                return unchecked((Int32)(~_runningCrc32Result));
            }
        }
        /// <summary>
        ///reset before reuse
        /// </summary>
        public void Reset()
        {

            _runningCrc32Result = 0xFFFFFFFF;
            _totalBytesRead = 0;
#if DEBUG
            _dbugFirstReset = true;
#endif
        }

        ///// <summary>
        ///// Get the CRC32 for the given (word,byte) combo.  This is a computation
        ///// defined by PKzip.
        ///// </summary>
        ///// <param name="W">The word to start with.</param>
        ///// <param name="B">The byte to combine it with.</param>
        ///// <returns>The CRC-ized result.</returns>
        //static Int32 ComputeCrc32(Int32 W, byte B)
        //{
        //    return _InternalComputeCrc32((UInt32)W, B);
        //}

        //internal static Int32 _InternalComputeCrc32(UInt32 W, byte B)
        //{
        //    return (Int32)(crc32Table[(W ^ B) & 0xFF] ^ (W >> 8));
        //}
        public void SlurpBlock(byte[] block)
        {
            SlurpBlock(block, 0, block.Length);
        }
        /// <summary>
        /// Update the value for the running CRC32 using the given block of bytes.
        /// This is useful when using the CRC32() class in a Stream.
        /// </summary>
        /// <param name="block">block of bytes to slurp</param>
        /// <param name="offset">starting point in the block</param>
        /// <param name="count">how many bytes within the block to slurp</param>
        public void SlurpBlock(byte[] block, int offset, int count)
        {
            if (block == null)
            {
                throw new NotSupportedException("The data buffer must not be null.");
            }
#if DEBUG
            if (_dbugFirstReset)
            {
                throw new NotSupportedException();
            }
#endif


            // UInt32 tmpRunningCRC32Result = _RunningCrc32Result;
            for (int i = 0; i < count; i++)
            {
#if DEBUG
                int x = offset + i;
#endif
                _runningCrc32Result = ((_runningCrc32Result) >> 8) ^ crc32Table[(block[offset + i]) ^ ((_runningCrc32Result) & 0x000000FF)];
            }

            _totalBytesRead += count;
            //_RunningCrc32Result = tmpRunningCRC32Result;
        }


        // pre-initialize the crc table for speed of lookup.

        static readonly uint[] crc32Table;
        static CRC32Calculator()
        {
            unchecked
            {
                // PKZip specifies CRC32 with a polynomial of 0xEDB88320;
                // This is also the CRC-32 polynomial used bby Ethernet, FDDI,
                // bzip2, gzip, and others.
                // Often the polynomial is shown reversed as 0x04C11DB7.
                // For more details, see http://en.wikipedia.org/wiki/Cyclic_redundancy_check
                UInt32 dwPolynomial = 0xEDB88320;


                crc32Table = new uint[256];
                UInt32 dwCrc;
                for (uint i = 0; i < 256; i++)
                {
                    dwCrc = i;
                    for (uint j = 8; j > 0; j--)
                    {
                        if ((dwCrc & 1) == 1)
                        {
                            dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                        }
                        else
                        {
                            dwCrc >>= 1;
                        }
                    }
                    crc32Table[i] = dwCrc;
                }
            }
        }
        static uint gf2_matrix_times(uint[] matrix, uint vec)
        {
            uint sum = 0;
            int i = 0;
            while (vec != 0)
            {
                if ((vec & 0x01) == 0x01)
                {
                    sum ^= matrix[i];
                }
                vec >>= 1;
                i++;
            }
            return sum;
        }

        static void gf2_matrix_square(uint[] square, uint[] mat)
        {
            for (int i = 0; i < 32; i++)
            {
                square[i] = gf2_matrix_times(mat, mat[i]);
            }
        }



        /// <summary>
        /// Combines the given CRC32 value with the current running total.
        /// </summary>
        /// <remarks>
        /// This is useful when using a divide-and-conquer approach to calculating a CRC.
        /// Multiple threads can each calculate a CRC32 on a segment of the data, and then
        /// combine the individual CRC32 values at the end.
        /// </remarks>
        /// <param name="crc">the crc value to be combined with this one</param>
        /// <param name="length">the length of data the CRC value was calculated on</param>
        public void Combine(int crc, int length)
        {
            uint[] even = new uint[32];     // even-power-of-two zeros operator
            uint[] odd = new uint[32];      // odd-power-of-two zeros operator

            if (length == 0)
                return;

            uint crc1 = ~_runningCrc32Result;
            uint crc2 = (uint)crc;

            // put operator for one zero bit in odd
            odd[0] = 0xEDB88320;  // the CRC-32 polynomial
            uint row = 1;
            for (int i = 1; i < 32; i++)
            {
                odd[i] = row;
                row <<= 1;
            }

            // put operator for two zero bits in even
            gf2_matrix_square(even, odd);

            // put operator for four zero bits in odd
            gf2_matrix_square(odd, even);

            uint len2 = (uint)length;

            // apply len2 zeros to crc1 (first square will put the operator for one
            // zero byte, eight zero bits, in even)
            do
            {
                // apply zeros operator for this bit of len2
                gf2_matrix_square(even, odd);

                if ((len2 & 1) == 1)
                    crc1 = gf2_matrix_times(even, crc1);
                len2 >>= 1;

                if (len2 == 0)
                    break;

                // another iteration of the loop with odd and even swapped
                gf2_matrix_square(odd, even);
                if ((len2 & 1) == 1)
                    crc1 = gf2_matrix_times(odd, crc1);
                len2 >>= 1;


            } while (len2 != 0);

            crc1 ^= crc2;

            _runningCrc32Result = ~crc1;

            //return (int) crc1;
            return;
        }





    }

}