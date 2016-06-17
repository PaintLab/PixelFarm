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

namespace Pencil.Gaming.MathUtils {
	/// <summary>
	/// Represents a 4x4 Matrix
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix : IEquatable<Matrix> {
		#region Fields

		/// <summary>
		/// Top row of the matrix.
		/// </summary>
		public Vector4 Row0;

		/// <summary>
		/// 2nd row of the matrix.
		/// </summary>
		public Vector4 Row1;

		/// <summary>
		/// 3rd row of the matrix.
		/// </summary>
		public Vector4 Row2;

		/// <summary>
		/// Bottom row of the matrix.
		/// </summary>
		public Vector4 Row3;
 
		/// <summary>
		/// The identity matrix.
		/// </summary>
		public static readonly Matrix Identity = new Matrix(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);

		/// <summary>
		/// The zero matrix.
		/// </summary>
		public static readonly Matrix Zero = new Matrix(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero);

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="row0">Top row of the matrix.</param>
		/// <param name="row1">Second row of the matrix.</param>
		/// <param name="row2">Third row of the matrix.</param>
		/// <param name="row3">Bottom row of the matrix.</param>
		public Matrix(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3) {
			Row0 = row0;
			Row1 = row1;
			Row2 = row2;
			Row3 = row3;
		}

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="m00">First item of the first row of the matrix.</param>
		/// <param name="m01">Second item of the first row of the matrix.</param>
		/// <param name="m02">Third item of the first row of the matrix.</param>
		/// <param name="m03">Fourth item of the first row of the matrix.</param>
		/// <param name="m10">First item of the second row of the matrix.</param>
		/// <param name="m11">Second item of the second row of the matrix.</param>
		/// <param name="m12">Third item of the second row of the matrix.</param>
		/// <param name="m13">Fourth item of the second row of the matrix.</param>
		/// <param name="m20">First item of the third row of the matrix.</param>
		/// <param name="m21">Second item of the third row of the matrix.</param>
		/// <param name="m22">Third item of the third row of the matrix.</param>
		/// <param name="m23">First item of the third row of the matrix.</param>
		/// <param name="m30">Fourth item of the fourth row of the matrix.</param>
		/// <param name="m31">Second item of the fourth row of the matrix.</param>
		/// <param name="m32">Third item of the fourth row of the matrix.</param>
		/// <param name="m33">Fourth item of the fourth row of the matrix.</param>
		public Matrix(
			float m00, float m01, float m02, float m03,
			float m10, float m11, float m12, float m13,
			float m20, float m21, float m22, float m23,
			float m30, float m31, float m32, float m33) {
			Row0 = new Vector4(m00, m01, m02, m03);
			Row1 = new Vector4(m10, m11, m12, m13);
			Row2 = new Vector4(m20, m21, m22, m23);
			Row3 = new Vector4(m30, m31, m32, m33);
		}

		#endregion

		#region Public Members

		#region Properties

		/// <summary>
		/// Gets the determinant of this matrix.
		/// </summary>
		public float Determinant {
			get {
				float m11 = Row0.X, m12 = Row0.Y, m13 = Row0.Z, m14 = Row0.W,
				m21 = Row1.X, m22 = Row1.Y, m23 = Row1.Z, m24 = Row1.W,
				m31 = Row2.X, m32 = Row2.Y, m33 = Row2.Z, m34 = Row2.W,
				m41 = Row3.X, m42 = Row3.Y, m43 = Row3.Z, m44 = Row3.W;

				return
					m11 * m22 * m33 * m44 - m11 * m22 * m34 * m43 + m11 * m23 * m34 * m42 - m11 * m23 * m32 * m44
					+ m11 * m24 * m32 * m43 - m11 * m24 * m33 * m42 - m12 * m23 * m34 * m41 + m12 * m23 * m31 * m44
					- m12 * m24 * m31 * m43 + m12 * m24 * m33 * m41 - m12 * m21 * m33 * m44 + m12 * m21 * m34 * m43
					+ m13 * m24 * m31 * m42 - m13 * m24 * m32 * m41 + m13 * m21 * m32 * m44 - m13 * m21 * m34 * m42
					+ m13 * m22 * m34 * m41 - m13 * m22 * m31 * m44 - m14 * m21 * m32 * m43 + m14 * m21 * m33 * m42
					- m14 * m22 * m33 * m41 + m14 * m22 * m31 * m43 - m14 * m23 * m31 * m42 + m14 * m23 * m32 * m41;
			}
		}

		/// <summary>
		/// Gets the first column of this matrix.
		/// </summary>
		public Vector4 Column0 {
			get { return new Vector4(Row0.X, Row1.X, Row2.X, Row3.X); }
			set {
				Row0.X = value.X;
				Row1.X = value.Y;
				Row2.X = value.Z;
				Row3.X = value.W;
			}
		}

		/// <summary>
		/// Gets the second column of this matrix.
		/// </summary>
		public Vector4 Column1 {
			get { return new Vector4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
			set {
				Row0.Y = value.X;
				Row1.Y = value.Y;
				Row2.Y = value.Z;
				Row3.Y = value.W;
			}
		}

		/// <summary>
		/// Gets the third column of this matrix.
		/// </summary>
		public Vector4 Column2 {
			get { return new Vector4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
			set {
				Row0.Z = value.X;
				Row1.Z = value.Y;
				Row2.Z = value.Z;
				Row3.Z = value.W;
			}
		}

		/// <summary>
		/// Gets the fourth column of this matrix.
		/// </summary>
		public Vector4 Column3 {
			get { return new Vector4(Row0.W, Row1.W, Row2.W, Row3.W); }
			set {
				Row0.W = value.X;
				Row1.W = value.Y;
				Row2.W = value.Z;
				Row3.W = value.W;
			}
		}

		/// <summary>
		/// Gets or sets the value at row 1, column 1 of this instance.
		/// </summary>
		public float M11 { get { return Row0.X; } set { Row0.X = value; } }

		/// <summary>
		/// Gets or sets the value at row 1, column 2 of this instance.
		/// </summary>
		public float M12 { get { return Row0.Y; } set { Row0.Y = value; } }

		/// <summary>
		/// Gets or sets the value at row 1, column 3 of this instance.
		/// </summary>
		public float M13 { get { return Row0.Z; } set { Row0.Z = value; } }

		/// <summary>
		/// Gets or sets the value at row 1, column 4 of this instance.
		/// </summary>
		public float M14 { get { return Row0.W; } set { Row0.W = value; } }

		/// <summary>
		/// Gets or sets the value at row 2, column 1 of this instance.
		/// </summary>
		public float M21 { get { return Row1.X; } set { Row1.X = value; } }

		/// <summary>
		/// Gets or sets the value at row 2, column 2 of this instance.
		/// </summary>
		public float M22 { get { return Row1.Y; } set { Row1.Y = value; } }

		/// <summary>
		/// Gets or sets the value at row 2, column 3 of this instance.
		/// </summary>
		public float M23 { get { return Row1.Z; } set { Row1.Z = value; } }

		/// <summary>
		/// Gets or sets the value at row 2, column 4 of this instance.
		/// </summary>
		public float M24 { get { return Row1.W; } set { Row1.W = value; } }

		/// <summary>
		/// Gets or sets the value at row 3, column 1 of this instance.
		/// </summary>
		public float M31 { get { return Row2.X; } set { Row2.X = value; } }

		/// <summary>
		/// Gets or sets the value at row 3, column 2 of this instance.
		/// </summary>
		public float M32 { get { return Row2.Y; } set { Row2.Y = value; } }

		/// <summary>
		/// Gets or sets the value at row 3, column 3 of this instance.
		/// </summary>
		public float M33 { get { return Row2.Z; } set { Row2.Z = value; } }

		/// <summary>
		/// Gets or sets the value at row 3, column 4 of this instance.
		/// </summary>
		public float M34 { get { return Row2.W; } set { Row2.W = value; } }

		/// <summary>
		/// Gets or sets the value at row 4, column 1 of this instance.
		/// </summary>
		public float M41 { get { return Row3.X; } set { Row3.X = value; } }

		/// <summary>
		/// Gets or sets the value at row 4, column 2 of this instance.
		/// </summary>
		public float M42 { get { return Row3.Y; } set { Row3.Y = value; } }

		/// <summary>
		/// Gets or sets the value at row 4, column 3 of this instance.
		/// </summary>
		public float M43 { get { return Row3.Z; } set { Row3.Z = value; } }

		/// <summary>
		/// Gets or sets the value at row 4, column 4 of this instance.
		/// </summary>
		public float M44 { get { return Row3.W; } set { Row3.W = value; } }

		#endregion

		#region Indexers

		/// <summary>
		/// Gets or sets the value at a specified row and column.
		/// </summary>
		public float this [int rowIndex, int columnIndex] {
			get {
				if (rowIndex == 0) {
					return Row0 [columnIndex];
				} else if (rowIndex == 1) {
					return Row1 [columnIndex];
				} else if (rowIndex == 2) {
					return Row2 [columnIndex];
				} else if (rowIndex == 3) {
					return Row3 [columnIndex];
				}
				throw new IndexOutOfRangeException("You tried to access this matrix at: (" + rowIndex + ", " + columnIndex + ")");
			}
			set {
				if (rowIndex == 0) {
					Row0 [columnIndex] = value;
				} else if (rowIndex == 1) {
					Row1 [columnIndex] = value;
				} else if (rowIndex == 2) {
					Row2 [columnIndex] = value;
				} else if (rowIndex == 3) {
					Row3 [columnIndex] = value;
				}
				throw new IndexOutOfRangeException("You tried to set this matrix at: (" + rowIndex + ", " + columnIndex + ")");
			}
		}

		#endregion

		#region Instance

		#region public void Invert()

		/// <summary>
		/// Converts this instance into its inverse.
		/// </summary>
		public void Invert() {
			this = Matrix.Invert(this);
		}

		#endregion

		#region public void Transpose()

		/// <summary>
		/// Converts this instance into its transpose.
		/// </summary>
		public void Transpose() {
			this = Matrix.Transpose(this);
		}

		#endregion

		#endregion

		#region Static
		
		#region CreateFromAxisAngle
		
		/// <summary>
		/// Build a rotation matrix from the specified axis/angle rotation.
		/// </summary>
		/// <param name="axis">The axis to rotate about.</param>
		/// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
		/// <param name="result">A matrix instance.</param>
		public static void CreateFromAxisAngle(Vector3 axis, float angle, out Matrix result) {
			// normalize and create a local copy of the vector.
			axis.Normalize();
			float axisX = axis.X, axisY = axis.Y, axisZ = axis.Z;

			// calculate angles
			float cos = (float)System.Math.Cos(-angle);
			float sin = (float)System.Math.Sin(-angle);
			float t = 1.0f - cos;

			// do the conversion math once
			float tXX = t * axisX * axisX,
			tXY = t * axisX * axisY,
			tXZ = t * axisX * axisZ,
			tYY = t * axisY * axisY,
			tYZ = t * axisY * axisZ,
			tZZ = t * axisZ * axisZ;

			float sinX = sin * axisX,
			sinY = sin * axisY,
			sinZ = sin * axisZ;

			result.Row0.X = tXX + cos;
			result.Row0.Y = tXY - sinZ;
			result.Row0.Z = tXZ + sinY;
			result.Row0.W = 0;
			result.Row1.X = tXY + sinZ;
			result.Row1.Y = tYY + cos;
			result.Row1.Z = tYZ - sinX;
			result.Row1.W = 0;
			result.Row2.X = tXZ - sinY;
			result.Row2.Y = tYZ + sinX;
			result.Row2.Z = tZZ + cos;
			result.Row2.W = 0;
			result.Row3 = Vector4.UnitW;
		}
		
		/// <summary>
		/// Build a rotation matrix from the specified axis/angle rotation.
		/// </summary>
		/// <param name="axis">The axis to rotate about.</param>
		/// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
		/// <returns>A matrix instance.</returns>
		public static Matrix CreateFromAxisAngle(Vector3 axis, float angle) {
			Matrix result;
			CreateFromAxisAngle(axis, angle, out result);
			return result;
		}
		
		#endregion

		#region CreateFromQuaternion

		/// <summary>
		/// Builds a rotation matrix from a quaternion.
		/// </summary>
		/// <param name="q">The quaternion to rotate by.</param>
		/// <param name="result">A matrix instance.</param>
		public static void CreateFromQuaternion(ref Quaternion q, out Matrix result) {
			Vector3 axis;
			float angle;
			q.ToAxisAngle(out axis, out angle);
			CreateFromAxisAngle(axis, angle, out result);
		}

		/// <summary>
		/// Builds a rotation matrix from a quaternion.
		/// </summary>
		/// <param name="q">The quaternion to rotate by.</param>
		/// <returns>A matrix instance.</returns>
		public static Matrix CreateFromQuaternion(Quaternion q) {
			Matrix result;
			CreateFromQuaternion(ref q, out result);
			return result;
		}

		#endregion

		#region CreateRotation[XYZ]

		/// <summary>
		/// Builds a rotation matrix for a rotation around the x-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix4 instance.</param>
		public static void CreateRotationX(float angle, out Matrix result) {
			float cos = (float)System.Math.Cos(angle);
			float sin = (float)System.Math.Sin(angle);

			result = Identity;
			result.Row1.Y = cos;
			result.Row1.Z = sin;
			result.Row2.Y = -sin;
			result.Row2.Z = cos;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the x-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting Matrix4 instance.</returns>
		public static Matrix CreateRotationX(float angle) {
			Matrix result;
			CreateRotationX(angle, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the y-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix4 instance.</param>
		public static void CreateRotationY(float angle, out Matrix result) {
			float cos = (float)System.Math.Cos(angle);
			float sin = (float)System.Math.Sin(angle);

			result = Identity;
			result.Row0.X = cos;
			result.Row0.Z = -sin;
			result.Row2.X = sin;
			result.Row2.Z = cos;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the y-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting Matrix4 instance.</returns>
		public static Matrix CreateRotationY(float angle) {
			Matrix result;
			CreateRotationY(angle, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the z-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix4 instance.</param>
		public static void CreateRotationZ(float angle, out Matrix result) {
			float cos = (float)System.Math.Cos(angle);
			float sin = (float)System.Math.Sin(angle);

			result = Identity;
			result.Row0.X = cos;
			result.Row0.Y = sin;
			result.Row1.X = -sin;
			result.Row1.Y = cos;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the z-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting Matrix4 instance.</returns>
		public static Matrix CreateRotationZ(float angle) {
			Matrix result;
			CreateRotationZ(angle, out result);
			return result;
		}

		#endregion

		#region CreateTranslation

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="x">X translation.</param>
		/// <param name="y">Y translation.</param>
		/// <param name="z">Z translation.</param>
		/// <param name="result">The resulting Matrix4 instance.</param>
		public static void CreateTranslation(float x, float y, float z, out Matrix result) {
			result = Identity;
			result.Row3.X = x;
			result.Row3.Y = y;
			result.Row3.Z = z;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="vector">The translation vector.</param>
		/// <param name="result">The resulting Matrix4 instance.</param>
		public static void CreateTranslation(ref Vector3 vector, out Matrix result) {
			result = Identity;
			result.Row3.X = vector.X;
			result.Row3.Y = vector.Y;
			result.Row3.Z = vector.Z;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="x">X translation.</param>
		/// <param name="y">Y translation.</param>
		/// <param name="z">Z translation.</param>
		/// <returns>The resulting Matrix4 instance.</returns>
		public static Matrix CreateTranslation(float x, float y, float z) {
			Matrix result;
			CreateTranslation(x, y, z, out result);
			return result;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="vector">The translation vector.</param>
		/// <returns>The resulting Matrix4 instance.</returns>
		public static Matrix CreateTranslation(Vector3 vector) {
			Matrix result;
			CreateTranslation(vector.X, vector.Y, vector.Z, out result);
			return result;
		}

		#endregion

		#region CreateScale

		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Single scale factor for the x, y, and z axes.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix CreateScale(float scale) {
			Matrix result;
			CreateScale(scale, out result);
			return result;
		}

		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Scale factors for the x, y, and z axes.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix CreateScale(Vector3 scale) {
			Matrix result;
			CreateScale(ref scale, out result);
			return result;
		}

		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="x">Scale factor for the x axis.</param>
		/// <param name="y">Scale factor for the y axis.</param>
		/// <param name="z">Scale factor for the z axis.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix CreateScale(float x, float y, float z) {
			Matrix result;
			CreateScale(x, y, z, out result);
			return result;
		}

		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Single scale factor for the x, y, and z axes.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(float scale, out Matrix result) {
			result = Identity;
			result.Row0.X = scale;
			result.Row1.Y = scale;
			result.Row2.Z = scale;
		}

		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Scale factors for the x, y, and z axes.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(ref Vector3 scale, out Matrix result) {
			result = Identity;
			result.Row0.X = scale.X;
			result.Row1.Y = scale.Y;
			result.Row2.Z = scale.Z;
		}

		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="x">Scale factor for the x axis.</param>
		/// <param name="y">Scale factor for the y axis.</param>
		/// <param name="z">Scale factor for the z axis.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(float x, float y, float z, out Matrix result) {
			result = Identity;
			result.Row0.X = x;
			result.Row1.Y = y;
			result.Row2.Z = z;
		}

		#endregion

		#region CreateOrthographic

		/// <summary>
		/// Creates an orthographic projection matrix.
		/// </summary>
		/// <param name="width">The width of the projection volume.</param>
		/// <param name="height">The height of the projection volume.</param>
		/// <param name="zNear">The near edge of the projection volume.</param>
		/// <param name="zFar">The far edge of the projection volume.</param>
		/// <param name="result">The resulting Matrix4 instance.</param>
		public static void CreateOrthographic(float width, float height, float zNear, float zFar, out Matrix result) {
			CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
		}

		/// <summary>
		/// Creates an orthographic projection matrix.
		/// </summary>
		/// <param name="width">The width of the projection volume.</param>
		/// <param name="height">The height of the projection volume.</param>
		/// <param name="zNear">The near edge of the projection volume.</param>
		/// <param name="zFar">The far edge of the projection volume.</param>
		/// <rereturns>The resulting Matrix4 instance.</rereturns>
		public static Matrix CreateOrthographic(float width, float height, float zNear, float zFar) {
			Matrix result;
			CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
			return result;
		}

		#endregion

		#region CreateOrthographicOffCenter

		/// <summary>
		/// Creates an orthographic projection matrix.
		/// </summary>
		/// <param name="left">The left edge of the projection volume.</param>
		/// <param name="right">The right edge of the projection volume.</param>
		/// <param name="bottom">The bottom edge of the projection volume.</param>
		/// <param name="top">The top edge of the projection volume.</param>
		/// <param name="zNear">The near edge of the projection volume.</param>
		/// <param name="zFar">The far edge of the projection volume.</param>
		/// <param name="result">The resulting Matrix4 instance.</param>
		public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix result) {
			result = Identity;

			float invRL = 1.0f / (right - left);
			float invTB = 1.0f / (top - bottom);
			float invFN = 1.0f / (zFar - zNear);

			result.Row0.X = 2 * invRL;
			result.Row1.Y = 2 * invTB;
			result.Row2.Z = -2 * invFN;

			result.Row3.X = -(right + left) * invRL;
			result.Row3.Y = -(top + bottom) * invTB;
			result.Row3.Z = -(zFar + zNear) * invFN;
		}

		/// <summary>
		/// Creates an orthographic projection matrix.
		/// </summary>
		/// <param name="left">The left edge of the projection volume.</param>
		/// <param name="right">The right edge of the projection volume.</param>
		/// <param name="bottom">The bottom edge of the projection volume.</param>
		/// <param name="top">The top edge of the projection volume.</param>
		/// <param name="zNear">The near edge of the projection volume.</param>
		/// <param name="zFar">The far edge of the projection volume.</param>
		/// <returns>The resulting Matrix4 instance.</returns>
		public static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNear, float zFar) {
			Matrix result;
			CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar, out result);
			return result;
		}

		#endregion
		
		#region CreatePerspectiveFieldOfView
		
		/// <summary>
		/// Creates a perspective projection matrix.
		/// </summary>
		/// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
		/// <param name="aspect">Aspect ratio of the view (width / height)</param>
		/// <param name="zNear">Distance to the near clip plane</param>
		/// <param name="zFar">Distance to the far clip plane</param>
		/// <param name="result">A projection matrix that transforms camera space to raster space</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown under the following conditions:
		/// <list type="bullet">
		/// <item>fovy is zero, less than zero or larger than Math.PI</item>
		/// <item>aspect is negative or zero</item>
		/// <item>zNear is negative or zero</item>
		/// <item>zFar is negative or zero</item>
		/// <item>zNear is larger than zFar</item>
		/// </list>
		/// </exception>
		public static void CreatePerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar, out Matrix result) {
			if (fovy <= 0 || fovy > System.Math.PI) {
				throw new ArgumentOutOfRangeException("fovy");
			}
			if (aspect <= 0) {
				throw new ArgumentOutOfRangeException("aspect");
			}
			if (zNear <= 0) {
				throw new ArgumentOutOfRangeException("zNear");
			}
			if (zFar <= 0) {
				throw new ArgumentOutOfRangeException("zFar");
			}
			
			float yMax = zNear * (float)System.Math.Tan(0.5f * fovy);
			float yMin = -yMax;
			float xMin = yMin * aspect;
			float xMax = yMax * aspect;

			CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out result);
		}
		
		/// <summary>
		/// Creates a perspective projection matrix.
		/// </summary>
		/// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
		/// <param name="aspect">Aspect ratio of the view (width / height)</param>
		/// <param name="zNear">Distance to the near clip plane</param>
		/// <param name="zFar">Distance to the far clip plane</param>
		/// <returns>A projection matrix that transforms camera space to raster space</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown under the following conditions:
		/// <list type="bullet">
		/// <item>fovy is zero, less than zero or larger than Math.PI</item>
		/// <item>aspect is negative or zero</item>
		/// <item>zNear is negative or zero</item>
		/// <item>zFar is negative or zero</item>
		/// <item>zNear is larger than zFar</item>
		/// </list>
		/// </exception>
		public static Matrix CreatePerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar) {
			Matrix result;
			CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out result);
			return result;
		}
		
		#endregion
		
		#region CreatePerspectiveOffCenter
		
		/// <summary>
		/// Creates an perspective projection matrix.
		/// </summary>
		/// <param name="left">Left edge of the view frustum</param>
		/// <param name="right">Right edge of the view frustum</param>
		/// <param name="bottom">Bottom edge of the view frustum</param>
		/// <param name="top">Top edge of the view frustum</param>
		/// <param name="zNear">Distance to the near clip plane</param>
		/// <param name="zFar">Distance to the far clip plane</param>
		/// <param name="result">A projection matrix that transforms camera space to raster space</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown under the following conditions:
		/// <list type="bullet">
		/// <item>zNear is negative or zero</item>
		/// <item>zFar is negative or zero</item>
		/// <item>zNear is larger than zFar</item>
		/// </list>
		/// </exception>
		public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix result) {
			if (zNear <= 0) {
				throw new ArgumentOutOfRangeException("zNear");
			}
			if (zFar <= 0) {
				throw new ArgumentOutOfRangeException("zFar");
			}
			if (zNear >= zFar) {
				throw new ArgumentOutOfRangeException("zNear");
			}
			
			float x = (2.0f * zNear) / (right - left);
			float y = (2.0f * zNear) / (top - bottom);
			float a = (right + left) / (right - left);
			float b = (top + bottom) / (top - bottom);
			float c = -(zFar + zNear) / (zFar - zNear);
			float d = -(2.0f * zFar * zNear) / (zFar - zNear);

			result.Row0.X = x;
			result.Row0.Y = 0;
			result.Row0.Z = 0;
			result.Row0.W = 0;
			result.Row1.X = 0;
			result.Row1.Y = y;
			result.Row1.Z = 0;
			result.Row1.W = 0;
			result.Row2.X = a;
			result.Row2.Y = b;
			result.Row2.Z = c;
			result.Row2.W = -1;
			result.Row3.X = 0;
			result.Row3.Y = 0;
			result.Row3.Z = d;
			result.Row3.W = 0;
		}
		
		/// <summary>
		/// Creates an perspective projection matrix.
		/// </summary>
		/// <param name="left">Left edge of the view frustum</param>
		/// <param name="right">Right edge of the view frustum</param>
		/// <param name="bottom">Bottom edge of the view frustum</param>
		/// <param name="top">Top edge of the view frustum</param>
		/// <param name="zNear">Distance to the near clip plane</param>
		/// <param name="zFar">Distance to the far clip plane</param>
		/// <returns>A projection matrix that transforms camera space to raster space</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown under the following conditions:
		/// <list type="bullet">
		/// <item>zNear is negative or zero</item>
		/// <item>zFar is negative or zero</item>
		/// <item>zNear is larger than zFar</item>
		/// </list>
		/// </exception>
		public static Matrix CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear, float zFar) {
			Matrix result;
			CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out result);
			return result;
		}

		#endregion

		#region Camera Helper Functions

		/// <summary>
		/// Build a world space to camera space matrix
		/// </summary>
		/// <param name="eye">Eye (camera) position in world space</param>
		/// <param name="target">Target position in world space</param>
		/// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
		/// <returns>A Matrix4 that transforms world space to camera space</returns>
		public static Matrix LookAt(Vector3 eye, Vector3 target, Vector3 up) {
			Vector3 z = Vector3.Normalize(eye - target);
			Vector3 x = Vector3.Normalize(Vector3.Cross(up, z));
			Vector3 y = Vector3.Normalize(Vector3.Cross(z, x));

			Matrix result;

			result.Row0.X = x.X;
			result.Row0.Y = y.X;
			result.Row0.Z = z.X;
			result.Row0.W = 0;
			result.Row1.X = x.Y;
			result.Row1.Y = y.Y;
			result.Row1.Z = z.Y;
			result.Row1.W = 0;
			result.Row2.X = x.Z;
			result.Row2.Y = y.Z;
			result.Row2.Z = z.Z;
			result.Row2.W = 0;
			result.Row3.X = -((x.X * eye.X) + (x.Y * eye.Y) + (x.Z * eye.Z));
			result.Row3.Y = -((y.X * eye.X) + (y.Y * eye.Y) + (y.Z * eye.Z));
			result.Row3.Z = -((z.X * eye.X) + (z.Y * eye.Y) + (z.Z * eye.Z));
			result.Row3.W = 1;

			return result;
		}

		/// <summary>
		/// Build a world space to camera space matrix
		/// </summary>
		/// <param name="eyeX">Eye (camera) position in world space</param>
		/// <param name="eyeY">Eye (camera) position in world space</param>
		/// <param name="eyeZ">Eye (camera) position in world space</param>
		/// <param name="targetX">Target position in world space</param>
		/// <param name="targetY">Target position in world space</param>
		/// <param name="targetZ">Target position in world space</param>
		/// <param name="upX">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
		/// <param name="upY">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
		/// <param name="upZ">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
		/// <returns>A Matrix4 that transforms world space to camera space</returns>
		public static Matrix LookAt(float eyeX, float eyeY, float eyeZ, float targetX, float targetY, float targetZ, float upX, float upY, float upZ) {
			return LookAt(new Vector3(eyeX, eyeY, eyeZ), new Vector3(targetX, targetY, targetZ), new Vector3(upX, upY, upZ));
		}

		#endregion

		#region Multiply Functions

		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The left operand of the multiplication.</param>
		/// <param name="right">The right operand of the multiplication.</param>
		/// <returns>A new instance that is the result of the multiplication.</returns>
		public static Matrix Mult(Matrix left, Matrix right) {
			Matrix result;
			Mult(ref left, ref right, out result);
			return result;
		}

		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The left operand of the multiplication.</param>
		/// <param name="right">The right operand of the multiplication.</param>
		/// <param name="result">A new instance that is the result of the multiplication.</param>
		public static void Mult(ref Matrix left, ref Matrix right, out Matrix result) {
			float lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z, lM14 = left.Row0.W,
			lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z, lM24 = left.Row1.W,
			lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z, lM34 = left.Row2.W,
			lM41 = left.Row3.X, lM42 = left.Row3.Y, lM43 = left.Row3.Z, lM44 = left.Row3.W,
			rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z, rM14 = right.Row0.W,
			rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z, rM24 = right.Row1.W,
			rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z, rM34 = right.Row2.W,
			rM41 = right.Row3.X, rM42 = right.Row3.Y, rM43 = right.Row3.Z, rM44 = right.Row3.W;

			result.Row0.X = (((lM11 * rM11) + (lM12 * rM21)) + (lM13 * rM31)) + (lM14 * rM41);
			result.Row0.Y = (((lM11 * rM12) + (lM12 * rM22)) + (lM13 * rM32)) + (lM14 * rM42);
			result.Row0.Z = (((lM11 * rM13) + (lM12 * rM23)) + (lM13 * rM33)) + (lM14 * rM43);
			result.Row0.W = (((lM11 * rM14) + (lM12 * rM24)) + (lM13 * rM34)) + (lM14 * rM44);
			result.Row1.X = (((lM21 * rM11) + (lM22 * rM21)) + (lM23 * rM31)) + (lM24 * rM41);
			result.Row1.Y = (((lM21 * rM12) + (lM22 * rM22)) + (lM23 * rM32)) + (lM24 * rM42);
			result.Row1.Z = (((lM21 * rM13) + (lM22 * rM23)) + (lM23 * rM33)) + (lM24 * rM43);
			result.Row1.W = (((lM21 * rM14) + (lM22 * rM24)) + (lM23 * rM34)) + (lM24 * rM44);
			result.Row2.X = (((lM31 * rM11) + (lM32 * rM21)) + (lM33 * rM31)) + (lM34 * rM41);
			result.Row2.Y = (((lM31 * rM12) + (lM32 * rM22)) + (lM33 * rM32)) + (lM34 * rM42);
			result.Row2.Z = (((lM31 * rM13) + (lM32 * rM23)) + (lM33 * rM33)) + (lM34 * rM43);
			result.Row2.W = (((lM31 * rM14) + (lM32 * rM24)) + (lM33 * rM34)) + (lM34 * rM44);
			result.Row3.X = (((lM41 * rM11) + (lM42 * rM21)) + (lM43 * rM31)) + (lM44 * rM41);
			result.Row3.Y = (((lM41 * rM12) + (lM42 * rM22)) + (lM43 * rM32)) + (lM44 * rM42);
			result.Row3.Z = (((lM41 * rM13) + (lM42 * rM23)) + (lM43 * rM33)) + (lM44 * rM43);
			result.Row3.W = (((lM41 * rM14) + (lM42 * rM24)) + (lM43 * rM34)) + (lM44 * rM44);
		}
		/// <summary>
		/// Mult the specified left and right where left is a Vector4 should only be used for debuging
		/// and seeing if the point you are tryng to draw is in screen space
		/// </summary>
		/// <param name='left'>
		/// Left.
		/// </param>
		/// <param name='right'>
		/// Right.
		/// </param>
		public static Vector4 Mult(Vector4 left,Matrix right)
		{
			return new Vector4(left.X*right.M11 + left.Y*right.M21+ left.Z*right.M31+ left.W*right.M41,
			                   left.X*right.M12 + left.Y*right.M22+ left.Z*right.M32+ left.W*right.M42,
			                   left.X*right.M13 + left.Y*right.M23+ left.Z*right.M33+ left.W*right.M43,
			                   left.X*right.M14 + left.Y*right.M24+ left.Z*right.M34+ left.W*right.M44);
		}
		public static Vector4 operator*(Vector4 left, Matrix right)
		{
			return Mult(left, right);
		}
		#endregion

		#region Invert Functions

		/// <summary>
		/// Calculate the inverse of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to invert</param>
		/// <param name="result">The inverse of the given matrix if it has one, or the input if it is singular</param>
		/// <exception cref="InvalidOperationException">Thrown if the Matrix4 is singular.</exception>
		public static void Invert(ref Matrix mat, out Matrix result) {
            float s0 = mat.M11 * mat.M22 - mat.M21 * mat.M12;
            float s1 = mat.M11 * mat.M23 - mat.M21 * mat.M13;
            float s2 = mat.M11 * mat.M24 - mat.M21 * mat.M14;
            float s3 = mat.M12 * mat.M23 - mat.M22 * mat.M13;
            float s4 = mat.M12 * mat.M24 - mat.M22 * mat.M14;
            float s5 = mat.M13 * mat.M24 - mat.M23 * mat.M14;

            float c5 = mat.M33 * mat.M44 - mat.M43 * mat.M34;
            float c4 = mat.M32 * mat.M44 - mat.M42 * mat.M34;
            float c3 = mat.M32 * mat.M43 - mat.M42 * mat.M33;
            float c2 = mat.M31 * mat.M44 - mat.M41 * mat.M34;
            float c1 = mat.M31 * mat.M43 - mat.M41 * mat.M33;
            float c0 = mat.M31 * mat.M42 - mat.M41 * mat.M32;

            float inverseDeterminant = 1.0f / (s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0);

            float m11 = mat.M11;
            float m12 = mat.M12;
            float m13 = mat.M13;
            float m14 = mat.M14;
            float m21 = mat.M21;
            float m22 = mat.M22;
            float m23 = mat.M23;
            float m31 = mat.M31;
            float m32 = mat.M32;
            float m33 = mat.M33;

            float m41 = mat.M41;
            float m42 = mat.M42;

            result = new Matrix();
            result.M11 = (mat.M22 * c5 - mat.M23 * c4 + mat.M24 * c3) * inverseDeterminant;
            result.M12 = (-mat.M12 * c5 + mat.M13 * c4 - mat.M14 * c3) * inverseDeterminant;
            result.M13 = (mat.M42 * s5 - mat.M43 * s4 + mat.M44 * s3) * inverseDeterminant;
            result.M14 = (-mat.M32 * s5 + mat.M33 * s4 - mat.M34 * s3) * inverseDeterminant;

            result.M21 = (-mat.M21 * c5 + mat.M23 * c2 - mat.M24 * c1) * inverseDeterminant;
            result.M22 = (m11 * c5 - m13 * c2 + m14 * c1) * inverseDeterminant;
            result.M23 = (-mat.M41 * s5 + mat.M43 * s2 - mat.M44 * s1) * inverseDeterminant;
            result.M24 = (mat.M31 * s5 - mat.M33 * s2 + mat.M34 * s1) * inverseDeterminant;

            result.M31 = (m21 * c4 - m22 * c2 + mat.M24 * c0) * inverseDeterminant;
            result.M32 = (-m11 * c4 + m12 * c2 - m14 * c0) * inverseDeterminant;
            result.M33 = (mat.M41 * s4 - mat.M42 * s2 + mat.M44 * s0) * inverseDeterminant;
            result.M34 = (-m31 * s4 + m32 * s2 - mat.M34 * s0) * inverseDeterminant;

            result.M41 = (-m21 * c3 + m22 * c1 - m23 * c0) * inverseDeterminant;
            result.M42 = (m11 * c3 - m12 * c1 + m13 * c0) * inverseDeterminant;
            result.M43 = (-m41 * s3 + m42 * s1 - mat.M43 * s0) * inverseDeterminant;
            result.M44 = (m31 * s3 - m32 * s1 + m33 * s0) * inverseDeterminant;
		}

		/// <summary>
		/// Calculate the inverse of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to invert</param>
		/// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
		/// <exception cref="InvalidOperationException">Thrown if the Matrix4 is singular.</exception>
		public static Matrix Invert(Matrix mat) {
			Matrix result;
			Invert(ref mat, out result);
			return result;
		}

		#endregion

		#region Transpose

		/// <summary>
		/// Calculate the transpose of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to transpose</param>
		/// <returns>The transpose of the given matrix</returns>
		public static Matrix Transpose(Matrix mat) {
			return new Matrix(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
		}


		/// <summary>
		/// Calculate the transpose of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to transpose</param>
		/// <param name="result">The result of the calculation</param>
		public static void Transpose(ref Matrix mat, out Matrix result) {
			result.Row0 = mat.Column0;
			result.Row1 = mat.Column1;
			result.Row2 = mat.Column2;
			result.Row3 = mat.Column3;
		}

		#endregion

		#endregion

		#region Operators

		/// <summary>
		/// Matrix multiplication
		/// </summary>
		/// <param name="left">left-hand operand</param>
		/// <param name="right">right-hand operand</param>
		/// <returns>A new Matrix4 which holds the result of the multiplication</returns>
		public static Matrix operator *(Matrix left, Matrix right) {
			return Matrix.Mult(left, right);
		}

		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator ==(Matrix left, Matrix right) {
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equal right; false otherwise.</returns>
		public static bool operator !=(Matrix left, Matrix right) {
			return !left.Equals(right);
		}

		#endregion

		#region Overrides

		#region public override string ToString()

		/// <summary>
		/// Returns a System.String that represents the current Matrix4.
		/// </summary>
		/// <returns>The string representation of the matrix.</returns>
		public override string ToString() {
			return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
		}

		#endregion

		#region public override int GetHashCode()

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode() {
			return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
		}

		#endregion

		#region public override bool Equals(object obj)

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare tresult.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals(object obj) {
			if (!(obj is Matrix)) {
				return false;
			}

			return this.Equals((Matrix)obj);
		}

		#endregion

		#endregion

		#endregion

		#region IEquatable<Matrix4> Members

		/// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
		/// <param name="other">An matrix to compare with this matrix.</param>
		/// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
		public bool Equals(Matrix other) {
			return
				Row0 == other.Row0 &&
				Row1 == other.Row1 &&
				Row2 == other.Row2 &&
				Row3 == other.Row3;
		}

		#endregion
	}
}
