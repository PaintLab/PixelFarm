/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * Copyright (c) 2018-present, Marius Klimantavičius
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

//https://github.com/marius-klimantavicius/yoga

namespace LayoutFarm.MariusYoga
{
    public partial class YogaNode
    {
        public YogaValue Left
        {
            get => _style.Position[YogaEdge.Left];
            set => SetStylePosition(YogaEdge.Left, value);
        }

        public YogaValue Top
        {
            get => _style.Position[YogaEdge.Top];
            set => SetStylePosition(YogaEdge.Top, value);
        }

        public YogaValue Right
        {
            get => _style.Position[YogaEdge.Right];
            set => SetStylePosition(YogaEdge.Right, value);
        }

        public YogaValue Bottom
        {
            get => _style.Position[YogaEdge.Bottom];
            set => SetStylePosition(YogaEdge.Bottom, value);
        }

        public YogaValue Start
        {
            get => _style.Position[YogaEdge.Start];
            set => SetStylePosition(YogaEdge.Start, value);
        }

        public YogaValue End
        {
            get => _style.Position[YogaEdge.End];
            set => SetStylePosition(YogaEdge.End, value);
        }

        public YogaValue MarginLeft
        {
            get => _style.Margin[YogaEdge.Left];
            set => SetStyleMargin(YogaEdge.Left, value);
        }

        public YogaValue MarginTop
        {
            get => _style.Margin[YogaEdge.Top];
            set => SetStyleMargin(YogaEdge.Top, value);
        }

        public YogaValue MarginRight
        {
            get => _style.Margin[YogaEdge.Right];
            set => SetStyleMargin(YogaEdge.Right, value);
        }

        public YogaValue MarginBottom
        {
            get => _style.Margin[YogaEdge.Bottom];
            set => SetStyleMargin(YogaEdge.Bottom, value);
        }

        public YogaValue MarginStart
        {
            get => _style.Margin[YogaEdge.Start];
            set => SetStyleMargin(YogaEdge.Start, value);
        }

        public YogaValue MarginEnd
        {
            get => _style.Margin[YogaEdge.End];
            set => SetStyleMargin(YogaEdge.End, value);
        }

        public YogaValue MarginHorizontal
        {
            get => _style.Margin[YogaEdge.Horizontal];
            set => SetStyleMargin(YogaEdge.Horizontal, value);
        }

        public YogaValue MarginVertical
        {
            get => _style.Margin[YogaEdge.Vertical];
            set => SetStyleMargin(YogaEdge.Vertical, value);
        }

        public YogaValue Margin
        {
            get => _style.Margin[YogaEdge.All];
            set => SetStyleMargin(YogaEdge.All, value);
        }

        public YogaValue PaddingLeft
        {
            get => _style.Padding[YogaEdge.Left];
            set => SetStylePadding(YogaEdge.Left, value);
        }

        public YogaValue PaddingTop
        {
            get => _style.Padding[YogaEdge.Top];
            set => SetStylePadding(YogaEdge.Top, value);
        }

        public YogaValue PaddingRight
        {
            get => _style.Padding[YogaEdge.Right];
            set => SetStylePadding(YogaEdge.Right, value);
        }

        public YogaValue PaddingBottom
        {
            get => _style.Padding[YogaEdge.Bottom];
            set => SetStylePadding(YogaEdge.Bottom, value);
        }

        public YogaValue PaddingStart
        {
            get => _style.Padding[YogaEdge.Start];
            set => SetStylePadding(YogaEdge.Start, value);
        }

        public YogaValue PaddingEnd
        {
            get => _style.Padding[YogaEdge.End];
            set => SetStylePadding(YogaEdge.End, value);
        }

        public YogaValue PaddingHorizontal
        {
            get => _style.Padding[YogaEdge.Horizontal];
            set => SetStylePadding(YogaEdge.Horizontal, value);
        }

        public YogaValue PaddingVertical
        {
            get => _style.Padding[YogaEdge.Vertical];
            set => SetStylePadding(YogaEdge.Vertical, value);
        }

        public YogaValue Padding
        {
            get => _style.Padding[YogaEdge.All];
            set => SetStylePadding(YogaEdge.All, value);
        }

        public float? BorderLeftWidth
        {
            get
            {
                var value = _style.Border[YogaEdge.Left];
                if (value.Unit == YogaUnit.Auto || value.Unit == YogaUnit.Undefined)
                    return null;

                return value.Value;
            }

            set
            {
                var edge = YogaEdge.Left;
                var current = _style.Border[edge];
                var next = new YogaValue() {
                    Unit = value == null ? YogaUnit.Undefined : YogaUnit.Point,
                    Value = value,
                };

                if (current.Unit != next.Unit || (next.Unit != YogaUnit.Undefined && current.Value != next.Value))
                {
                    _style.Border[edge] = next;
                    MarkDirty();
                }
            }
        }

        public float? BorderTopWidth
        {
            get
            {
                var value = _style.Border[YogaEdge.Top];
                if (value.Unit == YogaUnit.Auto || value.Unit == YogaUnit.Undefined)
                    return null;

                return value.Value;
            }

            set
            {
                var edge = YogaEdge.Top;
                var current = _style.Border[edge];
                var next = new YogaValue() {
                    Unit = value == null ? YogaUnit.Undefined : YogaUnit.Point,
                    Value = value,
                };

                if (current.Unit != next.Unit || (next.Unit != YogaUnit.Undefined && current.Value != next.Value))
                {
                    _style.Border[edge] = next;
                    MarkDirty();
                }
            }
        }

        public float? BorderRightWidth
        {
            get
            {
                var value = _style.Border[YogaEdge.Right];
                if (value.Unit == YogaUnit.Auto || value.Unit == YogaUnit.Undefined)
                    return null;

                return value.Value;
            }

            set
            {
                var edge = YogaEdge.Right;
                var current = _style.Border[edge];
                var next = new YogaValue() {
                    Unit = value == null ? YogaUnit.Undefined : YogaUnit.Point,
                    Value = value,
                };

                if (current.Unit != next.Unit || (next.Unit != YogaUnit.Undefined && current.Value != next.Value))
                {
                    _style.Border[edge] = next;
                    MarkDirty();
                }
            }
        }

        public float? BorderBottomWidth
        {
            get
            {
                var value = _style.Border[YogaEdge.Bottom];
                if (value.Unit == YogaUnit.Auto || value.Unit == YogaUnit.Undefined)
                    return null;

                return value.Value;
            }

            set
            {
                var edge = YogaEdge.Bottom;
                var current = _style.Border[edge];
                var next = new YogaValue() {
                    Unit = value == null ? YogaUnit.Undefined : YogaUnit.Point,
                    Value = value,
                };

                if (current.Unit != next.Unit || (next.Unit != YogaUnit.Undefined && current.Value != next.Value))
                {
                    _style.Border[edge] = next;
                    MarkDirty();
                }
            }
        }

        public float? BorderStartWidth
        {
            get
            {
                var value = _style.Border[YogaEdge.Start];
                if (value.Unit == YogaUnit.Auto || value.Unit == YogaUnit.Undefined)
                    return null;

                return value.Value;
            }

            set
            {
                var edge = YogaEdge.Start;
                var current = _style.Border[edge];
                var next = new YogaValue() {
                    Unit = value == null ? YogaUnit.Undefined : YogaUnit.Point,
                    Value = value,
                };

                if (current.Unit != next.Unit || (next.Unit != YogaUnit.Undefined && current.Value != next.Value))
                {
                    _style.Border[edge] = next;
                    MarkDirty();
                }
            }
        }

        public float? BorderEndWidth
        {
            get
            {
                var value = _style.Border[YogaEdge.End];
                if (value.Unit == YogaUnit.Auto || value.Unit == YogaUnit.Undefined)
                    return null;

                return value.Value;
            }

            set
            {
                var edge = YogaEdge.End;
                var current = _style.Border[edge];
                var next = new YogaValue() {
                    Unit = value == null ? YogaUnit.Undefined : YogaUnit.Point,
                    Value = value,
                };

                if (current.Unit != next.Unit || (next.Unit != YogaUnit.Undefined && current.Value != next.Value))
                {
                    _style.Border[edge] = next;
                    MarkDirty();
                }
            }
        }

        public float? BorderWidth
        {
            get
            {
                var value = _style.Border[YogaEdge.All];
                if (value.Unit == YogaUnit.Auto || value.Unit == YogaUnit.Undefined)
                    return null;

                return value.Value;
            }

            set
            {
                var edge = YogaEdge.All;
                var current = _style.Border[edge];
                var next = new YogaValue() {
                    Unit = value == null ? YogaUnit.Undefined : YogaUnit.Point,
                    Value = value,
                };

                if (current.Unit != next.Unit || (next.Unit != YogaUnit.Undefined && current.Value != next.Value))
                {
                    _style.Border[edge] = next;
                    MarkDirty();
                }
            }
        }

        public float? LayoutMarginLeft => GetLayoutMargin(YogaEdge.Left);

        public float? LayoutMarginTop => GetLayoutMargin(YogaEdge.Top);

        public float? LayoutMarginRight => GetLayoutMargin(YogaEdge.Right);

        public float? LayoutMarginBottom => GetLayoutMargin(YogaEdge.Bottom);

        public float? LayoutMarginStart => GetLayoutMargin(YogaEdge.Start);

        public float? LayoutMarginEnd => GetLayoutMargin(YogaEdge.End);

        public float LayoutPaddingLeft => GetLayoutPadding(YogaEdge.Left);

        public float LayoutPaddingTop => GetLayoutPadding(YogaEdge.Top);

        public float LayoutPaddingRight => GetLayoutPadding(YogaEdge.Right);

        public float LayoutPaddingBottom => GetLayoutPadding(YogaEdge.Bottom);

        public float LayoutPaddingStart => GetLayoutPadding(YogaEdge.Start);

        public float LayoutPaddingEnd => GetLayoutPadding(YogaEdge.End);


        private void SetStylePosition(YogaEdge edge, YogaValue value)
        {
            var current = _style.Position[edge];
            if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
            {
                _style.Position[edge] = value;
                MarkDirty();
            }
        }

        private void SetStyleMargin(YogaEdge edge, YogaValue value)
        {
            var current = _style.Margin[edge];
            if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
            {
                _style.Margin[edge] = value;
                MarkDirty();
            }
        }

        private void SetStylePadding(YogaEdge edge, YogaValue value)
        {
            var current = _style.Padding[edge];
            if (current.Unit != value.Unit || (value.Unit != YogaUnit.Undefined && current.Value != value.Value))
            {
                _style.Padding[edge] = value;
                MarkDirty();
            }
        }

        public float? GetLayoutMargin(YogaEdge edge)
        {
            if (edge == YogaEdge.Left)
            {
                if (_layout.Direction == YogaDirection.RightToLeft)
                    return _layout.Margin[YogaEdge.End];
                return _layout.Margin[YogaEdge.Start];
            }

            if (edge == YogaEdge.Right)
            {
                if (_layout.Direction == YogaDirection.RightToLeft)
                    return _layout.Margin[YogaEdge.Start];
                return _layout.Margin[YogaEdge.End];
            }

            return _layout.Margin[edge];
        }

        public float GetLayoutPadding(YogaEdge edge)
        {
            if (edge == YogaEdge.Left)
            {
                if (_layout.Direction == YogaDirection.RightToLeft)
                    return _layout.Padding[YogaEdge.End];
                return _layout.Padding[YogaEdge.Start];
            }

            if (edge == YogaEdge.Right)
            {
                if (_layout.Direction == YogaDirection.RightToLeft)
                    return _layout.Padding[YogaEdge.Start];
                return _layout.Padding[YogaEdge.End];
            }

            return _layout.Padding[edge];
        }
    }
}
