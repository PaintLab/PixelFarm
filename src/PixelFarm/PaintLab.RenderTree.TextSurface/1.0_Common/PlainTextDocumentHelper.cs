//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm.TextEditing
{
    public static class PlainTextDocumentHelper
    {
        public static List<PlainTextLine> CreatePlainTextDocument(string orgText)
        {
            var lines = new List<PlainTextLine>();
            using (System.IO.StringReader reader = new System.IO.StringReader(orgText))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    //...
                    lines.Add(new PlainTextLine(line));
                    line = reader.ReadLine();
                }
            }
            return lines;
        }
        public static List<PlainTextLine> CreatePlainTextDocument(IEnumerable<string> lines)
        {
            var plainTextLines = new List<PlainTextLine>();
            foreach (string line in lines)
            {
                plainTextLines.Add(new PlainTextLine(line));
            }
            return plainTextLines;
        }
    }
}