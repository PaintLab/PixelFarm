//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.CustomWidgets
{
    public class CheckBox : Box
    {
        //check icon
        ImageBox _imageBox;
        bool _isChecked;

        static Atlas_AUTOGEN_.TestAtlas1.Binders s_binders = new Atlas_AUTOGEN_.TestAtlas1.Binders();
        static PixelFarm.Drawing.ImageBinder s_checkedImg = s_binders._chk_checked_png;
        static PixelFarm.Drawing.ImageBinder s_uncheckedImg = s_binders._chk_unchecked_png;

        public CheckBox(int w, int h)
            : base(w, h)
        {
        }
        void EnsureImgBinders()
        {
             

            //if (s_checkedImg == null)
            //{
            //    s_checkedImg = ResImageList.GetImageBinder(ImageName.CheckBoxChecked);
            //}
            //if (s_uncheckedImg == null)
            //{
            //    s_uncheckedImg = ResImageList.GetImageBinder(ImageName.CheckBoxUnChecked);
            //}
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                //first time
                EnsureImgBinders();

                RenderElement baseRenderElement = base.GetPrimaryRenderElement(rootgfx);
                _imageBox = new ImageBox();
                _imageBox.ImageBinder = _isChecked ? s_checkedImg : s_uncheckedImg;
                _imageBox.MouseDown += (s, e) =>
                {
                    //toggle checked/unchecked
                    this.Checked = !this.Checked;
                };
                this.Add(_imageBox);
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
                    EnsureImgBinders();

                    _isChecked = value;
                    //check check image too!
                    //review here,

                    _imageBox.ImageBinder = _isChecked ? s_checkedImg : s_uncheckedImg;

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