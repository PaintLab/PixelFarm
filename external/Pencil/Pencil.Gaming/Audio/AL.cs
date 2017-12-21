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

namespace Pencil.Gaming.Audio {
	public static partial class AL {
#pragma warning disable 0414
		private static AlcManager manager = new AlcManager();
#pragma warning restore 0414

		public static void Enable(ALCapability capability) {
			ALDelegates.alEnable((int)capability);
		}
		public static void Disable(ALCapability capability) {
			ALDelegates.alDisable((int)capability);
		} 
		public static bool IsEnabled(ALCapability capability) {
			return ALDelegates.alIsEnabled((int)capability);
		} 
		public static unsafe string GetString(ALGetString param) {
			sbyte * bptr = ALDelegates.alGetString((int)param);
			return new string(bptr);
		}
		public static void GetBoolean(ALGetInteger param, bool[] data) {
			ALDelegates.alGetBooleanv((int)param, data);
		}
		public static void GetInteger(ALGetInteger param, int[] data) {
			ALDelegates.alGetIntegerv((int)param, data);
		}
		public static void GetFloat(ALGetFloat param, float[] data) {
			ALDelegates.alGetFloatv((int)param, data);
		}
//		public static void GetDouble(int param, double[] data) {
//			ALDelegates.alGetDoublev(param, data);
//		}
		public static bool GetBoolean(ALGetInteger param) {
			return ALDelegates.alGetBoolean((int)param);
		}
		public static int GetInteger(ALGetInteger param) {
			return ALDelegates.alGetInteger((int)param);
		}
		public static float GetFloat(ALGetFloat param) {
			return ALDelegates.alGetFloat((int)param);
		}
		public static double GetDouble(int param) {
			return ALDelegates.alGetDouble(param);
		}
		public static int GetError() {
			return ALDelegates.alGetError();
		}
		public static bool IsExtensionPresent(string extname) {
			return ALDelegates.alIsExtensionPresent(extname);
		}
		public static IntPtr GetProcAddress(string fname) {
			return ALDelegates.alGetProcAddress(fname);
		}
		public static int GetEnumValue(string ename) {
			return ALDelegates.alGetEnumValue(ename);
		}
		public static void Listener(ALListenerf param, float value) {
			ALDelegates.alListenerf((int)param, value);
		}
		public static void Listener(ALListener3f param, float value1, float value2, float value3) {
			ALDelegates.alListener3f((int)param, value1, value2, value3);
		}
		public static void Listener(ALListenerfv param, float[] values) {
			ALDelegates.alListenerfv((int)param, values);
		} 
//		public static void Listener(int param, int value) {
//			ALDelegates.alListeneri(param, value);
//		}
//		public static void Listener(int param, int value1, int value2, int value3) {
//			ALDelegates.alListener3i(param, value1, value2, value3);
//		}
//		public static void Listener(int param, int[] values) {
//			ALDelegates.alListeneriv(param, values);
//		}
		public static void GetListener(ALListenerf param, out float value) {
			ALDelegates.alGetListenerf((int)param, out value);
		}
		public static void GetListener(ALListener3f param, out float value1, out float value2, out float value3) {
			ALDelegates.alGetListener3f((int)param, out value1, out value2, out value3);
		}
		public static void GetListener(ALListenerfv param, float[] values) {
			ALDelegates.alGetListenerfv((int)param, values);
		}
//		public static void GetListener(int param, out int value) {
//			ALDelegates.alGetListeneri(param, out value);
//		}
//		public static void GetListener(int param, out int value1, out int value2, out int value3) {
//			ALDelegates.alGetListener3i(param, out value1, out value2, out value3);
//		}
//		public static void GetListener(int param, int[] values) {
//			ALDelegates.alGetListeneriv(param, values);
//		}
		public static void GenSources(int n, uint[] sources) {
			ALDelegates.alGenSources(n, sources);
		} 
		public static void GenSources(int n, out uint source) {
			ALDelegates.alGenSource(n, out source);
		}
		public static void DeleteSources(int n, uint[] sources) {
			ALDelegates.alDeleteSources(n, sources);
		}
		public static void DeleteSources(int n, ref uint source) {
			ALDelegates.alDeleteSource(n, ref source);
		}
		public static bool IsSource(uint sid) {
			return ALDelegates.alIsSource(sid);
		} 
		public static void Source(uint sid, ALSourcef param, float value) {
			ALDelegates.alSourcef(sid, (int)param, value);
		} 
		public static void Source(uint sid, ALSource3f param, float value1, float value2, float value3) {
			ALDelegates.alSource3f(sid, (int)param, value1, value2, value3);
		}
//		public static void Source(uint sid, int param, float[] values) {
//			ALDelegates.alSourcefv(sid, param, values);
//		} 
		public static void Source(uint sid, ALSourcei param, int value) {
			ALDelegates.alSourcei(sid, (int)param, value);
		} 
		public static void Source(uint sid, ALSource3i param, int value1, int value2, int value3) {
			ALDelegates.alSource3i(sid, (int)param, value1, value2, value3);
		}
		public static void Source(uint sid, ALSourceb param, bool value) {
			ALDelegates.alSourcei(sid, (int)param, value ? 1 : 0);
		}
//		public static void Source(uint sid, int param, int[] values) {
//			ALDelegates.alSourceiv(sid, param, values);
//		}
		public static void GetSource(uint sid, ALSourcef param, out float value) {
			ALDelegates.alGetSourcef(sid, (int)param, out value);
		}
		public static void GetSource(uint sid, ALSource3f param, out float value1, out float value2, out float value3) {
			ALDelegates.alGetSource3f(sid, (int)param, out value1, out value2, out value3);
		}
//		public static void GetSource(uint sid, int param, float[] values) {
//			ALDelegates.alGetSourcefv(sid, param, values);
//		}
		public static void GetSource(uint sid, ALSourcei param, out int value) {
			ALDelegates.alGetSourcei(sid, (int)param, out value);
		}
		public static void GetSource(uint sid, ALSource3i param, out int value1, out int value2, out int value3) {
			ALDelegates.alGetSource3i(sid, (int)param, out value1, out value2, out value3);
		}
		public static void GetSource(uint sid, ALSourceb param, out bool value) {
			int ivalue;
			ALDelegates.alGetSourcei(sid, (int)param, out ivalue);
			value = (ivalue != 0);
		}
//		public static void GetSource(uint sid, int param, int[] values) {
//			ALDelegates.alGetSourceiv(sid, param, values);
//		}
		public static void SourcePlay(int ns, uint[]sids) {
			ALDelegates.alSourcePlayv(ns, sids);
		}
		public static void SourceStop(int ns, uint[]sids) {
			ALDelegates.alSourceStopv(ns, sids);
		}
		public static void SourceRewind(int ns, uint[]sids) {
			ALDelegates.alSourceRewindv(ns, sids);
		}
		public static void SourcePause(int ns, uint[]sids) {
			ALDelegates.alSourcePausev(ns, sids);
		}
		public static void SourcePlay(uint sid) {
			ALDelegates.alSourcePlay(sid);
		}
		public static void SourceStop(uint sid) {
			ALDelegates.alSourceStop(sid);
		}
		public static void SourceRewind(uint sid) {
			ALDelegates.alSourceRewind(sid);
		}
		public static void SourcePause(uint sid) {
			ALDelegates.alSourcePause(sid);
		}
		public static void SourceQueueBuffers(uint sid, int numEntries, uint[]bids) {
			ALDelegates.alSourceQueueBuffers(sid, numEntries, bids);
		}
		public static void SourceUnqueueBuffers(uint sid, int numEntries, uint[]bids) {
			ALDelegates.alSourceUnqueueBuffers(sid, numEntries, bids);
		}
		public static void GenBuffers(int n, uint[] buffers) {
			ALDelegates.alGenBuffers(n, buffers);
		}
		public static void GenBuffers(int n, out uint buffer) {
			ALDelegates.alGenBuffer(n, out buffer);
		}
		public static void DeleteBuffers(int n, uint[] buffers) {
			ALDelegates.alDeleteBuffers(n, buffers);
		}
		public static void DeleteBuffers(int n, ref uint buffer) {
			ALDelegates.alDeleteBuffer(n, ref buffer);
		}
		public static bool IsBuffer(uint bid) {
			return ALDelegates.alIsBuffer(bid);
		}
		public static void BufferData(uint bid, ALFormat format, IntPtr data, int size, int freq) {
			ALDelegates.alBufferData(bid, (int)format, data, size, freq);
		}
//		public static void Buffer(uint bid, int param, float value) {
//			ALDelegates.alBufferf(bid, param, value);
//		}
//		public static void Buffer(uint bid, int param, float value1, float value2, float value3) {
//			ALDelegates.alBuffer3f(bid, param, value1, value2, value3);
//		}
//		public static void Buffer(uint bid, int param, float[] values) {
//			ALDelegates.alBufferfv(bid, param, values);
//		}
//		public static void Buffer(uint bid, int param, int value) {
//			ALDelegates.alBufferi(bid, param, value);
//		}
//		public static void Buffer(uint bid, int param, int value1, int value2, int value3) {
//			ALDelegates.alBuffer3i(bid, param, value1, value2, value3);
//		}
//		public static void Buffer(uint bid, int param, int[] values) {
//			ALDelegates.alBufferiv(bid, param, values);
//		}
//		public static void GetBuffer(uint bid, int param, out float value) {
//			ALDelegates.alGetBufferf(bid, param, out value);
//		}
//		public static void GetBuffer(uint bid, int param, out float value1, out float value2, out float value3) {
//			ALDelegates.alGetBuffer3f(bid, param, out value1, out value2, out value3);
//		}
//		public static void GetBuffer(uint bid, int param, float[] values) {
//			ALDelegates.alGetBufferfv(bid, param, values);
//		}
		public static void GetBuffer(uint bid, ALGetBufferi param, out int value) {
			ALDelegates.alGetBufferi(bid, (int)param, out value);
		}
//		public static void GetBuffer(uint bid, int param, out int value1, out int value2, out int value3) {
//			ALDelegates.alGetBuffer3i(bid, param, out value1, out value2, out value3);
//		}
//		public static void GetBuffer(uint bid, int param, int[] values) {
//			ALDelegates.alGetBufferiv(bid, param, values);
//		}
		public static void DopplerFactor(float value) {
			ALDelegates.alDopplerFactor(value);
		}
		public static void DopplerVelocity(float value) {
			ALDelegates.alDopplerVelocity(value);
		}
		public static void SpeedOfSound(float value) {
			ALDelegates.alSpeedOfSound(value);
		}
		public static void DistanceModel(int distanceModel) {
			ALDelegates.alDistanceModel(distanceModel);
		}
	}
}