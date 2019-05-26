//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class CheckBox : AbstractBox
    {
        //check icon
        ImageBox _imageBox;
        bool _isChecked;
        public CheckBox(int w, int h)
            : base(w, h)
        {
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                //first time
                RenderElement baseRenderElement = base.GetPrimaryRenderElement(rootgfx);
                _imageBox = new ImageBox(16, 16);
                if (_isChecked)
                {
                    _imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxChecked);
                }
                else
                {
                    _imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxUnChecked);
                }

                _imageBox.MouseDown += (s, e) =>
                {
                    //toggle checked/unchecked
                    this.Checked = !this.Checked;
                };
                this.AddChild(_imageBox);
                return baseRenderElement;
            }
            else
            {
                return base.GetPrimaryRenderElement(rootgfx);
            }
        }
        public bool Checked
        {
            get => _isChecked;
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    //check check image too!

                    if (_isChecked)
                    {
                        _imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxChecked);
                    }
                    else
                    {
                        _imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxUnChecked);
                    }



                    if (value && this.WhenChecked != null)
                    {
                        this.WhenChecked(this, EventArgs.Empty);
                    }
                }
            }
        }
        public event EventHandler WhenChecked;

    }
}