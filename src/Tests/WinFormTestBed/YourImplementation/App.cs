//Apache2, 2014-present, WinterDev
using System;
using PixelFarm.Drawing;

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
        public static Image LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            GdiPlusBitmap bmp = new GdiPlusBitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
    }

}