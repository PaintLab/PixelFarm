//BSD, 2014-present, WinterDev
//MIT, 2018-present, WinterDev
using System;
namespace PixelFarm.Drawing.Internal
{
    public static class UIMsgQueue
    {
        public delegate void RunOnceDelegate();
        static Action<RunOnceDelegate> s_runOnceRegisterImpl;
        public static void RegisterRunOnce(RunOnceDelegate runOnce)
        {
            if (s_runOnceRegisterImpl == null)
            {
                throw new NotSupportedException();
            }
            s_runOnceRegisterImpl(runOnce);
        }
        public static void RegisterRunOnceImpl(Action<RunOnceDelegate> runOnceRegisterImpl)
        {
            s_runOnceRegisterImpl = runOnceRegisterImpl;
        }
    }

}