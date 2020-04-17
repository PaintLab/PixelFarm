//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using Typography.OpenFont;
using Typography.Contours;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.Drawing;

namespace Mini
{
    class FontBuilderConfig
    {

        readonly Dictionary<string, GlyphTextureBuildDetail> _scriptDic = new Dictionary<string, GlyphTextureBuildDetail>();
        readonly Dictionary<string, float> _sizeDic = new Dictionary<string, float>();

        public List<FontBuilderTask> BuilderTasks { get; private set; } = new List<FontBuilderTask>();
        public TextureKind TextureKind { get; set; }
        public string FontFilename { get; set; }

        public void AddScriptLangAndHint(string lang, string hint)
        {

            ScriptLang scriptLang = ScriptLangs.GetRegisteredScriptLangFromLanguageName(lang);
            if (scriptLang == null)
            {
                //not found this lang
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                GlyphTextureBuildDetail buildDetail = new GlyphTextureBuildDetail();

                buildDetail.ScriptLang = scriptLang;
                switch (hint)
                {
                    case "truetype":
                        buildDetail.HintTechnique = HintTechnique.TrueTypeInstruction;
                        break;
                    case "truetype_vertical_only":
                        buildDetail.HintTechnique = HintTechnique.TrueTypeInstruction_VerticalOnly;
                        break;
                    case "cff":
                        buildDetail.HintTechnique = HintTechnique.CffHintInstruction;
                        break;
                }

                _scriptDic[lang] = buildDetail; //replace
            }

        }
        public void SetSizeList(string sizeList)
        {
            string[] splits = sizeList.Split(',');
            foreach (string s in splits)
            {
                string s1 = s.Trim().ToLower();
                if (s1.Length > 0)
                {
                    if (!float.TryParse(s, out float size))
                    {
                        //not found
                        System.Diagnostics.Debugger.Break();
                    }
                    _sizeDic[s1] = size;
                }
            }
        }
        public void SetTextureKind(string textureKind)
        {
            switch (textureKind)
            {
                //TODO:
                default: throw new NotSupportedException();//??
                case "msdf3":
                    TextureKind = TextureKind.Msdf;
                    break;
                case "msdf":
                    TextureKind = TextureKind.Msdf;
                    break;
                case "stencil_lcd":
                    TextureKind = TextureKind.StencilLcdEffect;
                    break;
            }
        }
        public void BuildConfigDetail()
        {

            //generate task detail
            //we need to interpolate all input data
            BuilderTasks.Clear();
            foreach (float fontSize in _sizeDic.Values)
            {

                var fontBuildTask = new FontBuilderTask
                {
                    Size = fontSize,
                    TextureKind = this.TextureKind,
                };

                //script and hint technique
                List<GlyphTextureBuildDetail> textureDetails = new List<GlyphTextureBuildDetail>();
                foreach (GlyphTextureBuildDetail detail in _scriptDic.Values)
                {
                    textureDetails.Add(detail);
                }

                fontBuildTask.TextureBuildDetails = textureDetails;
                BuilderTasks.Add(fontBuildTask);

            }
        }
    }
    class FontBuilderTask
    {

        /// <summary>
        /// size in point unit
        /// </summary>
        public float Size;
        public List<GlyphTextureBuildDetail> TextureBuildDetails;
        public TextureKind TextureKind;
      
#if DEBUG
        public override string ToString()
        {
            return Size.ToString();
        }
#endif
    }
}