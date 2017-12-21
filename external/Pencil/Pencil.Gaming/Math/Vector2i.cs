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
	/// <summary>Represents a 2D vector using two single-precision inting-point numbers.</summary>
	/// <remarks>
	/// The Vector2i structure is suitable for interoperation with unmanaged code requiring two consecutive ints.
	/// </remarks>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2i : IEquatable<Vector2i> {
		/// <summary>
		/// The X component of the Vector2i.
		/// </summary>
		public int X;

		/// <summary>
		/// The Y component of the Vector2i.
		/// </summary>
		public int Y;

		/// <summary>
		/// Defines a unit-length Vector2i that points towards the X-axis.
		/// </summary>
		public static readonly Vector2i UnitX = new Vector2i(1, 0);

		/// <summary>
		/// Defines a unit-length Vector2i that points towards the Y-axis.
		/// </summary>
		public static readonly Vector2i UnitY = new Vector2i(0, 1);

		/// <summary>
		/// Defines a zero-length Vector2i.
		/// </summary>
		public static readonly Vector2i Zero = new Vector2i(0, 0);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector2i One = new Vector2i(1, 1);

		/// <summary>
		/// Defines the size of the Vector2i struct in bytes.
		/// </summary>
		public const int SizeInBytes = sizeof(int) * 2;

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="value">The value that will initialize this instance.</param>
		public Vector2i(int value) {
			X = value;
			Y = value;
		}

		/// <summary>
		/// Constructs a new Vector2i.
		/// </summary>
		/// <param name="x">The x coordinate of the net Vector2i.</param>
		/// <param name="y">The y coordinate of the net Vector2i.</param>
		public Vector2i(int x, int y) {
			X = x;
			Y = y;
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
				throw new IndexOutOfRangeException("You tried to access this vector at index: " + index);
			} set {
				if (index == 0)
					X = value;
				else if (index == 1)
					Y = value;
				throw new IndexOutOfRangeException("You tried to set this vector at index: " + index);
			}
		}

		/// <summary>
		/// Gets the length (magnitude) of the vector.
		/// </summary>
		/// <see cref="LengthFast"/>
		/// <seealso cref="LengthSquared"/>
		public float Length {
			get { return (float)System.Math.Sqrt(X * X + Y * Y); }
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
			get { return 1.0f / MathHelper.InverseSqrtFast(X * X + Y * Y); }
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
			get { return X * X + Y * Y; }
		}

		/// <summary>
		/// Gets the perpendicular vector on the right side of this vector.
		/// </summary>
		public Vector2i PerpendicularRight {
			get { return new Vector2i(Y, -X); }
		}

		/// <summary>
		/// Gets the perpendicular vector on the left side of this vector.
		/// </summary>
		public Vector2i PerpendicularLeft {
			get { return new Vector2i(-Y, X); }
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <returns>Result of operation.</returns>
		public static Vector2i Add(Vector2i a, Vector2i b) {
			Add(ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <param name="result">Result of operation.</param>
		public static void Add(ref Vector2i a, ref Vector2i b, out Vector2i result) {
			result = new Vector2i(a.X + b.X, a.Y + b.Y);
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static Vector2i Subtract(Vector2i a, Vector2i b) {
			Subtract(ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Subtract(ref Vector2i a, ref Vector2i b, out Vector2i result) {
			result = new Vector2i(a.X - b.X, a.Y - b.Y);
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector2i Multiply(Vector2i vector, int scale) {
			Multiply(ref vector, scale, out vector);
			return vector;
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector2i vector, int scale, out Vector2i result) {
			result = new Vector2i(vector.X * scale, vector.Y * scale);
		}

		/// <summary>
		/// Multiplies a vector by the components a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector2i Multiply(Vector2i vector, Vector2i scale) {
			Multiply(ref vector, ref scale, out vector);
			return vector;
		}

		/// <summary>
		/// Multiplies a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector2i vector, ref Vector2i scale, out Vector2i result) {
			result = new Vector2i(vector.X * scale.X, vector.Y * scale.Y);
		}

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector2i Divide(Vector2i vector, int scale) {
			Divide(ref vector, scale, out vector);
			return vector;
		}

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector2i vector, int scale, out Vector2i result) {
			Multiply(ref vector, 1 / scale, out result);
		}

		/// <summary>
		/// Divides a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector2i Divide(Vector2i vector, Vector2i scale) {
			Divide(ref vector, ref scale, out vector);
			return vector;
		}

		/// <summary>
		/// Divide a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector2i vector, ref Vector2i scale, out Vector2i result) {
			result = new Vector2i(vector.X / scale.X, vector.Y / scale.Y);
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static Vector2i ComponentMin(Vector2i a, Vector2i b) {
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void ComponentMin(ref Vector2i a, ref Vector2i b, out Vector2i result) {
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static Vector2i ComponentMax(Vector2i a, Vector2i b) {
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void ComponentMax(ref Vector2i a, ref Vector2i b, out Vector2i result) {
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
		}

		/// <summary>
		/// Returns the Vector3 with the minimum magnitude
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param>
		/// <returns>The minimum Vector3</returns>
		public static Vector2i Min(Vector2i left, Vector2i right) {
			return left.LengthSquared < right.LengthSquared ? left : right;
		}

		/// <summary>
		/// Returns the Vector3 with the minimum magnitude
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param>
		/// <returns>The minimum Vector3</returns>
		public static Vector2i Max(Vector2i left, Vector2i right) {
			return left.LengthSquared >= right.LengthSquared ? left : right;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <returns>The clamped vector</returns>
		public static Vector2i Clamp(Vector2i vec, Vector2i min, Vector2i max) {
			vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			return vec;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <param name="result">The clamped vector</param>
		public static void Clamp(ref Vector2i vec, ref Vector2i min, ref Vector2i max, out Vector2i result) {
			result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
		}

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The dot product of the two inputs</returns>
		public static int Dot(Vector2i left, Vector2i right) {
			return left.X * right.X + left.Y * right.Y;
		}

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The dot product of the two inputs</param>
		public static void Dot(ref Vector2i left, ref Vector2i right, out int result) {
			result = left.X * right.X + left.Y * right.Y;
		}

		/// <summary>
		/// Calculate the perpendicular dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The perpendicular dot product of the two inputs</returns>
		public static int PerpDot(Vector2i left, Vector2i right) {
			return left.X * right.Y - left.Y * right.X;
		}

		/// <summary>
		/// Calculate the perpendicular dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The perpendicular dot product of the two inputs</param>
		public static void PerpDot(ref Vector2i left, ref Vector2i right, out int result) {
			result = left.X * right.Y - left.Y * right.X;
		}

		/// <summary>
		/// Gets or sets an Vector2i with the Y and X components of this instance.
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
		/// Adds the specified instances.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>Result of addition.</returns>
		public static Vector2i operator +(Vector2i left, Vector2i right) {
			left.X += right.X;
			left.Y += right.Y;
			return left;
		}

		/// <summary>
		/// Subtracts the specified instances.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>Result of subtraction.</returns>
		public static Vector2i operator -(Vector2i left, Vector2i right) {
			left.X -= right.X;
			left.Y -= right.Y;
			return left;
		}

		/// <summary>
		/// Negates the specified instance.
		/// </summary>
		/// <param name="vec">Operand.</param>
		/// <returns>Result of negation.</returns>
		public static Vector2i operator -(Vector2i vec) {
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			return vec;
		}

		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="vec">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2i operator *(Vector2i vec, int scale) {
			vec.X *= scale;
			vec.Y *= scale;
			return vec;
		}

		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="scale">Left operand.</param>
		/// <param name="vec">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2i operator *(int scale, Vector2i vec) {
			vec.X *= scale;
			vec.Y *= scale;
			return vec;
		}

		/// <summary>
		/// Divides the specified instance by a scalar.
		/// </summary>
		/// <param name="vec">Left operand</param>
		/// <param name="scale">Right operand</param>
		/// <returns>Result of the division.</returns>
		public static Vector2i operator /(Vector2i vec, int scale) {
			vec.X /= scale;
			vec.Y /= scale;
			return vec;
		}

		/// <summary>
		/// Compares the specified instances for equality.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>True if both instances are equal; false otherwise.</returns>
		public static bool operator ==(Vector2i left, Vector2i right) {
			return left.Equals(right);
		}

		/// <summary>
		/// Compares the specified instances for inequality.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>True if both instances are not equal; false otherwise.</returns>
		public static bool operator !=(Vector2i left, Vector2i right) {
			return !left.Equals(right);
		}

		internal static string listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
		/// <summary>
		/// Returns a System.String that represents the current Vector2i.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return String.Format("({0}{2} {1})", X, Y, listSeparator);
		}

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals(object obj) {
			if (!(obj is Vector2i))
				return false;

			return this.Equals((Vector2i)obj);
		}

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals(Vector2i other) {
			return
				X == other.X &&
				Y == other.Y;
		}
	}
}
