//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using PixelFarm.CpuBlit;
using PixelFarm.Drawing;

namespace LayoutFarm.UI
{

    //platform specific code

    static class UITimerManager
    {
        static Timer s_uiTimer = new Timer();
        //we create a hidden window form for invoke other task in 
        //UI thread 
        static Form s_msg_window = new Form();
        static bool s_isInit = false;
        static UITimerManager()
        {
        }
        static bool s_readyToInvoke;
        public delegate void SimpleAction();
        static SimpleAction s_timerAction;
        static SimpleAction s_tickAction;

        public static void Init(int minInterval, SimpleAction timerAction)
        {
            if (s_isInit) return;
            //
            s_isInit = true;

            s_timerAction = timerAction;

            s_tickAction = new SimpleAction(() =>
            {
                //stop all timer
                //TODO: review here not use enable
                //we should use field flags to stop/start 
                s_uiTimer.Enabled = false; //temporary pause 
                bool err = false;
                //try
                //{
                s_timerAction();
                //}
                //catch (Exception ex)
                //{
                //    err = true;
                //}

#if DEBUG
                if (err)
                {
                    dbugLatestUIMsgQueueErr.dbug_s_latestTask.InvokeAction();
                }
#endif

                s_uiTimer.Enabled = true;//enable again 
            });


            s_msg_window.Visible = false;
            s_uiTimer.Interval = minInterval;
            s_uiTimer.Tick += timer_tick;
            // 
            //force form to created?
            IntPtr formHandle = s_msg_window.Handle;
            s_uiTimer.Enabled = true;//last


        }
        static void timer_tick(object sender, System.EventArgs e)
        {
            if (!s_readyToInvoke)
            {
                if (s_msg_window != null)
                {
                    s_readyToInvoke = true;
                }
                return;
            }
            s_msg_window.Invoke(s_tickAction);
            //TODO: review here,again eg.post custom msg to the window event queue?
        }
    }


    public class UIPlatformWinForm : UIPlatform
    {
        static UIPlatformWinForm s_platform;
        //TODO: review how to adjust this value
        const int UI_MSG_TIMER_INTERVAL = 5;

        static UIPlatformWinForm()
        {
            //actual timer
            //for msg queue
            //
            SetUIMsgMinTimerCounterBackInMillisec(UI_MSG_TIMER_INTERVAL);
            UITimerManager.Init(UI_MSG_TIMER_INTERVAL, InvokeMsgPumpOneStep);
        }

        public static UIPlatformWinForm GetDefault()
        {
            return s_platform;
        }

        public UIPlatformWinForm()
        {
            //--------------------------------------------------------------------
            //TODO: review here again
            //NOTE: this class load native dll images (GLES2)
            //since GLES2 that we use is x86, 
            //so we must specific the file type to x86 ***
            //else this will error on TypeInitializer ( from BadImageFormatException);
            //--------------------------------------------------------------------

            if (s_platform == null)
            {
                s_platform = this;
                SetAsDefaultPlatform();
            }

            try
            {
                //set up winform platform 
                ////gdi+
                //PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetInstalledTypefaceProvider(installedTypefaces);
                LayoutFarm.UI.Clipboard.SetUIPlatform(this);
            }
            catch (System.Exception ex)
            {

            }
            ////gles2
            //OpenTK.Toolkit.Init();
            //PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);
            ////skia 
            //if (!YourImplementation.BootStrapSkia.IsNativeLibAvailable())
            //{
            //    //handle  when native dll is not ready
            //}
            //else
            //{
            //    //when ready
            //    PixelFarm.Drawing.Skia.SkiaGraphicsPlatform.SetFontLoader(YourImplementation.BootStrapSkia.myFontLoader);
            //}
            //_gdiPlusIFonts = new PixelFarm.Drawing.WinGdi.Gdi32IFonts();
        }

        public override Size GetPrimaryMonitorSize()
        {
            var prim_workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            return new Size(prim_workingArea.Width, prim_workingArea.Height);
        }
        public override void ClearClipboardData()
        {
            System.Windows.Forms.Clipboard.Clear();
        }
        public override string GetClipboardText()
        {
            return System.Windows.Forms.Clipboard.GetText();
        }
        public override bool ContainsClipboardData(string datatype)
        {
            switch (datatype)
            {
                case "text":
                    return System.Windows.Forms.Clipboard.ContainsText();
                case "image":
                    return System.Windows.Forms.Clipboard.ContainsImage();
                case "filedrops":
                    return System.Windows.Forms.Clipboard.ContainsFileDropList();
            }
            return false;
        }
        public override object GetClipboardData(string dataformat)
        {
            return System.Windows.Forms.Clipboard.GetData(dataformat);
        }
        public override void SetClipboardText(string textData)
        {
            if (!string.IsNullOrEmpty(textData))
            {
                System.Windows.Forms.Clipboard.SetText(textData);
            }
            else
            {
                System.Windows.Forms.Clipboard.Clear();
            }
        }
        public override IEnumerable<string> GetClipboardFileDropList()
        {
            foreach (string s in System.Windows.Forms.Clipboard.GetFileDropList())
            {
                yield return s;
            }
        }
        public override void SetClipboardFileDropList(string[] filedrops)
        {
            var stringCollection = new System.Collections.Specialized.StringCollection();
            stringCollection.AddRange(filedrops);
            System.Windows.Forms.Clipboard.SetFileDropList(stringCollection);
        }
        public override Image GetClipboardImage()
        {

            if (System.Windows.Forms.Clipboard.GetImage() is System.Drawing.Bitmap bmp)
            {
                MemBitmap memBmp = new MemBitmap(bmp.Width, bmp.Height);

                PixelFarm.CpuBlit.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(bmp, memBmp);
                return memBmp;
            }
            return null;
        }

        public override void SetClipboardImage(Image img)
        {
            if (img is MemBitmap memBmp)
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(memBmp.Width, memBmp.Height);
                using (TempMemPtr tmp = MemBitmap.GetBufferPtr(memBmp))
                {
                    PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSize(tmp.Ptr, bmp);
                }
            }
        }

        protected override Cursor CreateCursorImpl(CursorRequest cursorReq)
        {
            MyCursor myCursor = new MyCursor();
            //load cursor from input file
            //assume cursor is square
            //resolve
            if (cursorReq.Url.StartsWith("system:"))
            {
                System.Windows.Forms.Cursor selectedCursor = System.Windows.Forms.Cursors.Default;
                switch (cursorReq.Url.Substring(7))
                {
                    case "Arrow":
                        selectedCursor = System.Windows.Forms.Cursors.Arrow;
                        break;
                    case "Pointer":
                        selectedCursor = System.Windows.Forms.Cursors.Hand;
                        break;
                    case "IBeam":
                        selectedCursor = System.Windows.Forms.Cursors.IBeam;
                        break;
                }

                myCursor.LoadSystemCursor(selectedCursor);
            }
            else
            {
                myCursor.LoadFromFile(cursorReq.Url, cursorReq.Width);
            }
            return myCursor;
        }



        public class MyCursor : Cursor, IDisposable
        {
            IntPtr _nativeCursorHandler;
            System.Windows.Forms.Cursor _cursor;
            bool _isSystemCur;
            public void Dispose()
            {
                if (_isSystemCur)
                {
                    _cursor = null;
                }
                else
                {
                    if (_cursor != null)
                    {
                        _cursor.Dispose();
                        _cursor = null;
                    }
                    if (_nativeCursorHandler != IntPtr.Zero)
                    {
                        DestroyCursor(_nativeCursorHandler);
                        _nativeCursorHandler = IntPtr.Zero;
                    }
                }

            }
            internal void LoadFromFile(string cursorUrl, int width)
            {
                //TODO: review here,
                //we should load from 'virtual disk'
                _nativeCursorHandler = LoadImage(IntPtr.Zero, cursorUrl, 2, width, width, LR_DEFAULTSIZE | LR_LOADFROMFILE);
                _cursor = new System.Windows.Forms.Cursor(_nativeCursorHandler);
            }
            internal void LoadSystemCursor(System.Windows.Forms.Cursor systemCur)
            {
                _isSystemCur = true;
                _cursor = systemCur;
            }

            public System.Windows.Forms.Cursor WinFormCursor => _cursor;


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


}