using System;
using System.Drawing;
using System.Windows.Forms;

using LayoutFarm.Text;
namespace LayoutFarm.TestTextFlow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        class RequestFontImpl : RequestFont
        {
            public Font _platformFont;
        }

        class TextLayerFontServiceImpl : TextLayerFontService
        {
            public RequestFontImpl _defaultFont = new RequestFontImpl();
            public TextLayerFontServiceImpl()
            {
                _defaultFont = new RequestFontImpl();
                _defaultFont._platformFont = new Font("tahoma", 14);
            }
            public RequestFont DefaultFont
            {
                get
                {
                    return _defaultFont;
                }
            }
            public void CalculateGlyphAdvancePos(char[] buffer, int start, int len, RequestFont font, int[] outputGlyphPos)
            {
                //calculate glyph pos
                //sample only
                int j = buffer.Length;
                for (int i = 0; i < j; ++i)
                {
                    outputGlyphPos[i] = 11;
                }
            }
            public LayoutFarm.Text.Size MeasureString(char[] buffer, int start, int len, RequestFont r)
            {
                return new LayoutFarm.Text.Size();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextLayerFontServiceImpl fontService = new TextLayerFontServiceImpl();

            //1. create text flow layer
            EditableTextFlowLayer flowLayer = new EditableTextFlowLayer();
            //2. writer for this flow
            TextLineWriter writer = new TextLineWriter(flowLayer);
            //3. we write to the layer via the writer
            writer.AddCharacter('A');
        }
    }
}
