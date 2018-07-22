//----------------------------------------------------------------------------
//MIT, 2014-present, WinterDev



namespace PixelFarm.CpuBlit
{

    public static class SvgParserExt
    {
        public static SvgRenderVx GetResultAsRenderVx(this PaintLab.Svg.SvgParser parser)
        {

            ////resolve all elements...
            //int j = _renderVxList.Count;

            ////TODO: review here
            ////temp fix 
            //for (int i = 0; i < j; ++i)
            //{
            //    SvgPart svgpart = _renderVxList[i];
            //    if (svgpart.SvgClipPath != null)
            //    {
            //        //var clipPathRef = svgpart.SvgClipPath as SvgClipPathReference;
            //        //if (clipPathRef.ResolvedClip == null)
            //        //{
            //        //    //resolve this clip
            //        //    SvgClipPath clipPath = FindSvgClipPathById(clipPathRef.RefName);
            //        //    if (clipPath != null)
            //        //    {
            //        //        SvgClipPath foundClip = FindSvgClipPathById(clipPathRef.RefName);
            //        //    }
            //        //    else
            //        //    {

            //        //    }
            //        //}
            //    }
            //}

            //var result = new SvgRenderVx(_renderVxList.ToArray());
            ////result.SetSvgDefs(_defsList.ToArray());
            //return result;
            return null;
        }


        ////   ------------------------------------

        //public static VertexStore ParseSvgPathDefinitionToVxs(char[] buffer)
        //{

        //    using (VxsContext.Temp(out var flattenVxs))
        //    {
        //        VectorToolBox.GetFreePathWriter(out PathWriter pathWriter);
        //        _svgPathDataParser.SetPathWriter(pathWriter);
        //        _svgPathDataParser.Parse(buffer);

        //        _curveFlattener.MakeVxs(pathWriter.Vxs, flattenVxs);

        //        //create a small copy of the vxs 
        //        VectorToolBox.ReleasePathWriter(ref pathWriter);

        //        return flattenVxs.CreateTrim();
        //    }
        //}

    }

}