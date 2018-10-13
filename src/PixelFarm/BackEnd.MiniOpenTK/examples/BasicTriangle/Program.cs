using System;
using OpenTK;

using OpenTK.Graphics.ES30;
namespace BasicTriangle
{
    sealed class Program : GameWindow
    {

        static Program()
        {
            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();

            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferNative,

            });

            OpenTK.Graphics.PlatformAddressPortal.GetAddressDelegate = OpenTK.Platform.Utilities.CreateGetAddress();
        }

        //A simple vertex shader possible.Just passes through the position vector.
        //const string VertexShaderSource = @"
        //    #version 330

        //    layout(location = 0) in vec4 position;

        //    void main(void)
        //    {
        //        gl_Position = position;
        //    }
        //";

        //// A simple fragment shader. Just a constant red color.
        //const string FragmentShaderSource = @"
        //    #version 330

        //    out vec4 outputColor;

        //    void main(void)
        //    {
        //        outputColor = vec4(1.0, 0.0, 0.0, 1.0);
        //    }
        //";

        //string VertexShaderSource = @"      

        //    attribute vec2 a_position;
        //    attribute vec4 a_color;

        //    varying vec4 v_color;

        //    void main()
        //    {   

        //        gl_Position = vec4(a_position[0],a_position[1],0,1);  
        //        v_color = a_color;
        //    }
        //    ";
        ////fragment source
        //string FragmentShaderSource = @"
        //        precision mediump float;
        //        varying vec4 v_color;                 
        //        void main()
        //        { 
        //            gl_FragColor = v_color;
        //        }
        //    ";

        string VertexShaderSource =
            "#version 300 es\n" +
            @"in vec2 a_position;
              in vec4 a_color; 
              out vec4 v_color;
              void main()
              {    
                gl_Position = vec4(a_position[0],a_position[1],0,1);  
                v_color = a_color;
             }
            ";
        //fragment source
        string FragmentShaderSource =
            "#version 300 es\n" +
            @"  precision mediump float;
                in vec4 v_color;        
                out vec4 color;
                void main()
                { 
                    color = v_color;
                }
            ";
        // Points of a triangle in normalized device coordinates.
        readonly float[] Points = new float[] {
            // X, Y, Z, W
            -0.5f, 0.0f, 0.0f, 1.0f,
            0.5f, 0.0f, 0.0f, 1.0f,
            0.0f, 0.5f, 0.0f, 1.0f };

        int VertexShader;
        int FragmentShader;
        int ShaderProgram;
        int VertexBufferObject;
        int VertexArrayObject;

        protected override void OnLoad(EventArgs e)
        {
            // Load the source of the vertex shader and compile it.
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);

            // Load the source of the fragment shader and compile it.
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);

            // Create the shader program, attach the vertex and fragment shaders and link the program.
            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.LinkProgram(ShaderProgram);
            // Retrive the position location from the program.
            var positionLocation = GL.GetAttribLocation(ShaderProgram, "a_position");


            // Create the vertex buffer object (VBO) for the vertex data.
            VertexBufferObject = GL.GenBuffer();
            // Bind the VBO and copy the vertex data into it.
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Points.Length * sizeof(float), Points, BufferUsageHint.StaticDraw);


            // Create the vertex array object (VAO) for the program.
            //VertexArrayObject = GL.GenVertexArray();
            GL.GenVertexArrays(1, out VertexArrayObject);
            // Bind the VAO and setup the position attribute.
            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(positionLocation);

            // Set the clear color to blue
            GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteProgram(ShaderProgram);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            base.OnUnload(e);
        }

        protected override void OnResize(EventArgs e)
        {
            // Resize the viewport to match the window size.
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the color buffer.
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Bind the VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            // Bind the VAO
            GL.BindVertexArray(VertexArrayObject);
            // Use/Bind the program
            GL.UseProgram(ShaderProgram);
            // This draws the triangle.
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        [STAThread]
        static void Main()
        {
            var program = new Program();
            program.Run();
        }
    }
}
