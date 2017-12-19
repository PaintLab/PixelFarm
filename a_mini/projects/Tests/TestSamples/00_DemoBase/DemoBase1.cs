//MIT, 2014-2017, WinterDev
using System;
using System.Collections.Generic;

using PixelFarm.Agg;
using PixelFarm.Drawing;
using PixelFarm.DrawingGL;

namespace Mini
{

    public delegate void GLSwapBufferDelegate();
    public delegate IntPtr GetGLControlDisplay();
    public delegate IntPtr GetGLSurface();

    public abstract class DemoBase
    {
        public DemoBase()
        {
            this.Width = 800;
            this.Height = 600;
        }

        //when we use with opengl
        GLSwapBufferDelegate _swapBufferDelegate;
        GetGLControlDisplay _getGLControlDisplay;
        GetGLSurface _getGLSurface;
        GLPainter _painter;
        
        public virtual void Draw(Painter p) { }
        public void CloseDemo()
        {
            DemoClosing();
        }

        public virtual void Init() { }

        public virtual void MouseDrag(int x, int y) { }
        public virtual void MouseDown(int x, int y, bool isRightButton) { }
        public virtual void MouseUp(int x, int y) { }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        VertexStorePool _vxsPool = new VertexStorePool();
        public VertexStore GetFreeVxs()
        {
            return _vxsPool.GetFreeVxs();
        }
        public void ReleaseVxs(ref VertexStore vxs)
        {
            _vxsPool.Release(ref vxs);
        }



        protected virtual void DemoClosing()
        {
        }
        protected virtual void OnTimerTick(object sender, EventArgs e)
        {
        }
        protected virtual bool EnableAnimationTimer
        {
            get { return false; }
            set { }
        }

        //----------------------------------------------------
        //for GL
        public virtual void BuildCustomDemoGLContext(out GLRenderSurface glsf, out GLPainter painter)
        {
            glsf = null;
            painter = null;
        }
        public static void InvokeGLContextReady(DemoBase demo, GLRenderSurface glsf, GLPainter painter)
        {
            demo._painter = painter;
            demo.OnGLSurfaceReady(glsf, painter);
            demo.OnReadyForInitGLShaderProgram();
        }
        public static void InvokePainterReady(DemoBase demo, Painter painter)
        {
            demo.OnPainterReady(painter);
        }
        protected virtual void OnGLSurfaceReady(GLRenderSurface canvasGL, GLPainter painter)
        {

        }
        protected virtual void OnPainterReady(Painter painter)
        {

        }
        protected virtual void OnReadyForInitGLShaderProgram()
        {
            //this method is called when the demo is ready for create GLES shader program
        }
        protected virtual void OnGLRender(object sender, EventArgs args)
        {
            this.Draw(_painter);
        }
        public void InvokeGLPaint()
        {
            OnGLRender(this, EventArgs.Empty);
        }

        protected void SwapBuffers()
        {
            //manual swap buffer
            if (_swapBufferDelegate != null)
            {
                _swapBufferDelegate();
            }
        }
        public void SetEssentialGLHandlers(GLSwapBufferDelegate swapBufferDelegate,
            GetGLControlDisplay getGLControlDisplay,
            GetGLSurface getGLSurface)
        {
            _swapBufferDelegate = swapBufferDelegate;
            _getGLControlDisplay = getGLControlDisplay;
            _getGLSurface = getGLSurface;
        }
        protected IntPtr getGLControlDisplay()
        {
            return _getGLControlDisplay();
        }
        protected IntPtr getGLSurface()
        {
            return _getGLSurface();
        }
    }



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
    public static class RootDemoPath
    {
        public static string Path = "";
    }

    public enum DemoConfigPresentaionHint
    {
        TextBox,
        CheckBox,
        OptionBoxes,
        SlideBarDiscrete,
        SlideBarContinuous_R4,
        SlideBarContinuous_R8,
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

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NoteAttribute : Attribute
    {
        public NoteAttribute(string desc)
        {
            this.Desc = desc;
        }
        public string Desc { get; set; }
    }

    public class ExampleConfigDesc
    {
        System.Reflection.PropertyInfo property;
        List<ExampleConfigValue> optionFields;
        public ExampleConfigDesc(DemoConfigAttribute config, System.Reflection.PropertyInfo property)
        {
            this.property = property;
            this.OriginalConfigAttribute = config;
            if (!string.IsNullOrEmpty(config.Name))
            {
                this.Name = config.Name;
            }
            else
            {
                this.Name = property.Name;
            }


            Type propType = property.PropertyType;
            if (propType == typeof(bool))
            {
                this.PresentaionHint = DemoConfigPresentaionHint.CheckBox;
            }
            else if (propType.IsEnum)
            {
                this.PresentaionHint = Mini.DemoConfigPresentaionHint.OptionBoxes;
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
            else if (propType == typeof(Int32))
            {
                this.PresentaionHint = Mini.DemoConfigPresentaionHint.SlideBarDiscrete;
            }
            else if (propType == typeof(double))
            {
                this.PresentaionHint = Mini.DemoConfigPresentaionHint.SlideBarContinuous_R8;
            }
            else if (propType == typeof(float))
            {
                this.PresentaionHint = Mini.DemoConfigPresentaionHint.SlideBarContinuous_R4;
            }
            else
            {
                this.PresentaionHint = DemoConfigPresentaionHint.TextBox;
            }
        }
        public string Name
        {
            get;
            private set;
        }
        public DemoConfigAttribute OriginalConfigAttribute
        {
            get;
            private set;
        }
        public DemoConfigPresentaionHint PresentaionHint
        {
            get;
            private set;
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
    public class ExampleAndDesc
    {
        static Type exConfig = typeof(DemoConfigAttribute);
        static Type exInfoAttrType = typeof(InfoAttribute);
        List<ExampleConfigDesc> configList = new List<ExampleConfigDesc>();
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



            foreach (var property in t.GetProperties())
            {
                if (property.DeclaringType == t)
                {
                    var foundAttrs = property.GetCustomAttributes(exConfig, false);
                    if (foundAttrs.Length > 0)
                    {
                        //this is configurable attrs
                        configList.Add(new ExampleConfigDesc((DemoConfigAttribute)foundAttrs[0], property));
                    }
                }
            }
        }
        public Type Type { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return this.OrderCode + " : " + this.Name;
        }
        public List<ExampleConfigDesc> GetConfigList()
        {
            return this.configList;
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
    public class ExampleConfigValue
    {
        System.Reflection.FieldInfo fieldInfo;
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