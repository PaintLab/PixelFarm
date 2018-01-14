//MIT, 2015, Mauricio David
using System;
using System.IO;
using System.Threading;

namespace Numeria.IO
{
    internal static class BinaryWriterExtensions
    {
        private const int MAX_TRY_LOCK_FILE = 50; // Max try to lock the data file
        private const int DELAY_TRY_LOCK_FILE = 50; // in miliseconds

        public static void Write(this BinaryWriter writer, Guid guid)
        {
            writer.Write(guid.ToByteArray());
        }

        public static void Write(this BinaryWriter writer, DateTime dateTime)
        {
            writer.Write(dateTime.Ticks);
        }
        /// <summary>
        /// move to specific positioin from the begining of stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static long SetWritePos(this BinaryWriter writer, long position)
        {
            return writer.BaseStream.Seek(position, SeekOrigin.Begin);
        }

        public static void Lock(this BinaryWriter writer, long position, long length)
        {
            var fileStream = writer.BaseStream as FileStream;

            TryLockFile(fileStream, position, length, 0);
        }

        private static void TryLockFile(FileStream fileStream, long position, long length, int tryCount)
        {
            try
            {
                fileStream.Lock(position, length);
            }
            catch (IOException ex)
            {
                if (ex.IsLockException())
                {
                    if (tryCount >= DELAY_TRY_LOCK_FILE)
                        throw new FileDBException("Database file is in lock for a long time");

                    Thread.Sleep(tryCount * DELAY_TRY_LOCK_FILE);

                    TryLockFile(fileStream, position, length, ++tryCount);
                }
                else
                    throw ex;
            }
        }

        public static void Unlock(this BinaryWriter writer, long position, long length)
        {
            var fileStream = writer.BaseStream as FileStream;
            fileStream.Unlock(position, length);
        }
    }
}
