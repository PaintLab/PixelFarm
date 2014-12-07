//MIT 2014, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;


namespace Mini2
{


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InfoAttribute : Attribute
    {
        public InfoAttribute()
        {
        }
        public InfoAttribute(DemoCategory catg)
        {
            this.Category = catg;
        }
        public InfoAttribute(string desc)
        {
            this.Description = desc;
        }
        public InfoAttribute(DemoCategory catg, string desc)
        {
            this.Category = catg;
            this.Description = desc;
        }
        public string Description { get; private set; }
        public DemoCategory Category { get; private set; }
        public string OrderCode { get; set; }
    }

    public enum DemoCategory
    {
        Vector,
        Bitmap
    }
    public delegate Graphics2D RequestNewGraphic2DDelegate();

    public abstract class DemoBase
    {
        public DemoBase()
        {
            this.Width = 800;
            this.Height = 600;
        }

        public event RequestNewGraphic2DDelegate RequestNewGfx2d;
         
        public virtual void MouseDrag(int x, int y) { }
        public virtual void MouseDown(int x, int y, bool isRightButton) { }
        public virtual void MouseUp(int x, int y) { }
        public int Width { get; set; }
        public int Height { get; set; } 
        protected Graphics2D NewGraphics2D()
        {
            if (RequestNewGfx2d == null)
            {
                throw new NotSupportedException();
            }
            else
            {
                return RequestNewGfx2d();
            }
        }

        public virtual void Load()
        {
        }
    }

    public class DemoConfigAttribute : Attribute
    {
        public DemoConfigAttribute()
        {

        }
        public DemoConfigAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }


        public int MinValue { get; set; }
        public int MaxValue { get; set; }

    }
    enum DemoConfigPresentaionHint
    {
        TextBox,
        CheckBox,
        OptionBoxes,
        SlideBarDiscrete,
        SlideBarContinuous_R4,
        SlideBarContinuous_R8,
    }
     

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NoteAttribute : Attribute
    {
        public NoteAttribute(string desc)
        {
            this.Desc = desc;
        }
        public string Desc { get; set; }
    }

    class ExampleAndDesc
    {
        static Type exConfig = typeof(DemoConfigAttribute);
        static Type exInfoAttrType = typeof(InfoAttribute);
 
        public ExampleAndDesc(Type t, string name)
        {
            this.Type = t;
            this.Name = name;
            this.OrderCode = "";
            var p1 = t.GetProperties();

            InfoAttribute[] exInfoList = t.GetCustomAttributes(exInfoAttrType, false) as InfoAttribute[];
            int m = exInfoList.Length;


            if (m > 0)
            {

                for (int n = 0; n < m; ++n)
                {
                    InfoAttribute info = exInfoList[n];
                    if (!string.IsNullOrEmpty(info.OrderCode))
                    {
                        this.OrderCode = info.OrderCode;
                    }

                    if (!string.IsNullOrEmpty(info.Description))
                    {
                        this.Description += " " + info.Description;
                    }
                }

            }
            if (string.IsNullOrEmpty(this.Description))
            {
                this.Description = this.Name;
            }



 
        }
        public Type Type { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return this.OrderCode + " : " + this.Name;
        }
         
        public string Description
        {
            get;
            private set;
        }
        public string OrderCode
        {
            get;
            set;
        }
    }

}