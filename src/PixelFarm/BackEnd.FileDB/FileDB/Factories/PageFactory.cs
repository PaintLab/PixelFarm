//MIT, 2015, Mauricio David
using System.IO;

namespace Numeria.IO
{
    internal class PageFactory
    {
        #region Read/Write Index Page

        public static void ReadFromFile(IndexPage indexPage, BinaryReader reader)
        {
            // Seek the stream to the first byte on page
            long initPos = reader.SetReadPos(Header.FILE_START_HEADER_SIZE + (indexPage.PageID * BasePage.PAGE_SIZE));

            if (reader.ReadByte() != (byte)PageType.Index)
                throw new FileDBException("PageID {0} is not a Index Page", indexPage.PageID);

            indexPage.NextPageID = reader.ReadUInt32();
            indexPage.SetUsedNodeCount(reader.ReadByte());
            // Seek the stream to end of header data page
            reader.SetReadPos(initPos + IndexPage.INDEX_HEADER_SIZE);

            //in this version 
            //each node uses buffer= 16+1+1+4+1+4+4+8+4+2+36 =81 
            //IndexPage.NODES_PER_PAGE = 50;
            //so it use 81*50 = 4050
            //IndexPage.INDEX_HEADER_SIZE =46
            //so => 4050 +46 = 4096 
            //and each page has BasePage.PAGE_SIZE = 4096 => matched

            for (int i = 0; i <= indexPage.UsedNodeCount; i++)
            {
                var node = indexPage.Nodes[i];

                node.ID = reader.ReadGuid(); //16

                node.IsDeleted = reader.ReadBoolean(); //1 

                node.Right.Index = reader.ReadByte(); //1 
                node.Right.PageID = reader.ReadUInt32(); //4
                node.Left.Index = reader.ReadByte(); //1 
                node.Left.PageID = reader.ReadUInt32();//4

                node.DataPageID = reader.ReadUInt32();//4

                node.FileMetaDataLength = reader.ReadUInt16();//2
                node.FileLength = reader.ReadUInt32();//4

                int filenameCount = reader.ReadByte(); //1
                if (filenameCount > IndexNode.FILENAME_SIZE)
                {
                    node.FileUrl = reader.ReadUtf8String(IndexNode.FILENAME_SIZE);
                }
                else
                {
                    string filename = reader.ReadUtf8String(IndexNode.FILENAME_SIZE);
                    node.FileUrl = filename.Substring(0, filenameCount);
                }
            }
        }

        public static void WriteToFile(IndexPage indexPage, BinaryWriter writer)
        {
            // Seek the stream to the fist byte on page
            long initPos = writer.SetWritePos(Header.FILE_START_HEADER_SIZE + (indexPage.PageID * BasePage.PAGE_SIZE));

            // Write page header 
            writer.Write((byte)indexPage.Type);
            writer.Write(indexPage.NextPageID);
            writer.Write(indexPage.UsedNodeCount);

            // Seek the stream to end of header index page
            writer.SetWritePos(initPos + IndexPage.INDEX_HEADER_SIZE); //46

            for (int i = 0; i <= indexPage.UsedNodeCount; i++)
            {
                var node = indexPage.Nodes[i];

                writer.Write(node.ID); //16

                writer.Write(node.IsDeleted); //1

                writer.Write(node.Right.Index); //1
                writer.Write(node.Right.PageID);  //4
                writer.Write(node.Left.Index);//1
                writer.Write(node.Left.PageID); //4

                writer.Write(node.DataPageID); //4

                writer.Write(node.FileMetaDataLength); //2
                writer.Write(node.FileLength); //4

                byte[] fileUrlBytes = System.Text.Encoding.UTF8.GetBytes(node.FileUrl);
                if (fileUrlBytes.Length >= ushort.MaxValue)
                {
                    throw new FileDBException("filename is too long!");
                }


                int diff = fileUrlBytes.Length - IndexNode.FILENAME_SIZE;
                if (diff >= 0)
                {
                    //limit 
                    writer.Write((byte)(IndexNode.FILENAME_SIZE + 1)); //1
                    writer.Write(fileUrlBytes, 0, IndexNode.FILENAME_SIZE);
                }
                else
                {
                    writer.Write((byte)(fileUrlBytes.Length));
                    writer.Write(fileUrlBytes);
                    //diff                     
                    //write padding
                    writer.Write(new byte[-diff]);
                }
            }

        }

        #endregion

        #region Read/Write Data Page

        public static void ReadFromFile(DataPage dataPage, BinaryReader reader, bool onlyHeader)
        {
            // Seek the stream on first byte from data page
            long initPos = reader.SetReadPos(Header.FILE_START_HEADER_SIZE + (dataPage.PageID * BasePage.PAGE_SIZE));

            if (reader.ReadByte() != (byte)PageType.Data)
                throw new FileDBException("PageID {0} is not a Data Page", dataPage.PageID);

            dataPage.NextPageID = reader.ReadUInt32();
            dataPage.IsEmpty = reader.ReadBoolean();
            dataPage.DataBlockLength = reader.ReadInt16();

            // If page is empty or onlyHeader parameter, I don't read data content
            if (!dataPage.IsEmpty && !onlyHeader)
            {
                // Seek the stream at the end of page header
                reader.SetReadPos(initPos + DataPage.DATA_HEADER_SIZE);

                // Read all bytes from page
                dataPage.DataBlock = reader.ReadBytes(dataPage.DataBlockLength);
            }
        }

        public static void WriteToFile(DataPage dataPage, BinaryWriter writer)
        {
            // Seek the stream on first byte from data page
            long initPos = writer.SetWritePos(Header.FILE_START_HEADER_SIZE + (dataPage.PageID * BasePage.PAGE_SIZE));

            // Write data page header
            writer.Write((byte)dataPage.Type);
            writer.Write(dataPage.NextPageID);
            writer.Write(dataPage.IsEmpty);
            writer.Write(dataPage.DataBlockLength);

            // I will only save data content if the page is not empty
            if (!dataPage.IsEmpty)
            {
                // Seek the stream at the end of page header
                writer.SetWritePos(initPos + DataPage.DATA_HEADER_SIZE);

                writer.Write(dataPage.DataBlock, 0, (int)dataPage.DataBlockLength);
            }
        }

        #endregion

        #region Get Pages from File

        public static IndexPage GetIndexPage(uint pageID, BinaryReader reader)
        {
            var indexPage = new IndexPage(pageID);
            ReadFromFile(indexPage, reader);
            return indexPage;
        }

        public static DataPage GetDataPage(uint pageID, BinaryReader reader, bool onlyHeader)
        {
            var dataPage = new DataPage(pageID);
            ReadFromFile(dataPage, reader, onlyHeader);
            return dataPage;
        }

        public static BasePage GetBasePage(uint pageID, BinaryReader reader)
        {
            // Seek the stream at begin of page
            long initPos = reader.SetReadPos(Header.FILE_START_HEADER_SIZE + (pageID * BasePage.PAGE_SIZE));

            if (reader.ReadByte() == (byte)PageType.Index)
                return GetIndexPage(pageID, reader);
            else
                return GetDataPage(pageID, reader, true);
        }

        #endregion

    }
}
