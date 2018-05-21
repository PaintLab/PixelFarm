//MIT, 2015, Mauricio David
using System;
using System.IO;

namespace Numeria.IO
{
    internal class DataFactory
    {
        public static uint GetStartDataPageID(Engine engine)
        {
            if (engine.Header.FreeDataPageID != uint.MaxValue) // we have free page inside the disk file. Use it
            {
                // Take the first free data page
                var startPage = PageFactory.GetDataPage(engine.Header.FreeDataPageID, engine.Reader, true);

                engine.Header.FreeDataPageID = startPage.NextPageID; // and point the free page to new free one

                // If the next page is MAX, fix too LastFreeData

                if (engine.Header.FreeDataPageID == uint.MaxValue)
                    engine.Header.LastFreeDataPageID = uint.MaxValue;

                return startPage.PageID;
            }
            else // if we don't have free data pages, then create new one.
            {
                engine.Header.LastPageID++;
                return engine.Header.LastPageID;
            }
        }

        // Take a new data page on sequence and update the last
        public static DataPage GetNewDataPage(DataPage basePage, Engine engine)
        {
            if (basePage.NextPageID != uint.MaxValue)
            {
                PageFactory.WriteToFile(basePage, engine.Writer); // Write last page on disk

                var dataPage = PageFactory.GetDataPage(basePage.NextPageID, engine.Reader, false);

                engine.Header.FreeDataPageID = dataPage.NextPageID;

                if (engine.Header.FreeDataPageID == uint.MaxValue)
                    engine.Header.LastFreeDataPageID = uint.MaxValue;

                return dataPage;
            }
            else
            {
                var pageID = ++engine.Header.LastPageID;
                DataPage newPage = new DataPage(pageID);
                basePage.NextPageID = newPage.PageID;
                PageFactory.WriteToFile(basePage, engine.Writer); // Write last page on disk
                return newPage;
            }
        }

        static byte ReadByte(byte[] src, int startpos, out int outputpos)
        {
            outputpos = startpos + 1;
            return src[startpos];
        }
        static long ReadInt64(byte[] src, int startpos, out int outputpos)
        {
            //from ms reference src ***
            uint lo = (uint)(src[startpos + 0] | src[startpos + 1] << 8 |
                               src[startpos + 2] << 16 | src[startpos + 3] << 24);

            uint hi = (uint)(src[startpos + 4] | src[startpos + 5] << 8 |
                             src[startpos + 6] << 16 | src[startpos + 7] << 24);

            outputpos = startpos + 8;
            return (long)((ulong)hi) << 32 | lo;
        }
        static uint ReadUInt16(byte[] src, int startpos, out int outputpos)
        {
            //from ms reference src ***
            outputpos = startpos + 2;
            return (uint)(src[startpos + 0] | src[startpos + 1] << 8);
        }
        static int WriteByte(byte[] destination, int startpos, byte data)
        {
            destination[startpos] = data;
            return startpos += 1;
        }
        static int WriteInt64(byte[] destination, int startpos, long data)
        {   //from ms reference src ***
            destination[startpos + 0] = (byte)data;
            destination[startpos + 1] = (byte)(data >> 8);
            destination[startpos + 2] = (byte)(data >> 16);
            destination[startpos + 3] = (byte)(data >> 24);
            destination[startpos + 4] = (byte)(data >> 32);
            destination[startpos + 5] = (byte)(data >> 40);
            destination[startpos + 6] = (byte)(data >> 48);
            destination[startpos + 7] = (byte)(data >> 56);
            return startpos += 8;
        }
        static int WriteUInt16(byte[] destination, int startpos, ushort data)
        {   //from ms reference src ***
            destination[startpos + 0] = (byte)data;
            destination[startpos + 1] = (byte)(data >> 8);
            return startpos += 2;
        }
        static int WriteBuffer(byte[] destination, int startpos, byte[] data)
        {
            int j = data.Length;
            Array.Copy(data, 0, destination, startpos, j);
            return startpos += j;
        }
        static int WriteBuffer(byte[] destination, int startpos, byte[] data, int writeLen)
        {
            Array.Copy(data, 0, destination, startpos, writeLen);
            return startpos += writeLen;
        }





        public static void InsertFile(IndexNode node, EntryInfo enInfo, Stream stream, Engine engine)
        {

            var buffer = new byte[DataPage.DATA_PER_PAGE];

            int dataFreeInPage = DataPage.DATA_PER_PAGE;
            int writePos = 0;
            int read = 0;
            DataPage dataPage = engine.GetDataPage(node.DataPageID);
            if (!dataPage.IsEmpty) // This is never to happend!!
                throw new FileDBException("Page {0} is not empty", dataPage.PageID);
            //-----------------------
            //write metadata 
            //1. datetime of file
            DateTime filedtm = enInfo.FileDateTime;
            //-------------------------- 
            writePos = WriteByte(dataPage.DataBlock, 0, 1); //marker - 1
            writePos = WriteInt64(dataPage.DataBlock, writePos, filedtm.ToBinary());//data part
            //--------------------------            
            //2. long filename (if has long filename) 
            //-----------------------
            if (enInfo.HasLongFileName)
            {
                //write full filename here 
                byte[] longFileNameBuff = System.Text.Encoding.UTF8.GetBytes(enInfo.FileUrl);
                int buffLen = longFileNameBuff.Length;
                if (buffLen > 512)
                {
                    //for this version ***
                    throw new Exception("file name must not longer than 512 bytes");
                }
                writePos = WriteByte(dataPage.DataBlock, writePos, 2);//marker 2 long filename
                //--------
                writePos = WriteUInt16(dataPage.DataBlock, writePos, (ushort)buffLen); //2 bytes length of buffer
                writePos = WriteBuffer(dataPage.DataBlock, writePos, longFileNameBuff); //buffer
                //--------
            }

            dataPage.IsEmpty = false;
            dataPage.DataBlockLength = (short)writePos;
            dataFreeInPage -= writePos;


            node.FileMetaDataLength = (ushort)writePos;
            //-----------------------
            bool isFirstRound = true;
            uint fileLength = 0;
            while ((read = stream.Read(buffer, 0, dataFreeInPage)) > 0)
            {
                //if we have some data to write 
                fileLength += (uint)read;
                if (!isFirstRound)
                {
                    dataPage = GetNewDataPage(dataPage, engine);
                    if (!dataPage.IsEmpty) // This is never to happend!!
                        throw new FileDBException("Page {0} is not empty", dataPage.PageID);
                }
                writePos = WriteBuffer(dataPage.DataBlock, writePos, buffer, read);
                dataPage.IsEmpty = false;
                dataPage.DataBlockLength = (short)writePos;
                //----------------------------------------------
                //reset  for next
                dataFreeInPage = DataPage.DATA_PER_PAGE;
                writePos = 0;
                isFirstRound = false;
                //----------------------------------------------
            }


            // If the last page point to another one, i need to fix that
            if (dataPage.NextPageID != uint.MaxValue)
            {
                engine.Header.FreeDataPageID = dataPage.NextPageID;
                dataPage.NextPageID = uint.MaxValue;
            }

            // Save the last page on disk
            PageFactory.WriteToFile(dataPage, engine.Writer);

            // Save on node index that file length
            node.FileLength = fileLength;

        }
        //read file meta data only
        public static void ReadFileMetadata(IndexNode node, EntryInfo entry, Engine engine)
        {
            var dataPage = PageFactory.GetDataPage(node.DataPageID, engine.Reader, false);
            int metaDataStartPos = 0;
            int metaDataLength = entry.FileMetadataLength;
            int dataStartPos = metaDataStartPos + metaDataLength;

            int curReadPos = 0;
            if (dataPage != null && entry.FileMetadataLength > 0)
            {
                //first round
                //read file meta data
                while (curReadPos < metaDataLength)
                {
                    //1.marker
                    byte marker = ReadByte(dataPage.DataBlock, curReadPos, out curReadPos);
                    switch (marker)
                    {
                        case 1://date time
                            long binaryTime = ReadInt64(dataPage.DataBlock, curReadPos, out curReadPos);
                            entry.FileDateTime = DateTime.FromBinary(binaryTime);
                            break;
                        case 2:
                            //long filename
                            int nameLen = (int)ReadUInt16(dataPage.DataBlock, curReadPos, out curReadPos);
                            byte[] nameBuffer = new byte[nameLen];
                            Array.Copy(dataPage.DataBlock, curReadPos, nameBuffer, 0, nameLen);
                            entry.FileUrl = System.Text.Encoding.UTF8.GetString(nameBuffer);
                            curReadPos += nameLen;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }
            //-----------------------------------
            //not read file part 
        }
        public static void ReadFile(IndexNode node, EntryInfo entry, Stream stream, Engine engine)
        {
            var dataPage = PageFactory.GetDataPage(node.DataPageID, engine.Reader, false);
            int metaDataStartPos = 0;
            int metaDataLength = entry.FileMetadataLength;
            int dataStartPos = metaDataStartPos + metaDataLength;

            int curReadPos = 0;
            if (dataPage != null && entry.FileMetadataLength > 0)
            {
                //first round
                //read file meta data
                while (curReadPos < metaDataLength)
                {
                    //1.marker
                    byte marker = ReadByte(dataPage.DataBlock, curReadPos, out curReadPos);
                    switch (marker)
                    {
                        case 1://date time
                            long binaryTime = ReadInt64(dataPage.DataBlock, curReadPos, out curReadPos);
                            entry.FileDateTime = DateTime.FromBinary(binaryTime);
                            break;
                        case 2:
                            //long filename
                            int nameLen = (int)ReadUInt16(dataPage.DataBlock, curReadPos, out curReadPos);
                            byte[] nameBuffer = new byte[nameLen];
                            Array.Copy(dataPage.DataBlock, curReadPos, nameBuffer, 0, nameLen);
                            entry.FileUrl = System.Text.Encoding.UTF8.GetString(nameBuffer);
                            curReadPos += nameLen;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }
            //-----------------------------------
            //data part 
            int toReadLen = dataPage.DataBlockLength - curReadPos;
            while (dataPage != null)
            {
                stream.Write(dataPage.DataBlock, curReadPos, toReadLen);
                if (dataPage.NextPageID == uint.MaxValue)
                {
                    dataPage = null;
                }
                else
                {
                    dataPage = PageFactory.GetDataPage(dataPage.NextPageID, engine.Reader, false);
                    //reset
                    curReadPos = 0;
                    toReadLen = dataPage.DataBlockLength;
                }
            } 
        }
        public static void ReadOnlyFileContent(IndexNode node, EntryInfo entry, Stream stream, Engine engine)
        {
            var dataPage = PageFactory.GetDataPage(node.DataPageID, engine.Reader, false);
            int metaDataStartPos = 0;
            int metaDataLength = entry.FileMetadataLength;
            int dataStartPos = metaDataStartPos + metaDataLength;


            //if (dataPage != null && entry.FileMetadataLength > 0)
            //{
            //    //first round
            //    //read file meta data
            //    while (curReadPos < metaDataLength)
            //    {
            //        //1.marker
            //        byte marker = ReadByte(dataPage.DataBlock, curReadPos, out curReadPos);
            //        switch (marker)
            //        {
            //            case 1://date time
            //                long binaryTime = ReadInt64(dataPage.DataBlock, curReadPos, out curReadPos);
            //                entry.FileDateTime = DateTime.FromBinary(binaryTime);
            //                break;
            //            case 2:
            //                //long filename
            //                int nameLen = (int)ReadUInt16(dataPage.DataBlock, curReadPos, out curReadPos);
            //                byte[] nameBuffer = new byte[nameLen];
            //                Array.Copy(dataPage.DataBlock, curReadPos, nameBuffer, 0, nameLen);
            //                entry.FileUrl = System.Text.Encoding.UTF8.GetString(nameBuffer);
            //                curReadPos += nameLen;
            //                break;
            //            default:
            //                throw new NotSupportedException();
            //        }
            //    }
            //}

            int curReadPos = dataStartPos;
            //-----------------------------------
            //data part 
            int toReadLen = dataPage.DataBlockLength - curReadPos;
            while (dataPage != null)
            {
                stream.Write(dataPage.DataBlock, curReadPos, toReadLen);
                if (dataPage.NextPageID == uint.MaxValue)
                {
                    dataPage = null;
                }
                else
                {
                    dataPage = PageFactory.GetDataPage(dataPage.NextPageID, engine.Reader, false);
                    //reset
                    curReadPos = 0;
                    toReadLen = dataPage.DataBlockLength;
                }
            }
        }
        public static void MarkAsEmpty(uint firstPageID, Engine engine)
        {
            DataPage dataPage = PageFactory.GetDataPage(firstPageID, engine.Reader, true);
            uint lastPageID = uint.MaxValue;
            var cont = true;

            while (cont)
            {
                dataPage.IsEmpty = true;

                PageFactory.WriteToFile(dataPage, engine.Writer);

                if (dataPage.NextPageID != uint.MaxValue)
                {
                    lastPageID = dataPage.NextPageID;
                    dataPage = PageFactory.GetDataPage(lastPageID, engine.Reader, true);
                }
                else
                {
                    cont = false;
                }
            }

            // Fix header to correct pointer
            if (engine.Header.FreeDataPageID == uint.MaxValue) // No free pages
            {
                engine.Header.FreeDataPageID = firstPageID;
                engine.Header.LastFreeDataPageID = lastPageID == uint.MaxValue ? firstPageID : lastPageID;
            }
            else
            {
                // Take the last statment available
                var lastPage = PageFactory.GetDataPage(engine.Header.LastFreeDataPageID, engine.Reader, true);

                // Point this last statent to first of next one
                if (lastPage.NextPageID != uint.MaxValue || !lastPage.IsEmpty) // This is never to happend!!
                    throw new FileDBException("The page is not empty");

                // Update this last page to first new empty page
                lastPage.NextPageID = firstPageID;

                // Save on disk this update
                PageFactory.WriteToFile(lastPage, engine.Writer);

                // Point header to the new empty page
                engine.Header.LastFreeDataPageID = lastPageID == uint.MaxValue ? firstPageID : lastPageID;
            }
        }

    }
}
