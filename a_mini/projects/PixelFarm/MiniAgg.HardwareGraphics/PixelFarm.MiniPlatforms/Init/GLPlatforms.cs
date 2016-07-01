using System;

namespace PixelFarm
{
    public static class GLPlatforms
    {
        public static void Init()
        {

        }
    }
    public class GlfwWinInfo : OpenTK.Platform.IWindowInfo
    {
        IntPtr glfwHandle;
        public GlfwWinInfo(IntPtr glfwHandle)
        {
            this.glfwHandle = glfwHandle;
        }
        public void Dispose()
        {
        }
    }
}
