//Apache2, 2014-present, WinterDev
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("1.11_1 MultipleLabels")]
    class Demo_MultipleLabels : App
    {
        protected override void OnStart(AppHost host)
        {
            Box background_box = new Box(250, 500);
            background_box.BackColor = PixelFarm.Drawing.Color.Blue;
            host.AddChild(background_box);

            //PixelFarm.Drawing.RequestFont font = new PixelFarm.Drawing.RequestFont("Source Sans Pro", 20);
            PixelFarm.Drawing.RequestFont font = new PixelFarm.Drawing.RequestFont("Source Sans Pro", 20);
            for (int i = 0; i < 10; ++i)
            {
                Label label = new Label();
                label.SetLocation(i * 20, i * 20);
                label.TextColor = PixelFarm.Drawing.Color.Black;
                label.SetFont(font);
                label.Text = "Lpppyf ABCDEFG HI0123 456789 ABD";
                host.AddChild(label);
            }
        }
    }


    [DemoNote("1.11_2 MultipleLabels2")]
    class Demo_MultipleLabels2 : App
    {
        protected override void OnStart(AppHost host)
        {
            Box background_box = new Box(500, 500);
            background_box.BackColor = PixelFarm.Drawing.Color.Blue;
            host.AddChild(background_box);

            for (int i = 0; i < 10; ++i)
            {
                TextFlowLabel label = new TextFlowLabel(100, 50);
                label.SetLocation(i * 55, i * 55);
                label.BackColor = PixelFarm.Drawing.Color.Yellow;
                //label.Color = PixelFarm.Drawing.Color.Black;
                label.Text = "ABCDEFG\r\nHIJKLMNOP\r\nQRSTUVWXZYZ\r\n0123456789";
                host.AddChild(label);
            }
        }
    }
    [DemoNote("1.11_3 TwoLabels")]
    class Demo_TwoLabels : App
    {
        protected override void OnStart(AppHost host)
        {
            Box background_box = new Box(250, 500);
            background_box.BackColor = PixelFarm.Drawing.Color.White;
            host.AddChild(background_box);

            //PixelFarm.Drawing.RequestFont font = new PixelFarm.Drawing.RequestFont("Source Sans Pro", 20);
            PixelFarm.Drawing.RequestFont font = new PixelFarm.Drawing.RequestFont("Source Sans Pro", 20);
            {
                Label label = new Label();
                label.TextColor = PixelFarm.Drawing.Color.Black;
                label.SetLocation(10, 20);
                label.SetFont(font);
                label.Text = "A";
                host.AddChild(label);
               
            }
            {
                Label label = new Label();
                label.TextColor = PixelFarm.Drawing.Color.Black;
                label.SetFont(font);
                label.Text = "A12345";
                label.SetLocation(30, 20);
                host.AddChild(label);
            }
            //{
            //    Label label = new Label();
            //    label.TextColor = PixelFarm.Drawing.Color.Black;
            //    label.SetFont(font);
            //    label.Text = "XY";
            //    label.SetLocation(30, 20);
            //    host.AddChild(label);
            //}

        }
    }
}