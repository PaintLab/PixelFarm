//MIT, 2015, Mauricio David
using System;
using System.IO;

namespace Numeria.IO
{
    internal class HeaderFactory
    {
        public static void ReadFromFile(Header header, BinaryReader reader)
        {
            // Seek the stream on 0 position to read header
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            // Make same validation on header file
            if (reader.ReadUtf8String(Header.FileID.Length) != Header.FileID)
                throw new FileDBException("The file is not a valid storage archive");

            if (reader.ReadInt16() != Header.FileVersion)
                throw new FileDBException("The archive version is not valid");

            header.IndexRootPageID = reader.ReadUInt32();
            header.FreeIndexPageID = reader.ReadUInt32();
            header.FreeDataPageID = reader.ReadUInt32();
            header.LastFreeDataPageID = reader.ReadUInt32();
            header.LastPageID = reader.ReadUInt32();
            header.IsDirty = false;
        }

        public static void WriteToFile(Header header, BinaryWriter writer)
        {
            // Seek the stream on 0 position to save header
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            writer.Write(ToFixedBytes(Header.FileID, Header.FileID.Length));
            writer.Write(Header.FileVersion);

            writer.Write(header.IndexRootPageID);
            writer.Write(header.FreeIndexPageID);
            writer.Write(header.FreeDataPageID);
            writer.Write(header.LastFreeDataPageID);
            writer.Write(header.LastPageID);
        }

        static byte[] ToFixedBytes(string str, int size)
        {
            if (string.IsNullOrEmpty(str))
                return new byte[size]; 
            //fixed size
            var buffer = new byte[size];
            var strbytes = System.Text.Encoding.UTF8.GetBytes(str);
            Array.Copy(strbytes, buffer, size > strbytes.Length ? strbytes.Length : size);

            return buffer;
        }
    }
}
