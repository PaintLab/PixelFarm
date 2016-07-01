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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Pencil.Gaming.MathUtils;

namespace Pencil.Gaming.Graphics {
	public delegate T[] TArrayFromRetrievedData<T>(Vector4[] vertexs,Vector3[] normals,Vector2[] texCoords);

	public static partial class GL {
		public static class Utils {
			#region Image Loading

			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="path">The path to the image file.</param>
			public static int LoadImage(string path) {
				using (Bitmap bmp = new Bitmap(path)) {
					return LoadImage(bmp);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="path">The path to the image file.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			public static int LoadImage(string path, bool square) {
				using (Bitmap bmp = new Bitmap(path)) {
					return LoadImage(bmp, square);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="path">The path to the image file.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			/// <param name="tmin">The required minimization filter.</param>
			/// <param name="tmag">The required maximalization filter.</param> 
			public static int LoadImage(string path, bool square, TextureMinFilter tmin, TextureMagFilter tmag) {
				using (Bitmap bmp = new Bitmap(path)) {
					return LoadImage(bmp, square, tmin, tmag);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="file">The stream containing the image bytes.</param>
			public static int LoadImage(Stream file) {
				using (Bitmap bmp = new Bitmap(file)) {
					return LoadImage(bmp);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="file">The stream containing the image bytes.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			public static int LoadImage(Stream file, bool square) {
				using (Bitmap bmp = new Bitmap(file)) {
					return LoadImage(bmp, square);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="file">The stream containing the image bytes.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			/// <param name="tmin">The required minimization filter.</param>
			/// <param name="tmag">The required maximalization filter.</param> 
			public static int LoadImage(Stream file, bool square, TextureMinFilter tmin, TextureMagFilter tmag) {
				using (Bitmap bmp = new Bitmap(file)) {
					return LoadImage(bmp, square, tmin, tmag);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="bmp">The Bitmap representing the image.</param>
			public static int LoadImage(Bitmap bmp) {
				return LoadImage(bmp, true, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="bmp">The Bitmap representing the image.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			public static int LoadImage(Bitmap bmp, bool square) {
				return LoadImage(bmp, square, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="bmp">The Bitmap representing the image.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			/// <param name="tmin">The required minimization filter.</param>
			/// <param name="tmag">The required maximalization filter.</param> 
			public static int LoadImage(Bitmap bmp, bool square, TextureMinFilter tmin, TextureMagFilter tmag) {
				BitmapData bmpData = bmp.LockBits(
					new System.Drawing.Rectangle(Point.Empty, bmp.Size), 
					ImageLockMode.ReadOnly, 
					System.Drawing.Imaging.PixelFormat.Format32bppArgb
				);
				
				int result;
				TextureTarget target = (square ? TextureTarget.Texture2D : TextureTarget.TextureRectangle);

				try {
					GL.GenTextures(1, out result);
					GL.BindTexture(target, result);
					GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)tmag);
					GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)tmin);
					GL.TexImage2D(
						target, 
						0, 
						PixelInternalFormat.Rgba,
						bmp.Width,
						bmp.Height,
						0,
						Pencil.Gaming.Graphics.PixelFormat.Bgra,
						PixelType.UnsignedByte,
						bmpData.Scan0
					);
					GL.BindTexture(target, 0);
				} finally {
					bmp.UnlockBits(bmpData);
				}

				return result;
			}

			#endregion

			#region Cylinder Generation

			/// <summary>
			/// Draws the cylinder.
			/// </summary>
			/// <param name="lod">Level of detail with which the cylinder was created.</param>
			public static void DrawCylinderArrays(int lod) {
				GL.DrawArrays(BeginMode.TriangleStrip, 0, (lod + 1) * 2);
				GL.DrawArrays(BeginMode.TriangleFan, (lod + 1) * 2, lod);
				GL.DrawArrays(BeginMode.TriangleFan, (lod + 1) * 2 + lod, lod);
			}

			/// <summary>
			/// Generates a cylinder.
			/// </summary>
			/// <param name="vertices">Vertices.</param>
			/// <param name="normals">Normals.</param>
			/// <param name="height">Height.</param>
			/// <param name="radius">Radius.</param>
			/// <param name="lod">Level of detail.</param>
			public static void CreateCylinder(out Vector4[] vertices, out Vector3[] normals, float height, float radius, int lod) {
				int sideVCount = (lod + 1) * 2;
				int topVCount = lod;

				int totalVCount = sideVCount + topVCount * 2;

				vertices = new Vector4[totalVCount];
				normals = new Vector3[totalVCount];

				for (int i = 0; i <= lod; ++i) {
					float rad = (i / (float)lod) * MathHelper.Tau;
					float xNorm = (float)Math.Cos(rad);
					float zNorm = (float)Math.Sin(rad);
					float xVert = xNorm * radius;
					float zVert = zNorm * radius;

					normals[2 * i + 0] = new Vector3(xNorm, 0f, zNorm);
					normals[2 * i + 1] = new Vector3(xNorm, 0f, zNorm);
					vertices[2 * i + 0] = new Vector4(xVert, -height / 2f, zVert, 1f);
					vertices[2 * i + 1] = new Vector4(xVert, height / 2f, zVert, 1f);

					if (i != lod) {
						normals[sideVCount + i] = -Vector3.UnitY;
						vertices[sideVCount + i] = new Vector4(xVert, -height / 2f, zVert, 1f);

						normals[sideVCount + topVCount + i] = Vector3.UnitY;
						vertices[sideVCount + topVCount + i] = new Vector4(xVert, height / 2f, zVert, 1f);
					}
				}
			}

			#endregion

			#region Wavefront Model Loading

			/// <summary>
			/// Creates a float array from vertices and normals.
			/// This function is compatible with delegate type <see cref="TArrayFromRetrievedData"/>.
			/// The returned value is an interleaved float array, i.e. { v, n, v, n ... }.
			/// </summary>
			/// <returns>The the float array, in which the elements are interleaved.</returns>
			/// <param name="vertices">Vertices.</param>
			/// <param name="normals">Normals.</param>
			/// <param name="texCoords">Tex coords (can be empty).</param>
			public static float[] FloatArrayFromVerticesNormalsInterleaved(Vector4[] vertices, Vector3[] normals, Vector2[] texCoords) {
				float[] final = new float[vertices.Length * 4 + normals.Length * 3];
				for (int i = 0; i < final.Length; i += 7) {
					Vector4 vertex = vertices[i / 7];
					Vector3 normal = normals[i / 7];
					final[i] = vertex.X;
					final[i + 1] = vertex.Y;
					final[i + 2] = vertex.Z;
					final[i + 3] = vertex.W;
					final[i + 4] = normal.X;
					final[i + 5] = normal.Y;
					final[i + 6] = normal.Z;
				}
				return final;
			}
			/// <summary>
			/// Creates a float array from vertices, normals and tex coords.
			/// This function is compatible with delegate type <see cref="TArrayFromRetrievedData"/>.
			/// The returned value is an interleaved float array, i.e. { v, n, t, v, n t, ... }.
			/// </summary>
			/// <returns>The the float array, in which the elements are interleaved.</returns>
			/// <param name="vertices">Vertices.</param>
			/// <param name="normals">Normals.</param>
			/// <param name="texCoords">Tex coords.</param>
			public static float[] FloatArrayFromVerticesNormalsTexCoordsInterleaved(Vector4[] vertices, Vector3[] normals, Vector2[] texCoords) {
				float[] final = new float[vertices.Length * 4 + normals.Length * 3 + texCoords.Length * 2];
				for (int i = 0; i < final.Length; i += 9) {
					Vector4 vertex = vertices[i / 9];
					Vector3 normal = normals[i / 9];
					Vector2 texCoord = texCoords[i / 9];
					final[i] = vertex.X;
					final[i + 1] = vertex.Y;
					final[i + 2] = vertex.Z;
					final[i + 3] = vertex.W;
					final[i + 4] = normal.X;
					final[i + 5] = normal.Y;
					final[i + 6] = normal.Z;
					final[i + 7] = texCoord.X;
					final[i + 8] = texCoord.Y;
				}
				return final;
			}
			/// <summary>
			/// Creates a float array from vertices and normals.
			/// This function is compatible with delegate type <see cref="TArrayFromRetrievedData"/>.
			/// The returned value is a non-interleaved float array, i.e. { v, v, n, n, ... }.
			/// </summary>
			/// <returns>The the float array.</returns>
			/// <param name="vertices">Vertices.</param>
			/// <param name="normals">Normals.</param>
			/// <param name="texCoords">Tex coords (can be empty).</param>
			public static float[] FloatArrayFromVerticesNormals(Vector4[] vertices, Vector3[] normals, Vector2[] texCoords) {
				float[] final = new float[vertices.Length * 4 + normals.Length * 3];
				for (int i = 0; i < vertices.Length * 4; i += 4) {
					Vector4 vertex = vertices[i / 3];
					final[i] = vertex.X;
					final[i + 1] = vertex.Y;
					final[i + 2] = vertex.Z;
					final[i + 3] = vertex.W;
				}
				for (int i = vertices.Length * 4; i < final.Length; i += 3) {
					Vector3 normal = normals[i];
					final[i] = normal.X;
					final[i + 1] = normal.Y;
					final[i + 2] = normal.Z;
				}
				return final;
			}
			/// <summary>
			/// Creates a float array from vertices, normals and tex coords.
			/// This function is compatible with delegate type <see cref="TArrayFromRetrievedData"/>.
			/// The returned value is a non-interleaved float array, i.e. { v, v, n, n, t, t, ... }.
			/// </summary>
			/// <returns>The the float array.</returns>
			/// <param name="vertices">Vertices.</param>
			/// <param name="normals">Normals.</param>
			/// <param name="texCoords">Tex coords.</param>
			public static float[] FloatArrayFromVerticesNormalsTexCoords(Vector4[] vertices, Vector3[] normals, Vector2[] texCoords) {
				float[] final = new float[vertices.Length * 4 + normals.Length * 3 + texCoords.Length * 2];
				for (int i = 0; i < vertices.Length * 4; i += 4) {
					Vector4 vertex = vertices[i / 3];
					final[i] = vertex.X;
					final[i + 1] = vertex.Y;
					final[i + 2] = vertex.Z;
					final[i + 3] = vertex.W;
				}
				for (int i = vertices.Length * 4; i < vertices.Length * 4 + normals.Length * 3; i += 3) {
					Vector3 normal = normals[i];
					final[i] = normal.X;
					final[i + 1] = normal.Y;
					final[i + 2] = normal.Z;
				}
				for (int i = vertices.Length * 4 + normals.Length * 3; i < final.Length; i += 2) {
					Vector2 texCoord = texCoords[i];
					final[i] = texCoord.X;
					final[i + 1] = texCoord.Y;
				}
				return final;
			}

			/// <summary>
			/// Loads a wavefront model.
			/// </summary>
			/// <param name="path">Path to .obj file.</param>
			/// <param name="verticesOut">Vertices out.</param>
			/// <param name="normalsOut">Normals out.</param>
			/// <param name="tCoordsOut">Texture coords out.</param>
			/// <param name="indicesOut">Indices out.</param>
			public static void LoadModel(string path, out Vector4[] verticesOut, out Vector3[] normalsOut, out Vector2[] tCoordsOut, out int[] indicesOut) {
				LoadModel(path, out verticesOut, out normalsOut, out tCoordsOut, out indicesOut, true);
			}
			/// <summary>
			/// Loads a wavefront model.
			/// </summary>
			/// <param name="path">Path to .obj file.</param>
			/// <param name="verticesOut">Vertices out.</param>
			/// <param name="normalsOut">Normals out.</param>
			/// <param name="tCoordsOut">Texture coords out.</param>
			/// <param name="indicesOut">Indices out.</param>
			/// <param name="optimize">A value indicating whether indices should be optimized (less memory-usage, longer load time).</param>
			public static void LoadModel(string path, out Vector4[] verticesOut, out Vector3[] normalsOut, out Vector2[] tCoordsOut, out int[] indicesOut, bool optimize) {
				using (Stream str = File.OpenRead(path)) {
					LoadModel(str, out verticesOut, out normalsOut, out tCoordsOut, out indicesOut, optimize);
				}
			}
			/// <summary>
			/// Loads a wavefront model.
			/// </summary>
			/// <param name="file">The bytes resembling a .obj file.</param>
			/// <param name="verticesOut">Vertices out.</param>
			/// <param name="normalsOut">Normals out.</param>
			/// <param name="tCoordsOut">Texture coords out.</param>
			/// <param name="indicesOut">Indices out.</param>
			public static void LoadModel(Stream file, out Vector4[] verticesOut, out Vector3[] normalsOut, out Vector2[] tCoordsOut, out int[] indicesOut) {
				LoadModel(file, out verticesOut, out normalsOut, out tCoordsOut, out indicesOut, true);
			}
			/// <summary>
			/// Loads a wavefront model.
			/// </summary>
			/// <param name="path">Path to .obj file.</param>
			/// <param name="indicesOutArr">Indices out.</param>
			/// <param name="outArr">The array of Ts, as the given function is applied to the model data.</param>
			/// <param name="func">Func.</param>
			/// <typeparam name="T">The type of the required data.</typeparam>
			public static void LoadModel<T>(string path, out int[] indicesOutArr, out T[] outArr, TArrayFromRetrievedData<T> func) {
				using (Stream str = File.OpenRead(path)) {
					LoadModel<T>(str, out indicesOutArr, out outArr, func, true);
				}
			}
			/// <summary>
			/// Loads a wavefront model.
			/// </summary>
			/// <param name="path">Path to .obj file.</param>
			/// <param name="indicesOutArr">Indices out.</param>
			/// <param name="outArr">The array of Ts, as the given function is applied to the model data.</param>
			/// <param name="func">Func.</param>
			/// <param name="optimize">A value indicating whether indices should be optimized (less memory-usage, longer load time).</param>
			/// <typeparam name="T">The type of the required data.</typeparam>
			public static void LoadModel<T>(string path, out int[] indicesOutArr, out T[] outArr, TArrayFromRetrievedData<T> func, bool optimize) {
				using (Stream str = File.OpenRead(path)) {
					LoadModel<T>(str, out indicesOutArr, out outArr, func, optimize);
				}
			}
			/// <summary>
			/// Loads a wavefront model.
			/// </summary>
			/// <param name="path">The bytes resembling a .obj file.</param>
			/// <param name="indicesOutArr">Indices out.</param>
			/// <param name="outArr">The array of Ts, as the given function is applied to the model data.</param>
			/// <param name="func">Func.</param>
			/// <typeparam name="T">The type of the required data.</typeparam>
			public static void LoadModel<T>(Stream file, out int[] indicesOutArr, out T[] outArr, TArrayFromRetrievedData<T> func) {
				LoadModel<T>(file, out indicesOutArr, out outArr, func, true);
			}
			/// <summary>
			/// Loads a wavefront model.
			/// </summary>
			/// <param name="path">The bytes resembling a .obj file.</param>
			/// <param name="indicesOutArr">Indices out.</param>
			/// <param name="outArr">The array of Ts, as the given function is applied to the model data.</param>
			/// <param name="func">Func.</param>
			/// <param name="optimize">A value indicating whether indices should be optimized (less memory-usage, longer load time).</param>
			/// <typeparam name="T">The type of the required data.</typeparam>
			public static void LoadModel<T>(Stream file, out int[] indicesOutArr, out T[] outArr, TArrayFromRetrievedData<T> func, bool optimize) {
				Vector4[] vertices;
				Vector3[] normals;
				Vector2[] texCoords;
				GL.Utils.LoadModel(file, out vertices, out normals, out texCoords, out indicesOutArr, optimize);
				outArr = func(vertices, normals, texCoords);
			}
			/// <summary>
			/// Loads a wavefront model.
			/// </summary>
			/// <param name="path">The bytes resembling a .obj file.</param>
			/// <param name="verticesOut">Vertices out.</param>
			/// <param name="normalsOut">Normals out.</param>
			/// <param name="tCoordsOut">Texture coords out.</param>
			/// <param name="indicesOut">Indices out.</param>
			/// <param name="optimize">A value indicating whether indices should be optimized (less memory-usage, longer load time).</param>
			public static void LoadModel(Stream file, out Vector4[] verticesOutArr, out Vector3[] normalsOutArr, out Vector2[] tCoordsOutArr, out int[] indicesOutArr, bool optimize) {
				List<Vector4>vertices = new List<Vector4>(1024);
				List<Vector3> normals = new List<Vector3>(1024);
				List<Vector2> tCoords = new List<Vector2>(1024);

				List<Vector4> verticesOut;
				List<Vector3> normalsOut;
				List<Vector2> tCoordsOut;
				List<int> indicesOut;

				using (StreamReader sread = new StreamReader(file)) {
					List<Face> faces = new List<Face>(1024);

#if DEBUG
					Stopwatch sw = new Stopwatch();
					sw.Start();
#endif
					string line;
					while ((line = sread.ReadLine()) != null) {
						++currentLine;
						ParseObjLine(line, vertices, normals, tCoords, faces);
					}
#if DEBUG
					sw.Stop();
					Console.WriteLine("Parsing model took: {0} milliseconds", sw.ElapsedMilliseconds);

					sw.Reset();
					sw.Start();
#endif
					List<VertexIndices> vIndices = VIndicesFromFaces(faces);
					SortOutVIndices(vertices, out verticesOut, normals, out normalsOut, tCoords, out tCoordsOut, out indicesOut, vIndices, optimize);
#if DEBUG
					sw.Stop();
					Console.WriteLine("Optimizing/expanding model vertices took: {0} milliseconds", sw.ElapsedMilliseconds);
#endif
				}

#if DEBUG
				Stopwatch swatch = new Stopwatch();
				swatch.Start();
#endif
				verticesOutArr = verticesOut.ToArray();
				normalsOutArr = normalsOut.ToArray();
				tCoordsOutArr = tCoordsOut.ToArray();
				indicesOutArr = indicesOut.ToArray();
#if DEBUG
				swatch.Stop();
				Console.WriteLine("Converting model lists to arrays took: {0} milliseconds", swatch.ElapsedMilliseconds);
#endif
			}

			private static int currentLine = 0;

			#pragma warning disable 0661
			#pragma warning disable 0659

			internal struct VertexIndices {
				public int Vertex;
				public int? TexCoord;
				public int? Normal;
				public int FinalIndex;

				public VertexIndices(VertexIndices previous, int finalIndex) {
					Vertex = previous.Vertex;
					TexCoord = previous.TexCoord;
					Normal = previous.Normal;
					FinalIndex = finalIndex;
				}

				public VertexIndices(int vertex, int? texCoord, int? normal) {
					Vertex = vertex;
					TexCoord = texCoord;
					Normal = normal;
					FinalIndex = -1;
				}

				public override bool Equals(object obj) {
					return (this == (VertexIndices)obj);
				}

				public static bool operator ==(VertexIndices @this, VertexIndices other) {
					return (@this.Normal == other.Normal && @this.TexCoord == other.TexCoord && @this.Vertex == other.Vertex);
				}

				public static bool operator !=(VertexIndices @this, VertexIndices other) {
					return !(@this == other);
				}
			}

			#pragma warning restore 0661
			#pragma warning restore 0659

			internal struct Face {
				public List<VertexIndices> Vertices;

				public Face(List<VertexIndices> vertices) {
					Vertices = vertices;
				}
			}

			private static float ToFloat(string str) {
				return Convert.ToSingle(str, CultureInfo.InvariantCulture);
			}

			private static List<VertexIndices> VIndicesFromFaces(List<Face> faces) {
				List<VertexIndices> result = new List<VertexIndices>(faces.Count * 3);
				for (int i = 0; i < faces.Count; ++i) {
					Face face = faces[i];
					for (int j = 0; j < face.Vertices.Count; ++j) {
						result.Add(face.Vertices[j]);
					}
				}
				return result;
			}

			private static int GetFirstFinalIndexOfDuplicate(List<VertexIndices> vIndices, int currentIndex, bool optimize) {
				if (optimize) {
					VertexIndices vertex = vIndices[currentIndex];
					for (int i = 0; i < currentIndex; ++i) {
						VertexIndices other = vIndices[i];
						if (vertex == other) {
							return other.FinalIndex;
						}
					}
				}
				return -1;
			}

			private static void SortOutVIndices(
				List<Vector4> verticesIn, 
				out List<Vector4> verticesOut, 
				List<Vector3> normalsIn, 
				out List<Vector3> normalsOut, 
				List<Vector2> tCoordsIn,
				out List<Vector2> tCoordsOut,
				out List<int> indicesOut,
				List<VertexIndices> vIndices,
				bool optimize) {

				verticesOut = new List<Vector4>(verticesIn.Count);
				normalsOut = new List<Vector3>(normalsIn.Count);
				tCoordsOut = new List<Vector2>(tCoordsIn.Count);
				indicesOut = new List<int>(verticesIn.Count);

				for (int i = 0; i < vIndices.Count; ++i) {
					int firstFinalIndex = GetFirstFinalIndexOfDuplicate(vIndices, i, optimize);

					if (firstFinalIndex == -1) {
						VertexIndices vertex = vIndices[i];
						vIndices[i] = new VertexIndices(vertex, verticesOut.Count);

						indicesOut.Add(verticesOut.Count);
						verticesOut.Add(verticesIn[vertex.Vertex - 1]);

						if (vertex.TexCoord.HasValue) {
							tCoordsOut.Add(tCoordsIn[vertex.TexCoord.Value - 1]);
						}
						if (vertex.Normal.HasValue) {
							normalsOut.Add(normalsIn[vertex.Normal.Value - 1]);
						}
					} else {
						indicesOut.Add(firstFinalIndex);
					}
				}
			}


			private static void ParseVElement(string line, List<Vector4> vertices) {
				string verticesString = line.Substring(2);
				string[] elements = verticesString.Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
				switch (elements.Length) {
				case 3:
					vertices.Add(new Vector4(
						ToFloat(elements[0]),
						ToFloat(elements[1]),
						ToFloat(elements[2]), 1f));
                    break;
				case 4:
					vertices.Add(new Vector4(
						ToFloat(elements[0]),
						ToFloat(elements[1]), 
						ToFloat(elements[2]), 
						ToFloat(elements[3])));
					break;
				default:
					throw new AssetLoadException("model", "vertices can only have 3 or 4 elements @ line " + currentLine.ToString());
				}
			}
			private static void ParseVNElement(string line, List<Vector3> normals) {
				Vector3 result = Vector3.Zero;

				string elementsString = line.Substring(3);
				string[] elements = elementsString.Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
				if (elements.Length != 3) {
					throw new AssetLoadException("model", "normals must define 3 elements @ line " + currentLine.ToString());
				}

				result = Vector3.Normalize(new Vector3(
					ToFloat(elements[0]), 
					ToFloat(elements[1]), 
					ToFloat(elements[2])));
				normals.Add(result);
			}
			private static void ParseVTElement(string line, List<Vector2> tCoords) {
				Vector2 result = Vector2.Zero;

				string elementsString = line.Substring(3);
				string[] elements = elementsString.Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
				if (elements.Length == 3) {
					Console.WriteLine("WARNING: Object file specifies third texture coordinate, ignored @ line " + currentLine.ToString());
				} else if (elements.Length != 2) {
					throw new AssetLoadException("model", "texture coordinates must define either 3 or 4 elements @ line " + currentLine.ToString());
				}

				result = new Vector2(ToFloat(elements[0]), ToFloat(elements[1]));
				tCoords.Add(result);
			}
			private static void ParseFElement(string line, List<Face> faces) {
				Face result = new Face();

				string elementsString = line.Substring(2);
				List<string> elements = new List<string>(elementsString.Split(null as string[], StringSplitOptions.RemoveEmptyEntries));
				elements.RemoveAll(str => string.IsNullOrEmpty(str));
				List<VertexIndices> vIndices = new List<VertexIndices>(elements.Count);
				switch (elements.Count) {
				case 3:
					// Triangular face
					for (int i = elements.Count - 1; i >= 0; --i) {
						vIndices.Add(ParseVertexIndices(elements[i]));
					}
					break;
				case 4:
					// Quad face
					vIndices.Add(ParseVertexIndices(elements[3]));
					vIndices.Add(ParseVertexIndices(elements[2]));
					vIndices.Add(ParseVertexIndices(elements[1]));

					vIndices.Add(ParseVertexIndices(elements[1]));
					vIndices.Add(ParseVertexIndices(elements[0]));
					vIndices.Add(ParseVertexIndices(elements[3]));
					break;
				default:
					// Other polygon, not supported
					throw new AssetLoadException("model", "faces with " + elements.Count.ToString() + " elements are currently not supported @ line " + currentLine.ToString());
				}

				result.Vertices = vIndices;
				faces.Add(result);
			}
			private static VertexIndices ParseVertexIndices(string element) {
				int count = element.Count(ch => ch == '/');
				VertexIndices result = new VertexIndices();

				if (element.Contains("//")) {
					// Vertex, normal
					string[] vertexNormal = element.Split(new [] { "//" }, StringSplitOptions.None);
					result.Vertex = int.Parse(vertexNormal[0]);
					result.Normal = int.Parse(vertexNormal[1]);
				} else if (count == 1) {
					// Vertex, texcoord
					string[] vertexTCoord = element.Split('/');
					result.Vertex = int.Parse(vertexTCoord[0]);
					result.TexCoord = int.Parse(vertexTCoord[1]);
				} else if (count == 2) {
					// Vertex, texcoord, normal
					string[] vertexTCoordNormal = element.Split('/');
					result.Vertex = int.Parse(vertexTCoordNormal[0]);
					result.TexCoord = int.Parse(vertexTCoordNormal[1]);
					result.Normal = int.Parse(vertexTCoordNormal[2]);
				} else {
					throw new AssetLoadException("model", "texture face element declaration incorrect, count was " + count.ToString() + " @ line " + currentLine.ToString());
				}

				return result;
			}
			private static void ParseObjLine(string line, List<Vector4> vertices, List<Vector3> normals, List<Vector2> tCoords, List<Face> faces) {
				if (line.StartsWith("v ", StringComparison.InvariantCulture)) {
					ParseVElement(line, vertices);
				} else if (line.StartsWith("vn ", StringComparison.InvariantCulture)) {
					ParseVNElement(line, normals);
				} else if (line.StartsWith("vt ", StringComparison.InvariantCulture)) {
					ParseVTElement(line, tCoords);
				} else if (line.StartsWith("f ", StringComparison.InvariantCulture)) {
					ParseFElement(line, faces);
				} else if (!line.StartsWith("#", StringComparison.InvariantCulture)) {
					
				}
			}
#endregion
		
			public static Vector3 UnProject(Vector3 winCoords, Matrix modelView, Matrix proj, Rectanglei viewport) {
				Matrix finalMatrix = Matrix.Invert(modelView * proj);

				Vector4 inVec = new Vector4(winCoords, 1f);
				inVec.X = (inVec.X - viewport.X) / viewport.Width;
				inVec.Y = (inVec.Y - viewport.Y) / viewport.Height;

				inVec.X = inVec.X * 2 - 1;
				inVec.Y = inVec.Y * 2 - 1;
				inVec.Z = inVec.Z * 2 - 1;

				Vector4 outVec = Vector4.Transform(inVec, finalMatrix);
				if (Math.Abs(outVec.Z) < 10f * float.Epsilon) {
					throw new Exception();
				}

				outVec.X /= outVec.W;
				outVec.Y /= outVec.W;
				outVec.Z /= outVec.W;
				return outVec.Xyz;
			}
		}
	}
}

