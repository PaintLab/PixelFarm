//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public interface IBoxElement
    {
        //for css layout 

        void ChangeElementSize(int w, int h);
        int MinHeight { get; }
    }


}