using System;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using OpenTK.Graphics.ES20;
namespace TestGlfw
{
    class SimpleWindowProgram
    {
        //[System.Runtime.InteropServices.DllImport("user32")]
        //static extern bool SetWindowText(IntPtr hwnd, string title);

        public static void Start()
        {
            if (!Glfw.Init())
            {
                Console.WriteLine("can't init glfw");
                return;
            }
            GlfwMonitorPtr monitor = new GlfwMonitorPtr();
            GlfwWindowPtr winPtr = new GlfwWindowPtr();
            GlfwWindowPtr glWindow = Glfw.CreateWindow(800, 600, "Test Glfw", monitor, winPtr);

            ////---------------
            //IntPtr nativeWin32Hwnd = Glfw.GetNativeWindowHandle(glWindow);
            //SetWindowText(nativeWin32Hwnd, "OKOK...");
            ////---------------
            while (!Glfw.WindowShouldClose(glWindow))
            {

                Glfw.PollEvents();
            }
            Glfw.Terminate();
        }
    }
}