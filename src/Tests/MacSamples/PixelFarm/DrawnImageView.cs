using System;
 
using CoreGraphics;

using System.IO;

using PixelFarm;
using PixelFarm.OpenType;
using PixelFarm.Drawing;
using PixelFarm.Agg;
using AppKit;
using NOpenType;

namespace PixelFarmSkia
{
	using CoreText;
	using Foundation;

	public class DrawnImageView : NSView
	{
		ActualImage destImg;
		ImageGraphics2D imgGfx2d;
		AggCanvasPainter p;
		VertexStore vxs;
		bool drawInit;
		int destImgW = 100;
		int destImgH = 100;
		int stride = 0;
		public DrawnImageView()
		{
			this.SetFrameSize(new CGSize(800, 600));


		}
		public override bool IsFlipped
		{
			get
			{
				return true;
			}
		}
		void LoadGlyphs()
		{
			using (FileStream fs = new FileStream("tahoma.ttf", FileMode.Open))
			{

				OpenTypeReader reader = new OpenTypeReader();
				Typeface typeface = reader.Read(fs);

				var testChar = 'a';
				var builder = new GlyphPathBuilderVxs(typeface);
				float sizeInPoint = 48;

				builder.Build(testChar, sizeInPoint);
				vxs = builder.GetVxs();
			}
		}
		public override void DrawRect(CGRect dirtyRect)
		{
			
			if (!drawInit)
			{
				this.Window.Title = "PixelFarm";
				drawInit = true;
				destImg = new ActualImage(destImgW, destImgH, PixelFarm.Agg.PixelFormat.ARGB32);
				imgGfx2d = new ImageGraphics2D(destImg); //no platform
				p = new AggCanvasPainter(imgGfx2d);
				stride = destImg.Stride;
				LoadGlyphs();

			}
			//base.Draw(rect);
			base.DrawRect(dirtyRect);


			p.Clear(Color.Yellow);
			p.FillColor = Color.Black;
			p.Fill(vxs);

			var data = Foundation.NSData.FromArray(ActualImage.GetBuffer(destImg));
			CGDataProvider provider = new CGDataProvider(data);
			using (var myImg2 = new CGImage(
				destImgW, destImgH,
				8, 32,
				stride, CGColorSpace.CreateGenericRgb(),
				CGBitmapFlags.PremultipliedLast,
				provider,
				null, true,
				CGColorRenderingIntent.AbsoluteColorimetric))

			using (var nsGraphics = AppKit.NSGraphicsContext.CurrentContext)
			{

				CGContext g = nsGraphics.CGContext;
				CGColor color0 = new CGColor(1, 1, 1, 1);
				g.SetFillColor(color0);
				//g.ClearRect(new CGRect(0, 0, 800, 600));
				//----------

				CGColor color1 = new CGColor(1, 0, 0, 1);
				g.SetFillColor(color1);

				CGRect s1 = CGRect.FromLTRB(0, 0, 50, 50);
				CGPath gpath = new CGPath();
				gpath.AddRect(CGAffineTransform.MakeTranslation(20, 20), s1);
				g.AddPath(gpath);
				g.FillPath();


				CGRect s2 = new CGRect(50, 50, destImgW, destImgH);
				g.DrawImage(s2, myImg2);

				//

				//g.FillRect(s1);

				CGColor color2 = new CGColor(0, 0, 1, 1);
				g.SetFillColor(color2);
				g.TranslateCTM(30, 30);


				var strAttr = new CTStringAttributes
				{
					ForegroundColorFromContext = true,
					Font = new CTFont("Arial", 24)
				};


				g.ScaleCTM(1, -1);//flip
				NSAttributedString a_str = new NSAttributedString("abcd", strAttr);
				using (CTLine line = new CTLine(a_str))
				{
					line.Draw(g);
				}


				////if (chkBorder.Checked)
				////{
				////	//5.4 
				////	p.StrokeColor = PixelFarm.Drawing.Color.Green;
				////	//user can specific border width here...
				////	//p.StrokeWidth = 2;
				////	//5.5 
				////	p.Draw(vxs);
				////}
				////6. use this util to copy image from Agg actual image to System.Drawing.Bitmap
				//BitmapHelper.CopyToWindowsBitmap(destImg, winBmp, new RectInt(0, 0, 300, 300));
				////--------------- 
				////7. just render our bitmap
				//g.ClearRect(rect);

				//g.DrawImage(winBmp, new Point(10, 0)); 
			}



			//// scale and translate the CTM so the image appears upright
			//g.ScaleCTM (1, -1);
			//g.TranslateCTM (0, -Bounds.Height);
			//g.DrawImage (rect, UIImage.FromFile ("MyImage.png").CGImage);  
			//// translate the CTM by the font size so it displays on screen
			//float fontSize = 35f;
			//g.TranslateCTM (0, fontSize);

			//// set general-purpose graphics state
			//g.SetLineWidth (1.0f);
			//g.SetStrokeColor (UIColor.Yellow.CGColor);
			//g.SetFillColor (UIColor.Red.CGColor);
			//g.SetShadow (new CGSize (5, 5), 0, UIColor.Blue.CGColor);

			//// set text specific graphics state
			//g.SetTextDrawingMode (CGTextDrawingMode.FillStroke);
			//g.SelectFont ("Helvetica", fontSize, CGTextEncoding.MacRoman);

			//// show the text
			//g.ShowText ("Hello Core Graphics");
		}
		 
	}

}

