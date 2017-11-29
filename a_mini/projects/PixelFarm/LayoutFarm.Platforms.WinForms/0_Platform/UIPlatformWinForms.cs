//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace LayoutFarm.UI
{
    //platform specific code

    static class UITimerManager
    {
        static Timer s_uiTimer = new Timer();
        //we create a hidden window form for invoke other task in 
        //UI thread
        static List<UITimerTask> s_uiTimerTasks = new List<UITimerTask>();
        static Form s_msg_window = new Form();

        static bool s_isInit = false;
        const int MIN_INTERVAL = 10;
        static UITimerManager()
        {
            s_uiTimer.Interval = MIN_INTERVAL; //minimum msg queue timer ?, TODO: review here
        }
        public static void Init()
        {
            if (s_isInit) return;
            //
            s_isInit = true;
            s_msg_window.Visible = false;
            s_uiTimer.Tick += timer_tick;
            // 
            //force form to created?
            IntPtr formHandle = s_msg_window.Handle;
            s_uiTimer.Enabled = true;//last
        }

        static bool s_readyToInvoke;
        delegate void SimpleAction();

        private static void timer_tick(object sender, System.EventArgs e)
        {
            if (!s_readyToInvoke)
            {
                if (s_msg_window != null)
                {
                    s_readyToInvoke = true;
                }
                return;
            }
            s_msg_window.Invoke(new SimpleAction(() =>
            {
                //stop all timer
                s_uiTimer.Enabled = false; //temporary pause

                int j = s_uiTimerTasks.Count;
                for (int i = 0; i < j; ++i)
                {
                    UITimerTask ui_task = s_uiTimerTasks[i];
                    if (ui_task.Enabled)
                    {
                        UITimerTask.CountDown(ui_task, MIN_INTERVAL);
                    }
                }

                s_uiTimer.Enabled = true;//enable again
            }));
            //TODO: review here,again eg.post custom msg to the window event queue?
        }
        public static bool RegisterTimerTask(UITimerTask timerTask)
        {   

            if (timerTask.IsRegistered) return false;
            if (timerTask.IntervalInMillisec <= 0) return false;
            //
            s_uiTimerTasks.Add(timerTask);
            timerTask.IsRegistered = true;
            return true;
        }

    }


    public class UIPlatformWinForm : UIPlatform
    {
        static UIPlatformWinForm platform;
        static UIPlatformWinForm()
        {
            //actual timer
            //for msg queue
            //
            UITimerManager.Init();
        }
        public static UIPlatformWinForm GetDefault()
        {
            return platform;
        }
        protected override void InternalRegisterTimerTask(UITimerTask timerTask)
        {
            UITimerManager.RegisterTimerTask(timerTask);
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

            if (platform == null)
            {
                platform = this;
                SetAsDefaultPlatform();
            }

            var fontLoader = new PixelFarm.Drawing.Fonts.OpenFontStore();
            try
            {
                //set up winform platform 
                ////gdi+
                PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(fontLoader);
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
            System.Windows.Forms.Clipboard.SetText(textData);
        }


    }
}