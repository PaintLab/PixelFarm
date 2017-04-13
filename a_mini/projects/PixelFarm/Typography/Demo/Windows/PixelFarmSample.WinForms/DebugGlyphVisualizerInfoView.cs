//MIT, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Typography.Rendering;

namespace SampleWinForms.UI
{

    delegate void SimpleAction();
    class DebugGlyphVisualizerInfoView
    {

        TreeView _treeView;
        TreeNode _rootNode;
        List<EdgeLine> _edgeLines = new List<EdgeLine>();
        int _addDebugMarkOnEdgeNo = 0;

        public event EventHandler RequestGlyphRender;
        SimpleAction _flushOutput;
        bool _clearInfoView;
        int _testEdgeCount;

        public void SetTreeView(TreeView treeView)
        {
            _treeView = treeView;

            _treeView.NodeMouseClick += (s, e) => DrawMarkedNode(e.Node);
            _treeView.KeyDown += (s, e) =>
            {
                TreeNode selectedNode = _treeView.SelectedNode;
                if (selectedNode != null)
                {
                    DrawMarkedNode(selectedNode);
                }
            };


            _treeView.Nodes.Clear();
            _rootNode = new TreeNode();
            _rootNode.Text = "root";
            _treeView.Nodes.Add(_rootNode);
            _clearInfoView = true;//default
        }
        public void SetFlushOutputHander(SimpleAction flushOutput)
        {
            _flushOutput = flushOutput;
        }

        void DrawMarkedNode(TreeNode node)
        {

            NodeInfo nodeinfo = node.Tag as NodeInfo;
            if (nodeinfo == null) { return; }
            //---------------
            _addDebugMarkOnEdgeNo = nodeinfo.EdgeNo;
            if (RequestGlyphRender != null)
            {
                _clearInfoView = false;
                RequestGlyphRender(this, EventArgs.Empty);
                if (_flushOutput != null)
                {
                    //TODO: review here
                    _flushOutput();
                }
                _clearInfoView = true;
            }
        }
        public float PxScale { get; set; }
        public void Clear()
        {
            if (_clearInfoView)
            {
                _rootNode.Nodes.Clear();
                _edgeLines.Clear();
            }
            _testEdgeCount = 0;
        }
        public void ShowEdge(EdgeLine edge)
        {
            HasDebugMark = false; //reset for this 

            //---------------
            if (_testEdgeCount == _addDebugMarkOnEdgeNo)
            {
                HasDebugMark = true;
            }
            _testEdgeCount++;
            if (!_clearInfoView)
            {
                return;
            }
            //---------------
            Poly2Tri.TriangulationPoint p = edge.p;
            Poly2Tri.TriangulationPoint q = edge.q;
            var u_data_p = p.userData as GlyphPoint2D;
            var u_data_q = q.userData as GlyphPoint2D;

            //-------------------------------

            NodeInfo nodeInfo = new NodeInfo(edge, _edgeLines.Count);
            TreeNode nodeEdge = new TreeNode();
            nodeEdge.Tag = nodeInfo;
            nodeEdge.Text = "e " + _testEdgeCount  +" :(" + p.X + "," + p.Y + ")" + "=>(" + q.X + "," + q.Y + ")";
            _rootNode.Nodes.Add(nodeEdge);
            //------------------------------- 

            _edgeLines.Add(edge);
        }
        public bool HasDebugMark
        {
            get;
            set;
        }
        public void SetDebugMarkOnEdgeNo(int edgeNo)
        {
            this._addDebugMarkOnEdgeNo = edgeNo;
        }

        class NodeInfo
        {
            EdgeLine edge;
            public NodeInfo(EdgeLine edge, int edgeNo)
            {
                this.edge = edge;
                this.EdgeNo = edgeNo;
            }
            public int EdgeNo
            {
                get; set;
            }
        }
    }
}