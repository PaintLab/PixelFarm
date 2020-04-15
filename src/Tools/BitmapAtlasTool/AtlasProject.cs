//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;

using System.IO;
using System.Xml;
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
        public bool GenerateCsSource { get; set; } = true;
        public string CsSourceNamespace { get; set; }

        public string OutputFilename { get; set; }

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
            //check if project file has namespace or not
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
            CsSourceNamespace = "Atlas_AUTOGEN_" + onlyFilename;
            //------


            //------
            //check for config data
            foreach (AtlasItemSourceFile atlasSrcItem in Items)
            {
                switch (atlasSrcItem.Extension)
                {
                    case ".ttc":
                    case ".otf":
                    case ".ttf":
                    case ".otc":
                        atlasSrcItem.Kind = AtlasItemSourceKind.Font;
                        break;
                    case ".png":
                    case ".jpg":
                        atlasSrcItem.Kind = AtlasItemSourceKind.Image;
                        break;
                    case ".xml":
                        //this may be data or config
                        ReadConfigFile(atlasSrcItem);
                        break;
                }
            }
        }

        static void ReadConfigFile(AtlasItemSourceFile atlasSrcItem)
        {
            atlasSrcItem.Kind = AtlasItemSourceKind.Config;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(atlasSrcItem.AbsoluteFilename);
            XmlElement docElem = xmldoc.DocumentElement;
            switch (docElem.Name)
            {
                case "typeface_builder_config":
                    //this  typeface config
                    ReadFontBuilderConfig(atlasSrcItem, docElem);
                    break;
            }

        }
        static void ReadFontBuilderConfig(AtlasItemSourceFile atlasSrcItem, XmlElement docElem)
        {
            FontBuilderConfig fontBuilderConfig = new FontBuilderConfig();
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
        Config,
        Data,
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
    }



}