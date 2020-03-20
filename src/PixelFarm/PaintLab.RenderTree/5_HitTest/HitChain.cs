//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    public struct HitInfo
    {
        public readonly Point point;

        object _hitObject;
        public static readonly HitInfo Empty = new HitInfo();
        public HitInfo(object hitObject, Point point)
        {
            this.point = point;
            _hitObject = hitObject;
        }
        //
        public RenderElement HitElemAsRenderElement => _hitObject as RenderElement;
        //
        public object HitElem => _hitObject;
        //
        public static bool operator ==(HitInfo pair1, HitInfo pair2)
        {
            return ((pair1._hitObject == pair2._hitObject) && (pair1.point == pair2.point));
        }
        public static bool operator !=(HitInfo pair1, HitInfo pair2)
        {
            return ((pair1._hitObject == pair2._hitObject) && (pair1.point == pair2.point));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

#if DEBUG
        public override string ToString()
        {
            RenderElement renderE = _hitObject as RenderElement;
            if (renderE != null)
            {
                object controller = renderE.GetController();
                if (controller != null)
                {
                    return point + " :" + renderE.ToString() + " " + renderE.RectBounds.ToString() + " " + renderE.GetType().Name + " , ctrl=" + controller;
                }
                else
                {
                    return point + " :" + renderE.ToString() + " " + renderE.RectBounds.ToString() + " " + renderE.GetType().Name;
                }

            }
            else
            {
                return point + " :" + _hitObject.ToString();
            }
        }
#endif
    }

#if DEBUG
    public enum dbugHitChainPhase
    {
        Unknown,
        MouseDown,
        MouseMove,
        MouseUp,
        MouseWheel,
    }
#endif

    public class HitChain
    {
        List<HitInfo> _hitList = new List<HitInfo>();
        int _startTestX;
        int _startTestY;
        int _testPointX;
        int _testPointY;
        public HitChain()
        {

        }

        public Point TestPoint => new Point(_testPointX, _testPointY);
        //
        public int TestPointX => _testPointX;
        public int TestPointY => _testPointY;
        //
        public void GetTestPoint(out int x, out int y)
        {
            x = _testPointX;
            y = _testPointY;
        }
        public void SetStartTestPoint(int x, int y)
        {
            _testPointX = x;
            _testPointY = y;
            _startTestX = x;
            _startTestY = y;
        }

        public void OffsetTestPoint(int dx, int dy)
        {
            _testPointX += dx;
            _testPointY += dy;
        }
        public void Reset()
        {
#if DEBUG
            dbugHitPhase = dbugHitChainPhase.Unknown;
#endif
            _testPointX = _testPointY = 0;
            //skip reset  _startTestX and _startTestY
            _hitList.Clear();
            Exclude_TransparentMouse_Element = true;
        }

#if DEBUG
        dbugHitChainPhase _dbugHitChainPhase;
        public dbugHitChainPhase dbugHitPhase
        {
            get { return _dbugHitChainPhase; }
            set
            {

                _dbugHitChainPhase = value;

            }
        }
        public dbugHitTestTracker dbugHitTracker;
#endif
        //
        public int Count => _hitList.Count;
        //
        public HitInfo GetHitInfo(int index) => _hitList[index];
        //
        public RenderElement TopMostElement
        {
            get
            {
                if (_hitList.Count > 0)
                {
                    return _hitList[_hitList.Count - 1].HitElemAsRenderElement;
                }
                else
                {
                    return null;
                }
            }
        }
        public void AddHitObject(RenderElement hitObject)
        {
            _hitList.Add(new HitInfo(hitObject, new Point(_testPointX, _testPointY)));
#if DEBUG
            //if (hitObject.dbug_ObjectNote == "AAA")
            //{

            //}
            if (this.dbugHitPhase == dbugHitChainPhase.MouseDown)
            {

            }

            if (dbugHitTracker != null)
            {
                dbugHitTracker.WriteTrackNode(_hitList.Count,
                    new Point(_testPointX, _testPointY).ToString() + " on "
                    + hitObject.ToString());
            }
#endif
        }
        public void RemoveCurrentHit()
        {
            if (_hitList.Count > 0)
            {
                _hitList.RemoveAt(_hitList.Count - 1);
            }
        }

        public bool Exclude_TransparentMouse_Element { get; set; } = true;

#if DEBUG
        public bool dbugBreak;
#endif

    }
}
