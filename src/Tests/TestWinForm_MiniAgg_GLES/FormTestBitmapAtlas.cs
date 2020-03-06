//MIT, 2020, WinterDev
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Mini
{
    public partial class FormTestBitmapAtlas : Form
    {
        Bitmap _currentBmp;
        public FormTestBitmapAtlas()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            string filename = (string)listBox1.SelectedItem;

            pictureBox1.Image = null;
            if (_currentBmp != null)
            {
                _currentBmp.Dispose();
                _currentBmp = null;
            }
            pictureBox1.Image = _currentBmp = new Bitmap(filename);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void FormTestBitmapAtlas_Load(object sender, EventArgs e)
        {
            //load bitmap file list

            string[] filenames = Directory.GetFiles("Samples\\BmpAtlasItems", "*.png");
            foreach (string filename in filenames)
            {
                listBox1.Items.Add(filename);
            }

        }
        static PixelFarm.CpuBlit.MemBitmap LoadBmp(string filename)
        {
            using (System.Drawing.Bitmap bmp = new Bitmap(filename))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                PixelFarm.CpuBlit.MemBitmap membmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(bmp.Width, bmp.Height, bmp.Width * bmp.Height * 4, bmpdata.Scan0);
                bmp.UnlockBits(bmpdata);
                return membmp;
            }
        }
        private void cmdBuildBmpAtlas_Click(object sender, EventArgs e)
        {

            OpenTkEssTest.TestBitmapAtlasBuilder.Test("Samples\\BmpAtlasItems", LoadBmp, "test_bmpAtlas");
            pictureBox2.Image = new Bitmap("test_bmpAtlas.png");

            ////-----
            ////test, read data back
            //bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
            //SimpleBitmaptAtlas bitmapAtlas = bmpAtlasBuilder.LoadAtlasInfo(atlasInfoFile);
            ////
            //MemBitmap totalAtlasImg = imgLoader(totalImgFile);
            //AtlasItemImage atlasImg = new AtlasItemImage(totalAtlasImg.Width, totalAtlasImg.Height);
            //bitmapAtlas.TotalImg = atlasImg;

            ////-----
            //for (int i = 0; i < index; ++i)
            //{
            //    if (bitmapAtlas.TryGetBitmapMapData((ushort)i, out BitmapMapData bmpMapData))
            //    {
            //        //test copy data from bitmap
            //        MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
            //        itemImg.SaveImage("test1_atlas_item" + i + ".png");
            //    }
            //}
            ////test,
            //{
            //    if (bitmapAtlas.TryGetBitmapMapData(@"\chk_checked.png", out BitmapMapData bmpMapData))
            //    {
            //        MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
            //        itemImg.SaveImage("test1_atlas_item_a.png");
            //    }
            //}


        }

    }
}
