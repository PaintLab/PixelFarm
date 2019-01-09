// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
// 
//--------------------
//Copyright (c) 2014, Lars Brubaker
//All rights reserved.

//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met:

//1. Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//2. Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
//ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

//The views and conclusions contained in the software and documentation are those
//of the authors and should not be interpreted as representing official policies,
//either expressed or implied, of the FreeBSD Project.
//--------------------
//BSD, 2019-present, WinterDev


using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using Mini;
using PixelFarm.CpuBlit.Imaging;

//image_filters2.cpp
namespace PixelFarm.CpuBlit.ImgFilterDemo
{
    
    [Info(OrderCode = "23", AvailableOn = AvailableOn.Agg)]
    [Info(DemoCategory.Bitmap, "img filter2")]
    public class ImgFilter2Demo : DemoBase
    {

        ImageFilterBilinear _filterBilinear = new ImageFilterBilinear();
        ImageFilterBicubic _filterBicubic = new ImageFilterBicubic();

        public ImgFilter2Demo()
        {

        }
        public override void Draw(Painter p)
        {
            PixelFarm.CpuBlit.Imaging.IImageFilterFunc selectedFilter = null;
            switch (Filter)
            {
                case FilterName.Bicubic:
                    selectedFilter = _filterBicubic;
                    break;
                case FilterName.Bilinear:
                    selectedFilter = _filterBilinear;
                    break;
                default:
                    break;
            }

            if (selectedFilter == null) return;
            //
            double radius = selectedFilter.GetRadius();
            

            base.Draw(p);
        }

        [DemoConfig]
        public FilterName Filter
        {
            get;
            set;
        }

    }



  
}
