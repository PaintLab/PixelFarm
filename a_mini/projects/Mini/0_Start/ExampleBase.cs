using System;
using System.Collections.Generic;


namespace Mini
{

    public class ExDescAttribute : Attribute
    {
        public ExDescAttribute(string desc)
        {
            this.Description = desc;
        }
        public string Description { get; set; }
    }

    public abstract class ExampleBase
    {
        public abstract void Draw(MatterHackers.Agg.Graphics2D g);
        public virtual void Init() { }
        public virtual void MouseDrag(int x, int y) { }
    }

    public class ExConfigAttribute : Attribute
    {
        public ExConfigAttribute()
        {

        }
        public ExConfigAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }
    enum PresentaionHint
    {
        CheckBox,
        SlideBar,
        TextBox
    }
    class ExampleConfigDesc
    {
        System.Reflection.PropertyInfo property;
        public ExampleConfigDesc(System.Reflection.PropertyInfo property, string name)
        {
            this.property = property;
            this.Name = name;

            if (property.PropertyType == typeof(bool))
            {
                this.PresentaionHint = PresentaionHint.CheckBox;
            }
            else
            {
                this.PresentaionHint = PresentaionHint.TextBox;
            }

        }
        public string Name
        {
            get;
            set;
        }
        public PresentaionHint PresentaionHint
        {
            get;
            set;
        }

        public void InvokeSet(object target, object value)
        {
            this.property.GetSetMethod().Invoke(target, new object[] { value });
        }
        public object InvokeGet(object target)
        {
            return this.property.GetGetMethod().Invoke(target, null);
        }
    }
    class ExampleAndDesc
    {
        static Type exConfig = typeof(ExConfigAttribute);

        List<ExampleConfigDesc> configList = new List<ExampleConfigDesc>();
        public ExampleAndDesc(Type t, string desc)
        {
            this.Type = t;
            this.Desc = desc;
            var p1 = t.GetProperties();

            foreach (var property in t.GetProperties())
            {
                if (property.DeclaringType == t)
                {
                    var foundAttrs = property.GetCustomAttributes(exConfig, false);
                    if (foundAttrs.Length > 0)
                    {
                        //this is configurable attrs
                        configList.Add(new ExampleConfigDesc(property, property.Name));
                    }
                }

            }
        }
        public Type Type { get; set; }
        public string Desc { get; set; }
        public override string ToString()
        {
            return this.Desc;
        }
        public List<ExampleConfigDesc> GetConfigList()
        {
            return this.configList;
        }
    }
}