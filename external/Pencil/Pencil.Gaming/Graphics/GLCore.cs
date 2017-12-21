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
using System.Runtime.InteropServices;

namespace Pencil.Gaming.Graphics {
	internal static unsafe class GLCore {
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glAccum(AccumOp op, Single value);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glAlphaFunc(AlphaFunction func, Single @ref);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern bool glAreTexturesResident(Int32 n, UInt32* textures, [OutAttribute] bool* residences);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glArrayElement(Int32 i);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glBegin(BeginMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glBindTexture(TextureTarget target, UInt32 texture);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glBitmap(Int32 width, Int32 height, Single xorig, Single yorig, Single xmove, Single ymove, Byte* bitmap);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glBlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glCallList(UInt32 list);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glCallLists(Int32 n, ListNameType type, IntPtr lists);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glClear(ClearBufferMask mask);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glClearAccum(Single red, Single green, Single blue, Single alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glClearColor(Single red, Single green, Single blue, Single alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glClearDepth(Double depth);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glClearIndex(Single c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glClearStencil(Int32 s);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glClipPlane(ClipPlaneName plane, Double* equation);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3b(SByte red, SByte green, SByte blue);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3bv(SByte* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3d(Double red, Double green, Double blue);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3f(Single red, Single green, Single blue);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3i(Int32 red, Int32 green, Int32 blue);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3s(Int16 red, Int16 green, Int16 blue);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3ub(Byte red, Byte green, Byte blue);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3ubv(Byte* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3ui(UInt32 red, UInt32 green, UInt32 blue);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3uiv(UInt32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3us(UInt16 red, UInt16 green, UInt16 blue);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor3usv(UInt16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4b(SByte red, SByte green, SByte blue, SByte alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4bv(SByte* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4d(Double red, Double green, Double blue, Double alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4f(Single red, Single green, Single blue, Single alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4i(Int32 red, Int32 green, Int32 blue, Int32 alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4s(Int16 red, Int16 green, Int16 blue, Int16 alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4ub(Byte red, Byte green, Byte blue, Byte alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4ubv(Byte* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4ui(UInt32 red, UInt32 green, UInt32 blue, UInt32 alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4uiv(UInt32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4us(UInt16 red, UInt16 green, UInt16 blue, UInt16 alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColor4usv(UInt16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColorMask(bool red, bool green, bool blue, bool alpha);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColorMaterial(MaterialFace face, ColorMaterialParameter mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glColorPointer(Int32 size, ColorPointerType type, Int32 stride, IntPtr pointer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glCopyPixels(Int32 x, Int32 y, Int32 width, Int32 height, PixelCopyType type);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glCopyTexImage1D(TextureTarget target, Int32 level, PixelInternalFormat internalformat, Int32 x, Int32 y, Int32 width, Int32 border);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glCopyTexImage2D(TextureTarget target, Int32 level, PixelInternalFormat internalformat, Int32 x, Int32 y, Int32 width, Int32 height, Int32 border);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glCopyTexSubImage1D(TextureTarget target, Int32 level, Int32 xoffset, Int32 x, Int32 y, Int32 width);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glCopyTexSubImage2D(TextureTarget target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 x, Int32 y, Int32 width, Int32 height);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glCullFace(CullFaceMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDeleteLists(UInt32 list, Int32 range);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDeleteTextures(Int32 n, UInt32* textures);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDepthFunc(DepthFunction func);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDepthMask(bool flag);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDepthRange(Double near, Double far);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDisable(EnableCap cap);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDisableClientState(ArrayCap array);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDrawArrays(BeginMode mode, Int32 first, Int32 count);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDrawBuffer(DrawBufferMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDrawElements(BeginMode mode, Int32 count, DrawElementsType type, IntPtr indices);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glDrawPixels(Int32 width, Int32 height, PixelFormat format, PixelType type, IntPtr pixels);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEdgeFlag(bool flag);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEdgeFlagPointer(Int32 stride, IntPtr pointer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEdgeFlagv(bool* flag);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEnable(EnableCap cap);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEnableClientState(ArrayCap array);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEnd();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEndList();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalCoord1d(Double u);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalCoord1dv(Double* u);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalCoord1f(Single u);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalCoord1fv(Single* u);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalCoord2d(Double u, Double v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalCoord2dv(Double* u);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalCoord2f(Single u, Single v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalCoord2fv(Single* u);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalMesh1(MeshMode1 mode, Int32 i1, Int32 i2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalMesh2(MeshMode2 mode, Int32 i1, Int32 i2, Int32 j1, Int32 j2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalPoint1(Int32 i);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glEvalPoint2(Int32 i, Int32 j);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFeedbackBuffer(Int32 size, FeedbackType type, [OutAttribute] Single* buffer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFinish();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFlush();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFogf(FogParameter pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFogfv(FogParameter pname, Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFogi(FogParameter pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFogiv(FogParameter pname, Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFrontFace(FrontFaceDirection mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glFrustum(Double left, Double right, Double bottom, Double top, Double zNear, Double zFar);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int glGenLists(Int32 range);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGenTextures(Int32 n, [OutAttribute] UInt32* textures);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetBooleanv(GetPName pname, [OutAttribute] bool* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetClipPlane(ClipPlaneName plane, [OutAttribute] Double* equation);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetDoublev(GetPName pname, [OutAttribute] Double* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern ErrorCode glGetError();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetFloatv(GetPName pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetIntegerv(GetPName pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetLightfv(LightName light, LightParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetLightiv(LightName light, LightParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetMapdv(MapTarget target, GetMapQuery query, [OutAttribute] Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetMapfv(MapTarget target, GetMapQuery query, [OutAttribute] Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetMapiv(MapTarget target, GetMapQuery query, [OutAttribute] Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetMaterialfv(MaterialFace face, MaterialParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetMaterialiv(MaterialFace face, MaterialParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetPixelMapfv(PixelMap map, [OutAttribute] Single* values);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetPixelMapuiv(PixelMap map, [OutAttribute] UInt32* values);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetPixelMapusv(PixelMap map, [OutAttribute] UInt16* values);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetPointerv(GetPointervPName pname, [OutAttribute] IntPtr @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetPolygonStipple([OutAttribute] Byte* mask);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr glGetString(StringName name);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexEnvfv(TextureEnvTarget target, TextureEnvParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexEnviv(TextureEnvTarget target, TextureEnvParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexGendv(TextureCoordName coord, TextureGenParameter pname, [OutAttribute] Double* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexGenfv(TextureCoordName coord, TextureGenParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexGeniv(TextureCoordName coord, TextureGenParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexImage(TextureTarget target, Int32 level, PixelFormat format, PixelType type, [OutAttribute] IntPtr pixels);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexLevelParameterfv(TextureTarget target, Int32 level, GetTextureParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexLevelParameteriv(TextureTarget target, Int32 level, GetTextureParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexParameterfv(TextureTarget target, GetTextureParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glGetTexParameteriv(TextureTarget target, GetTextureParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glHint(HintTarget target, HintMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexMask(UInt32 mask);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexPointer(IndexPointerType type, Int32 stride, IntPtr pointer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexd(Double c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexdv(Double* c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexf(Single c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexfv(Single* c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexi(Int32 c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexiv(Int32* c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexs(Int16 c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexsv(Int16* c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexub(Byte c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glIndexubv(Byte* c);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glInitNames();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glInterleavedArrays(InterleavedArrayFormat format, Int32 stride, IntPtr pointer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern bool glIsEnabled(EnableCap cap);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern bool glIsList(UInt32 list);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern bool glIsTexture(UInt32 texture);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLightModelf(LightModelParameter pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLightModelfv(LightModelParameter pname, Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLightModeli(LightModelParameter pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLightModeliv(LightModelParameter pname, Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLightf(LightName light, LightParameter pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLightfv(LightName light, LightParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLighti(LightName light, LightParameter pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLightiv(LightName light, LightParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLineStipple(Int32 factor, UInt16 pattern);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLineWidth(Single width);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glListBase(UInt32 @base);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLoadIdentity();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLoadMatrixd(Double* m);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLoadMatrixf(Single* m);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLoadName(UInt32 name);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glLogicOp(LogicOp opcode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMap1d(MapTarget target, Double u1, Double u2, Int32 stride, Int32 order, Double* points);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMap1f(MapTarget target, Single u1, Single u2, Int32 stride, Int32 order, Single* points);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMap2d(MapTarget target, Double u1, Double u2, Int32 ustride, Int32 uorder, Double v1, Double v2, Int32 vstride, Int32 vorder, Double* points);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMap2f(MapTarget target, Single u1, Single u2, Int32 ustride, Int32 uorder, Single v1, Single v2, Int32 vstride, Int32 vorder, Single* points);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMapGrid1d(Int32 un, Double u1, Double u2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMapGrid1f(Int32 un, Single u1, Single u2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMapGrid2d(Int32 un, Double u1, Double u2, Int32 vn, Double v1, Double v2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMapGrid2f(Int32 un, Single u1, Single u2, Int32 vn, Single v1, Single v2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMaterialf(MaterialFace face, MaterialParameter pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMaterialfv(MaterialFace face, MaterialParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMateriali(MaterialFace face, MaterialParameter pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMaterialiv(MaterialFace face, MaterialParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMatrixMode(MatrixMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMultMatrixd(Double* m);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glMultMatrixf(Single* m);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNewList(UInt32 list, ListMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3b(SByte nx, SByte ny, SByte nz);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3bv(SByte* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3d(Double nx, Double ny, Double nz);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3f(Single nx, Single ny, Single nz);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3i(Int32 nx, Int32 ny, Int32 nz);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3s(Int16 nx, Int16 ny, Int16 nz);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormal3sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glNormalPointer(NormalPointerType type, Int32 stride, IntPtr pointer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glOrtho(Double left, Double right, Double bottom, Double top, Double zNear, Double zFar);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPassThrough(Single token);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPixelMapfv(PixelMap map, Int32 mapsize, Single* values);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPixelMapuiv(PixelMap map, Int32 mapsize, UInt32* values);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPixelMapusv(PixelMap map, Int32 mapsize, UInt16* values);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPixelStoref(PixelStoreParameter pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPixelStorei(PixelStoreParameter pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPixelTransferf(PixelTransferParameter pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPixelTransferi(PixelTransferParameter pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPixelZoom(Single xfactor, Single yfactor);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPointSize(Single size);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPolygonMode(MaterialFace face, PolygonMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPolygonOffset(Single factor, Single units);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPolygonStipple([OutAttribute] Byte* mask);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPopAttrib();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPopClientAttrib();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPopMatrix();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPopName();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPrioritizeTextures(Int32 n, UInt32* textures, Single* priorities);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPushAttrib(AttribMask mask);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPushClientAttrib(ClientAttribMask mask);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPushMatrix();
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glPushName(UInt32 name);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos2d(Double x, Double y);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos2dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos2f(Single x, Single y);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos2fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos2i(Int32 x, Int32 y);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos2iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos2s(Int16 x, Int16 y);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos2sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos3d(Double x, Double y, Double z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos3dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos3f(Single x, Single y, Single z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos3fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos3i(Int32 x, Int32 y, Int32 z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos3iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos3s(Int16 x, Int16 y, Int16 z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos3sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos4d(Double x, Double y, Double z, Double w);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos4dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos4f(Single x, Single y, Single z, Single w);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos4fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos4i(Int32 x, Int32 y, Int32 z, Int32 w);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos4iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos4s(Int16 x, Int16 y, Int16 z, Int16 w);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRasterPos4sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glReadBuffer(ReadBufferMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glReadPixels(Int32 x, Int32 y, Int32 width, Int32 height, PixelFormat format, PixelType type, [OutAttribute] IntPtr pixels);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRectd(Double x1, Double y1, Double x2, Double y2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRectdv(Double* v1, Double* v2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRectf(Single x1, Single y1, Single x2, Single y2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRectfv(Single* v1, Single* v2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRecti(Int32 x1, Int32 y1, Int32 x2, Int32 y2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRectiv(Int32* v1, Int32* v2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRects(Int16 x1, Int16 y1, Int16 x2, Int16 y2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRectsv(Int16* v1, Int16* v2);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int glRenderMode(RenderingMode mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRotated(Double angle, Double x, Double y, Double z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glRotatef(Single angle, Single x, Single y, Single z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glScaled(Double x, Double y, Double z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glScalef(Single x, Single y, Single z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glScissor(Int32 x, Int32 y, Int32 width, Int32 height);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glSelectBuffer(Int32 size, [OutAttribute] UInt32* buffer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glShadeModel(ShadingModel mode);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glStencilFunc(StencilFunction func, Int32 @ref, UInt32 mask);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glStencilMask(UInt32 mask);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glStencilOp(StencilOp fail, StencilOp zfail, StencilOp zpass);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord1d(Double s);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord1dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord1f(Single s);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord1fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord1i(Int32 s);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord1iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord1s(Int16 s);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord1sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord2d(Double s, Double t);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord2dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord2f(Single s, Single t);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord2fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord2i(Int32 s, Int32 t);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord2iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord2s(Int16 s, Int16 t);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord2sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord3d(Double s, Double t, Double r);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord3dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord3f(Single s, Single t, Single r);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord3fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord3i(Int32 s, Int32 t, Int32 r);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord3iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord3s(Int16 s, Int16 t, Int16 r);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord3sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord4d(Double s, Double t, Double r, Double q);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord4dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord4f(Single s, Single t, Single r, Single q);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord4fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord4i(Int32 s, Int32 t, Int32 r, Int32 q);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord4iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord4s(Int16 s, Int16 t, Int16 r, Int16 q);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoord4sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexCoordPointer(Int32 size, TexCoordPointerType type, Int32 stride, IntPtr pointer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexEnvf(TextureEnvTarget target, TextureEnvParameter pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexEnvfv(TextureEnvTarget target, TextureEnvParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexEnvi(TextureEnvTarget target, TextureEnvParameter pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexEnviv(TextureEnvTarget target, TextureEnvParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexGend(TextureCoordName coord, TextureGenParameter pname, Double param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexGendv(TextureCoordName coord, TextureGenParameter pname, [OutAttribute] Double* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexGenf(TextureCoordName coord, TextureGenParameter pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexGenfv(TextureCoordName coord, TextureGenParameter pname, [OutAttribute] Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexGeni(TextureCoordName coord, TextureGenParameter pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexGeniv(TextureCoordName coord, TextureGenParameter pname, [OutAttribute] Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexImage1D(TextureTarget target, Int32 level, PixelInternalFormat internalformat, Int32 width, Int32 border, PixelFormat format, PixelType type, IntPtr data);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexImage2D(TextureTarget target, Int32 level, PixelInternalFormat internalformat, Int32 width, Int32 height, Int32 border, PixelFormat format, PixelType type, IntPtr data);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexParameterf(TextureTarget target, TextureParameterName pname, Single param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexParameterfv(TextureTarget target, TextureParameterName pname, Single* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexParameteri(TextureTarget target, TextureParameterName pname, Int32 param);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexParameteriv(TextureTarget target, TextureParameterName pname, Int32* @params);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexSubImage1D(TextureTarget target, Int32 level, Int32 xoffset, Int32 width, PixelFormat format, PixelType type, IntPtr pixels);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTexSubImage2D(TextureTarget target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 width, Int32 height, PixelFormat format, PixelType type, IntPtr pixels);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTranslated(Double x, Double y, Double z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glTranslatef(Single x, Single y, Single z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex2d(Double x, Double y);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex2dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex2f(Single x, Single y);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex2fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex2i(Int32 x, Int32 y);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex2iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex2s(Int16 x, Int16 y);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex2sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex3d(Double x, Double y, Double z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex3dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex3f(Single x, Single y, Single z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex3fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex3i(Int32 x, Int32 y, Int32 z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex3iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex3s(Int16 x, Int16 y, Int16 z);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex3sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex4d(Double x, Double y, Double z, Double w);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex4dv(Double* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex4f(Single x, Single y, Single z, Single w);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex4fv(Single* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex4i(Int32 x, Int32 y, Int32 z, Int32 w);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex4iv(Int32* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex4s(Int16 x, Int16 y, Int16 z, Int16 w);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertex4sv(Int16* v);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glVertexPointer(Int32 size, VertexPointerType type, Int32 stride, IntPtr pointer);
		[DllImport("opengl32.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void glViewport(Int32 x, Int32 y, Int32 width, Int32 height);
	}
}