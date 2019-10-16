//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
namespace LayoutFarm
{
    public abstract class SmartApp : App
    {
        Box _primaryContainer;
        AppHost _appHost;       
        
        protected AppHost AppHost => _appHost;
        protected sealed override void OnStart(AppHost host)
        {
            _appHost = host;
            OnInitializing();
        }
        protected virtual void OnInitializing() { }
        public void SetPrimaryContainer(Box primaryContainer)
        {
            _primaryContainer = primaryContainer;
        }
        protected void AddToPrimaryContainer(UIElement ui)
        {
            if (_primaryContainer != null)
            {
                _primaryContainer.AddChild(ui);
            }
            else
            {
                _appHost.AddChild(ui);
            }
        }
        protected Size GetPrimaryContainerSize()
        {
            if (_primaryContainer != null)
            {
                return new Size(_primaryContainer.Width, _primaryContainer.Height);
            }
            else
            {
                //use primary screen size ?
                //or main form size?
                return new Size(_appHost.PrimaryScreenWidth, _appHost.PrimaryScreenHeight);
            }
        }
    }
}