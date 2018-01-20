//MIT, 2015, Mauricio David
using System;
 

namespace Numeria.IO
{
    public class EntryInfo
    {
        public Guid ID { get; private set; }
        public string FileUrl { get; internal set; }  //file url 
        public uint FileLength { get; internal set; }
        public ushort FileMetadataLength { get; internal set; }

        //metadata
        public DateTime FileDateTime { get; internal set; }
        internal bool HasLongFileName { get; private set; }

        internal EntryInfo(string fileName)
            : this(fileName, Guid.NewGuid(), DateTime.Now) { }

        internal EntryInfo(string fileName, Guid guid, DateTime datetime)
        {
            //this version filename must not longer than FILENAME_SIZE                      

            ID = Guid.NewGuid();
            FileUrl = fileName;
            FileLength = 0;
            FileMetadataLength = 0;
            FileDateTime = datetime;

            this.HasLongFileName = fileName.Length > IndexNode.FILENAME_SIZE;
            if (this.HasLongFileName)
            {

            }
        }
        internal EntryInfo(IndexNode node)
        {
            ID = node.ID;
            FileUrl = node.FileUrl;
            FileLength = node.FileLength;
            FileMetadataLength = node.FileMetaDataLength;
            //---------------------
            //no datetime or other metadata here             
            //---------------------
        }
        public override string ToString()
        {
            return this.FileUrl;
        }
    }
}
