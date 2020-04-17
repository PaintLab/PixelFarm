/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * Copyright (c) 2018-present, Marius Klimantavičius
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
//https://github.com/marius-klimantavicius/yoga

using System;
using System.Collections.Generic;
using System.Threading;

namespace LayoutFarm.MariusYoga
{
    public partial class YogaNode : IEnumerable<YogaNode>
    {
        public const float DEFAULT_FLEX_GLOW = 0.0f;
        public const float DEFAULT_FLEX_SHRINK = 0.0f;
        public const float WEB_DEFAULT_FLEX_SHRINK = 1.0f;

#if DEBUG
        private static int s_dbuginstanceCount = 0;
#endif
        private YogaMeasure _measure;
        private bool _isReferenceBaseline;
        private YogaStyle _style;
        private YogaLayout _layout;
        private List<YogaNode> _children;
        private bool _isDirty;

        public YogaNode()
        {
            PrintFunction = null;
            HasNewLayout = true;
            NodeType = YogaNodeType.Default;
            _measure = null;
            Baseline = null;
            Dirtied = null;
            _style = new YogaStyle();
            _layout = new YogaLayout();
            LineIndex = 0;
            Owner = null;
            _children = new List<YogaNode>();
            NextChild = null;
            Config = new YogaConfig();
            _isDirty = false;
            ResolvedDimensions = new YogaArray<YogaValue>(new YogaValue[] { YogaValue.Undefined, YogaValue.Undefined });
#if DEBUG
            Interlocked.Increment(ref s_dbuginstanceCount);
#endif
        }

        public YogaNode(YogaNode node)
        {
            PrintFunction = node.PrintFunction;
            HasNewLayout = node.HasNewLayout;
            NodeType = node.NodeType;
            _measure = node._measure;
            Baseline = node.Baseline;
            Dirtied = node.Dirtied;
            _style = node._style;
            _layout = node._layout;
            LineIndex = node.LineIndex;
            Owner = node.Owner;
            _children = new List<YogaNode>(node._children);
            NextChild = node.NextChild;
            Config = node.Config;
            _isDirty = node._isDirty;
            ResolvedDimensions = YogaArray.From(node.ResolvedDimensions);

#if DEBUG
            Interlocked.Increment(ref s_dbuginstanceCount);
#endif
        }

        public YogaNode(YogaNode node, YogaNode owner)
            : this(node)
        {
            Owner = owner;
        }

        public YogaNode(
            YogaPrint print,
            bool hasNewLayout,
            YogaNodeType nodeType,
            YogaMeasure measure,
            YogaBaseline baseline,
            YogaDirtied dirtied,
            YogaStyle style,
            YogaLayout layout,
            int lineIndex,
            YogaNode owner,
            List<YogaNode> children,
            YogaNode nextChild,
            YogaConfig config,
            bool isDirty,
            YogaValue[] resolvedDimensions)
        {
            PrintFunction = print;
            HasNewLayout = hasNewLayout;
            NodeType = nodeType;
            _measure = measure;
            Baseline = baseline;
            Dirtied = dirtied;
            _style = style;
            _layout = layout;
            LineIndex = lineIndex;
            Owner = owner;
            _children = new List<YogaNode>(children);
            NextChild = nextChild;
            Config = config;
            _isDirty = isDirty;
            ResolvedDimensions = YogaArray.From(resolvedDimensions);

#if DEBUG
            Interlocked.Increment(ref s_dbuginstanceCount);
#endif
        }

        public YogaNode(YogaConfig config) : this()
        {
            Config = config ?? new YogaConfig();

            if (Config.UseWebDefaults)
            {
                Style.FlexDirection = YogaFlexDirection.Row;
                Style.AlignContent = YogaAlign.Stretch;
            }
        }
#if DEBUG
        ~YogaNode()
        {
            //TODO: review here
            Interlocked.Decrement(ref s_dbuginstanceCount);
        }


        public static int dbugGetInstanceCount()
        {
            return s_dbuginstanceCount;
        }
#endif

        public string Note { get; set; }

        public YogaPrint PrintFunction { get; set; }

        public bool HasNewLayout { get; set; }

        public YogaNodeType NodeType { get; set; }

        public YogaMeasure Measure
        {
            get => _measure;
            set
            {
                if (_children.Count > 0)
                    throw new InvalidOperationException("Cannot set measure function: Nodes with measure functions cannot have children.");

                if (value == null)
                {
                    _measure = null;
                    // TODO: t18095186 Move nodeType to opt-in function and mark appropriate
                    // places in Litho
                    NodeType = YogaNodeType.Default;
                }
                else
                {
                    _measure = value;
                    // TODO: t18095186 Move nodeType to opt-in function and mark appropriate
                    // places in Litho
                    NodeType = YogaNodeType.Text;
                }

                _measure = value;
            }
        }

        public YogaBaseline Baseline { get; set; }

        public YogaDirtied Dirtied { get; set; }

        public YogaStyle Style
        {
            get => _style;
            set => _style.CopyFrom(value);
        }

        public YogaLayout Layout
        {
            get => _layout;
            set => _layout.CopyFrom(value);
        }

        public int LineIndex { get; set; }

        public YogaNode Owner { get; set; }

        public YogaNode Parent => Owner;

        public List<YogaNode> Children
        {
            get => _children;
            set { _children = (value != null) ? new List<YogaNode>(value) : new List<YogaNode>(); }
        }

        public YogaNode GetChild(int index) => _children[index];

        public YogaNode NextChild { get; set; }

        public YogaConfig Config { get; set; }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (value == _isDirty)
                    return;

                _isDirty = value;
                if (value && Dirtied != null)
                    Dirtied(this);
            }
        }

        public YogaArray<YogaValue> ResolvedDimensions { get; private set; }

        public YogaValue GetResolvedDimension(YogaDimension index) => ResolvedDimensions[index];

        // Methods related to positions, margin, padding and border
        public float? GetLeadingPosition(YogaFlexDirection axis, float? axisSize)
        {
            var leadingPosition = default(YogaValue);
            if (axis.IsRow())
            {
                leadingPosition = ComputedEdgeValue(_style.Position, YogaEdge.Start, YogaValue.Undefined);
                if (leadingPosition.Unit != YogaUnit.Undefined)
                    return leadingPosition.Resolve(axisSize);
            }

            leadingPosition = ComputedEdgeValue(_style.Position, Leading[axis], YogaValue.Undefined);
            return leadingPosition.Unit == YogaUnit.Undefined ? 0.0f : leadingPosition.Resolve(axisSize);
        }

        public float? GetTrailingPosition(YogaFlexDirection axis, float? axisSize)
        {
            var trailingPosition = default(YogaValue);
            if (axis.IsRow())
            {
                trailingPosition = ComputedEdgeValue(_style.Position, YogaEdge.End, YogaValue.Undefined);
                if (trailingPosition.Unit != YogaUnit.Undefined)
                    return trailingPosition.Resolve(axisSize);
            }

            trailingPosition = ComputedEdgeValue(_style.Position, Trailing[axis], YogaValue.Undefined);
            return trailingPosition.Unit == YogaUnit.Undefined ? 0.0f : trailingPosition.Resolve(axisSize);
        }

        public float? GetRelativePosition(YogaFlexDirection axis, float? axisSize)
        {
            return IsLeadingPositionDefined(axis) ? GetLeadingPosition(axis, axisSize) : -GetTrailingPosition(axis, axisSize);
        }

        public bool IsLeadingPositionDefined(YogaFlexDirection axis)
        {
            return (axis.IsRow() && ComputedEdgeValue(_style.Position, YogaEdge.Start, YogaValue.Undefined).Unit != YogaUnit.Undefined)
                || ComputedEdgeValue(_style.Position, Leading[axis], YogaValue.Undefined).Unit != YogaUnit.Undefined;
        }

        public bool IsTrailingPositionDefined(YogaFlexDirection axis)
        {
            return (axis.IsRow() && ComputedEdgeValue(_style.Position, YogaEdge.End, YogaValue.Undefined).Unit != YogaUnit.Undefined)
                || ComputedEdgeValue(_style.Position, Trailing[axis], YogaValue.Undefined).Unit != YogaUnit.Undefined;
        }

        public float? GetLeadingMargin(YogaFlexDirection axis, float? widthSize)
        {
            if (axis.IsRow() && _style.Margin[YogaEdge.Start].Unit != YogaUnit.Undefined)
                return ResolveValueMargin(_style.Margin[YogaEdge.Start], widthSize);

            return ResolveValueMargin(ComputedEdgeValue(_style.Margin, Leading[axis], YogaValue.Zero), widthSize);
        }

        public float? GetTrailingMargin(YogaFlexDirection axis, float? widthSize)
        {
            if (axis.IsRow() && _style.Margin[YogaEdge.End].Unit != YogaUnit.Undefined)
                return ResolveValueMargin(_style.Margin[YogaEdge.End], widthSize);

            return ResolveValueMargin(ComputedEdgeValue(_style.Margin, Trailing[axis], YogaValue.Zero), widthSize);
        }

        public float? GetMarginForAxis(YogaFlexDirection axis, float? widthSize)
        {
            return GetLeadingMargin(axis, widthSize) + GetTrailingMargin(axis, widthSize);
        }

        public YogaValue GetMarginLeadingValue(YogaFlexDirection axis)
        {
            if (axis.IsRow() && _style.Margin[YogaEdge.Start].Unit != YogaUnit.Undefined)
                return _style.Margin[YogaEdge.Start];

            return _style.Margin[Leading[axis]];
        }

        public YogaValue GetMarginTrailingValue(YogaFlexDirection axis)
        {
            if (axis.IsRow() && _style.Margin[YogaEdge.End].Unit != YogaUnit.Undefined)
                return _style.Margin[YogaEdge.End];

            return _style.Margin[Trailing[axis]];
        }

        public float GetLeadingBorder(YogaFlexDirection axis)
        {
            if (axis.IsRow()
                && _style.Border[YogaEdge.Start].Unit != YogaUnit.Undefined
                && _style.Border[YogaEdge.Start].Value != null
                && _style.Border[YogaEdge.Start].Value >= 0.0f)
            {
                return _style.Border[YogaEdge.Start].Value.Value;
            }

            var computedEdgeValue = ComputedEdgeValue(_style.Border, Leading[axis], YogaValue.Zero).Value;
            return YogaMath.Max(computedEdgeValue, 0.0F);
        }

        public float GetTrailingBorder(YogaFlexDirection flexDirection)
        {
            if (flexDirection.IsRow()
                && _style.Border[YogaEdge.End].Unit != YogaUnit.Undefined
                && _style.Border[YogaEdge.End].Value != null
                && _style.Border[YogaEdge.End].Value >= 0.0f)
            {
                return _style.Border[YogaEdge.End].Value.Value;
            }

            var computedEdgeValue = ComputedEdgeValue(_style.Border, Trailing[flexDirection], YogaValue.Zero).Value;
            return YogaMath.Max(computedEdgeValue, 0.0f);
        }

        public float GetLeadingPadding(YogaFlexDirection axis, float? widthSize)
        {
            var paddingEdgeStart = _style.Padding[YogaEdge.Start].Resolve(widthSize);
            if (axis.IsRow()
                && _style.Padding[YogaEdge.Start].Unit != YogaUnit.Undefined
                && paddingEdgeStart != null
                && paddingEdgeStart >= 0.0f)
            {
                return paddingEdgeStart.Value;
            }

            var resolvedValue = ComputedEdgeValue(_style.Padding, Leading[axis], YogaValue.Zero).Resolve(widthSize);
            return YogaMath.Max(resolvedValue, 0.0f);
        }

        public float GetTrailingPadding(YogaFlexDirection axis, float? widthSize)
        {
            var paddingEdgeEnd = _style.Padding[YogaEdge.End].Resolve(widthSize);
            if (axis.IsRow()
                && _style.Padding[YogaEdge.End].Unit != YogaUnit.Undefined
                && paddingEdgeEnd != null
                && paddingEdgeEnd >= 0.0f)
            {
                return paddingEdgeEnd.Value;
            }

            var resolvedValue = ComputedEdgeValue(_style.Padding, Trailing[axis], YogaValue.Zero).Resolve(widthSize);
            return YogaMath.Max(resolvedValue, 0.0f);
        }

        public float GetLeadingPaddingAndBorder(YogaFlexDirection axis, float? widthSize)
        {
            return GetLeadingPadding(axis, widthSize) + GetLeadingBorder(axis);
        }

        public float GetTrailingPaddingAndBorder(YogaFlexDirection axis, float? widthSize)
        {
            return GetTrailingPadding(axis, widthSize) + GetTrailingBorder(axis);
        }

        public float ResolveFlexGrow()
        {
            // Root nodes flexGrow should always be 0
            if (Owner == null)
                return 0.0f;

            if (_style.FlexGrow != null)
                return _style.FlexGrow.Value;

            if (_style.Flex != null && _style.Flex.Value > 0.0f)
                return _style.Flex.Value;

            return DEFAULT_FLEX_GLOW;
        }

        public float ResolveFlexShrink()
        {
            if (Owner == null)
                return 0.0f;

            if (_style.FlexShrink != null)
                return _style.FlexShrink.Value;

            if (!Config.UseWebDefaults && _style.Flex != null && _style.Flex.Value < 0.0f)
                return -_style.Flex.Value;

            return Config.UseWebDefaults ? WEB_DEFAULT_FLEX_SHRINK : DEFAULT_FLEX_SHRINK;
        }

        public YogaValue ResolveFlexBasis()
        {
            var flexBasis = _style.FlexBasis;
            if (flexBasis.Unit != YogaUnit.Auto && flexBasis.Unit != YogaUnit.Undefined)
                return flexBasis;

            if (_style.Flex != null && _style.Flex.Value > 0.0f)
                return Config.UseWebDefaults ? YogaValue.Auto : YogaValue.Zero;

            return YogaValue.Auto;
        }

        public void ResolveDimension()
        {
            for (var dim = (int)YogaDimension.Width; dim < 2; dim++)
            {
                if (_style.MaxDimensions[dim].Unit != YogaUnit.Undefined && _style.MaxDimensions[dim].Equals(_style.MinDimensions[dim]))
                    ResolvedDimensions[dim] = _style.MaxDimensions[dim];
                else
                    ResolvedDimensions[dim] = _style.Dimensions[dim];
            }
        }

        public YogaDirection ResolveDirection(YogaDirection ownerDirection)
        {
            if (_style.Direction == YogaDirection.Inherit)
                return ownerDirection > YogaDirection.Inherit ? ownerDirection : YogaDirection.LeftToRight;

            return _style.Direction;
        }

        public bool IsNodeFlexible()
        {
            return ((_style.PositionType == YogaPositionType.Relative) && (ResolveFlexGrow() != 0 || ResolveFlexShrink() != 0));
        }

        public bool DidUseLegacyFlag()
        {
            var didUseLegacyFlag = _layout.DidUseLegacyFlag;
            if (didUseLegacyFlag)
                return true;

            foreach (var child in _children)
            {
                if (child._layout.DidUseLegacyFlag)
                    return true;
            }

            return false;
        }

        // setters
        public void SetLayoutMargin(float? margin, YogaEdge index)
        {
            _layout.Margin[index] = margin;
        }

        public void SetLayoutBorder(float border, YogaEdge index)
        {
            _layout.Border[index] = border;
        }

        public void SetLayoutPadding(float padding, YogaEdge index)
        {
            _layout.Padding[index] = padding;
        }

        public void SetLayoutPosition(float? position, YogaEdge index)
        {
            _layout.Position[(int)index] = position;
        }

        public void SetLayoutMeasuredDimension(float? measuredDimension, YogaDimension index)
        {
            _layout.MeasuredDimensions[index] = measuredDimension;
        }

        public void SetLayoutDimension(float? dimension, YogaDimension index)
        {
            _layout.Dimensions[index] = dimension;
        }

        public void SetPosition(YogaDirection direction, float? mainSize, float? crossSize, float? ownerWidth)
        {
            /* Root nodes should be always layouted as LTR, so we don't return negative
             * values. */
            var directionRespectingRoot = Owner != null ? direction : YogaDirection.LeftToRight;
            var mainAxis = _style.FlexDirection.ResolveFlexDirection(directionRespectingRoot);
            var crossAxis = mainAxis.FlexDirectionCross(directionRespectingRoot);

            var relativePositionMain = GetRelativePosition(mainAxis, mainSize);
            var relativePositionCross = GetRelativePosition(crossAxis, crossSize);

            SetLayoutPosition((GetLeadingMargin(mainAxis, ownerWidth) + relativePositionMain), Leading[mainAxis]);
            SetLayoutPosition((GetTrailingMargin(mainAxis, ownerWidth) + relativePositionMain), Trailing[mainAxis]);
            SetLayoutPosition((GetLeadingMargin(crossAxis, ownerWidth) + relativePositionCross), Leading[crossAxis]);
            SetLayoutPosition((GetTrailingMargin(crossAxis, ownerWidth) + relativePositionCross), Trailing[crossAxis]);
        }

        public void SetAndPropogateUseLegacyFlag(bool useLegacyFlag)
        {
            Config.UseLegacyStretchBehaviour = useLegacyFlag;
            foreach (var item in _children)
                item.Config.UseLegacyStretchBehaviour = useLegacyFlag;
        }

        // Other methods

        public int IndexOf(YogaNode node)
        {
            return _children.IndexOf(node);
        }

        public void Clear()
        {
            foreach (var item in _children)
                item.Owner = null;

            _children.Clear();
            _isDirty = true;
        }

        public void ReplaceChild(YogaNode oldChild, YogaNode newChild)
        {
            var index = _children.IndexOf(oldChild);
            if (index < 0)
                return;

            newChild.Owner = this;
            _children[index] = newChild;

            MarkDirty();
        }

        public void ReplaceChild(YogaNode child, int index)
        {
            child.Owner = this;
            _children[index] = child;

            MarkDirty();
        }

        public YogaNode Insert(int index, YogaNode child)
        {
            if (child.Owner != null)
                throw new InvalidOperationException("Child already has a owner, it must be removed first.");

            if (_measure != null)
                throw new InvalidOperationException("Cannot add child: Nodes with measure functions cannot have children.");

            child.Owner = this;
            _children.Insert(index, child);

            MarkDirty();

            return child;
        }

        public bool Remove(YogaNode child)
        {
            if (child.Owner == this)
                child.Owner = null;

            var result = _children.Remove(child);
            if (result)
                MarkDirty();

            return result;
        }

        public void RemoveAt(int index)
        {
            var child = _children[index];
            child.Owner = null;
            _children.RemoveAt(index);

            MarkDirty();
        }

        public void MarkDirty()
        {
            if (!_isDirty)
            {
                IsDirty = true;

                Layout.ComputedFlexBasis = default(float?);
                if (Owner != null)
                    Owner.MarkDirty();
            }
        }

        public void MarkDirtyAndPropogateDownwards()
        {
            _isDirty = true;
            foreach (var item in _children)
                item.MarkDirtyAndPropogateDownwards();
        }

        public bool IsLayoutTreeEqualToNode(YogaNode node)
        {
            if (_children.Count != node._children.Count)
                return false;

            if (_layout != node._layout)
                return false;

            if (_children.Count == 0)
                return true;

            var isLayoutTreeEqual = true;
            YogaNode otherNodeChildren;
            for (var i = 0; i < _children.Count; ++i)
            {
                otherNodeChildren = node._children[i];
                isLayoutTreeEqual = _children[i].IsLayoutTreeEqualToNode(otherNodeChildren);
                if (!isLayoutTreeEqual)
                    return false;
            }

            return isLayoutTreeEqual;
        }

        private void SetChildTrailingPosition(YogaNode child, YogaFlexDirection axis)
        {
            var size = child.Layout.MeasuredDimensions[Dimension[axis]];
            child.SetLayoutPosition(Layout.MeasuredDimensions[Dimension[axis]] - size - child.Layout.Position[Position[axis]], Trailing[axis]);
        }

        private void CloneChildrenIfNeeded()
        {
            var childCount = _children.Count;
            if (childCount == 0)
                return;

            var firstChild = _children[0];
            if (firstChild.Owner == this)
            {
                // If the first child has this node as its owner, we assume that it is
                // already unique. We can do this because if we have it as a child, that
                // means that its owner was at some point cloned which made that subtree
                // immutable. We also assume that all its sibling are cloned as well.
                return;
            }

            var cloneNodeCallback = Config.OnNodeCloned;
            for (var i = 0; i < childCount; ++i)
            {
                var oldChild = _children[i];
                var newChild = new YogaNode(oldChild)
                {
                    Owner = null
                };

                ReplaceChild(newChild, i);
                newChild.Owner = this;
                cloneNodeCallback?.Invoke(oldChild, newChild, this, i);
            }
        }

        private static float? ResolveValueMargin(YogaValue value, float? ownerSize)
        {
            return value.Unit == YogaUnit.Auto ? 0F : value.Resolve(ownerSize);
        }

        private static YogaValue ComputedEdgeValue(YogaArray<YogaValue> edges, YogaEdge edge, YogaValue defaultValue)
        {
            if (edges[edge].Unit != YogaUnit.Undefined)
                return edges[edge];

            if ((edge == YogaEdge.Top || edge == YogaEdge.Bottom) && edges[YogaEdge.Vertical].Unit != YogaUnit.Undefined)
                return edges[YogaEdge.Vertical];

            if ((edge == YogaEdge.Left || edge == YogaEdge.Right || edge == YogaEdge.Start || edge == YogaEdge.End) && edges[YogaEdge.Horizontal].Unit != YogaUnit.Undefined)
                return edges[YogaEdge.Horizontal];

            if (edges[YogaEdge.All].Unit != YogaUnit.Undefined)
                return edges[YogaEdge.All];

            if (edge == YogaEdge.Start || edge == YogaEdge.End)
                return YogaValue.Undefined;

            return defaultValue;
        }
    }
}
