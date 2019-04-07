using Android.App;
using Android.Widget;
using Android.OS;
using Test_Android_Glyph;
using Android.Content.PM;
using Android.Content.Res;



namespace TestApp01.Droid
{
    [Activity(Label = "TestApp01", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        GLView1 view;
        private static AssetManager assetManager;
        public static AssetManager AssetManager { get { return assetManager; } }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            view = new GLView1(this);
            SetContentView(view);
        }
        protected override void OnPause()
        {
            base.OnPause();
            view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            view.Resume();
        }
    }
}

