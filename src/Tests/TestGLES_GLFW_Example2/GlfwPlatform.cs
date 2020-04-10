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
        public override void SetClipboardData(string textData)
        {
            //TODO: implement 
        }

        protected override Cursor CreateCursorImpl(CursorRequest curReq) => new GlfwCursor();

        public override void SetClipboardImage(Image img)
        {

        }
        public override List<string> GetClipboardFileDropList()
        {
            return null;
        }

        public override Image GetClipboardImage()
        {
            return null;
        }

    }
    class GlfwCursor : Cursor
    {

    }
}