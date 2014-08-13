using System;
using System.IO;

using MatterHackers.Agg.Image;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.RasterizerScanline;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

using Mini;
namespace MatterHackers.Agg
{

    [Info(OrderCode = "06")]
    [Info("AGG has a gray-scale renderer that can use any 8-bit color channel of an RGB or RGBA frame buffer. Most likely it will be used to draw gray-scale images directly in the alpha-channel.")]
    public class ComponentRendering : DemoBase
    {

        public ComponentRendering()
        {
            this.AlphaValue = 255;
        }

        [DemoConfig(MaxValue = 255)]
        public int AlphaValue
        {
            get;
            set;
        }
        [DemoConfig]
        public bool UseBlackBlackground
        {
            get;
            set;

        }


        public override void Draw(Graphics2D graphics2D)
        {
            if (graphics2D.DestImage != null)
            {
                ImageBuffer widgetsSubImage = ImageBuffer.NewSubImageReference(graphics2D.DestImage, graphics2D.GetClippingRect());

                IImageByte backBuffer = widgetsSubImage;

                int distBetween = backBuffer.GetBytesBetweenPixelsInclusive();
                ImageBuffer redImageBuffer = new ImageBuffer();
                redImageBuffer.Attach(backBuffer, new blender_gray(distBetween), distBetween, 2, 8);
                ImageBuffer greenImageBuffer = new ImageBuffer();
                greenImageBuffer.Attach(backBuffer, new blender_gray(distBetween), distBetween, 1, 8);
                ImageBuffer blueImageBuffer = new ImageBuffer();
                blueImageBuffer.Attach(backBuffer, new blender_gray(distBetween), distBetween, 0, 8);

                ImageClippingProxy clippingProxy = new ImageClippingProxy(backBuffer);
                ImageClippingProxy clippingProxyRed = new ImageClippingProxy(redImageBuffer);
                ImageClippingProxy clippingProxyGreen = new ImageClippingProxy(greenImageBuffer);
                ImageClippingProxy clippingProxyBlue = new ImageClippingProxy(blueImageBuffer);

                ScanlineRasterizer ras = new ScanlineRasterizer();
                ScanlineCachePacked8 sl = new ScanlineCachePacked8();

                RGBA_Bytes clearColor = this.UseBlackBlackground ? new RGBA_Bytes(0, 0, 0) : new RGBA_Bytes(255, 255, 255);
                clippingProxy.clear(clearColor);
                //alphaSlider.View.BackgroundColor = clearColor;

                RGBA_Bytes FillColor = this.UseBlackBlackground ?
                    new RGBA_Bytes(255, 255, 255, (byte)(this.AlphaValue)) :
                    new RGBA_Bytes(0, 0, 0, (byte)(this.AlphaValue));

                VertexSource.Ellipse er = new MatterHackers.Agg.VertexSource.Ellipse(Width / 2 - 0.87 * 50, Height / 2 - 0.5 * 50, 100, 100, 100);
                ras.add_path(er);
                ScanlineRenderer scanlineRenderer = new ScanlineRenderer();
                scanlineRenderer.render_scanlines_aa_solid(clippingProxyRed, ras, sl, FillColor);

                VertexSource.Ellipse eg = new MatterHackers.Agg.VertexSource.Ellipse(Width / 2 + 0.87 * 50, Height / 2 - 0.5 * 50, 100, 100, 100);
                ras.add_path(eg);
                scanlineRenderer.render_scanlines_aa_solid(clippingProxyGreen, ras, sl, FillColor);

                VertexSource.Ellipse eb = new MatterHackers.Agg.VertexSource.Ellipse(Width / 2, Height / 2 + 50, 100, 100, 100);
                ras.add_path(eb);
                scanlineRenderer.render_scanlines_aa_solid(clippingProxyBlue, ras, sl, FillColor);
            }
            else if (graphics2D.DestImageFloat != null)
            {
#if false
                IImageFloat backBuffer = graphics2D.DestImageFloat;

                int distBetween = backBuffer.GetFloatsBetweenPixelsInclusive();
                ImageBufferFloat redImageBuffer = new ImageBufferFloat();
                redImageBuffer.Attach(backBuffer, new blender_gray(distBetween), distBetween, 2, 8);
                ImageBufferFloat greenImageBuffer = new ImageBufferFloat();
                greenImageBuffer.Attach(backBuffer, new blender_gray(distBetween), distBetween, 1, 8);
                ImageBufferFloat blueImageBuffer = new ImageBufferFloat();
                blueImageBuffer.Attach(backBuffer, new blender_gray(distBetween), distBetween, 0, 8);

                ImageClippingProxy clippingProxy = new ImageClippingProxy(backBuffer);
                ImageClippingProxy clippingProxyRed = new ImageClippingProxy(redImageBuffer);
                ImageClippingProxy clippingProxyGreen = new ImageClippingProxy(greenImageBuffer);
                ImageClippingProxy clippingProxyBlue = new ImageClippingProxy(blueImageBuffer);

                ScanlineRasterizer ras = new ScanlineRasterizer();
                ScanlineCachePacked8 sl = new ScanlineCachePacked8();

                RGBA_Bytes clearColor = useBlackBackgroundCheckbox.Checked ? new RGBA_Bytes(0, 0, 0) : new RGBA_Bytes(255, 255, 255);
                clippingProxy.clear(clearColor);
                alphaSlider.View.BackGroundColor = clearColor;

                RGBA_Bytes FillColor = useBlackBackgroundCheckbox.Checked ? new RGBA_Bytes(255, 255, 255, (int)(alphaSlider.Value)) : new RGBA_Bytes(0, 0, 0, (int)(alphaSlider.Value));

                VertexSource.Ellipse er = new AGG.VertexSource.Ellipse(Width / 2 - 0.87 * 50, Height / 2 - 0.5 * 50, 100, 100, 100);
                ras.add_path(er);
                agg_renderer_scanline.Default.render_scanlines_aa_solid(clippingProxyRed, ras, sl, FillColor);

                VertexSource.Ellipse eg = new AGG.VertexSource.Ellipse(Width / 2 + 0.87 * 50, Height / 2 - 0.5 * 50, 100, 100, 100);
                ras.add_path(eg);
                agg_renderer_scanline.Default.render_scanlines_aa_solid(clippingProxyGreen, ras, sl, FillColor);

                VertexSource.Ellipse eb = new AGG.VertexSource.Ellipse(Width / 2, Height / 2 + 50, 100, 100, 100);
                ras.add_path(eb);
                agg_renderer_scanline.Default.render_scanlines_aa_solid(clippingProxyBlue, ras, sl, FillColor);
#endif
            }


        }
    }

}
