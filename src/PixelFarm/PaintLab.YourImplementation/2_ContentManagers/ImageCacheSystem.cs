//BSD, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.ContentManagers
{

    public class ImageCacheSystem
    {
        protected Dictionary<string, Image> _cacheImages = new Dictionary<string, Image>();
        public ImageCacheSystem()
        {
        }
        public virtual bool TryGetCacheImage(string url, out Image img)
        {
            return _cacheImages.TryGetValue(url, out img);
        }
        public virtual void Replace(string url, Image img)
        {
            _cacheImages[url] = img;
        }
    }
}