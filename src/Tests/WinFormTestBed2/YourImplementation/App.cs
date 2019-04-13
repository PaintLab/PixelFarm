//Apache2, 2014-present, WinterDev


namespace LayoutFarm
{

    public abstract class App
    {
        public delegate TResult Func<out TResult>();
        public delegate TResult Func<in T, out TResult>(T arg);
        public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
        public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);

        internal void StartApp(AppHost host)
        {
            OnStart(host);
        }
        protected virtual void OnStart(AppHost host)
        {
        }
        public virtual void OnClosing()
        {

        }
        public virtual void OnClosed()
        {

        }
        //
        public virtual string Desciption => "";
        //
        protected virtual System.IO.Stream ReadStream(string url)
        {
            if (s_readStreamDelegate != null)
            {
                return s_readStreamDelegate(url);
            }
            return null;
        }

        static Func<string, System.IO.Stream> s_readStreamDelegate;
        static Func<string, System.IO.Stream> s_writeStreamDelegate;
        static Func<string, System.IO.Stream, bool> s_uploadStreamDelegate;

        public static void RegisterUploadStreamDelegate(Func<string, System.IO.Stream, bool> uploadStreamDel)
        {
            s_uploadStreamDelegate = uploadStreamDel;
        }
        public static void RegisterReadStreamDelegate(Func<string, System.IO.Stream> getReadStreamDel)
        {
            s_readStreamDelegate = getReadStreamDel;
        }
        public static void RegisterGetWriteStreamDelegate(Func<string, System.IO.Stream> getWriteStreamDel)
        {
            s_writeStreamDelegate = getWriteStreamDel;
        }
        public static System.IO.Stream ReadStreamS(string url)
        {
            return s_readStreamDelegate(url);
        }
        public static System.IO.Stream GetWriteStream(string url)
        {
            if (s_writeStreamDelegate != null)
            {
                return s_writeStreamDelegate(url);
            }
            return null;
        }
        public static bool UploadStream(string url, System.IO.Stream uploadstream)
        {
            return s_uploadStreamDelegate(url, uploadstream);
        }
    }

}