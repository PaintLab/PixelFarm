//MIT, 2016-present, WinterDev 

using PixelFarm.Drawing;

namespace PixelFarm.CpuBlit.VertexProcessing
{

    public class LineDashGenerator : IDashGenerator
    {
        public struct DashSegment
        {
            public readonly float Len;
            public readonly bool IsSolid;
            public DashSegment(float len, bool isSolid)
            {
                Len = len;
                IsSolid = isSolid;
            }
            public override string ToString() => (IsSolid ? "s" : "b") + Len;
        }

        LineWalker _dashGenLineWalker;
        DashSegment[] _staicDashSegments = null;
        string _patternAsString;

        public LineDashGenerator()
        {

        }
        public void SetDashPattern(float solid, float blank)
        {
            IsStaticPattern = true;
            _staicDashSegments = new DashSegment[] { new DashSegment(solid, true), new DashSegment(blank, false) };
            _patternAsString = null;
            _dashGenLineWalker = new LineWalker(); //TODO: reuse the walker
            _dashGenLineWalker.AddMark(solid, LineWalkDashStyle.Solid);
            _dashGenLineWalker.AddMark(blank, LineWalkDashStyle.Blank);
        }
        public void SetDashPattern(float solid0, float blank0, float solid1, float blank1)
        {
            IsStaticPattern = true;
            _staicDashSegments = new DashSegment[] {
                new DashSegment(solid0, true),
                new DashSegment(blank0, false),
                new DashSegment(solid1, true),
                new DashSegment(blank1, false)
            };
            _patternAsString = null;
            _dashGenLineWalker = new LineWalker(); //TODO: reuse the walker
            _dashGenLineWalker.AddMark(solid0, LineWalkDashStyle.Solid);
            _dashGenLineWalker.AddMark(blank0, LineWalkDashStyle.Blank);
            //
            _dashGenLineWalker.AddMark(solid1, LineWalkDashStyle.Solid);
            _dashGenLineWalker.AddMark(blank1, LineWalkDashStyle.Blank);
        }

        public void SetDashPattern(LineWalker lineWalker)
        {
            IsStaticPattern = false;
            _staicDashSegments = null;
            _dashGenLineWalker = lineWalker;
        }

        public DashSegment[] GetStaticDashSegments() => _staicDashSegments;

        public bool IsStaticPattern { get; set; }

        public string GetPatternAsString()
        {
            if (IsStaticPattern)
            {
                //create 
                if (_patternAsString == null)
                {
                    //TODO: string builder pool
                    _patternAsString = "";
                    for (int i = 0; i < _staicDashSegments.Length; ++i)
                    {
                        _patternAsString += _staicDashSegments[i].ToString();
                    }
                }
                return _patternAsString;
            }
            return null;
        }
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