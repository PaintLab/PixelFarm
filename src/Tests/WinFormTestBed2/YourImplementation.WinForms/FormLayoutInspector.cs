//Apache2, 2014-present, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.Dev
{
    public partial class FormLayoutInspector : Form
    {

#if DEBUG
        EventHandler _rootDrawMsgEventHandler;
        EventHandler _rootHitMsgEventHandler;
        LayoutFarm.UI.GraphicsViewRoot _viewroot;
        bool _pauseRecord;
#endif
        public FormLayoutInspector()
        {
            InitializeComponent();
#if DEBUG
            _rootDrawMsgEventHandler = new EventHandler(artUISurfaceViewport1_dbug_VisualRootDebugMsg);
            _rootHitMsgEventHandler = new EventHandler(artUISurfaceViewport1_dbug_VisualRootHitChainMsg);
            this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
#endif

            //listBox2.MouseDown += listBox2_MouseDown;
            //listBox3.MouseDown += listBox3_MouseDown;

            listBox2.MouseDown += ListBox2_MouseDown;
            listBox3.MouseDown += ListBox3_MouseDown1;
        }

        private void ListBox3_MouseDown1(object sender, System.Windows.Forms.MouseEventArgs e)
        {

#if DEBUG
            dbugLayoutMsg msg = listBox3.SelectedItem as dbugLayoutMsg;
            if (msg == null)
            {
                return;
            }
            switch (msg.msgOwnerKind)
            {
                case dbugLayoutMsgOwnerKind.Layer:
                    {
                        //RenderElementLayer layer = (RenderElementLayer)msg.owner;
                    }
                    break;
                case dbugLayoutMsgOwnerKind.Line:
                    {

                    }
                    break;
                case dbugLayoutMsgOwnerKind.VisualElement:
                    {
                        RenderElement ve = (RenderElement)msg.owner;
                        dbugHelper01.dbugVE_HighlightMe = ve;
                        lastestSelectVE = ve;

                        _viewroot.PaintToOutputWindow();

                    }
                    break;

            }
#endif
        }

        private void ListBox2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

#if DEBUG
            dbugLayoutMsg msg = listBox2.SelectedItem as dbugLayoutMsg;
            if (msg == null)
            {
                return;
            }
            switch (msg.msgOwnerKind)
            {
                case dbugLayoutMsgOwnerKind.Layer:
                    {
                        //RenderElementLayer layer =
                        //   (RenderElementLayer)msg.owner;
                    }
                    break;
                case dbugLayoutMsgOwnerKind.Line:
                    {

                    }
                    break;
                case dbugLayoutMsgOwnerKind.VisualElement:
                    {
                        RenderElement ve = (RenderElement)msg.owner;
                        dbugHelper01.dbugVE_HighlightMe = ve;
                        lastestSelectVE = ve;

                        _viewroot.PaintToOutputWindow();

                    }
                    break;

            }
#endif
        }


#if DEBUG
        RenderElement lastestSelectVE;
        List<dbugLayoutMsg> lastestMessages;

#endif

        void CollectList1Item(StringBuilder stBuilder)
        {
            foreach (object obj in listBox1.Items)
            {
                stBuilder.AppendLine(obj.ToString());
            }
        }
#if DEBUG
        void LoadList2NewContent(List<dbugLayoutMsg> msgs)
        {
            listBox2.Items.Clear();
            foreach (dbugLayoutMsg s in msgs)
            {
                listBox2.Items.Add(s);
            }
        }
        void LoadList3NewContent(List<dbugLayoutMsg> msgs)
        {
            listBox3.Items.Clear();
            foreach (dbugLayoutMsg s in msgs)
            {
                listBox3.Items.Add(s);
            }
        }
        void LoadList1NewContent(List<dbugLayoutMsg> msgs)
        {
            listBox1.Items.Clear();
            foreach (dbugLayoutMsg s in msgs)
            {
                listBox1.Items.Add(s);
            }
            int dumpWhen = Int32.Parse(toolStripTextBox1.Text);

            if (listBox1.Items.Count > dumpWhen)
            {

                //int j = listBox1.Items.Count; 
                //FileStream fs = new FileStream("lim_" + Guid.NewGuid().ToString() + ".txt", FileMode.Create);
                //StreamWriter strmWriter = new StreamWriter(fs);
                //strmWriter.AutoFlush = true;

                //for (int i = 0; i < j; ++i)
                //{
                //    strmWriter.WriteLine(listBox1.Items[i].ToString());
                //}

                //strmWriter.Close();
                //fs.Close();
                //fs.Dispose();
            }
        }
#endif
        public void Connect(LayoutFarm.UI.GraphicsViewRoot vwport)
        {
#if DEBUG
            _viewroot = vwport;
            IdbugOutputWindow outputWin = vwport.IdebugOutputWin;
            outputWin.dbug_VisualRootDrawMsg += _rootDrawMsgEventHandler;
            outputWin.dbug_VisualRootHitChainMsg += artUISurfaceViewport1_dbug_VisualRootHitChainMsg;
            outputWin.dbug_EnableAllDebugInfo();
#endif
        }
#if DEBUG
        protected override void OnClosing(CancelEventArgs e)
        {
            IdbugOutputWindow outputWin = _viewroot.IdebugOutputWin;
            if (outputWin != null)
            {
                outputWin.dbug_VisualRootDrawMsg -= _rootDrawMsgEventHandler;
                outputWin.dbug_VisualRootHitChainMsg -= _rootHitMsgEventHandler;
                outputWin.dbug_DisableAllDebugInfo();
            }
            base.OnClosing(e);
        }

        void artUISurfaceViewport1_dbug_VisualRootHitChainMsg(object sender, EventArgs e)
        {
            LoadList2NewContent(_viewroot.IdebugOutputWin.dbug_rootDocHitChainMsgs);
        }
        void artUISurfaceViewport1_dbug_VisualRootDebugMsg(object sender, EventArgs e)
        {
            LoadList1NewContent(_viewroot.IdebugOutputWin.dbug_rootDocDebugMsgs);
        }
        public void TogglePauseMode()
        {
            if (!_pauseRecord)
            {
                _pauseRecord = true; _viewroot.IdebugOutputWin.dbug_VisualRootDrawMsg -= _rootDrawMsgEventHandler;
                this.Text = "Pause - LayoutFarm LayoutInspector 2016";

                StringBuilder stBuilder = new StringBuilder();
                CollectList1Item(stBuilder);
                System.Windows.Forms.Clipboard.SetText(stBuilder.ToString());

            }
            else
            {
                _pauseRecord = false;
                _viewroot.IdebugOutputWin.dbug_VisualRootDrawMsg += _rootDrawMsgEventHandler;
                this.Text = "LayoutFarm LayoutInspector 2016";
            }
        }
#endif
        private void toolStripButton1_Click(object sender, EventArgs e)
        {

#if DEBUG
            int j = listBox1.Items.Count;
            StringBuilder stBuilder = new StringBuilder();
            for (int i = 0; i < j; ++i)
            {
                stBuilder.AppendLine(listBox1.Items[i].ToString());
            }

            System.Windows.Forms.Clipboard.SetText(stBuilder.ToString());
#endif
        }

        private void tlstrpDumpSelectedVisualProps_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (lastestSelectVE != null)
            {
                dbugLayoutMsgWriter writer = new dbugLayoutMsgWriter();
                lastestSelectVE.dbug_DumpVisualProps(writer);
                lastestMessages = writer.allMessages;
                listBox3.Items.Clear();
                int j = lastestMessages.Count;
                for (int i = 0; i < j; ++i)
                {
                    listBox3.Items.Add(lastestMessages[i]);
                }
            }
#endif
        }

        private void tlstrpSaveSelectedVisualProps_Click(object sender, EventArgs e)
        {
#if DEBUG
            int j = lastestMessages.Count;
            if (j > 0)
            {
                FileStream fs = new FileStream("layout_inspect" + Guid.NewGuid().ToString() + ".txt", FileMode.Create);
                StreamWriter strmWriter = new StreamWriter(fs);
                for (int i = 0; i < j; ++i)
                {
                    strmWriter.WriteLine(lastestMessages[i].ToString());
                }
                strmWriter.Close();
                fs.Close();
                fs.Dispose();
            }
#endif
        }

        private void tlstrpReArrange_Click(object sender, EventArgs e)
        {

#if DEBUG
            _viewroot.IdebugOutputWin.dbug_ReArrangeWithBreakOnSelectedNode();
#endif

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
#if DEBUG
            _viewroot.dbugPaintMeFullMode();
#endif 

        }



    }
}
