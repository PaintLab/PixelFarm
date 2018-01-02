//Apache2, 2017-2018, WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using PixelFarm;

namespace InterfaceGen
{

    //Experiment


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //1. PixelFarm.DrawingCore
            Assembly asm = typeof(PixelFarm.Drawing.Rectangle).Assembly;
            Analyze(asm);
            //2. PixelFarm.DrawingCanvas


        }
        static void Analyze(Assembly asm)
        {
            Type[] allTypes = asm.GetTypes();
            //collect only public type
            List<Type> onlyPublicTypes = new List<Type>();
            foreach (Type type in allTypes)
            {
                CollectPublicType(type, onlyPublicTypes);
            }
            //--
            //write output to the text file
            StringBuilder stbuilder = new StringBuilder();
            foreach (Type publicType in onlyPublicTypes)
            {
                
                stbuilder.Append("public interface ");
                stbuilder.AppendLine(publicType.Name);
                stbuilder.AppendLine("{");
                //write each public member name 
                MemberInfo[] allPublicMembers = publicType.GetMembers(BindingFlags.DeclaredOnly);


                stbuilder.AppendLine("}");
                stbuilder.AppendLine();
            }
        }
        static void CollectPublicType(Type t, List<Type> typeList)
        {
            if (!t.IsPublic)
            {
                return;
            }
            typeList.Add(t);
            MemberInfo[] allMembers = t.GetMembers();
            foreach (MemberInfo mm in allMembers)
            {
                if (mm is Type)
                {
                    CollectPublicType((Type)mm, typeList);
                }
            }

        }

    }
}
