

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.DrawingGL;
using DrawingBridge;

namespace LayoutFarm.Drawing.DrawingGL
{
    //platform specific
    class GdiTextBoard
    {
        System.Drawing.Bitmap textBoardBmp;
        System.Drawing.Graphics gx;
        int textBoardW = 800;
        int textBoardH = 100;
        TextureAtlas textureAtlas;
        int width;
        int height;
        Dictionary<char, LayoutFarm.Drawing.RectangleF> charMap = new Dictionary<char, RectangleF>();
        LayoutFarm.Drawing.Bitmap myTextBoardBmp;
        IntPtr hFont;
        public GdiTextBoard(int width, int height, IntPtr hFont)
        {
            this.width = width;
            this.height = height;
            this.textureAtlas = new TextureAtlas(width, height);
            this.hFont = hFont;
            //32 bits bitmap
            textBoardBmp = new System.Drawing.Bitmap(textBoardW, textBoardH, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gx = System.Drawing.Graphics.FromImage(textBoardBmp);
            //draw character map
            PrepareCharacterMap("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray());
        }

        void PrepareCharacterMap(char[] buffer)
        {
            //-----------------------------------------------------------------------
            IntPtr gxdc = gx.GetHdc();
            int len = buffer.Length;
            //draw each character
            int curX = 0;
            int curY = 0;

            //transparent background
            MyWin32.SetBkMode(gxdc, MyWin32._SetBkMode_TRANSPARENT);
            
            //set user font to dc
            //MyWin32.SelectObject(gxdc, this.hFont);

            for (int i = 0; i < len; ++i)
            {

                //-----------------------------------------------------------------------
                //measure string 
                //and make simple character map
                //----------------------------------------------------------------------- 
                //measure each character ***
                //not adjust kerning*** 
                char c = buffer[i];
                char[] buff = new char[] { c };
                NativeTextWin32.WIN32SIZE size;
                NativeTextWin32.GetTextExtentPoint32(gxdc, buff, 1, out size);
                NativeTextWin32.TextOut(gxdc, curX, curY, buff, 1);

                charMap.Add(c, new RectangleF(curX, curY, size.Width, size.Height));

                curX += size.Width; //move next 
            }

            gx.ReleaseHdc(gxdc);

            myTextBoardBmp = new Bitmap(width, height, new LazyGdiBitmapBufferProvider(this.textBoardBmp));
            myTextBoardBmp.InnerImage = GLBitmapTextureHelper.CreateBitmapTexture(this.textBoardBmp);

        }
        public void DrawText(MyCanvasGL canvasGL2d, char[] buffer, int x, int y)
        {
            //same font
            //load bitmap texture
            //find texture map position
            var glbmp = GetBitmap();
            //create reference bmp
            int len = buffer.Length;
            float curX = x;
            float curY = y;

            //create destAndSrcArray
            LayoutFarm.Drawing.RectangleF[] destAndSrc = new RectangleF[len * 2];

            int pp = 0;
            for (int i = 0; i < len; ++i)
            {
                //find map glyph
                RectangleF found;
                if (charMap.TryGetValue(buffer[i], out found))
                {
                    //found
                    //canvasGL2d.DrawImage(myTextBoardBmp,
                    //    new RectangleF(curX, curY,
                    //        found.Width, found.Height),
                    //    found);
                    //dest
                    destAndSrc[pp] = new RectangleF(curX, curY, found.Width, found.Height);
                    //src
                    destAndSrc[pp + 1] = found;
                    curX += found.Width;

                }
                else
                {
                    //draw missing glyph

                }
                pp += 2;
            }

            canvasGL2d.DrawImages(myTextBoardBmp, destAndSrc);
            glbmp.Dispose();

        }
        GLBitmap GetBitmap()
        {
            return GLBitmapTextureHelper.CreateBitmapTexture(this.textBoardBmp);
        }

    }

}