//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.BitmapAtlas;
using Typography.OpenFont;

namespace Mini
{
    public partial class FormBitmapAtlasBuilder : Form
    {

        string _atlasProjectsDir = "../../../../x_resource_projects";


        class ProjectDirHistoryMx
        {
            string _projectDirHxFile = "latest_proj_dirs.txt";
            public readonly Dictionary<string, bool> Dirs = new Dictionary<string, bool>();
            public ProjectDirHistoryMx() { }
            public void LoadHistory()
            {
                Dirs.Clear();
                if (File.Exists(_projectDirHxFile))
                {
                    //read history
                    string[] hxLines = File.ReadAllLines(_projectDirHxFile);
                    foreach (string hx_line in hxLines)
                    {
                        string hx_line2 = hx_line.Trim();
                        if (hx_line2.Length == 0) { continue; }
                        //
                        Dirs[hx_line2] = (Directory.Exists(hx_line2));
                    }
                }
            }
            public void AddHistoryAndSave(string dir)
            {
                Dirs[dir] = (Directory.Exists(dir));
                //save to files
                using (FileStream fs = new FileStream(_projectDirHxFile + ".tmp", FileMode.Create))
                using (StreamWriter w = new StreamWriter(fs))
                {
                    foreach (string s in Dirs.Keys)
                    {
                        w.WriteLine(s);
                    }
                    w.Flush();
                }

                if (File.Exists(_projectDirHxFile))
                {
                    File.Delete(_projectDirHxFile);
                }
                File.Move(_projectDirHxFile + ".tmp", _projectDirHxFile);
            }

        }

        ProjectDirHistoryMx _projectDirHxMx = new ProjectDirHistoryMx();

        public FormBitmapAtlasBuilder()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            listBox2.SelectedIndexChanged += ListBox2_SelectedIndexChanged;
            lstProjectList.SelectedIndexChanged += LstProjectList_SelectedIndexChanged;
            listBox3.SelectedIndexChanged += ListBox3_SelectedIndexChanged;

            //default
            _atlasProjectsDir = PathUtils.GetAbsolutePathRelativeTo(_atlasProjectsDir, Directory.GetCurrentDirectory());
            _projectDirHxMx.LoadHistory();//***

            if (_projectDirHxMx.Dirs.Count > 0)
            {
                //load into combo box
                foreach (string dir in _projectDirHxMx.Dirs.Keys)
                {
                    cmbProjectDirs.Items.Add(dir);
                    _atlasProjectsDir = dir;
                }
            }


            cmbProjectDirs.Text = _atlasProjectsDir;
            cmbProjectDirs.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (Directory.Exists(cmbProjectDirs.Text))
                    {
                        LoadAtlasProjects(cmbProjectDirs.Text);
                        _projectDirHxMx.AddHistoryAndSave(cmbProjectDirs.Text);
                    }
                }
            };
            cmbProjectDirs.SelectedIndexChanged += (s, e) =>
            {
                LoadAtlasProjects(cmbProjectDirs.Text);
            };

#if DEBUG
            if (!Directory.Exists(_atlasProjectsDir))
            {

            }
#endif
        }


        private void ListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem is string atlasFile)
            {
                string dir = Path.GetDirectoryName(atlasFile);
                string filename = Path.GetFileNameWithoutExtension(atlasFile);
                TestLoadBitmapAtlas(dir + "\\" + filename);
            }
        }

        AtlasProject _currentAtlasProj;
        bool _latestAtlasProjSuccess;

        private void LstProjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProjectList.SelectedItem is AtlasProject atlasProj)
            {
                _selectedAtlasItemSourceFile = null;//reset
                _currentAtlasProj = atlasProj;

                //read project detail 
                //extract only interested files
                //eg. image files
                if (!atlasProj.Isloaded)
                {
                    //load content
                    atlasProj.LoadProjectDetail();
                }
                //show detail 
                listBox1.Items.Clear();
                foreach (AtlasItemSourceFile file in atlasProj.Items)
                {
                    listBox1.Items.Add(file);
                }
            }
        }

        static void DisposeExistingPictureBoxImage(PictureBox pictureBox)
        {
            if (pictureBox.Image is Bitmap currentBmp)
            {
                pictureBox.Image = null;
                currentBmp.Dispose();
            }
        }

        AtlasItemSourceFile _selectedAtlasItemSourceFile;
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            if (listBox1.SelectedItem is AtlasItemSourceFile atlasItemFile)
            {
                _selectedAtlasItemSourceFile = atlasItemFile;
                //check if it is image file or not   
                DisposeExistingPictureBoxImage(pictureBox1);
                switch (atlasItemFile.Extension)
                {
                    case ".png":
                        pictureBox1.Image = new Bitmap(atlasItemFile.AbsoluteFilename);
                        break;
                    default:
                        break;
                }
            }

        }

        void LoadAtlasProjects(string projDir)
        {
            if (!Directory.Exists(projDir)) { return; }

            _atlasProjectsDir = projDir;
            string[] project_dirs = Directory.GetDirectories(projDir);
            lstProjectList.Items.Clear();
            foreach (string dir in project_dirs)
            {
                //check cs project file
                foreach (string cs_proj in Directory.GetFiles(dir, "*.csproj"))
                {
                    //convert to absolute path relative to specific path
                    string fullFilename = PathUtils.GetAbsolutePathRelativeTo(cs_proj, projDir);
                    lstProjectList.Items.Add(new AtlasProject() { Filename = Path.GetFileName(cs_proj), FullFilename = cs_proj });
                }
            }
        }

        private void FormTestBitmapAtlas_Load(object sender, EventArgs e)
        {
            //load project list from specific folder
            LoadAtlasProjects(_atlasProjectsDir);
            //---------
            //load atlas output folder

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
        static PixelFarm.CpuBlit.MemBitmap LoadBmp2(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            using (System.Drawing.Bitmap bmp = new Bitmap(ms))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                PixelFarm.CpuBlit.MemBitmap membmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(bmp.Width, bmp.Height, bmp.Width * bmp.Height * 4, bmpdata.Scan0);
                bmp.UnlockBits(bmpdata);
                return membmp;
            }
        }
        private void cmdBuild_Click(object sender, EventArgs e)
        {
            //build current project 
            _latestAtlasProjSuccess = false;

            if (_currentAtlasProj.IsBitmapAtlasProject)
            {
                BitmapAtlasBuilderUtils.BuildBitmapAtlas(_currentAtlasProj, LoadBmp);
                txtOutput.Text += "build bitmap-atlas=> finish\r\n";
                _latestAtlasProjSuccess = true;
                //test load the atlas on right side
                TestLoadBitmapAtlas(_currentAtlasProj.OutputFilename);
            }

            if (_currentAtlasProj.IsFontAtlasProject)
            {
                //build font atlas
                FontAtlasBuilderUtils.BuildFontAtlas(_currentAtlasProj);
                txtOutput.Text += "build bitmap-atlas=> finish\r\n";
            }

            if (_currentAtlasProj.IsResourceProject)
            {
                ResourceBuilderUtils.BuildResources(_currentAtlasProj);
            }
        }
        //------------
        SimpleBitmapAtlasBuilder _bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
        SimpleBitmapAtlas _bitmapAtlas;
        MemBitmap _totalAtlasImg;

        void TestLoadBitmapAtlas(string atlas_file)
        {
            //bitmap atlas file


            _bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
            _bitmapAtlas = _bmpAtlasBuilder.LoadAtlasInfo(atlas_file + ".info")[0];//default atlas

            _totalAtlasImg = LoadBmp(atlas_file + ".png");
            //-----
            //int count = _bitmapAtlas.ImgUrlDict.Count;
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

        private void cmdReadBmpAtlas_Click(object sender, EventArgs e)
        {
            //testload bitmap atlas
            listBox3.Items.Clear();
            string[] dirs = Directory.GetDirectories(_atlasProjectsDir);
            foreach (string dir in dirs)
            {
                //in this dir
                //check if we have bitmap atlas file or not
                string atlas_file = dir + "//" + Path.GetFileName(dir) + ".info";
                if (File.Exists(atlas_file))
                {
                    listBox3.Items.Add(atlas_file);
                }
            }

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

            if (_bitmapAtlas.TryGetItem(imgUri, out AtlasItem bmpMapData))
            {

                _pic2Gfx.DrawRectangle(Pens.Red,
                    new Rectangle(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height));

                //example
                int[] buffer = null;
                using (MemBitmap itemImg = _totalAtlasImg.CopyImgBuffer(bmpMapData.Left, bmpMapData.Top, bmpMapData.Width, bmpMapData.Height))
                {
                    buffer = MemBitmap.CopyImgBuffer(itemImg);
                }
                //convert from membitmap to bmp

                System.Drawing.Bitmap test = new Bitmap(bmpMapData.Width, bmpMapData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var bmp_data = test.LockBits(new Rectangle(0, 0, bmpMapData.Width, bmpMapData.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, test.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmp_data.Scan0, buffer.Length);
                test.UnlockBits(bmp_data);


                DisposeExistingPictureBoxImage(pictureBox1);
                pictureBox1.Image = test;
            }
        }

        private void cmdOpenOutputFolder_Click(object sender, EventArgs e)
        {
            if (_latestAtlasProjSuccess)
            {
                System.Diagnostics.Process.Start("explorer.exe", Directory.GetCurrentDirectory());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestLoadBitmapAtlas2(
                Atlas_AUTOGEN_.TestAtlas1.RawAtlasData.info,
                Atlas_AUTOGEN_.TestAtlas1.RawAtlasData.img);

        }
        void TestLoadBitmapAtlas2(byte[] atlasInfo, byte[] atlasImg)
        {
            //bitmap atlas file


            _bmpAtlasBuilder = new SimpleBitmapAtlasBuilder();
            using (MemoryStream ms = new MemoryStream(atlasInfo))
            {
                _bitmapAtlas = _bmpAtlasBuilder.LoadAtlasInfo(ms)[0];//default atlas
            }

            _totalAtlasImg = LoadBmp2(atlasImg);

            //-----
            int count = _bitmapAtlas.ImgUrlDict.Count;
            listBox2.Items.Clear();

            foreach (var kv in _bitmapAtlas.ImgUrlDict)
            {
                listBox2.Items.Add(kv.Key);
            }

            DisposeExistingPictureBoxImage(pictureBox2);

            //save to file?           
            string temp_file = Guid.NewGuid() + ".png";
            File.WriteAllBytes(temp_file, atlasImg);
            pictureBox2.Image = new Bitmap(temp_file);

        }

        private void cmdShowFontAtlas_Click(object sender, EventArgs e)
        {

            if (_selectedAtlasItemSourceFile != null)
            {
                //check if this is a font file?
                Typeface selectedTypeface = null;
                switch (_selectedAtlasItemSourceFile.Extension)
                {
                    case ".ttf":
                    case ".otf":
                        {

                            using (FileStream fs = new FileStream(_selectedAtlasItemSourceFile.AbsoluteFilename, FileMode.Open))
                            {
                                OpenFontReader reader = new OpenFontReader();
                                selectedTypeface = reader.Read(fs);
                            }
                        }
                        break;
                }

                if (selectedTypeface != null)
                {
                    SampleWinForms.FormFontAtlas formFontAtlas = new SampleWinForms.FormFontAtlas();
                    formFontAtlas.SetFont(selectedTypeface, 34);//load with init size
                    formFontAtlas.ShowDialog();
                }
            }

        }
    }
}
