namespace YourImplementation
{
    public static class TestBedStartup
    {
        public static void StartSetup()
        {
#if GL_ENABLE
            OpenTK.Toolkit.Init();
#endif
            //you can use your font loader
            YourImplementation.BootStrapWinGdi.SetupDefaultValues();

#if GL_ENABLE
            YourImplementation.BootStrapOpenGLES2.SetupDefaultValues();
#endif
            //default text breaker, this bridge between 
            LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker();

        }
    }
}