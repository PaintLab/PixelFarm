//BSD, 2014-present, WinterDev  

namespace PixelFarm.Drawing.WinGdi
{
    class MyRegion : Region
    {
        System.Drawing.Region _rgn = new System.Drawing.Region();
        public override object InnerRegion
        {
            get { return this._rgn; }
        }
        public override void Dispose()
        {
            if (_rgn != null)
            {
                _rgn.Dispose();
                _rgn = null;
            }
        }
    }
}