#region License
// Copyright (c) 2013 Antonie Blom
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NVorbis;

namespace Pencil.Gaming.Audio {
	public static partial class AL {
		public static class Utils {
			#region Ogg Loading

			public static void LoadOgg(string file, out byte[] data, out ALFormat format, out uint sampleRate, out TimeSpan len) {
				using (VorbisReader vorbis = new VorbisReader(file)) {
					LoadOgg(vorbis, out data, out format, out sampleRate, out len);
				}
			}

			public static void LoadOgg(Stream file, out byte[] data, out ALFormat format, out uint sampleRate, out TimeSpan len) {
				using (VorbisReader vorbis = new VorbisReader(file, false)) {
					LoadOgg(vorbis, out data, out format, out sampleRate, out len);
				}
			}

			private static void LoadOgg(VorbisReader vorbis, out byte[] data, out ALFormat format, out uint sampleRate, out TimeSpan len) {
				sampleRate = (uint)vorbis.SampleRate;
				format = vorbis.Channels == 1 ? ALFormat.Mono16 : ALFormat.Stereo16;
				len = vorbis.TotalTime;

				float[] buffer = new float[vorbis.SampleRate / 10 * vorbis.Channels];
				List<byte> bytes = new List<byte>((int)(vorbis.SampleRate * vorbis.Channels * 2 * len.TotalSeconds));
				int count = 0;
				while ((count = vorbis.ReadSamples(buffer, 0, buffer.Length)) > 0) {
					for (int i = 0; i < count; i++) {
						int temp = (int)(short.MaxValue * buffer [i]);
						if (temp > short.MaxValue) {
							temp = short.MaxValue;
						} else if (temp < short.MinValue) {
							temp = short.MinValue;
						}
						short tempBytes = (short)temp;
						byte byte1 = (byte)((tempBytes >> 8) & 0x00FF);
						byte byte2 = (byte)((tempBytes >> 0) & 0x00FF);

						// Little endian
						bytes.Add(byte2);
						bytes.Add(byte1);
					}
				}
				// TODO: Add better implementation so that there's no need for array copying
				data = bytes.ToArray();
			}

			public static uint BufferFromOgg(string file) {
				using (VorbisReader vorbis = new VorbisReader(file)) {
					return BufferFromOgg(vorbis);
				}
			}

			public static uint BufferFromOgg(Stream file) {
				using (VorbisReader vorbis = new VorbisReader(file, false)) {
					return BufferFromOgg(vorbis);
				}
			}

			private unsafe static uint BufferFromOgg(VorbisReader vorbis) {
				uint result;
				AL.GenBuffers(1, out result);

				byte[] data;
				ALFormat format;
				uint sampleRate;
				TimeSpan len;
				LoadOgg(vorbis, out data, out format, out sampleRate, out len);

				fixed (byte * dataPtr = &data[0]) {
					IntPtr dataIntPtr = new IntPtr(dataPtr);
					AL.BufferData(result, format, dataIntPtr, data.Length, (int)sampleRate);
				}

				return result;
			}

			#endregion

			#region Wav Loading

			public static void LoadWav(
				string file,
				out byte[] data,
				out ALFormat format,
				out uint sampleRate) {

				LoadWav(File.ReadAllBytes(file), out data, out format, out sampleRate);
			}

			public static unsafe void LoadWavExt(
				string file, 
				out byte[] data,
				out uint chunkSize,
				out ALFormat format,
				out uint sampleRate,
				out uint avgBytesPerSec,
				out short bytesPerSample,
				out short bitsPerSample) {

				LoadWavExt(File.ReadAllBytes(file), out data, out chunkSize, out format, out sampleRate, out avgBytesPerSec, out bytesPerSample, out bitsPerSample);
			}
		
			public static void LoadWav(
				Stream file,
				out byte[] data,
				out ALFormat format,
				out uint sampleRate) {

				using (MemoryStream ms = new MemoryStream()) {
					if (!file.CanRead) {
						throw new NotSupportedException("This stream does not support reading");
					}
					byte[] buffer = new byte[16 * 1024];
					int nread;
					while ((nread = file.Read(buffer, 0, 16 * 1024)) != 0) {
						ms.Write(buffer, 0, nread);
					}

					LoadWav(ms.ToArray(), out data, out format, out sampleRate);
				}
			}

			public static void LoadWavExt(
				Stream file,
				out byte[] data,
				out uint chunkSize,
				out ALFormat format,
				out uint sampleRate,
				out uint avgBytesPerSec,
				out short bytesPerSample,
				out short bitsPerSample) {

				using (MemoryStream ms = new MemoryStream()) {
					if (!file.CanRead) {
						throw new NotSupportedException("This stream does not support reading");
					}
					byte[] buffer = new byte[16 * 1024];
					int nread;
					while ((nread = file.Read(buffer, 0, 16 * 1024)) != 0) {
						ms.Write(buffer, 0, nread);
					}

					LoadWavExt(ms.ToArray(), out data, out chunkSize, out format, out sampleRate, out avgBytesPerSec, out bytesPerSample, out bitsPerSample);
				}
			}

			public static void LoadWav(
				byte[] file,
				out byte[] data,
				out ALFormat format,
				out uint sampleRate) {

				uint dummyui;
				short dummys;
				LoadWavExt(file, out data, out dummyui, out format, out sampleRate, out dummyui, out dummys, out dummys);
			}

			public static void LoadWavExt(
				byte[] sound,
				out byte[] data,
				out uint chunkSize,
				out ALFormat format,
				out uint sampleRate,
				out uint avgBytesPerSec,
				out short bytesPerSample,
				out short bitsPerSample) {

#if DEBUG
				Stopwatch sw = new Stopwatch();
				sw.Start();
#endif

				short channels;

				int ptrOffset = 4;
				if (sound [0] != 'R' || sound [1] != 'I' || sound [2] != 'F' || sound [3] != 'F') {
					throw new Exception("Invalid file format.");
				}
				//size = ((uint)sound[3 + ptrOffset] << 24) | ((uint)sound[2 + ptrOffset] << 16) | ((uint)sound[1 + ptrOffset] << 8) | ((uint)sound[ptrOffset]);
				if (sound [8] != 'W' || sound [9] != 'A' || sound [10] != 'V' || sound [11] != 'E') {
					throw new Exception("Invalid file format.");
				}
				if (sound [12] != 'f' || sound [13] != 'm' || sound [14] != 't' || sound [15] != ' ') {
					throw new Exception("Invalid file format.");
				}
				ptrOffset = 16;
				chunkSize = ((uint)sound [3 + ptrOffset] << 24) | ((uint)sound [2 + ptrOffset] << 16) | ((uint)sound [1 + ptrOffset] << 8) | ((uint)sound [ptrOffset]);
				//ptrOffset = 20;
				//formatType = ((short)(((short)sound[1 + ptrOffset] << 8) | ((short)sound[0 + ptrOffset])));
				ptrOffset = 22;
				channels = (short)(((short)sound [1 + ptrOffset] << 8) | ((short)sound [0 + ptrOffset]));
				ptrOffset = 24;
				sampleRate = ((uint)sound [3 + ptrOffset] << 24) | ((uint)sound [2 + ptrOffset] << 16) | ((uint)sound [1 + ptrOffset] << 8) | ((uint)sound [ptrOffset]);
				ptrOffset = 28;
				avgBytesPerSec = ((uint)sound [3 + ptrOffset] << 24) | ((uint)sound [2 + ptrOffset] << 16) | ((uint)sound [1 + ptrOffset] << 8) | ((uint)sound [ptrOffset]);
				ptrOffset = 32;
				bytesPerSample = (short)(((short)sound [1 + ptrOffset] << 8) | ((short)sound [0 + ptrOffset]));
				ptrOffset = 34;
				bitsPerSample = (short)(((short)sound [1 + ptrOffset] << 8) | ((short)sound [0 + ptrOffset]));
				if (sound [36] != 'd' || sound [37] != 'a' || sound [38] != 't' || sound [39] != 'a') {
					throw new Exception("Invalid file format.");
				}
				ptrOffset = 40;
				int dataSize = ((int)sound [3 + ptrOffset] << 24) | ((int)sound [2 + ptrOffset] << 16) | ((int)sound [1 + ptrOffset] << 8) | ((int)sound [ptrOffset]);

				format = (ALFormat)0;
				if (bitsPerSample == 8) {
					if (channels == 1) {
						format = ALFormat.Mono8;
					} else if (channels == 2) {
						format = ALFormat.Stereo8;
					}
				} else if (bitsPerSample == 16) {
					if (channels == 1) {
						format = ALFormat.Mono16;
					} else if (channels == 2) {
						format = ALFormat.Stereo16;
					}
				}

				data = new byte[dataSize];
				Array.Copy(sound, 44, data, 0, dataSize);

#if DEBUG
				sw.Stop();
				Console.WriteLine("Loading audio file took {0} milliseconds.", sw.ElapsedMilliseconds);
#endif
			}
		
			public static uint BufferFromWav(string file) {
				return BufferFromWav(File.ReadAllBytes(file));
			}

			public static uint BufferFromWav(Stream file) {
				using (MemoryStream ms = new MemoryStream()) {
					if (!file.CanRead) {
						throw new NotSupportedException("This stream does not support reading");
					}
					byte[] buffer = new byte[16 * 1024];
					int nread;
					while ((nread = file.Read(buffer, 0, 16 * 1024)) != 0) {
						ms.Write(buffer, 0, nread);
					}

					return BufferFromWav(ms.ToArray());
				}
			}

			public static unsafe uint BufferFromWav(byte[] wave) {
				uint result;
				AL.GenBuffers(1, out result);

				byte[] data;
				ALFormat format;
				uint sampleRate;
				AL.Utils.LoadWav(wave, out data, out format, out sampleRate);

				fixed (byte * dataPtr = &data[0]) {
					IntPtr dataIntPtr = new IntPtr(dataPtr);
					AL.BufferData(result, format, dataIntPtr, data.Length, (int)sampleRate);
				}

				return result;
			}

			#endregion
		}
	}
}

