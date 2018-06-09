//BSD, 2014-2018, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using PixelFarm.Drawing;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
using PixelFarm.Agg.Transform;

using PixelFarm;
using PixelFarm.Drawing.Fonts;
using Typography.TextServices;
using Typography.OpenFont;

using Mini;


using Typography.Rendering;
using Typography.OpenFont.Extensions;
using PixelFarm.Agg;
namespace PixelFarm.Agg.Sample_Draw
{

    [Info(OrderCode = "01")]
    public class DrawSample04 : DemoBase
    {
        PixelFarm.Agg.ActualBitmap totalImg;
        SimpleFontAtlas fontAtlas;
        Stroke stroke = new Stroke(1);

        public override void Init()
        {



            //create font atlas
            RequestFont reqFont = new RequestFont("tahoma", 14);
            var textServices = new LayoutFarm.OpenFontTextService();
            GlyphImage totalGlyphsImg = null;
            SimpleFontAtlasBuilder atlasBuilder = null;
            var textureGen = new GlyphTextureBitmapGenerator();

            float fontSize = 14;
            TextureKind textureKind = TextureKind.StencilLcdEffect;
            SetCurrentScriptLangs(new ScriptLang[]
                {
                    ScriptLangs.Latin
                });

            Typeface resolvedTypeface = textServices.ResolveTypeface(reqFont);
            textureGen.CreateTextureFontFromScriptLangs(
                resolvedTypeface,
                fontSize,
                textureKind,
               _textureBuildDetails,
                (glyphIndex, glyphImage, outputAtlasBuilder) =>
                {
                    if (outputAtlasBuilder != null)
                    {
                        //finish
                        atlasBuilder = outputAtlasBuilder;
                    }
                }
            );

            //
            totalGlyphsImg = atlasBuilder.BuildSingleImage(); 
            //create atlas
            fontAtlas = atlasBuilder.CreateSimpleFontAtlas();
            fontAtlas.TotalGlyph = totalGlyphsImg;

#if DEBUG
            string fontTexturePrefixFileName = "d:\\WImageTest\\total_" + reqFont.Name + "_" + reqFont.SizeInPoints;

            //save glyph image for debug 
            PixelFarm.Agg.ActualBitmap.SaveImgBufferToPngFile(
                totalGlyphsImg.GetImageBuffer(),
                totalGlyphsImg.Width * 4,
                totalGlyphsImg.Width, totalGlyphsImg.Height,
                fontTexturePrefixFileName + ".png");
            ////save image to cache

#endif

            //cache the atlas
            //_createdAtlases.Add(fontKey, fontAtlas);
            //
            //calculate some commonly used values
            fontAtlas.SetTextureScaleInfo(
                resolvedTypeface.CalculateScaleToPixelFromPointSize(fontAtlas.OriginalFontSizePts),
                resolvedTypeface.CalculateScaleToPixelFromPointSize(reqFont.SizeInPoints));
            //TODO: review here, use scaled or unscaled values
            fontAtlas.SetCommonFontMetricValues(
                resolvedTypeface.Ascender,
                resolvedTypeface.Descender,
                resolvedTypeface.LineGap,
                resolvedTypeface.CalculateRecommendLineSpacing());

            ///
#if DEBUG

            //_dbugStopWatch.Stop();
            //System.Diagnostics.Debug.WriteLine("build font atlas: " + _dbugStopWatch.ElapsedMilliseconds + " ms");
#endif

            //save font info to cache
            string textureFileName = fontTexturePrefixFileName + ".info";
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                atlasBuilder.SaveAtlasInfo(ms);
                //save data to a file
                //StorageService.Provider.SaveData(fontTextureInfoFile, ms.ToArray());
                System.IO.File.WriteAllBytes(textureFileName, ms.ToArray());
            }
        }

        //-------------------------------------------------------
        GlyphTextureBuildDetail[] _textureBuildDetails;
        ScriptLang[] _currentScriptLangs;
        void SetCurrentScriptLangs(ScriptLang[] currentScriptLangs)
        {
            this._currentScriptLangs = currentScriptLangs;

            //TODO: review here again,
            //this is a fixed version for tahoma font 
            //temp fix here 
            _textureBuildDetails = new GlyphTextureBuildDetail[]
            {
                new GlyphTextureBuildDetail{ ScriptLang= ScriptLangs.Latin,
                    DoFilter = false, HintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction_VerticalOnly },
                new GlyphTextureBuildDetail{ OnlySelectedGlyphIndices=new char[]{ 'x', 'X', '7','k','K','Z','z','R','Y','%' },
                    DoFilter = false ,  HintTechnique = Typography.Contours.HintTechnique.None},
                new GlyphTextureBuildDetail{ ScriptLang= ScriptLangs.Thai, DoFilter= false, HintTechnique = Typography.Contours.HintTechnique.None},
            };
        }
        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }
        public override void Draw(Painter p)
        {

            if (UseBitmapExt)
            {
                p.RenderQuality = RenderQualtity.Fast;
            }
            else
            {
                p.RenderQuality = RenderQualtity.HighQuality;
            }

            p.Orientation = DrawBoardOrientation.LeftBottom;

            int width = 800;
            int height = 600;
            //clear the image to white         
            // draw a circle
            p.Clear(Drawing.Color.White);
            p.FillColor = Color.Black;

            //----
            //test draw img 
            //g.Render(littlePoly.MakeVertexSnap(), ColorRGBA.Cyan);
            // draw some text
            // draw some text  

            //var textPrinter = new TextPrinter();
            //textPrinter.CurrentActualFont = svgFontStore.LoadFont(SvgFontStore.DEFAULT_SVG_FONTNAME, 30);
            //new TypeFacePrinter("Printing from a printer", 30, justification: Justification.Center);

            //VertexStore vxs = textPrinter.CreateVxs("Printing from a printer".ToCharArray());
            //var affTx = Affine.NewTranslation(width / 2, height / 4 * 3);
            //VertexStore s1 = affTx.TransformToVxs(vxs);
            //p.FillColor = Drawing.Color.Black;
            //p.Fill(s1);
            ////g.Render(s1, ColorRGBA.Black);
            //p.FillColor = Drawing.Color.Red;
            //p.Fill(StrokeHelp.MakeVxs(s1, 1));
            ////g.Render(StrokeHelp.MakeVxs(s1, 1), ColorRGBA.Red);
            //var aff2 = Affine.NewMatix(
            //    AffinePlan.Rotate(MathHelper.DegreesToRadians(90)),
            //    AffinePlan.Translate(40, height / 2));
            //p.FillColor = Drawing.Color.Black;
            //p.Fill(aff2.TransformToVertexSnap(vxs));
            ////g.Render(aff2.TransformToVertexSnap(vxs), ColorRGBA.Black);
        }
    }
}