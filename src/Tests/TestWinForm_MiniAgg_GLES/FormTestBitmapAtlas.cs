//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;

namespace Mini
{
    public partial class FormTestBitmapAtlas : Form
    {

        string _srcDir = "Samples\\BmpAtlasItems";

        public FormTestBitmapAtlas()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;

            listBox2.SelectedIndexChanged += ListBox2_SelectedIndexChanged;
        }


        static void DisposeExistingPictureBoxImage(PictureBox pictureBox)
        {
            if (pictureBox.Image is Bitmap currentBmp)
            {
                pictureBox.Image = null;
                currentBmp.Dispose();
                currentBmp = null;
            }
        }
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            string filename = (string)listBox1.SelectedItem;

            DisposeExistingPictureBoxImage(pictureBox1);

            pictureBox1.Image = new Bitmap(filename);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void FormTestBitmapAtlas_Load(object sender, EventArgs e)
        {
            //load bitmap file list
            lbl_src.Text = "src:" + _srcDir;
            string[] filenames = Directory.GetFiles(_srcDir, "*.png");
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

            string atlas_file = "test1_atlas";
            BuildBitmapAtlas(_srcDir, LoadBmp, atlas_file);

            DisposeExistingPictureBoxImage(pictureBox2);

            //total atlas
            pictureBox2.Image = new Bitmap(atlas_file + ".png");
        }


        SimpleBitmapAtlasBuilder _bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
        SimpleBitmapAtlas _bitmapAtlas;
        MemBitmap _totalAtlasImg;

        private void cmdReadBmpAtlas_Click(object sender, EventArgs e)
        {
            string atlas_file = "test1_atlas";

            _bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
            _bitmapAtlas = _bmpAtlasBuilder.LoadAtlasInfo(atlas_file + ".info")[0];//default atlas

            _totalAtlasImg = LoadBmp(atlas_file + ".png");
            //-----
            int count = _bitmapAtlas.ImgUrlDict.Count;
            listBox2.Items.Clear();

            foreach (var kv in _bitmapAtlas.ImgUrlDict)
            {
                listBox2.Items.Add(kv.Key);
            }
            DisposeExistingPictureBoxImage(pictureBox2);
            pictureBox2.Image = new Bitmap(atlas_file + ".png");

            //for (int i = 0; i < count; ++i)
            //{
            //    if (bitmapAtlas.TryGetBitmapMapData((ushort)i, out BitmapMapData bmpMapData))
            //    {
            //        listBox2.Items.Add(bmpMapData);
            //        //test copy data from bitmap
            //        //MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
            //        //itemImg.SaveImage("test1_atlas_item" + i + ".png");
            //    }
            //}

            ////test,
            //{
            //    if (bitmapAtlas.TryGetBitmapMapData(@"\chk_checked.png", out BitmapMapData bmpMapData))
            //    {
            //        //MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
            //        //itemImg.SaveImage("test1_atlas_item_a.png");
            //    }
            //}
        }

        Graphics _pic2Gfx;
        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_bitmapAtlas == null) return;

            string imgUri = (string)listBox2.SelectedItem;
            if (_pic2Gfx == null)
            {
                _pic2Gfx = pictureBox2.CreateGraphics();
            }

            _pic2Gfx.Clear(Color.White);
            if (pictureBox2.Image != null)
            {
                _pic2Gfx.DrawImage(pictureBox2.Image, 0, 0);
            }

            if (_bitmapAtlas.TryGetGlyphMapData(imgUri, out TextureGlyphMapData bmpMapData))
            {

                _pic2Gfx.DrawRectangle(Pens.Red,
                    new Rectangle(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height));

                //example
                MemBitmap itemImg = _totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
                //convert from membitmap to bmp
                int[] buffer = MemBitmap.CopyImgBuffer(itemImg);

                System.Drawing.Bitmap test = new Bitmap(bmpMapData.Width, bmpMapData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var bmp_data = test.LockBits(new Rectangle(0, 0, bmpMapData.Width, bmpMapData.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, test.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmp_data.Scan0, buffer.Length);
                test.UnlockBits(bmp_data);


                DisposeExistingPictureBoxImage(pictureBox1);
                pictureBox1.Image = test;
            }
        }

        public static void BuildBitmapAtlas(string imgdir, Func<string, MemBitmap> imgLoader, string outputFilename, bool test_extract = false)
        {

            //demonstrate how to build a bitmap atlas

            //1. create builder
            var bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();

            //2. collect all image-files
            int imgdirNameLen = imgdir.Length;
            string[] filenames = System.IO.Directory.GetFiles(imgdir, "*.png");
            ushort index = 0;

            Dictionary<string, ushort> imgDic = new Dictionary<string, ushort>();
            foreach (string f in filenames)
            {
                //3. load a bitmap
                MemBitmap itemBmp = imgLoader(f);
                //4. get information about it

                var atlasItem = new BitmapAtlasItem(itemBmp.Width, itemBmp.Height);
                atlasItem.SetImageBuffer(MemBitmap.CopyImgBuffer(itemBmp));
                //5. add to builder
                //bmpAtlasBuilder.AddAtlasItemImage(index, atlasItem);
                bmpAtlasBuilder.AddGlyph(index, atlasItem);
                string imgPath = f.Substring(imgdirNameLen);
                imgDic.Add(imgPath, index);
                index++;

                //------------
#if DEBUG
                if (index >= ushort.MaxValue)
                {
                    throw new NotSupportedException();
                }
#endif
                //------------
            }


            string atlasInfoFile = outputFilename + ".info";
            string totalImgFile = outputFilename + ".png";

            //5. merge all small images into a bigone 
            MemBitmap totalImg = bmpAtlasBuilder.BuildSingleImage(false);
            bmpAtlasBuilder.ImgUrlDict = imgDic;
            bmpAtlasBuilder.SetAtlasInfo(TextureKind.Bitmap, 0);//font size
            //6. save atlas info and total-img (.png file)
            bmpAtlasBuilder.SaveAtlasInfo(atlasInfoFile);
            totalImg.SaveImage(totalImgFile);

            //----------------------
            //test, read data back
            //----------------------
            if (test_extract)
            {
                bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
                SimpleBitmapAtlas bitmapAtlas = bmpAtlasBuilder.LoadAtlasInfo(atlasInfoFile)[0];
                //
                MemBitmap totalAtlasImg = imgLoader(totalImgFile);
                bitmapAtlas.MainBitmap = imgLoader(totalImgFile);

                //-----
                for (int i = 0; i < index; ++i)
                {
                    if (bitmapAtlas.TryGetGlyphMapData((ushort)i, out TextureGlyphMapData bmpMapData))
                    {
                        //test copy data from bitmap
                        MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
                        itemImg.SaveImage("test1_atlas_item" + i + ".png");
                    }
                }
                //test,
                {
                    if (bitmapAtlas.TryGetGlyphMapData(@"\chk_checked.png", out TextureGlyphMapData bmpMapData))
                    {
                        MemBitmap itemImg = totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height);
                        itemImg.SaveImage("test1_atlas_item_a.png");
                    }
                }
            }
        }

    }
}
