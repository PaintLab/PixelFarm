using System;
using System.Drawing;
using System.Windows.Forms;

using LayoutFarm.Text;
using Win32;
using System.Text;

namespace LayoutFarm.TestTextFlow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static TextLayerFontServiceImpl fontServiceImpl;
        private void Form1_Load(object sender, EventArgs e)
        {
            if (fontServiceImpl == null)
            {
                fontServiceImpl = new TextLayerFontServiceImpl();
                RequestFontImpl reqFont = new RequestFontImpl("tahoma", 14);
                WinGdiFont gdiFont = WinGdiFontSystem.GetWinGdiFont(reqFont);
                fontServiceImpl._defaultFont = reqFont;

            }
            FontService1.RegisterFontService(fontServiceImpl);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            ////1. create text flow layer
            //EditableTextFlowLayer flowLayer = new EditableTextFlowLayer();

            ////2. writer for this flow
            //TextLineWriter writer = new TextLineWriter(flowLayer);
            ////3. we write to the layer via the writer
            //writer.CharIndex = -1;//
            //writer.AddCharacter('A');
            //writer.AddCharacter('B');
            ////\r\n
            //int lineNo1 = writer.LineNumber;
            ////-----------------------------------------------
            //writer.SplitToNewLine();
            //writer.MoveToLine(lineNo1 + 1);
            //writer.CharIndex = -1; //do home
            ////-----------------------------------------------



            //writer.AddCharacter('C');
            //writer.AddCharacter('D');

            //StringBuilder stbuilder = new StringBuilder();
            //writer.CopyContentToStrignBuilder(stbuilder);
            //System.Diagnostics.Debug.WriteLine(stbuilder.ToString());
        }
        class MyTextSurfaceListener : TextSurfaceEventListener
        {

        }
        class MyTextEditRenderBox : TextEditRenderBox
        {
            TextSurfaceEventListener _listener;
            public TextSurfaceEventListener TextSurfaceEventListener
            {
                get
                {
                    return _listener;
                }
            }
            public void SetTextSurfaceEventListner(TextSurfaceEventListener listener)
            {
                _listener = listener;
            }
            public void NotifyTextContentSizeChanged()
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //1. create text flow layer
            EditableTextFlowLayer flowLayer = new EditableTextFlowLayer();
            //
            //2.
            MyTextEditRenderBox renderBox = new MyTextEditRenderBox();

            //3. listen important event from text layer
            MyTextSurfaceListener listener = new MyTextSurfaceListener();
            renderBox.SetTextSurfaceEventListner(listener);

            //4. create text layer controller
            //this simulate user's input
            InternalTextLayerController layerController = new InternalTextLayerController(renderBox, flowLayer);
            layerController.CurrentLineNumber = 0;
            layerController.TryMoveCaretTo(-1);
            layerController.AddCharToCurrentLine('a');
            layerController.AddCharToCurrentLine('b');
            layerController.AddCharToCurrentLine('C');
            layerController.SplitCurrentLineIntoNewLine();
            layerController.AddCharToCurrentLine('x');
            layerController.AddCharToCurrentLine('Y');
            //------------
            layerController.CurrentLineNumber = 0; //move to line 0 again //should be 1? 
            //------------
            //
            StringBuilder stbuilder = new StringBuilder();
            layerController.CopyAllToPlainText(stbuilder);
            //
            System.Diagnostics.Debug.WriteLine(stbuilder.ToString());
        }
    }
}
