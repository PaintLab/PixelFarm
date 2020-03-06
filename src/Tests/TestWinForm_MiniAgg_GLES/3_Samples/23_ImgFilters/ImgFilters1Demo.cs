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
using PixelFarm.CpuBlit.Imaging;

using Mini;

namespace PixelFarm.CpuBlit.ImgFilterDemo
{

    public enum FilterName
    {
        Unknown,
        NearestNeighbor,
        Bilinear,
        Bicubic,
        //others...
        Catrom,
        Gaussain,

    }
    [Info(OrderCode = "23", AvailableOn = AvailableOn.Agg)]
    [Info(DemoCategory.Bitmap, "Image Filters Comparison")]
    public class ImgFilter1Demo : DemoBase
    {
        MemBitmap _orgImg;

        ImageFilterLookUpTable _lut;
        FilterName _selectedFilterName;

        PixelFarm.CpuBlit.FragmentProcessing.ImgSpanGenRGBA_NN _imgSpanGenNN = new FragmentProcessing.ImgSpanGenRGBA_NN();
        PixelFarm.CpuBlit.FragmentProcessing.ImgSpanGenRGBA_CustomFilter _imgSpanGenCustom;
        float _thumbnailScaleDown = 0.25f;

        int _imgW, _imgH;
        double _rotationDeg;//rotation angle in degree
        public ImgFilter1Demo()
        {
            _orgImg = MemBitmap.LoadBitmap("../Data/spheres.png");
            _imgW = _orgImg.Width;
            _imgH = _orgImg.Height;

            _lut = new ImageFilterLookUpTable();
            Normalization = true;//default
            _imgSpanGenCustom = new FragmentProcessing.ImgSpanGenRGBA_CustomFilter();
        }
        public override void Draw(Painter p)
        {
            AggPainter painter = p as AggPainter;
            if (painter == null) return;

            painter.Clear(Color.White);

            switch (FilterName)
            {
                case FilterName.Unknown:
                    painter.RenderSurface.CustomImgSpanGen = null;
                    break;
                case FilterName.NearestNeighbor:
                    painter.RenderSurface.CustomImgSpanGen = _imgSpanGenNN;
                    break;
                default:
                    DrawWeightDistributionGraph(p, _lut.WeightArray);
                    painter.RenderSurface.CustomImgSpanGen = _imgSpanGenCustom;
                    break;
            }

            VertexProcessing.AffinePlan[] p1 = new VertexProcessing.AffinePlan[]
            {
                 VertexProcessing.AffinePlan.Translate(-_imgW /2.0,-_imgH /2.0),
                 VertexProcessing.AffinePlan.RotateDeg(_rotationDeg),
                 VertexProcessing.AffinePlan.Translate(_imgW /2.0,_imgH /2.0),
            };

            p.DrawImage(_orgImg, p1);

            if (_thumbnailScaleDown > 0 && _thumbnailScaleDown < 1)
            {
                using (MemBitmap thumbnail = _orgImg.CreateThumbnailWithSuperSamplingTechnique(_thumbnailScaleDown))
                {
                    painter.DrawImage(thumbnail, 400, 300);
                }
            }

            base.Draw(p);
        }
        void DrawWeightDistributionGraph(Painter p, int[] weightArr)
        {
            //draw a graph that show filter's weight distribution
            int scale = PixelFarm.CpuBlit.Imaging.ImageFilterLookUpTable.ImgFilterConst.SCALE;

            int graph_width = 200;
            int graph_height = 50;
            using (VxsTemp.Borrow(out var v1))
            {

                float oneStepW = graph_width / (float)weightArr.Length;
                double newX = 0, newY = 0;
                v1.AddMoveTo(newX, newY);
                for (int i = 0; i < weightArr.Length; ++i)
                {
                    double newvalue = weightArr[i] / (double)scale;
                    //System.Diagnostics.Debug.WriteLine(newvalue);
                    newX += oneStepW;
                    newY = -graph_height * newvalue;
                    v1.AddLineTo(newX, newY);
                }


                p.StrokeColor = Color.Black;
                p.StrokeWidth = 1;

                p.SetOrigin(0, 500);
                p.Draw(v1);
                p.SetOrigin(0, 0);
            }



        }
        [DemoConfig]
        public FilterName FilterName
        {
            get => _selectedFilterName;
            set
            {
                if (_selectedFilterName == value) return;
                //                 

                IImageFilterFunc selectedImgFilter = null;
                switch (_selectedFilterName = value)
                {
                    case FilterName.Unknown:
                        return;//**
                    case FilterName.NearestNeighbor:
                        return;
                    case FilterName.Bicubic:
                        selectedImgFilter = new ImageFilterBicubic();
                        break;
                    case FilterName.Bilinear:
                        selectedImgFilter = new ImageFilterBilinear();
                        break;
                    case FilterName.Catrom:
                        selectedImgFilter = new ImageFilterCatrom();
                        break;
                    case FilterName.Gaussain:
                        selectedImgFilter = new ImageFilterGaussian();
                        break;
                }
                _lut.Rebuild(selectedImgFilter, Normalization);
                _imgSpanGenCustom.SetLookupTable(_lut);
            }

        }
        [DemoConfig]
        public bool Normalization { get; set; }
        [DemoAction]
        public void RotateLeft()
        {
            _rotationDeg -= 20;
        }
        [DemoAction]
        public void RotateRight()
        {
            _rotationDeg += 20;
        }
        [DemoAction]
        public void Reset()
        {
            _rotationDeg = 0;
        }

    }
}

