//MIT, 2017, WinterDev (modified from Xamarin's iOS code template)

using System;
using System.Diagnostics;

using Foundation;
using GLKit;
using OpenGLES;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace Test_iOSApp1
{
    [Register("GameViewController")]
    public class GameViewController : GLKViewController, IGLKViewDelegate
    {
        EAGLContext context { get; set; }

        [Export("initWithCoder:")]
        public GameViewController(NSCoder coder) : base(coder)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
#endif

            //create gl context 
            context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);

            if (context == null)
            {
                Debug.WriteLine("Failed to create ES context");
            }

            var view = (GLKView)View;
            view.Context = context;
            view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;
            view_width = (int)view.Frame.Width;
            view_height = (int)view.Frame.Height;
            SetupGL();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            TearDownGL();

            if (EAGLContext.CurrentContext == context)
                EAGLContext.SetCurrentContext(null);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            if (IsViewLoaded && View.Window == null)
            {
                View = null;

                TearDownGL();

                if (EAGLContext.CurrentContext == context)
                {
                    EAGLContext.SetCurrentContext(null);
                }
            }

            // Dispose of any resources that can be recreated.
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        int view_width;
        int view_height;
        void SetupGL()
        {
            EAGLContext.SetCurrentContext(context);

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
        void TearDownGL()
        {
            EAGLContext.SetCurrentContext(context);
            shaderProgram.DeleteMe();
        }

        #region GLKView and GLKViewController delegate methods

        public override void Update()
        {
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

        //string LoadResource(string name, string type)
        //{
        //    var path = NSBundle.MainBundle.PathForResource(name, type);
        //    return System.IO.File.ReadAllText(path);
        //}

        #endregion 
    }
}