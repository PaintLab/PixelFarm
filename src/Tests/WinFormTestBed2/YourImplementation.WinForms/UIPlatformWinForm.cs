//Apache2, 2014-present, WinterDev


using System;
using System.Windows.Forms;
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
                UIMsgQueue.RegisterRunOnceImpl(runOnceDelegate =>
                {
                    UIPlatform.RegisterRunOnceTask(tt => runOnceDelegate());
                });
            }

            //var installedTypefaces = new Typography.FontManagement.InstalledTypefaceCollection();
             

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


        public override void ClearClipboardData()
        {
            System.Windows.Forms.Clipboard.Clear();
        }
        public override string GetClipboardData()
        {
            return System.Windows.Forms.Clipboard.GetText();
        }
        public override void SetClipboardData(string textData)
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
    }
}