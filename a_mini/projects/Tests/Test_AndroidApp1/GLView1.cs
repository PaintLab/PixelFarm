//MIT, 2017, WinterDev (modified from Xamarin's Android code template)
using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;



namespace Test_AndroidApp1
{
    class GLView1 : AndroidGameView
    {
        public GLView1(Context context) : base(context)
        {

        }
        int view_width;
        int view_height;
        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Android.Graphics.Point sc_size = new Android.Graphics.Point();
            Display.GetSize(sc_size);

            this.view_width = sc_size.X;
            this.view_height = sc_size.Y;
            SetupGL();

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

            //set clear color to white
            GL.ClearColor(1f, 1, 1, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //---------------------------------------------------------  
            u_matrix.SetData(orthoView.data);
            //---------------------------------------------------------   
            float x1 = 50, y1 = 20,
                 x2 = 300, y2 = 20;

            float[] vtxs = new float[] {
                        x1, y1, 1,
                        x2, y2, 1,
                        50, 300, 1
                    };

            u_useSolidColor.SetValue(1);
            u_solidColor.SetValue(1f, 0f, 0f, 1f);//use solid color  
            a_position.LoadPureV3f(vtxs);
            GL.DrawArrays(BeginMode.Triangles, 0, 3);

            //---------------
            SwapBuffers();
        }

        //--------------------------------
        MiniShaderProgram shaderProgram;
        ShaderVtxAttrib3f a_position;
        ShaderVtxAttrib4f a_color;
        ShaderUniformMatrix4 u_matrix;
        ShaderUniformVar1 u_useSolidColor;
        ShaderUniformVar4 u_solidColor;
        MyMat4 orthoView;
        //--------------------------------
        bool LoadShaders()
        {

            //-------------------------------------- 
            //create shader program
            shaderProgram = new MiniShaderProgram();
            //--------------------------------------            

            string vs = @"        
                attribute vec3 a_position;
                attribute vec4 a_color;  

                uniform mat4 u_mvpMatrix;
                uniform vec4 u_solidColor;
                uniform int u_useSolidColor;              

                varying vec4 v_color;
                varying vec4 a_position_output;
                void main()
                {

                    a_position_output =  u_mvpMatrix* vec4(a_position[0],a_position[1],0,1);
                    gl_Position = a_position_output;
                    v_color=  vec4(1,0,0,1); 
                }
                ";
            //fragment source
            //            string fs = @"void main()
            //                {
            //                    gl_FragColor = vec4(0.0, 1.0, 0.0, 1.0);
            //                }
            //            ";
            string fs = @"
                    precision mediump float;
                    varying vec4 v_color;  
                    varying vec4 a_position_output;
                    void main()
                    {
                        if(a_position_output[1]>0.5){
                            gl_FragColor = vec4(0,1,1,1);
                        }else{
                            gl_FragColor= vec4(0,1,0,1); 
                        }
                    }
                ";
            if (!shaderProgram.Build(vs, fs))
            {
                throw new NotSupportedException();
            }

            a_position = shaderProgram.GetAttrV3f("a_position");
            a_color = shaderProgram.GetAttrV4f("a_color");

            u_matrix = shaderProgram.GetUniformMat4("u_mvpMatrix");
            u_useSolidColor = shaderProgram.GetUniform1("u_useSolidColor");
            u_solidColor = shaderProgram.GetUniform4("u_solidColor");


            return true;
        }
        
        void SetupGL()
        {

            //EAGLContext.SetCurrentContext(context);
            MakeCurrent();

            LoadShaders();
            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size

            int ww_w = view_width;
            int ww_h = view_height;
            int max = Math.Max(ww_w, ww_h);
            //square viewport
            GL.Viewport(0, 0, max, max);
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            //-------------------------------------------------------------------------------- 
            shaderProgram.UseProgram();
        }
        
    }
}
