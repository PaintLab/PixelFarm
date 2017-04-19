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
        TreeNode _orgVxsNode;
        TreeNode _flattenVxsNode;
        TreeNode _tessEdgesNode;
        TreeNode _jointsNode;
        TreeNode _trianglesNode;
        TreeNode _bonesNode;

        //
        VertexStore _orgVxs;
        VertexStore _flattenVxs;


        List<EdgeLine> _edgeLines = new List<EdgeLine>();
        int _addDebugMarkOnEdgeNo = 0;
        int _addDebugVertexCmd = 0;

        public event EventHandler RequestGlyphRender;
        SimpleAction _flushOutput;
        bool _clearInfoView;
        int _testEdgeCount;
        TreeNode _latestSelectedTreeNode;
        public DebugGlyphVisualizer Owner
        {
            get;
            set;
        }
        public void SetTreeView(TreeView treeView)
        {
            _treeView = treeView;

            _treeView.NodeMouseClick += (s, e) =>
            {
                _clearInfoView = false;
                DrawMarkedNode(e.Node);
                _clearInfoView = true;
            };
            _treeView.KeyDown += (s, e) =>
            {
                _clearInfoView = false;
                TreeNode selectedNode = _treeView.SelectedNode;
                if (selectedNode != null && _latestSelectedTreeNode != selectedNode)
                {
                    _latestSelectedTreeNode = selectedNode;
                    DrawMarkedNode(selectedNode);
                }
                _clearInfoView = true;
            };


            _treeView.Nodes.Clear();
            _rootNode = new TreeNode();
            _rootNode.Text = "root";
            _treeView.Nodes.Add(_rootNode);
            //
            //original
            _orgVxsNode = new TreeNode();
            _orgVxsNode.Text = "org";
            _rootNode.Nodes.Add(_orgVxsNode);

            //
            //flatten borders 
            _flattenVxsNode = new TreeNode();
            _flattenVxsNode.Text = "flattenBorders";
            _rootNode.Nodes.Add(_flattenVxsNode);
            //
            //edges
            _tessEdgesNode = new TreeNode();
            _tessEdgesNode.Text = "tess_edges";
            _rootNode.Nodes.Add(_tessEdgesNode);
            //
            //joints
            _jointsNode = new TreeNode();
            _jointsNode.Text = "joints";
            _rootNode.Nodes.Add(_jointsNode);

            //triangles
            _trianglesNode = new TreeNode();
            _trianglesNode.Text = "triangles";
            _rootNode.Nodes.Add(_trianglesNode);
            //
            _bonesNode = new TreeNode();
            _bonesNode.Text = "bones";
            _rootNode.Nodes.Add(_bonesNode);

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
                default: throw new NotSupportedException();
                case NodeInfoKind.Bone:
                    {
                        if (RequestGlyphRender != null)
                        {
                            _clearInfoView = false;
                            RequestGlyphRender(this, EventArgs.Empty);

                            GlyphBone bone = nodeinfo.Bone;
                            var midPoint = bone.GetMidPoint() * PxScale;
                            Owner.DrawMarker(midPoint.X, midPoint.Y, PixelFarm.Drawing.Color.Yellow);
                            if (_flushOutput != null)
                            {
                                //TODO: review here
                                _flushOutput();
                            }
                            _clearInfoView = true;
                        }
                    }
                    break;
                case NodeInfoKind.Tri:
                    {

                        //draw glyph triangle
                        if (RequestGlyphRender != null)
                        {
                            _clearInfoView = false;
                            RequestGlyphRender(this, EventArgs.Empty);

                            GlyphTriangle tri = nodeinfo.GlyphTri;
                            var cen_x = (float)(tri.CentroidX * PxScale);
                            var cen_y = (float)(tri.CentroidY * PxScale);

                            Owner.DrawMarker(cen_x, cen_y, PixelFarm.Drawing.Color.Yellow);
                            if (_flushOutput != null)
                            {
                                //TODO: review here
                                _flushOutput();
                            }
                            _clearInfoView = true;
                        }
                    }
                    break;
                case NodeInfoKind.RibEndPoint:
                case NodeInfoKind.Joint:
                    {
                        if (RequestGlyphRender != null)
                        {
                            _clearInfoView = false;
                            RequestGlyphRender(this, EventArgs.Empty);

                            var pos = nodeinfo.Pos * PxScale;
                            Owner.DrawMarker(pos.X, pos.Y, PixelFarm.Drawing.Color.Red);
                            if (_flushOutput != null)
                            {
                                //TODO: review here
                                _flushOutput();
                            }
                            _clearInfoView = true;
                        }
                    }
                    break;
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
                case NodeInfoKind.FlattenVertexCommand:
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
                case NodeInfoKind.OrgVertexCommand:
                    {
                        if (RequestGlyphRender != null)
                        {
                            _clearInfoView = false;
                            RequestGlyphRender(this, EventArgs.Empty);
                            //
                            double x, y;
                            _orgVxs.GetVertex(nodeinfo.VertexCommandNo, out x, out y);
                            Owner.DrawMarker((float)x, (float)y, PixelFarm.Drawing.Color.Red);
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
                _tessEdgesNode.Nodes.Clear();
                _edgeLines.Clear();
                _jointsNode.Nodes.Clear();
                _trianglesNode.Nodes.Clear();
                _bonesNode.Nodes.Clear();
            }
            _testEdgeCount = 0;
        }
        public void ShowTriangles(GlyphTriangle tri)
        {
            if (!_clearInfoView) { return; }
            //-----------------------------
            TreeNode triangleNode = new TreeNode() { Text = "tri:" + tri.ToString(), Tag = new NodeInfo(tri) };
            _trianglesNode.Nodes.Add(triangleNode);
        }
        public void ShowBone(GlyphBone bone, GlyphBoneJoint jointA, GlyphBoneJoint jointB)
        {
            if (!_clearInfoView) { return; }
            _treeView.SuspendLayout();
            TreeNode jointNode = new TreeNode() { Text = bone.ToString(), Tag = new NodeInfo(bone, jointA, jointB) };
            _bonesNode.Nodes.Add(jointNode);

            _treeView.ResumeLayout();
        }
        public void ShowBone(GlyphBone bone, GlyphBoneJoint jointA, EdgeLine tipEdge)
        {
            if (!_clearInfoView) { return; }
            _treeView.SuspendLayout();
            TreeNode jointNode = new TreeNode() { Text = bone.ToString(), Tag = new NodeInfo(bone, jointA, tipEdge) };
            _bonesNode.Nodes.Add(jointNode);

            _treeView.ResumeLayout();
        }
        public void ShowJoint(GlyphBoneJoint joint)
        {
            if (!_clearInfoView) { return; }
            //-------------- 
            EdgeLine p_contactEdge = joint._p_contact_edge;
            //mid point
            var jointPos = joint.Position;
            //painter.FillRectLBWH(jointPos.X * pxscale, jointPos.Y * pxscale, 4, 4, PixelFarm.Drawing.Color.Yellow);

            TreeNode jointNode = new TreeNode() { Tag = new NodeInfo(joint) };
            bool added = false;
            switch (joint.SelectedEdgePointCount)
            {
                default: throw new NotSupportedException();
                case 0:
                    //no rib
                    jointNode.Text = "j:" + joint.ToString();
                    _jointsNode.Nodes.Add(jointNode);
                    added = true;
                    break;
                case 1:

                    jointNode.Text = "j:" + joint.ToString();
                    jointNode.Nodes.Add(new TreeNode() { Text = "rib_a:" + joint.RibEndPointA, Tag = new NodeInfo(NodeInfoKind.RibEndPoint, joint.RibEndPointA) });
                    //
                    _jointsNode.Nodes.Add(jointNode);
                    added = true;

                    break;
                case 2:

                    jointNode.Text = "j:" + joint.ToString();
                    jointNode.Nodes.Add(new TreeNode() { Text = "rib_a:" + joint.RibEndPointA, Tag = new NodeInfo(NodeInfoKind.RibEndPoint, joint.RibEndPointA) });
                    jointNode.Nodes.Add(new TreeNode() { Text = "rib_b:" + joint.RibEndPointB, Tag = new NodeInfo(NodeInfoKind.RibEndPoint, joint.RibEndPointB) });
                    //
                    _jointsNode.Nodes.Add(jointNode);
                    added = true;
                    break;
            }
            if (joint.TipPoint != System.Numerics.Vector2.Zero)
            {
                //painter.Line(
                //   jointPos.X * pxscale, jointPos.Y * pxscale,
                //   joint.TipPoint.X * pxscale, joint.TipPoint.Y * pxscale,
                //   PixelFarm.Drawing.Color.White);
                jointNode.Nodes.Add(new TreeNode() { Text = "tip:" + joint.TipPoint, Tag = new NodeInfo(NodeInfoKind.RibEndPoint, joint.TipPoint) });
                if (!added)
                {
                    _jointsNode.Nodes.Add(jointNode);
                    added = true;
                }
            }

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
            var u_data_p = p.userData as GlyphPoint;
            var u_data_q = q.userData as GlyphPoint;

            //-------------------------------

            NodeInfo nodeInfo = new NodeInfo(NodeInfoKind.TessEdge, edge, _edgeLines.Count);
            TreeNode nodeEdge = new TreeNode();
            nodeEdge.Tag = nodeInfo;
            nodeEdge.Text = "e id=" + edge.dbugId + ",count="
                + _testEdgeCount + " :(" + p.X + "," + p.Y + ")" + "=>(" + q.X + "," + q.Y + ") ";
            //if (edge.cutPointOnBone != System.Numerics.Vector2.Zero)
            //{
            //    nodeEdge.Text += " cut:" + edge.cutPointOnBone;
            //}

            _tessEdgesNode.Nodes.Add(nodeEdge);
            //------------------------------- 

            _edgeLines.Add(edge);
        }
        public void ShowFlatternBorderInfo(VertexStore vxs)
        {
            if (!_clearInfoView) { return; }
            _flattenVxsNode.Nodes.Clear();
            _treeView.SuspendLayout();
            _flattenVxs = vxs;
            int count = vxs.Count;
            VertexCmd cmd;
            double x, y;
            int index = 0;
            while ((cmd = vxs.GetVertex(index, out x, out y)) != VertexCmd.NoMore)
            {
                NodeInfo nodeInfo = new NodeInfo(NodeInfoKind.FlattenVertexCommand, index);
                TreeNode node = new TreeNode();
                node.Tag = nodeInfo;
                node.Text = (index) + " " + cmd + ": (" + x + "," + y + ")";
                _flattenVxsNode.Nodes.Add(node);
                index++;
            }
            _treeView.ResumeLayout();
        }
        public void ShowOrgBorderInfo(VertexStore vxs)
        {
            if (!_clearInfoView) { return; }
            _orgVxsNode.Nodes.Clear();
            _treeView.SuspendLayout();
            _orgVxs = vxs;

            int count = vxs.Count;
            VertexCmd cmd;
            double x, y;
            int index = 0;
            while ((cmd = vxs.GetVertex(index, out x, out y)) != VertexCmd.NoMore)
            {
                NodeInfo nodeInfo = new NodeInfo(NodeInfoKind.OrgVertexCommand, index);
                TreeNode node = new TreeNode();
                node.Tag = nodeInfo;
                node.Text = (index) + " " + cmd + ": (" + x + "," + y + ")";
                _orgVxsNode.Nodes.Add(node);
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
            OrgVertexCommand,
            FlattenVertexCommand,
            TessEdge,
            Joint,
            RibEndPoint,
            Tri,
            Bone,
        }
        class NodeInfo
        {
            EdgeLine edge;
            GlyphBoneJoint joint;
            GlyphBone bone;
            System.Numerics.Vector2 pos;
            GlyphTriangle tri;

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
            public NodeInfo(GlyphBoneJoint joint)
            {
                this.joint = joint;
                this.pos = joint.Position;
                this.NodeKind = NodeInfoKind.Joint;
            }
            public NodeInfo(GlyphBone bone, GlyphBoneJoint a, GlyphBoneJoint b)
            {
                this.bone = bone;
                this.NodeKind = NodeInfoKind.Bone;
            }

            public NodeInfo(GlyphBone bone, GlyphBoneJoint a, EdgeLine tipEdge)
            {
                this.bone = bone;
                this.NodeKind = NodeInfoKind.Bone;
            }
            public NodeInfo(GlyphTriangle tri)
            {
                this.tri = tri;
                this.pos = new System.Numerics.Vector2((float)tri.CentroidX, (float)tri.CentroidY);
                this.NodeKind = NodeInfoKind.Tri;
            }
            public NodeInfo(NodeInfoKind nodeKind, System.Numerics.Vector2 pos)
            {
                this.pos = pos;
                this.NodeKind = NodeInfoKind.Joint;
            }
            public int VertexCommandNo { get; set; }
            public NodeInfoKind NodeKind { get; set; }
            public int TessEdgeNo
            {
                get; set;
            }
            public GlyphTriangle GlyphTri { get { return tri; } }
            public GlyphBone Bone { get { return this.bone; } }

            public System.Numerics.Vector2 Pos { get { return pos; } }
        }
    }
}