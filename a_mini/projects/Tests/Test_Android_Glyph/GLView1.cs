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


    struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }

    /// <summary>
    /// sharing data between canvas and shaders
    /// </summary>
    class CanvasToShaderSharedResource
    {
        /// <summary>
        /// stroke width here is the sum of both side of the line.
        /// </summary>
        internal float _strokeWidth = 1;
        OpenTK.Graphics.ES20.MyMat4 _orthoView;
        internal ShaderBase _currentShader;
        int _orthoViewVersion = 0;
        Color _strokeColor;

        internal OpenTK.Graphics.ES20.MyMat4 OrthoView
        {
            get { return _orthoView; }
            set
            {

                _orthoView = value;
                unchecked { _orthoViewVersion++; }
            }
        }
        public int OrthoViewVersion
        {
            get { return this._orthoViewVersion; }
        }

        internal Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                _strokeColor = value;
                _stroke_r = value.R / 255f;
                _stroke_g = value.G / 255f;
                _stroke_b = value.B / 255f;
                _stroke_a = value.A / 255f;
            }
        }

        float _stroke_r;
        float _stroke_g;
        float _stroke_b;
        float _stroke_a;
        internal void AssignStrokeColorToVar(OpenTK.Graphics.ES20.ShaderUniformVar4 color)
        {
            color.SetValue(_stroke_r, _stroke_g, _stroke_b, _stroke_a);
        }
    }

    abstract class ShaderBase
    {
        protected readonly CanvasToShaderSharedResource _canvasShareResource;
        protected readonly MiniShaderProgram shaderProgram = new MiniShaderProgram();
        public ShaderBase(CanvasToShaderSharedResource canvasShareResource)
        {
            _canvasShareResource = canvasShareResource;
        }
        /// <summary>
        /// set as current shader
        /// </summary>
        protected void SetCurrent()
        {
            if (_canvasShareResource._currentShader != this)
            {
                shaderProgram.UseProgram();
                _canvasShareResource._currentShader = this;
                this.OnSwithToThisShader();
            }
        }
        protected virtual void OnSwithToThisShader()
        {
        }
    }

    class BasicFillShader : ShaderBase
    {
        ShaderVtxAttrib2f a_position;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar4 u_solidColor;
        public BasicFillShader(CanvasToShaderSharedResource canvasShareResource)
            : base(canvasShareResource)
        {
            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec2 a_position; 
            uniform mat4 u_mvpMatrix;
            uniform vec4 u_solidColor;              
            varying vec4 v_color;
 
            void main()
            {
                gl_Position = u_mvpMatrix* vec4(a_position[0],a_position[1],0,1); 
                v_color= u_solidColor;
            }
            ";
            //fragment source
            string fs = @"
                precision mediump float;
                varying vec4 v_color; 
                void main()
                {
                    gl_FragColor = v_color;
                }
            ";
            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }

            a_position = shaderProgram.GetAttrV2f("a_position");
            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");
        }
        public void FillTriangleStripWithVertexBuffer(float[] linesBuffer, int nelements, Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadPureV2f(linesBuffer);
            GL.DrawArrays(BeginMode.TriangleStrip, 0, nelements);
        }
        //--------------------------------------------
        int orthoviewVersion = -1;
        void CheckViewMatrix()
        {
            int version = 0;
            if (orthoviewVersion != (version = _canvasShareResource.OrthoViewVersion))
            {
                orthoviewVersion = version;
                u_matrix.SetData(_canvasShareResource.OrthoView.data);
            }
        }
        //--------------------------------------------
        public void FillTriangles(float[] polygon2dVertices, int nelements, Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------  

            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.LoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.Triangles, 0, nelements);
        }
        public unsafe void DrawLineLoopWithVertexBuffer(float* polygon2dVertices, int nelements, Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------
            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.LineLoop, 0, nelements);
        }
        public unsafe void FillTriangleFan(float* polygon2dVertices, int nelements, Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            a_position.UnsafeLoadPureV2f(polygon2dVertices);
            GL.DrawArrays(BeginMode.TriangleFan, 0, nelements);
        }
        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            SetCurrent();
            CheckViewMatrix();
            //--------------------------------------------

            u_solidColor.SetValue((float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f);
            unsafe
            {
                float* vtx = stackalloc float[4];
                vtx[0] = x1; vtx[1] = y1;
                vtx[2] = x2; vtx[3] = y2;
                a_position.UnsafeLoadPureV2f(vtx);
            }
            GL.DrawArrays(BeginMode.Lines, 0, 2);
        }
    }

    class CustomApp
    {

        /// <summary> the text context </summary>
        private TypographyTextContext textContext;
        //--------------------------------
        CanvasToShaderSharedResource shaderRes;
        BasicFillShader fillShader;
        //--------------------------------

        int view_width;
        int view_height;
        MyMat4 orthoView;
        MyMat4 flipVerticalView;
        MyMat4 orthoAndFlip;


        Typography.Rendering.TextMesh textMesh;
        PixelFarm.DrawingGL.TessTool tessTool;
        PixelFarm.DrawingGL.SimpleCurveFlattener curveFlattener;
        System.Collections.Generic.List<float[]> vxsList = new System.Collections.Generic.List<float[]>();
        //
        public void Setup(int canvasW, int canvasH)
        {

            this.view_width = canvasW;
            this.view_height = canvasH;

            Tesselate.Tesselator tt = new Tesselate.Tesselator();
            tessTool = new PixelFarm.DrawingGL.TessTool(tt);

            curveFlattener = new PixelFarm.DrawingGL.SimpleCurveFlattener();

            int max = Math.Max(canvasW, canvasH);
            ////square viewport 
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            flipVerticalView = MyMat4.scale(1, -1) * MyMat4.translate(new OpenTK.Vector3(0, -max, 0));
            orthoAndFlip = orthoView * flipVerticalView;
            //-----------------------------------------------------------------------
            shaderRes = new CanvasToShaderSharedResource();
            shaderRes.OrthoView = orthoView;
            //-----------------------------------------------------------------------  
            fillShader = new BasicFillShader(shaderRes);
            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size  
            //--------------------------------------------------------------------------
            var text = "T";

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
            for (int i = 0; i < 1; ++i)
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
                vxsList.Add(vtx);
            }



            // textMesh.Tess(curFlattener, tessTool);
            //-------------------------------------
            //create a tess version of the text mesh
            //textMesh.PathTessPolygon(Typography.Rendering.Color.Black); 
            ////create vertex and index buffer
            //TriangleBuffer.Fill(textMesh.IndexBuffer, textMesh.VertexBuffer);
            //BezierBuffer.Fill(textMesh.BezierIndexBuffer, textMesh.BezierVertexBuffer);
            ////--------------------------------------------------------------------------
        }

        public void RenderFrame()
        {
            //set clear color to white
            GL.ClearColor(1f, 1, 1, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Color color = new Color();
            color.A = 255;//black

            fillShader.DrawLine(0, 0, 700, 700, color);
            int n = vxsList.Count;
            for (int i = 0; i < n; ++i)
            {

                float[] vxPoints = vxsList[i];
                fillShader.FillTriangles(vxPoints, vxPoints.Length / 2, color);
                //float[] triangles = new float[]
                //{
                //    10,10,
                //    100,50,
                //    50,

                //};
                //fillShader.FillTriangles()
            }
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
