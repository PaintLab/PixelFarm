using System;

namespace OpenTK.Platform.Dummy
{
    public class DummyWindowInfo : IWindowInfo
    {
        public void Dispose()
        {
        }

        public IntPtr Handle
        {
            get { return IntPtr.Zero; }
        }
    }
}
