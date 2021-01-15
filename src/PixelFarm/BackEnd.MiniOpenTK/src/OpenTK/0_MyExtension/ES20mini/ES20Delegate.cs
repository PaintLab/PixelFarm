//autogen 2018-10-12 05:20:44Z
namespace OpenTK.Graphics.ES20
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;

    [System.Security.SuppressUnmanagedCodeSecurity()] //apply to all members
    static class Delegates
    {

        //m* 1

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BeginPerfMonitorAMD(UInt32 monitor);
        public static BeginPerfMonitorAMD glBeginPerfMonitorAMD;


        //m* 2

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeletePerfMonitorsAMD(Int32 n, [CountAttribute(Parameter = "n")] UInt32* monitors);
        public static DeletePerfMonitorsAMD glDeletePerfMonitorsAMD;


        //m* 3

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EndPerfMonitorAMD(UInt32 monitor);
        public static EndPerfMonitorAMD glEndPerfMonitorAMD;


        //m* 4

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenPerfMonitorsAMD(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* monitors);
        public static GenPerfMonitorsAMD glGenPerfMonitorsAMD;


        //m* 5

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfMonitorCounterDataAMD(UInt32 monitor, System.Int32 pname, Int32 dataSize, [OutAttribute, CountAttribute(Parameter = "dataSize")] UInt32* data, [OutAttribute, CountAttribute(Count = 1)] Int32* bytesWritten);
        public static GetPerfMonitorCounterDataAMD glGetPerfMonitorCounterDataAMD;


        //m* 6

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void GetPerfMonitorCounterInfoAMD(UInt32 group, UInt32 counter, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] IntPtr data);
        public static GetPerfMonitorCounterInfoAMD glGetPerfMonitorCounterInfoAMD;


        //m* 7

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfMonitorCountersAMD(UInt32 group, [OutAttribute, CountAttribute(Count = 1)] Int32* numCounters, [OutAttribute, CountAttribute(Count = 1)] Int32* maxActiveCounters, Int32 counterSize, [OutAttribute, CountAttribute(Parameter = "counterSize")] UInt32* counters);
        public static GetPerfMonitorCountersAMD glGetPerfMonitorCountersAMD;


        //m* 8

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfMonitorCounterStringAMD(UInt32 group, UInt32 counter, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr counterString);
        public static GetPerfMonitorCounterStringAMD glGetPerfMonitorCounterStringAMD;


        //m* 9

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfMonitorGroupsAMD([OutAttribute, CountAttribute(Count = 1)] Int32* numGroups, Int32 groupsSize, [OutAttribute, CountAttribute(Parameter = "groupsSize")] UInt32* groups);
        public static GetPerfMonitorGroupsAMD glGetPerfMonitorGroupsAMD;


        //m* 10

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfMonitorGroupStringAMD(UInt32 group, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr groupString);
        public static GetPerfMonitorGroupStringAMD glGetPerfMonitorGroupStringAMD;


        //m* 11

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void SelectPerfMonitorCountersAMD(UInt32 monitor, bool enable, UInt32 group, Int32 numCounters, [OutAttribute, CountAttribute(Parameter = "numCounters")] UInt32* counterList);
        public static SelectPerfMonitorCountersAMD glSelectPerfMonitorCountersAMD;


        //m* 12

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlitFramebufferANGLE(Int32 srcX0, Int32 srcY0, Int32 srcX1, Int32 srcY1, Int32 dstX0, Int32 dstY0, Int32 dstX1, Int32 dstY1, System.Int32 mask, System.Int32 filter);
        public static BlitFramebufferANGLE glBlitFramebufferANGLE;


        //m* 13

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawArraysInstancedANGLE(System.Int32 mode, Int32 first, Int32 count, Int32 primcount);
        public static DrawArraysInstancedANGLE glDrawArraysInstancedANGLE;


        //m* 14

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsInstancedANGLE(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 primcount);
        public static DrawElementsInstancedANGLE glDrawElementsInstancedANGLE;


        //m* 15

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetTranslatedShaderSourceANGLE(UInt32 shader, Int32 bufsize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufsize")] IntPtr source);
        public static GetTranslatedShaderSourceANGLE glGetTranslatedShaderSourceANGLE;


        //m* 16

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void RenderbufferStorageMultisampleANGLE(System.Int32 target, Int32 samples, System.Int32 internalformat, Int32 width, Int32 height);
        public static RenderbufferStorageMultisampleANGLE glRenderbufferStorageMultisampleANGLE;


        //m* 17

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void VertexAttribDivisorANGLE(UInt32 index, UInt32 divisor);
        public static VertexAttribDivisorANGLE glVertexAttribDivisorANGLE;


        //m* 18

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 ClientWaitSyncAPPLE(IntPtr sync, System.Int32 flags, UInt64 timeout);
        public static ClientWaitSyncAPPLE glClientWaitSyncAPPLE;


        //m* 19

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CopyTextureLevelsAPPLE(UInt32 destinationTexture, UInt32 sourceTexture, Int32 sourceBaseLevel, Int32 sourceLevelCount);
        public static CopyTextureLevelsAPPLE glCopyTextureLevelsAPPLE;


        //m* 20

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DeleteSyncAPPLE(IntPtr sync);
        public static DeleteSyncAPPLE glDeleteSyncAPPLE;


        //m* 21

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate IntPtr FenceSyncAPPLE(System.Int32 condition, System.Int32 flags);
        public static FenceSyncAPPLE glFenceSyncAPPLE;


        //m* 22

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetInteger64vAPPLE(System.Int32 pname, [OutAttribute] Int64* @params);
        public static GetInteger64vAPPLE glGetInteger64vAPPLE;


        //m* 23

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetSyncivAPPLE(IntPtr sync, System.Int32 pname, Int32 bufSize, [OutAttribute] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] Int32* values);
        public static GetSyncivAPPLE glGetSyncivAPPLE;


        //m* 24

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsSyncAPPLE(IntPtr sync);
        public static IsSyncAPPLE glIsSyncAPPLE;


        //m* 25

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void RenderbufferStorageMultisampleAPPLE(System.Int32 target, Int32 samples, System.Int32 internalformat, Int32 width, Int32 height);
        public static RenderbufferStorageMultisampleAPPLE glRenderbufferStorageMultisampleAPPLE;


        //m* 26

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ResolveMultisampleFramebufferAPPLE();
        public static ResolveMultisampleFramebufferAPPLE glResolveMultisampleFramebufferAPPLE;


        //m* 27

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void WaitSyncAPPLE(IntPtr sync, System.Int32 flags, UInt64 timeout);
        public static WaitSyncAPPLE glWaitSyncAPPLE;


        //m* 28

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ApplyFramebufferAttachmentCMAAINTEL();
        public static ApplyFramebufferAttachmentCMAAINTEL glApplyFramebufferAttachmentCMAAINTEL;


        //m* 29

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ActiveTexture(System.Int32 texture);
        public static ActiveTexture glActiveTexture;


        //m* 30

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void AttachShader(UInt32 program, UInt32 shader);
        public static AttachShader glAttachShader;


        //m* 31

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindAttribLocation(UInt32 program, UInt32 index, string name);
        public static BindAttribLocation glBindAttribLocation;


        //m* 32

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindBuffer(System.Int32 target, UInt32 buffer);
        public static BindBuffer glBindBuffer;


        //m* 33

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindFramebuffer(System.Int32 target, UInt32 framebuffer);
        public static BindFramebuffer glBindFramebuffer;


        //m* 34

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindRenderbuffer(System.Int32 target, UInt32 renderbuffer);
        public static BindRenderbuffer glBindRenderbuffer;


        //m* 35

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindTexture(System.Int32 target, UInt32 texture);
        public static BindTexture glBindTexture;


        //m* 36

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendColor(Single red, Single green, Single blue, Single alpha);
        public static BlendColor glBlendColor;


        //m* 37

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendEquation(System.Int32 mode);
        public static BlendEquation glBlendEquation;


        //m* 38

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendEquationSeparate(System.Int32 modeRGB, System.Int32 modeAlpha);
        public static BlendEquationSeparate glBlendEquationSeparate;


        //m* 39

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendFunc(System.Int32 sfactor, System.Int32 dfactor);
        public static BlendFunc glBlendFunc;


        //m* 40

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendFuncSeparate(System.Int32 sfactorRGB, System.Int32 dfactorRGB, System.Int32 sfactorAlpha, System.Int32 dfactorAlpha);
        public static BlendFuncSeparate glBlendFuncSeparate;


        //m* 41

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BufferData(System.Int32 target, IntPtr size, [CountAttribute(Parameter = "size")] IntPtr data, System.Int32 usage);
        public static BufferData glBufferData;


        //m* 42

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BufferSubData(System.Int32 target, IntPtr offset, IntPtr size, [CountAttribute(Parameter = "size")] IntPtr data);
        public static BufferSubData glBufferSubData;


        //m* 43

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 CheckFramebufferStatus(System.Int32 target);
        public static CheckFramebufferStatus glCheckFramebufferStatus;


        //m* 44

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Clear(System.Int32 mask);
        public static Clear glClear;


        //m* 45

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ClearColor(Single red, Single green, Single blue, Single alpha);
        public static ClearColor glClearColor;


        //m* 46

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ClearDepthf(Single d);
        public static ClearDepthf glClearDepthf;


        //m* 47

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ClearStencil(Int32 s);
        public static ClearStencil glClearStencil;


        //m* 48

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ColorMask(bool red, bool green, bool blue, bool alpha);
        public static ColorMask glColorMask;


        //m* 49

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CompileShader(UInt32 shader);
        public static CompileShader glCompileShader;


        //m* 50

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CompressedTexImage2D(System.Int32 target, Int32 level, System.Int32 internalformat, Int32 width, Int32 height, Int32 border, Int32 imageSize, [CountAttribute(Parameter = "imageSize")] IntPtr data);
        public static CompressedTexImage2D glCompressedTexImage2D;


        //m* 51

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CompressedTexSubImage2D(System.Int32 target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 width, Int32 height, System.Int32 format, Int32 imageSize, [CountAttribute(Parameter = "imageSize")] IntPtr data);
        public static CompressedTexSubImage2D glCompressedTexSubImage2D;


        //m* 52

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CopyTexImage2D(System.Int32 target, Int32 level, System.Int32 internalformat, Int32 x, Int32 y, Int32 width, Int32 height, Int32 border);
        public static CopyTexImage2D glCopyTexImage2D;


        //m* 53

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CopyTexSubImage2D(System.Int32 target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 x, Int32 y, Int32 width, Int32 height);
        public static CopyTexSubImage2D glCopyTexSubImage2D;


        //m* 54

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 CreateProgram();
        public static CreateProgram glCreateProgram;


        //m* 55

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 CreateShader(System.Int32 type);
        public static CreateShader glCreateShader;


        //m* 56

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CullFace(System.Int32 mode);
        public static CullFace glCullFace;


        //m* 57

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DebugMessageCallback(DebugProc callback, IntPtr userParam);
        public static DebugMessageCallback glDebugMessageCallback;


        //m* 58

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DebugMessageControl(System.Int32 source, System.Int32 type, System.Int32 severity, Int32 count, [CountAttribute(Parameter = "count")] UInt32* ids, bool enabled);
        public static DebugMessageControl glDebugMessageControl;


        //m* 59

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DebugMessageInsert(System.Int32 source, System.Int32 type, UInt32 id, System.Int32 severity, Int32 length, [CountAttribute(Computed = "buf,length")] string buf);
        public static DebugMessageInsert glDebugMessageInsert;


        //m* 60

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteBuffers(Int32 n, [CountAttribute(Parameter = "n")] UInt32* buffers);
        public static DeleteBuffers glDeleteBuffers;


        //m* 61

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteFramebuffers(Int32 n, [CountAttribute(Parameter = "n")] UInt32* framebuffers);
        public static DeleteFramebuffers glDeleteFramebuffers;


        //m* 62

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DeleteProgram(UInt32 program);
        public static DeleteProgram glDeleteProgram;


        //m* 63

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteRenderbuffers(Int32 n, [CountAttribute(Parameter = "n")] UInt32* renderbuffers);
        public static DeleteRenderbuffers glDeleteRenderbuffers;


        //m* 64

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DeleteShader(UInt32 shader);
        public static DeleteShader glDeleteShader;


        //m* 65

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteTextures(Int32 n, [CountAttribute(Parameter = "n")] UInt32* textures);
        public static DeleteTextures glDeleteTextures;


        //m* 66

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DepthFunc(System.Int32 func);
        public static DepthFunc glDepthFunc;


        //m* 67

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DepthMask(bool flag);
        public static DepthMask glDepthMask;


        //m* 68

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DepthRangef(Single n, Single f);
        public static DepthRangef glDepthRangef;


        //m* 69

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DetachShader(UInt32 program, UInt32 shader);
        public static DetachShader glDetachShader;


        //m* 70

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Disable(System.Int32 cap);
        public static Disable glDisable;


        //m* 71

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DisableVertexAttribArray(UInt32 index);
        public static DisableVertexAttribArray glDisableVertexAttribArray;


        //m* 72

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawArrays(System.Int32 mode, Int32 first, Int32 count);
        public static DrawArrays glDrawArrays;


        //m* 73

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElements(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices);
        public static DrawElements glDrawElements;


        //m* 74

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Enable(System.Int32 cap);
        public static Enable glEnable;


        //m* 75

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EnableVertexAttribArray(UInt32 index);
        public static EnableVertexAttribArray glEnableVertexAttribArray;


        //m* 76

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Finish();
        public static Finish glFinish;


        //m* 77

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Flush();
        public static Flush glFlush;


        //m* 78

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferRenderbuffer(System.Int32 target, System.Int32 attachment, System.Int32 renderbuffertarget, UInt32 renderbuffer);
        public static FramebufferRenderbuffer glFramebufferRenderbuffer;


        //m* 79

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTexture2D(System.Int32 target, System.Int32 attachment, System.Int32 textarget, UInt32 texture, Int32 level);
        public static FramebufferTexture2D glFramebufferTexture2D;


        //m* 80

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FrontFace(System.Int32 mode);
        public static FrontFace glFrontFace;


        //m* 81

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenBuffers(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* buffers);
        public static GenBuffers glGenBuffers;


        //m* 82

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void GenerateMipmap(System.Int32 target);
        public static GenerateMipmap glGenerateMipmap;


        //m* 83

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenFramebuffers(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* framebuffers);
        public static GenFramebuffers glGenFramebuffers;


        //m* 84

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenRenderbuffers(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* renderbuffers);
        public static GenRenderbuffers glGenRenderbuffers;


        //m* 85

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenTextures(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* textures);
        public static GenTextures glGenTextures;


        //m* 86

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetActiveAttrib(UInt32 program, UInt32 index, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Count = 1)] Int32* size, [OutAttribute, CountAttribute(Count = 1)] System.Int32* type, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr name);
        public static GetActiveAttrib glGetActiveAttrib;


        //m* 87

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetActiveUniform(UInt32 program, UInt32 index, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Count = 1)] Int32* size, [OutAttribute, CountAttribute(Count = 1)] System.Int32* type, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr name);
        public static GetActiveUniform glGetActiveUniform;


        //m* 88

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetAttachedShaders(UInt32 program, Int32 maxCount, [OutAttribute, CountAttribute(Count = 1)] Int32* count, [OutAttribute, CountAttribute(Parameter = "maxCount")] UInt32* shaders);
        public static GetAttachedShaders glGetAttachedShaders;


        //m* 89

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 GetAttribLocation(UInt32 program, string name);
        public static GetAttribLocation glGetAttribLocation;


        //m* 90

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetBooleanv(System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] bool* data);
        public static GetBooleanv glGetBooleanv;


        //m* 91

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetBufferParameteriv(System.Int32 target, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetBufferParameteriv glGetBufferParameteriv;


        //m* 92

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate Int32 GetDebugMessageLog(UInt32 count, Int32 bufSize, [OutAttribute, CountAttribute(Parameter = "count")] System.Int32* sources, [OutAttribute, CountAttribute(Parameter = "count")] System.Int32* types, [OutAttribute, CountAttribute(Parameter = "count")] UInt32* ids, [OutAttribute, CountAttribute(Parameter = "count")] System.Int32* severities, [OutAttribute, CountAttribute(Parameter = "count")] Int32* lengths, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr messageLog);
        public static GetDebugMessageLog glGetDebugMessageLog;


        //m* 93

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 GetError();
        public static GetError glGetError;


        //m* 94

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetFloatv(System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Single* data);
        public static GetFloatv glGetFloatv;


        //m* 95

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetFramebufferAttachmentParameteriv(System.Int32 target, System.Int32 attachment, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetFramebufferAttachmentParameteriv glGetFramebufferAttachmentParameteriv;


        //m* 96

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 GetGraphicsResetStatus();
        public static GetGraphicsResetStatus glGetGraphicsResetStatus;


        //m* 97

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetIntegerv(System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* data);
        public static GetIntegerv glGetIntegerv;


        //m* 98

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetnUniformfv(UInt32 program, Int32 location, Int32 bufSize, [OutAttribute] Single* @params);
        public static GetnUniformfv glGetnUniformfv;


        //m* 99

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetnUniformiv(UInt32 program, Int32 location, Int32 bufSize, [OutAttribute] Int32* @params);
        public static GetnUniformiv glGetnUniformiv;


        //m* 100

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetnUniformuiv(UInt32 program, Int32 location, Int32 bufSize, [OutAttribute] UInt32* @params);
        public static GetnUniformuiv glGetnUniformuiv;


        //m* 101

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetObjectLabel(System.Int32 identifier, UInt32 name, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr label);
        public static GetObjectLabel glGetObjectLabel;


        //m* 102

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetObjectPtrLabel(IntPtr ptr, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr label);
        public static GetObjectPtrLabel glGetObjectPtrLabel;


        //m* 103

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void GetPointerv(System.Int32 pname, [OutAttribute, CountAttribute(Count = 1)] IntPtr @params);
        public static GetPointerv glGetPointerv;


        //m* 104

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetProgramInfoLog(UInt32 program, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr infoLog);
        public static GetProgramInfoLog glGetProgramInfoLog;


        //m* 105

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetProgramiv(UInt32 program, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetProgramiv glGetProgramiv;


        //m* 106

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetRenderbufferParameteriv(System.Int32 target, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetRenderbufferParameteriv glGetRenderbufferParameteriv;


        //m* 107

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetShaderInfoLog(UInt32 shader, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr infoLog);
        public static GetShaderInfoLog glGetShaderInfoLog;


        //m* 108

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetShaderiv(UInt32 shader, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetShaderiv glGetShaderiv;


        //m* 109

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetShaderPrecisionFormat(System.Int32 shadertype, System.Int32 precisiontype, [OutAttribute, CountAttribute(Count = 2)] Int32* range, [OutAttribute, CountAttribute(Count = 1)] Int32* precision);
        public static GetShaderPrecisionFormat glGetShaderPrecisionFormat;


        //m* 110

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetShaderSource(UInt32 shader, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr source);
        public static GetShaderSource glGetShaderSource;


        //m* 111

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate IntPtr GetString(System.Int32 name);
        public static GetString glGetString;


        //m* 112

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetTexParameterfv(System.Int32 target, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Single* @params);
        public static GetTexParameterfv glGetTexParameterfv;


        //m* 113

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetTexParameteriv(System.Int32 target, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetTexParameteriv glGetTexParameteriv;


        //m* 114

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetUniformfv(UInt32 program, Int32 location, [OutAttribute, CountAttribute(Computed = "program,location")] Single* @params);
        public static GetUniformfv glGetUniformfv;


        //m* 115

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetUniformiv(UInt32 program, Int32 location, [OutAttribute, CountAttribute(Computed = "program,location")] Int32* @params);
        public static GetUniformiv glGetUniformiv;


        //m* 116

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 GetUniformLocation(UInt32 program, string name);
        public static GetUniformLocation glGetUniformLocation;


        //m* 117

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetVertexAttribfv(UInt32 index, System.Int32 pname, [OutAttribute, CountAttribute(Count = 4)] Single* @params);
        public static GetVertexAttribfv glGetVertexAttribfv;


        //m* 118

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetVertexAttribiv(UInt32 index, System.Int32 pname, [OutAttribute, CountAttribute(Count = 4)] Int32* @params);
        public static GetVertexAttribiv glGetVertexAttribiv;


        //m* 119

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void GetVertexAttribPointerv(UInt32 index, System.Int32 pname, [OutAttribute, CountAttribute(Count = 1)] IntPtr pointer);
        public static GetVertexAttribPointerv glGetVertexAttribPointerv;


        //m* 120

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Hint(System.Int32 target, System.Int32 mode);
        public static Hint glHint;


        //m* 121

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsBuffer(UInt32 buffer);
        public static IsBuffer glIsBuffer;


        //m* 122

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsEnabled(System.Int32 cap);
        public static IsEnabled glIsEnabled;


        //m* 123

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsFramebuffer(UInt32 framebuffer);
        public static IsFramebuffer glIsFramebuffer;


        //m* 124

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsProgram(UInt32 program);
        public static IsProgram glIsProgram;


        //m* 125

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsRenderbuffer(UInt32 renderbuffer);
        public static IsRenderbuffer glIsRenderbuffer;


        //m* 126

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsShader(UInt32 shader);
        public static IsShader glIsShader;


        //m* 127

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsTexture(UInt32 texture);
        public static IsTexture glIsTexture;


        //m* 128

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void LineWidth(Single width);
        public static LineWidth glLineWidth;


        //m* 129

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void LinkProgram(UInt32 program);
        public static LinkProgram glLinkProgram;


        //m* 130

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ObjectLabel(System.Int32 identifier, UInt32 name, Int32 length, [CountAttribute(Computed = "label,length")] string label);
        public static ObjectLabel glObjectLabel;


        //m* 131

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ObjectPtrLabel(IntPtr ptr, Int32 length, [CountAttribute(Computed = "label,length")] string label);
        public static ObjectPtrLabel glObjectPtrLabel;


        //m* 132

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PixelStorei(System.Int32 pname, Int32 param);
        public static PixelStorei glPixelStorei;


        //m* 133

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PolygonOffset(Single factor, Single units);
        public static PolygonOffset glPolygonOffset;


        //m* 134

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PopDebugGroup();
        public static PopDebugGroup glPopDebugGroup;


        //m* 135

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PushDebugGroup(System.Int32 source, UInt32 id, Int32 length, [CountAttribute(Computed = "message,length")] string message);
        public static PushDebugGroup glPushDebugGroup;


        //m* 136

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ReadnPixels(Int32 x, Int32 y, Int32 width, Int32 height, System.Int32 format, System.Int32 type, Int32 bufSize, [OutAttribute] IntPtr data);
        public static ReadnPixels glReadnPixels;


        //m* 137

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ReadPixels(Int32 x, Int32 y, Int32 width, Int32 height, System.Int32 format, System.Int32 type, [OutAttribute, CountAttribute(Computed = "format,type,width,height")] IntPtr pixels);
        public static ReadPixels glReadPixels;


        //m* 138

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ReleaseShaderCompiler();
        public static ReleaseShaderCompiler glReleaseShaderCompiler;


        //m* 139

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void RenderbufferStorage(System.Int32 target, System.Int32 internalformat, Int32 width, Int32 height);
        public static RenderbufferStorage glRenderbufferStorage;


        //m* 140

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void SampleCoverage(Single value, bool invert);
        public static SampleCoverage glSampleCoverage;


        //m* 141

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Scissor(Int32 x, Int32 y, Int32 width, Int32 height);
        public static Scissor glScissor;


        //m* 142

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ShaderBinary(Int32 count, [CountAttribute(Parameter = "count")] UInt32* shaders, System.Int32 binaryformat, [CountAttribute(Parameter = "length")] IntPtr binary, Int32 length);
        public static ShaderBinary glShaderBinary;


        //m* 143

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ShaderSource(UInt32 shader, Int32 count, [CountAttribute(Parameter = "count")] string[] @string, [CountAttribute(Parameter = "count")] Int32* length);
        public static ShaderSource glShaderSource;


        //m* 144

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilFunc(System.Int32 func, Int32 @ref, UInt32 mask);
        public static StencilFunc glStencilFunc;


        //m* 145

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilFuncSeparate(System.Int32 face, System.Int32 func, Int32 @ref, UInt32 mask);
        public static StencilFuncSeparate glStencilFuncSeparate;


        //m* 146

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilMask(UInt32 mask);
        public static StencilMask glStencilMask;


        //m* 147

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilMaskSeparate(System.Int32 face, UInt32 mask);
        public static StencilMaskSeparate glStencilMaskSeparate;


        //m* 148

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilOp(System.Int32 fail, System.Int32 zfail, System.Int32 zpass);
        public static StencilOp glStencilOp;


        //m* 149

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilOpSeparate(System.Int32 face, System.Int32 sfail, System.Int32 dpfail, System.Int32 dppass);
        public static StencilOpSeparate glStencilOpSeparate;


        //m* 150

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexImage2D(System.Int32 target, Int32 level, System.Int32 internalformat, Int32 width, Int32 height, Int32 border, System.Int32 format, System.Int32 type, [CountAttribute(Computed = "format,type,width,height")] IntPtr pixels);
        public static TexImage2D glTexImage2D;


        //m* 151

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexParameterf(System.Int32 target, System.Int32 pname, Single param);
        public static TexParameterf glTexParameterf;


        //m* 152

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void TexParameterfv(System.Int32 target, System.Int32 pname, [CountAttribute(Computed = "pname")] Single* @params);
        public static TexParameterfv glTexParameterfv;


        //m* 153

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexParameteri(System.Int32 target, System.Int32 pname, Int32 param);
        public static TexParameteri glTexParameteri;


        //m* 154

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void TexParameteriv(System.Int32 target, System.Int32 pname, [CountAttribute(Computed = "pname")] Int32* @params);
        public static TexParameteriv glTexParameteriv;


        //m* 155

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexSubImage2D(System.Int32 target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 width, Int32 height, System.Int32 format, System.Int32 type, [CountAttribute(Computed = "format,type,width,height")] IntPtr pixels);
        public static TexSubImage2D glTexSubImage2D;


        //m* 156

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform1f(Int32 location, Single v0);
        public static Uniform1f glUniform1f;


        //m* 157

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform1fv(Int32 location, Int32 count, [CountAttribute(Parameter = "count*1")] Single* value);
        public static Uniform1fv glUniform1fv;


        //m* 158

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform1i(Int32 location, Int32 v0);
        public static Uniform1i glUniform1i;


        //m* 159

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform1iv(Int32 location, Int32 count, [CountAttribute(Parameter = "count*1")] Int32* value);
        public static Uniform1iv glUniform1iv;


        //m* 160

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform2f(Int32 location, Single v0, Single v1);
        public static Uniform2f glUniform2f;


        //m* 161

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform2fv(Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] Single* value);
        public static Uniform2fv glUniform2fv;


        //m* 162

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform2i(Int32 location, Int32 v0, Int32 v1);
        public static Uniform2i glUniform2i;


        //m* 163

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform2iv(Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] Int32* value);
        public static Uniform2iv glUniform2iv;


        //m* 164

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform3f(Int32 location, Single v0, Single v1, Single v2);
        public static Uniform3f glUniform3f;


        //m* 165

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform3fv(Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] Single* value);
        public static Uniform3fv glUniform3fv;


        //m* 166

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform3i(Int32 location, Int32 v0, Int32 v1, Int32 v2);
        public static Uniform3i glUniform3i;


        //m* 167

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform3iv(Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] Int32* value);
        public static Uniform3iv glUniform3iv;


        //m* 168

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform4f(Int32 location, Single v0, Single v1, Single v2, Single v3);
        public static Uniform4f glUniform4f;


        //m* 169

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform4fv(Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] Single* value);
        public static Uniform4fv glUniform4fv;


        //m* 170

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform4i(Int32 location, Int32 v0, Int32 v1, Int32 v2, Int32 v3);
        public static Uniform4i glUniform4i;


        //m* 171

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform4iv(Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] Int32* value);
        public static Uniform4iv glUniform4iv;


        //m* 172

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix2fv(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*4")] Single* value);
        public static UniformMatrix2fv glUniformMatrix2fv;


        //m* 173

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix3fv(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*9")] Single* value);
        public static UniformMatrix3fv glUniformMatrix3fv;


        //m* 174

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix4fv(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*16")] Single* value);
        public static UniformMatrix4fv glUniformMatrix4fv;


        //m* 175

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void UseProgram(UInt32 program);
        public static UseProgram glUseProgram;


        //m* 176

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ValidateProgram(UInt32 program);
        public static ValidateProgram glValidateProgram;


        //m* 177

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void VertexAttrib1f(UInt32 index, Single x);
        public static VertexAttrib1f glVertexAttrib1f;


        //m* 178

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void VertexAttrib1fv(UInt32 index, [CountAttribute(Count = 1)] Single* v);
        public static VertexAttrib1fv glVertexAttrib1fv;


        //m* 179

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void VertexAttrib2f(UInt32 index, Single x, Single y);
        public static VertexAttrib2f glVertexAttrib2f;


        //m* 180

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void VertexAttrib2fv(UInt32 index, [CountAttribute(Count = 2)] Single* v);
        public static VertexAttrib2fv glVertexAttrib2fv;


        //m* 181

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void VertexAttrib3f(UInt32 index, Single x, Single y, Single z);
        public static VertexAttrib3f glVertexAttrib3f;


        //m* 182

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void VertexAttrib3fv(UInt32 index, [CountAttribute(Count = 3)] Single* v);
        public static VertexAttrib3fv glVertexAttrib3fv;


        //m* 183

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void VertexAttrib4f(UInt32 index, Single x, Single y, Single z, Single w);
        public static VertexAttrib4f glVertexAttrib4f;


        //m* 184

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void VertexAttrib4fv(UInt32 index, [CountAttribute(Count = 4)] Single* v);
        public static VertexAttrib4fv glVertexAttrib4fv;


        //m* 185

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void VertexAttribPointer(UInt32 index, Int32 size, System.Int32 type, bool normalized, Int32 stride, [CountAttribute(Computed = "size,type,stride")] IntPtr pointer);
        public static VertexAttribPointer glVertexAttribPointer;


        //m* 186

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Viewport(Int32 x, Int32 y, Int32 width, Int32 height);
        public static Viewport glViewport;


        //m* 187

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte AcquireKeyedMutexWin32EXT(UInt32 memory, UInt64 key, UInt32 timeout);
        public static AcquireKeyedMutexWin32EXT glAcquireKeyedMutexWin32EXT;


        //m* 188

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ActiveProgramEXT(UInt32 program);
        public static ActiveProgramEXT glActiveProgramEXT;


        //m* 189

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ActiveShaderProgramEXT(UInt32 pipeline, UInt32 program);
        public static ActiveShaderProgramEXT glActiveShaderProgramEXT;


        //m* 190

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BeginQueryEXT(System.Int32 target, UInt32 id);
        public static BeginQueryEXT glBeginQueryEXT;


        //m* 191

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindFragDataLocationEXT(UInt32 program, UInt32 color, [CountAttribute(Computed = "name")] string name);
        public static BindFragDataLocationEXT glBindFragDataLocationEXT;


        //m* 192

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindFragDataLocationIndexedEXT(UInt32 program, UInt32 colorNumber, UInt32 index, string name);
        public static BindFragDataLocationIndexedEXT glBindFragDataLocationIndexedEXT;


        //m* 193

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindProgramPipelineEXT(UInt32 pipeline);
        public static BindProgramPipelineEXT glBindProgramPipelineEXT;


        //m* 194

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendEquationEXT(System.Int32 mode);
        public static BlendEquationEXT glBlendEquationEXT;


        //m* 195

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendEquationiEXT(UInt32 buf, System.Int32 mode);
        public static BlendEquationiEXT glBlendEquationiEXT;


        //m* 196

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendEquationSeparateiEXT(UInt32 buf, System.Int32 modeRGB, System.Int32 modeAlpha);
        public static BlendEquationSeparateiEXT glBlendEquationSeparateiEXT;


        //m* 197

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendFunciEXT(UInt32 buf, System.Int32 src, System.Int32 dst);
        public static BlendFunciEXT glBlendFunciEXT;


        //m* 198

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendFuncSeparateiEXT(UInt32 buf, System.Int32 srcRGB, System.Int32 dstRGB, System.Int32 srcAlpha, System.Int32 dstAlpha);
        public static BlendFuncSeparateiEXT glBlendFuncSeparateiEXT;


        //m* 199

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BufferStorageEXT(System.Int32 target, IntPtr size, [CountAttribute(Parameter = "size")] IntPtr data, System.Int32 flags);
        public static BufferStorageEXT glBufferStorageEXT;


        //m* 200

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BufferStorageExternalEXT(System.Int32 target, IntPtr offset, IntPtr size, IntPtr clientBuffer, System.Int32 flags);
        public static BufferStorageExternalEXT glBufferStorageExternalEXT;


        //m* 201

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BufferStorageMemEXT(System.Int32 target, IntPtr size, UInt32 memory, UInt64 offset);
        public static BufferStorageMemEXT glBufferStorageMemEXT;


        //m* 202

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ClearPixelLocalStorageuiEXT(Int32 offset, Int32 n, [CountAttribute(Parameter = "n")] UInt32* values);
        public static ClearPixelLocalStorageuiEXT glClearPixelLocalStorageuiEXT;


        //m* 203

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ClearTexImageEXT(UInt32 texture, Int32 level, System.Int32 format, System.Int32 type, [CountAttribute(Computed = "format,type")] IntPtr data);
        public static ClearTexImageEXT glClearTexImageEXT;


        //m* 204

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ClearTexSubImageEXT(UInt32 texture, Int32 level, Int32 xoffset, Int32 yoffset, Int32 zoffset, Int32 width, Int32 height, Int32 depth, System.Int32 format, System.Int32 type, [CountAttribute(Computed = "format,type")] IntPtr data);
        public static ClearTexSubImageEXT glClearTexSubImageEXT;


        //m* 205

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ClipControlEXT(System.Int32 origin, System.Int32 depth);
        public static ClipControlEXT glClipControlEXT;


        //m* 206

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ColorMaskiEXT(UInt32 index, bool r, bool g, bool b, bool a);
        public static ColorMaskiEXT glColorMaskiEXT;


        //m* 207

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CopyImageSubDataEXT(UInt32 srcName, System.Int32 srcTarget, Int32 srcLevel, Int32 srcX, Int32 srcY, Int32 srcZ, UInt32 dstName, System.Int32 dstTarget, Int32 dstLevel, Int32 dstX, Int32 dstY, Int32 dstZ, Int32 srcWidth, Int32 srcHeight, Int32 srcDepth);
        public static CopyImageSubDataEXT glCopyImageSubDataEXT;


        //m* 208

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void CreateMemoryObjectsEXT(Int32 n, [OutAttribute] UInt32* memoryObjects);
        public static CreateMemoryObjectsEXT glCreateMemoryObjectsEXT;


        //m* 209

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 CreateShaderProgramEXT(System.Int32 type, string @string);
        public static CreateShaderProgramEXT glCreateShaderProgramEXT;


        //m* 210

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 CreateShaderProgramvEXT(System.Int32 type, Int32 count, [CountAttribute(Parameter = "count")] string[] strings);
        public static CreateShaderProgramvEXT glCreateShaderProgramvEXT;


        //m* 211

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteMemoryObjectsEXT(Int32 n, [CountAttribute(Parameter = "n")] UInt32* memoryObjects);
        public static DeleteMemoryObjectsEXT glDeleteMemoryObjectsEXT;


        //m* 212

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteProgramPipelinesEXT(Int32 n, [CountAttribute(Parameter = "n")] UInt32* pipelines);
        public static DeleteProgramPipelinesEXT glDeleteProgramPipelinesEXT;


        //m* 213

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteQueriesEXT(Int32 n, [CountAttribute(Parameter = "n")] UInt32* ids);
        public static DeleteQueriesEXT glDeleteQueriesEXT;


        //m* 214

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteSemaphoresEXT(Int32 n, [CountAttribute(Parameter = "n")] UInt32* semaphores);
        public static DeleteSemaphoresEXT glDeleteSemaphoresEXT;


        //m* 215

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DisableiEXT(System.Int32 target, UInt32 index);
        public static DisableiEXT glDisableiEXT;


        //m* 216

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DiscardFramebufferEXT(System.Int32 target, Int32 numAttachments, [CountAttribute(Parameter = "numAttachments")] System.Int32* attachments);
        public static DiscardFramebufferEXT glDiscardFramebufferEXT;


        //m* 217

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawArraysInstancedBaseInstanceEXT(System.Int32 mode, Int32 first, Int32 count, Int32 instancecount, UInt32 baseinstance);
        public static DrawArraysInstancedBaseInstanceEXT glDrawArraysInstancedBaseInstanceEXT;


        //m* 218

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawArraysInstancedEXT(System.Int32 mode, Int32 start, Int32 count, Int32 primcount);
        public static DrawArraysInstancedEXT glDrawArraysInstancedEXT;


        //m* 219

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DrawBuffersEXT(Int32 n, [CountAttribute(Parameter = "n")] System.Int32* bufs);
        public static DrawBuffersEXT glDrawBuffersEXT;


        //m* 220

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DrawBuffersIndexedEXT(Int32 n, [CountAttribute(Parameter = "n")] System.Int32* location, [CountAttribute(Parameter = "n")] Int32* indices);
        public static DrawBuffersIndexedEXT glDrawBuffersIndexedEXT;


        //m* 221

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsBaseVertexEXT(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 basevertex);
        public static DrawElementsBaseVertexEXT glDrawElementsBaseVertexEXT;


        //m* 222

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsInstancedBaseInstanceEXT(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Parameter = "count")] IntPtr indices, Int32 instancecount, UInt32 baseinstance);
        public static DrawElementsInstancedBaseInstanceEXT glDrawElementsInstancedBaseInstanceEXT;


        //m* 223

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsInstancedBaseVertexBaseInstanceEXT(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Parameter = "count")] IntPtr indices, Int32 instancecount, Int32 basevertex, UInt32 baseinstance);
        public static DrawElementsInstancedBaseVertexBaseInstanceEXT glDrawElementsInstancedBaseVertexBaseInstanceEXT;


        //m* 224

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsInstancedBaseVertexEXT(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 instancecount, Int32 basevertex);
        public static DrawElementsInstancedBaseVertexEXT glDrawElementsInstancedBaseVertexEXT;


        //m* 225

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsInstancedEXT(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 primcount);
        public static DrawElementsInstancedEXT glDrawElementsInstancedEXT;


        //m* 226

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawRangeElementsBaseVertexEXT(System.Int32 mode, UInt32 start, UInt32 end, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 basevertex);
        public static DrawRangeElementsBaseVertexEXT glDrawRangeElementsBaseVertexEXT;


        //m* 227

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawTransformFeedbackEXT(System.Int32 mode, UInt32 id);
        public static DrawTransformFeedbackEXT glDrawTransformFeedbackEXT;


        //m* 228

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawTransformFeedbackInstancedEXT(System.Int32 mode, UInt32 id, Int32 instancecount);
        public static DrawTransformFeedbackInstancedEXT glDrawTransformFeedbackInstancedEXT;


        //m* 229

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EnableiEXT(System.Int32 target, UInt32 index);
        public static EnableiEXT glEnableiEXT;


        //m* 230

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EndQueryEXT(System.Int32 target);
        public static EndQueryEXT glEndQueryEXT;


        //m* 231

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FlushMappedBufferRangeEXT(System.Int32 target, IntPtr offset, IntPtr length);
        public static FlushMappedBufferRangeEXT glFlushMappedBufferRangeEXT;


        //m* 232

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferPixelLocalStorageSizeEXT(UInt32 target, Int32 size);
        public static FramebufferPixelLocalStorageSizeEXT glFramebufferPixelLocalStorageSizeEXT;


        //m* 233

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTexture2DMultisampleEXT(System.Int32 target, System.Int32 attachment, System.Int32 textarget, UInt32 texture, Int32 level, Int32 samples);
        public static FramebufferTexture2DMultisampleEXT glFramebufferTexture2DMultisampleEXT;


        //m* 234

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTextureEXT(System.Int32 target, System.Int32 attachment, UInt32 texture, Int32 level);
        public static FramebufferTextureEXT glFramebufferTextureEXT;


        //m* 235

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenProgramPipelinesEXT(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* pipelines);
        public static GenProgramPipelinesEXT glGenProgramPipelinesEXT;


        //m* 236

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenQueriesEXT(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* ids);
        public static GenQueriesEXT glGenQueriesEXT;


        //m* 237

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenSemaphoresEXT(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* semaphores);
        public static GenSemaphoresEXT glGenSemaphoresEXT;


        //m* 238

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 GetFragDataIndexEXT(UInt32 program, string name);
        public static GetFragDataIndexEXT glGetFragDataIndexEXT;


        //m* 239

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 GetFramebufferPixelLocalStorageSizeEXT(System.Int32 target);
        public static GetFramebufferPixelLocalStorageSizeEXT glGetFramebufferPixelLocalStorageSizeEXT;


        //m* 240

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 GetGraphicsResetStatusEXT();
        public static GetGraphicsResetStatusEXT glGetGraphicsResetStatusEXT;


        //m* 241

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetIntegeri_vEXT(System.Int32 target, UInt32 index, [OutAttribute] Int32* data);
        public static GetIntegeri_vEXT glGetIntegeri_vEXT;


        //m* 242

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetMemoryObjectParameterivEXT(UInt32 memoryObject, System.Int32 pname, [OutAttribute] Int32* @params);
        public static GetMemoryObjectParameterivEXT glGetMemoryObjectParameterivEXT;


        //m* 243

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetnUniformfvEXT(UInt32 program, Int32 location, Int32 bufSize, [OutAttribute, CountAttribute(Parameter = "bufSize")] Single* @params);
        public static GetnUniformfvEXT glGetnUniformfvEXT;


        //m* 244

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetnUniformivEXT(UInt32 program, Int32 location, Int32 bufSize, [OutAttribute, CountAttribute(Parameter = "bufSize")] Int32* @params);
        public static GetnUniformivEXT glGetnUniformivEXT;


        //m* 245

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetObjectLabelEXT(System.Int32 type, UInt32 @object, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr label);
        public static GetObjectLabelEXT glGetObjectLabelEXT;


        //m* 246

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetProgramPipelineInfoLogEXT(UInt32 pipeline, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr infoLog);
        public static GetProgramPipelineInfoLogEXT glGetProgramPipelineInfoLogEXT;


        //m* 247

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetProgramPipelineivEXT(UInt32 pipeline, System.Int32 pname, [OutAttribute] Int32* @params);
        public static GetProgramPipelineivEXT glGetProgramPipelineivEXT;


        //m* 248

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 GetProgramResourceLocationIndexEXT(UInt32 program, System.Int32 programInterface, [CountAttribute(Computed = "name")] string name);
        public static GetProgramResourceLocationIndexEXT glGetProgramResourceLocationIndexEXT;


        //m* 249

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetQueryivEXT(System.Int32 target, System.Int32 pname, [OutAttribute] Int32* @params);
        public static GetQueryivEXT glGetQueryivEXT;


        //m* 250

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetQueryObjecti64vEXT(UInt32 id, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int64* @params);
        public static GetQueryObjecti64vEXT glGetQueryObjecti64vEXT;


        //m* 251

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetQueryObjectivEXT(UInt32 id, System.Int32 pname, [OutAttribute] Int32* @params);
        public static GetQueryObjectivEXT glGetQueryObjectivEXT;


        //m* 252

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetQueryObjectui64vEXT(UInt32 id, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] UInt64* @params);
        public static GetQueryObjectui64vEXT glGetQueryObjectui64vEXT;


        //m* 253

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetQueryObjectuivEXT(UInt32 id, System.Int32 pname, [OutAttribute] UInt32* @params);
        public static GetQueryObjectuivEXT glGetQueryObjectuivEXT;


        //m* 254

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetSamplerParameterIivEXT(UInt32 sampler, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetSamplerParameterIivEXT glGetSamplerParameterIivEXT;


        //m* 255

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetSamplerParameterIuivEXT(UInt32 sampler, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] UInt32* @params);
        public static GetSamplerParameterIuivEXT glGetSamplerParameterIuivEXT;


        //m* 256

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetSemaphoreParameterui64vEXT(UInt32 semaphore, System.Int32 pname, [OutAttribute] UInt64* @params);
        public static GetSemaphoreParameterui64vEXT glGetSemaphoreParameterui64vEXT;


        //m* 257

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetTexParameterIivEXT(System.Int32 target, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetTexParameterIivEXT glGetTexParameterIivEXT;


        //m* 258

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetTexParameterIuivEXT(System.Int32 target, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] UInt32* @params);
        public static GetTexParameterIuivEXT glGetTexParameterIuivEXT;


        //m* 259

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetUnsignedBytei_vEXT(System.Int32 target, UInt32 index, [OutAttribute, CountAttribute(Computed = "target")] Byte* data);
        public static GetUnsignedBytei_vEXT glGetUnsignedBytei_vEXT;


        //m* 260

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetUnsignedBytevEXT(System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Byte* data);
        public static GetUnsignedBytevEXT glGetUnsignedBytevEXT;


        //m* 261

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ImportMemoryFdEXT(UInt32 memory, UInt64 size, System.Int32 handleType, Int32 fd);
        public static ImportMemoryFdEXT glImportMemoryFdEXT;


        //m* 262

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ImportMemoryWin32HandleEXT(UInt32 memory, UInt64 size, System.Int32 handleType, [OutAttribute] IntPtr handle);
        public static ImportMemoryWin32HandleEXT glImportMemoryWin32HandleEXT;


        //m* 263

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ImportMemoryWin32NameEXT(UInt32 memory, UInt64 size, System.Int32 handleType, IntPtr name);
        public static ImportMemoryWin32NameEXT glImportMemoryWin32NameEXT;


        //m* 264

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ImportSemaphoreFdEXT(UInt32 semaphore, System.Int32 handleType, Int32 fd);
        public static ImportSemaphoreFdEXT glImportSemaphoreFdEXT;


        //m* 265

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ImportSemaphoreWin32HandleEXT(UInt32 semaphore, System.Int32 handleType, [OutAttribute] IntPtr handle);
        public static ImportSemaphoreWin32HandleEXT glImportSemaphoreWin32HandleEXT;


        //m* 266

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ImportSemaphoreWin32NameEXT(UInt32 semaphore, System.Int32 handleType, IntPtr name);
        public static ImportSemaphoreWin32NameEXT glImportSemaphoreWin32NameEXT;


        //m* 267

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void InsertEventMarkerEXT(Int32 length, string marker);
        public static InsertEventMarkerEXT glInsertEventMarkerEXT;


        //m* 268

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsEnablediEXT(System.Int32 target, UInt32 index);
        public static IsEnablediEXT glIsEnablediEXT;


        //m* 269

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsMemoryObjectEXT(UInt32 memoryObject);
        public static IsMemoryObjectEXT glIsMemoryObjectEXT;


        //m* 270

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsProgramPipelineEXT(UInt32 pipeline);
        public static IsProgramPipelineEXT glIsProgramPipelineEXT;


        //m* 271

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsQueryEXT(UInt32 id);
        public static IsQueryEXT glIsQueryEXT;


        //m* 272

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsSemaphoreEXT(UInt32 semaphore);
        public static IsSemaphoreEXT glIsSemaphoreEXT;


        //m* 273

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void LabelObjectEXT(System.Int32 type, UInt32 @object, Int32 length, string label);
        public static LabelObjectEXT glLabelObjectEXT;


        //m* 274

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate IntPtr MapBufferRangeEXT(System.Int32 target, IntPtr offset, IntPtr length, System.Int32 access);
        public static MapBufferRangeEXT glMapBufferRangeEXT;


        //m* 275

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixFrustumEXT(System.Int32 mode, Double left, Double right, Double bottom, Double top, Double zNear, Double zFar);
        public static MatrixFrustumEXT glMatrixFrustumEXT;


        //m* 276

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixLoaddEXT(System.Int32 mode, [CountAttribute(Count = 16)] Double* m);
        public static MatrixLoaddEXT glMatrixLoaddEXT;


        //m* 277

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixLoadfEXT(System.Int32 mode, [CountAttribute(Count = 16)] Single* m);
        public static MatrixLoadfEXT glMatrixLoadfEXT;


        //m* 278

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixLoadIdentityEXT(System.Int32 mode);
        public static MatrixLoadIdentityEXT glMatrixLoadIdentityEXT;


        //m* 279

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixLoadTransposedEXT(System.Int32 mode, [CountAttribute(Count = 16)] Double* m);
        public static MatrixLoadTransposedEXT glMatrixLoadTransposedEXT;


        //m* 280

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixLoadTransposefEXT(System.Int32 mode, [CountAttribute(Count = 16)] Single* m);
        public static MatrixLoadTransposefEXT glMatrixLoadTransposefEXT;


        //m* 281

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixMultdEXT(System.Int32 mode, [CountAttribute(Count = 16)] Double* m);
        public static MatrixMultdEXT glMatrixMultdEXT;


        //m* 282

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixMultfEXT(System.Int32 mode, [CountAttribute(Count = 16)] Single* m);
        public static MatrixMultfEXT glMatrixMultfEXT;


        //m* 283

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixMultTransposedEXT(System.Int32 mode, [CountAttribute(Count = 16)] Double* m);
        public static MatrixMultTransposedEXT glMatrixMultTransposedEXT;


        //m* 284

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixMultTransposefEXT(System.Int32 mode, [CountAttribute(Count = 16)] Single* m);
        public static MatrixMultTransposefEXT glMatrixMultTransposefEXT;


        //m* 285

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixOrthoEXT(System.Int32 mode, Double left, Double right, Double bottom, Double top, Double zNear, Double zFar);
        public static MatrixOrthoEXT glMatrixOrthoEXT;


        //m* 286

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixPopEXT(System.Int32 mode);
        public static MatrixPopEXT glMatrixPopEXT;


        //m* 287

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixPushEXT(System.Int32 mode);
        public static MatrixPushEXT glMatrixPushEXT;


        //m* 288

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixRotatedEXT(System.Int32 mode, Double angle, Double x, Double y, Double z);
        public static MatrixRotatedEXT glMatrixRotatedEXT;


        //m* 289

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixRotatefEXT(System.Int32 mode, Single angle, Single x, Single y, Single z);
        public static MatrixRotatefEXT glMatrixRotatefEXT;


        //m* 290

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixScaledEXT(System.Int32 mode, Double x, Double y, Double z);
        public static MatrixScaledEXT glMatrixScaledEXT;


        //m* 291

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixScalefEXT(System.Int32 mode, Single x, Single y, Single z);
        public static MatrixScalefEXT glMatrixScalefEXT;


        //m* 292

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixTranslatedEXT(System.Int32 mode, Double x, Double y, Double z);
        public static MatrixTranslatedEXT glMatrixTranslatedEXT;


        //m* 293

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MatrixTranslatefEXT(System.Int32 mode, Single x, Single y, Single z);
        public static MatrixTranslatefEXT glMatrixTranslatefEXT;


        //m* 294

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MemoryObjectParameterivEXT(UInt32 memoryObject, System.Int32 pname, Int32* @params);
        public static MemoryObjectParameterivEXT glMemoryObjectParameterivEXT;


        //m* 295

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MultiDrawArraysEXT(System.Int32 mode, [CountAttribute(Computed = "primcount")] Int32* first, [CountAttribute(Computed = "primcount")] Int32* count, Int32 primcount);
        public static MultiDrawArraysEXT glMultiDrawArraysEXT;


        //m* 296

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MultiDrawArraysIndirectEXT(System.Int32 mode, [CountAttribute(Computed = "drawcount,stride")] IntPtr indirect, Int32 drawcount, Int32 stride);
        public static MultiDrawArraysIndirectEXT glMultiDrawArraysIndirectEXT;


        //m* 297

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MultiDrawElementsBaseVertexEXT(System.Int32 mode, [CountAttribute(Computed = "drawcount")] Int32* count, System.Int32 type, [CountAttribute(Computed = "drawcount")] IntPtr indices, Int32 primcount, [CountAttribute(Computed = "drawcount")] Int32* basevertex);
        public static MultiDrawElementsBaseVertexEXT glMultiDrawElementsBaseVertexEXT;


        //m* 298

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MultiDrawElementsEXT(System.Int32 mode, [CountAttribute(Computed = "primcount")] Int32* count, System.Int32 type, [CountAttribute(Computed = "primcount")] IntPtr indices, Int32 primcount);
        public static MultiDrawElementsEXT glMultiDrawElementsEXT;


        //m* 299

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MultiDrawElementsIndirectEXT(System.Int32 mode, System.Int32 type, [CountAttribute(Computed = "drawcount,stride")] IntPtr indirect, Int32 drawcount, Int32 stride);
        public static MultiDrawElementsIndirectEXT glMultiDrawElementsIndirectEXT;


        //m* 300

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void NamedBufferStorageExternalEXT(UInt32 buffer, IntPtr offset, IntPtr size, IntPtr clientBuffer, System.Int32 flags);
        public static NamedBufferStorageExternalEXT glNamedBufferStorageExternalEXT;


        //m* 301

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void NamedBufferStorageMemEXT(UInt32 buffer, IntPtr size, UInt32 memory, UInt64 offset);
        public static NamedBufferStorageMemEXT glNamedBufferStorageMemEXT;


        //m* 302

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PatchParameteriEXT(System.Int32 pname, Int32 value);
        public static PatchParameteriEXT glPatchParameteriEXT;


        //m* 303

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PolygonOffsetClampEXT(Single factor, Single units, Single clamp);
        public static PolygonOffsetClampEXT glPolygonOffsetClampEXT;


        //m* 304

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PopGroupMarkerEXT();
        public static PopGroupMarkerEXT glPopGroupMarkerEXT;


        //m* 305

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PrimitiveBoundingBoxEXT(Single minX, Single minY, Single minZ, Single minW, Single maxX, Single maxY, Single maxZ, Single maxW);
        public static PrimitiveBoundingBoxEXT glPrimitiveBoundingBoxEXT;


        //m* 306

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramParameteriEXT(UInt32 program, System.Int32 pname, Int32 value);
        public static ProgramParameteriEXT glProgramParameteriEXT;


        //m* 307

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform1fEXT(UInt32 program, Int32 location, Single v0);
        public static ProgramUniform1fEXT glProgramUniform1fEXT;


        //m* 308

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform1fvEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count")] Single* value);
        public static ProgramUniform1fvEXT glProgramUniform1fvEXT;


        //m* 309

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform1iEXT(UInt32 program, Int32 location, Int32 v0);
        public static ProgramUniform1iEXT glProgramUniform1iEXT;


        //m* 310

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform1ivEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count")] Int32* value);
        public static ProgramUniform1ivEXT glProgramUniform1ivEXT;


        //m* 311

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform1uiEXT(UInt32 program, Int32 location, UInt32 v0);
        public static ProgramUniform1uiEXT glProgramUniform1uiEXT;


        //m* 312

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform1uivEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count")] UInt32* value);
        public static ProgramUniform1uivEXT glProgramUniform1uivEXT;


        //m* 313

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform2fEXT(UInt32 program, Int32 location, Single v0, Single v1);
        public static ProgramUniform2fEXT glProgramUniform2fEXT;


        //m* 314

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform2fvEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] Single* value);
        public static ProgramUniform2fvEXT glProgramUniform2fvEXT;


        //m* 315

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform2iEXT(UInt32 program, Int32 location, Int32 v0, Int32 v1);
        public static ProgramUniform2iEXT glProgramUniform2iEXT;


        //m* 316

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform2ivEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] Int32* value);
        public static ProgramUniform2ivEXT glProgramUniform2ivEXT;


        //m* 317

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform2uiEXT(UInt32 program, Int32 location, UInt32 v0, UInt32 v1);
        public static ProgramUniform2uiEXT glProgramUniform2uiEXT;


        //m* 318

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform2uivEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] UInt32* value);
        public static ProgramUniform2uivEXT glProgramUniform2uivEXT;


        //m* 319

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform3fEXT(UInt32 program, Int32 location, Single v0, Single v1, Single v2);
        public static ProgramUniform3fEXT glProgramUniform3fEXT;


        //m* 320

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform3fvEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] Single* value);
        public static ProgramUniform3fvEXT glProgramUniform3fvEXT;


        //m* 321

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform3iEXT(UInt32 program, Int32 location, Int32 v0, Int32 v1, Int32 v2);
        public static ProgramUniform3iEXT glProgramUniform3iEXT;


        //m* 322

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform3ivEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] Int32* value);
        public static ProgramUniform3ivEXT glProgramUniform3ivEXT;


        //m* 323

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform3uiEXT(UInt32 program, Int32 location, UInt32 v0, UInt32 v1, UInt32 v2);
        public static ProgramUniform3uiEXT glProgramUniform3uiEXT;


        //m* 324

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform3uivEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] UInt32* value);
        public static ProgramUniform3uivEXT glProgramUniform3uivEXT;


        //m* 325

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform4fEXT(UInt32 program, Int32 location, Single v0, Single v1, Single v2, Single v3);
        public static ProgramUniform4fEXT glProgramUniform4fEXT;


        //m* 326

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform4fvEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] Single* value);
        public static ProgramUniform4fvEXT glProgramUniform4fvEXT;


        //m* 327

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform4iEXT(UInt32 program, Int32 location, Int32 v0, Int32 v1, Int32 v2, Int32 v3);
        public static ProgramUniform4iEXT glProgramUniform4iEXT;


        //m* 328

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform4ivEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] Int32* value);
        public static ProgramUniform4ivEXT glProgramUniform4ivEXT;


        //m* 329

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform4uiEXT(UInt32 program, Int32 location, UInt32 v0, UInt32 v1, UInt32 v2, UInt32 v3);
        public static ProgramUniform4uiEXT glProgramUniform4uiEXT;


        //m* 330

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform4uivEXT(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] UInt32* value);
        public static ProgramUniform4uivEXT glProgramUniform4uivEXT;


        //m* 331

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix2fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*4")] Single* value);
        public static ProgramUniformMatrix2fvEXT glProgramUniformMatrix2fvEXT;


        //m* 332

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix2x3fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*6")] Single* value);
        public static ProgramUniformMatrix2x3fvEXT glProgramUniformMatrix2x3fvEXT;


        //m* 333

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix2x4fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*8")] Single* value);
        public static ProgramUniformMatrix2x4fvEXT glProgramUniformMatrix2x4fvEXT;


        //m* 334

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix3fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*9")] Single* value);
        public static ProgramUniformMatrix3fvEXT glProgramUniformMatrix3fvEXT;


        //m* 335

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix3x2fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*6")] Single* value);
        public static ProgramUniformMatrix3x2fvEXT glProgramUniformMatrix3x2fvEXT;


        //m* 336

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix3x4fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*12")] Single* value);
        public static ProgramUniformMatrix3x4fvEXT glProgramUniformMatrix3x4fvEXT;


        //m* 337

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix4fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*16")] Single* value);
        public static ProgramUniformMatrix4fvEXT glProgramUniformMatrix4fvEXT;


        //m* 338

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix4x2fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*8")] Single* value);
        public static ProgramUniformMatrix4x2fvEXT glProgramUniformMatrix4x2fvEXT;


        //m* 339

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformMatrix4x3fvEXT(UInt32 program, Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*12")] Single* value);
        public static ProgramUniformMatrix4x3fvEXT glProgramUniformMatrix4x3fvEXT;


        //m* 340

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PushGroupMarkerEXT(Int32 length, string marker);
        public static PushGroupMarkerEXT glPushGroupMarkerEXT;


        //m* 341

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void QueryCounterEXT(UInt32 id, System.Int32 target);
        public static QueryCounterEXT glQueryCounterEXT;


        //m* 342

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void RasterSamplesEXT(UInt32 samples, bool fixedsamplelocations);
        public static RasterSamplesEXT glRasterSamplesEXT;


        //m* 343

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ReadBufferIndexedEXT(System.Int32 src, Int32 index);
        public static ReadBufferIndexedEXT glReadBufferIndexedEXT;


        //m* 344

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ReadnPixelsEXT(Int32 x, Int32 y, Int32 width, Int32 height, System.Int32 format, System.Int32 type, Int32 bufSize, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr data);
        public static ReadnPixelsEXT glReadnPixelsEXT;


        //m* 345

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte ReleaseKeyedMutexWin32EXT(UInt32 memory, UInt64 key);
        public static ReleaseKeyedMutexWin32EXT glReleaseKeyedMutexWin32EXT;


        //m* 346

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void RenderbufferStorageMultisampleEXT(System.Int32 target, Int32 samples, System.Int32 internalformat, Int32 width, Int32 height);
        public static RenderbufferStorageMultisampleEXT glRenderbufferStorageMultisampleEXT;


        //m* 347

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void SamplerParameterIivEXT(UInt32 sampler, System.Int32 pname, [CountAttribute(Computed = "pname")] Int32* param);
        public static SamplerParameterIivEXT glSamplerParameterIivEXT;


        //m* 348

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void SamplerParameterIuivEXT(UInt32 sampler, System.Int32 pname, [CountAttribute(Computed = "pname")] UInt32* param);
        public static SamplerParameterIuivEXT glSamplerParameterIuivEXT;


        //m* 349

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void SemaphoreParameterui64vEXT(UInt32 semaphore, System.Int32 pname, UInt64* @params);
        public static SemaphoreParameterui64vEXT glSemaphoreParameterui64vEXT;


        //m* 350

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void SignalSemaphoreEXT(UInt32 semaphore, UInt32 numBufferBarriers, [CountAttribute(Computed = "numBufferBarriers")] UInt32* buffers, UInt32 numTextureBarriers, [CountAttribute(Computed = "numTextureBarriers")] UInt32* textures, [CountAttribute(Computed = "numTextureBarriers")] System.Int32* dstLayouts);
        public static SignalSemaphoreEXT glSignalSemaphoreEXT;


        //m* 351

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexBufferEXT(System.Int32 target, System.Int32 internalformat, UInt32 buffer);
        public static TexBufferEXT glTexBufferEXT;


        //m* 352

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexBufferRangeEXT(System.Int32 target, System.Int32 internalformat, UInt32 buffer, IntPtr offset, IntPtr size);
        public static TexBufferRangeEXT glTexBufferRangeEXT;


        //m* 353

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexPageCommitmentEXT(System.Int32 target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 zoffset, Int32 width, Int32 height, Int32 depth, bool commit);
        public static TexPageCommitmentEXT glTexPageCommitmentEXT;


        //m* 354

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void TexParameterIivEXT(System.Int32 target, System.Int32 pname, [CountAttribute(Computed = "pname")] Int32* @params);
        public static TexParameterIivEXT glTexParameterIivEXT;


        //m* 355

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void TexParameterIuivEXT(System.Int32 target, System.Int32 pname, [CountAttribute(Computed = "pname")] UInt32* @params);
        public static TexParameterIuivEXT glTexParameterIuivEXT;


        //m* 356

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorage1DEXT(System.Int32 target, Int32 levels, System.Int32 internalformat, Int32 width);
        public static TexStorage1DEXT glTexStorage1DEXT;


        //m* 357

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorage2DEXT(System.Int32 target, Int32 levels, System.Int32 internalformat, Int32 width, Int32 height);
        public static TexStorage2DEXT glTexStorage2DEXT;


        //m* 358

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorage3DEXT(System.Int32 target, Int32 levels, System.Int32 internalformat, Int32 width, Int32 height, Int32 depth);
        public static TexStorage3DEXT glTexStorage3DEXT;


        //m* 359

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorageMem1DEXT(System.Int32 target, Int32 levels, System.Int32 internalFormat, Int32 width, UInt32 memory, UInt64 offset);
        public static TexStorageMem1DEXT glTexStorageMem1DEXT;


        //m* 360

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorageMem2DEXT(System.Int32 target, Int32 levels, System.Int32 internalFormat, Int32 width, Int32 height, UInt32 memory, UInt64 offset);
        public static TexStorageMem2DEXT glTexStorageMem2DEXT;


        //m* 361

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorageMem2DMultisampleEXT(System.Int32 target, Int32 samples, System.Int32 internalFormat, Int32 width, Int32 height, bool fixedSampleLocations, UInt32 memory, UInt64 offset);
        public static TexStorageMem2DMultisampleEXT glTexStorageMem2DMultisampleEXT;


        //m* 362

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorageMem3DEXT(System.Int32 target, Int32 levels, System.Int32 internalFormat, Int32 width, Int32 height, Int32 depth, UInt32 memory, UInt64 offset);
        public static TexStorageMem3DEXT glTexStorageMem3DEXT;


        //m* 363

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorageMem3DMultisampleEXT(System.Int32 target, Int32 samples, System.Int32 internalFormat, Int32 width, Int32 height, Int32 depth, bool fixedSampleLocations, UInt32 memory, UInt64 offset);
        public static TexStorageMem3DMultisampleEXT glTexStorageMem3DMultisampleEXT;


        //m* 364

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureStorage1DEXT(UInt32 texture, System.Int32 target, Int32 levels, System.Int32 internalformat, Int32 width);
        public static TextureStorage1DEXT glTextureStorage1DEXT;


        //m* 365

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureStorage2DEXT(UInt32 texture, System.Int32 target, Int32 levels, System.Int32 internalformat, Int32 width, Int32 height);
        public static TextureStorage2DEXT glTextureStorage2DEXT;


        //m* 366

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureStorage3DEXT(UInt32 texture, System.Int32 target, Int32 levels, System.Int32 internalformat, Int32 width, Int32 height, Int32 depth);
        public static TextureStorage3DEXT glTextureStorage3DEXT;


        //m* 367

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureStorageMem1DEXT(UInt32 texture, Int32 levels, System.Int32 internalFormat, Int32 width, UInt32 memory, UInt64 offset);
        public static TextureStorageMem1DEXT glTextureStorageMem1DEXT;


        //m* 368

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureStorageMem2DEXT(UInt32 texture, Int32 levels, System.Int32 internalFormat, Int32 width, Int32 height, UInt32 memory, UInt64 offset);
        public static TextureStorageMem2DEXT glTextureStorageMem2DEXT;


        //m* 369

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureStorageMem2DMultisampleEXT(UInt32 texture, Int32 samples, System.Int32 internalFormat, Int32 width, Int32 height, bool fixedSampleLocations, UInt32 memory, UInt64 offset);
        public static TextureStorageMem2DMultisampleEXT glTextureStorageMem2DMultisampleEXT;


        //m* 370

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureStorageMem3DEXT(UInt32 texture, Int32 levels, System.Int32 internalFormat, Int32 width, Int32 height, Int32 depth, UInt32 memory, UInt64 offset);
        public static TextureStorageMem3DEXT glTextureStorageMem3DEXT;


        //m* 371

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureStorageMem3DMultisampleEXT(UInt32 texture, Int32 samples, System.Int32 internalFormat, Int32 width, Int32 height, Int32 depth, bool fixedSampleLocations, UInt32 memory, UInt64 offset);
        public static TextureStorageMem3DMultisampleEXT glTextureStorageMem3DMultisampleEXT;


        //m* 372

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureViewEXT(UInt32 texture, System.Int32 target, UInt32 origtexture, System.Int32 internalformat, UInt32 minlevel, UInt32 numlevels, UInt32 minlayer, UInt32 numlayers);
        public static TextureViewEXT glTextureViewEXT;


        //m* 373

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void UseProgramStagesEXT(UInt32 pipeline, System.Int32 stages, UInt32 program);
        public static UseProgramStagesEXT glUseProgramStagesEXT;


        //m* 374

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void UseShaderProgramEXT(System.Int32 type, UInt32 program);
        public static UseShaderProgramEXT glUseShaderProgramEXT;


        //m* 375

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ValidateProgramPipelineEXT(UInt32 pipeline);
        public static ValidateProgramPipelineEXT glValidateProgramPipelineEXT;


        //m* 376

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void VertexAttribDivisorEXT(UInt32 index, UInt32 divisor);
        public static VertexAttribDivisorEXT glVertexAttribDivisorEXT;


        //m* 377

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void WaitSemaphoreEXT(UInt32 semaphore, UInt32 numBufferBarriers, [CountAttribute(Computed = "numBufferBarriers")] UInt32* buffers, UInt32 numTextureBarriers, [CountAttribute(Computed = "numTextureBarriers")] UInt32* textures, [CountAttribute(Computed = "numTextureBarriers")] System.Int32* srcLayouts);
        public static WaitSemaphoreEXT glWaitSemaphoreEXT;


        //m* 378

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void WindowRectanglesEXT(System.Int32 mode, Int32 count, [CountAttribute(Computed = "count")] Int32* box);
        public static WindowRectanglesEXT glWindowRectanglesEXT;


        //m* 379

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTexture2DDownsampleIMG(System.Int32 target, System.Int32 attachment, System.Int32 textarget, UInt32 texture, Int32 level, Int32 xscale, Int32 yscale);
        public static FramebufferTexture2DDownsampleIMG glFramebufferTexture2DDownsampleIMG;


        //m* 380

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTexture2DMultisampleIMG(System.Int32 target, System.Int32 attachment, System.Int32 textarget, UInt32 texture, Int32 level, Int32 samples);
        public static FramebufferTexture2DMultisampleIMG glFramebufferTexture2DMultisampleIMG;


        //m* 381

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTextureLayerDownsampleIMG(System.Int32 target, System.Int32 attachment, UInt32 texture, Int32 level, Int32 layer, Int32 xscale, Int32 yscale);
        public static FramebufferTextureLayerDownsampleIMG glFramebufferTextureLayerDownsampleIMG;


        //m* 382

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int64 GetTextureHandleIMG(UInt32 texture);
        public static GetTextureHandleIMG glGetTextureHandleIMG;


        //m* 383

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int64 GetTextureSamplerHandleIMG(UInt32 texture, UInt32 sampler);
        public static GetTextureSamplerHandleIMG glGetTextureSamplerHandleIMG;


        //m* 384

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniformHandleui64IMG(UInt32 program, Int32 location, UInt64 value);
        public static ProgramUniformHandleui64IMG glProgramUniformHandleui64IMG;


        //m* 385

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformHandleui64vIMG(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count")] UInt64* values);
        public static ProgramUniformHandleui64vIMG glProgramUniformHandleui64vIMG;


        //m* 386

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void RenderbufferStorageMultisampleIMG(System.Int32 target, Int32 samples, System.Int32 internalformat, Int32 width, Int32 height);
        public static RenderbufferStorageMultisampleIMG glRenderbufferStorageMultisampleIMG;


        //m* 387

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void UniformHandleui64IMG(Int32 location, UInt64 value);
        public static UniformHandleui64IMG glUniformHandleui64IMG;


        //m* 388

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformHandleui64vIMG(Int32 location, Int32 count, [CountAttribute(Parameter = "count")] UInt64* value);
        public static UniformHandleui64vIMG glUniformHandleui64vIMG;


        //m* 389

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BeginPerfQueryINTEL(UInt32 queryHandle);
        public static BeginPerfQueryINTEL glBeginPerfQueryINTEL;


        //m* 390

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void CreatePerfQueryINTEL(UInt32 queryId, [OutAttribute] UInt32* queryHandle);
        public static CreatePerfQueryINTEL glCreatePerfQueryINTEL;


        //m* 391

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DeletePerfQueryINTEL(UInt32 queryHandle);
        public static DeletePerfQueryINTEL glDeletePerfQueryINTEL;


        //m* 392

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EndPerfQueryINTEL(UInt32 queryHandle);
        public static EndPerfQueryINTEL glEndPerfQueryINTEL;


        //m* 393

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetFirstPerfQueryIdINTEL([OutAttribute] UInt32* queryId);
        public static GetFirstPerfQueryIdINTEL glGetFirstPerfQueryIdINTEL;


        //m* 394

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetNextPerfQueryIdINTEL(UInt32 queryId, [OutAttribute] UInt32* nextQueryId);
        public static GetNextPerfQueryIdINTEL glGetNextPerfQueryIdINTEL;


        //m* 395

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfCounterInfoINTEL(UInt32 queryId, UInt32 counterId, UInt32 counterNameLength, [OutAttribute, CountAttribute(Parameter = "counterNameLength")] IntPtr counterName, UInt32 counterDescLength, [OutAttribute, CountAttribute(Parameter = "counterDescLength")] IntPtr counterDesc, [OutAttribute] UInt32* counterOffset, [OutAttribute] UInt32* counterDataSize, [OutAttribute] UInt32* counterTypeEnum, [OutAttribute] UInt32* counterDataTypeEnum, [OutAttribute] UInt64* rawCounterMaxValue);
        public static GetPerfCounterInfoINTEL glGetPerfCounterInfoINTEL;


        //m* 396

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfQueryDataINTEL(UInt32 queryHandle, UInt32 flags, Int32 dataSize, [OutAttribute] IntPtr data, [OutAttribute] UInt32* bytesWritten);
        public static GetPerfQueryDataINTEL glGetPerfQueryDataINTEL;


        //m* 397

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfQueryIdByNameINTEL(string queryName, [OutAttribute] UInt32* queryId);
        public static GetPerfQueryIdByNameINTEL glGetPerfQueryIdByNameINTEL;


        //m* 398

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPerfQueryInfoINTEL(UInt32 queryId, UInt32 queryNameLength, [OutAttribute, CountAttribute(Parameter = "queryNameLength")] IntPtr queryName, [OutAttribute] UInt32* dataSize, [OutAttribute] UInt32* noCounters, [OutAttribute] UInt32* noInstances, [OutAttribute] UInt32* capsMask);
        public static GetPerfQueryInfoINTEL glGetPerfQueryInfoINTEL;


        //m* 399

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendBarrierKHR();
        public static BlendBarrierKHR glBlendBarrierKHR;


        //m* 400

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DebugMessageCallbackKHR(DebugProcKhr callback, IntPtr userParam);
        public static DebugMessageCallbackKHR glDebugMessageCallbackKHR;


        //m* 401

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DebugMessageControlKHR(System.Int32 source, System.Int32 type, System.Int32 severity, Int32 count, UInt32* ids, bool enabled);
        public static DebugMessageControlKHR glDebugMessageControlKHR;


        //m* 402

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DebugMessageInsertKHR(System.Int32 source, System.Int32 type, UInt32 id, System.Int32 severity, Int32 length, string buf);
        public static DebugMessageInsertKHR glDebugMessageInsertKHR;


        //m* 403

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate Int32 GetDebugMessageLogKHR(UInt32 count, Int32 bufSize, [OutAttribute, CountAttribute(Parameter = "count")] System.Int32* sources, [OutAttribute, CountAttribute(Parameter = "count")] System.Int32* types, [OutAttribute, CountAttribute(Parameter = "count")] UInt32* ids, [OutAttribute, CountAttribute(Parameter = "count")] System.Int32* severities, [OutAttribute, CountAttribute(Parameter = "count")] Int32* lengths, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr messageLog);
        public static GetDebugMessageLogKHR glGetDebugMessageLogKHR;


        //m* 404

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 GetGraphicsResetStatusKHR();
        public static GetGraphicsResetStatusKHR glGetGraphicsResetStatusKHR;


        //m* 405

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetnUniformfvKHR(UInt32 program, Int32 location, Int32 bufSize, [OutAttribute] Single* @params);
        public static GetnUniformfvKHR glGetnUniformfvKHR;


        //m* 406

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetnUniformivKHR(UInt32 program, Int32 location, Int32 bufSize, [OutAttribute] Int32* @params);
        public static GetnUniformivKHR glGetnUniformivKHR;


        //m* 407

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetnUniformuivKHR(UInt32 program, Int32 location, Int32 bufSize, [OutAttribute] UInt32* @params);
        public static GetnUniformuivKHR glGetnUniformuivKHR;


        //m* 408

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetObjectLabelKHR(System.Int32 identifier, UInt32 name, Int32 bufSize, [OutAttribute] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr label);
        public static GetObjectLabelKHR glGetObjectLabelKHR;


        //m* 409

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetObjectPtrLabelKHR(IntPtr ptr, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr label);
        public static GetObjectPtrLabelKHR glGetObjectPtrLabelKHR;


        //m* 410

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void GetPointervKHR(System.Int32 pname, [OutAttribute] IntPtr @params);
        public static GetPointervKHR glGetPointervKHR;


        //m* 411

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MaxShaderCompilerThreadsKHR(UInt32 count);
        public static MaxShaderCompilerThreadsKHR glMaxShaderCompilerThreadsKHR;


        //m* 412

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ObjectLabelKHR(System.Int32 identifier, UInt32 name, Int32 length, string label);
        public static ObjectLabelKHR glObjectLabelKHR;


        //m* 413

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ObjectPtrLabelKHR(IntPtr ptr, Int32 length, string label);
        public static ObjectPtrLabelKHR glObjectPtrLabelKHR;


        //m* 414

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PopDebugGroupKHR();
        public static PopDebugGroupKHR glPopDebugGroupKHR;


        //m* 415

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PushDebugGroupKHR(System.Int32 source, UInt32 id, Int32 length, string message);
        public static PushDebugGroupKHR glPushDebugGroupKHR;


        //m* 416

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ReadnPixelsKHR(Int32 x, Int32 y, Int32 width, Int32 height, System.Int32 format, System.Int32 type, Int32 bufSize, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr data);
        public static ReadnPixelsKHR glReadnPixelsKHR;


        //m* 417

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BeginConditionalRenderNV(UInt32 id, System.Int32 mode);
        public static BeginConditionalRenderNV glBeginConditionalRenderNV;


        //m* 418

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendBarrierNV();
        public static BlendBarrierNV glBlendBarrierNV;


        //m* 419

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendParameteriNV(System.Int32 pname, Int32 value);
        public static BlendParameteriNV glBlendParameteriNV;


        //m* 420

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlitFramebufferNV(Int32 srcX0, Int32 srcY0, Int32 srcX1, Int32 srcY1, Int32 dstX0, Int32 dstY0, Int32 dstX1, Int32 dstY1, System.Int32 mask, System.Int32 filter);
        public static BlitFramebufferNV glBlitFramebufferNV;


        //m* 421

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ConservativeRasterParameteriNV(System.Int32 pname, Int32 param);
        public static ConservativeRasterParameteriNV glConservativeRasterParameteriNV;


        //m* 422

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CopyBufferSubDataNV(System.Int32 readTarget, System.Int32 writeTarget, IntPtr readOffset, IntPtr writeOffset, IntPtr size);
        public static CopyBufferSubDataNV glCopyBufferSubDataNV;


        //m* 423

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CopyPathNV(UInt32 resultPath, UInt32 srcPath);
        public static CopyPathNV glCopyPathNV;


        //m* 424

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CoverageMaskNV(bool mask);
        public static CoverageMaskNV glCoverageMaskNV;


        //m* 425

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CoverageModulationNV(System.Int32 components);
        public static CoverageModulationNV glCoverageModulationNV;


        //m* 426

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void CoverageModulationTableNV(Int32 n, [CountAttribute(Parameter = "n")] Single* v);
        public static CoverageModulationTableNV glCoverageModulationTableNV;


        //m* 427

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CoverageOperationNV(System.Int32 operation);
        public static CoverageOperationNV glCoverageOperationNV;


        //m* 428

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void CoverFillPathInstancedNV(Int32 numPaths, System.Int32 pathNameType, [CountAttribute(Computed = "numPaths,pathNameType,paths")] IntPtr paths, UInt32 pathBase, System.Int32 coverMode, System.Int32 transformType, [CountAttribute(Computed = "numPaths,transformType")] Single* transformValues);
        public static CoverFillPathInstancedNV glCoverFillPathInstancedNV;


        //m* 429

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CoverFillPathNV(UInt32 path, System.Int32 coverMode);
        public static CoverFillPathNV glCoverFillPathNV;


        //m* 430

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void CoverStrokePathInstancedNV(Int32 numPaths, System.Int32 pathNameType, [CountAttribute(Computed = "numPaths,pathNameType,paths")] IntPtr paths, UInt32 pathBase, System.Int32 coverMode, System.Int32 transformType, [CountAttribute(Computed = "numPaths,transformType")] Single* transformValues);
        public static CoverStrokePathInstancedNV glCoverStrokePathInstancedNV;


        //m* 431

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CoverStrokePathNV(UInt32 path, System.Int32 coverMode);
        public static CoverStrokePathNV glCoverStrokePathNV;


        //m* 432

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteFencesNV(Int32 n, [CountAttribute(Parameter = "n")] UInt32* fences);
        public static DeleteFencesNV glDeleteFencesNV;


        //m* 433

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DeletePathsNV(UInt32 path, Int32 range);
        public static DeletePathsNV glDeletePathsNV;


        //m* 434

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DepthRangeArrayfvNV(UInt32 first, Int32 count, Single* v);
        public static DepthRangeArrayfvNV glDepthRangeArrayfvNV;


        //m* 435

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DepthRangeIndexedfNV(UInt32 index, Single n, Single f);
        public static DepthRangeIndexedfNV glDepthRangeIndexedfNV;


        //m* 436

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DisableiNV(System.Int32 target, UInt32 index);
        public static DisableiNV glDisableiNV;


        //m* 437

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawArraysInstancedNV(System.Int32 mode, Int32 first, Int32 count, Int32 primcount);
        public static DrawArraysInstancedNV glDrawArraysInstancedNV;


        //m* 438

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DrawBuffersNV(Int32 n, [CountAttribute(Parameter = "n")] System.Int32* bufs);
        public static DrawBuffersNV glDrawBuffersNV;


        //m* 439

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsInstancedNV(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 primcount);
        public static DrawElementsInstancedNV glDrawElementsInstancedNV;


        //m* 440

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawVkImageNV(UInt64 vkImage, UInt32 sampler, Single x0, Single y0, Single x1, Single y1, Single z, Single s0, Single t0, Single s1, Single t1);
        public static DrawVkImageNV glDrawVkImageNV;


        //m* 441

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EnableiNV(System.Int32 target, UInt32 index);
        public static EnableiNV glEnableiNV;


        //m* 442

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EndConditionalRenderNV();
        public static EndConditionalRenderNV glEndConditionalRenderNV;


        //m* 443

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FinishFenceNV(UInt32 fence);
        public static FinishFenceNV glFinishFenceNV;


        //m* 444

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FragmentCoverageColorNV(UInt32 color);
        public static FragmentCoverageColorNV glFragmentCoverageColorNV;


        //m* 445

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void FramebufferSampleLocationsfvNV(System.Int32 target, UInt32 start, Int32 count, Single* v);
        public static FramebufferSampleLocationsfvNV glFramebufferSampleLocationsfvNV;


        //m* 446

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenFencesNV(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* fences);
        public static GenFencesNV glGenFencesNV;


        //m* 447

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int32 GenPathsNV(Int32 range);
        public static GenPathsNV glGenPathsNV;


        //m* 448

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetCoverageModulationTableNV(Int32 bufsize, [OutAttribute] Single* v);
        public static GetCoverageModulationTableNV glGetCoverageModulationTableNV;


        //m* 449

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetFenceivNV(UInt32 fence, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetFenceivNV glGetFenceivNV;


        //m* 450

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetFloati_vNV(System.Int32 target, UInt32 index, [OutAttribute, CountAttribute(Computed = "target")] Single* data);
        public static GetFloati_vNV glGetFloati_vNV;


        //m* 451

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int64 GetImageHandleNV(UInt32 texture, Int32 level, bool layered, Int32 layer, System.Int32 format);
        public static GetImageHandleNV glGetImageHandleNV;


        //m* 452

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetInternalformatSampleivNV(System.Int32 target, System.Int32 internalformat, Int32 samples, System.Int32 pname, Int32 bufSize, [OutAttribute, CountAttribute(Parameter = "bufSize")] Int32* @params);
        public static GetInternalformatSampleivNV glGetInternalformatSampleivNV;


        //m* 453

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathColorGenfvNV(System.Int32 color, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Single* value);
        public static GetPathColorGenfvNV glGetPathColorGenfvNV;


        //m* 454

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathColorGenivNV(System.Int32 color, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* value);
        public static GetPathColorGenivNV glGetPathColorGenivNV;


        //m* 455

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathCommandsNV(UInt32 path, [OutAttribute, CountAttribute(Computed = "path")] Byte* commands);
        public static GetPathCommandsNV glGetPathCommandsNV;


        //m* 456

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathCoordsNV(UInt32 path, [OutAttribute, CountAttribute(Computed = "path")] Single* coords);
        public static GetPathCoordsNV glGetPathCoordsNV;


        //m* 457

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathDashArrayNV(UInt32 path, [OutAttribute, CountAttribute(Computed = "path")] Single* dashArray);
        public static GetPathDashArrayNV glGetPathDashArrayNV;


        //m* 458

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Single GetPathLengthNV(UInt32 path, Int32 startSegment, Int32 numSegments);
        public static GetPathLengthNV glGetPathLengthNV;


        //m* 459

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathMetricRangeNV(System.Int32 metricQueryMask, UInt32 firstPathName, Int32 numPaths, Int32 stride, [OutAttribute, CountAttribute(Computed = "metricQueryMask,numPaths,stride")] Single* metrics);
        public static GetPathMetricRangeNV glGetPathMetricRangeNV;


        //m* 460

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathMetricsNV(System.Int32 metricQueryMask, Int32 numPaths, System.Int32 pathNameType, [CountAttribute(Computed = "numPaths,pathNameType,paths")] IntPtr paths, UInt32 pathBase, Int32 stride, [OutAttribute, CountAttribute(Computed = "metricQueryMask,numPaths,stride")] Single* metrics);
        public static GetPathMetricsNV glGetPathMetricsNV;


        //m* 461

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathParameterfvNV(UInt32 path, System.Int32 pname, [OutAttribute, CountAttribute(Count = 4)] Single* value);
        public static GetPathParameterfvNV glGetPathParameterfvNV;


        //m* 462

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathParameterivNV(UInt32 path, System.Int32 pname, [OutAttribute, CountAttribute(Count = 4)] Int32* value);
        public static GetPathParameterivNV glGetPathParameterivNV;


        //m* 463

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathSpacingNV(System.Int32 pathListMode, Int32 numPaths, System.Int32 pathNameType, [CountAttribute(Computed = "numPaths,pathNameType,paths")] IntPtr paths, UInt32 pathBase, Single advanceScale, Single kerningScale, System.Int32 transformType, [OutAttribute, CountAttribute(Computed = "pathListMode,numPaths")] Single* returnedSpacing);
        public static GetPathSpacingNV glGetPathSpacingNV;


        //m* 464

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathTexGenfvNV(System.Int32 texCoordSet, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Single* value);
        public static GetPathTexGenfvNV glGetPathTexGenfvNV;


        //m* 465

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetPathTexGenivNV(System.Int32 texCoordSet, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* value);
        public static GetPathTexGenivNV glGetPathTexGenivNV;


        //m* 466

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetProgramResourcefvNV(UInt32 program, System.Int32 programInterface, UInt32 index, Int32 propCount, System.Int32* props, Int32 bufSize, [OutAttribute] Int32* length, [OutAttribute] Single* @params);
        public static GetProgramResourcefvNV glGetProgramResourcefvNV;


        //m* 467

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int64 GetTextureHandleNV(UInt32 texture);
        public static GetTextureHandleNV glGetTextureHandleNV;


        //m* 468

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate Int64 GetTextureSamplerHandleNV(UInt32 texture, UInt32 sampler);
        public static GetTextureSamplerHandleNV glGetTextureSamplerHandleNV;


        //m* 469

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetUniformi64vNV(UInt32 program, Int32 location, [OutAttribute, CountAttribute(Computed = "program,location")] Int64* @params);
        public static GetUniformi64vNV glGetUniformi64vNV;


        //m* 470

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate IntPtr GetVkProcAddrNV([CountAttribute(Computed = "name")] string name);
        public static GetVkProcAddrNV glGetVkProcAddrNV;


        //m* 471

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void InterpolatePathsNV(UInt32 resultPath, UInt32 pathA, UInt32 pathB, Single weight);
        public static InterpolatePathsNV glInterpolatePathsNV;


        //m* 472

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsEnablediNV(System.Int32 target, UInt32 index);
        public static IsEnablediNV glIsEnablediNV;


        //m* 473

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsFenceNV(UInt32 fence);
        public static IsFenceNV glIsFenceNV;


        //m* 474

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsImageHandleResidentNV(UInt64 handle);
        public static IsImageHandleResidentNV glIsImageHandleResidentNV;


        //m* 475

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsPathNV(UInt32 path);
        public static IsPathNV glIsPathNV;


        //m* 476

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsPointInFillPathNV(UInt32 path, UInt32 mask, Single x, Single y);
        public static IsPointInFillPathNV glIsPointInFillPathNV;


        //m* 477

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsPointInStrokePathNV(UInt32 path, Single x, Single y);
        public static IsPointInStrokePathNV glIsPointInStrokePathNV;


        //m* 478

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsTextureHandleResidentNV(UInt64 handle);
        public static IsTextureHandleResidentNV glIsTextureHandleResidentNV;


        //m* 479

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MakeImageHandleNonResidentNV(UInt64 handle);
        public static MakeImageHandleNonResidentNV glMakeImageHandleNonResidentNV;


        //m* 480

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MakeImageHandleResidentNV(UInt64 handle, System.Int32 access);
        public static MakeImageHandleResidentNV glMakeImageHandleResidentNV;


        //m* 481

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MakeTextureHandleNonResidentNV(UInt64 handle);
        public static MakeTextureHandleNonResidentNV glMakeTextureHandleNonResidentNV;


        //m* 482

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MakeTextureHandleResidentNV(UInt64 handle);
        public static MakeTextureHandleResidentNV glMakeTextureHandleResidentNV;


        //m* 483

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixLoad3x2fNV(System.Int32 matrixMode, Single* m);
        public static MatrixLoad3x2fNV glMatrixLoad3x2fNV;


        //m* 484

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixLoad3x3fNV(System.Int32 matrixMode, Single* m);
        public static MatrixLoad3x3fNV glMatrixLoad3x3fNV;


        //m* 485

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixLoadTranspose3x3fNV(System.Int32 matrixMode, Single* m);
        public static MatrixLoadTranspose3x3fNV glMatrixLoadTranspose3x3fNV;


        //m* 486

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixMult3x2fNV(System.Int32 matrixMode, Single* m);
        public static MatrixMult3x2fNV glMatrixMult3x2fNV;


        //m* 487

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixMult3x3fNV(System.Int32 matrixMode, Single* m);
        public static MatrixMult3x3fNV glMatrixMult3x3fNV;


        //m* 488

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void MatrixMultTranspose3x3fNV(System.Int32 matrixMode, Single* m);
        public static MatrixMultTranspose3x3fNV glMatrixMultTranspose3x3fNV;


        //m* 489

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void NamedFramebufferSampleLocationsfvNV(UInt32 framebuffer, UInt32 start, Int32 count, Single* v);
        public static NamedFramebufferSampleLocationsfvNV glNamedFramebufferSampleLocationsfvNV;


        //m* 490

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void PathColorGenNV(System.Int32 color, System.Int32 genMode, System.Int32 colorFormat, [CountAttribute(Computed = "genMode,colorFormat")] Single* coeffs);
        public static PathColorGenNV glPathColorGenNV;


        //m* 491

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void PathCommandsNV(UInt32 path, Int32 numCommands, [CountAttribute(Parameter = "numCommands")] Byte* commands, Int32 numCoords, System.Int32 coordType, [CountAttribute(Computed = "numCoords,coordType")] IntPtr coords);
        public static PathCommandsNV glPathCommandsNV;


        //m* 492

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathCoordsNV(UInt32 path, Int32 numCoords, System.Int32 coordType, [CountAttribute(Computed = "numCoords,coordType")] IntPtr coords);
        public static PathCoordsNV glPathCoordsNV;


        //m* 493

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathCoverDepthFuncNV(System.Int32 func);
        public static PathCoverDepthFuncNV glPathCoverDepthFuncNV;


        //m* 494

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void PathDashArrayNV(UInt32 path, Int32 dashCount, [CountAttribute(Parameter = "dashCount")] Single* dashArray);
        public static PathDashArrayNV glPathDashArrayNV;


        //m* 495

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathFogGenNV(System.Int32 genMode);
        public static PathFogGenNV glPathFogGenNV;


        //m* 496

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 PathGlyphIndexArrayNV(UInt32 firstPathName, System.Int32 fontTarget, IntPtr fontName, System.Int32 fontStyle, UInt32 firstGlyphIndex, Int32 numGlyphs, UInt32 pathParameterTemplate, Single emScale);
        public static PathGlyphIndexArrayNV glPathGlyphIndexArrayNV;


        //m* 497

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 PathGlyphIndexRangeNV(System.Int32 fontTarget, IntPtr fontName, System.Int32 fontStyle, UInt32 pathParameterTemplate, Single emScale, UInt32 baseAndCount);
        public static PathGlyphIndexRangeNV glPathGlyphIndexRangeNV;


        //m* 498

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathGlyphRangeNV(UInt32 firstPathName, System.Int32 fontTarget, [CountAttribute(Computed = "fontTarget,fontName")] IntPtr fontName, System.Int32 fontStyle, UInt32 firstGlyph, Int32 numGlyphs, System.Int32 handleMissingGlyphs, UInt32 pathParameterTemplate, Single emScale);
        public static PathGlyphRangeNV glPathGlyphRangeNV;


        //m* 499

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathGlyphsNV(UInt32 firstPathName, System.Int32 fontTarget, [CountAttribute(Computed = "fontTarget,fontName")] IntPtr fontName, System.Int32 fontStyle, Int32 numGlyphs, System.Int32 type, [CountAttribute(Computed = "numGlyphs,type,charcodes")] IntPtr charcodes, System.Int32 handleMissingGlyphs, UInt32 pathParameterTemplate, Single emScale);
        public static PathGlyphsNV glPathGlyphsNV;


        //m* 500

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate System.Int32 PathMemoryGlyphIndexArrayNV(UInt32 firstPathName, System.Int32 fontTarget, IntPtr fontSize, IntPtr fontData, Int32 faceIndex, UInt32 firstGlyphIndex, Int32 numGlyphs, UInt32 pathParameterTemplate, Single emScale);
        public static PathMemoryGlyphIndexArrayNV glPathMemoryGlyphIndexArrayNV;


        //m* 501

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathParameterfNV(UInt32 path, System.Int32 pname, Single value);
        public static PathParameterfNV glPathParameterfNV;


        //m* 502

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void PathParameterfvNV(UInt32 path, System.Int32 pname, [CountAttribute(Computed = "pname")] Single* value);
        public static PathParameterfvNV glPathParameterfvNV;


        //m* 503

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathParameteriNV(UInt32 path, System.Int32 pname, Int32 value);
        public static PathParameteriNV glPathParameteriNV;


        //m* 504

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void PathParameterivNV(UInt32 path, System.Int32 pname, [CountAttribute(Computed = "pname")] Int32* value);
        public static PathParameterivNV glPathParameterivNV;


        //m* 505

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathStencilDepthOffsetNV(Single factor, Single units);
        public static PathStencilDepthOffsetNV glPathStencilDepthOffsetNV;


        //m* 506

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathStencilFuncNV(System.Int32 func, Int32 @ref, UInt32 mask);
        public static PathStencilFuncNV glPathStencilFuncNV;


        //m* 507

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathStringNV(UInt32 path, System.Int32 format, Int32 length, [CountAttribute(Parameter = "length")] IntPtr pathString);
        public static PathStringNV glPathStringNV;


        //m* 508

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void PathSubCommandsNV(UInt32 path, Int32 commandStart, Int32 commandsToDelete, Int32 numCommands, [CountAttribute(Parameter = "numCommands")] Byte* commands, Int32 numCoords, System.Int32 coordType, [CountAttribute(Computed = "numCoords,coordType")] IntPtr coords);
        public static PathSubCommandsNV glPathSubCommandsNV;


        //m* 509

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PathSubCoordsNV(UInt32 path, Int32 coordStart, Int32 numCoords, System.Int32 coordType, [CountAttribute(Computed = "numCoords,coordType")] IntPtr coords);
        public static PathSubCoordsNV glPathSubCoordsNV;


        //m* 510

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void PathTexGenNV(System.Int32 texCoordSet, System.Int32 genMode, Int32 components, [CountAttribute(Computed = "genMode,components")] Single* coeffs);
        public static PathTexGenNV glPathTexGenNV;


        //m* 511

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate byte PointAlongPathNV(UInt32 path, Int32 startSegment, Int32 numSegments, Single distance, [OutAttribute, CountAttribute(Count = 1)] Single* x, [OutAttribute, CountAttribute(Count = 1)] Single* y, [OutAttribute, CountAttribute(Count = 1)] Single* tangentX, [OutAttribute, CountAttribute(Count = 1)] Single* tangentY);
        public static PointAlongPathNV glPointAlongPathNV;


        //m* 512

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PolygonModeNV(System.Int32 face, System.Int32 mode);
        public static PolygonModeNV glPolygonModeNV;


        //m* 513

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramPathFragmentInputGenNV(UInt32 program, Int32 location, System.Int32 genMode, Int32 components, Single* coeffs);
        public static ProgramPathFragmentInputGenNV glProgramPathFragmentInputGenNV;


        //m* 514

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform1i64NV(UInt32 program, Int32 location, Int64 x);
        public static ProgramUniform1i64NV glProgramUniform1i64NV;


        //m* 515

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform1i64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count")] Int64* value);
        public static ProgramUniform1i64vNV glProgramUniform1i64vNV;


        //m* 516

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform1ui64NV(UInt32 program, Int32 location, UInt64 x);
        public static ProgramUniform1ui64NV glProgramUniform1ui64NV;


        //m* 517

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform1ui64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count")] UInt64* value);
        public static ProgramUniform1ui64vNV glProgramUniform1ui64vNV;


        //m* 518

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform2i64NV(UInt32 program, Int32 location, Int64 x, Int64 y);
        public static ProgramUniform2i64NV glProgramUniform2i64NV;


        //m* 519

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform2i64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] Int64* value);
        public static ProgramUniform2i64vNV glProgramUniform2i64vNV;


        //m* 520

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform2ui64NV(UInt32 program, Int32 location, UInt64 x, UInt64 y);
        public static ProgramUniform2ui64NV glProgramUniform2ui64NV;


        //m* 521

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform2ui64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] UInt64* value);
        public static ProgramUniform2ui64vNV glProgramUniform2ui64vNV;


        //m* 522

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform3i64NV(UInt32 program, Int32 location, Int64 x, Int64 y, Int64 z);
        public static ProgramUniform3i64NV glProgramUniform3i64NV;


        //m* 523

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform3i64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] Int64* value);
        public static ProgramUniform3i64vNV glProgramUniform3i64vNV;


        //m* 524

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform3ui64NV(UInt32 program, Int32 location, UInt64 x, UInt64 y, UInt64 z);
        public static ProgramUniform3ui64NV glProgramUniform3ui64NV;


        //m* 525

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform3ui64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] UInt64* value);
        public static ProgramUniform3ui64vNV glProgramUniform3ui64vNV;


        //m* 526

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform4i64NV(UInt32 program, Int32 location, Int64 x, Int64 y, Int64 z, Int64 w);
        public static ProgramUniform4i64NV glProgramUniform4i64NV;


        //m* 527

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform4i64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] Int64* value);
        public static ProgramUniform4i64vNV glProgramUniform4i64vNV;


        //m* 528

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniform4ui64NV(UInt32 program, Int32 location, UInt64 x, UInt64 y, UInt64 z, UInt64 w);
        public static ProgramUniform4ui64NV glProgramUniform4ui64NV;


        //m* 529

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniform4ui64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] UInt64* value);
        public static ProgramUniform4ui64vNV glProgramUniform4ui64vNV;


        //m* 530

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramUniformHandleui64NV(UInt32 program, Int32 location, UInt64 value);
        public static ProgramUniformHandleui64NV glProgramUniformHandleui64NV;


        //m* 531

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ProgramUniformHandleui64vNV(UInt32 program, Int32 location, Int32 count, [CountAttribute(Parameter = "count")] UInt64* values);
        public static ProgramUniformHandleui64vNV glProgramUniformHandleui64vNV;


        //m* 532

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ReadBufferNV(System.Int32 mode);
        public static ReadBufferNV glReadBufferNV;


        //m* 533

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void RenderbufferStorageMultisampleNV(System.Int32 target, Int32 samples, System.Int32 internalformat, Int32 width, Int32 height);
        public static RenderbufferStorageMultisampleNV glRenderbufferStorageMultisampleNV;


        //m* 534

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ResolveDepthValuesNV();
        public static ResolveDepthValuesNV glResolveDepthValuesNV;


        //m* 535

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ScissorArrayvNV(UInt32 first, Int32 count, [CountAttribute(Computed = "count")] Int32* v);
        public static ScissorArrayvNV glScissorArrayvNV;


        //m* 536

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ScissorIndexedNV(UInt32 index, Int32 left, Int32 bottom, Int32 width, Int32 height);
        public static ScissorIndexedNV glScissorIndexedNV;


        //m* 537

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ScissorIndexedvNV(UInt32 index, [CountAttribute(Count = 4)] Int32* v);
        public static ScissorIndexedvNV glScissorIndexedvNV;


        //m* 538

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void SetFenceNV(UInt32 fence, System.Int32 condition);
        public static SetFenceNV glSetFenceNV;


        //m* 539

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void SignalVkFenceNV(UInt64 vkFence);
        public static SignalVkFenceNV glSignalVkFenceNV;


        //m* 540

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void SignalVkSemaphoreNV(UInt64 vkSemaphore);
        public static SignalVkSemaphoreNV glSignalVkSemaphoreNV;


        //m* 541

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void StencilFillPathInstancedNV(Int32 numPaths, System.Int32 pathNameType, [CountAttribute(Computed = "numPaths,pathNameType,paths")] IntPtr paths, UInt32 pathBase, System.Int32 fillMode, UInt32 mask, System.Int32 transformType, [CountAttribute(Computed = "numPaths,transformType")] Single* transformValues);
        public static StencilFillPathInstancedNV glStencilFillPathInstancedNV;


        //m* 542

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilFillPathNV(UInt32 path, System.Int32 fillMode, UInt32 mask);
        public static StencilFillPathNV glStencilFillPathNV;


        //m* 543

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void StencilStrokePathInstancedNV(Int32 numPaths, System.Int32 pathNameType, [CountAttribute(Computed = "numPaths,pathNameType,paths")] IntPtr paths, UInt32 pathBase, Int32 reference, UInt32 mask, System.Int32 transformType, [CountAttribute(Computed = "numPaths,transformType")] Single* transformValues);
        public static StencilStrokePathInstancedNV glStencilStrokePathInstancedNV;


        //m* 544

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilStrokePathNV(UInt32 path, Int32 reference, UInt32 mask);
        public static StencilStrokePathNV glStencilStrokePathNV;


        //m* 545

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void StencilThenCoverFillPathInstancedNV(Int32 numPaths, System.Int32 pathNameType, IntPtr paths, UInt32 pathBase, System.Int32 fillMode, UInt32 mask, System.Int32 coverMode, System.Int32 transformType, Single* transformValues);
        public static StencilThenCoverFillPathInstancedNV glStencilThenCoverFillPathInstancedNV;


        //m* 546

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilThenCoverFillPathNV(UInt32 path, System.Int32 fillMode, UInt32 mask, System.Int32 coverMode);
        public static StencilThenCoverFillPathNV glStencilThenCoverFillPathNV;


        //m* 547

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void StencilThenCoverStrokePathInstancedNV(Int32 numPaths, System.Int32 pathNameType, IntPtr paths, UInt32 pathBase, Int32 reference, UInt32 mask, System.Int32 coverMode, System.Int32 transformType, Single* transformValues);
        public static StencilThenCoverStrokePathInstancedNV glStencilThenCoverStrokePathInstancedNV;


        //m* 548

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StencilThenCoverStrokePathNV(UInt32 path, Int32 reference, UInt32 mask, System.Int32 coverMode);
        public static StencilThenCoverStrokePathNV glStencilThenCoverStrokePathNV;


        //m* 549

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void SubpixelPrecisionBiasNV(UInt32 xbits, UInt32 ybits);
        public static SubpixelPrecisionBiasNV glSubpixelPrecisionBiasNV;


        //m* 550

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte TestFenceNV(UInt32 fence);
        public static TestFenceNV glTestFenceNV;


        //m* 551

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void TransformPathNV(UInt32 resultPath, UInt32 srcPath, System.Int32 transformType, [CountAttribute(Computed = "transformType")] Single* transformValues);
        public static TransformPathNV glTransformPathNV;


        //m* 552

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform1i64NV(Int32 location, Int64 x);
        public static Uniform1i64NV glUniform1i64NV;


        //m* 553

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform1i64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count*1")] Int64* value);
        public static Uniform1i64vNV glUniform1i64vNV;


        //m* 554

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform1ui64NV(Int32 location, UInt64 x);
        public static Uniform1ui64NV glUniform1ui64NV;


        //m* 555

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform1ui64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count*1")] UInt64* value);
        public static Uniform1ui64vNV glUniform1ui64vNV;


        //m* 556

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform2i64NV(Int32 location, Int64 x, Int64 y);
        public static Uniform2i64NV glUniform2i64NV;


        //m* 557

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform2i64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] Int64* value);
        public static Uniform2i64vNV glUniform2i64vNV;


        //m* 558

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform2ui64NV(Int32 location, UInt64 x, UInt64 y);
        public static Uniform2ui64NV glUniform2ui64NV;


        //m* 559

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform2ui64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count*2")] UInt64* value);
        public static Uniform2ui64vNV glUniform2ui64vNV;


        //m* 560

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform3i64NV(Int32 location, Int64 x, Int64 y, Int64 z);
        public static Uniform3i64NV glUniform3i64NV;


        //m* 561

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform3i64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] Int64* value);
        public static Uniform3i64vNV glUniform3i64vNV;


        //m* 562

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform3ui64NV(Int32 location, UInt64 x, UInt64 y, UInt64 z);
        public static Uniform3ui64NV glUniform3ui64NV;


        //m* 563

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform3ui64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count*3")] UInt64* value);
        public static Uniform3ui64vNV glUniform3ui64vNV;


        //m* 564

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform4i64NV(Int32 location, Int64 x, Int64 y, Int64 z, Int64 w);
        public static Uniform4i64NV glUniform4i64NV;


        //m* 565

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform4i64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] Int64* value);
        public static Uniform4i64vNV glUniform4i64vNV;


        //m* 566

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void Uniform4ui64NV(Int32 location, UInt64 x, UInt64 y, UInt64 z, UInt64 w);
        public static Uniform4ui64NV glUniform4ui64NV;


        //m* 567

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void Uniform4ui64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count*4")] UInt64* value);
        public static Uniform4ui64vNV glUniform4ui64vNV;


        //m* 568

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void UniformHandleui64NV(Int32 location, UInt64 value);
        public static UniformHandleui64NV glUniformHandleui64NV;


        //m* 569

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformHandleui64vNV(Int32 location, Int32 count, [CountAttribute(Parameter = "count")] UInt64* value);
        public static UniformHandleui64vNV glUniformHandleui64vNV;


        //m* 570

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix2x3fvNV(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*6")] Single* value);
        public static UniformMatrix2x3fvNV glUniformMatrix2x3fvNV;


        //m* 571

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix2x4fvNV(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*8")] Single* value);
        public static UniformMatrix2x4fvNV glUniformMatrix2x4fvNV;


        //m* 572

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix3x2fvNV(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*6")] Single* value);
        public static UniformMatrix3x2fvNV glUniformMatrix3x2fvNV;


        //m* 573

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix3x4fvNV(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*12")] Single* value);
        public static UniformMatrix3x4fvNV glUniformMatrix3x4fvNV;


        //m* 574

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix4x2fvNV(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*8")] Single* value);
        public static UniformMatrix4x2fvNV glUniformMatrix4x2fvNV;


        //m* 575

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void UniformMatrix4x3fvNV(Int32 location, Int32 count, bool transpose, [CountAttribute(Parameter = "count*12")] Single* value);
        public static UniformMatrix4x3fvNV glUniformMatrix4x3fvNV;


        //m* 576

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void VertexAttribDivisorNV(UInt32 index, UInt32 divisor);
        public static VertexAttribDivisorNV glVertexAttribDivisorNV;


        //m* 577

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ViewportArrayvNV(UInt32 first, Int32 count, [CountAttribute(Computed = "count")] Single* v);
        public static ViewportArrayvNV glViewportArrayvNV;


        //m* 578

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ViewportIndexedfNV(UInt32 index, Single x, Single y, Single w, Single h);
        public static ViewportIndexedfNV glViewportIndexedfNV;


        //m* 579

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ViewportIndexedfvNV(UInt32 index, [CountAttribute(Count = 4)] Single* v);
        public static ViewportIndexedfvNV glViewportIndexedfvNV;


        //m* 580

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ViewportPositionWScaleNV(UInt32 index, Single xcoeff, Single ycoeff);
        public static ViewportPositionWScaleNV glViewportPositionWScaleNV;


        //m* 581

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ViewportSwizzleNV(UInt32 index, System.Int32 swizzlex, System.Int32 swizzley, System.Int32 swizzlez, System.Int32 swizzlew);
        public static ViewportSwizzleNV glViewportSwizzleNV;


        //m* 582

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void WaitVkSemaphoreNV(UInt64 vkSemaphore);
        public static WaitVkSemaphoreNV glWaitVkSemaphoreNV;


        //m* 583

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void WeightPathsNV(UInt32 resultPath, Int32 numPaths, [CountAttribute(Parameter = "numPaths")] UInt32* paths, [CountAttribute(Parameter = "numPaths")] Single* weights);
        public static WeightPathsNV glWeightPathsNV;


        //m* 584

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BindVertexArrayOES(UInt32 array);
        public static BindVertexArrayOES glBindVertexArrayOES;


        //m* 585

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendEquationiOES(UInt32 buf, System.Int32 mode);
        public static BlendEquationiOES glBlendEquationiOES;


        //m* 586

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendEquationSeparateiOES(UInt32 buf, System.Int32 modeRGB, System.Int32 modeAlpha);
        public static BlendEquationSeparateiOES glBlendEquationSeparateiOES;


        //m* 587

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendFunciOES(UInt32 buf, System.Int32 src, System.Int32 dst);
        public static BlendFunciOES glBlendFunciOES;


        //m* 588

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void BlendFuncSeparateiOES(UInt32 buf, System.Int32 srcRGB, System.Int32 dstRGB, System.Int32 srcAlpha, System.Int32 dstAlpha);
        public static BlendFuncSeparateiOES glBlendFuncSeparateiOES;


        //m* 589

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ColorMaskiOES(UInt32 index, bool r, bool g, bool b, bool a);
        public static ColorMaskiOES glColorMaskiOES;


        //m* 590

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CompressedTexImage3DOES(System.Int32 target, Int32 level, System.Int32 internalformat, Int32 width, Int32 height, Int32 depth, Int32 border, Int32 imageSize, [CountAttribute(Parameter = "imageSize")] IntPtr data);
        public static CompressedTexImage3DOES glCompressedTexImage3DOES;


        //m* 591

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CompressedTexSubImage3DOES(System.Int32 target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 zoffset, Int32 width, Int32 height, Int32 depth, System.Int32 format, Int32 imageSize, [CountAttribute(Parameter = "imageSize")] IntPtr data);
        public static CompressedTexSubImage3DOES glCompressedTexSubImage3DOES;


        //m* 592

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CopyImageSubDataOES(UInt32 srcName, System.Int32 srcTarget, Int32 srcLevel, Int32 srcX, Int32 srcY, Int32 srcZ, UInt32 dstName, System.Int32 dstTarget, Int32 dstLevel, Int32 dstX, Int32 dstY, Int32 dstZ, Int32 srcWidth, Int32 srcHeight, Int32 srcDepth);
        public static CopyImageSubDataOES glCopyImageSubDataOES;


        //m* 593

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void CopyTexSubImage3DOES(System.Int32 target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 zoffset, Int32 x, Int32 y, Int32 width, Int32 height);
        public static CopyTexSubImage3DOES glCopyTexSubImage3DOES;


        //m* 594

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DeleteVertexArraysOES(Int32 n, [CountAttribute(Parameter = "n")] UInt32* arrays);
        public static DeleteVertexArraysOES glDeleteVertexArraysOES;


        //m* 595

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void DepthRangeArrayfvOES(UInt32 first, Int32 count, Single* v);
        public static DepthRangeArrayfvOES glDepthRangeArrayfvOES;


        //m* 596

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DepthRangeIndexedfOES(UInt32 index, Single n, Single f);
        public static DepthRangeIndexedfOES glDepthRangeIndexedfOES;


        //m* 597

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DisableiOES(System.Int32 target, UInt32 index);
        public static DisableiOES glDisableiOES;


        //m* 598

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsBaseVertexOES(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 basevertex);
        public static DrawElementsBaseVertexOES glDrawElementsBaseVertexOES;


        //m* 599

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawElementsInstancedBaseVertexOES(System.Int32 mode, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 instancecount, Int32 basevertex);
        public static DrawElementsInstancedBaseVertexOES glDrawElementsInstancedBaseVertexOES;


        //m* 600

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DrawRangeElementsBaseVertexOES(System.Int32 mode, UInt32 start, UInt32 end, Int32 count, System.Int32 type, [CountAttribute(Computed = "count,type")] IntPtr indices, Int32 basevertex);
        public static DrawRangeElementsBaseVertexOES glDrawRangeElementsBaseVertexOES;


        //m* 601

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EGLImageTargetRenderbufferStorageOES(System.Int32 target, IntPtr image);
        public static EGLImageTargetRenderbufferStorageOES glEGLImageTargetRenderbufferStorageOES;


        //m* 602

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EGLImageTargetTexture2DOES(System.Int32 target, IntPtr image);
        public static EGLImageTargetTexture2DOES glEGLImageTargetTexture2DOES;


        //m* 603

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EnableiOES(System.Int32 target, UInt32 index);
        public static EnableiOES glEnableiOES;


        //m* 604

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTexture3DOES(System.Int32 target, System.Int32 attachment, System.Int32 textarget, UInt32 texture, Int32 level, Int32 zoffset);
        public static FramebufferTexture3DOES glFramebufferTexture3DOES;


        //m* 605

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTextureOES(System.Int32 target, System.Int32 attachment, UInt32 texture, Int32 level);
        public static FramebufferTextureOES glFramebufferTextureOES;


        //m* 606

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GenVertexArraysOES(Int32 n, [OutAttribute, CountAttribute(Parameter = "n")] UInt32* arrays);
        public static GenVertexArraysOES glGenVertexArraysOES;


        //m* 607

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void GetBufferPointervOES(System.Int32 target, System.Int32 pname, [OutAttribute] IntPtr @params);
        public static GetBufferPointervOES glGetBufferPointervOES;


        //m* 608

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetFloati_vOES(System.Int32 target, UInt32 index, [OutAttribute, CountAttribute(Computed = "target")] Single* data);
        public static GetFloati_vOES glGetFloati_vOES;


        //m* 609

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetProgramBinaryOES(UInt32 program, Int32 bufSize, [OutAttribute, CountAttribute(Count = 1)] Int32* length, [OutAttribute, CountAttribute(Count = 1)] System.Int32* binaryFormat, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr binary);
        public static GetProgramBinaryOES glGetProgramBinaryOES;


        //m* 610

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetSamplerParameterIivOES(UInt32 sampler, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetSamplerParameterIivOES glGetSamplerParameterIivOES;


        //m* 611

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetSamplerParameterIuivOES(UInt32 sampler, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] UInt32* @params);
        public static GetSamplerParameterIuivOES glGetSamplerParameterIuivOES;


        //m* 612

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetTexParameterIivOES(System.Int32 target, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] Int32* @params);
        public static GetTexParameterIivOES glGetTexParameterIivOES;


        //m* 613

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetTexParameterIuivOES(System.Int32 target, System.Int32 pname, [OutAttribute, CountAttribute(Computed = "pname")] UInt32* @params);
        public static GetTexParameterIuivOES glGetTexParameterIuivOES;


        //m* 614

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsEnablediOES(System.Int32 target, UInt32 index);
        public static IsEnablediOES glIsEnablediOES;


        //m* 615

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte IsVertexArrayOES(UInt32 array);
        public static IsVertexArrayOES glIsVertexArrayOES;


        //m* 616

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate IntPtr MapBufferOES(System.Int32 target, System.Int32 access);
        public static MapBufferOES glMapBufferOES;


        //m* 617

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void MinSampleShadingOES(Single value);
        public static MinSampleShadingOES glMinSampleShadingOES;


        //m* 618

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PatchParameteriOES(System.Int32 pname, Int32 value);
        public static PatchParameteriOES glPatchParameteriOES;


        //m* 619

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void PrimitiveBoundingBoxOES(Single minX, Single minY, Single minZ, Single minW, Single maxX, Single maxY, Single maxZ, Single maxW);
        public static PrimitiveBoundingBoxOES glPrimitiveBoundingBoxOES;


        //m* 620

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ProgramBinaryOES(UInt32 program, System.Int32 binaryFormat, [CountAttribute(Parameter = "length")] IntPtr binary, Int32 length);
        public static ProgramBinaryOES glProgramBinaryOES;


        //m* 621

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void SamplerParameterIivOES(UInt32 sampler, System.Int32 pname, [CountAttribute(Computed = "pname")] Int32* param);
        public static SamplerParameterIivOES glSamplerParameterIivOES;


        //m* 622

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void SamplerParameterIuivOES(UInt32 sampler, System.Int32 pname, [CountAttribute(Computed = "pname")] UInt32* param);
        public static SamplerParameterIuivOES glSamplerParameterIuivOES;


        //m* 623

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ScissorArrayvOES(UInt32 first, Int32 count, [CountAttribute(Computed = "count")] Int32* v);
        public static ScissorArrayvOES glScissorArrayvOES;


        //m* 624

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ScissorIndexedOES(UInt32 index, Int32 left, Int32 bottom, Int32 width, Int32 height);
        public static ScissorIndexedOES glScissorIndexedOES;


        //m* 625

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ScissorIndexedvOES(UInt32 index, [CountAttribute(Count = 4)] Int32* v);
        public static ScissorIndexedvOES glScissorIndexedvOES;


        //m* 626

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexBufferOES(System.Int32 target, System.Int32 internalformat, UInt32 buffer);
        public static TexBufferOES glTexBufferOES;


        //m* 627

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexBufferRangeOES(System.Int32 target, System.Int32 internalformat, UInt32 buffer, IntPtr offset, IntPtr size);
        public static TexBufferRangeOES glTexBufferRangeOES;


        //m* 628

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexImage3DOES(System.Int32 target, Int32 level, System.Int32 internalformat, Int32 width, Int32 height, Int32 depth, Int32 border, System.Int32 format, System.Int32 type, [CountAttribute(Computed = "format,type,width,height,depth")] IntPtr pixels);
        public static TexImage3DOES glTexImage3DOES;


        //m* 629

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void TexParameterIivOES(System.Int32 target, System.Int32 pname, [CountAttribute(Computed = "pname")] Int32* @params);
        public static TexParameterIivOES glTexParameterIivOES;


        //m* 630

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void TexParameterIuivOES(System.Int32 target, System.Int32 pname, [CountAttribute(Computed = "pname")] UInt32* @params);
        public static TexParameterIuivOES glTexParameterIuivOES;


        //m* 631

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexStorage3DMultisampleOES(System.Int32 target, Int32 samples, System.Int32 internalformat, Int32 width, Int32 height, Int32 depth, bool fixedsamplelocations);
        public static TexStorage3DMultisampleOES glTexStorage3DMultisampleOES;


        //m* 632

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TexSubImage3DOES(System.Int32 target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 zoffset, Int32 width, Int32 height, Int32 depth, System.Int32 format, System.Int32 type, [CountAttribute(Computed = "format,type,width,height,depth")] IntPtr pixels);
        public static TexSubImage3DOES glTexSubImage3DOES;


        //m* 633

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureViewOES(UInt32 texture, System.Int32 target, UInt32 origtexture, System.Int32 internalformat, UInt32 minlevel, UInt32 numlevels, UInt32 minlayer, UInt32 numlayers);
        public static TextureViewOES glTextureViewOES;


        //m* 634

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte UnmapBufferOES(System.Int32 target);
        public static UnmapBufferOES glUnmapBufferOES;


        //m* 635

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ViewportArrayvOES(UInt32 first, Int32 count, [CountAttribute(Computed = "count")] Single* v);
        public static ViewportArrayvOES glViewportArrayvOES;


        //m* 636

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ViewportIndexedfOES(UInt32 index, Single x, Single y, Single w, Single h);
        public static ViewportIndexedfOES glViewportIndexedfOES;


        //m* 637

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ViewportIndexedfvOES(UInt32 index, [CountAttribute(Count = 4)] Single* v);
        public static ViewportIndexedfvOES glViewportIndexedfvOES;


        //m* 638

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTextureMultisampleMultiviewOVR(System.Int32 target, System.Int32 attachment, UInt32 texture, Int32 level, Int32 samples, Int32 baseViewIndex, Int32 numViews);
        public static FramebufferTextureMultisampleMultiviewOVR glFramebufferTextureMultisampleMultiviewOVR;


        //m* 639

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferTextureMultiviewOVR(System.Int32 target, System.Int32 attachment, UInt32 texture, Int32 level, Int32 baseViewIndex, Int32 numViews);
        public static FramebufferTextureMultiviewOVR glFramebufferTextureMultiviewOVR;


        //m* 640

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void AlphaFuncQCOM(System.Int32 func, Single @ref);
        public static AlphaFuncQCOM glAlphaFuncQCOM;


        //m* 641

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void DisableDriverControlQCOM(UInt32 driverControl);
        public static DisableDriverControlQCOM glDisableDriverControlQCOM;


        //m* 642

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EnableDriverControlQCOM(UInt32 driverControl);
        public static EnableDriverControlQCOM glEnableDriverControlQCOM;


        //m* 643

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void EndTilingQCOM(System.Int32 preserveMask);
        public static EndTilingQCOM glEndTilingQCOM;


        //m* 644

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ExtGetBufferPointervQCOM(System.Int32 target, [OutAttribute] IntPtr @params);
        public static ExtGetBufferPointervQCOM glExtGetBufferPointervQCOM;


        //m* 645

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ExtGetBuffersQCOM([OutAttribute, CountAttribute(Parameter = "maxBuffers")] UInt32* buffers, Int32 maxBuffers, [OutAttribute, CountAttribute(Count = 1)] Int32* numBuffers);
        public static ExtGetBuffersQCOM glExtGetBuffersQCOM;


        //m* 646

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ExtGetFramebuffersQCOM([OutAttribute, CountAttribute(Parameter = "maxFramebuffers")] UInt32* framebuffers, Int32 maxFramebuffers, [OutAttribute, CountAttribute(Count = 1)] Int32* numFramebuffers);
        public static ExtGetFramebuffersQCOM glExtGetFramebuffersQCOM;


        //m* 647

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ExtGetProgramBinarySourceQCOM(UInt32 program, System.Int32 shadertype, [OutAttribute, CountAttribute(Parameter = "*length")] IntPtr source, [OutAttribute] Int32* length);
        public static ExtGetProgramBinarySourceQCOM glExtGetProgramBinarySourceQCOM;


        //m* 648

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ExtGetProgramsQCOM([OutAttribute, CountAttribute(Parameter = "maxPrograms")] UInt32* programs, Int32 maxPrograms, [OutAttribute, CountAttribute(Count = 1)] Int32* numPrograms);
        public static ExtGetProgramsQCOM glExtGetProgramsQCOM;


        //m* 649

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ExtGetRenderbuffersQCOM([OutAttribute, CountAttribute(Parameter = "maxRenderbuffers")] UInt32* renderbuffers, Int32 maxRenderbuffers, [OutAttribute, CountAttribute(Count = 1)] Int32* numRenderbuffers);
        public static ExtGetRenderbuffersQCOM glExtGetRenderbuffersQCOM;


        //m* 650

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ExtGetShadersQCOM([OutAttribute, CountAttribute(Parameter = "maxShaders")] UInt32* shaders, Int32 maxShaders, [OutAttribute, CountAttribute(Count = 1)] Int32* numShaders);
        public static ExtGetShadersQCOM glExtGetShadersQCOM;


        //m* 651

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ExtGetTexLevelParameterivQCOM(UInt32 texture, System.Int32 face, Int32 level, System.Int32 pname, [OutAttribute] Int32* @params);
        public static ExtGetTexLevelParameterivQCOM glExtGetTexLevelParameterivQCOM;


        //m* 652

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ExtGetTexSubImageQCOM(System.Int32 target, Int32 level, Int32 xoffset, Int32 yoffset, Int32 zoffset, Int32 width, Int32 height, Int32 depth, System.Int32 format, System.Int32 type, [OutAttribute] IntPtr texels);
        public static ExtGetTexSubImageQCOM glExtGetTexSubImageQCOM;


        //m* 653

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void ExtGetTexturesQCOM([OutAttribute] UInt32* textures, Int32 maxTextures, [OutAttribute] Int32* numTextures);
        public static ExtGetTexturesQCOM glExtGetTexturesQCOM;


        //m* 654

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate byte ExtIsProgramBinaryQCOM(UInt32 program);
        public static ExtIsProgramBinaryQCOM glExtIsProgramBinaryQCOM;


        //m* 655

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void ExtTexObjectStateOverrideiQCOM(System.Int32 target, System.Int32 pname, Int32 param);
        public static ExtTexObjectStateOverrideiQCOM glExtTexObjectStateOverrideiQCOM;


        //m* 656

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferFetchBarrierQCOM();
        public static FramebufferFetchBarrierQCOM glFramebufferFetchBarrierQCOM;


        //m* 657

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void FramebufferFoveationConfigQCOM(UInt32 framebuffer, UInt32 numLayers, UInt32 focalPointsPerLayer, UInt32 requestedFeatures, [OutAttribute, CountAttribute(Count = 1)] UInt32* providedFeatures);
        public static FramebufferFoveationConfigQCOM glFramebufferFoveationConfigQCOM;


        //m* 658

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void FramebufferFoveationParametersQCOM(UInt32 framebuffer, UInt32 layer, UInt32 focalPoint, Single focalX, Single focalY, Single gainX, Single gainY, Single foveaArea);
        public static FramebufferFoveationParametersQCOM glFramebufferFoveationParametersQCOM;


        //m* 659

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetDriverControlsQCOM([OutAttribute] Int32* num, Int32 size, [OutAttribute, CountAttribute(Parameter = "size")] UInt32* driverControls);
        public static GetDriverControlsQCOM glGetDriverControlsQCOM;


        //m* 660

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public unsafe delegate void GetDriverControlStringQCOM(UInt32 driverControl, Int32 bufSize, [OutAttribute] Int32* length, [OutAttribute, CountAttribute(Parameter = "bufSize")] IntPtr driverControlString);
        public static GetDriverControlStringQCOM glGetDriverControlStringQCOM;


        //m* 661

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void StartTilingQCOM(UInt32 x, UInt32 y, UInt32 width, UInt32 height, System.Int32 preserveMask);
        public static StartTilingQCOM glStartTilingQCOM;


        //m* 662

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate void TextureFoveationParametersQCOM(UInt32 texture, UInt32 layer, UInt32 focalPoint, Single focalX, Single focalY, Single gainX, Single gainY, Single foveaArea);
        public static TextureFoveationParametersQCOM glTextureFoveationParametersQCOM;

    }
    static class GLDelInit
    {
        

        static void AssignDelegate<T>(out T del, string funcName)
        {
            IntPtr funcPtr = PlatformAddressPortal.GetAddressDelegate(funcName);
            del = (funcPtr == IntPtr.Zero) ? default(T) : (T)(object)(Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T)));
        }
        public static void LoadAll()
        {
           
            AssignDelegate(out Delegates.glBeginPerfMonitorAMD, "glBeginPerfMonitorAMD");
            AssignDelegate(out Delegates.glDeletePerfMonitorsAMD, "glDeletePerfMonitorsAMD");
            AssignDelegate(out Delegates.glEndPerfMonitorAMD, "glEndPerfMonitorAMD");
            AssignDelegate(out Delegates.glGenPerfMonitorsAMD, "glGenPerfMonitorsAMD");
            AssignDelegate(out Delegates.glGetPerfMonitorCounterDataAMD, "glGetPerfMonitorCounterDataAMD");
            AssignDelegate(out Delegates.glGetPerfMonitorCounterInfoAMD, "glGetPerfMonitorCounterInfoAMD");
            AssignDelegate(out Delegates.glGetPerfMonitorCountersAMD, "glGetPerfMonitorCountersAMD");
            AssignDelegate(out Delegates.glGetPerfMonitorCounterStringAMD, "glGetPerfMonitorCounterStringAMD");
            AssignDelegate(out Delegates.glGetPerfMonitorGroupsAMD, "glGetPerfMonitorGroupsAMD");
            AssignDelegate(out Delegates.glGetPerfMonitorGroupStringAMD, "glGetPerfMonitorGroupStringAMD");
            AssignDelegate(out Delegates.glSelectPerfMonitorCountersAMD, "glSelectPerfMonitorCountersAMD");
            AssignDelegate(out Delegates.glBlitFramebufferANGLE, "glBlitFramebufferANGLE");
            AssignDelegate(out Delegates.glDrawArraysInstancedANGLE, "glDrawArraysInstancedANGLE");
            AssignDelegate(out Delegates.glDrawElementsInstancedANGLE, "glDrawElementsInstancedANGLE");
            AssignDelegate(out Delegates.glGetTranslatedShaderSourceANGLE, "glGetTranslatedShaderSourceANGLE");
            AssignDelegate(out Delegates.glRenderbufferStorageMultisampleANGLE, "glRenderbufferStorageMultisampleANGLE");
            AssignDelegate(out Delegates.glVertexAttribDivisorANGLE, "glVertexAttribDivisorANGLE");
            AssignDelegate(out Delegates.glClientWaitSyncAPPLE, "glClientWaitSyncAPPLE");
            AssignDelegate(out Delegates.glCopyTextureLevelsAPPLE, "glCopyTextureLevelsAPPLE");
            AssignDelegate(out Delegates.glDeleteSyncAPPLE, "glDeleteSyncAPPLE");
            AssignDelegate(out Delegates.glFenceSyncAPPLE, "glFenceSyncAPPLE");
            AssignDelegate(out Delegates.glGetInteger64vAPPLE, "glGetInteger64vAPPLE");
            AssignDelegate(out Delegates.glGetSyncivAPPLE, "glGetSyncivAPPLE");
            AssignDelegate(out Delegates.glIsSyncAPPLE, "glIsSyncAPPLE");
            AssignDelegate(out Delegates.glRenderbufferStorageMultisampleAPPLE, "glRenderbufferStorageMultisampleAPPLE");
            AssignDelegate(out Delegates.glResolveMultisampleFramebufferAPPLE, "glResolveMultisampleFramebufferAPPLE");
            AssignDelegate(out Delegates.glWaitSyncAPPLE, "glWaitSyncAPPLE");
            AssignDelegate(out Delegates.glApplyFramebufferAttachmentCMAAINTEL, "glApplyFramebufferAttachmentCMAAINTEL");
            AssignDelegate(out Delegates.glActiveTexture, "glActiveTexture");
            AssignDelegate(out Delegates.glAttachShader, "glAttachShader");
            AssignDelegate(out Delegates.glBindAttribLocation, "glBindAttribLocation");
            AssignDelegate(out Delegates.glBindBuffer, "glBindBuffer");
            AssignDelegate(out Delegates.glBindFramebuffer, "glBindFramebuffer");
            AssignDelegate(out Delegates.glBindRenderbuffer, "glBindRenderbuffer");
            AssignDelegate(out Delegates.glBindTexture, "glBindTexture");
            AssignDelegate(out Delegates.glBlendColor, "glBlendColor");
            AssignDelegate(out Delegates.glBlendEquation, "glBlendEquation");
            AssignDelegate(out Delegates.glBlendEquationSeparate, "glBlendEquationSeparate");
            AssignDelegate(out Delegates.glBlendFunc, "glBlendFunc");
            AssignDelegate(out Delegates.glBlendFuncSeparate, "glBlendFuncSeparate");
            AssignDelegate(out Delegates.glBufferData, "glBufferData");
            AssignDelegate(out Delegates.glBufferSubData, "glBufferSubData");
            AssignDelegate(out Delegates.glCheckFramebufferStatus, "glCheckFramebufferStatus");
            AssignDelegate(out Delegates.glClear, "glClear");
            AssignDelegate(out Delegates.glClearColor, "glClearColor");
            AssignDelegate(out Delegates.glClearDepthf, "glClearDepthf");
            AssignDelegate(out Delegates.glClearStencil, "glClearStencil");
            AssignDelegate(out Delegates.glColorMask, "glColorMask");
            AssignDelegate(out Delegates.glCompileShader, "glCompileShader");
            AssignDelegate(out Delegates.glCompressedTexImage2D, "glCompressedTexImage2D");
            AssignDelegate(out Delegates.glCompressedTexSubImage2D, "glCompressedTexSubImage2D");
            AssignDelegate(out Delegates.glCopyTexImage2D, "glCopyTexImage2D");
            AssignDelegate(out Delegates.glCopyTexSubImage2D, "glCopyTexSubImage2D");
            AssignDelegate(out Delegates.glCreateProgram, "glCreateProgram");
            AssignDelegate(out Delegates.glCreateShader, "glCreateShader");
            AssignDelegate(out Delegates.glCullFace, "glCullFace");
            AssignDelegate(out Delegates.glDebugMessageCallback, "glDebugMessageCallback");
            AssignDelegate(out Delegates.glDebugMessageControl, "glDebugMessageControl");
            AssignDelegate(out Delegates.glDebugMessageInsert, "glDebugMessageInsert");
            AssignDelegate(out Delegates.glDeleteBuffers, "glDeleteBuffers");
            AssignDelegate(out Delegates.glDeleteFramebuffers, "glDeleteFramebuffers");
            AssignDelegate(out Delegates.glDeleteProgram, "glDeleteProgram");
            AssignDelegate(out Delegates.glDeleteRenderbuffers, "glDeleteRenderbuffers");
            AssignDelegate(out Delegates.glDeleteShader, "glDeleteShader");
            AssignDelegate(out Delegates.glDeleteTextures, "glDeleteTextures");
            AssignDelegate(out Delegates.glDepthFunc, "glDepthFunc");
            AssignDelegate(out Delegates.glDepthMask, "glDepthMask");
            AssignDelegate(out Delegates.glDepthRangef, "glDepthRangef");
            AssignDelegate(out Delegates.glDetachShader, "glDetachShader");
            AssignDelegate(out Delegates.glDisable, "glDisable");
            AssignDelegate(out Delegates.glDisableVertexAttribArray, "glDisableVertexAttribArray");
            AssignDelegate(out Delegates.glDrawArrays, "glDrawArrays");
            AssignDelegate(out Delegates.glDrawElements, "glDrawElements");
            AssignDelegate(out Delegates.glEnable, "glEnable");
            AssignDelegate(out Delegates.glEnableVertexAttribArray, "glEnableVertexAttribArray");
            AssignDelegate(out Delegates.glFinish, "glFinish");
            AssignDelegate(out Delegates.glFlush, "glFlush");
            AssignDelegate(out Delegates.glFramebufferRenderbuffer, "glFramebufferRenderbuffer");
            AssignDelegate(out Delegates.glFramebufferTexture2D, "glFramebufferTexture2D");
            AssignDelegate(out Delegates.glFrontFace, "glFrontFace");
            AssignDelegate(out Delegates.glGenBuffers, "glGenBuffers");
            AssignDelegate(out Delegates.glGenerateMipmap, "glGenerateMipmap");
            AssignDelegate(out Delegates.glGenFramebuffers, "glGenFramebuffers");
            AssignDelegate(out Delegates.glGenRenderbuffers, "glGenRenderbuffers");
            AssignDelegate(out Delegates.glGenTextures, "glGenTextures");
            AssignDelegate(out Delegates.glGetActiveAttrib, "glGetActiveAttrib");
            AssignDelegate(out Delegates.glGetActiveUniform, "glGetActiveUniform");
            AssignDelegate(out Delegates.glGetAttachedShaders, "glGetAttachedShaders");
            AssignDelegate(out Delegates.glGetAttribLocation, "glGetAttribLocation");
            AssignDelegate(out Delegates.glGetBooleanv, "glGetBooleanv");
            AssignDelegate(out Delegates.glGetBufferParameteriv, "glGetBufferParameteriv");
            AssignDelegate(out Delegates.glGetDebugMessageLog, "glGetDebugMessageLog");
            AssignDelegate(out Delegates.glGetError, "glGetError");
            AssignDelegate(out Delegates.glGetFloatv, "glGetFloatv");
            AssignDelegate(out Delegates.glGetFramebufferAttachmentParameteriv, "glGetFramebufferAttachmentParameteriv");
            AssignDelegate(out Delegates.glGetGraphicsResetStatus, "glGetGraphicsResetStatus");
            AssignDelegate(out Delegates.glGetIntegerv, "glGetIntegerv");
            AssignDelegate(out Delegates.glGetnUniformfv, "glGetnUniformfv");
            AssignDelegate(out Delegates.glGetnUniformiv, "glGetnUniformiv");
            AssignDelegate(out Delegates.glGetnUniformuiv, "glGetnUniformuiv");
            AssignDelegate(out Delegates.glGetObjectLabel, "glGetObjectLabel");
            AssignDelegate(out Delegates.glGetObjectPtrLabel, "glGetObjectPtrLabel");
            AssignDelegate(out Delegates.glGetPointerv, "glGetPointerv");
            AssignDelegate(out Delegates.glGetProgramInfoLog, "glGetProgramInfoLog");
            AssignDelegate(out Delegates.glGetProgramiv, "glGetProgramiv");
            AssignDelegate(out Delegates.glGetRenderbufferParameteriv, "glGetRenderbufferParameteriv");
            AssignDelegate(out Delegates.glGetShaderInfoLog, "glGetShaderInfoLog");
            AssignDelegate(out Delegates.glGetShaderiv, "glGetShaderiv");
            AssignDelegate(out Delegates.glGetShaderPrecisionFormat, "glGetShaderPrecisionFormat");
            AssignDelegate(out Delegates.glGetShaderSource, "glGetShaderSource");
            AssignDelegate(out Delegates.glGetString, "glGetString");
            AssignDelegate(out Delegates.glGetTexParameterfv, "glGetTexParameterfv");
            AssignDelegate(out Delegates.glGetTexParameteriv, "glGetTexParameteriv");
            AssignDelegate(out Delegates.glGetUniformfv, "glGetUniformfv");
            AssignDelegate(out Delegates.glGetUniformiv, "glGetUniformiv");
            AssignDelegate(out Delegates.glGetUniformLocation, "glGetUniformLocation");
            AssignDelegate(out Delegates.glGetVertexAttribfv, "glGetVertexAttribfv");
            AssignDelegate(out Delegates.glGetVertexAttribiv, "glGetVertexAttribiv");
            AssignDelegate(out Delegates.glGetVertexAttribPointerv, "glGetVertexAttribPointerv");
            AssignDelegate(out Delegates.glHint, "glHint");
            AssignDelegate(out Delegates.glIsBuffer, "glIsBuffer");
            AssignDelegate(out Delegates.glIsEnabled, "glIsEnabled");
            AssignDelegate(out Delegates.glIsFramebuffer, "glIsFramebuffer");
            AssignDelegate(out Delegates.glIsProgram, "glIsProgram");
            AssignDelegate(out Delegates.glIsRenderbuffer, "glIsRenderbuffer");
            AssignDelegate(out Delegates.glIsShader, "glIsShader");
            AssignDelegate(out Delegates.glIsTexture, "glIsTexture");
            AssignDelegate(out Delegates.glLineWidth, "glLineWidth");
            AssignDelegate(out Delegates.glLinkProgram, "glLinkProgram");
            AssignDelegate(out Delegates.glObjectLabel, "glObjectLabel");
            AssignDelegate(out Delegates.glObjectPtrLabel, "glObjectPtrLabel");
            AssignDelegate(out Delegates.glPixelStorei, "glPixelStorei");
            AssignDelegate(out Delegates.glPolygonOffset, "glPolygonOffset");
            AssignDelegate(out Delegates.glPopDebugGroup, "glPopDebugGroup");
            AssignDelegate(out Delegates.glPushDebugGroup, "glPushDebugGroup");
            AssignDelegate(out Delegates.glReadnPixels, "glReadnPixels");
            AssignDelegate(out Delegates.glReadPixels, "glReadPixels");
            AssignDelegate(out Delegates.glReleaseShaderCompiler, "glReleaseShaderCompiler");
            AssignDelegate(out Delegates.glRenderbufferStorage, "glRenderbufferStorage");
            AssignDelegate(out Delegates.glSampleCoverage, "glSampleCoverage");
            AssignDelegate(out Delegates.glScissor, "glScissor");
            AssignDelegate(out Delegates.glShaderBinary, "glShaderBinary");
            AssignDelegate(out Delegates.glShaderSource, "glShaderSource");
            AssignDelegate(out Delegates.glStencilFunc, "glStencilFunc");
            AssignDelegate(out Delegates.glStencilFuncSeparate, "glStencilFuncSeparate");
            AssignDelegate(out Delegates.glStencilMask, "glStencilMask");
            AssignDelegate(out Delegates.glStencilMaskSeparate, "glStencilMaskSeparate");
            AssignDelegate(out Delegates.glStencilOp, "glStencilOp");
            AssignDelegate(out Delegates.glStencilOpSeparate, "glStencilOpSeparate");
            AssignDelegate(out Delegates.glTexImage2D, "glTexImage2D");
            AssignDelegate(out Delegates.glTexParameterf, "glTexParameterf");
            AssignDelegate(out Delegates.glTexParameterfv, "glTexParameterfv");
            AssignDelegate(out Delegates.glTexParameteri, "glTexParameteri");
            AssignDelegate(out Delegates.glTexParameteriv, "glTexParameteriv");
            AssignDelegate(out Delegates.glTexSubImage2D, "glTexSubImage2D");
            AssignDelegate(out Delegates.glUniform1f, "glUniform1f");
            AssignDelegate(out Delegates.glUniform1fv, "glUniform1fv");
            AssignDelegate(out Delegates.glUniform1i, "glUniform1i");
            AssignDelegate(out Delegates.glUniform1iv, "glUniform1iv");
            AssignDelegate(out Delegates.glUniform2f, "glUniform2f");
            AssignDelegate(out Delegates.glUniform2fv, "glUniform2fv");
            AssignDelegate(out Delegates.glUniform2i, "glUniform2i");
            AssignDelegate(out Delegates.glUniform2iv, "glUniform2iv");
            AssignDelegate(out Delegates.glUniform3f, "glUniform3f");
            AssignDelegate(out Delegates.glUniform3fv, "glUniform3fv");
            AssignDelegate(out Delegates.glUniform3i, "glUniform3i");
            AssignDelegate(out Delegates.glUniform3iv, "glUniform3iv");
            AssignDelegate(out Delegates.glUniform4f, "glUniform4f");
            AssignDelegate(out Delegates.glUniform4fv, "glUniform4fv");
            AssignDelegate(out Delegates.glUniform4i, "glUniform4i");
            AssignDelegate(out Delegates.glUniform4iv, "glUniform4iv");
            AssignDelegate(out Delegates.glUniformMatrix2fv, "glUniformMatrix2fv");
            AssignDelegate(out Delegates.glUniformMatrix3fv, "glUniformMatrix3fv");
            AssignDelegate(out Delegates.glUniformMatrix4fv, "glUniformMatrix4fv");
            AssignDelegate(out Delegates.glUseProgram, "glUseProgram");
            AssignDelegate(out Delegates.glValidateProgram, "glValidateProgram");
            AssignDelegate(out Delegates.glVertexAttrib1f, "glVertexAttrib1f");
            AssignDelegate(out Delegates.glVertexAttrib1fv, "glVertexAttrib1fv");
            AssignDelegate(out Delegates.glVertexAttrib2f, "glVertexAttrib2f");
            AssignDelegate(out Delegates.glVertexAttrib2fv, "glVertexAttrib2fv");
            AssignDelegate(out Delegates.glVertexAttrib3f, "glVertexAttrib3f");
            AssignDelegate(out Delegates.glVertexAttrib3fv, "glVertexAttrib3fv");
            AssignDelegate(out Delegates.glVertexAttrib4f, "glVertexAttrib4f");
            AssignDelegate(out Delegates.glVertexAttrib4fv, "glVertexAttrib4fv");
            AssignDelegate(out Delegates.glVertexAttribPointer, "glVertexAttribPointer");
            AssignDelegate(out Delegates.glViewport, "glViewport");
            AssignDelegate(out Delegates.glAcquireKeyedMutexWin32EXT, "glAcquireKeyedMutexWin32EXT");
            AssignDelegate(out Delegates.glActiveProgramEXT, "glActiveProgramEXT");
            AssignDelegate(out Delegates.glActiveShaderProgramEXT, "glActiveShaderProgramEXT");
            AssignDelegate(out Delegates.glBeginQueryEXT, "glBeginQueryEXT");
            AssignDelegate(out Delegates.glBindFragDataLocationEXT, "glBindFragDataLocationEXT");
            AssignDelegate(out Delegates.glBindFragDataLocationIndexedEXT, "glBindFragDataLocationIndexedEXT");
            AssignDelegate(out Delegates.glBindProgramPipelineEXT, "glBindProgramPipelineEXT");
            AssignDelegate(out Delegates.glBlendEquationEXT, "glBlendEquationEXT");
            AssignDelegate(out Delegates.glBlendEquationiEXT, "glBlendEquationiEXT");
            AssignDelegate(out Delegates.glBlendEquationSeparateiEXT, "glBlendEquationSeparateiEXT");
            AssignDelegate(out Delegates.glBlendFunciEXT, "glBlendFunciEXT");
            AssignDelegate(out Delegates.glBlendFuncSeparateiEXT, "glBlendFuncSeparateiEXT");
            AssignDelegate(out Delegates.glBufferStorageEXT, "glBufferStorageEXT");
            AssignDelegate(out Delegates.glBufferStorageExternalEXT, "glBufferStorageExternalEXT");
            AssignDelegate(out Delegates.glBufferStorageMemEXT, "glBufferStorageMemEXT");
            AssignDelegate(out Delegates.glClearPixelLocalStorageuiEXT, "glClearPixelLocalStorageuiEXT");
            AssignDelegate(out Delegates.glClearTexImageEXT, "glClearTexImageEXT");
            AssignDelegate(out Delegates.glClearTexSubImageEXT, "glClearTexSubImageEXT");
            AssignDelegate(out Delegates.glClipControlEXT, "glClipControlEXT");
            AssignDelegate(out Delegates.glColorMaskiEXT, "glColorMaskiEXT");
            AssignDelegate(out Delegates.glCopyImageSubDataEXT, "glCopyImageSubDataEXT");
            AssignDelegate(out Delegates.glCreateMemoryObjectsEXT, "glCreateMemoryObjectsEXT");
            AssignDelegate(out Delegates.glCreateShaderProgramEXT, "glCreateShaderProgramEXT");
            AssignDelegate(out Delegates.glCreateShaderProgramvEXT, "glCreateShaderProgramvEXT");
            AssignDelegate(out Delegates.glDeleteMemoryObjectsEXT, "glDeleteMemoryObjectsEXT");
            AssignDelegate(out Delegates.glDeleteProgramPipelinesEXT, "glDeleteProgramPipelinesEXT");
            AssignDelegate(out Delegates.glDeleteQueriesEXT, "glDeleteQueriesEXT");
            AssignDelegate(out Delegates.glDeleteSemaphoresEXT, "glDeleteSemaphoresEXT");
            AssignDelegate(out Delegates.glDisableiEXT, "glDisableiEXT");
            AssignDelegate(out Delegates.glDiscardFramebufferEXT, "glDiscardFramebufferEXT");
            AssignDelegate(out Delegates.glDrawArraysInstancedBaseInstanceEXT, "glDrawArraysInstancedBaseInstanceEXT");
            AssignDelegate(out Delegates.glDrawArraysInstancedEXT, "glDrawArraysInstancedEXT");
            AssignDelegate(out Delegates.glDrawBuffersEXT, "glDrawBuffersEXT");
            AssignDelegate(out Delegates.glDrawBuffersIndexedEXT, "glDrawBuffersIndexedEXT");
            AssignDelegate(out Delegates.glDrawElementsBaseVertexEXT, "glDrawElementsBaseVertexEXT");
            AssignDelegate(out Delegates.glDrawElementsInstancedBaseInstanceEXT, "glDrawElementsInstancedBaseInstanceEXT");
            AssignDelegate(out Delegates.glDrawElementsInstancedBaseVertexBaseInstanceEXT, "glDrawElementsInstancedBaseVertexBaseInstanceEXT");
            AssignDelegate(out Delegates.glDrawElementsInstancedBaseVertexEXT, "glDrawElementsInstancedBaseVertexEXT");
            AssignDelegate(out Delegates.glDrawElementsInstancedEXT, "glDrawElementsInstancedEXT");
            AssignDelegate(out Delegates.glDrawRangeElementsBaseVertexEXT, "glDrawRangeElementsBaseVertexEXT");
            AssignDelegate(out Delegates.glDrawTransformFeedbackEXT, "glDrawTransformFeedbackEXT");
            AssignDelegate(out Delegates.glDrawTransformFeedbackInstancedEXT, "glDrawTransformFeedbackInstancedEXT");
            AssignDelegate(out Delegates.glEnableiEXT, "glEnableiEXT");
            AssignDelegate(out Delegates.glEndQueryEXT, "glEndQueryEXT");
            AssignDelegate(out Delegates.glFlushMappedBufferRangeEXT, "glFlushMappedBufferRangeEXT");
            AssignDelegate(out Delegates.glFramebufferPixelLocalStorageSizeEXT, "glFramebufferPixelLocalStorageSizeEXT");
            AssignDelegate(out Delegates.glFramebufferTexture2DMultisampleEXT, "glFramebufferTexture2DMultisampleEXT");
            AssignDelegate(out Delegates.glFramebufferTextureEXT, "glFramebufferTextureEXT");
            AssignDelegate(out Delegates.glGenProgramPipelinesEXT, "glGenProgramPipelinesEXT");
            AssignDelegate(out Delegates.glGenQueriesEXT, "glGenQueriesEXT");
            AssignDelegate(out Delegates.glGenSemaphoresEXT, "glGenSemaphoresEXT");
            AssignDelegate(out Delegates.glGetFragDataIndexEXT, "glGetFragDataIndexEXT");
            AssignDelegate(out Delegates.glGetFramebufferPixelLocalStorageSizeEXT, "glGetFramebufferPixelLocalStorageSizeEXT");
            AssignDelegate(out Delegates.glGetGraphicsResetStatusEXT, "glGetGraphicsResetStatusEXT");
            AssignDelegate(out Delegates.glGetIntegeri_vEXT, "glGetIntegeri_vEXT");
            AssignDelegate(out Delegates.glGetMemoryObjectParameterivEXT, "glGetMemoryObjectParameterivEXT");
            AssignDelegate(out Delegates.glGetnUniformfvEXT, "glGetnUniformfvEXT");
            AssignDelegate(out Delegates.glGetnUniformivEXT, "glGetnUniformivEXT");
            AssignDelegate(out Delegates.glGetObjectLabelEXT, "glGetObjectLabelEXT");
            AssignDelegate(out Delegates.glGetProgramPipelineInfoLogEXT, "glGetProgramPipelineInfoLogEXT");
            AssignDelegate(out Delegates.glGetProgramPipelineivEXT, "glGetProgramPipelineivEXT");
            AssignDelegate(out Delegates.glGetProgramResourceLocationIndexEXT, "glGetProgramResourceLocationIndexEXT");
            AssignDelegate(out Delegates.glGetQueryivEXT, "glGetQueryivEXT");
            AssignDelegate(out Delegates.glGetQueryObjecti64vEXT, "glGetQueryObjecti64vEXT");
            AssignDelegate(out Delegates.glGetQueryObjectivEXT, "glGetQueryObjectivEXT");
            AssignDelegate(out Delegates.glGetQueryObjectui64vEXT, "glGetQueryObjectui64vEXT");
            AssignDelegate(out Delegates.glGetQueryObjectuivEXT, "glGetQueryObjectuivEXT");
            AssignDelegate(out Delegates.glGetSamplerParameterIivEXT, "glGetSamplerParameterIivEXT");
            AssignDelegate(out Delegates.glGetSamplerParameterIuivEXT, "glGetSamplerParameterIuivEXT");
            AssignDelegate(out Delegates.glGetSemaphoreParameterui64vEXT, "glGetSemaphoreParameterui64vEXT");
            AssignDelegate(out Delegates.glGetTexParameterIivEXT, "glGetTexParameterIivEXT");
            AssignDelegate(out Delegates.glGetTexParameterIuivEXT, "glGetTexParameterIuivEXT");
            AssignDelegate(out Delegates.glGetUnsignedBytei_vEXT, "glGetUnsignedBytei_vEXT");
            AssignDelegate(out Delegates.glGetUnsignedBytevEXT, "glGetUnsignedBytevEXT");
            AssignDelegate(out Delegates.glImportMemoryFdEXT, "glImportMemoryFdEXT");
            AssignDelegate(out Delegates.glImportMemoryWin32HandleEXT, "glImportMemoryWin32HandleEXT");
            AssignDelegate(out Delegates.glImportMemoryWin32NameEXT, "glImportMemoryWin32NameEXT");
            AssignDelegate(out Delegates.glImportSemaphoreFdEXT, "glImportSemaphoreFdEXT");
            AssignDelegate(out Delegates.glImportSemaphoreWin32HandleEXT, "glImportSemaphoreWin32HandleEXT");
            AssignDelegate(out Delegates.glImportSemaphoreWin32NameEXT, "glImportSemaphoreWin32NameEXT");
            AssignDelegate(out Delegates.glInsertEventMarkerEXT, "glInsertEventMarkerEXT");
            AssignDelegate(out Delegates.glIsEnablediEXT, "glIsEnablediEXT");
            AssignDelegate(out Delegates.glIsMemoryObjectEXT, "glIsMemoryObjectEXT");
            AssignDelegate(out Delegates.glIsProgramPipelineEXT, "glIsProgramPipelineEXT");
            AssignDelegate(out Delegates.glIsQueryEXT, "glIsQueryEXT");
            AssignDelegate(out Delegates.glIsSemaphoreEXT, "glIsSemaphoreEXT");
            AssignDelegate(out Delegates.glLabelObjectEXT, "glLabelObjectEXT");
            AssignDelegate(out Delegates.glMapBufferRangeEXT, "glMapBufferRangeEXT");
            AssignDelegate(out Delegates.glMatrixFrustumEXT, "glMatrixFrustumEXT");
            AssignDelegate(out Delegates.glMatrixLoaddEXT, "glMatrixLoaddEXT");
            AssignDelegate(out Delegates.glMatrixLoadfEXT, "glMatrixLoadfEXT");
            AssignDelegate(out Delegates.glMatrixLoadIdentityEXT, "glMatrixLoadIdentityEXT");
            AssignDelegate(out Delegates.glMatrixLoadTransposedEXT, "glMatrixLoadTransposedEXT");
            AssignDelegate(out Delegates.glMatrixLoadTransposefEXT, "glMatrixLoadTransposefEXT");
            AssignDelegate(out Delegates.glMatrixMultdEXT, "glMatrixMultdEXT");
            AssignDelegate(out Delegates.glMatrixMultfEXT, "glMatrixMultfEXT");
            AssignDelegate(out Delegates.glMatrixMultTransposedEXT, "glMatrixMultTransposedEXT");
            AssignDelegate(out Delegates.glMatrixMultTransposefEXT, "glMatrixMultTransposefEXT");
            AssignDelegate(out Delegates.glMatrixOrthoEXT, "glMatrixOrthoEXT");
            AssignDelegate(out Delegates.glMatrixPopEXT, "glMatrixPopEXT");
            AssignDelegate(out Delegates.glMatrixPushEXT, "glMatrixPushEXT");
            AssignDelegate(out Delegates.glMatrixRotatedEXT, "glMatrixRotatedEXT");
            AssignDelegate(out Delegates.glMatrixRotatefEXT, "glMatrixRotatefEXT");
            AssignDelegate(out Delegates.glMatrixScaledEXT, "glMatrixScaledEXT");
            AssignDelegate(out Delegates.glMatrixScalefEXT, "glMatrixScalefEXT");
            AssignDelegate(out Delegates.glMatrixTranslatedEXT, "glMatrixTranslatedEXT");
            AssignDelegate(out Delegates.glMatrixTranslatefEXT, "glMatrixTranslatefEXT");
            AssignDelegate(out Delegates.glMemoryObjectParameterivEXT, "glMemoryObjectParameterivEXT");
            AssignDelegate(out Delegates.glMultiDrawArraysEXT, "glMultiDrawArraysEXT");
            AssignDelegate(out Delegates.glMultiDrawArraysIndirectEXT, "glMultiDrawArraysIndirectEXT");
            AssignDelegate(out Delegates.glMultiDrawElementsBaseVertexEXT, "glMultiDrawElementsBaseVertexEXT");
            AssignDelegate(out Delegates.glMultiDrawElementsEXT, "glMultiDrawElementsEXT");
            AssignDelegate(out Delegates.glMultiDrawElementsIndirectEXT, "glMultiDrawElementsIndirectEXT");
            AssignDelegate(out Delegates.glNamedBufferStorageExternalEXT, "glNamedBufferStorageExternalEXT");
            AssignDelegate(out Delegates.glNamedBufferStorageMemEXT, "glNamedBufferStorageMemEXT");
            AssignDelegate(out Delegates.glPatchParameteriEXT, "glPatchParameteriEXT");
            AssignDelegate(out Delegates.glPolygonOffsetClampEXT, "glPolygonOffsetClampEXT");
            AssignDelegate(out Delegates.glPopGroupMarkerEXT, "glPopGroupMarkerEXT");
            AssignDelegate(out Delegates.glPrimitiveBoundingBoxEXT, "glPrimitiveBoundingBoxEXT");
            AssignDelegate(out Delegates.glProgramParameteriEXT, "glProgramParameteriEXT");
            AssignDelegate(out Delegates.glProgramUniform1fEXT, "glProgramUniform1fEXT");
            AssignDelegate(out Delegates.glProgramUniform1fvEXT, "glProgramUniform1fvEXT");
            AssignDelegate(out Delegates.glProgramUniform1iEXT, "glProgramUniform1iEXT");
            AssignDelegate(out Delegates.glProgramUniform1ivEXT, "glProgramUniform1ivEXT");
            AssignDelegate(out Delegates.glProgramUniform1uiEXT, "glProgramUniform1uiEXT");
            AssignDelegate(out Delegates.glProgramUniform1uivEXT, "glProgramUniform1uivEXT");
            AssignDelegate(out Delegates.glProgramUniform2fEXT, "glProgramUniform2fEXT");
            AssignDelegate(out Delegates.glProgramUniform2fvEXT, "glProgramUniform2fvEXT");
            AssignDelegate(out Delegates.glProgramUniform2iEXT, "glProgramUniform2iEXT");
            AssignDelegate(out Delegates.glProgramUniform2ivEXT, "glProgramUniform2ivEXT");
            AssignDelegate(out Delegates.glProgramUniform2uiEXT, "glProgramUniform2uiEXT");
            AssignDelegate(out Delegates.glProgramUniform2uivEXT, "glProgramUniform2uivEXT");
            AssignDelegate(out Delegates.glProgramUniform3fEXT, "glProgramUniform3fEXT");
            AssignDelegate(out Delegates.glProgramUniform3fvEXT, "glProgramUniform3fvEXT");
            AssignDelegate(out Delegates.glProgramUniform3iEXT, "glProgramUniform3iEXT");
            AssignDelegate(out Delegates.glProgramUniform3ivEXT, "glProgramUniform3ivEXT");
            AssignDelegate(out Delegates.glProgramUniform3uiEXT, "glProgramUniform3uiEXT");
            AssignDelegate(out Delegates.glProgramUniform3uivEXT, "glProgramUniform3uivEXT");
            AssignDelegate(out Delegates.glProgramUniform4fEXT, "glProgramUniform4fEXT");
            AssignDelegate(out Delegates.glProgramUniform4fvEXT, "glProgramUniform4fvEXT");
            AssignDelegate(out Delegates.glProgramUniform4iEXT, "glProgramUniform4iEXT");
            AssignDelegate(out Delegates.glProgramUniform4ivEXT, "glProgramUniform4ivEXT");
            AssignDelegate(out Delegates.glProgramUniform4uiEXT, "glProgramUniform4uiEXT");
            AssignDelegate(out Delegates.glProgramUniform4uivEXT, "glProgramUniform4uivEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix2fvEXT, "glProgramUniformMatrix2fvEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix2x3fvEXT, "glProgramUniformMatrix2x3fvEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix2x4fvEXT, "glProgramUniformMatrix2x4fvEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix3fvEXT, "glProgramUniformMatrix3fvEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix3x2fvEXT, "glProgramUniformMatrix3x2fvEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix3x4fvEXT, "glProgramUniformMatrix3x4fvEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix4fvEXT, "glProgramUniformMatrix4fvEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix4x2fvEXT, "glProgramUniformMatrix4x2fvEXT");
            AssignDelegate(out Delegates.glProgramUniformMatrix4x3fvEXT, "glProgramUniformMatrix4x3fvEXT");
            AssignDelegate(out Delegates.glPushGroupMarkerEXT, "glPushGroupMarkerEXT");
            AssignDelegate(out Delegates.glQueryCounterEXT, "glQueryCounterEXT");
            AssignDelegate(out Delegates.glRasterSamplesEXT, "glRasterSamplesEXT");
            AssignDelegate(out Delegates.glReadBufferIndexedEXT, "glReadBufferIndexedEXT");
            AssignDelegate(out Delegates.glReadnPixelsEXT, "glReadnPixelsEXT");
            AssignDelegate(out Delegates.glReleaseKeyedMutexWin32EXT, "glReleaseKeyedMutexWin32EXT");
            AssignDelegate(out Delegates.glRenderbufferStorageMultisampleEXT, "glRenderbufferStorageMultisampleEXT");
            AssignDelegate(out Delegates.glSamplerParameterIivEXT, "glSamplerParameterIivEXT");
            AssignDelegate(out Delegates.glSamplerParameterIuivEXT, "glSamplerParameterIuivEXT");
            AssignDelegate(out Delegates.glSemaphoreParameterui64vEXT, "glSemaphoreParameterui64vEXT");
            AssignDelegate(out Delegates.glSignalSemaphoreEXT, "glSignalSemaphoreEXT");
            AssignDelegate(out Delegates.glTexBufferEXT, "glTexBufferEXT");
            AssignDelegate(out Delegates.glTexBufferRangeEXT, "glTexBufferRangeEXT");
            AssignDelegate(out Delegates.glTexPageCommitmentEXT, "glTexPageCommitmentEXT");
            AssignDelegate(out Delegates.glTexParameterIivEXT, "glTexParameterIivEXT");
            AssignDelegate(out Delegates.glTexParameterIuivEXT, "glTexParameterIuivEXT");
            AssignDelegate(out Delegates.glTexStorage1DEXT, "glTexStorage1DEXT");
            AssignDelegate(out Delegates.glTexStorage2DEXT, "glTexStorage2DEXT");
            AssignDelegate(out Delegates.glTexStorage3DEXT, "glTexStorage3DEXT");
            AssignDelegate(out Delegates.glTexStorageMem1DEXT, "glTexStorageMem1DEXT");
            AssignDelegate(out Delegates.glTexStorageMem2DEXT, "glTexStorageMem2DEXT");
            AssignDelegate(out Delegates.glTexStorageMem2DMultisampleEXT, "glTexStorageMem2DMultisampleEXT");
            AssignDelegate(out Delegates.glTexStorageMem3DEXT, "glTexStorageMem3DEXT");
            AssignDelegate(out Delegates.glTexStorageMem3DMultisampleEXT, "glTexStorageMem3DMultisampleEXT");
            AssignDelegate(out Delegates.glTextureStorage1DEXT, "glTextureStorage1DEXT");
            AssignDelegate(out Delegates.glTextureStorage2DEXT, "glTextureStorage2DEXT");
            AssignDelegate(out Delegates.glTextureStorage3DEXT, "glTextureStorage3DEXT");
            AssignDelegate(out Delegates.glTextureStorageMem1DEXT, "glTextureStorageMem1DEXT");
            AssignDelegate(out Delegates.glTextureStorageMem2DEXT, "glTextureStorageMem2DEXT");
            AssignDelegate(out Delegates.glTextureStorageMem2DMultisampleEXT, "glTextureStorageMem2DMultisampleEXT");
            AssignDelegate(out Delegates.glTextureStorageMem3DEXT, "glTextureStorageMem3DEXT");
            AssignDelegate(out Delegates.glTextureStorageMem3DMultisampleEXT, "glTextureStorageMem3DMultisampleEXT");
            AssignDelegate(out Delegates.glTextureViewEXT, "glTextureViewEXT");
            AssignDelegate(out Delegates.glUseProgramStagesEXT, "glUseProgramStagesEXT");
            AssignDelegate(out Delegates.glUseShaderProgramEXT, "glUseShaderProgramEXT");
            AssignDelegate(out Delegates.glValidateProgramPipelineEXT, "glValidateProgramPipelineEXT");
            AssignDelegate(out Delegates.glVertexAttribDivisorEXT, "glVertexAttribDivisorEXT");
            AssignDelegate(out Delegates.glWaitSemaphoreEXT, "glWaitSemaphoreEXT");
            AssignDelegate(out Delegates.glWindowRectanglesEXT, "glWindowRectanglesEXT");
            AssignDelegate(out Delegates.glFramebufferTexture2DDownsampleIMG, "glFramebufferTexture2DDownsampleIMG");
            AssignDelegate(out Delegates.glFramebufferTexture2DMultisampleIMG, "glFramebufferTexture2DMultisampleIMG");
            AssignDelegate(out Delegates.glFramebufferTextureLayerDownsampleIMG, "glFramebufferTextureLayerDownsampleIMG");
            AssignDelegate(out Delegates.glGetTextureHandleIMG, "glGetTextureHandleIMG");
            AssignDelegate(out Delegates.glGetTextureSamplerHandleIMG, "glGetTextureSamplerHandleIMG");
            AssignDelegate(out Delegates.glProgramUniformHandleui64IMG, "glProgramUniformHandleui64IMG");
            AssignDelegate(out Delegates.glProgramUniformHandleui64vIMG, "glProgramUniformHandleui64vIMG");
            AssignDelegate(out Delegates.glRenderbufferStorageMultisampleIMG, "glRenderbufferStorageMultisampleIMG");
            AssignDelegate(out Delegates.glUniformHandleui64IMG, "glUniformHandleui64IMG");
            AssignDelegate(out Delegates.glUniformHandleui64vIMG, "glUniformHandleui64vIMG");
            AssignDelegate(out Delegates.glBeginPerfQueryINTEL, "glBeginPerfQueryINTEL");
            AssignDelegate(out Delegates.glCreatePerfQueryINTEL, "glCreatePerfQueryINTEL");
            AssignDelegate(out Delegates.glDeletePerfQueryINTEL, "glDeletePerfQueryINTEL");
            AssignDelegate(out Delegates.glEndPerfQueryINTEL, "glEndPerfQueryINTEL");
            AssignDelegate(out Delegates.glGetFirstPerfQueryIdINTEL, "glGetFirstPerfQueryIdINTEL");
            AssignDelegate(out Delegates.glGetNextPerfQueryIdINTEL, "glGetNextPerfQueryIdINTEL");
            AssignDelegate(out Delegates.glGetPerfCounterInfoINTEL, "glGetPerfCounterInfoINTEL");
            AssignDelegate(out Delegates.glGetPerfQueryDataINTEL, "glGetPerfQueryDataINTEL");
            AssignDelegate(out Delegates.glGetPerfQueryIdByNameINTEL, "glGetPerfQueryIdByNameINTEL");
            AssignDelegate(out Delegates.glGetPerfQueryInfoINTEL, "glGetPerfQueryInfoINTEL");
            AssignDelegate(out Delegates.glBlendBarrierKHR, "glBlendBarrierKHR");
            AssignDelegate(out Delegates.glDebugMessageCallbackKHR, "glDebugMessageCallbackKHR");
            AssignDelegate(out Delegates.glDebugMessageControlKHR, "glDebugMessageControlKHR");
            AssignDelegate(out Delegates.glDebugMessageInsertKHR, "glDebugMessageInsertKHR");
            AssignDelegate(out Delegates.glGetDebugMessageLogKHR, "glGetDebugMessageLogKHR");
            AssignDelegate(out Delegates.glGetGraphicsResetStatusKHR, "glGetGraphicsResetStatusKHR");
            AssignDelegate(out Delegates.glGetnUniformfvKHR, "glGetnUniformfvKHR");
            AssignDelegate(out Delegates.glGetnUniformivKHR, "glGetnUniformivKHR");
            AssignDelegate(out Delegates.glGetnUniformuivKHR, "glGetnUniformuivKHR");
            AssignDelegate(out Delegates.glGetObjectLabelKHR, "glGetObjectLabelKHR");
            AssignDelegate(out Delegates.glGetObjectPtrLabelKHR, "glGetObjectPtrLabelKHR");
            AssignDelegate(out Delegates.glGetPointervKHR, "glGetPointervKHR");
            AssignDelegate(out Delegates.glMaxShaderCompilerThreadsKHR, "glMaxShaderCompilerThreadsKHR");
            AssignDelegate(out Delegates.glObjectLabelKHR, "glObjectLabelKHR");
            AssignDelegate(out Delegates.glObjectPtrLabelKHR, "glObjectPtrLabelKHR");
            AssignDelegate(out Delegates.glPopDebugGroupKHR, "glPopDebugGroupKHR");
            AssignDelegate(out Delegates.glPushDebugGroupKHR, "glPushDebugGroupKHR");
            AssignDelegate(out Delegates.glReadnPixelsKHR, "glReadnPixelsKHR");
            AssignDelegate(out Delegates.glBeginConditionalRenderNV, "glBeginConditionalRenderNV");
            AssignDelegate(out Delegates.glBlendBarrierNV, "glBlendBarrierNV");
            AssignDelegate(out Delegates.glBlendParameteriNV, "glBlendParameteriNV");
            AssignDelegate(out Delegates.glBlitFramebufferNV, "glBlitFramebufferNV");
            AssignDelegate(out Delegates.glConservativeRasterParameteriNV, "glConservativeRasterParameteriNV");
            AssignDelegate(out Delegates.glCopyBufferSubDataNV, "glCopyBufferSubDataNV");
            AssignDelegate(out Delegates.glCopyPathNV, "glCopyPathNV");
            AssignDelegate(out Delegates.glCoverageMaskNV, "glCoverageMaskNV");
            AssignDelegate(out Delegates.glCoverageModulationNV, "glCoverageModulationNV");
            AssignDelegate(out Delegates.glCoverageModulationTableNV, "glCoverageModulationTableNV");
            AssignDelegate(out Delegates.glCoverageOperationNV, "glCoverageOperationNV");
            AssignDelegate(out Delegates.glCoverFillPathInstancedNV, "glCoverFillPathInstancedNV");
            AssignDelegate(out Delegates.glCoverFillPathNV, "glCoverFillPathNV");
            AssignDelegate(out Delegates.glCoverStrokePathInstancedNV, "glCoverStrokePathInstancedNV");
            AssignDelegate(out Delegates.glCoverStrokePathNV, "glCoverStrokePathNV");
            AssignDelegate(out Delegates.glDeleteFencesNV, "glDeleteFencesNV");
            AssignDelegate(out Delegates.glDeletePathsNV, "glDeletePathsNV");
            AssignDelegate(out Delegates.glDepthRangeArrayfvNV, "glDepthRangeArrayfvNV");
            AssignDelegate(out Delegates.glDepthRangeIndexedfNV, "glDepthRangeIndexedfNV");
            AssignDelegate(out Delegates.glDisableiNV, "glDisableiNV");
            AssignDelegate(out Delegates.glDrawArraysInstancedNV, "glDrawArraysInstancedNV");
            AssignDelegate(out Delegates.glDrawBuffersNV, "glDrawBuffersNV");
            AssignDelegate(out Delegates.glDrawElementsInstancedNV, "glDrawElementsInstancedNV");
            AssignDelegate(out Delegates.glDrawVkImageNV, "glDrawVkImageNV");
            AssignDelegate(out Delegates.glEnableiNV, "glEnableiNV");
            AssignDelegate(out Delegates.glEndConditionalRenderNV, "glEndConditionalRenderNV");
            AssignDelegate(out Delegates.glFinishFenceNV, "glFinishFenceNV");
            AssignDelegate(out Delegates.glFragmentCoverageColorNV, "glFragmentCoverageColorNV");
            AssignDelegate(out Delegates.glFramebufferSampleLocationsfvNV, "glFramebufferSampleLocationsfvNV");
            AssignDelegate(out Delegates.glGenFencesNV, "glGenFencesNV");
            AssignDelegate(out Delegates.glGenPathsNV, "glGenPathsNV");
            AssignDelegate(out Delegates.glGetCoverageModulationTableNV, "glGetCoverageModulationTableNV");
            AssignDelegate(out Delegates.glGetFenceivNV, "glGetFenceivNV");
            AssignDelegate(out Delegates.glGetFloati_vNV, "glGetFloati_vNV");
            AssignDelegate(out Delegates.glGetImageHandleNV, "glGetImageHandleNV");
            AssignDelegate(out Delegates.glGetInternalformatSampleivNV, "glGetInternalformatSampleivNV");
            AssignDelegate(out Delegates.glGetPathColorGenfvNV, "glGetPathColorGenfvNV");
            AssignDelegate(out Delegates.glGetPathColorGenivNV, "glGetPathColorGenivNV");
            AssignDelegate(out Delegates.glGetPathCommandsNV, "glGetPathCommandsNV");
            AssignDelegate(out Delegates.glGetPathCoordsNV, "glGetPathCoordsNV");
            AssignDelegate(out Delegates.glGetPathDashArrayNV, "glGetPathDashArrayNV");
            AssignDelegate(out Delegates.glGetPathLengthNV, "glGetPathLengthNV");
            AssignDelegate(out Delegates.glGetPathMetricRangeNV, "glGetPathMetricRangeNV");
            AssignDelegate(out Delegates.glGetPathMetricsNV, "glGetPathMetricsNV");
            AssignDelegate(out Delegates.glGetPathParameterfvNV, "glGetPathParameterfvNV");
            AssignDelegate(out Delegates.glGetPathParameterivNV, "glGetPathParameterivNV");
            AssignDelegate(out Delegates.glGetPathSpacingNV, "glGetPathSpacingNV");
            AssignDelegate(out Delegates.glGetPathTexGenfvNV, "glGetPathTexGenfvNV");
            AssignDelegate(out Delegates.glGetPathTexGenivNV, "glGetPathTexGenivNV");
            AssignDelegate(out Delegates.glGetProgramResourcefvNV, "glGetProgramResourcefvNV");
            AssignDelegate(out Delegates.glGetTextureHandleNV, "glGetTextureHandleNV");
            AssignDelegate(out Delegates.glGetTextureSamplerHandleNV, "glGetTextureSamplerHandleNV");
            AssignDelegate(out Delegates.glGetUniformi64vNV, "glGetUniformi64vNV");
            AssignDelegate(out Delegates.glGetVkProcAddrNV, "glGetVkProcAddrNV");
            AssignDelegate(out Delegates.glInterpolatePathsNV, "glInterpolatePathsNV");
            AssignDelegate(out Delegates.glIsEnablediNV, "glIsEnablediNV");
            AssignDelegate(out Delegates.glIsFenceNV, "glIsFenceNV");
            AssignDelegate(out Delegates.glIsImageHandleResidentNV, "glIsImageHandleResidentNV");
            AssignDelegate(out Delegates.glIsPathNV, "glIsPathNV");
            AssignDelegate(out Delegates.glIsPointInFillPathNV, "glIsPointInFillPathNV");
            AssignDelegate(out Delegates.glIsPointInStrokePathNV, "glIsPointInStrokePathNV");
            AssignDelegate(out Delegates.glIsTextureHandleResidentNV, "glIsTextureHandleResidentNV");
            AssignDelegate(out Delegates.glMakeImageHandleNonResidentNV, "glMakeImageHandleNonResidentNV");
            AssignDelegate(out Delegates.glMakeImageHandleResidentNV, "glMakeImageHandleResidentNV");
            AssignDelegate(out Delegates.glMakeTextureHandleNonResidentNV, "glMakeTextureHandleNonResidentNV");
            AssignDelegate(out Delegates.glMakeTextureHandleResidentNV, "glMakeTextureHandleResidentNV");
            AssignDelegate(out Delegates.glMatrixLoad3x2fNV, "glMatrixLoad3x2fNV");
            AssignDelegate(out Delegates.glMatrixLoad3x3fNV, "glMatrixLoad3x3fNV");
            AssignDelegate(out Delegates.glMatrixLoadTranspose3x3fNV, "glMatrixLoadTranspose3x3fNV");
            AssignDelegate(out Delegates.glMatrixMult3x2fNV, "glMatrixMult3x2fNV");
            AssignDelegate(out Delegates.glMatrixMult3x3fNV, "glMatrixMult3x3fNV");
            AssignDelegate(out Delegates.glMatrixMultTranspose3x3fNV, "glMatrixMultTranspose3x3fNV");
            AssignDelegate(out Delegates.glNamedFramebufferSampleLocationsfvNV, "glNamedFramebufferSampleLocationsfvNV");
            AssignDelegate(out Delegates.glPathColorGenNV, "glPathColorGenNV");
            AssignDelegate(out Delegates.glPathCommandsNV, "glPathCommandsNV");
            AssignDelegate(out Delegates.glPathCoordsNV, "glPathCoordsNV");
            AssignDelegate(out Delegates.glPathCoverDepthFuncNV, "glPathCoverDepthFuncNV");
            AssignDelegate(out Delegates.glPathDashArrayNV, "glPathDashArrayNV");
            AssignDelegate(out Delegates.glPathFogGenNV, "glPathFogGenNV");
            AssignDelegate(out Delegates.glPathGlyphIndexArrayNV, "glPathGlyphIndexArrayNV");
            AssignDelegate(out Delegates.glPathGlyphIndexRangeNV, "glPathGlyphIndexRangeNV");
            AssignDelegate(out Delegates.glPathGlyphRangeNV, "glPathGlyphRangeNV");
            AssignDelegate(out Delegates.glPathGlyphsNV, "glPathGlyphsNV");
            AssignDelegate(out Delegates.glPathMemoryGlyphIndexArrayNV, "glPathMemoryGlyphIndexArrayNV");
            AssignDelegate(out Delegates.glPathParameterfNV, "glPathParameterfNV");
            AssignDelegate(out Delegates.glPathParameterfvNV, "glPathParameterfvNV");
            AssignDelegate(out Delegates.glPathParameteriNV, "glPathParameteriNV");
            AssignDelegate(out Delegates.glPathParameterivNV, "glPathParameterivNV");
            AssignDelegate(out Delegates.glPathStencilDepthOffsetNV, "glPathStencilDepthOffsetNV");
            AssignDelegate(out Delegates.glPathStencilFuncNV, "glPathStencilFuncNV");
            AssignDelegate(out Delegates.glPathStringNV, "glPathStringNV");
            AssignDelegate(out Delegates.glPathSubCommandsNV, "glPathSubCommandsNV");
            AssignDelegate(out Delegates.glPathSubCoordsNV, "glPathSubCoordsNV");
            AssignDelegate(out Delegates.glPathTexGenNV, "glPathTexGenNV");
            AssignDelegate(out Delegates.glPointAlongPathNV, "glPointAlongPathNV");
            AssignDelegate(out Delegates.glPolygonModeNV, "glPolygonModeNV");
            AssignDelegate(out Delegates.glProgramPathFragmentInputGenNV, "glProgramPathFragmentInputGenNV");
            AssignDelegate(out Delegates.glProgramUniform1i64NV, "glProgramUniform1i64NV");
            AssignDelegate(out Delegates.glProgramUniform1i64vNV, "glProgramUniform1i64vNV");
            AssignDelegate(out Delegates.glProgramUniform1ui64NV, "glProgramUniform1ui64NV");
            AssignDelegate(out Delegates.glProgramUniform1ui64vNV, "glProgramUniform1ui64vNV");
            AssignDelegate(out Delegates.glProgramUniform2i64NV, "glProgramUniform2i64NV");
            AssignDelegate(out Delegates.glProgramUniform2i64vNV, "glProgramUniform2i64vNV");
            AssignDelegate(out Delegates.glProgramUniform2ui64NV, "glProgramUniform2ui64NV");
            AssignDelegate(out Delegates.glProgramUniform2ui64vNV, "glProgramUniform2ui64vNV");
            AssignDelegate(out Delegates.glProgramUniform3i64NV, "glProgramUniform3i64NV");
            AssignDelegate(out Delegates.glProgramUniform3i64vNV, "glProgramUniform3i64vNV");
            AssignDelegate(out Delegates.glProgramUniform3ui64NV, "glProgramUniform3ui64NV");
            AssignDelegate(out Delegates.glProgramUniform3ui64vNV, "glProgramUniform3ui64vNV");
            AssignDelegate(out Delegates.glProgramUniform4i64NV, "glProgramUniform4i64NV");
            AssignDelegate(out Delegates.glProgramUniform4i64vNV, "glProgramUniform4i64vNV");
            AssignDelegate(out Delegates.glProgramUniform4ui64NV, "glProgramUniform4ui64NV");
            AssignDelegate(out Delegates.glProgramUniform4ui64vNV, "glProgramUniform4ui64vNV");
            AssignDelegate(out Delegates.glProgramUniformHandleui64NV, "glProgramUniformHandleui64NV");
            AssignDelegate(out Delegates.glProgramUniformHandleui64vNV, "glProgramUniformHandleui64vNV");
            AssignDelegate(out Delegates.glReadBufferNV, "glReadBufferNV");
            AssignDelegate(out Delegates.glRenderbufferStorageMultisampleNV, "glRenderbufferStorageMultisampleNV");
            AssignDelegate(out Delegates.glResolveDepthValuesNV, "glResolveDepthValuesNV");
            AssignDelegate(out Delegates.glScissorArrayvNV, "glScissorArrayvNV");
            AssignDelegate(out Delegates.glScissorIndexedNV, "glScissorIndexedNV");
            AssignDelegate(out Delegates.glScissorIndexedvNV, "glScissorIndexedvNV");
            AssignDelegate(out Delegates.glSetFenceNV, "glSetFenceNV");
            AssignDelegate(out Delegates.glSignalVkFenceNV, "glSignalVkFenceNV");
            AssignDelegate(out Delegates.glSignalVkSemaphoreNV, "glSignalVkSemaphoreNV");
            AssignDelegate(out Delegates.glStencilFillPathInstancedNV, "glStencilFillPathInstancedNV");
            AssignDelegate(out Delegates.glStencilFillPathNV, "glStencilFillPathNV");
            AssignDelegate(out Delegates.glStencilStrokePathInstancedNV, "glStencilStrokePathInstancedNV");
            AssignDelegate(out Delegates.glStencilStrokePathNV, "glStencilStrokePathNV");
            AssignDelegate(out Delegates.glStencilThenCoverFillPathInstancedNV, "glStencilThenCoverFillPathInstancedNV");
            AssignDelegate(out Delegates.glStencilThenCoverFillPathNV, "glStencilThenCoverFillPathNV");
            AssignDelegate(out Delegates.glStencilThenCoverStrokePathInstancedNV, "glStencilThenCoverStrokePathInstancedNV");
            AssignDelegate(out Delegates.glStencilThenCoverStrokePathNV, "glStencilThenCoverStrokePathNV");
            AssignDelegate(out Delegates.glSubpixelPrecisionBiasNV, "glSubpixelPrecisionBiasNV");
            AssignDelegate(out Delegates.glTestFenceNV, "glTestFenceNV");
            AssignDelegate(out Delegates.glTransformPathNV, "glTransformPathNV");
            AssignDelegate(out Delegates.glUniform1i64NV, "glUniform1i64NV");
            AssignDelegate(out Delegates.glUniform1i64vNV, "glUniform1i64vNV");
            AssignDelegate(out Delegates.glUniform1ui64NV, "glUniform1ui64NV");
            AssignDelegate(out Delegates.glUniform1ui64vNV, "glUniform1ui64vNV");
            AssignDelegate(out Delegates.glUniform2i64NV, "glUniform2i64NV");
            AssignDelegate(out Delegates.glUniform2i64vNV, "glUniform2i64vNV");
            AssignDelegate(out Delegates.glUniform2ui64NV, "glUniform2ui64NV");
            AssignDelegate(out Delegates.glUniform2ui64vNV, "glUniform2ui64vNV");
            AssignDelegate(out Delegates.glUniform3i64NV, "glUniform3i64NV");
            AssignDelegate(out Delegates.glUniform3i64vNV, "glUniform3i64vNV");
            AssignDelegate(out Delegates.glUniform3ui64NV, "glUniform3ui64NV");
            AssignDelegate(out Delegates.glUniform3ui64vNV, "glUniform3ui64vNV");
            AssignDelegate(out Delegates.glUniform4i64NV, "glUniform4i64NV");
            AssignDelegate(out Delegates.glUniform4i64vNV, "glUniform4i64vNV");
            AssignDelegate(out Delegates.glUniform4ui64NV, "glUniform4ui64NV");
            AssignDelegate(out Delegates.glUniform4ui64vNV, "glUniform4ui64vNV");
            AssignDelegate(out Delegates.glUniformHandleui64NV, "glUniformHandleui64NV");
            AssignDelegate(out Delegates.glUniformHandleui64vNV, "glUniformHandleui64vNV");
            AssignDelegate(out Delegates.glUniformMatrix2x3fvNV, "glUniformMatrix2x3fvNV");
            AssignDelegate(out Delegates.glUniformMatrix2x4fvNV, "glUniformMatrix2x4fvNV");
            AssignDelegate(out Delegates.glUniformMatrix3x2fvNV, "glUniformMatrix3x2fvNV");
            AssignDelegate(out Delegates.glUniformMatrix3x4fvNV, "glUniformMatrix3x4fvNV");
            AssignDelegate(out Delegates.glUniformMatrix4x2fvNV, "glUniformMatrix4x2fvNV");
            AssignDelegate(out Delegates.glUniformMatrix4x3fvNV, "glUniformMatrix4x3fvNV");
            AssignDelegate(out Delegates.glVertexAttribDivisorNV, "glVertexAttribDivisorNV");
            AssignDelegate(out Delegates.glViewportArrayvNV, "glViewportArrayvNV");
            AssignDelegate(out Delegates.glViewportIndexedfNV, "glViewportIndexedfNV");
            AssignDelegate(out Delegates.glViewportIndexedfvNV, "glViewportIndexedfvNV");
            AssignDelegate(out Delegates.glViewportPositionWScaleNV, "glViewportPositionWScaleNV");
            AssignDelegate(out Delegates.glViewportSwizzleNV, "glViewportSwizzleNV");
            AssignDelegate(out Delegates.glWaitVkSemaphoreNV, "glWaitVkSemaphoreNV");
            AssignDelegate(out Delegates.glWeightPathsNV, "glWeightPathsNV");
            AssignDelegate(out Delegates.glBindVertexArrayOES, "glBindVertexArrayOES");
            AssignDelegate(out Delegates.glBlendEquationiOES, "glBlendEquationiOES");
            AssignDelegate(out Delegates.glBlendEquationSeparateiOES, "glBlendEquationSeparateiOES");
            AssignDelegate(out Delegates.glBlendFunciOES, "glBlendFunciOES");
            AssignDelegate(out Delegates.glBlendFuncSeparateiOES, "glBlendFuncSeparateiOES");
            AssignDelegate(out Delegates.glColorMaskiOES, "glColorMaskiOES");
            AssignDelegate(out Delegates.glCompressedTexImage3DOES, "glCompressedTexImage3DOES");
            AssignDelegate(out Delegates.glCompressedTexSubImage3DOES, "glCompressedTexSubImage3DOES");
            AssignDelegate(out Delegates.glCopyImageSubDataOES, "glCopyImageSubDataOES");
            AssignDelegate(out Delegates.glCopyTexSubImage3DOES, "glCopyTexSubImage3DOES");
            AssignDelegate(out Delegates.glDeleteVertexArraysOES, "glDeleteVertexArraysOES");
            AssignDelegate(out Delegates.glDepthRangeArrayfvOES, "glDepthRangeArrayfvOES");
            AssignDelegate(out Delegates.glDepthRangeIndexedfOES, "glDepthRangeIndexedfOES");
            AssignDelegate(out Delegates.glDisableiOES, "glDisableiOES");
            AssignDelegate(out Delegates.glDrawElementsBaseVertexOES, "glDrawElementsBaseVertexOES");
            AssignDelegate(out Delegates.glDrawElementsInstancedBaseVertexOES, "glDrawElementsInstancedBaseVertexOES");
            AssignDelegate(out Delegates.glDrawRangeElementsBaseVertexOES, "glDrawRangeElementsBaseVertexOES");
            AssignDelegate(out Delegates.glEGLImageTargetRenderbufferStorageOES, "glEGLImageTargetRenderbufferStorageOES");
            AssignDelegate(out Delegates.glEGLImageTargetTexture2DOES, "glEGLImageTargetTexture2DOES");
            AssignDelegate(out Delegates.glEnableiOES, "glEnableiOES");
            AssignDelegate(out Delegates.glFramebufferTexture3DOES, "glFramebufferTexture3DOES");
            AssignDelegate(out Delegates.glFramebufferTextureOES, "glFramebufferTextureOES");
            AssignDelegate(out Delegates.glGenVertexArraysOES, "glGenVertexArraysOES");
            AssignDelegate(out Delegates.glGetBufferPointervOES, "glGetBufferPointervOES");
            AssignDelegate(out Delegates.glGetFloati_vOES, "glGetFloati_vOES");
            AssignDelegate(out Delegates.glGetProgramBinaryOES, "glGetProgramBinaryOES");
            AssignDelegate(out Delegates.glGetSamplerParameterIivOES, "glGetSamplerParameterIivOES");
            AssignDelegate(out Delegates.glGetSamplerParameterIuivOES, "glGetSamplerParameterIuivOES");
            AssignDelegate(out Delegates.glGetTexParameterIivOES, "glGetTexParameterIivOES");
            AssignDelegate(out Delegates.glGetTexParameterIuivOES, "glGetTexParameterIuivOES");
            AssignDelegate(out Delegates.glIsEnablediOES, "glIsEnablediOES");
            AssignDelegate(out Delegates.glIsVertexArrayOES, "glIsVertexArrayOES");
            AssignDelegate(out Delegates.glMapBufferOES, "glMapBufferOES");
            AssignDelegate(out Delegates.glMinSampleShadingOES, "glMinSampleShadingOES");
            AssignDelegate(out Delegates.glPatchParameteriOES, "glPatchParameteriOES");
            AssignDelegate(out Delegates.glPrimitiveBoundingBoxOES, "glPrimitiveBoundingBoxOES");
            AssignDelegate(out Delegates.glProgramBinaryOES, "glProgramBinaryOES");
            AssignDelegate(out Delegates.glSamplerParameterIivOES, "glSamplerParameterIivOES");
            AssignDelegate(out Delegates.glSamplerParameterIuivOES, "glSamplerParameterIuivOES");
            AssignDelegate(out Delegates.glScissorArrayvOES, "glScissorArrayvOES");
            AssignDelegate(out Delegates.glScissorIndexedOES, "glScissorIndexedOES");
            AssignDelegate(out Delegates.glScissorIndexedvOES, "glScissorIndexedvOES");
            AssignDelegate(out Delegates.glTexBufferOES, "glTexBufferOES");
            AssignDelegate(out Delegates.glTexBufferRangeOES, "glTexBufferRangeOES");
            AssignDelegate(out Delegates.glTexImage3DOES, "glTexImage3DOES");
            AssignDelegate(out Delegates.glTexParameterIivOES, "glTexParameterIivOES");
            AssignDelegate(out Delegates.glTexParameterIuivOES, "glTexParameterIuivOES");
            AssignDelegate(out Delegates.glTexStorage3DMultisampleOES, "glTexStorage3DMultisampleOES");
            AssignDelegate(out Delegates.glTexSubImage3DOES, "glTexSubImage3DOES");
            AssignDelegate(out Delegates.glTextureViewOES, "glTextureViewOES");
            AssignDelegate(out Delegates.glUnmapBufferOES, "glUnmapBufferOES");
            AssignDelegate(out Delegates.glViewportArrayvOES, "glViewportArrayvOES");
            AssignDelegate(out Delegates.glViewportIndexedfOES, "glViewportIndexedfOES");
            AssignDelegate(out Delegates.glViewportIndexedfvOES, "glViewportIndexedfvOES");
            AssignDelegate(out Delegates.glFramebufferTextureMultisampleMultiviewOVR, "glFramebufferTextureMultisampleMultiviewOVR");
            AssignDelegate(out Delegates.glFramebufferTextureMultiviewOVR, "glFramebufferTextureMultiviewOVR");
            AssignDelegate(out Delegates.glAlphaFuncQCOM, "glAlphaFuncQCOM");
            AssignDelegate(out Delegates.glDisableDriverControlQCOM, "glDisableDriverControlQCOM");
            AssignDelegate(out Delegates.glEnableDriverControlQCOM, "glEnableDriverControlQCOM");
            AssignDelegate(out Delegates.glEndTilingQCOM, "glEndTilingQCOM");
            AssignDelegate(out Delegates.glExtGetBufferPointervQCOM, "glExtGetBufferPointervQCOM");
            AssignDelegate(out Delegates.glExtGetBuffersQCOM, "glExtGetBuffersQCOM");
            AssignDelegate(out Delegates.glExtGetFramebuffersQCOM, "glExtGetFramebuffersQCOM");
            AssignDelegate(out Delegates.glExtGetProgramBinarySourceQCOM, "glExtGetProgramBinarySourceQCOM");
            AssignDelegate(out Delegates.glExtGetProgramsQCOM, "glExtGetProgramsQCOM");
            AssignDelegate(out Delegates.glExtGetRenderbuffersQCOM, "glExtGetRenderbuffersQCOM");
            AssignDelegate(out Delegates.glExtGetShadersQCOM, "glExtGetShadersQCOM");
            AssignDelegate(out Delegates.glExtGetTexLevelParameterivQCOM, "glExtGetTexLevelParameterivQCOM");
            AssignDelegate(out Delegates.glExtGetTexSubImageQCOM, "glExtGetTexSubImageQCOM");
            AssignDelegate(out Delegates.glExtGetTexturesQCOM, "glExtGetTexturesQCOM");
            AssignDelegate(out Delegates.glExtIsProgramBinaryQCOM, "glExtIsProgramBinaryQCOM");
            AssignDelegate(out Delegates.glExtTexObjectStateOverrideiQCOM, "glExtTexObjectStateOverrideiQCOM");
            AssignDelegate(out Delegates.glFramebufferFetchBarrierQCOM, "glFramebufferFetchBarrierQCOM");
            AssignDelegate(out Delegates.glFramebufferFoveationConfigQCOM, "glFramebufferFoveationConfigQCOM");
            AssignDelegate(out Delegates.glFramebufferFoveationParametersQCOM, "glFramebufferFoveationParametersQCOM");
            AssignDelegate(out Delegates.glGetDriverControlsQCOM, "glGetDriverControlsQCOM");
            AssignDelegate(out Delegates.glGetDriverControlStringQCOM, "glGetDriverControlStringQCOM");
            AssignDelegate(out Delegates.glStartTilingQCOM, "glStartTilingQCOM");
            AssignDelegate(out Delegates.glTextureFoveationParametersQCOM, "glTextureFoveationParametersQCOM");
        }
    }
}
