//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using IconMaker;

namespace LayoutFarm
{
    [DemoNote("3.4_CursorTest")]
    class Demo_CursorTest : App
    {

        Cursor _myCursor;
        bool _showCustomCursor;

        protected override void OnStart(AppHost host)
        {
            var sampleButton = new LayoutFarm.CustomWidgets.Box(30, 30);
            host.AddChild(sampleButton);

            sampleButton.MouseDown += ((s, e2) =>
            {
                //click to create custom cursor
                if (!_showCustomCursor)
                {
                    if (_myCursor == null)
                    {
                        using (MemBitmap temp = new MemBitmap(16, 16))
                        using (Tools.Borrow(temp, out AggPainter p))
                        {

                            //1. create a simple bitmap for our cursor
                            p.Clear(Color.FromArgb(0, Color.White));
                            p.FillRect(1, 1, 10, 10, Color.FromArgb(150, Color.Green));
                            p.FillRect(3, 3, 4, 4, Color.Yellow);
                            //-------
                            //create cursor file

                            CursorFile curFile = new CursorFile();

                            var iconBmp = new WindowBitmap(temp.Width, temp.Height);
                            iconBmp.RawBitmapData = MemBitmap.CopyImgBuffer(temp);

                            curFile.AddBitmap(iconBmp, 1, 1);//add bitmap with hotspot
                            curFile.Save("myicon.cur");//save to temp file  
                            _myCursor = UIPlatform.CreateCursor(new CursorRequest("myicon.cur", 16));
                        }
                    }
                    e2.CustomMouseCursor = _myCursor;
                    _showCustomCursor = true;
                }
                else
                {
                    _showCustomCursor = false;
                    e2.MouseCursorStyle = MouseCursorStyle.Default;
                }

            });

            sampleButton.MouseMove += ((s, e2) =>
            {
                if (_showCustomCursor && _myCursor != null)
                {
                    e2.CustomMouseCursor = _myCursor;
                }
            });
        }

        private void SampleButton_MouseMove(object sender, UIMouseEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}