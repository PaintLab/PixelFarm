//MIT, 2016-present, WinterDev 
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace PixelFarm.CpuBlit.VertexProcessing
{
    public class LineDashGenerator : IDashGenerator
    {

        LineWalker _dashGenLineWalker;
        float[] _preBuiltPattern = null;
        public LineDashGenerator()
        {

        }
        public void SetDashPattern(float solid, float blank)
        {
            IsPrebuiltPattern = true;
            _preBuiltPattern = new float[] { solid, blank };
            _dashGenLineWalker = new LineWalker();
            _dashGenLineWalker.AddMark(solid, LineWalkDashStyle.Solid);
            _dashGenLineWalker.AddMark(blank, LineWalkDashStyle.Blank);
        }
        public void SetDashPattern(float solid0, float blank0, float solid1, float blank1)
        {
            IsPrebuiltPattern = true;
            _preBuiltPattern = new float[] { solid0, blank0, solid1, blank1 };

            _dashGenLineWalker = new LineWalker();
            _dashGenLineWalker.AddMark(solid0, LineWalkDashStyle.Solid);
            _dashGenLineWalker.AddMark(blank0, LineWalkDashStyle.Blank);
            //
            _dashGenLineWalker.AddMark(solid1, LineWalkDashStyle.Solid);
            _dashGenLineWalker.AddMark(blank1, LineWalkDashStyle.Blank);
        }

        public void SetDashPattern(LineWalker lineWalker)
        {
            IsPrebuiltPattern = false;
            _preBuiltPattern = null;
            _dashGenLineWalker = lineWalker;
        }

        public float[] GetPrebuiltPattern() => _preBuiltPattern;

        public bool IsPrebuiltPattern { get; set; }

        public void CreateDash(VertexStore srcVxs, VertexStore output)
        {
            if (_dashGenLineWalker == null)
            {
                return;
            }
            //-------------------------------
            _dashGenLineWalker.Walk(srcVxs, output);
        }
    }
}