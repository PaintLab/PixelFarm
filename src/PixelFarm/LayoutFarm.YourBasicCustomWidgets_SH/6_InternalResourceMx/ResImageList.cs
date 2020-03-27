//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    //library specific 
    //for 
    public static class ResImageList
    {
        //temp ***

        static Dictionary<ImageName, Image> s_images;
        public static bool HasImages => s_images != null;
        public static void SetImageList(Dictionary<ImageName, Image> images)
        {
            ResImageList.s_images = images;
        }
        public static Image GetImage(ImageName imageName)
        {
            s_images.TryGetValue(imageName, out Image found);
            return found;
        }
        public static ImageBinder GetImageBinder(ImageName imageName)
        {
            s_images.TryGetValue(imageName, out Image found);
            ImageBinder binder = new MyClientImageBinder(null);
            binder.SetLocalImage(found);
            binder.State = BinderState.Loaded;
            return binder;
        }
    }

    public enum ImageName
    {
        CheckBoxChecked,
        CheckBoxUnChecked,
        RadioBoxChecked,
        RadioBoxUnChecked
    }
}