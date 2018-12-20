//BSD, 2014-present, WinterDev

/*
Copyright (c) 2013, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Collections.Generic;
using PixelFarm.VectorMath;

using Mini;
namespace PixelFarm.CpuBlit.Samples
{
    [Info(OrderCode = "21")]
    [Info("hand paint!")]
    public class HandPaintExample : DemoBase
    {
        Point _latestMousePoint;
        List<List<Point>> _pointSets = new List<List<Point>>();
         
        List<Point> _currentPointSet;// = new List<Point>();//current point list 
        public override void Init()
        {
        }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(Drawing.Color.White);
            var plistCount = _pointSets.Count;
            for (int n = 0; n < plistCount; ++n)
            {
                var contPoints = _pointSets[n];
                int pcount = contPoints.Count;
                for (int i = 1; i < pcount; ++i)
                {
                    Point p0 = contPoints[i - 1];
                    Point p1 = contPoints[i];
                    p.DrawLine(p0.x, p0.y, p1.x, p1.y);
                }
            }
        }

        public override void MouseDrag(int x, int y)
        {
            //add data to draw             
            _currentPointSet.Add(new Point(x, y));
        }
        public override void MouseDown(int x, int y, bool isRightButton)
        {
            _currentPointSet = new List<Point>();
            _pointSets.Add(_currentPointSet);
            _latestMousePoint = new Point(x, y);
            base.MouseDown(x, y, isRightButton);
        }
        public override void MouseUp(int x, int y)
        {
            //finish the current set
            base.MouseUp(x, y);
        }
    }
}

