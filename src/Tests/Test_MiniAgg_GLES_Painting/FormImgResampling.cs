//MIT, 2020-present,WinterDev

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;


namespace Mini
{
    public partial class FormImgResampling : Form
    {
        public FormImgResampling()
        {
            InitializeComponent();
        }

        private void FormImgResampling_Load(object sender, EventArgs e)
        {

        }
        PaintFx.Surface CreateSurfaceFromMemBitmap(MemBitmap memBmp)
        {
            var tmpBuffer = MemBitmap.GetBufferPtr(memBmp);
            PaintFx.MemHolder holder = new PaintFx.MemHolder(tmpBuffer.Ptr, tmpBuffer.LengthInBytes);
            PaintFx.Surface surface = new PaintFx.Surface(memBmp.Stride, memBmp.Width, memBmp.Height, holder);
            return surface;
        }
        static void SaveImage(MemBitmap bmp, string filename)
        {
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSize(bmp, newBmp);
            newBmp.Save(filename);
        }
        static MemBitmap LoadImage(string filename)
        {
            //read sample image
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename))
            {
                //read to image buffer 
                int bmpW = bmp.Width;
                int bmpH = bmp.Height;
                MemBitmap img = new MemBitmap(bmpW, bmpH);
                PixelFarm.CpuBlit.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(bmp, img);
                return img;
            }
        }


        Bitmap _resultBmp;
        private void cmdFreeTransform_Click(object sender, EventArgs e)
        {


            PixelFarm.CpuBlit.Imaging.FreeTransform freeTx = new PixelFarm.CpuBlit.Imaging.FreeTransform();
            MemBitmap bmp = LoadImage("Samples\\lion1.png");
            freeTx.Interpolation = PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode.Bicubic;// PixelFarm.Agg.Imaging.FreeTransform.InterpolationMode.Bilinear;
       
            //freeTx.SetFourCorners(
            //    new PixelFarm.VectorMath.PointF(0, 0),
            //    new PixelFarm.VectorMath.PointF(bmp.Width / 5, 0),
            //    new PixelFarm.VectorMath.PointF(bmp.Width / 5, bmp.Height / 5),
            //    new PixelFarm.VectorMath.PointF(0, bmp.Height / 5)
            //);

             freeTx.SetFourCorners(
                new PixelFarm.VectorMath.PointF(0, 0),
                new PixelFarm.VectorMath.PointF(bmp.Width * 3, 0),
                new PixelFarm.VectorMath.PointF(bmp.Width * 3, bmp.Height * 3),
                new PixelFarm.VectorMath.PointF(0, bmp.Height * 3)
            );


            using (MemBitmap transferBmp = freeTx.GetTransformedBitmap(bmp))
            {
                SaveImage(transferBmp, "test01_tx" + freeTx.Interpolation + ".png");
            }

            pictureBox1.Image = null;
            if (_resultBmp != null)
            {
                _resultBmp.Dispose();
                _resultBmp = null;
            }
            pictureBox1.Image = _resultBmp = new Bitmap("test01_tx" + freeTx.Interpolation + ".png");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MemBitmap srcBmp = LoadImage("Samples\\lion1.png");
            PaintFx.Surface src = CreateSurfaceFromMemBitmap(srcBmp);

            MemBitmap dstBmp = new MemBitmap(srcBmp.Width / 2, srcBmp.Height / 2);
            PaintFx.Surface dst = CreateSurfaceFromMemBitmap(dstBmp);


            dst.SuperSamplingBlit(src, new PixelFarm.Drawing.Rectangle(0, 0, src.Width / 5, src.Height / 5));

            SaveImage(dstBmp, "test01_txPaintFx.png");

            pictureBox1.Image = null;
            if (_resultBmp != null)
            {
                _resultBmp.Dispose();
                _resultBmp = null;
            }

            pictureBox1.Image = _resultBmp = new Bitmap("test01_txPaintFx.png");
        }
    }
}
