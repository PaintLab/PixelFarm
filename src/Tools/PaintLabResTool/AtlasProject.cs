//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;

using System.IO;
using System.Xml;
using Typography.OpenFont;

namespace Mini
{

    class AtlasProject
    {
        public AtlasProject() { }

        public bool Isloaded { get; private set; }
        public string Filename { get; set; }
        public string FullFilename { get; set; }
        public override string ToString() => Filename;
        public List<AtlasItemSourceFile> Items { get; private set; }
        public string OutputFilename { get; set; }

        public bool GenerateCsSource { get; set; } = true;
        public string CsSourceNamespace { get; set; }

        public bool IsBitmapAtlasProject { get; set; }
        public bool IsFontAtlasProject { get; set; }
        public bool IsResourceProject { get; set; }

        public void LoadProjectDetail()
        {

            //we support bitmap font atlas
            //and font-atlas

            Items = new List<AtlasItemSourceFile>();

            string dir = Path.GetDirectoryName(FullFilename);
            OutputFilename = dir + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(FullFilename);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(FullFilename);
            string xmlNs = xmldoc.DocumentElement.GetAttribute("xmlns");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmldoc.NameTable);

            //xml note: check if project file has namespace or not
            //if it has namespace=> we need to add namespace manager too

            string onlyDirName = Path.GetDirectoryName(FullFilename);
            string ns = "";
            if (xmlNs != "")
            {
                ns = "cs";
                nsmgr.AddNamespace(ns, xmlNs);
                ns += ":";
            }

            foreach (XmlElement content in xmldoc.DocumentElement.SelectNodes("//" + ns + "Content", nsmgr))
            {
                //content node
                string include = content.GetAttribute("Include");
                string extension = Path.GetExtension(include);
                switch (extension)
                {
                    case ".css":
                    case ".html":
                    case ".htm":
                    case ".svg":
                    case ".txt":
                    case ".xml": //data or config
                    case ".png":
                        {
                            var atlasItemFile = new AtlasItemSourceFile();
                            atlasItemFile.Include = include;
                            atlasItemFile.Extension = extension;
                            atlasItemFile.AbsoluteFilename = PathUtils.GetAbsolutePathRelativeTo(include, onlyDirName);
                            atlasItemFile.FileExist = File.Exists(atlasItemFile.AbsoluteFilename);
                            if (content.SelectSingleNode(ns + "Link", nsmgr) is XmlElement linkNode)
                            {
                                atlasItemFile.Link = linkNode.InnerText;
                            }
                            else
                            {
                                //no link node, use include
                                atlasItemFile.Link = include;
                            }
                            Items.Add(atlasItemFile);
                        }
                        break;
                }
            }

            foreach (XmlElement content in xmldoc.DocumentElement.SelectNodes("//" + ns + "None", nsmgr))
            {
                string include = content.GetAttribute("Include");
                string extension = Path.GetExtension(include);
                switch (extension)
                {
                    case ".ttf":
                    case ".otf":
                        {
                            //TODO: include webfont (woff2, woff1), ttc,otc  
                            var atlasItemFile = new AtlasItemSourceFile();
                            atlasItemFile.Include = include;
                            atlasItemFile.Extension = extension;
                            atlasItemFile.AbsoluteFilename = PathUtils.GetAbsolutePathRelativeTo(include, onlyDirName);
                            atlasItemFile.FileExist = File.Exists(atlasItemFile.AbsoluteFilename);
                            if (content.SelectSingleNode(ns + "Link", nsmgr) is XmlElement linkNode)
                            {
                                atlasItemFile.Link = linkNode.InnerText;
                            }
                            else
                            {
                                //no link node, use include
                                atlasItemFile.Link = include;
                            }
                            Items.Add(atlasItemFile);
                        }
                        break;
                }
            }
            Isloaded = true;

            //------
            //then resolve for absolute filename
            foreach (AtlasItemSourceFile atlasItem in Items)
            {
                string include = atlasItem.Include;
                if (Path.IsPathRooted(include))
                {
                    atlasItem.AbsoluteFilename = include;
                }
                else
                {
                    //relative to the project file

                }
            }

            string onlyFilename = Path.GetFileNameWithoutExtension(Filename);
            CsSourceNamespace = "Atlas_AUTOGEN_." + onlyFilename;
            //------ 
            //check for config data 

            ResolveItems();
        }


        Dictionary<string, Typeface> _typefaces = new Dictionary<string, Typeface>();

        public Typeface GetTypeface(string fontfilename)
        {
            return _typefaces[fontfilename];
        }
        void ResolveItems()
        {

            _typefaces.Clear();
            foreach (AtlasItemSourceFile atlasSrcItem in Items)
            {
                //check file
                if (!File.Exists(atlasSrcItem.AbsoluteFilename))
                {
                    throw new NotSupportedException();
                }

                switch (atlasSrcItem.Extension)
                {
                    case ".ttc":
                    case ".otf":
                    case ".ttf":
                    case ".otc":
                        {
                            atlasSrcItem.Kind = AtlasItemSourceKind.Font;
                            using (FileStream fs = new FileStream(atlasSrcItem.AbsoluteFilename, FileMode.Open))
                            {
                                OpenFontReader reader = new OpenFontReader();
                                _typefaces.Add(Path.GetFileName(atlasSrcItem.AbsoluteFilename), reader.Read(fs));
                            }
                        }
                        break;
                    case ".css":
                    case ".html":
                    case ".htm":
                    case ".svg":
                        atlasSrcItem.Kind = AtlasItemSourceKind.Text;
                        break;
                    case ".png":
                    case ".jpg":
                        atlasSrcItem.Kind = AtlasItemSourceKind.Image;
                        IsBitmapAtlasProject = true;
                        break;
                    case ".xml":
                        {
                            atlasSrcItem.Kind = AtlasItemSourceKind.Data;
                            TryReadConfigFile(atlasSrcItem);
                            switch (atlasSrcItem.Kind)
                            {
                                case AtlasItemSourceKind.FontAtlasConfig:
                                    IsFontAtlasProject = true;
                                    break;
                                case AtlasItemSourceKind.ResourceConfig:
                                    IsResourceProject = true;
                                    break;
                            }
                        }
                        break;
                }
            }

        }
        static void TryReadConfigFile(AtlasItemSourceFile atlasSrcItem)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(atlasSrcItem.AbsoluteFilename);
            XmlElement docElem = xmldoc.DocumentElement;
            switch (docElem.Name)
            {
                case "typeface_builder_config":
                    //this  typeface config
                    ReadFontBuilderConfig(atlasSrcItem, docElem);
                    //change item kind to config
                    atlasSrcItem.Kind = AtlasItemSourceKind.FontAtlasConfig;
                    atlasSrcItem.IsConfig = true;
                    break;
                case "resource_builder_config":
                    atlasSrcItem.Kind = AtlasItemSourceKind.ResourceConfig;
                    atlasSrcItem.IsConfig = true;
                    break;
            }
        }
        static void ReadFontBuilderConfig(AtlasItemSourceFile atlasSrcItem, XmlElement docElem)
        {
            FontBuilderConfig fontBuilderConfig = new FontBuilderConfig();
            //cut 1 extension
            //if font filename=aaa.ttf
            //configure must be aaa.ttf.xml

            fontBuilderConfig.FontFilename = Path.GetFileNameWithoutExtension(atlasSrcItem.AbsoluteFilename);
            atlasSrcItem.FontBuilderConfig = fontBuilderConfig;

            foreach (XmlElement setElem in docElem.SelectNodes("set"))
            {
                //defail
                fontBuilderConfig.SetTextureKind(setElem.GetAttribute("texture_kind"));
                foreach (XmlNode cc in setElem.ChildNodes)
                {
                    if (cc is XmlElement childElem)
                    {
                        switch (childElem.Name)
                        {
                            case "script_lang":
                                fontBuilderConfig.AddScriptLangAndHint(
                                    childElem.GetAttribute("lang"),
                                    childElem.GetAttribute("hint"));
                                break;
                            case "size":
                                fontBuilderConfig.SetSizeList(childElem.InnerText);
                                break;
                        }
                    }
                }
            }
            fontBuilderConfig.BuildConfigDetail();
        }

    }


    enum AtlasItemSourceKind
    {
        Image,
        Font,
        Data,
        Text,


        FontAtlasConfig,
        BitmapAtlasConfig,
        ResourceConfig,
    }

    class AtlasItemSourceFile
    {
        public string Include { get; set; }
        public string Link { get; set; }
        public string AbsoluteFilename { get; set; }
        public bool FileExist { get; set; }
        public string Extension { get; set; }
        public override string ToString()
        {
            if (Link != null)
            {
                return Link;
            }
            else
            {
                return Include;
            }
        }

        public AtlasItemSourceKind Kind { get; set; }


        public FontBuilderConfig FontBuilderConfig { get; set; }
        public bool IsConfig { get; set; }
    }



}