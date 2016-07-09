using System;
using System.Runtime.InteropServices;
namespace PixelFarm.Drawing
{
    public static class MyFtImageLib
    {
        const string myfontLib = @"myft.dll";
       
        [DllImport(myfontLib)]
        public static extern int MyFtLibGetVersion();
        [DllImport(myfontLib)]
        public static extern void DeleteUnmanagedObj(IntPtr unmanagedObject);
        [DllImport(myfontLib)]
        public static extern IntPtr stbi_load(string filename, out int w, out int h, out int comp, int requestOutputComponent);

    }
}