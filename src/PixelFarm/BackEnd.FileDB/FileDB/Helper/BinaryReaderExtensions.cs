//MIT, 2015, Mauricio David
using System;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    internal static class BinaryReaderExtensions
    {
        public static string ReadUtf8String(this BinaryReader reader, int size)
        {
            var bytes = reader.ReadBytes(size);
            return Encoding.UTF8.GetString(bytes);
        }

        public static Guid ReadGuid(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(16);
            return new Guid(bytes);
        }

        public static DateTime ReadDateTime(this BinaryReader reader)
        {
            var ticks = reader.ReadInt64();
            return new DateTime(ticks);
        }
        /// <summary>
        /// seek from this beginning
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static long SetReadPos(this BinaryReader reader, long position)
        {
            return reader.BaseStream.Seek(position, SeekOrigin.Begin);
        }
    }
}
