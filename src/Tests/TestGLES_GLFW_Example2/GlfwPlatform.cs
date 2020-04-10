//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm;
using LayoutFarm.UI;
using PixelFarm.Drawing;
namespace PixelFarm.Forms
{
    public class GlfwPlatform : UIPlatform
    {
        public GlfwPlatform()
        {
            SetAsDefaultPlatform();
        }
        public override void ClearClipboardData()
        {

        }
        public override object GetClipboardData(string dataformat)
        {
            throw new NotImplementedException();
        }
        public override string GetClipboardText()
        {
            throw new NotImplementedException();
        }
        public override bool ContainsClipboardData(string datatype)
        {
            throw new NotImplementedException();
        }
         
          
        protected override Cursor CreateCursorImpl(CursorRequest curReq) => new GlfwCursor();

        public override void SetClipboardImage(Image img)
        {

        }
         
        public override Image GetClipboardImage()
        {
            return null;
        }

        public override IEnumerable<string> GetClipboardFileDropList()
        {
            throw new NotImplementedException();
        }

        public override void SetClipboardText(string textData)
        {
            throw new NotImplementedException();
        }

        public override void SetClipboardFileDropList(string[] filedrops)
        {
            throw new NotImplementedException();
        }
    }
    class GlfwCursor : Cursor
    {

    }
}