//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{

    class PlatformWinBoxForm : IPlatformWindowBox
    {
        AbstractCompletionWindow _form;
        bool _evalLocationRelativeToDesktop;
        System.Drawing.Point _locationRelToDesktop;
        int _formLocalX;
        int _formLocalY;

        public event EventHandler VisibityChanged;
        public event EventHandler BoundsChanged;
        public event EventHandler PreviewBoundChanged;
        public event EventHandler PreviewVisibilityChanged;

        public PlatformWinBoxForm(AbstractCompletionWindow form)
        {

            _form = form;
            _form.Move += (s, e) => _evalLocationRelativeToDesktop = false;

        }
        public bool UseRelativeLocationToParent { get; set; }

        public bool Visible
        {
            get => _form.Visible;
            set
            {
                if (value)
                {
                    if (!_form.Visible)
                    {
                        PreviewVisibilityChanged?.Invoke(this, EventArgs.Empty);
                        _form.ShowForm();
                        VisibityChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (_form.Visible)
                    {
                        PreviewVisibilityChanged?.Invoke(this, EventArgs.Empty);
                        _form.Hide();
                        VisibityChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }
        void IPlatformWindowBox.Close()
        {
            _form.Close();
            _form = null;
        }
        void IPlatformWindowBox.SetLocation(int x, int y)
        {
            //invoke Before accept new location
            PreviewBoundChanged?.Invoke(this, EventArgs.Empty);
            //------------------ 
            _formLocalX = x;
            _formLocalY = y;
            //System.Diagnostics.Debug.WriteLine("set location " + x + "," + y);
            //
            if (this.UseRelativeLocationToParent)
            {

                if (!_evalLocationRelativeToDesktop)
                {
                    _locationRelToDesktop = new System.Drawing.Point();
                    if (_form.LinkedParentControl != null)
                    {
                        //get location of this control relative to desktop
                        _locationRelToDesktop = _form.LinkedParentControl.PointToScreen(System.Drawing.Point.Empty);//**
                    }
                    _evalLocationRelativeToDesktop = true;
                }
                _form.Location = new System.Drawing.Point(
                    _locationRelToDesktop.X + x,
                    _locationRelToDesktop.Y + y);
            }
            else
            {
                _form.Location = new System.Drawing.Point(x, y);

            }
            BoundsChanged?.Invoke(this, EventArgs.Empty);
        }

        void IPlatformWindowBox.SetSize(int w, int h)
        {
            PreviewBoundChanged?.Invoke(this, EventArgs.Empty);
            _form.Size = new System.Drawing.Size(w, h);
            BoundsChanged?.Invoke(this, EventArgs.Empty);
        }
        public void GetLocalBounds(out int x, out int y, out int w, out int h)
        {
            System.Drawing.Size size = _form.Size;
            x = _formLocalX;
            y = _formLocalY;
            w = size.Width;
            h = size.Height;
        }
        public void GetLocalBoundsIncludeShadow(out int x, out int y, out int w, out int h)
        {
            System.Drawing.Size size = _form.Size;
            x = _formLocalX;
            y = _formLocalY;
            w = size.Width + FormPopupShadow.SHADOW_SIZE + 5;
            h = size.Height + FormPopupShadow.SHADOW_SIZE + 5;
        }
    }
}