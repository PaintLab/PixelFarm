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
                switch (Path.GetExtension(include))
                {
                    case ".png":
                        {
                            var atlasItemFile = new AtlasItemSourceFile();
                            atlasItemFile.Include = include;
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

        }
    }
    class AtlasItemSourceFile
    {
        public string Include { get; set; }
        public string Link { get; set; }
        public string AbsoluteFilename { get; set; }
        public bool FileExist { get; set; }
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
    }


}