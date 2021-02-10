//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.Text;
namespace LayoutFarm.TextEditing
{
    public static class PlainTextDocumentHelper
    {
        public static PlainTextDocument1 CreatePlainTextDocument(string orgText)
        {
            PlainTextDocument1 doc = new PlainTextDocument1();
            doc.LoadText(orgText);
            return doc;
        }
        public static PlainTextDocument1 CreatePlainTextDocument(IEnumerable<string> lines)
        {
            PlainTextDocument1 doc = new PlainTextDocument1();
            doc.LoadText(lines);
            return doc;
        }
    }
}