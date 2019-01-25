﻿//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace PaintLab.Svg
{
    public class VgVisualDoc
    {
        //vector graphic document root


        VgVisualDocHost _vgVisualDocHost;
        internal List<SvgElement> _defsList = new List<SvgElement>();
        internal List<SvgElement> _styleList = new List<SvgElement>();
        internal Dictionary<string, VgVisualElement> _registeredElemsById = new Dictionary<string, VgVisualElement>();
        internal Dictionary<string, VgVisualElement> _clipPathDic = new Dictionary<string, VgVisualElement>();
        internal Dictionary<string, VgVisualElement> _markerDic = new Dictionary<string, VgVisualElement>();
        internal Dictionary<string, VgVisualElement> _filterDic = new Dictionary<string, VgVisualElement>();

        public VgVisualDoc(VgVisualDocHost vgVisualDocHost = null)
        {
            _vgVisualDocHost = vgVisualDocHost;
        }
        public VgVisualDocHost DocHost
        {
            get => _vgVisualDocHost;
            set => _vgVisualDocHost = value;
        }
        public bool TryGetVgVisualElementById(string id, out VgVisualElement found)
        {
            return _registeredElemsById.TryGetValue(id, out found);
        }
        internal void Invalidate(VgVisualElement e)
        {
            _vgVisualDocHost?.InvalidateGraphics(e);
        }

        internal void RequestImageAsync(LayoutFarm.ImageBinder binder, VgVisualElement imgRun, object requestFrom)
        {
            _vgVisualDocHost?.RequestImageAsync(binder, imgRun, requestFrom);
        }

        //
        public VgVisualElement VgRootElem { get; set; }


        public VgVisualElement CreateVgUseVisualElement(VgVisualElement refVgVisualElem)
        {

            //#if DEBUG 
            //#endif
            SvgUseSpec useSpec = new SvgUseSpec();
            VgUseVisualElement vgUseVisualElem = new VgUseVisualElement(useSpec, this);
            vgUseVisualElem.HRefSvgRenderElement = refVgVisualElem;
            return vgUseVisualElem;
        }
    }

}