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

        private void cmd3_Click(object sender, EventArgs e)
        {

            string base64AreaOrg = Convert.ToBase64String(AreaTex.areaTexBytes);


            byte[] dataBuffer = null;
            using (System.Drawing.Bitmap bmp = new Bitmap("d:\\WImageTest\\smaa\\AreaTexDX9.png"))
            {
                //bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                      System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                dataBuffer = new byte[bmpdata.Stride * bmpdata.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, dataBuffer, 0, dataBuffer.Length);
                bmp.UnlockBits(bmpdata);

                ////--
                //bmp.Save("d:\\WImageTest\\smaa_text2.png");
            }

            string base64 = Convert.ToBase64String(dataBuffer);

            //int j = dataBuffer.Length;
            //StringBuilder stbuilder = new StringBuilder();

            //int m = 0;
            //for (int i = 0; i < j; ++i)
            //{
            //    if ((i % 3) == 0)
            //    {
            //        continue; //skip
            //    }

            //    if (i > 0)
            //    {
            //        stbuilder.Append(',');
            //    }
            //    if ((m % 12) == 0)
            //    {
            //        stbuilder.Append("\r\n");
            //    }
            //    m++;
            //    stbuilder.Append(dataBuffer[i].ToString("X"));
            //}
            //string total = stbuilder.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string jsSearchTextImg = "iVBORw0KGgoAAAANSUhEUgAAAEIAAAAhCAAAAABIXyLAAAAAOElEQVRIx2NgGAWjYBSMglEwEICREYRgFBZBqDCSLA2MGPUIVQETE9iNUAqLR5gIeoQKRgwXjwAAGn4AtaFeYLEAAAAASUVORK5CYII=";
            byte[] img1 = Convert.FromBase64String(jsSearchTextImg);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(img1))
            {
                using (System.Drawing.Bitmap bmp1 = new Bitmap(ms))
                {

                    var bmpdata = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                    byte[] textSearch2 = new byte[bmpdata.Stride * bmpdata.Height];
                    System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, textSearch2, 0, textSearch2.Length);
                    string rawBmpDataBase64 = Convert.ToBase64String(textSearch2);

                    bmp1.UnlockBits(bmpdata);
                    bmp1.Save("d:\\WImageTest\\seach_text_a002.png");
                }
            }

            //-------------------------------------------------------------------


            byte[] orgBuffer = SMAASearchTex.searchTexBytes;
            int orgLen = orgBuffer.Length;
            using (System.Drawing.Bitmap bmp = new Bitmap(SMAASearchTex.SEARCHTEX_WIDTH, SMAASearchTex.SEARCHTEX_HEIGHT, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                byte[] expandBuffer = new byte[bmpdata.Stride * bmpdata.Height];
                int writeIndex = 0;
                for (int readIndex = 0; readIndex < orgLen; ++readIndex)
                {
                    byte src = orgBuffer[readIndex];
                    expandBuffer[writeIndex] = src;
                    expandBuffer[writeIndex + 1] = src;
                    expandBuffer[writeIndex + 2] = src;
                    expandBuffer[writeIndex + 3] = 255;

                    writeIndex += 4;
                }

                //
                System.Runtime.InteropServices.Marshal.Copy(
                    expandBuffer, 0,
                    bmpdata.Scan0, expandBuffer.Length);
                //
                bmp.UnlockBits(bmpdata);

                string base64 = Convert.ToBase64String(expandBuffer);

                bmp.Save("d:\\WImageTest\\search_text_a01.png");
            }
        }



        static class SMAASearchTex
        {

            internal const int SEARCHTEX_WIDTH = 64;
            internal const int SEARCHTEX_HEIGHT = 16;
            internal const int SEARCHTEX_PITCH = SEARCHTEX_WIDTH;
            internal const int SEARCHTEX_SIZE = (SEARCHTEX_HEIGHT * SEARCHTEX_PITCH);

            const string seachTextContentBase64 = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACyAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAsgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACyAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABmAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAfQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABmAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAfQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABmAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAZgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAagAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAdQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB1";
            internal static readonly byte[] searchTexBytes = Convert.FromBase64String(seachTextContentBase64);


            internal static readonly byte[] searchTexBytes3 = {
                0xfe, 0xfe, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0xfe, 0xfe, 0x00, 0x7f, 0x7f,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00,
                0x7f, 0x7f, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0xfe, 0x7f, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xfe, 0xfe, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0xfe,
                0xfe, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f,
                0x7f, 0xfe, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xfe, 0xfe, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0xfe, 0xfe, 0x00, 0x7f, 0x7f,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00,
                0x7f, 0x7f, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0xfe, 0x7f, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xfe, 0xfe, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0xfe,
                0xfe, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f,
                0x7f, 0xfe, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x7f,
                0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f,
                0x7f, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f,
                0x7f, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00,
                0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x7f,
                0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f,
                0x7f, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f,
                0x7f, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x00, 0x00,
                0x7f, 0x7f, 0x00, 0x7f, 0x7f, 0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x7f, 0x7f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
            };
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //original areaText is RG 
            //so we extend is to RGB
            //internal const int AREATEX_WIDTH = 160;
            //internal const int AREATEX_HEIGHT = 560;
            //internal const int AREATEX_PITCH = AREATEX_WIDTH;
            //internal const int AREATEX_SIZE = (AREATEX_HEIGHT * AREATEX_PITCH);



            int stride = 160 * 3;
            byte[] rgbBuffer = new byte[stride * 560];
            int readIndex = 0;
            int writeIndex = 0;
            //invert image
            bool invertOutput = false;
            if (invertOutput)
            {
                int writeRowNo = 560 - 1;
                for (int row = 0; row < 560; ++row)
                {
                    writeIndex = writeRowNo * stride;
                    for (int col = 0; col < 160; ++col)
                    {
                        rgbBuffer[writeIndex] = AreaTex.areaTexBytes[readIndex];
                        rgbBuffer[writeIndex + 1] = AreaTex.areaTexBytes[readIndex + 1];

                        readIndex += 2;
                        writeIndex += 3;
                    }
                    writeRowNo--;
                }
            }
            else
            {
                int writeRowNo = 0;
                for (int row = 0; row < 560; ++row)
                {
                    writeIndex = writeRowNo * stride;
                    for (int col = 0; col < 160; ++col)
                    {
                        //BGRA
                        rgbBuffer[writeIndex + 1] = AreaTex.areaTexBytes[readIndex + 1];
                        rgbBuffer[writeIndex + 2] = AreaTex.areaTexBytes[readIndex + 0];

                        readIndex += 2;
                        writeIndex += 3;
                    }
                    writeRowNo++;
                }
                SaveOutputImageAsRGB24(rgbBuffer, "d:\\WImageTest\\area_nz1.png");
            }


            ////finish
            ////string base64Str = Convert.ToBase64String(AreaTex.areaTexBytes);
            string base64StrOfRGB = Convert.ToBase64String(rgbBuffer);


        }
        void SaveOutputImageAsRGB24(byte[] rgbBuffer, string filename)
        {
            using (System.Drawing.Bitmap bmp = new Bitmap(160, 560, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(
                  rgbBuffer, 0,
                  bmpdata.Scan0, rgbBuffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save(filename);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string tt_01 = "d:\\WImageTest\\tt_01.png";
            using (System.Drawing.Bitmap bmp = new Bitmap(tt_01))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                int buffLen = bmpdata.Stride * bmpdata.Height;

                byte[] buffer = new byte[buffLen];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffLen);
                bmp.UnlockBits(bmpdata);
                string base64StrOfRGB = Convert.ToBase64String(buffer);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] orgSearch = SMAASearchTex.searchTexBytes3;
            //internal const int SEARCHTEX_WIDTH = 64;
            //internal const int SEARCHTEX_HEIGHT = 16;

            //invert
            byte[] invertedSearch = new byte[orgSearch.Length * 3];
            int width = 64;
            int height = 16;
            //
            int readIndex = 0;
            int stride = width * 3;
            int writeIndex = (height * stride) - stride;

            for (int h = 0; h < height; ++h)
            {
                readIndex = h * width;

                for (int row = 0; row < width; ++row)
                {
                    invertedSearch[writeIndex + row] = orgSearch[readIndex];
                    invertedSearch[writeIndex + row + 1] = orgSearch[readIndex];
                    invertedSearch[writeIndex + row + 2] = orgSearch[readIndex];
                    readIndex++;
                }

                writeIndex -= stride;
            }

            //expand
            string searchInvertBase64 = Convert.ToBase64String(invertedSearch);
            using (System.Drawing.Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Runtime.InteropServices.Marshal.Copy(invertedSearch, 0, bmpdata.Scan0, invertedSearch.Length);
                bmp.UnlockBits(bmpdata);

                bmp.Save("d:\\WImageTest\\search_flip.png");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int width = 64;
            int height = 16;
            string searchFlip = "d:\\WImageTest\\smaa\\flipY.png";
            byte[] outputBuffer = new byte[width * 4 * height];
            using (System.Drawing.Bitmap bmp = new Bitmap(searchFlip))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, outputBuffer, 0, outputBuffer.Length);
                bmp.UnlockBits(bmpdata);
            }
            //-----
            //reduce to 1 dimension

            int newLen = width * height;
            byte[] outputBuffer2 = new byte[width * height];
            int writeIndex = 0;
            int readIndex = 0;
            for (int i = 0; i < newLen; ++i)
            {
                outputBuffer2[writeIndex] = outputBuffer[readIndex];
            }


        }

        private void button7_Click(object sender, EventArgs e)
        {
            //read new inverted png
            //save bmp data to bmp buffer

            string areaTextFlip = "d:\\WImageTest\\smaa\\AreaTex001.png";
            string searchTextFlip = "d:\\WImageTest\\smaa\\SearchTex001.png";


            string base64_areaTextFlip = null;
            string base64_searchTextFlip = null;

            using (System.Drawing.Bitmap bmp = new Bitmap(areaTextFlip))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                byte[] data = new byte[bmpdata.Stride * bmp.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, data, 0, data.Length);
                bmp.UnlockBits(bmpdata);
                base64_areaTextFlip = Convert.ToBase64String(data);
                using (System.Drawing.Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    var bmpdata2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    System.Runtime.InteropServices.Marshal.Copy(data, 0, bmpdata2.Scan0, data.Length);
                    bmp2.UnlockBits(bmpdata2);
                    bmp2.Save("d:\\WImageTest\\smaa\\areaTextFlip_tt.png");
                }
            }
            using (System.Drawing.Bitmap bmp = new Bitmap(searchTextFlip))
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                byte[] data = new byte[bmpdata.Stride * bmp.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, data, 0, data.Length);
                bmp.UnlockBits(bmpdata);
                base64_searchTextFlip = Convert.ToBase64String(data);

                using (System.Drawing.Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    var bmpdata2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    System.Runtime.InteropServices.Marshal.Copy(data, 0, bmpdata2.Scan0, data.Length);
                    bmp2.UnlockBits(bmpdata2);
                    bmp2.Save("d:\\WImageTest\\smaa\\searchTextFlip_tt.png");
                }
            }
        }
    }
}
