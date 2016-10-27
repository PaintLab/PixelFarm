﻿//MIT, 2014-2016, WinterDev 
using System;
using System.Collections.Generic;

namespace PixelFarm.Drawing
{

    public abstract class GraphicsPlatform
    {

        public abstract Canvas CreateCanvas(
            int left,
            int top,
            int width,
            int height,
            CanvasInitParameters canvasInitPars = new CanvasInitParameters());


        /// <summary>
        /// font management system for this graphics platform
        /// </summary>
        public abstract IFonts Fonts { get; }


        //----------------------------------------------------------------------
        //set provider delegates before use it from comment graphics platform
        //----------------------------------------------------------------------

        //1.installed fonts provider
        static IEnumerable<string> s_installedFontProviderIter;
        public static void SetInstalledFontProvider(IEnumerable<string> installedFontProviderIter)
        {
            s_installedFontProviderIter = installedFontProviderIter;
        }
        //----------------------
        //2. image buffer provider from filename
        static ImageBufferProviderDelegate s_imgBufferProviderDel;
        public static void SetImageBufferProviderDelegate(ImageBufferProviderDelegate imgBufferProviderDel)
        {
            s_imgBufferProviderDel = imgBufferProviderDel;
        }
        internal static IEnumerable<string> GetInstalledFontIter()
        {
            return s_installedFontProviderIter;
        }

    }

    public delegate byte[] ImageBufferProviderDelegate(string filename);

    public struct CanvasInitParameters
    {
        public object externalCanvas;
        public CanvasBackEnd canvasBackEnd;

        internal bool IsEmpty()
        {
            return externalCanvas == null && canvasBackEnd == CanvasBackEnd.Software;
        }
    }

}