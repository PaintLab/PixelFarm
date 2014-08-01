using System;


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
        public virtual void Init() { }
        public abstract void Draw(MatterHackers.Agg.Graphics2D g);
        public virtual void MouseDrag(int x, int y) { }
    }

}