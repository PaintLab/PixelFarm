//MIT, 2017-present, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using System.Text;
using System.Windows.Forms;

using PixelFarm.Drawing.Fonts;
using Typography.Rendering;
using Typography.OpenFont;


namespace TypographyTest.WinForms
{
    public partial class GlyphTextureBitmapGenUserControl : UserControl
    {
        GlyphTextureBitmapGenerator _glyphTextureBitmapController;
        Typography.OpenFont.Typeface _typeface;

        public GlyphTextureBitmapGenUserControl()
        {
            InitializeComponent();
            _glyphTextureBitmapController = new GlyphTextureBitmapGenerator();
            SelectedScriptLangs = new List<Typography.OpenFont.ScriptLang>();
            FontSizeInPoints = 18;//default

            //-
            this.cmbSpaceCompactOption.Items.AddRange(
                new object[] {
                    SimpleFontAtlasBuilder.CompactOption.None,
                    SimpleFontAtlasBuilder.CompactOption.ArrangeByHeight,
                    SimpleFontAtlasBuilder.CompactOption.BinPack,
                });
            this.cmbSpaceCompactOption.SelectedIndex = 0;//default
        }
        public float FontSizeInPoints { get; set; }
        public Typography.OpenFont.Typeface SelectedTypeface
        {
            get { return _typeface; }
            set
            {
                _typeface = value;
            }
        }
        public List<Typography.OpenFont.ScriptLang> SelectedScriptLangs { get; private set; }

        private void GenGlyphBitmapTextureUserControl_Load(object sender, EventArgs e)
        {
            lstTextureType.Items.Add(TextureKind.StencilGreyScale);
            lstTextureType.Items.Add(TextureKind.StencilLcdEffect);
            lstTextureType.Items.Add(TextureKind.Msdf);
            lstTextureType.SelectedIndex = 0;
            this.textBox1.Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-*/?=(){}[]%@#^$&|.";

        }
        static void CopyToGdiPlusBitmapSameSize(
          IntPtr srcBuffer,
          Bitmap bitmap)
        {
            //agg store image buffer head-down
            //when copy to window bmp we here to flip 
            //style1: copy row by row *** (fastest)***
            {
                //System.GC.Collect();
                //System.Diagnostics.Stopwatch sss = new System.Diagnostics.Stopwatch();
                //sss.Start();
                //for (int i = 0; i < 1000; ++i)
                //{
                int h = bitmap.Height;
                int w = bitmap.Width;
                BitmapData bitmapData1 = bitmap.LockBits(
                          new Rectangle(0, 0,
                              w,
                              h),
                              System.Drawing.Imaging.ImageLockMode.ReadWrite,
                              bitmap.PixelFormat);
                IntPtr scan0 = bitmapData1.Scan0;
                int stride = bitmapData1.Stride;

                unsafe
                {
                    byte* bufferH = (byte*)srcBuffer;
                    byte* target = (byte*)scan0;
                    int startRowAt = ((h - 1) * stride);

                    for (int y = h; y > 0; --y)
                    {
                        byte* src = bufferH + ((y - 1) * stride);
                        //AggMemMx.memcpy()
                        //System.Runtime.InteropServices.Marshal.Copy(
                        //   srcBuffer,//src
                        //   startRowAt,
                        //   (IntPtr)target,
                        //   stride);
                        //startRowAt -= stride;
                        //target += stride;  
                        PixelFarm.Agg.AggMemMx.memcpy(target, src, stride);
                        startRowAt -= stride;
                        target += stride;
                    }

                }
                bitmap.UnlockBits(bitmapData1);
                //}
                //sss.Stop();
                //long ms = sss.ElapsedMilliseconds;
            }
            //-----------------------------------
            //style2: copy all, then flip again
            //{
            //    System.GC.Collect();
            //    System.Diagnostics.Stopwatch sss = new System.Diagnostics.Stopwatch();
            //    sss.Start();
            //    for (int i = 0; i < 1000; ++i)
            //    {
            //        byte[] rawBuffer = ActualImage.GetBuffer(actualImage);
            //        var bmpdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            //          System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //         bitmap.PixelFormat);


            //        System.Runtime.InteropServices.Marshal.Copy(rawBuffer, 0,
            //            bmpdata.Scan0, rawBuffer.Length);

            //        bitmap.UnlockBits(bmpdata);
            //        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //    }

            //    sss.Stop();
            //    long ms = sss.ElapsedMilliseconds; 
            //}
            //-----------------------------------

            //-----------------------------------
            //style3: copy row by row + 
            //{
            //    System.GC.Collect();
            //    System.Diagnostics.Stopwatch sss = new System.Diagnostics.Stopwatch();
            //    sss.Start();
            //    for (int i = 0; i < 1000; ++i)
            //    {
            //        int h = bitmap.Height;
            //        int w = bitmap.Width;
            //        BitmapData bitmapData1 = bitmap.LockBits(
            //                  new Rectangle(0, 0,
            //                      w,
            //                      h),
            //                      System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //                      bitmap.PixelFormat);
            //        IntPtr scan0 = bitmapData1.Scan0;
            //        int stride = bitmapData1.Stride;
            //        byte[] buffer = ActualImage.GetBuffer(actualImage);
            //        unsafe
            //        {
            //            fixed (byte* bufferH = &buffer[0])
            //            {
            //                byte* target = (byte*)scan0;
            //                for (int y = h; y > 0; --y)
            //                {
            //                    byte* src = bufferH + ((y - 1) * stride);
            //                    for (int n = stride - 1; n >= 0; --n)
            //                    {
            //                        *target = *src;
            //                        target++;
            //                        src++;
            //                    }
            //                }
            //            }
            //        }
            //        bitmap.UnlockBits(bitmapData1);
            //    }
            //    sss.Stop();
            //    long ms = sss.ElapsedMilliseconds;
            //} 
        }



        private void cmdMakeFromScriptLangs_Click(object sender, EventArgs e)
        {
            //create a simple stencil texture font

            //string sampleFontFile = "../../../TestFonts/tahoma.ttf";
            if (this.textBox1.Text == null || _typeface == null)
            {
                return;
            }
            if (SelectedScriptLangs.Count == 0)
            {
                return;
            }

            //
            string sampleFontFile = _typeface.Filename ?? "";

            TextureKind selectedTextureKind = (TextureKind)lstTextureType.SelectedItem;
            char[] chars = this.textBox1.Text.ToCharArray();

            string bitmapImgSaveFileName = "d:\\WImageTest\\sample_" + selectedTextureKind + "_" +
               System.IO.Path.GetFileNameWithoutExtension(sampleFontFile);

            //------------
            //TODO: review here again, this is not a good way, 
            //it is only temp fix for Tahoma.

            var _textureBuildDetails = new GlyphTextureBuildDetail[]
            {
                new GlyphTextureBuildDetail{ ScriptLang= ScriptLangs.Latin, DoFilter= false, HintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction_VerticalOnly },
                new GlyphTextureBuildDetail{ OnlySelectedGlyphIndices=new char[]{ 'x', 'X', '7' },
                    DoFilter = true,  HintTechnique = Typography.Contours.HintTechnique.TrueTypeInstruction_VerticalOnly},
                //
                new GlyphTextureBuildDetail{ ScriptLang= ScriptLangs.Thai, DoFilter= true, HintTechnique = Typography.Contours.HintTechnique.None},
            };
            //------------


            bool saveEachGlyphSeparatly = chkSaveEachGlyph.Checked;
            var textureGen = new GlyphTextureBitmapGenerator();
            textureGen.CreateTextureFontFromScriptLangs(
               _typeface,
               FontSizeInPoints,
               selectedTextureKind,
               _textureBuildDetails,
               (gindex, glyphImg, atlasBuilder) =>
               {
                   if (atlasBuilder != null)
                   {
                       atlasBuilder.SpaceCompactOption = (SimpleFontAtlasBuilder.CompactOption)cmbSpaceCompactOption.SelectedValue;
                       GlyphImage totalGlyphs = atlasBuilder.BuildSingleImage();
                       SaveImgBufferToFile(totalGlyphs, bitmapImgSaveFileName + ".png");

                       using (System.IO.FileStream fs = new System.IO.FileStream(bitmapImgSaveFileName + ".xml", System.IO.FileMode.Create))
                       {
                           atlasBuilder.SaveAtlasInfo(fs);
                       }

                       MessageBox.Show("glyph gen " + bitmapImgSaveFileName);
                   }
                   else
                   {

                       //save each glyph
                       if (saveEachGlyphSeparatly)
                       {
                           SaveImgBufferToFile(glyphImg, bitmapImgSaveFileName + "_" + gindex + ".png");
                       }

                   }
               });
        }

        private void cmdMakeFromSelectedString_Click(object sender, EventArgs e)
        {
            //create a simple stencil texture font

            //string sampleFontFile = "../../../TestFonts/tahoma.ttf";
            if (this.textBox1.Text == null || _typeface == null)
            {
                return;
            }

            string sampleFontFile = _typeface.Filename ?? "";

            TextureKind selectedTextureKind = (TextureKind)lstTextureType.SelectedItem;
            string bitmapImgSaveFileName = "d:\\WImageTest\\sample_" + selectedTextureKind + "_" +
              System.IO.Path.GetFileNameWithoutExtension(sampleFontFile);

            var textureGen = new GlyphTextureBitmapGenerator();
            bool saveEachGlyphSeparatly = chkSaveEachGlyph.Checked;
            char[] chars = this.textBox1.Text.ToCharArray();
            textureGen.CreateTextureFontFromInputChars(
               _typeface,
               FontSizeInPoints,
               selectedTextureKind,
               chars, //eg. ABCD
              (gindex, glyphImg, atlasBuilder) =>
              {
                  if (atlasBuilder != null)
                  {
                      atlasBuilder.SpaceCompactOption = (SimpleFontAtlasBuilder.CompactOption)cmbSpaceCompactOption.SelectedValue;
                      GlyphImage totalGlyphs = atlasBuilder.BuildSingleImage();
                      //
                      SaveImgBufferToFile(totalGlyphs, bitmapImgSaveFileName + ".png");
                      using (System.IO.FileStream fs = new System.IO.FileStream(bitmapImgSaveFileName + ".xml", System.IO.FileMode.Create))
                      {
                          atlasBuilder.SaveAtlasInfo(fs);
                      }


                      MessageBox.Show("glyph gen " + bitmapImgSaveFileName);
                  }
                  else
                  {
                      //save each glyph
                      if (saveEachGlyphSeparatly)
                      {
                          SaveImgBufferToFile(glyphImg, bitmapImgSaveFileName + "_" + gindex + ".png");
                      }
                  }
              });
        }

        static void SaveImgBufferToFile(GlyphImage glyphImg, string filename)
        {
            int[] intBuffer = glyphImg.GetImageBuffer();
            using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(glyphImg.Width, glyphImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                unsafe
                {
                    fixed (int* head = &intBuffer[0])
                    {
                        CopyToGdiPlusBitmapSameSize((IntPtr)head, newBmp);
                    }
                }
                //save
                newBmp.Save(filename);
            }

        }
    }
}
