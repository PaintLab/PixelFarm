//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.CustomWidgets
{
    public class CheckBox : AbstractControlBox
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

            _imageBox = new ImageBox();
            _imageBox.ImageBinder = _isChecked ? s_checkedImg : s_uncheckedImg;
            _imageBox.MouseDown += (s, e) =>
            {
                //toggle checked/unchecked
                this.Checked = !this.Checked;
            };
            
            AddChild(_imageBox);
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
                    //review here,
                    _imageBox.ImageBinder = _isChecked ? s_checkedImg : s_uncheckedImg;
                    this.CheckChanged?.Invoke(this, EventArgs.Empty);                     
                }
            }
        }
        public event EventHandler CheckChanged;

    }
}