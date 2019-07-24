//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm.TextEditing
{
    public static class PlainTextDocumentHelper
    {
        public static PlainTextDocument CreatePlainTextDocument(string orgText)
        {
            PlainTextDocument doc = new PlainTextDocument();
            using (System.IO.StringReader reader = new System.IO.StringReader(orgText))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    //...
                    doc.AppendLine(line);
                    line = reader.ReadLine();
                }
            }
            return doc;
        }
        public static PlainTextDocument CreatePlainTextDocument(IEnumerable<string> lines)
        {
            PlainTextDocument doc = new PlainTextDocument();
            foreach (string line in lines)
            {
                doc.AppendLine(line);
            }
            return doc;
        }
    }
}