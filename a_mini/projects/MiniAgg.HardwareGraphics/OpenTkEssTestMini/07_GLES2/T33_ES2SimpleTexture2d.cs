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
    [Info("T33_ES2SimpleTexture2d")]
    public class T33_ES2SimpleTexture2dDemo : DemoBase
    {
        public override void Init()
        {
            T33_ES2SimpleTexture2d example;
            try
            {
                example = new T33_ES2SimpleTexture2d(GraphicsContextFlags.Embedded);
            }
            catch
            {
                example = new T33_ES2SimpleTexture2d(GraphicsContextFlags.Default);
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

    public class T33_ES2SimpleTexture2d : GameWindow
    {
        #region Constructor
        int mProgram;

        // Attribute locations
        int mPositionLoc;
        int mTexCoordLoc;

        // Sampler location
        int mSamplerLoc;

        // Texture handle
        int mTexture;
        public T33_ES2SimpleTexture2d(GraphicsContextFlags flags)
            : base(800, 600, GraphicsMode.Default, "", GameWindowFlags.Default, DisplayDevice.Default, 2, 0, flags)
        {
        }

        #endregion

        #region OnLoad

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //     const std::string vs = SHADER_SOURCE
            //(
            //    attribute vec4 a_position;
            //     attribute vec2 a_texCoord;
            //     varying vec2 v_texCoord;

            //     void main()
            //     {
            //         gl_Position = a_position;
            //         v_texCoord = a_texCoord;
            //     }			
            // );

            //     const std::string fs = SHADER_SOURCE
            //     (
            //         precision mediump float;
            //     varying vec2 v_texCoord;
            //     uniform sampler2D s_texture;
            //     void main()
            //     {
            //         gl_FragColor = texture2D(s_texture, v_texCoord);
            //     }
            // );
            string vs = @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;
                varying vec2 v_texCoord;
                void main()
                {
                    gl_Position = a_position;
                    v_texCoord = a_texCoord;
                 }	 
            ";

            string fs = @"
                  precision mediump float;
                  varying vec2 v_texCoord;
                  uniform sampler2D s_texture;
                  void main()
                  {
                     gl_FragColor = texture2D(s_texture, v_texCoord);
                  }
            ";

            mProgram = ES2Utils.CompileProgram(vs, fs);
            if (mProgram == 0)
            {
                //return false
            }
            // Get the attribute locations
            mPositionLoc = GL.GetAttribLocation(mProgram, "a_position");
            mTexCoordLoc = GL.GetAttribLocation(mProgram, "a_texCoord");

            // Get the sampler location
            mSamplerLoc = GL.GetAttribLocation(mProgram, "s_texture");

            //// Load the texture
            mTexture = ES2Utils.CreateSimpleTexture2D();


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

            //    GLfloat vertices[] =
            //{
            //    -0.5f,  0.5f, 0.0f,  // Position 0
            //     0.0f,  0.0f,        // TexCoord 0
            //    -0.5f, -0.5f, 0.0f,  // Position 1
            //     0.0f,  1.0f,        // TexCoord 1
            //     0.5f, -0.5f, 0.0f,  // Position 2
            //     1.0f,  1.0f,        // TexCoord 2
            //     0.5f,  0.5f, 0.0f,  // Position 3
            //     1.0f,  0.0f         // TexCoord 3
            //};
            //    GLushort indices[] = { 0, 1, 2, 0, 2, 3 };

            float[] vertices = new float[] {
                -0.5f,  0.5f, 0.0f,  // Position 0
                 0.0f,  0.0f,        // TexCoord 0
                -0.5f, -0.5f, 0.0f,  // Position 1
                 0.0f,  1.0f,        // TexCoord 1
                 0.5f, -0.5f, 0.0f,  // Position 2
                 1.0f,  1.0f,        // TexCoord 2
                 0.5f,  0.5f, 0.0f,  // Position 3
                 1.0f,  0.0f         // TexCoord 3
                 };
            ushort[] indices = new ushort[] { 0, 1, 2, 0, 2, 3 };


            //    // Set the viewport
            //    glViewport(0, 0, getWindow()->getWidth(), getWindow()->getHeight());
            GL.Viewport(0, 0, this.Width, this.Height);

            //    // Clear the color buffer
            //    glClear(GL_COLOR_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //    // Use the program object
            //    glUseProgram(mProgram);
            GL.UseProgram(mProgram);
            //    // Load the vertex position
            //    glVertexAttribPointer(mPositionLoc, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), vertices);
            GL.VertexAttribPointer(mPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), vertices);

            //    // Load the texture coordinate
            //    glVertexAttribPointer(mTexCoordLoc, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), vertices + 3);
            unsafe
            {
                fixed (float* v_3 = &vertices[3])
                {
                    GL.VertexAttribPointer(mTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (IntPtr)v_3);
                }
            }
            //    glEnableVertexAttribArray(mPositionLoc);
            //    glEnableVertexAttribArray(mTexCoordLoc);
            GL.EnableVertexAttribArray(mPositionLoc);
            GL.EnableVertexAttribArray(mTexCoordLoc);


            //    // Bind the texture
            //    glActiveTexture(GL_TEXTURE0);
            GL.ActiveTexture(TextureUnit.Texture0);
            //    glBindTexture(GL_TEXTURE_2D, mTexture);
            GL.BindTexture(TextureTarget.Texture2D, mTexture);
            //    // Set the texture sampler to texture unit to 0
            //    glUniform1i(mSamplerLoc, 0);
            GL.Uniform1(mSamplerLoc, 0);
            //    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_SHORT, indices);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, indices);

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
