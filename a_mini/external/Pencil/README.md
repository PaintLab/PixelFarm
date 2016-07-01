Pencil.Gaming
=============
Pencil.Gaming is a gaming library for C#, providing support for OpenGL, GLFW, OpenAL and Lua. It's a stable, cross-platform, open-source (some prefer the term "free") alternative to libraries like XNA, which has pretty much died now, OpenTK, which hasn't been updated for about a year, and SharpDX, which is not cross-platform. A feature that Pencil.Gaming has over most other C# gaming libraries, is that users **do not need to install any redistributables besides Mono/.NET!** 

The OpenGL implementation is based on the OpenTK source code.

Functionality and stability
===========================
GLFW2
-----
| Platform       | OpenGL core     | OpenGL extensions | GLFW            | OpenAL    |
| --------------:|:---------------:|:-----------------:|:---------------:|:---------:|
| Linux 64-bit   | Stable          | Stable            | Stable          | Stable    |
| Linux 32-bit   | Stable          | Stable            | Stable          | Stable    |
| Windows 64-bit | Stable          | Stable            | Stable          | Stable    |
| Windows 32-bit | Stable          | Stable            | Stable          | Stable    |
| Mac OS X       | Stable          | Stable            | Stable          | Stable    |

GLFW3
-----
| Platform       | OpenGL core     | OpenGL extensions | GLFW            | OpenAL    |
| --------------:|:---------------:|:-----------------:|:---------------:|:---------:|
| Linux 64-bit   | Stable          | Stable            | Stable          | Stable    |
| Linux 32-bit   | Stable          | Stable            | **Broken**      | Stable    |
| Windows 64-bit | Stable          | Stable            | Stable          | Stable    |
| Windows 32-bit | Stable          | Stable            | Presumed Stable | Stable    |
| Mac OS X       | Stable          | Stable            | Stable*          | Stable    |

*Both 32 and 64-bit versions provided for Mac OS X, but mono is realistically only available for 32-bit, so those are recommended.

Building using Makefile
=======================
Some don't like monodevelop/visual studio, for those a GNU Makefile has been made available. To generate this makefile, run the script in the main directory:
```Bash
./genmakefile.sh
```
After that, a makefile is made, and you can choose from four different profiles:
- Compatibility/GLFW 2 (profile: `compat_glfw2`)
- Compatibility/GLFW 3 (profile: `compat_glfw3`)
- Core/GLFW 2 (profile: `core_glfw2`)
- Core/GLFW 3 (profile: `core_glfw3`)
Compatibility means that legacy OpenGL will be included in the binaries, core means that only core (non-legacy, "modern") OpenGL will be included in the binary.

So an example might be:
```Bash
antonijn@antonijn-desktop ~/Programming/C Sharp/Pencil.Gaming $ ./genmakefile.sh
Makefile generated successfully.
antonijn@antonijn-desktop ~/Programming/C Sharp/Pencil.Gaming $ make core_glfw3
mkdir "Pencil.Gaming/bin/Core - GLFW3" -p
cp -r "Pencil.Gaming/natives32-glfw3" "Pencil.Gaming/bin/Core - GLFW3"
cp -r "Pencil.Gaming/natives64-glfw3" "Pencil.Gaming/bin/Core - GLFW3/natives64"
mv "Pencil.Gaming/bin/Core - GLFW3/natives32-glfw3" "Pencil.Gaming/bin/Core - GLFW3/natives32"
cp "Pencil.Gaming/Pencil.Gaming.dll.config" "Pencil.Gaming/bin/Core - GLFW3/Pencil.Gaming.dll.config"
gmcs ./Pencil.Gaming/Audio/AL32.cs ./Pencil.Gaming/Audio/AlcManager.cs ./Pencil.Gaming/Audio/AL.cs ./Pencil.Gaming/Audio/ALUtils.cs ./Pencil.Gaming/Audio/ALEnums.cs ./Pencil.Gaming/Audio/ALDelegates.cs ./Pencil.Gaming/Audio/Alc32.cs ./Pencil.Gaming/Audio/Alc64.cs ./Pencil.Gaming/Audio/Sound.cs ./Pencil.Gaming/Audio/Listener.cs ./Pencil.Gaming/Audio/AL64.cs ./Pencil.Gaming/Graphics/GL.cs ./Pencil.Gaming/Graphics/GLCore.cs ./Pencil.Gaming/Graphics/GLUtils.cs ./Pencil.Gaming/Graphics/GlEnums.cs ./Pencil.Gaming/Graphics/GLDelegates.cs ./Pencil.Gaming/Graphics/AssetLoadException.cs ./Pencil.Gaming/Graphics/GLLoadException.cs ./Pencil.Gaming/Graphics/GLHelper.cs ./Pencil.Gaming/Graphics/Color4.cs ./Pencil.Gaming/AssemblyInfo.cs ./Pencil.Gaming/Glfw/Glfw2.cs ./Pencil.Gaming/Glfw/Glfw3DelegateTypes.cs ./Pencil.Gaming/Glfw/Glfw3Structs.cs ./Pencil.Gaming/Glfw/Glfw3MouseState.cs ./Pencil.Gaming/Glfw/Glfw2KeyboardState.cs ./Pencil.Gaming/Glfw/Glfw3_64.cs ./Pencil.Gaming/Glfw/Glfw2Delegates.cs ./Pencil.Gaming/Glfw/Glfw2_64.cs ./Pencil.Gaming/Glfw/Glfw3KeyboardState.cs ./Pencil.Gaming/Glfw/Glfw3Delegates.cs ./Pencil.Gaming/Glfw/Glfw3_32.cs ./Pencil.Gaming/Glfw/Glfw3.cs ./Pencil.Gaming/Glfw/Glfw2DelegateTypes.cs ./Pencil.Gaming/Glfw/Glfw2Enum.cs ./Pencil.Gaming/Glfw/Glfw2MouseState.cs ./Pencil.Gaming/Glfw/Glfw3Enum.cs ./Pencil.Gaming/Glfw/Glfw2Structs.cs ./Pencil.Gaming/Glfw/Glfw2_32.cs ./Pencil.Gaming/Math/Vector4.cs ./Pencil.Gaming/Math/Vector2.cs ./Pencil.Gaming/Math/Quaternion.cs ./Pencil.Gaming/Math/MathHelper.cs ./Pencil.Gaming/Math/Rectanglei.cs ./Pencil.Gaming/Math/Matrix.cs ./Pencil.Gaming/Math/Vector3.cs ./Pencil.Gaming/Math/Vector2i.cs ./Pencil.Gaming/Math/Rectangle.cs ./Pencil.Gaming/Math/Vector3i.cs ./Pencil.Gaming/Math/Vector4i.cs ./Pencil.Gaming/Applets/AppletRequest.cs ./Pencil.Gaming/Applets/Browsers.cs ./Pencil.Gaming/Applets/PApplet.cs ./Pencil.Gaming/Applets/SupportedBrowsersAttribute.cs ./Pencil.Gaming/AutoGeneratedAttribute.cs ./Pencil.Gaming/Scripting/Lua32.cs ./Pencil.Gaming/Scripting/LuaDelegates.cs ./Pencil.Gaming/Scripting/Lua64.cs ./Pencil.Gaming/Scripting/LuaL64.cs ./Pencil.Gaming/Scripting/LuaLMacroFunctions.cs ./Pencil.Gaming/Scripting/LuaL32.cs ./Pencil.Gaming/Scripting/Lua.cs ./Pencil.Gaming/Scripting/LuaEnums.cs ./Pencil.Gaming/Scripting/LuaStructs.cs ./Pencil.Gaming/Scripting/LuaMacroFunctions.cs ./Pencil.Gaming/Scripting/LuaL.cs ./Pencil.Gaming/Scripting/LuaDelegateTypes.cs ./Pencil.Gaming/Scripting/LuaLDelegates.cs  -out:"Pencil.Gaming/bin/Core - GLFW3/Pencil.Gaming.exe" -define:USE_GL_CORE\;USE_GLFW3 -r:System.Drawing -r:System.Core -r:Pencil.Gaming/NVorbis.dll -optimize+ -debug- -target:library -platform:anycpu -unsafe+
```

The binaries will be in the `Pencil.Gaming/bin/<Profile>` directory.

Image loading utility
---------------------
```C#
int image = GL.Utils.LoadImage("myfile.png"); // Works with multiple file formats
GL.BindTexture(TextureTarget.Texture2D, image);

GL.Begin(BeginMode.TriangleStrip);
  GL.TexCoord2(0f, 1f);
  GL.Vertex2(0.1f, 0.9f);
  GL.TexCoord2(0f, 0f);
  GL.Vertex2(0.1f, 0.1f);
  GL.TexCoord2(1f, 1f);
  GL.Vertex2(0.9f, 0.9f);
  GL.TexCoord2(1f, 0f);
  GL.Vertex2(0.9f, 0.1f);
GL.End();

GL.DeleteTextures(1, ref image);
```

Model loading utility
----------------------

#### Fields
```C#
int modelVbo;
int indexVbo;
int numberOfIndices;
```

#### During program initialization
```C#
GL.Enable(EnapleCap.DepthTest);

Vector4[] vertices;
Vector3[] normals;
Vector2[] texCoords;
int[] indices;
GL.Utils.LoadModel("model.obj", out vertices, out normals, out texCoords, out indices, false);

numberOfIndices = indices.Length;

GL.GenBuffers(1, out modelVbo);
GL.BindBuffer(BufferTarget.ArrayBuffer, modelVbo);
GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * 4 * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

GL.GenBuffers(1, out indexVbo);
GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexVbo);
GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
```

#### In the draw function
```C#
// NOTE: This uses legacy OpenGL, just to fit in the readme...
GL.EnableClientState(ArrayCap.VertexArray);

GL.BindBuffer(BufferTarget.ArrayBuffer, modelVbo);
GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexVbo);

GL.VertexPointer(4, VertexPointerType.Float, 4 * sizeof(float), 0);
GL.DrawElements(BeginMode.Triangles, numberOfIndices, DrawElementsType.UnsignedInt, 0);

GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

GL.DisableClientState(ArrayCap.VertexArray);
```

#### During cleanup
```C#
GL.DeleteBuffers(1, ref modelVbo);
GL.DeleteBuffers(1, ref indexVbo);
```

Lua
===
Lua is a light-weight scripting language, perfectly suitable for use in game development.

Pencil.Gaming provides support for Lua, using the default C# `PascalCased` identifiers, making it integrate seamlessly with other C# code.

Whereas a C-api function call might be `lua_pcall(L, 0, LUA_MULTRET, 0)`, the Pencil.Gaming C# API call would be `Lua.PCall(L, 0, Lua.MultRet, 0)`.

For more information on using these lua bindings, see the [Lua C api docs](http://www.lua.org/pil/contents.html#24).

Sample usage (OpenAL)
=====================
Another utility is the `AL.Utils.BufferFromWav` utility, which is able to load wave files into an OpenAL buffer. Similarly, there's the `AL.Utils.BufferFromOgg` utility, allowing Ogg/Vorbis file loading.

```C#
uint buffer = AL.Utils.BufferFromWav("MyWaveFile.wav");
uint source;
AL.GenSources(1, out source);

AL.Source(source, ALSourcei.Buffer, (int) buffer);
AL.Source(source, ALSourceb.Looping, true);

AL.SourcePlay(source);

// ...
// ...

// When cleaning up:
AL.DeleteSources(1, ref source);
AL.DeleteBuffers(1, ref buffer);
```

Other Resources
===============

* [Collada Importer](http://sourceforge.net/projects/csharpcollada/)