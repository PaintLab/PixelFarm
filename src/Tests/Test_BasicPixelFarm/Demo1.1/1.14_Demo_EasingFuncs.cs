//Apache2, 2014-2018, WinterDev

using System.Collections.Generic;
using LayoutFarm.CustomWidgets;

namespace LayoutFarm
{
    [DemoNote("1.14 EasingFuncs")]
    class Demo_EasingFuncs : DemoBase
    {
        SimpleBox animationBoard;
        SampleViewport viewport;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.viewport = viewport;
            {
                animationBoard = new SimpleBox(800, 800);
                animationBoard.BackColor = PixelFarm.Drawing.Color.White;
                viewport.AddChild(animationBoard);
            }
            //
            {

                List<PennerAnimationInfo> pennerAnimationList = LoadAllPennerAnimationList();
                ListView easingFuncs_List = new ListView(200, 850);
                easingFuncs_List.SetLocation(600, 20);
                viewport.AddChild(easingFuncs_List);
                easingFuncs_List.ListItemMouseEvent += (s, e) =>
                {

                    //do animation
                    PennerAnimationInfo animation = pennerAnimationList[easingFuncs_List.SelectedIndex];
                    // 
                    GenerateAnimation(animation._generatorDelegate);
                };

                //add item
                foreach (PennerAnimationInfo pennerAnimation in pennerAnimationList)
                {
                    ListItem listItem = new ListItem(200, 20);
                    listItem.Text = pennerAnimation._name;
                    listItem.Tag = pennerAnimation;
                    easingFuncs_List.AddItem(listItem);
                }
            }
        }
        void GenerateAnimation(PennerAnimationGeneratorDelegate pennerAnimator)
        {

            float startValue = 0;
            float stopValue = 300;
            float sec = 0;
            float completeDuration = 10;

            animationBoard.ClearChildren();

            List<double> calculatedValues = new List<double>();
            while (sec < completeDuration)
            {

                double currentValue = pennerAnimator(sec, startValue, stopValue, completeDuration);
                //step 0.1 sec (100 ms), until complete at 5 sec
                sec += 0.1f;
                calculatedValues.Add(currentValue);
                //System.Console.WriteLine(currentValue.ToString());
            }

            //create image box that present the results
            int j = calculatedValues.Count;
            for (int i = 0; i < j; ++i)
            {
                SimpleBox box = new SimpleBox(5, 5);
                box.SetLocation(5 * i, (int)calculatedValues[i]);
                animationBoard.AddChild(box);
            }

            //-----
            //show animation

            SimpleBox sampleBox1 = new SimpleBox(600, 20);
            animationBoard.AddChild(sampleBox1);
            sampleBox1.BackColor = PixelFarm.Drawing.Color.Red;
            int step = 0;
            UIPlatform.RegisterTimerTask(10, tim =>
            {
                //animate the box
                sampleBox1.SetLocation(10, (int)calculatedValues[step]);
                if (step < j - 1)
                {
                    step++;
                }
                else
                {
                    //unregister and remove the task
                    tim.Remove();
                }
            });
        }

        delegate double PennerAnimationGeneratorDelegate(double t, double b, double c, double d);

        struct PennerAnimationInfo
        {
            public readonly string _name;
            public readonly PennerAnimationGeneratorDelegate _generatorDelegate;
            public PennerAnimationInfo(string name, PennerAnimationGeneratorDelegate targetDel)
            {
                this._name = name;
                this._generatorDelegate = targetDel;
            }
            public override string ToString()
            {
                return _name;
            }
        }

        static List<PennerAnimationInfo> LoadAllPennerAnimationList()
        {
            List<PennerAnimationInfo> pennerAnimationList = new List<PennerAnimationInfo>();
            System.Reflection.MethodInfo[] mets = typeof(PennerDoubleAnimation).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            int j = mets.Length;
            for (int i = 0; i < j; ++i)
            {
                pennerAnimationList.Add(new PennerAnimationInfo(mets[i].Name, (PennerAnimationGeneratorDelegate)System.Delegate.CreateDelegate(typeof(PennerAnimationGeneratorDelegate), mets[i])));
            }
            return pennerAnimationList;

        }

    }
}