//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImgCodec.IconAndCursor
{
    public partial class Form1 : Form
    {
        Cursor _myCustomCursor1;
        IntPtr _myCustomCursorHandle; 
        Icon _myIcon;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //create a simple bitmap for our color
            using (Bitmap temp = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.Clear(Color.Transparent);
                //
                using (Brush solid1 = new SolidBrush(Color.FromArgb(150, Color.Green))) //alpha test
                {
                    g.FillRectangle(solid1, new RectangleF(1, 1, 10, 10));
                    g.FillRectangle(Brushes.Yellow, new RectangleF(3, 3, 4, 4));
                }
                //-------------------------------------------------------

                //create blank cursor file
                IconMaker.CursorFile curFile = new IconMaker.CursorFile();
                //copy bitmap from temp to 
                var iconBmp = new IconMaker.WindowBitmap(temp.Width, temp.Height);
                var bmpdata = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int[] rawBmpData = new int[temp.Width * temp.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, rawBmpData, 0, rawBmpData.Length);
                temp.UnlockBits(bmpdata);

                iconBmp.RawBitmapData = rawBmpData;

                curFile.AddBitmap(iconBmp, 1, 1);//add bitmap with hotspot

                curFile.Save("myicon.cur");

                //save to temp file

                _myCustomCursorHandle = LoadImage(IntPtr.Zero, "myicon.cur", 2, 0, 0, LR_DEFAULTSIZE | LR_LOADFROMFILE);
                _myCustomCursor1 = new Cursor(_myCustomCursorHandle);
                this.Cursor = _myCustomCursor1;
            }            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            IntPtr hcursor = LoadCursorFromFileA("Cursor1.cur");
            this.Cursor = new System.Windows.Forms.Cursor(hcursor);
        }

        [System.Runtime.InteropServices.DllImport("User32")]
        static extern IntPtr LoadCursorFromFileA(string lpFileName);

        [System.Runtime.InteropServices.DllImport("User32")]
        static extern bool DestroyCursor(IntPtr hCursor);

        [System.Runtime.InteropServices.DllImport("User32")]
        static extern bool DestroyIcon(IntPtr hIcon);


        [System.Runtime.InteropServices.DllImport("User32")]
        static extern IntPtr LoadImage(
                IntPtr hInst,
                string name,

                uint type, //0=bitmap, 2=cursor,1=icon
                int cx, //request width
                int cy, //request height
                uint fuLoad
        );

        const uint LR_DEFAULTSIZE = 0x00000040;
        const uint LR_LOADFROMFILE = 0x00000010;

        private void button4_Click(object sender, EventArgs e)
        {
            //create a simple bitmap for our color
            using (Bitmap temp = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.Clear(Color.Transparent);
                //
                using (Brush solid1 = new SolidBrush(Color.FromArgb(150, Color.Green))) //alpha test
                {
                    g.FillRectangle(solid1, new RectangleF(1, 1, 10, 10));
                    g.FillRectangle(Brushes.Yellow, new RectangleF(3, 3, 4, 4));
                }
                //-------------------------------------------------------

                //create blank cursor file
                IconMaker.IconFile icoFile = new IconMaker.IconFile();
                //copy bitmap from temp to 
                var iconBmp = new IconMaker.WindowBitmap(temp.Width, temp.Height);
                var bmpdata = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int[] rawBmpData = new int[temp.Width * temp.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, rawBmpData, 0, rawBmpData.Length);
                temp.UnlockBits(bmpdata);

                iconBmp.RawBitmapData = rawBmpData;

                icoFile.AddBitmap(iconBmp);//add bitmap with hotspot
                icoFile.Save("myicon.ico");

                //save to temp file

                _myIcon = new Icon("myicon.ico");
                this.Icon = _myIcon;
            }

        }

        //LR_CREATEDIBSECTION
        //0x00002000

        //	When the uType parameter specifies IMAGE_BITMAP, causes the function to return a DIB section bitmap rather than a compatible bitmap. This flag is useful for loading a bitmap without mapping it to the colors of the display device.

        //LR_DEFAULTCOLOR
        //0x00000000

        //	The default flag; it does nothing. All it means is "not LR_MONOCHROME".

        //LR_DEFAULTSIZE
        //0x00000040

        //	Uses the width or height specified by the system metric values for cursors or icons, if the cxDesired or cyDesired values are set to zero. If this flag is not specified and cxDesired and cyDesired are set to zero, the function uses the actual resource size. If the resource contains multiple images, the function uses the size of the first image.

        //LR_LOADFROMFILE
        //0x00000010

        //	Loads the stand-alone image from the file specified by lpszName (icon, cursor, or bitmap file).

        //LR_LOADMAP3DCOLORS
        //0x00001000

        //	Searches the color table for the image and replaces the following shades of gray with the corresponding 3-D color.

        //    Dk Gray, RGB(128,128,128) with COLOR_3DSHADOW
        //    Gray, RGB(192,192,192) with COLOR_3DFACE
        //    Lt Gray, RGB(223,223,223) with COLOR_3DLIGHT

        //Do not use this option if you are loading a bitmap with a color depth greater than 8bpp.

        //LR_LOADTRANSPARENT
        //0x00000020

        //	Retrieves the color value of the first pixel in the image and replaces the corresponding entry in the color table with the default window color (COLOR_WINDOW). All pixels in the image that use that entry become the default window color. This value applies only to images that have corresponding color tables.

        //Do not use this option if you are loading a bitmap with a color depth greater than 8bpp.

        //If fuLoad includes both the LR_LOADTRANSPARENT and LR_LOADMAP3DCOLORS values, LR_LOADTRANSPARENT takes precedence. However, the color table entry is replaced with COLOR_3DFACE rather than COLOR_WINDOW.

        //LR_MONOCHROME
        //0x00000001

        //	Loads the image in black and white.

        //LR_SHARED
        //0x00008000

        //	Shares the image handle if the image is loaded multiple times. If LR_SHARED is not set, a second call to LoadImage for the same resource will load the image again and return a different handle.

        //When you use this flag, the system will destroy the resource when it is no longer needed.

        //Do not use LR_SHARED for images that have non-standard sizes, that may change after loading, or that are loaded from a file.

        //When loading a system icon or cursor, you must use LR_SHARED or the function will fail to load the resource.

        //This function finds the first image in the cache with the requested resource name, regardless of the size requested.

        //LR_VGACOLOR
        //0x00000080

        //	Uses true VGA colors. 
    }
}
