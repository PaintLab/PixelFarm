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

#region --- Using Directives ---

#define MINIMAL

using System;
using System.Collections.Generic;
#if !MINIMAL
using System.Drawing;
#endif
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Reflection.Emit;
using Pencil.Gaming.MathUtils;

#endregion

namespace Pencil.Gaming.Graphics {
	/// <summary>
	/// OpenGL bindings for .NET, implementing the full OpenGL API, including extensions.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class contains all OpenGL enums and functions defined in the latest OpenGL specification.
	/// The official .spec files can be found at: http://opengl.org/registry/.
	/// </para>
	/// <para> A valid OpenGL context must be created before calling any OpenGL function.</para>
	/// <para>
	/// Use the GL.Load and GL.LoadAll methods to prepare function entry points prior to use. To maintain
	/// cross-platform compatibility, this must be done for both core and extension functions. The GameWindow
	/// and the GLControl class will take care of this automatically.
	/// </para>
	/// <para>
	/// You can use the GL.SupportsExtension method to check whether any given category of extension functions
	/// exists in the current OpenGL context. Keep in mind that different OpenGL contexts may support different
	/// extensions, and under different entry points. Always check if all required extensions are still supported
	/// when changing visuals or pixel formats.
	/// </para>
	/// <para>
	/// You may retrieve the entry point for an OpenGL function using the GL.GetDelegate method.
	/// </para>
	/// </remarks>
	/// <see href="http://opengl.org/registry/"/>
	public static partial class GL {
		#region --- GL Overloads ---

#pragma warning disable 3019
#pragma warning disable 1591
#pragma warning disable 1572
#pragma warning disable 1573

		// Note: Mono 1.9.1 truncates StringBuilder results (for 'out string' parameters).
		// We work around this issue by doubling the StringBuilder capacity.

		#region public static void Color[34]() overloads

//		public static void Color3(Color color) {
//			GL.Color3(color.R, color.G, color.B);
//		}
//
//		public static void Color4(Color color) {
//			GL.Color4(color.R, color.G, color.B, color.A);
//		}

#if USE_GL_COMPAT
		public static void Color3(Vector3 color) {
			GL.Color3(color.X, color.Y, color.Z);
		}

		public static void Color4(Vector4 color) {
			GL.Color4(color.X, color.Y, color.Z, color.W);
		}

		public static void Color4(Color4 color) {
			GL.Color4(color.R, color.G, color.B, color.A);
		}
#endif

		#endregion

		#region public static void ClearColor() overloads

//		public static void ClearColor(Color color) {
//			GL.ClearColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
//		}

		public static void ClearColor(Color4 color) {
			GL.ClearColor(color.R, color.G, color.B, color.A);
		}

		#endregion

		#region public static void BlendColor() overloads

//		public static void BlendColor(Color color) {
//			GL.BlendColor(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
//		}

		public static void BlendColor(Color4 color) {
			GL.BlendColor(color.R, color.G, color.B, color.A);
		}

		#endregion

		#region public static void Material() overloads

#if USE_GL_COMPAT
		public static void Material(MaterialFace face, MaterialParameter pname, Vector4 @params) {
			unsafe {
				Material(face, pname, (float*)&@params.X);
			}
		}

		public static void Material(MaterialFace face, MaterialParameter pname, Color4 @params) {
			unsafe {
				GL.Material(face, pname, (float*)&@params);
			}
		}
#endif

		#endregion

		#region public static void Light() overloads

#if USE_GL_COMPAT
		public static void Light(LightName name, LightParameter pname, Vector4 @params) {
			unsafe {
				GL.Light(name, pname, (float*)&@params.X);
			}
		}

		public static void Light(LightName name, LightParameter pname, Color4 @params) {
			unsafe {
				GL.Light(name, pname, (float*)&@params);
			}
		}
#endif

		#endregion

		#region Normal|RasterPos|Vertex|TexCoord|Rotate|Scale|Translate|*Matrix

#if USE_GL_COMPAT
		public static void Normal3(Vector3 normal) {
			GL.Normal3(normal.X, normal.Y, normal.Z);
		}

		public static void RasterPos2(Vector2 pos) {
			GL.RasterPos2(pos.X, pos.Y);
		}

		public static void RasterPos3(Vector3 pos) {
			GL.RasterPos3(pos.X, pos.Y, pos.Z);
		}

		public static void RasterPos4(Vector4 pos) {
			GL.RasterPos4(pos.X, pos.Y, pos.Z, pos.W);
		}

		public static void Vertex2(Vector2 v) {
			GL.Vertex2(v.X, v.Y);
		}

		public static void Vertex3(Vector3 v) {
			GL.Vertex3(v.X, v.Y, v.Z);
		}

		public static void Vertex4(Vector4 v) {
			GL.Vertex4(v.X, v.Y, v.Z, v.W);
		}

		public static void TexCoord2(Vector2 v) {
			GL.TexCoord2(v.X, v.Y);
		}

		public static void TexCoord3(Vector3 v) {
			GL.TexCoord3(v.X, v.Y, v.Z);
		}

		public static void TexCoord4(Vector4 v) {
			GL.TexCoord4(v.X, v.Y, v.Z, v.W);
		}

		public static void Rotate(Single angle, Vector3 axis) {
			GL.Rotate((Single)angle, axis.X, axis.Y, axis.Z);
		}

		public static void Scale(Vector3 scale) {
			GL.Scale(scale.X, scale.Y, scale.Z);
		}

		public static void Translate(Vector3 trans) {
			GL.Translate(trans.X, trans.Y, trans.Z);
		}

		public static void MultMatrix(ref Matrix mat) {
			unsafe {
				fixed (Single* m_ptr = &mat.Row0.X) {
					GL.MultMatrix((Single*)m_ptr);
				}
			}
		}

		public static void LoadMatrix(ref Matrix mat) {
			unsafe {
				fixed (Single* m_ptr = &mat.Row0.X) {
					GL.LoadMatrix((Single*)m_ptr);
				}
			}
		}

		public static void LoadTransposeMatrix(ref Matrix mat) {
			unsafe {
				fixed (Single* m_ptr = &mat.Row0.X) {
					GL.LoadTransposeMatrix((Single*)m_ptr);
				}
			}
		}

		public static void MultTransposeMatrix(ref Matrix mat) {
			unsafe {
				fixed (Single* m_ptr = &mat.Row0.X) {
					GL.MultTransposeMatrix((Single*)m_ptr);
				}
			}
		}
#endif

		#endregion

		#region Uniform

		public static void Uniform2(int location, ref Vector2 vector) {
			GL.Uniform2(location, vector.X, vector.Y);
		}

		public static void Uniform3(int location, ref Vector3 vector) {
			GL.Uniform3(location, vector.X, vector.Y, vector.Z);
		}

		public static void Uniform4(int location, ref Vector4 vector) {
			GL.Uniform4(location, vector.X, vector.Y, vector.Z, vector.W);
		}

		public static void Uniform2(int location, Vector2 vector) {
			GL.Uniform2(location, vector.X, vector.Y);
		}

		public static void Uniform3(int location, Vector3 vector) {
			GL.Uniform3(location, vector.X, vector.Y, vector.Z);
		}

		public static void Uniform4(int location, Vector4 vector) {
			GL.Uniform4(location, vector.X, vector.Y, vector.Z, vector.W);
		}

		public static void Uniform4(int location, Color4 color) {
			GL.Uniform4(location, color.R, color.G, color.B, color.A);
		}

		public static void Uniform4(int location, Quaternion quaternion) {
			GL.Uniform4(location, quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
		}

		public static void UniformMatrix4(int location, bool transpose, ref Matrix matrix) {
			unsafe {
				fixed (float* matrix_ptr = &matrix.Row0.X) {
					GL.UniformMatrix4(location, 1, transpose, matrix_ptr);
				}
			}
		}

		#endregion

		#region Shaders

		#region GetActiveAttrib

		public static string GetActiveAttrib(int program, int index, out int size, out ActiveAttribType type) {
			int length;
			GetProgram(program, Pencil.Gaming.Graphics.ProgramParameter.ActiveAttributeMaxLength, out length);
			StringBuilder sb = new StringBuilder(length == 0 ? 1 : length * 2);

			GetActiveAttrib(program, index, sb.Capacity, out length, out size, out type, sb);
			return sb.ToString();
		}

		#endregion

		#region GetActiveUniform

		public static string GetActiveUniform(int program, int uniformIndex, out int size, out ActiveUniformType type) {
			int length;
			GetProgram(program, Pencil.Gaming.Graphics.ProgramParameter.ActiveUniformMaxLength, out length);

			StringBuilder sb = new StringBuilder(length == 0 ? 1 : length);
			GetActiveUniform(program, uniformIndex, sb.Capacity, out length, out size, out type, sb);
			return sb.ToString();
		}

		#endregion

		#region GetActiveUniformName

		public static string GetActiveUniformName(int program, int uniformIndex) {
			int length;
			GetProgram(program, Pencil.Gaming.Graphics.ProgramParameter.ActiveUniformMaxLength, out length);
			StringBuilder sb = new StringBuilder(length == 0 ? 1 : length * 2);

			GetActiveUniformName(program, uniformIndex, sb.Capacity, out length, sb);
			return sb.ToString();
		}

		#endregion

		#region GetActiveUniformBlockName

		public static string GetActiveUniformBlockName(int program, int uniformIndex) {
			int length;
			GetProgram(program, Pencil.Gaming.Graphics.ProgramParameter.ActiveUniformBlockMaxNameLength, out length);
			StringBuilder sb = new StringBuilder(length == 0 ? 1 : length * 2);

			GetActiveUniformBlockName(program, uniformIndex, sb.Capacity, out length, sb);
			return sb.ToString();
		}

		#endregion

		#region public static void ShaderSource(Int32 shader, System.String @string)

		public static void ShaderSource(UInt32 shader, System.String @string) {
			unsafe {
				int length = @string.Length;
				GL.ShaderSource(shader, 1, new string[] { @string }, &length);
			}
		}

		#endregion

		#region public static string GetShaderInfoLog(Int32 shader)

		public static string GetShaderInfoLog(Int32 shader) {
			string info;
			GetShaderInfoLog(shader, out info);
			return info;
		}

		#endregion

		#region public static void GetShaderInfoLog(Int32 shader, out string info)

		public static void GetShaderInfoLog(Int32 shader, out string info) {
			unsafe {
				int length;
				GL.GetShader(shader, ShaderParameter.InfoLogLength, out length);
				if (length == 0) {
					info = String.Empty;
					return;
				}
				StringBuilder sb = new StringBuilder(length * 2);
				GL.GetShaderInfoLog((UInt32)shader, sb.Capacity, &length, sb);
				info = sb.ToString();
			}
		}

		#endregion

		#region public static string GetProgramInfoLog(Int32 program)

		public static string GetProgramInfoLog(Int32 program) {
			string info;
			GetProgramInfoLog(program, out info);
			return info;
		}

		#endregion

		#region public static void GetProgramInfoLog(Int32 program, out string info)

		public static void GetProgramInfoLog(Int32 program, out string info) {
			unsafe {
				int length;
				GL.GetProgram(program, Pencil.Gaming.Graphics.ProgramParameter.InfoLogLength, out length);
				if (length == 0) {
					info = String.Empty;
					return;
				}
				StringBuilder sb = new StringBuilder(length * 2);
				GL.GetProgramInfoLog((UInt32)program, sb.Capacity, &length, sb);
				info = sb.ToString();
			}
		}

		#endregion

		#endregion

		#region public static void PointParameter(PointSpriteCoordOriginParameter param)

		/// <summary>
		/// Helper function that defines the coordinate origin of the Point Sprite.
		/// </summary>
		/// <param name="param">
		/// A Pencil.Gaming.Graphics.GL.PointSpriteCoordOriginParameter token,
		/// denoting the origin of the Point Sprite.
		/// </param>
		public static void PointParameter(PointSpriteCoordOriginParameter param) {
			GL.PointParameter(PointParameterName.PointSpriteCoordOrigin, (int)param);
		}

		#endregion

		#region VertexAttrib|MultiTexCoord

		public static void VertexAttrib2(Int32 index, ref Vector2 v) {
			GL.VertexAttrib2(index, v.X, v.Y);
		}

		public static void VertexAttrib3(Int32 index, ref Vector3 v) {
			GL.VertexAttrib3(index, v.X, v.Y, v.Z);
		}

		public static void VertexAttrib4(Int32 index, ref Vector4 v) {
			GL.VertexAttrib4(index, v.X, v.Y, v.Z, v.W);
		}

		public static void VertexAttrib2(Int32 index, Vector2 v) {
			GL.VertexAttrib2(index, v.X, v.Y);
		}

		public static void VertexAttrib3(Int32 index, Vector3 v) {
			GL.VertexAttrib3(index, v.X, v.Y, v.Z);
		}

		public static void VertexAttrib4(Int32 index, Vector4 v) {
			GL.VertexAttrib4(index, v.X, v.Y, v.Z, v.W);
		}

#if USE_GL_COMPAT
		public static void MultiTexCoord2(TextureUnit target, ref Vector2 v) {
			GL.MultiTexCoord2(target, v.X, v.Y);
		}

		public static void MultiTexCoord3(TextureUnit target, ref Vector3 v) {
			GL.MultiTexCoord3(target, v.X, v.Y, v.Z);
		}

		public static void MultiTexCoord4(TextureUnit target, ref Vector4 v) {
			GL.MultiTexCoord4(target, v.X, v.Y, v.Z, v.W);
		}
#endif

		#endregion

		#region Rect

//		public static void Rect(RectangleF rect) {
//			GL.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
//		}
//
//		public static void Rect(Rectangle rect) {
//			GL.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
//		}
//
//		public static void Rect(ref RectangleF rect) {
//			GL.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
//		}
//
//		public static void Rect(ref Rectangle rect) {
//			GL.Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
//		}

		#endregion

		#region GenBuffer

		/// <summary>[requires: v1.5]
		/// Generates a single buffer object name
		/// </summary>
		/// <returns>The generated buffer object name</returns>
		public static int GenBuffer() {
			int id;
			GenBuffers(1, out id);
			return id;
		}

		#endregion

		#region DeleteBuffer

		/// <summary>[requires: v1.5]
		/// Deletes a single buffer object
		/// </summary>
		/// <param name="id">The buffer object to be deleted</param>
		public static void DeleteBuffer(int id) {
			DeleteBuffers(1, ref id);
		}

		/// <summary>[requires: v1.5]
		/// Deletes a single buffer object
		/// </summary>
		/// <param name="id">The buffer object to be deleted</param>
		public static void DeleteBuffer(uint id) {
			DeleteBuffers(1, ref id);
		}

		#endregion

		#region GenFramebuffer

		/// <summary>[requires: v3.0 and ARB_framebuffer_object]
		/// Generates a single framebuffer object name
		/// </summary>
		/// <returns>The generated framebuffer object name</returns>
		public static int GenFramebuffer() {
			int id;
			GenFramebuffers(1, out id);
			return id;
		}

		#endregion

		#region DeleteFramebuffer

		/// <summary>[requires: v3.0 and ARB_framebuffer_object]
		/// Deletes a single framebuffer object
		/// </summary>
		/// <param name="id">The framebuffer object to be deleted</param>
		public static void DeleteFramebuffer(int id) {
			DeleteFramebuffers(1, ref id);
		}

		/// <summary>[requires: v3.0 and ARB_framebuffer_object]
		/// Deletes a single framebuffer object
		/// </summary>
		/// <param name="id">The framebuffer object to be deleted</param>
		public static void DeleteFramebuffer(uint id) {
			DeleteFramebuffers(1, ref id);
		}

		#endregion

		#region GenProgramPipeline

		/// <summary>[requires: v4.1 and ARB_separate_shader_objects]
		/// Generates a single single pipeline object name
		/// </summary>
		/// <returns>The generated single pipeline object name</returns>
		public static int GenProgramPipeline() {
			int id;
			GenProgramPipelines(1, out id);
			return id;
		}

		#endregion

		#region DeleteProgramPipeline

		/// <summary>[requires: v4.1 and ARB_separate_shader_objects]
		/// Deletes a single program pipeline object
		/// </summary>
		/// <param name="id">The program pipeline object to be deleted</param>
		public static void DeleteProgramPipeline(int id) {
			DeleteProgramPipelines(1, ref id);
		}

		/// <summary>[requires: v4.1 and ARB_separate_shader_objects]
		/// Deletes a single program pipeline object
		/// </summary>
		/// <param name="id">The program pipeline object to be deleted</param>
		public static void DeleteProgramPipeline(uint id) {
			DeleteProgramPipelines(1, ref id);
		}

		#endregion

		#region GenQuery

		/// <summary>[requires: v1.5]
		/// Generates a single query object name
		/// </summary>
		/// <returns>The generated query object name</returns>
		public static int GenQuery() {
			int id;
			GenQueries(1, out id);
			return id;
		}

		#endregion

		#region DeleteQuery

		/// <summary>[requires: v1.5]
		/// Deletes a single query object
		/// </summary>
		/// <param name="id">The query object to be deleted</param>
		public static void DeleteQuery(int id) {
			DeleteQueries(1, ref id);
		}

		/// <summary>
		/// Deletes a single query object
		/// </summary>
		/// <param name="id">The query object to be deleted</param>
		public static void DeleteQuery(uint id) {
			DeleteQueries(1, ref id);
		}

		#endregion

		#region GenRenderbuffer

		/// <summary>[requires: v3.0 and ARB_framebuffer_object]
		/// Generates a single renderbuffer object name
		/// </summary>
		/// <returns>The generated renderbuffer object name</returns>
		public static int GenRenderbuffer() {
			int id;
			GenRenderbuffers(1, out id);
			return id;
		}

		#endregion

		#region DeleteRenderbuffer

		/// <summary>[requires: v3.0 and ARB_framebuffer_object]
		/// Deletes a single renderbuffer object
		/// </summary>
		/// <param name="id">The renderbuffer object to be deleted</param>
		public static void DeleteRenderbuffer(int id) {
			DeleteRenderbuffers(1, ref id);
		}

		/// <summary>[requires: v3.0 and ARB_framebuffer_object]
		/// Deletes a single renderbuffer object
		/// </summary>
		/// <param name="id">The renderbuffer object to be deleted</param>
		public static void DeleteRenderbuffer(uint id) {
			DeleteRenderbuffers(1, ref id);
		}

		#endregion

		#region GenSampler

		/// <summary>
		/// Generates a single sampler object name
		/// </summary>
		/// <returns>The generated sampler object name</returns>
		public static int GenSampler() {
			int id;
			GenSamplers(1, out id);
			return id;
		}

		#endregion

		#region DeleteSampler

		/// <summary>
		/// Deletes a single sampler object
		/// </summary>
		/// <param name="id">The sampler object to be deleted</param>
		public static void DeleteSampler(int id) {
			DeleteSamplers(1, ref id);
		}

		/// <summary>
		/// Deletes a single sampler object
		/// </summary>
		/// <param name="id">The sampler object to be deleted</param>
		public static void DeleteSampler(uint id) {
			DeleteSamplers(1, ref id);
		}

		#endregion

		#region GenTexture

		/// <summary>[requires: v1.1]
		/// Generate a single texture name
		/// </summary>
		/// <returns>The generated texture name</returns>
		public static int GenTexture() {
			int id;
			GenTextures(1, out id);
			return id;
		}

		#endregion

		#region DeleteTexture

		/// <summary>[requires: v1.1]
		/// Delete a single texture name
		/// </summary>
		/// <param name="id">The texture to be deleted</param>
		public static void DeleteTexture(int id) {
			DeleteTextures(1, ref id);
		}

		/// <summary>[requires: v1.1]
		/// Delete a single texture name
		/// </summary>
		/// <param name="id">The texture to be deleted</param>
		public static void DeleteTexture(uint id) {
			DeleteTextures(1, ref id);
		}

		#endregion

		#region GenTransformFeedback

		/// <summary>[requires: v1.2 and ARB_transform_feedback2]
		/// Generates a single transform feedback object name
		/// </summary>
		/// <returns>The generated transform feedback object name</returns>
		public static int GenTransformFeedback() {
			int id;
			GenTransformFeedback(1, out id);
			return id;
		}

		#endregion

		#region DeleteTransformFeedback

		/// <summary>[requires: v1.2 and ARB_transform_feedback2]
		/// Deletes a single transform feedback object
		/// </summary>
		/// <param name="id">The transform feedback object to be deleted</param>
		public static void DeleteTransformFeedback(int id) {
			DeleteTransformFeedback(1, ref id);
		}

		/// <summary>[requires: v1.2 and ARB_transform_feedback2]
		/// Deletes a single transform feedback object
		/// </summary>
		/// <param name="id">The transform feedback object to be deleted</param>
		public static void DeleteTransformFeedback(uint id) {
			DeleteTransformFeedback(1, ref id);
		}

		#endregion

		#region GenVertexArray

		/// <summary>[requires: v3.0 and ARB_vertex_array_object]
		/// Generates a single vertex array object name
		/// </summary>
		/// <returns>The generated vertex array object name</returns>
		public static int GenVertexArray() {
			int id;
			GenVertexArrays(1, out id);
			return id;
		}

		#endregion

		#region DeleteVertexArray

		/// <summary>[requires: v3.0 and ARB_vertex_array_object]
		/// Deletes a single vertex array object
		/// </summary>
		/// <param name="id">The vertex array object to be deleted</param>
		public static void DeleteVertexArray(int id) {
			DeleteVertexArrays(1, ref id);
		}

		/// <summary>[requires: v3.0 and ARB_vertex_array_object]
		/// Deletes a single vertex array object
		/// </summary>
		/// <param name="id">The vertex array object to be deleted</param>
		public static void DeleteVertexArray(uint id) {
			DeleteVertexArrays(1, ref id);
		}

		#endregion

		#region [Vertex|Normal|Index|Color|FogCoord|VertexAttrib]Pointer

#if USE_GL_COMPAT
		public static void VertexPointer(int size, VertexPointerType type, int stride, int offset) {
			VertexPointer(size, type, stride, (IntPtr)offset);
		}

		public static void NormalPointer(NormalPointerType type, int stride, int offset) {
			NormalPointer(type, stride, (IntPtr)offset);
		}

		public static void IndexPointer(IndexPointerType type, int stride, int offset) {
			IndexPointer(type, stride, (IntPtr)offset);
		}

		public static void ColorPointer(int size, ColorPointerType type, int stride, int offset) {
			ColorPointer(size, type, stride, (IntPtr)offset);
		}

		public static void FogCoordPointer(FogPointerType type, int stride, int offset) {
			FogCoordPointer(type, stride, (IntPtr)offset);
		}

		public static void EdgeFlagPointer(int stride, int offset) {
			EdgeFlagPointer(stride, (IntPtr)offset);
		}

		public static void TexCoordPointer(int size, TexCoordPointerType type, int stride, int offset) {
			TexCoordPointer(size, type, stride, (IntPtr)offset);
		}

#endif

		public static void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset) {
			VertexAttribPointer(index, size, type, normalized, stride, (IntPtr)offset);
		}

		#endregion

		#region DrawElements

		public static void DrawElements(BeginMode mode, int count, DrawElementsType type, int offset) {
			DrawElements(mode, count, type, new IntPtr(offset));
		}

		#endregion

		#region Get[Float|Double]

		public static void GetFloat(GetPName pname, out Vector2 vector) {
			unsafe {
				fixed (Vector2* ptr = &vector)
					GetFloat(pname, (float*)ptr);
			}
		}

		public static void GetFloat(GetPName pname, out Vector3 vector) {
			unsafe {
				fixed (Vector3* ptr = &vector)
					GetFloat(pname, (float*)ptr);
			}
		}

		public static void GetFloat(GetPName pname, out Vector4 vector) {
			unsafe {
				fixed (Vector4* ptr = &vector)
					GetFloat(pname, (float*)ptr);
			}
		}

		public static void GetFloat(GetPName pname, out Matrix matrix) {
			unsafe {
				fixed (Matrix* ptr = &matrix)
					GetFloat(pname, (float*)ptr);
			}
		}

		#endregion

		#region Viewport

//		public static void Viewport(Size size) {
//			GL.Viewport(0, 0, size.Width, size.Height);
//		}
//
//		public static void Viewport(Point location, Size size) {
//			GL.Viewport(location.X, location.Y, size.Width, size.Height);
//		}
//
//		public static void Viewport(Rectangle rectangle) {
//			GL.Viewport(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
//		}
#if NO_SYSDRAWING
		public static void Viewport(Point location, Size size)
		{
			GL.Viewport(location.X, location.Y, size.Width, size.Height);
		}

		public static void Viewport(Rectangle rectangle)
		{
			GL.Viewport(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
#endif
		#endregion

		#region TexEnv

//		public static void TexEnv(TextureEnvTarget target, TextureEnvParameter pname, Color color) {
//			Color4 c = new Color4(color.R, color.G, color.B, color.A);
//			unsafe {
//				TexEnv(target, pname, &c.R);
//			}
//		}

#if USE_GL_COMPAT
		public static void TexEnv(TextureEnvTarget target, TextureEnvParameter pname, Color4 color) {
			unsafe {
				TexEnv(target, pname, &color.R);
			}
		}
#endif

		#endregion

#pragma warning restore 3019
#pragma warning restore 1591
#pragma warning restore 1572
#pragma warning restore 1573

		#endregion
	}
}
