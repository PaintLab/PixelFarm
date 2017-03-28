//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
namespace Typography.Rendering
{
    enum PathPointKind : byte
    {
        Point,
        CurveControl
    }
    struct PathPoint
    {
        public readonly float x;
        public readonly float y;
        public readonly PathPointKind kind;
        public PathPoint(float x, float y, PathPointKind k)
        {
            this.x = x;
            this.y = y;
            this.kind = k;
        }

#if DEBUG
        public override string ToString()
        {
            return "(" + x + "," + y + ")" + ((kind == PathPointKind.Point) ? " p " : "c");
        }
#endif
    }


    class WritablePath : IWritablePath
    {
        //record all cmd 
        internal List<PathPoint> _points = new List<PathPoint>();
        float _latestX;
        float _latestY;
        float _lastMoveX;
        float _lastMoveY;
        bool _addMoveTo;

        public WritablePath()
        {

        }
        public void MoveTo(float x0, float y0)
        {
            _latestX = _lastMoveX = x0;
            _latestY = _lastMoveY = y0;
            _addMoveTo = true;

            //_points.Add(new PathPoint(_latestX = x0, _latestY = y0, PathPointKind.Point));
        }
        public void BezireTo(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            if (_addMoveTo)
            {
                _points.Add(new PathPoint(_latestX, _latestY, PathPointKind.Point));
                _addMoveTo = false;
            }
            _points.Add(new PathPoint(x1, y1, PathPointKind.CurveControl));
            _points.Add(new PathPoint(x2, y2, PathPointKind.CurveControl));
            _points.Add(new PathPoint(_latestX = x3, _latestY = y3, PathPointKind.Point));
        }
        public void CloseFigure()
        {
            if (_lastMoveX != _latestX ||
                _lastMoveY != _latestY)
            {
                _points.Add(new PathPoint(_lastMoveX, _lastMoveY, PathPointKind.Point));
            }
            _lastMoveX = _latestX;
            _lastMoveY = _latestY;
        }
        public void LineTo(float x1, float y1)
        {
            if (_addMoveTo)
            {
                _points.Add(new PathPoint(_latestX, _latestY, PathPointKind.Point));
                _addMoveTo = false;
            }
            _points.Add(new PathPoint(_latestX = x1, _latestY = y1, PathPointKind.Point));
        }
        //-------------------- 
    }


    class GlyphMesh
    {
        public WritablePath path;
        Typography.TextLayout.GlyphPlan glyphPlan;
        public float[] tessData;
        public int nElements;

        public GlyphMesh(WritablePath path, Typography.TextLayout.GlyphPlan glyphPlan)
        {
            this.glyphPlan = glyphPlan;
            this.path = path;
        }

        public float OffsetX
        {
            get { return glyphPlan.x; }
        }
        public float OffsetY
        {
            get { return glyphPlan.y; }
        }
    }
    class TextMesh
    {
        internal List<GlyphMesh> _glyphs = new List<GlyphMesh>();
        public void AddGlyph(GlyphMesh glyph)
        {
            _glyphs.Add(glyph);
        }

    }
}