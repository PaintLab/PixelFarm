//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TypographyTest.WinForms
{
    public partial class GlyphTextureBitmapGenUserControl : UserControl
    {
        GlyphTextureBitmapGenController _glyphTextureBitmapController;
        Typography.OpenFont.Typeface _typeface;

        public GlyphTextureBitmapGenUserControl()
        {
            InitializeComponent();
            _glyphTextureBitmapController = new GlyphTextureBitmapGenController();
            SelectedScriptLangs = new List<Typography.OpenFont.ScriptLang>();
        }
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
            lstTextureType.Items.Add(TextureKind.Stencil);
            lstTextureType.Items.Add(TextureKind.Msdf);
            lstTextureType.SelectedIndex = 0;

            //
            this.textBox1.Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-*/?=(){}[]%@#^$&|.";
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
            string sampleFontFile = sampleFontFile = _typeface.Filename ?? "";

            TextureKind selectedTextureKind = (TextureKind)lstTextureType.SelectedItem;
            char[] chars = this.textBox1.Text.ToCharArray();
            GlyphTextureBitmapGenController.CreateSampleTextureFontFromScriptLangs(
               _typeface,
               18,
               selectedTextureKind,
               SelectedScriptLangs.ToArray(),
               "d:\\WImageTest\\sample_" + selectedTextureKind + "_" +
               System.IO.Path.GetFileNameWithoutExtension(sampleFontFile) + ".png");
        }

        private void cmdMakeFromSelectedString_Click(object sender, EventArgs e)
        {
            //create a simple stencil texture font

            //string sampleFontFile = "../../../TestFonts/tahoma.ttf";
            if (this.textBox1.Text == null || _typeface == null)
            {
                return;
            }

            string sampleFontFile = sampleFontFile = _typeface.Filename ?? "";
            //
            TextureKind selectedTextureKind = (TextureKind)lstTextureType.SelectedItem;
            char[] chars = this.textBox1.Text.ToCharArray();
            GlyphTextureBitmapGenController.CreateSampleTextureFontFromInputChars(
               _typeface,
               18,
               selectedTextureKind,
               chars, //eg. ABCD
               "d:\\WImageTest\\sample_" + selectedTextureKind + "_" +
               System.IO.Path.GetFileNameWithoutExtension(sampleFontFile) + ".png");
        }
    }
}
