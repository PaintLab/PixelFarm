/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * Copyright (c) 2018-present, Marius Klimantavičius
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
//https://github.com/marius-klimantavicius/yoga

using System;
using System.Collections;
using System.Collections.Generic;


namespace LayoutFarm.MariusYoga
{
    public partial class YogaNode
    {
        public bool IsMeasureDefined => _measure != null;

        public bool IsBaselineDefined => Baseline != null;

        public bool IsReferenceBaseline
        {
            get => _isReferenceBaseline;
            set
            {
                if (_isReferenceBaseline != value)
                {
                    _isReferenceBaseline = value;
                    MarkDirty();
                }
            }
        }

        public void CopyStyle(YogaNode srcNode)
        {
            _style.CopyFrom(srcNode.Style);
        }

        public YogaDirection StyleDirection
        {
            get => _style.Direction;
            set
            {
                if (_style.Direction != value)
                {
                    _style.Direction = value;
                    MarkDirty();
                }
            }
        }

        public YogaFlexDirection FlexDirection
        {
            get => _style.FlexDirection;
            set
            {
                if (_style.FlexDirection != value)
                {
                    _style.FlexDirection = value;
                    MarkDirty();
                }
            }
        }

        public YogaJustify JustifyContent
        {
            get => _style.JustifyContent;
            set
            {
                if (_style.JustifyContent != value)
                {
                    _style.JustifyContent = value;
                    MarkDirty();
                }
            }
        }

        public YogaDisplay Display
        {
            get => _style.Display;
            set
            {
                if (_style.Display != value)
                {
                    _style.Display = value;
                    MarkDirty();
                }
            }
        }

        public YogaAlign AlignItems
        {
            get => _style.AlignItems;
            set
            {
                if (_style.AlignItems != value)
                {
                    _style.AlignItems = value;
                    MarkDirty();
                }
            }
        }

        public YogaAlign AlignSelf
        {
            get => _style.AlignSelf;
            set
            {
                if (_style.AlignSelf != value)
                {
                    _style.AlignSelf = value;
                    MarkDirty();
                }
            }
        }

        public YogaAlign AlignContent
        {
            get => _style.AlignContent;
            set
            {
                if (_style.AlignContent != value)
                {
                    _style.AlignContent = value;
                    MarkDirty();
                }
            }
        }

        public YogaPositionType PositionType
        {
            get => _style.PositionType;
            set
            {
                if (_style.PositionType != value)
                {
                    _style.PositionType = value;
                    MarkDirty();
                }
            }
        }

        public YogaWrap Wrap
        {
            get => _style.FlexWrap;
            set
            {
                if (_style.FlexWrap != value)
                {
                    _style.FlexWrap = value;
                    MarkDirty();
                }
            }
        }

        public float? Flex
        {
            set
            {
                if (value == float.NaN)
                    value = null;

                if (_style.Flex != value)
                {
                    _style.Flex = value;
                    MarkDirty();
                }
            }
        }

        public float? FlexGrow
        {
            get => _style.FlexGrow;
            set
            {
                if (value == float.NaN)
                    value = null;

                if (_style.FlexGrow != value)
                {
                    _style.FlexGrow = value;
                    MarkDirty();
                }
            }
        }

        public float? FlexShrink
        {
            get => _style.FlexShrink;
            set
            {
                if (value == float.NaN)
                    value = null;

                if (_style.FlexShrink != value)
                {
                    _style.FlexShrink = value;
                    MarkDirty();
                }
            }
        }

        public YogaValue FlexBasis
        {
            get => _style.FlexBasis;
            set
            {
                var current = _style.FlexBasis;
                if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
                {
                    _style.FlexBasis = value;
                    MarkDirty();
                }
            }
        }

        public YogaValue Width
        {
            get => _style.Dimensions[YogaDimension.Width];
            set
            {
                var current = _style.Dimensions[YogaDimension.Width];
                if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
                {
                    _style.Dimensions[YogaDimension.Width] = value;
                    MarkDirty();
                }
            }
        }

        public YogaValue Height
        {
            get
            {
                return _style.Dimensions[YogaDimension.Height];
            }

            set
            {
                var current = _style.Dimensions[YogaDimension.Height];
                if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
                {
                    _style.Dimensions[YogaDimension.Height] = value;
                    MarkDirty();
                }
            }
        }

        public YogaValue MaxWidth
        {
            get => _style.MaxDimensions[YogaDimension.Width];
            set
            {
                var current = _style.MaxDimensions[YogaDimension.Width];
                if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
                {
                    _style.MaxDimensions[YogaDimension.Width] = value;
                    MarkDirty();
                }
            }
        }

        public YogaValue MaxHeight
        {
            get => _style.MaxDimensions[YogaDimension.Height];
            set
            {
                var current = _style.MaxDimensions[YogaDimension.Height];
                if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
                {
                    _style.MaxDimensions[YogaDimension.Height] = value;
                    MarkDirty();
                }
            }
        }

        public YogaValue MinWidth
        {
            get => _style.MinDimensions[YogaDimension.Width];
            set
            {
                var current = _style.MinDimensions[YogaDimension.Width];
                if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
                {
                    _style.MinDimensions[YogaDimension.Width] = value;
                    MarkDirty();
                }
            }
        }

        public YogaValue MinHeight
        {
            get => _style.MinDimensions[YogaDimension.Height];
            set
            {
                var current = _style.MinDimensions[YogaDimension.Height];
                if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
                {
                    _style.MinDimensions[YogaDimension.Height] = value;
                    MarkDirty();
                }
            }
        }

        public float? AspectRatio
        {
            get => _style.AspectRatio;
            set
            {
                if (_style.AspectRatio != value)
                {
                    _style.AspectRatio = value;
                    MarkDirty();
                }
            }
        }

        public float LayoutX => _layout.Position[YogaEdge.Left] ?? 0;

        public float LayoutY => _layout.Position[YogaEdge.Top] ?? 0;

        public float LayoutWidth => _layout.Dimensions[YogaDimension.Width] ?? 0;

        public float LayoutHeight => _layout.Dimensions[YogaDimension.Height] ?? 0;

        public YogaDirection LayoutDirection => _layout.Direction;

        public YogaOverflow Overflow
        {
            get => _style.Overflow;
            set
            {
                if (_style.Overflow != value)
                {
                    _style.Overflow = value;
                    MarkDirty();
                }
            }
        }

        public object Data { get; set; }

        public YogaNode this[int index] => _children[index];

        public int Count => _children != null ? _children.Count : 0;

        public void MarkLayoutSeen() => HasNewLayout = false;

        public void SetMeasureFunction(YogaMeasure measure) => Measure = measure;

        public void SetBaselineFunction(YogaBaseline baseline) => Baseline = baseline;

        public void Reset()
        {
            if (_children.Count > 0)
                throw new InvalidOperationException("Cannot reset a node which still has children attached");

            if (Owner != null)
                throw new InvalidOperationException("Cannot reset a node still attached to a owner");

            Clear();

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
            _isDirty = false;
            ResolvedDimensions = new YogaArray<YogaValue>(new YogaValue[] { YogaValue.Undefined, YogaValue.Undefined });

            if (Config.UseWebDefaults)
            {
                Style.FlexDirection = YogaFlexDirection.Row;
                Style.AlignContent = YogaAlign.Stretch;
            }
        }

        public IEnumerator<YogaNode> GetEnumerator() => _children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
