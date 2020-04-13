//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library, except where noted.
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
//


using OpenTK;

namespace LayoutFarm.UI
{

    public static class GLESInit
    {
        static OpenTK.Graphics.GraphicsMode s_gfxmode;
        static bool s_initOpenTK;
        public static void InitGLES()
        {


          

            if (s_initOpenTK) return;

            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();
            OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions {
                Backend = OpenTK.PlatformBackend.PreferNative,
            });
            OpenTK.Graphics.PlatformAddressPortal.GetAddressDelegate = OpenTK.Platform.Utilities.CreateGetAddress();

            (new OpenTK.Graphics.ES20.GL()).LoadAll();
            (new OpenTK.Graphics.ES30.GL()).LoadAll();
            s_initOpenTK = true;
        }

        public static OpenTK.Graphics.GraphicsMode GetDefaultGraphicsMode()
        {
            if (!s_initOpenTK)
            {
                InitGLES();
            }
            if (s_gfxmode == null)
            {
                s_gfxmode = new OpenTK.Graphics.GraphicsMode(
                    DisplayDevice.Default.BitsPerPixel,//default 32 bits color
                    16,//depth buffer => 16
                    8, //stencil buffer => 8 (set this if you want to use stencil buffer toos)
                    0, //number of sample of FSAA (not always work)
                    0, //accum buffer
                    2, // n buffer, 2=> double buffer
                    false);//sterio 
            }
            return s_gfxmode;
        }

        public static int GLES_Major = 2;
        public static int GLES_Minor = 1;
    }
}