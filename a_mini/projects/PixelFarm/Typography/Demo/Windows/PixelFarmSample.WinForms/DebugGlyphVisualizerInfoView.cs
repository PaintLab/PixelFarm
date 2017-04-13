//MIT, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Typography.Rendering;
using PixelFarm;
using PixelFarm.Agg;
namespace SampleWinForms.UI
{

    delegate void SimpleAction();
    class DebugGlyphVisualizerInfoView
    {

        TreeView _treeView;
        TreeNode _rootNode;
        TreeNode _borderNode;
        TreeNode _tessEdgeNode;

        List<EdgeLine> _edgeLines = new List<EdgeLine>();
        int _addDebugMarkOnEdgeNo = 0;
        int _addDebugVertexCmd = 0;

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
            //
            //original edges
            _borderNode = new TreeNode();
            _borderNode.Text = "borders";
            _rootNode.Nodes.Add(_borderNode);
            //
            //edges
            _tessEdgeNode = new TreeNode();
            _tessEdgeNode.Text = "tess_edges";
            _rootNode.Nodes.Add(_tessEdgeNode);
            //

            _clearInfoView = true;//default
        }
        public void SetFlushOutputHander(SimpleAction flushOutput)
        {
            _flushOutput = flushOutput;
        }
        public int DebugMarkVertexCommand
        {
            get
            {
                return _addDebugVertexCmd;
            }
        }
        void DrawMarkedNode(TreeNode node)
        {

            NodeInfo nodeinfo = node.Tag as NodeInfo;
            if (nodeinfo == null) { return; }
            //---------------
            //what kind of nodeinfo
            //--------------- 

            switch (nodeinfo.NodeKind)
            {
                case NodeInfoKind.TessEdge:
                    {
                        _addDebugMarkOnEdgeNo = nodeinfo.TessEdgeNo;
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
                    break;
                case NodeInfoKind.VertexCommand:
                    {
                        _addDebugVertexCmd = nodeinfo.VertexCommandNo;
                        if (RequestGlyphRender != null)
                        {
                            _clearInfoView = false;
                            RequestGlyphRender(this, EventArgs.Empty);
                            //

                            if (_flushOutput != null)
                            {
                                //TODO: review here
                                _flushOutput();
                            }
                            _clearInfoView = true;
                        }

                    }
                    break;
            }

        }
        public float PxScale { get; set; }
        public void Clear()
        {
            if (_clearInfoView)
            {
                _tessEdgeNode.Nodes.Clear();
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

            NodeInfo nodeInfo = new NodeInfo(NodeInfoKind.TessEdge, edge, _edgeLines.Count);
            TreeNode nodeEdge = new TreeNode();
            nodeEdge.Tag = nodeInfo;
            nodeEdge.Text = "e " + _testEdgeCount + " :(" + p.X + "," + p.Y + ")" + "=>(" + q.X + "," + q.Y + ")";
            _tessEdgeNode.Nodes.Add(nodeEdge);
            //------------------------------- 

            _edgeLines.Add(edge);
        }
        public void ShowBorderInfo(VertexStore vxs)
        {
            _borderNode.Nodes.Clear();
            _treeView.SuspendLayout();
           
            int count = vxs.Count;
            VertexCmd cmd;
            double x, y;
            int index = 0;
            while ((cmd = vxs.GetVertex(index, out x, out y)) != VertexCmd.NoMore)
            {
                NodeInfo nodeInfo = new NodeInfo(NodeInfoKind.VertexCommand, index);
                TreeNode node = new TreeNode();
                node.Tag = nodeInfo;
                node.Text = (index) + " " + cmd + ": (" + x + "," + y + ")";
                _borderNode.Nodes.Add(node);
                index++;
            }
            _treeView.ResumeLayout();
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


        enum NodeInfoKind
        {
            VertexCommand,
            TessEdge,
        }
        class NodeInfo
        {
            EdgeLine edge;

            public NodeInfo(NodeInfoKind nodeKind, EdgeLine edge, int edgeNo)
            {
                this.edge = edge;
                this.TessEdgeNo = edgeNo;
                this.NodeKind = nodeKind;
            }
            public NodeInfo(NodeInfoKind nodeKind, int borderNo)
            {
                this.VertexCommandNo = borderNo;
                this.NodeKind = nodeKind;
            }
            public int VertexCommandNo { get; set; }
            public NodeInfoKind NodeKind { get; set; }
            public int TessEdgeNo
            {
                get; set;
            }
        }
    }
}