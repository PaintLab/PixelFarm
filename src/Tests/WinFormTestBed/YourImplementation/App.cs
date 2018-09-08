//Apache2, 2014-present, WinterDev


namespace LayoutFarm
{

    public delegate System.IO.Stream ReadStreamDelegate(string url);

    public abstract class App
    {
        public void Start(AppHost host)
        {
            OnStart(host);
        }
        protected virtual void OnStart(AppHost host)
        {
        }
        public virtual string Desciption
        {
            get { return ""; }
        }

        protected virtual System.IO.Stream ReadStream(string url)
        {
            if (s_readStreamDelegate != null)
            {
                return s_readStreamDelegate(url);
            }
            return null;
        }

        static ReadStreamDelegate s_readStreamDelegate;
        public static void RegisterReadStreamDelegate(ReadStreamDelegate readStreamDelegate)
        {
            s_readStreamDelegate = readStreamDelegate;
        }
        public static System.IO.Stream ReadStreamS(string url)
        {
            return s_readStreamDelegate(url);
        }
    }

}