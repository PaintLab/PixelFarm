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
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Pencil.Gaming.MathUtils {
	/// <summary>
	/// Represents a 3D vector using three single-precision floating-point numbers.
	/// </summary>
	/// <remarks>
	/// The Vector3i structure is suitable for interoperation with unmanaged code requiring three consecutive floats.
	/// </remarks>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3i : IEquatable<Vector3i> {
		/// <summary>
		/// The X component of the Vector3i.
		/// </summary>
		public int X;

		/// <summary>
		/// The Y component of the Vector3i.
		/// </summary>
		public int Y;

		/// <summary>
		/// The Z component of the Vector3i.
		/// </summary>
		public int Z;

		/// <summary>
		/// Defines a unit-length Vector3i that points towards the X-axis.
		/// </summary>
		public static readonly Vector3i UnitX = new Vector3i(1, 0, 0);

		/// <summary>
		/// Defines a unit-length Vector3i that points towards the Y-axis.
		/// </summary>
		public static readonly Vector3i UnitY = new Vector3i(0, 1, 0);

		/// <summary>
		/// /// Defines a unit-length Vector3i that points towards the Z-axis.
		/// </summary>
		public static readonly Vector3i UnitZ = new Vector3i(0, 0, 1);

		/// <summary>
		/// Defines a zero-length Vector3i.
		/// </summary>
		public static readonly Vector3i Zero = new Vector3i(0, 0, 0);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector3i One = new Vector3i(1, 1, 1);

		/// <summary>
		/// Defines the size of the Vector3i struct in bytes.
		/// </summary>
		public const int SizeInBytes = sizeof(int) * 3;

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="value">The value that will initialize this instance.</param>
		public Vector3i(int value) {
			X = value;
			Y = value;
			Z = value;
		}

		/// <summary>
		/// Constructs a new Vector3i.
		/// </summary>
		/// <param name="x">The x component of the Vector3i.</param>
		/// <param name="y">The y component of the Vector3i.</param>
		/// <param name="z">The z component of the Vector3i.</param>
		public Vector3i(int x, int y, int z) {
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Constructs a new Vector3i from the given Vector2.
		/// </summary>
		/// <param name="v">The Vector2 to copy components from.</param>
		public Vector3i(Vector2i v) {
			X = v.X;
			Y = v.Y;
			Z = 0;
		}

		/// <summary>
		/// Constructs a new Vector3i from the given Vector3i.
		/// </summary>
		/// <param name="v">The Vector3i to copy components from.</param>
		public Vector3i(Vector3i v) {
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		/// <summary>
		/// Constructs a new Vector3i from the given Vector4.
		/// </summary>
		/// <param name="v">The Vector4 to copy components from.</param>
		public Vector3i(Vector4i v) {
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}


		/// <summary>
		/// Gets or sets the value at the index of the Vector.
		/// </summary>
		public int this[int index] {
			get {
				if (index == 0)
					return X;
				else if (index == 1)
					return Y;
				else if (index == 2)
					return Z;
				throw new IndexOutOfRangeException("You tried to access this vector at index: " + index);
			} set {
				if (index == 0)
					X = value;
				else if (index == 1)
					Y = value;
				else if (index == 2)
					Z = value;
				throw new IndexOutOfRangeException("You tried to set this vector at index: " + index);
			}
		}

		/// <summary>
		/// Gets the length (magnitude) of the vector.
		/// </summary>
		/// <see cref="LengthFast"/>
		/// <seealso cref="LengthSquared"/>
		public float Length {
			get { return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z); }
		}

		/// <summary>
		/// Gets an approximation of the vector length (magnitude).
		/// </summary>
		/// <remarks>
		/// This property uses an approximation of the square root function to calculate vector magnitude, with
		/// an upper error bound of 0.001.
		/// </remarks>
		/// <see cref="Length"/>
		/// <seealso cref="LengthSquared"/>
		public float LengthFast {
			get { return 1.0f / MathHelper.InverseSqrtFast(X * X + Y * Y + Z * Z); }
		}

		/// <summary>
		/// Gets the square of the vector length (magnitude).
		/// </summary>
		/// <remarks>
		/// This property avoids the costly square root operation required by the Length property. This makes it more suitable
		/// for comparisons.
		/// </remarks>
		/// <see cref="Length"/>
		/// <seealso cref="LengthFast"/>
		public int LengthSquared {
			get { return X * X + Y * Y + Z * Z; }
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <returns>Result of operation.</returns>
		public static Vector3i Add(Vector3i a, Vector3i b) {
			Add(ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <param name="result">Result of operation.</param>
		public static void Add(ref Vector3i a, ref Vector3i b, out Vector3i result) {
			result = new Vector3i(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static Vector3i Subtract(Vector3i a, Vector3i b) {
			Subtract(ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Subtract(ref Vector3i a, ref Vector3i b, out Vector3i result) {
			result = new Vector3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector3i Multiply(Vector3i vector, int scale) {
			Multiply(ref vector, scale, out vector);
			return vector;
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector3i vector, int scale, out Vector3i result) {
			result = new Vector3i(vector.X * scale, vector.Y * scale, vector.Z * scale);
		}

		/// <summary>
		/// Multiplies a vector by the components a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector3i Multiply(Vector3i vector, Vector3i scale) {
			Multiply(ref vector, ref scale, out vector);
			return vector;
		}

		/// <summary>
		/// Multiplies a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector3i vector, ref Vector3i scale, out Vector3i result) {
			result = new Vector3i(vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z);
		}

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector3i Divide(Vector3i vector, int scale) {
			Divide(ref vector, scale, out vector);
			return vector;
		}

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector3i vector, int scale, out Vector3i result) {
			Multiply(ref vector, 1 / scale, out result);
		}

		/// <summary>
		/// Divides a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector3i Divide(Vector3i vector, Vector3i scale) {
			Divide(ref vector, ref scale, out vector);
			return vector;
		}

		/// <summary>
		/// Divide a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector3i vector, ref Vector3i scale, out Vector3i result) {
			result = new Vector3i(vector.X / scale.X, vector.Y / scale.Y, vector.Z / scale.Z);
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static Vector3i ComponentMin(Vector3i a, Vector3i b) {
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			a.Z = a.Z < b.Z ? a.Z : b.Z;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void ComponentMin(ref Vector3i a, ref Vector3i b, out Vector3i result) {
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
			result.Z = a.Z < b.Z ? a.Z : b.Z;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static Vector3i ComponentMax(Vector3i a, Vector3i b) {
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			a.Z = a.Z > b.Z ? a.Z : b.Z;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void ComponentMax(ref Vector3i a, ref Vector3i b, out Vector3i result) {
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
			result.Z = a.Z > b.Z ? a.Z : b.Z;
		}

		/// <summary>
		/// Returns the Vector3i with the minimum magnitude
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param>
		/// <returns>The minimum Vector3i</returns>
		public static Vector3i Min(Vector3i left, Vector3i right) {
			return left.LengthSquared < right.LengthSquared ? left : right;
		}

		/// <summary>
		/// Returns the Vector3i with the minimum magnitude
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param>
		/// <returns>The minimum Vector3i</returns>
		public static Vector3i Max(Vector3i left, Vector3i right) {
			return left.LengthSquared >= right.LengthSquared ? left : right;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <returns>The clamped vector</returns>
		public static Vector3i Clamp(Vector3i vec, Vector3i min, Vector3i max) {
			vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			vec.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
			return vec;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <param name="result">The clamped vector</param>
		public static void Clamp(ref Vector3i vec, ref Vector3i min, ref Vector3i max, out Vector3i result) {
			result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			result.Z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
		}

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The dot product of the two inputs</returns>
		public static int Dot(Vector3i left, Vector3i right) {
			return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
		}

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The dot product of the two inputs</param>
		public static void Dot(ref Vector3i left, ref Vector3i right, out int result) {
			result = left.X * right.X + left.Y * right.Y + left.Z * right.Z;
		}

		/// <summary>
		/// Caclulate the cross (vector) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The cross product of the two inputs</returns>
		public static Vector3i Cross(Vector3i left, Vector3i right) {
			Vector3i result;
			Cross(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Caclulate the cross (vector) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The cross product of the two inputs</returns>
		/// <param name="result">The cross product of the two inputs</param>
		public static void Cross(ref Vector3i left, ref Vector3i right, out Vector3i result) {
			result = new Vector3i(left.Y * right.Z - left.Z * right.Y,
				left.Z * right.X - left.X * right.Z,
				left.X * right.Y - left.Y * right.X);
		}

		/// <summary>
		/// Calculates the angle (in radians) between two vectors.
		/// </summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
		/// <returns>Angle (in radians) between the vectors.</returns>
		/// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
		public static float CalculateAngle(Vector3i first, Vector3i second) {
			return (float)System.Math.Acos((Vector3i.Dot(first, second)) / (first.Length * second.Length));
		}

		/// <summary>Calculates the angle (in radians) between two vectors.</summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
		/// <param name="result">Angle (in radians) between the vectors.</param>
		/// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
		public static void CalculateAngle(ref Vector3i first, ref Vector3i second, out float result) {
			int temp;
			Vector3i.Dot(ref first, ref second, out temp);
			result = (float)System.Math.Acos(temp / (first.Length * second.Length));
		}

		/// <summary>
		/// Gets or sets an Vector2 with the X and Y components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2i Xy {
			get { return new Vector2i(X, Y); }
			set {
				X = value.X;
				Y = value.Y;
			}
		}

		/// <summary>
		/// Gets or sets an Vector2 with the X and Z components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2i Xz {
			get { return new Vector2i(X, Z); }
			set {
				X = value.X;
				Z = value.Y;
			}
		}

		/// <summary>
		/// Gets or sets an Vector2 with the Y and X components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2i Yx {
			get { return new Vector2i(Y, X); }
			set {
				Y = value.X;
				X = value.Y;
			}
		}

		/// <summary>
		/// Gets or sets an Vector2 with the Y and Z components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2i Yz {
			get { return new Vector2i(Y, Z); }
			set {
				Y = value.X;
				Z = value.Y;
			}
		}

		/// <summary>
		/// Gets or sets an Vector2 with the Z and X components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2i Zx {
			get { return new Vector2i(Z, X); }
			set {
				Z = value.X;
				X = value.Y;
			}
		}

		/// <summary>
		/// Gets or sets an Vector2 with the Z and Y components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2i Zy {
			get { return new Vector2i(Z, Y); }
			set {
				Z = value.X;
				Y = value.Y;
			}
		}

		/// <summary>
		/// Gets or sets an Vector3i with the X, Z, and Y components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector3i Xzy {
			get { return new Vector3i(X, Z, Y); }
			set {
				X = value.X;
				Z = value.Y;
				Y = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets an Vector3i with the Y, X, and Z components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector3i Yxz {
			get { return new Vector3i(Y, X, Z); }
			set {
				Y = value.X;
				X = value.Y;
				Z = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets an Vector3i with the Y, Z, and X components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector3i Yzx {
			get { return new Vector3i(Y, Z, X); }
			set {
				Y = value.X;
				Z = value.Y;
				X = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets an Vector3i with the Z, X, and Y components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector3i Zxy {
			get { return new Vector3i(Z, X, Y); }
			set {
				Z = value.X;
				X = value.Y;
				Y = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets an Vector3i with the Z, Y, and X components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector3i Zyx {
			get { return new Vector3i(Z, Y, X); }
			set {
				Z = value.X;
				Y = value.Y;
				X = value.Z;
			}
		}

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator +(Vector3i left, Vector3i right) {
			left.X += right.X;
			left.Y += right.Y;
			left.Z += right.Z;
			return left;
		}

		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator -(Vector3i left, Vector3i right) {
			left.X -= right.X;
			left.Y -= right.Y;
			left.Z -= right.Z;
			return left;
		}

		/// <summary>
		/// Negates an instance.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator -(Vector3i vec) {
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			vec.Z = -vec.Z;
			return vec;
		}

		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator *(Vector3i vec, int scale) {
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="scale">The scalar.</param>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator *(int scale, Vector3i vec) {
			vec.X *= scale;
			vec.Y *= scale;
			vec.Z *= scale;
			return vec;
		}

		/// <summary>
		/// Divides an instance by a scalar.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3i operator /(Vector3i vec, int scale) {
			vec.X /= scale;
			vec.Y /= scale;
			vec.Z /= scale;
			return vec;
		}

		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator ==(Vector3i left, Vector3i right) {
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equa lright; false otherwise.</returns>
		public static bool operator !=(Vector3i left, Vector3i right) {
			return !left.Equals(right);
		}

		internal static string listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
		/// <summary>
		/// Returns a System.String that represents the current Vector3i.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return String.Format("({0}{3} {1}{3} {2})", X, Y, Z, listSeparator);
		}

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals(object obj) {
			if (!(obj is Vector3i))
				return false;

			return this.Equals((Vector3i)obj);
		}

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals(Vector3i other) {
			return
				X == other.X &&
				Y == other.Y &&
				Z == other.Z;
		}
	}
}
