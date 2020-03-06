//MIT, 2016-present, WinterDev
//MIT, 2016, Viktor Chlumsky, Multi-channel signed distance field generator, from https://github.com/Chlumsky/msdfgen
//-----------------------------------  
using System;
namespace ExtMsdfGen
{

    /// <summary>
    /// parameter for msdf generation
    /// </summary>
    public class MsdfGenParams
    {
        public float scaleX = 1;
        public float scaleY = 1;
        public float shapeScale = 1;
        public int minImgWidth = 5;
        public int minImgHeight = 5;

        public double angleThreshold = 3; //default
        public double pxRange = 4; //default
        public double edgeThreshold = 1.00000001;//default,(from original code)


        public MsdfGenParams()
        {

        }
        public void SetScale(float scaleX, float scaleY)
        {
            this.scaleX = scaleX;
            this.scaleY = scaleY;
        }


    }
    public static class MsdfGlyphGen
    {

        public static void PreviewSizeAndLocation(ExtMsdfGen.Shape shape, ExtMsdfGen.MsdfGenParams genParams,
            out int imgW, out int imgH,
            out Vector2 translate1)
        {
            double left = MAX;
            double bottom = MAX;
            double right = -MAX;
            double top = -MAX;

            shape.findBounds(ref left, ref bottom, ref right, ref top);
            int w = (int)Math.Ceiling((right - left));
            int h = (int)Math.Ceiling((top - bottom));

            if (w < genParams.minImgWidth)
            {
                w = genParams.minImgWidth;
            }
            if (h < genParams.minImgHeight)
            {
                h = genParams.minImgHeight;
            }

            //temp, for debug with glyph 'I', tahoma font
            //double edgeThreshold = 1.00000001;//default, if edgeThreshold < 0 then  set  edgeThreshold=1 
            //Msdfgen.Vector2 scale = new Msdfgen.Vector2(0.98714652956298199, 0.98714652956298199);
            //double pxRange = 4;
            //translate = new Msdfgen.Vector2(12.552083333333332, 4.0520833333333330);
            //double range = pxRange / Math.Min(scale.x, scale.y);


            int borderW = (int)((float)w / 5f) + 3;

            //org
            //var translate = new ExtMsdfgen.Vector2(left < 0 ? -left + borderW : borderW, bottom < 0 ? -bottom + borderW : borderW);
            //test
            var translate = new Vector2(-left + borderW, -bottom + borderW);

            w += borderW * 2; //borders,left- right
            h += borderW * 2; //borders, top- bottom

            imgW = w;
            imgH = h;
            translate1 = translate;
        }

        const double MAX = 1e240;

        public static SpriteTextureMapData<PixelFarm.CpuBlit.MemBitmap> CreateMsdfImage(ExtMsdfGen.Shape shape, MsdfGenParams genParams, EdgeBmpLut lutBuffer = null)
        {
            double left = MAX;
            double bottom = MAX;
            double right = -MAX;
            double top = -MAX;

            shape.findBounds(ref left, ref bottom, ref right, ref top);
            int w = (int)Math.Ceiling((right - left));
            int h = (int)Math.Ceiling((top - bottom));

            if (w < genParams.minImgWidth)
            {
                w = genParams.minImgWidth;
            }
            if (h < genParams.minImgHeight)
            {
                h = genParams.minImgHeight;
            }


            //temp, for debug with glyph 'I', tahoma font
            //double edgeThreshold = 1.00000001;//default, if edgeThreshold < 0 then  set  edgeThreshold=1 
            //Msdfgen.Vector2 scale = new Msdfgen.Vector2(0.98714652956298199, 0.98714652956298199);
            //double pxRange = 4;
            //translate = new Msdfgen.Vector2(12.552083333333332, 4.0520833333333330);
            //double range = pxRange / Math.Min(scale.x, scale.y);


            int borderW = (int)((float)w / 5f) + 3;

            //org
            //var translate = new ExtMsdfgen.Vector2(left < 0 ? -left + borderW : borderW, bottom < 0 ? -bottom + borderW : borderW);
            //test
            var translate = new Vector2(-left + borderW, -bottom + borderW);

            w += borderW * 2; //borders,left- right
            h += borderW * 2; //borders, top- bottom

            double edgeThreshold = genParams.edgeThreshold;
            if (edgeThreshold < 0)
            {
                edgeThreshold = 1.00000001; //use default if  edgeThreshold <0
            }

            var scale = new Vector2(genParams.scaleX, genParams.scaleY); //scale               
            double range = genParams.pxRange / Math.Min(scale.x, scale.y);
            //---------
            FloatRGBBmp frgbBmp = new FloatRGBBmp(w, h);
            EdgeColoring.edgeColoringSimple(shape, genParams.angleThreshold);

            if (lutBuffer != null)
            {
                MsdfGenerator.generateMSDF2(frgbBmp,
                  shape,
                  range,
                  scale,
                  translate,//translate to positive quadrant
                  edgeThreshold,
                  lutBuffer);
            }
            else
            {
                MsdfGenerator.generateMSDF(frgbBmp,
                  shape,
                  range,
                  scale,
                  translate,//translate to positive quadrant
                  edgeThreshold);
            }

            var spriteData = new SpriteTextureMapData<PixelFarm.CpuBlit.MemBitmap>(0, 0, w, h);
            spriteData.Source = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(w, h, MsdfGenerator.ConvertToIntBmp(frgbBmp));
            spriteData.TextureXOffset = (float)translate.x;
            spriteData.TextureYOffset = (float)translate.y;
            return spriteData;
        }
    }
}