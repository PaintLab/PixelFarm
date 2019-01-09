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
    public enum FilterName
    {
        Unknown,
        Bilinear,
        Bicubic,
    }
    [Info(OrderCode = "23", AvailableOn = AvailableOn.Agg)]
    [Info(DemoCategory.Bitmap, "Image Filters Comparison")]
    public class ImgFilter1Demo : DemoBase
    {
        MemBitmap _orgImg;
        MemBitmap _tmpDestImg;
        MemBitmap _rotatedImg;


        ImageFilterLookUpTable _lut;
        FilterName _selectedFilterName;

        ImageFilterBicubic _filterFuncBicubic = new ImageFilterBicubic();
        ImageFilterBilinear _filterFuncBilinear = new ImageFilterBilinear();
        PixelFarm.CpuBlit.FragmentProcessing.ImgSpanGenRGBA_CustomFilter _imgSpanGenCustom;


        public ImgFilter1Demo()
        {
            _orgImg = PixelFarm.Platforms.StorageService.Provider.ReadPngBitmap("../Data/spheres.png");
            _lut = new ImageFilterLookUpTable();
            Normalization = true;//default
            _imgSpanGenCustom = new FragmentProcessing.ImgSpanGenRGBA_CustomFilter();
        }
        public override void Draw(Painter p)
        {
            AggPainter painter = p as AggPainter;
            if (painter == null) return;

            if (Filter == FilterName.Unknown)
            {
                painter.RenderSurface.CustomImgSpanGen = null;
            }
            else
            {
                painter.RenderSurface.CustomImgSpanGen = _imgSpanGenCustom;
            }


            p.DrawImage(_orgImg);
            base.Draw(p);
        }

        [DemoConfig]
        public FilterName Filter
        {
            get => _selectedFilterName;
            set
            {
                if (_selectedFilterName == value) return;
                //                 
                switch (_selectedFilterName = value)
                {
                    case FilterName.Unknown:
                        return;
                    case FilterName.Bicubic:
                        _lut.Rebuild(_filterFuncBicubic, Normalization);
                        _imgSpanGenCustom.SetLookupTable(_lut);
                        break;
                    case FilterName.Bilinear:
                        _lut.Rebuild(_filterFuncBilinear, Normalization);
                        _imgSpanGenCustom.SetLookupTable(_lut);
                        break;
                }
            }

        }
        [DemoConfig]
        public bool Normalization
        {
            get;
            set;
        }
    }
}

