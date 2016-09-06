using System;
using System.Collections.Generic;  
using Pencil.Gaming; 
using Pencil.Gaming.Graphics;

namespace TestGlfw
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //PureGLFWProgram.Start();
            SimpleWindowProgram.Start();
        }
    }
}
