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
using System.Security;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Pencil.Gaming.Audio {
	public static class ALDelegates {
		static ALDelegates() {
#if DEBUG
			Stopwatch sw = new Stopwatch();
			sw.Start();
#endif
			Type alInterop = (IntPtr.Size == 8) ? typeof(AL64) : typeof(AL32);
			FieldInfo[] fields = typeof(ALDelegates).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (FieldInfo fi in fields) {
				MethodInfo mi = alInterop.GetMethod(fi.Name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
				Delegate function = Delegate.CreateDelegate(fi.FieldType, mi);
				fi.SetValue(null, function);
			}
#if DEBUG
			sw.Stop();
			Console.WriteLine("Copying OpenAL delegates took {0} milliseconds.", sw.ElapsedMilliseconds);
#endif
		}

		[SuppressUnmanagedCodeSecurity]
		internal delegate void Enable(int capability);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Disable(int capability); 
		[SuppressUnmanagedCodeSecurity]
		internal delegate bool IsEnabled(int capability); 
		[SuppressUnmanagedCodeSecurity]
		internal unsafe delegate sbyte *GetString(int param);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetBooleanv(int param,[MarshalAs(UnmanagedType.LPArray)] bool[] data);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetIntegerv(int param,[MarshalAs(UnmanagedType.LPArray)] int[] data);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetFloatv(int param,[MarshalAs(UnmanagedType.LPArray)] float[] data);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetDoublev(int param,[MarshalAs(UnmanagedType.LPArray)] double[] data);
		[SuppressUnmanagedCodeSecurity]
		internal delegate bool GetBoolean(int param);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetInteger(int param);
		[SuppressUnmanagedCodeSecurity]
		internal delegate float GetFloat(int param);
		[SuppressUnmanagedCodeSecurity]
		internal delegate double GetDouble(int param);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetError();
		[SuppressUnmanagedCodeSecurity]
		internal delegate bool IsExtensionPresent([MarshalAs(UnmanagedType.LPStr)] string extname);
		[SuppressUnmanagedCodeSecurity]
		internal delegate IntPtr GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string fname);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetEnumValue([MarshalAs(UnmanagedType.LPStr)] string ename);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Listenerf(int param,float value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Listener3f(int param,float value1,float value2,float value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Listenerfv(int param,[MarshalAs(UnmanagedType.LPArray)] float[] values); 
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Listeneri(int param,int value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Listener3i(int param,int value1,int value2,int value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Listeneriv(int param,[MarshalAs(UnmanagedType.LPArray)] int[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetListenerf(int param,out float value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetListener3f(int param,out float value1,out float value2,out float value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetListenerfv(int param,[MarshalAs(UnmanagedType.LPArray)] float[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetListeneri(int param,out int value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetListener3i(int param,out int value1,out int value2,out int value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetListeneriv(int param,[MarshalAs(UnmanagedType.LPArray)] int[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GenSources(int n,[MarshalAs(UnmanagedType.LPArray)] uint[] sources); 
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GenSource(int n,out uint source);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void DeleteSources(int n,[MarshalAs(UnmanagedType.LPArray)] uint[] sources);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void DeleteSource(int n,ref uint source);
		[SuppressUnmanagedCodeSecurity]
		internal delegate bool IsSource(uint sid); 
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Sourcef(uint sid,int param,float value); 
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Source3f(uint sid,int param,float value1,float value2,float value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Sourcefv(uint sid,int param,[MarshalAs(UnmanagedType.LPArray)] float[] values); 
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Sourcei(uint sid,int param,int value); 
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Source3i(uint sid,int param,int value1,int value2,int value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Sourceiv(uint sid,int param,[MarshalAs(UnmanagedType.LPArray)] int[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetSourcef(uint sid,int param,out float value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetSource3f(uint sid,int param,out float value1,out float value2,out float value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetSourcefv(uint sid,int param,[MarshalAs(UnmanagedType.LPArray)] float[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetSourcei(uint sid,int param,out int value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetSource3i(uint sid,int param,out int value1,out int value2,out int value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetSourceiv(uint sid,int param,[MarshalAs(UnmanagedType.LPArray)] int[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourcePlayv(int ns,[MarshalAs(UnmanagedType.LPArray)] uint[]sids);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourceStopv(int ns,[MarshalAs(UnmanagedType.LPArray)] uint[]sids);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourceRewindv(int ns,[MarshalAs(UnmanagedType.LPArray)] uint[]sids);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourcePausev(int ns,[MarshalAs(UnmanagedType.LPArray)] uint[]sids);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourcePlay(uint sid);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourceStop(uint sid);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourceRewind(uint sid);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourcePause(uint sid);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourceQueueBuffers(uint sid,int numEntries,[MarshalAs(UnmanagedType.LPArray)] uint[]bids);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SourceUnqueueBuffers(uint sid,int numEntries,[MarshalAs(UnmanagedType.LPArray)] uint[]bids);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GenBuffers(int n,[MarshalAs(UnmanagedType.LPArray)] uint[] buffers);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GenBuffer(int n,out uint buffer);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void DeleteBuffers(int n,[MarshalAs(UnmanagedType.LPArray)] uint[] buffers);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void DeleteBuffer(int n,ref uint buffer);
		[SuppressUnmanagedCodeSecurity]
		internal delegate bool IsBuffer(uint bid);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void BufferData(uint bid,int format,IntPtr data,int size,int freq);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Bufferf(uint bid,int param,float value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Buffer3f(uint bid,int param,float value1,float value2,float value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Bufferfv(uint bid,int param,[MarshalAs(UnmanagedType.LPArray)] float[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Bufferi(uint bid,int param,int value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Buffer3i(uint bid,int param,int value1,int value2,int value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Bufferiv(uint bid,int param,[MarshalAs(UnmanagedType.LPArray)] int[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetBufferf(uint bid,int param,out float value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetBuffer3f(uint bid,int param,out float value1,out float value2,out float value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetBufferfv(uint bid,int param,[MarshalAs(UnmanagedType.LPArray)] float[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetBufferi(uint bid,int param,out int value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetBuffer3i(uint bid,int param,out int value1,out int value2,out int value3);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetBufferiv(uint bid,int param,[MarshalAs(UnmanagedType.LPArray)] int[] values);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void DopplerFactor(float value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void DopplerVelocity(float value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SpeedOfSound(float value);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void DistanceModel(int distanceModel);

		internal static Enable alEnable;
		internal static Disable alDisable;
		internal static IsEnabled alIsEnabled;
		internal static GetString alGetString;
		internal static GetBooleanv alGetBooleanv;
		internal static GetIntegerv alGetIntegerv;
		internal static GetFloatv alGetFloatv;
		internal static GetDoublev alGetDoublev;
		internal static GetBoolean alGetBoolean;
		internal static GetInteger alGetInteger;
		internal static GetFloat alGetFloat;
		internal static GetDouble alGetDouble;
		internal static GetError alGetError;
		internal static IsExtensionPresent alIsExtensionPresent;
		internal static GetProcAddress alGetProcAddress;
		internal static GetEnumValue alGetEnumValue;
		internal static Listenerf alListenerf;
		internal static Listener3f alListener3f;
		internal static Listenerfv alListenerfv;
		internal static Listeneri alListeneri;
		internal static Listener3i alListener3i;
		internal static Listeneriv alListeneriv;
		internal static GetListenerf alGetListenerf;
		internal static GetListener3f alGetListener3f;
		internal static GetListenerfv alGetListenerfv;
		internal static GetListeneri alGetListeneri;
		internal static GetListener3i alGetListener3i;
		internal static GetListeneriv alGetListeneriv;
		internal static GenSources alGenSources;
		internal static GenSource alGenSource;
		internal static DeleteSources alDeleteSources;
		internal static DeleteSource alDeleteSource;
		internal static IsSource alIsSource;
		internal static Sourcef alSourcef;
		internal static Source3f alSource3f;
		internal static Sourcefv alSourcefv;
		internal static Sourcei alSourcei;
		internal static Source3i alSource3i;
		internal static Sourceiv alSourceiv;
		internal static GetSourcef alGetSourcef;
		internal static GetSource3f alGetSource3f;
		internal static GetSourcefv alGetSourcefv;
		internal static GetSourcei alGetSourcei;
		internal static GetSource3i alGetSource3i;
		internal static GetSourceiv alGetSourceiv;
		internal static SourcePlayv alSourcePlayv;
		internal static SourceStopv alSourceStopv;
		internal static SourceRewindv alSourceRewindv;
		internal static SourcePausev alSourcePausev;
		internal static SourcePlay alSourcePlay;
		internal static SourceStop alSourceStop;
		internal static SourceRewind alSourceRewind;
		internal static SourcePause alSourcePause;
		internal static SourceQueueBuffers alSourceQueueBuffers;
		internal static SourceUnqueueBuffers alSourceUnqueueBuffers;
		internal static GenBuffers alGenBuffers;
		internal static GenBuffer alGenBuffer;
		internal static DeleteBuffers alDeleteBuffers;
		internal static DeleteBuffer alDeleteBuffer;
		internal static IsBuffer alIsBuffer;
		internal static BufferData alBufferData;
		internal static Bufferf alBufferf;
		internal static Buffer3f alBuffer3f;
		internal static Bufferfv alBufferfv;
		internal static Bufferi alBufferi;
		internal static Buffer3i alBuffer3i;
		internal static Bufferiv alBufferiv;
		internal static GetBufferf alGetBufferf;
		internal static GetBuffer3f alGetBuffer3f;
		internal static GetBufferfv alGetBufferfv;
		internal static GetBufferi alGetBufferi;
		internal static GetBuffer3i alGetBuffer3i;
		internal static GetBufferiv alGetBufferiv;
		internal static DopplerFactor alDopplerFactor;
		internal static DopplerVelocity alDopplerVelocity;
		internal static SpeedOfSound alSpeedOfSound;
		internal static DistanceModel alDistanceModel;
	}
}

