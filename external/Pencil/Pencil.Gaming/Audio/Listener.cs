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
using Pencil.Gaming.Audio;
using Pencil.Gaming.MathUtils;

namespace Pencil.Gaming.Audio {
	public class Listener {
		private static Listener current = null;

		public Listener(Vector3 orientation, Vector3 position, Vector3 velocity, float efxMetersPerUnit, float gain) {
			Orientation = orientation;
			Position = position;
			Velocity = velocity;
			EfxMetersPerUnit = efxMetersPerUnit;
			Gain = gain;
		}

		public Vector3 Orientation;
		public Vector3 Position;
		public Vector3 Velocity;
		public float EfxMetersPerUnit;
		public float Gain;

		public virtual void MakeCurrent() {
			if (current != this) {
				AL.Listener(ALListener3f.Position, Position.X, Position.Y, Position.Z);
				AL.Listener(ALListener3f.Velocity, Velocity.X, Velocity.Y, Velocity.Z);
				AL.Listener(ALListenerf.EfxMetersPerUnit, EfxMetersPerUnit);
				AL.Listener(ALListenerf.Gain, Gain);
				AL.Listener(ALListenerfv.Orientation, new float[] { Orientation.X, Orientation.Y, Orientation.Z });
				current = this;
			}
		}
	}
}

