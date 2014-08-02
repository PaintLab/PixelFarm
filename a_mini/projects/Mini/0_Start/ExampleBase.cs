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
        public virtual void MouseDown(int x, int y) { }
        public virtual void MouseUp(int x, int y) { }


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
        TextBox,
        OptionBoxes
    }

    public class NoteAttribute : Attribute
    {
        public NoteAttribute(string desc)
        {
            this.Desc = desc;
        }
        public string Desc { get; set; }
    }

    class ExampleConfigDesc
    {
        System.Reflection.PropertyInfo property;
        List<ExampleConfigValue> optionFields;


        public ExampleConfigDesc(System.Reflection.PropertyInfo property, string name)
        {
            this.property = property;
            this.Name = name;

            Type propType = property.PropertyType;
            if (propType == typeof(bool))
            {
                this.PresentaionHint = PresentaionHint.CheckBox;
            }
            else if (propType.IsEnum)
            {
                this.PresentaionHint = Mini.PresentaionHint.OptionBoxes;
                //find option
                var enumFields = propType.GetFields();

                int j = enumFields.Length;
                optionFields = new List<ExampleConfigValue>(j);
                for (int i = 0; i < j; ++i)
                {
                    var enumField = enumFields[i];
                    if (enumField.IsStatic)
                    {
                        //use field name or note that assoc with its field name

                        string fieldNameOrNote = enumField.Name;
                        var foundNotAttr = enumField.GetCustomAttributes(typeof(NoteAttribute), false);
                        if (foundNotAttr.Length > 0)
                        {
                            fieldNameOrNote = ((NoteAttribute)foundNotAttr[0]).Desc;
                        }
                        optionFields.Add(new ExampleConfigValue(property, enumField, fieldNameOrNote));
                    }
                }
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
        public List<ExampleConfigValue> GetOptionFields()
        {
            return this.optionFields;
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

    class ExampleConfigValue
    {

        internal System.Reflection.FieldInfo fieldInfo;
        System.Reflection.PropertyInfo property;
        public ExampleConfigValue(System.Reflection.PropertyInfo property, System.Reflection.FieldInfo fieldInfo, string name)
        {
            this.property = property;
            this.fieldInfo = fieldInfo;
            this.Name = name;
            this.ValueAsInt32 = (int)fieldInfo.GetValue(null);

        }
        public string Name { get; set; }
        public int ValueAsInt32 { get; private set; }

        public void InvokeSet(object target)
        {

            this.property.GetSetMethod().Invoke(target, new object[] { ValueAsInt32 });
        }
    }
}