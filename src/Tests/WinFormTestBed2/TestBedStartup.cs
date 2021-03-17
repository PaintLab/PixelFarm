﻿//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LayoutFarm.UI;

namespace YourImplementation
{
    using PixelFarm.Drawing;
    using Typography.FontCollections;

    public static class TestBedStartup
    {

        public static class InstalledTypefaceCollectionMx
        {
            //APPLICATION specific code
            static InstalledTypefaceCollection s_intalledTypefaces;
            public static InstalledTypefaceCollection GetDefaultInstalledTypefaceCollection() => s_intalledTypefaces;
            public static void Setup()
            {
                if (s_intalledTypefaces != null)
                {
                    //once
                    return;
                }
                s_intalledTypefaces = new InstalledTypefaceCollection();
                s_intalledTypefaces.SetFontNameDuplicatedHandler((existing, newone) => FontNameDuplicatedDecision.Skip);

                s_intalledTypefaces.SetFontNotFoundHandler(req =>
                {
                    if (req.foundSameNames != null)
                    {
                        //weight may not match--
                        return InstalledTypefaceCollectionExtensions.ResolveFontWeightMissing(req, req.foundSameNames);
                    }

                    //This is application specific ***                    
                    switch (req.fontName.ToUpper())
                    {
                        default:
                            {

                            }
                            break;
                        case "SANS-SERIF":
                            {
                                //temp fix
                                InstalledTypeface ss = req.typefaceCollection.GetInstalledTypeface("Microsoft Sans Serif", TypefaceStyle.Regular, (ushort)RequestFontWeight.Normal);
                                if (ss != null)
                                {
                                    return ss;
                                }
                            }
                            break;
                        case "SERIF":
                            {
                                //temp fix
                                InstalledTypeface ss = req.typefaceCollection.GetInstalledTypeface("Palatino linotype", TypefaceStyle.Regular, (ushort)RequestFontWeight.Normal);
                                if (ss != null)
                                {
                                    return ss;
                                }
                            }
                            break;
                        case "TAHOMA":
                            {
                                switch (req.style)
                                {
                                    case TypefaceStyle.Italic:
                                        {
                                            InstalledTypeface anotherCandidate = req.typefaceCollection.GetInstalledTypeface(req.fontName, TypefaceStyle.Italic, (ushort)RequestFontWeight.Normal);
                                            if (anotherCandidate != null)
                                            {
                                                return anotherCandidate;
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                        case "MONOSPACE":
                            //use Courier New
                            return req.typefaceCollection.GetInstalledTypeface("Courier New", TypefaceStyle.Regular, (ushort)RequestFontWeight.Normal);
                        case "HELVETICA":
                            return req.typefaceCollection.GetInstalledTypeface("Arial", TypefaceStyle.Regular, (ushort)RequestFontWeight.Normal);
                    }
                    return null;
                });


                //if you don't want to load entire system fonts
                //then you can add only specfic font by yourself
                //when the service can' resolve the requested font

                s_intalledTypefaces.LoadSystemFonts();
                s_intalledTypefaces.LoadFontsFromFolder(@"D:\projects\Typography\Demo\Windows\TestFonts");//demo
                s_intalledTypefaces.LoadFontsFromFolder(@"D:\projects\Typography\Demo\Windows\Test_PrivateFonts");//demo
                s_intalledTypefaces.UpdateUnicodeRanges();

                YourImplementation.CommonTextServiceSetup.SetInstalledTypefaceCollection(s_intalledTypefaces);
                InstalledTypefaceCollection.SetAsSharedTypefaceCollection(s_intalledTypefaces);
            }
        }

        public static void Setup()
        {

            if (CommonTextServiceSetup.FontLoader == null)
            {
                InstalledTypefaceCollectionMx.Setup();
                CommonTextServiceSetup.SetInstalledTypefaceCollection(InstalledTypefaceCollectionMx.GetDefaultInstalledTypefaceCollection());
            }


            PixelFarm.DrawingGL.CachedBinaryShaderIO.SetActualImpl(
               () => new PixelFarm.DrawingGL.LocalFileCachedBinaryShaderIO(Application.CommonAppDataPath));

            FrameworkInitGLES.SetupDefaultValues();

            //TODO: review namespace***
            var pars = new PixelFarm.Platforms.ImageIOSetupParameters();
            pars.SaveToPng = (IntPtr imgBuffer, int stride, int width, int height, string filename) =>
            {
                using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSize(imgBuffer, newBmp);
                    //save
                    newBmp.Save(filename);
                }
            };
            pars.ReadFromMemStream = (System.IO.MemoryStream ms, string kind) =>
            {
                //read  
                //TODO: review here again
                using (System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(ms))
                {
                    PixelFarm.CpuBlit.MemBitmap memBmp = new PixelFarm.CpuBlit.MemBitmap(gdiBmp.Width, gdiBmp.Height);
                    //#if DEBUG
                    //                        memBmp._dbugNote = "img;
                    //#endif

                    PixelFarm.CpuBlit.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(gdiBmp, memBmp);
                    return memBmp;
                }

            };
            PixelFarm.Platforms.ImageIOPortal.Setup(pars);

            //you can use your font loader
            YourImplementation.FrameworkInitWinGDI.SetupDefaultValues();
            //default text breaker, this bridge between 
            LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker();
        }
        public static void Close()
        {
            LayoutFarm.UIPlatform.Close();
        }

        public static void RunDemoList(System.Reflection.Assembly asm)
        {

            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ////------------------------------- 
            var formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(asm);
            Application.Run(formDemoList);
        }
        /// <summary>
        /// close our application system
        /// </summary>

#if DEBUG
        public static bool dbugShowLayoutInspectorForm { get; set; }
#endif
        public static Form RunSpecificDemo(LayoutFarm.App demo,
            LayoutFarm.AppHost appHost,
            InnerViewportKind innerViewportKind = InnerViewportKind.GdiPlusOnGLES)
        {
            System.Drawing.Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            Form formCanvas = LayoutFarm.UI.FormCanvasHelper.CreateNewFormCanvas(
               workingArea.Width,
               workingArea.Height,
               innerViewportKind,
               out GraphicsViewRoot viewroot);
#if DEBUG
            formCanvas.Text = innerViewportKind.ToString();
#endif


            formCanvas.FormClosed += (s, e) =>
            {
                demo.OnClosing();
                demo.OnClosed();
            };


            LayoutFarm.AppHostConfig config = new LayoutFarm.AppHostConfig();
            YourImplementation.UISurfaceViewportSetupHelper.SetUISurfaceViewportControl(config, viewroot);
            appHost.Setup(config);

            appHost.StartApp(demo);
            //
            //viewroot.TopDownRecalculateContent();
            //==================================================  
            viewroot.PaintToOutputWindow();

            //formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
#if DEBUG
            if (dbugShowLayoutInspectorForm)
            {
                LayoutInspectorUtils.ShowFormLayoutInspector(viewroot);

            }
#endif

            //Application.Run(formCanvas);

            formCanvas.Show();
            return formCanvas;
        }


    }
    public static class LayoutInspectorUtils
    {

        public static void ShowFormLayoutInspector(LayoutFarm.UI.GraphicsViewRoot viewroot)
        {
            var formLayoutInspector = new LayoutFarm.Dev.FormLayoutInspector();
            formLayoutInspector.Show();

            formLayoutInspector.Connect(viewroot);
            formLayoutInspector.Show();
        }
    }

    public static class DemoFormCreatorHelper
    {
        public static void CreateReadyForm(
         InnerViewportKind innerViewportKind,
         out GraphicsViewRoot viewroot,
         out Form formCanvas)
        {

            //1. select view port kind  

            var workingArea = Screen.PrimaryScreen.WorkingArea;

            formCanvas = FormCanvasHelper.CreateNewFormCanvas(
              workingArea.Width,
              workingArea.Height,
              innerViewportKind,
              out viewroot);

            formCanvas.Text = "FormCanvas 1 :" + innerViewportKind;

            viewroot.PaintToOutputWindow();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
        }
    }


}