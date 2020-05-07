//Apache2, 2014-present, WinterDev

using LayoutFarm.RenderBoxes;
using System;
using System.Collections.Generic;
using System.IO;
namespace LayoutFarm
{
#if DEBUG
    public class dbugVisualLayoutTracer
    {
        StreamWriter _strmWriter;
        RootGraphic _visualroot;
        string _outputFileName = null;
        int _msgLineNum = 0;
        Stack<object> _elementStack = new Stack<object>();
        int _indentCount = 0;
        int _myTraceCount = 0;
        static int s_tracerCount = 0;
        public dbugVisualLayoutTracer(RootGraphic visualroot)
        {
            this._visualroot = visualroot;
            _myTraceCount = s_tracerCount;
            ++s_tracerCount;
            _outputFileName = dbugCoreConst.dbugRootFolder + "\\layout_trace\\" + _myTraceCount + "_" + Guid.NewGuid().ToString() + ".txt";
        }
        public override string ToString()
        {
            return _msgLineNum.ToString();
        }
        public void BeginNewContext()
        {
            ++_indentCount;
        }
        public void EndCurrentContext()
        {
            --_indentCount;
        }
        public void PushVisualElement(RenderElement v)
        {
            _elementStack.Push(v);
            BeginNewContext();
        }
        public void PopVisualElement()
        {
            _elementStack.Pop();
            EndCurrentContext();
        }
        public void PushLayerElement(dbugLayoutInfo layer)
        {
            _elementStack.Push(layer);
            BeginNewContext();
        }
        public void PopLayerElement()
        {
            _elementStack.Pop();
            EndCurrentContext();
        }

        public object PeekElement()
        {
            return _elementStack.Peek();
        }

        public void Start(StreamWriter strmWriter)
        {
            this._strmWriter = strmWriter;
            strmWriter.AutoFlush = true;
        }
        public void Stop()
        {
            _strmWriter.Flush();
        }
        public void WriteInfo(RenderElement v, string info, string indentPrefix, string indentPostfix)
        {
            ++_msgLineNum;
            ShouldBreak();
            _strmWriter.Write(new string('\t', _indentCount));
            _strmWriter.Write(indentPrefix + _indentCount + indentPostfix + info + " ");
            _strmWriter.Write(v.dbug_FullElementDescription());
            _strmWriter.Write("\r\n"); _strmWriter.Flush();
        }
        public void WriteInfo(string info)
        {
            ++_msgLineNum;
            ShouldBreak();
            _strmWriter.Write(new string('\t', _indentCount));
            _strmWriter.Write(info);
            _strmWriter.Write("\r\n"); _strmWriter.Flush();
        }
        public void WriteInfo(string info, RenderElement v)
        {
            ++_msgLineNum;
            ShouldBreak();
            _strmWriter.Write(new string('\t', _indentCount));
            _strmWriter.Write(info);
            _strmWriter.Write(v.dbug_FullElementDescription());
            _strmWriter.Write("\r\n"); _strmWriter.Flush();
        }
        public void WriteInfo(string info, dbugLayoutInfo layer)
        {
            ++_msgLineNum;
            ShouldBreak();
            _strmWriter.Write(new string('\t', _indentCount));
            _strmWriter.Write(info);
            _strmWriter.Write(layer.ToString());
            _strmWriter.Write("\r\n"); _strmWriter.Flush();
        }
        public void Flush()
        {
            _strmWriter.Flush();
        }
        void ShouldBreak()
        {
        }
    }
#endif
}
