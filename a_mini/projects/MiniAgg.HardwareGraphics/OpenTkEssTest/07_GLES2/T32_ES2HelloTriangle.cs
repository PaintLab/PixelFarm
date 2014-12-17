//
// Copyright (c) 2014 The ANGLE Project Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//

//            Based on Hello_Triangle.c from
// Book:      OpenGL(R) ES 2.0 Programming Guide
// Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
// ISBN-10:   0321502795
// ISBN-13:   9780321502797
// Publisher: Addison-Wesley Professional
// URLs:      http://safari.informit.com/9780321563835
//            http://www.opengles-book.com


#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
//using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.ES20;
using Examples.Tutorial;
using Mini;

#endregion




namespace OpenTkEssTest
{
    [Info(OrderCode = "31")]
    [Info("T32_HelloTriangle")]
    public class T32_ES2HelloTriangleDemo : DemoBase
    {
        public override void Init()
        {
            T32_ES2HelloTriangle example;
            try
            {
                example = new T32_ES2HelloTriangle(GraphicsContextFlags.Embedded);
            }
            catch
            {
                example = new T32_ES2HelloTriangle(GraphicsContextFlags.Default);
            }

            if (example != null)
            {
                //using (example)
                //{
                //    Utilities.SetWindowTitle(example);
                example.Run(30.0, 0.0);
                //}
            }
        }
    }
    //[Example("Simple ES 2.0", ExampleCategory.OpenGLES, "2.0", Documentation = "SimpleES20Window")]
    public class T32_ES2HelloTriangle : GameWindow
    {
        #region Constructor
        int mProgram;


        public T32_ES2HelloTriangle(GraphicsContextFlags flags)
            : base(800, 600, GraphicsMode.Default, "", GameWindowFlags.Default, DisplayDevice.Default, 2, 0, flags)
        {
        }

        #endregion

        #region OnLoad

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //----------------
            //vertex shader source
            string vs = @"        
            attribute vec4 vPosition;
            void main()
            {
                gl_Position = vPosition;
            }
            ";


            //fragment source
            string fs = @"
                 precision mediump float;
                void main()
                {
                    gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
                }
            ";


            mProgram = ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                //return false
            }

            GL.ClearColor(0, 0, 0, 0);

        }

        #endregion

        #region OnResize

        /// <summary>
        /// Called when the user resizes the window.
        /// </summary>
        /// <param name="e">Contains the new width/height of the window.</param>
        /// <remarks>
        /// You want the OpenGL viewport to match the window. This is the place to do it!
        /// </remarks>
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Prepares the next frame for rendering.
        /// </summary>
        /// <remarks>
        /// Place your control logic here. This is the place to respond to user input,
        /// update object positions etc.
        /// </remarks>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //var keyboard = OpenTK.Input.Keyboard.GetState();
            //if (keyboard[OpenTK.Input.Key.Escape])
            //{
            //    this.Exit();
            //    return;
            //}
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Place your rendering code here.
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs e)
        {

            //------------------------------------------------------------------------------------------------
            int width = this.Width;
            int height = this.Height;

            float[] vertices =
            {
             0.0f,  0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            };

            GL.Viewport(0, 0, width, height);

            // Set the viewport
            //glViewport(0, 0, getWindow()->getWidth(), getWindow()->getHeight());
            GL.Clear(ClearBufferMask.ColorBufferBit);
            // Clear the color buffer
            // glClear(GL_COLOR_BUFFER_BIT);

            // Use the program object
            //glUseProgram(mProgram);
            GL.UseProgram(mProgram);
            // Load the vertex data
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, vertices);
            GL.EnableVertexAttribArray(0);
            //glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, vertices);
            //glEnableVertexAttribArray(0);
            GL.DrawArrays(BeginMode.Triangles, 0, 3);
            //glDrawArrays(GL_TRIANGLES, 0, 3);


            this.SwapBuffers();
        }

        #endregion

        #region private void DrawCube()

        private void DrawCube()
        {
        }

        #endregion

    }
}
