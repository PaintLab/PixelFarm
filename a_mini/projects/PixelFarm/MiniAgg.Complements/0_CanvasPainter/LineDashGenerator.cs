//MIT, 2016-2017, WinterDev 
using System.Collections.Generic;
using PixelFarm.Agg.VertexSource;
namespace PixelFarm.Agg
{


    public class LineDashGenerator
    {
        List<LineWalker> _dashWalkers = new List<LineWalker>();
        LineWalker _dashGenLineWalker; //current
        int currentPatternNum = 0;
        public LineDashGenerator()
        {
            _dashWalkers.Add(null);//first 0 is null
        }
        public int CreatePattern(float solid, float blank)
        {
            //***
            //you can customize what happend with the line segment
            LineWalker dashGenLineWalker = new LineWalker();
            dashGenLineWalker.AddMark(solid, LineWalkDashStyle.Solid);
            dashGenLineWalker.AddMark(blank, LineWalkDashStyle.Blank);
            _dashWalkers.Add(dashGenLineWalker);
            return _dashWalkers.Count - 1;
        }
        public int CreatePattern(float solid0, float blank0, float solid1, float blank1)
        {
            LineWalker dashGenLineWalker = new LineWalker();
            dashGenLineWalker.AddMark(solid0, LineWalkDashStyle.Solid);
            dashGenLineWalker.AddMark(blank0, LineWalkDashStyle.Blank);
            //
            dashGenLineWalker.AddMark(solid1, LineWalkDashStyle.Solid);
            dashGenLineWalker.AddMark(blank1, LineWalkDashStyle.Blank);
            _dashWalkers.Add(dashGenLineWalker);
            return _dashWalkers.Count - 1;
        }
        public void SetCurrentPattern(int currentPatternNumber)
        {
            if (currentPatternNum < _dashWalkers.Count)
            {
                currentPatternNum = currentPatternNumber;
                _dashGenLineWalker = _dashWalkers[currentPatternNumber];
            }
            else
            {
                currentPatternNum = 0;
                _dashGenLineWalker = null;
            }
        }
        public int CurrentPatternNum
        {
            get { return this.currentPatternNum; }
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