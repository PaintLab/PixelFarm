//MIT 2014, WinterDev   
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using PixelFarm;
using PixelFarm.Agg;

namespace Mini
{
    public class WinAggContex
    {
        Graphics destGraphics;
        public WinAggContex(Graphics destGraphics)
        {
            this.destGraphics = destGraphics;
        }
    }

}