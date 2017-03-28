//MIT, 2017, Zou Wei(github/zwcloud)
//MIT, 2017, WinterDev (modified from Xamarin's Android code template)
using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;
using AndroidOS = Android.OS;

using Xamarin.OpenGL;

namespace Test_Android_Glyph
{



    class CustomApp
    {

        /// <summary> the text context </summary>
        TypographyTextContext textContext;
        //--------------------------------
        CanvasToShaderSharedResource shaderRes;
        GlyphFillShader fillShader;
        //--------------------------------

        int view_width;
        int view_height;
        int max;
        MyMat4 orthoView;
        MyMat4 flipVerticalView;
        MyMat4 orthoAndFlip;


        Typography.Rendering.TextMesh textMesh;
        PixelFarm.DrawingGL.TessTool tessTool;
        PixelFarm.DrawingGL.SimpleCurveFlattener curveFlattener;
        System.Collections.Generic.List<Typography.Rendering.GlyphMesh> vxsList = new System.Collections.Generic.List<Typography.Rendering.GlyphMesh>();
        //
        public void Setup(int canvasW, int canvasH)
        {
            this.max = Math.Max(canvasW, canvasH);
            this.view_width = canvasW;
            this.view_height = canvasH;

            Tesselate.Tesselator tt = new Tesselate.Tesselator();
            tessTool = new PixelFarm.DrawingGL.TessTool(tt);

            curveFlattener = new PixelFarm.DrawingGL.SimpleCurveFlattener();


            ////square viewport 
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            flipVerticalView = MyMat4.scale(1, -1) * MyMat4.translate(new OpenTK.Vector3(0, -max, 0));
            orthoAndFlip = orthoView * flipVerticalView;
            //-----------------------------------------------------------------------
            shaderRes = new CanvasToShaderSharedResource();
            shaderRes.OrthoView = orthoView;
            //-----------------------------------------------------------------------  
            fillShader = new GlyphFillShader(shaderRes);
            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size  
            //--------------------------------------------------------------------------
            var text = "Glyph";

            //optional ....
            //var directory = AndroidOS.Environment.ExternalStorageDirectory;
            //var fullFileName = Path.Combine(directory.ToString(), "TypographyTest.txt");
            //if (File.Exists(fullFileName))
            //{
            //    text = File.ReadAllText(fullFileName);
            //}
            //--------------------------------------------------------------------------
            textContext = new TypographyTextContext(
                text,
                "DroidSans.ttf", //corresponding to font file Assets/DroidSans.ttf
                36,//font size
                   //all following is duumy and not implemented
                FontStretch.Normal, FontStyle.Normal, FontWeight.Normal, 4096, 4096,
                Xamarin.OpenGL.TextAlignment.Leading
            );
            //Build text mesh
            //textContext.Build(0, 0, textMesh);
            textMesh = new Typography.Rendering.TextMesh();
            textContext.Build(0, 0, textMesh);
            System.Collections.Generic.List<Typography.Rendering.GlyphMesh> glyphs = textMesh._glyphs;
            int j = glyphs.Count;
            for (int i = 0; i < j; ++i)
            {
                Typography.Rendering.GlyphMesh glyph = glyphs[i];
                Typography.Rendering.WritablePath path = glyph.path;
                float[] flattenPoints = curveFlattener.Flatten(path._points);
                System.Collections.Generic.List<PixelFarm.DrawingGL.Vertex> vertextList = tessTool.TessPolygon(flattenPoints);
                //-----------------------------   
                //switch how to fill polygon
                int vxcount = vertextList.Count;
                float[] vtx = new float[vxcount * 2];
                int n = 0;
                for (int p = 0; p < vxcount; ++p)
                {
                    var v = vertextList[p];
                    vtx[n] = (float)v.m_X;
                    vtx[n + 1] = (float)v.m_Y;
                    n += 2;
                }
                ////triangle list
                //int tessAreaTriangleCount = vxcount;
                //-------------------------------------     
                glyph.nElements = vxcount;
                glyph.tessData = vtx;
                vxsList.Add(glyph);
            }

        }

        public void RenderFrame()
        {
            GL.Viewport(0, 0, max, max);
            //set clear color to white
            GL.ClearColor(1f, 1, 1, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Color color = new Color();
            color.A = 255;//black
            //test fill line
            fillShader.DrawLine(0, 0, 700, 700, color);
            //
            int n = vxsList.Count;
            float scale = textContext.Typeface.CalculateToPixelScaleFromPointSize(36);

            for (int i = 0; i < n; ++i)
            {
                Typography.Rendering.GlyphMesh mesh = vxsList[i]; 
                fillShader.SetOffset(mesh.OffsetX * scale, mesh.OffsetY * scale);
                fillShader.FillTriangles(mesh.tessData, mesh.nElements, color);
            }
            fillShader.SetOffset(0, 0);
        }
    }
    class GLView1 : AndroidGameView
    {
        int view_width;
        int view_height;
        CustomApp customApp = new CustomApp();

        public GLView1(Context context) : base(context)
        {

        }

        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Android.Graphics.Point sc_size = new Android.Graphics.Point();
            Display.GetSize(sc_size);

            this.view_width = sc_size.X;
            this.view_height = sc_size.Y;
            MakeCurrent();
            //-----------
            customApp.Setup(this.view_width, this.view_height);
            //-----------
            // Run the render loop
            Run();
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer()
        {
            //essential, from https://github.com/xamarin/monodroid-samples/blob/master/GLTriangle20-1.0/PaintingView.cs
            ContextRenderingApi = GLVersion.ES2;

            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                Log.Verbose("GLTriangle", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLTriangle", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try
            {
                Log.Verbose("GLTriangle", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode(0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLTriangle", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        // This gets called on each frame render
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            customApp.RenderFrame();
            SwapBuffers();
        }


    }
}
