//MIT, 2018, WinterDev

using System.Collections.Generic;
using Numeria.IO;
namespace YourImplementation
{
    class LocalFileStorageProvider : PixelFarm.Platforms.StorageServiceProvider
    {
        public override bool DataExists(string dataName)
        {
            //implement with file
            return System.IO.File.Exists(dataName);
        }
        public override byte[] ReadData(string dataName)
        {
            return System.IO.File.ReadAllBytes(dataName);
        }
        public override void SaveData(string dataName, byte[] content)
        {
            System.IO.File.WriteAllBytes(dataName, content);
        }
    }

    class FileDBStorageProvider : PixelFarm.Platforms.StorageServiceProvider
    {
        //user can implement this with other technology, eg Sqlite.


        FileDB filedb;
        object _filelock = new object();

        Dictionary<string, EntryInfo> _allFiles = new Dictionary<string, EntryInfo>();

        public FileDBStorageProvider(string filename)
        {
            filedb = new FileDB(filename, System.IO.FileAccess.ReadWrite);
            EntryInfo[] entryInfoArr = filedb.ListFiles();

            //
            foreach (EntryInfo en in entryInfoArr)
            {
                //replace with latest datatime
                string fileUrl = en.FileUrl;
                if (!_allFiles.ContainsKey(fileUrl))
                {
                    _allFiles[en.FileUrl] = en;
                }
                else
                {
                    //?
                }
            }
        }
        public override bool DataExists(string dataName)
        {
            //TODO: resolve the dataName to absolute path on the  database
            lock (_filelock)
            {
                return _allFiles.ContainsKey(dataName);
            }
        }
        public override byte[] ReadData(string dataName)
        {
            lock (_filelock)
            {
                EntryInfo entryInfo;
                if (_allFiles.TryGetValue(dataName, out entryInfo))
                {
                    byte[] dataBuffer = null;
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        filedb.ReadContent(entryInfo, ms);
                        dataBuffer = ms.ToArray();
                    }
                    return dataBuffer;
                }

                return null;
            }
        }

        public override void SaveData(string dataName, byte[] content)
        {
            lock (_filelock)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(content))
                {
                    EntryInfo en = filedb.Store(dataName, ms);
                    filedb.Flush();
                    //then replace 
                    _allFiles[dataName] = en;
                }
            }
        }
    }

}