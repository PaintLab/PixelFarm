//Apache2, 2014-present, WinterDev


namespace LayoutFarm
{

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
    }

}