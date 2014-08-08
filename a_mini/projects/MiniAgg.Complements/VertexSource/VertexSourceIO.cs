using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MatterHackers.Agg.VertexSource
{
    public class VertexSourceIO
    {
        public static void Load(PathStorage vertexSource, string pathAndFileName)
        {
            vertexSource.remove_all();
            string[] allLines = File.ReadAllLines(pathAndFileName);
            foreach (string line in allLines)
            {
                string[] elements = line.Split(',');
                double x = double.Parse(elements[0]);
                double y = double.Parse(elements[1]);
                ShapePath.FlagsAndCommand flagsAndCommand = (ShapePath.FlagsAndCommand)System.Enum.Parse(typeof(ShapePath.FlagsAndCommand), elements[2].Trim());
                for (int i = 3; i < elements.Length; i++)
                {
                    flagsAndCommand |= (ShapePath.FlagsAndCommand)System.Enum.Parse(typeof(ShapePath.FlagsAndCommand), elements[i].Trim());
                }

                vertexSource.Add(x, y, flagsAndCommand);
            }
        }

        public static void Save(IVertexSource vertexSource, string pathAndFileName, bool oldStyle = true)
        {
            if (oldStyle)
            {
                using (StreamWriter outFile = new StreamWriter(pathAndFileName))
                {
                    vertexSource.rewind(0);
                    double x;
                    double y;
                    ShapePath.FlagsAndCommand flagsAndCommand = vertexSource.vertex(out x, out y);
                    do
                    {
                        outFile.WriteLine("{0}, {1}, {2}", x, y, flagsAndCommand.ToString());
                        flagsAndCommand = vertexSource.vertex(out x, out y);
                    }
                    while (flagsAndCommand != ShapePath.FlagsAndCommand.CommandStop);
                }
            }
            else
            {
                using (StreamWriter outFile = new StreamWriter(pathAndFileName))
                {
                    foreach (VertexData vertexData in vertexSource.Vertices())
                    {
                        outFile.WriteLine("{0}, {1}, {2}", vertexData.position.x, vertexData.position.y, vertexData.command.ToString());
                    }
                }
            }
        }


        //-------------------------------------------------
        public static void WriteToStream(BinaryWriter writer, PathStorage pathSource)
        {
            int num_vertice;
            int num_alloc_vertice;
            double[] coord_xy;
            ShapePath.FlagsAndCommand[] cmds;
            PathStorage.UnsafeDirectGetData(pathSource,
                out num_alloc_vertice,
                out num_vertice,
                out coord_xy,
                out cmds);

            //write to binary format ?
            //1.  
            writer.Write(num_alloc_vertice);//hint
            //2. 
            writer.Write(num_vertice); //actual vertices
            //3. all vertice
            int totalCoord = num_vertice << 1;
            for (int i = 0; i < totalCoord; )
            {
                writer.Write(coord_xy[i]);//x
                i++;
                writer.Write(coord_xy[i]);//y
                i++;
            }
            writer.Write(num_vertice); //actual vertices
            //4. all commands
            for (int i = 0; i < num_vertice; ++i)
            {
                writer.Write((byte)cmds[i]);
            }
            writer.Write((int)0);
            writer.Flush();
            //------------------------------------
        }
        public static void WriteColorsToStream(BinaryWriter writer, MatterHackers.Agg.RGBA_Bytes[] colors)
        {
            int len = colors.Length;
            //1.
            writer.Write(len);
            for (int i = 0; i < len; ++i)
            {
                MatterHackers.Agg.RGBA_Bytes color = colors[i];
                writer.Write(color.red);
                writer.Write(color.green);
                writer.Write(color.blue);
                writer.Write(color.alpha);
            }
            writer.Write((int)0);
            writer.Flush();

        }
        public static void WritePathIndexListToStream(BinaryWriter writer, int[] pathIndice, int len)
        {

            //1.
            writer.Write(len);
            for (int i = 0; i < len; ++i)
            {
                writer.Write(pathIndice[i]);
            }
            writer.Write((int)0);
            writer.Flush();

        }
        public static void ReadPathDataFromStream(BinaryReader reader, out PathStorage newPath)
        {
            newPath = new PathStorage();
            //1.
            int num_alloc_vertice = reader.ReadInt32();//hint
            //2.
            int num_vertice = reader.ReadInt32();//actual vertice num
            int totalCoord = num_vertice << 1;
            //3.
            double[] coord_xy = new double[totalCoord];
            //4.
            ShapePath.FlagsAndCommand[] cmds = new ShapePath.FlagsAndCommand[num_vertice];

            for (int i = 0; i < totalCoord; )
            {
                coord_xy[i] = reader.ReadDouble();
                i++;
                coord_xy[i] = reader.ReadDouble();
                i++;
            }
            //4.
            int cmds_count = reader.ReadInt32();
            for (int i = 0; i < cmds_count; ++i)
            {
                cmds[i] = (ShapePath.FlagsAndCommand)reader.ReadByte();
            }

            PathStorage.UnsafeDirectSetData(
                newPath,
                num_alloc_vertice,
                num_vertice,
                coord_xy,
                cmds);
            int end = reader.ReadInt32();

        }

        public static void ReadColorDataFromStream(BinaryReader reader, out MatterHackers.Agg.RGBA_Bytes[] colors)
        {
            int len = reader.ReadInt32();
            colors = new RGBA_Bytes[len];
            for (int i = 0; i < len; ++i)
            {
                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                byte a = reader.ReadByte();
                colors[i] = new RGBA_Bytes(r, g, b, a);
            }
            int end = reader.ReadInt32();
        }
        public static void ReadPathIndexListFromStream(BinaryReader reader, out int len, out int[] pathIndexList)
        {
            len = reader.ReadInt32();
            pathIndexList = new int[len];
            for (int i = 0; i < len; ++i)
            {
                pathIndexList[i] = reader.ReadInt32();
            }
            int end = reader.ReadInt32();
        }
    }
}
