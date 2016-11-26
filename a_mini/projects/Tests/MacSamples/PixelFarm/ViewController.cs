using System;

using AppKit;
using Foundation;

namespace PixelFarmSkia
{
	public partial class ViewController :NSViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
			//this.Title = "ABC";
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Do any additional setup after loading the view.
			this.View = new DrawnImageView();


		}
		public override NSObject RepresentedObject
		{
			get
			{
				return base.RepresentedObject;
			}
			set
			{
				base.RepresentedObject = value;
				// Update the view, if already loaded.
				 
			}
		}
	}

	 
}
