//MIT, 2018-present, WinterDev

using System;
using System.Collections.Generic;
using System.IO;

namespace YourImplementation
{
    public class LocalFileStorageProvider : PixelFarm.Platforms.StorageServiceProvider
    {
        

        readonly string _baseDir;
        public LocalFileStorageProvider(string baseDir, bool disableAbsolutePath = false)
        {
            _baseDir = baseDir;
            DisableAbsolutePath = disableAbsolutePath;
        }
        public string BaseDir => _baseDir;

        public bool DisableAbsolutePath { get; }
        public override string[] GetDataNameList(string dir)
        {
            if (Path.IsPathRooted(dir))
            {
                if (DisableAbsolutePath) return null;
            }
            else
            {
                dir = Path.Combine(_baseDir, dir);
            }
            return System.IO.Directory.GetFiles(dir);
        }
        public override string[] GetDataDirNameList(string dir)
        {
            if (Path.IsPathRooted(dir))
            {
                if (DisableAbsolutePath) return null;
            }
            else
            {
                dir = Path.Combine(_baseDir, dir);
            }
            return System.IO.Directory.GetFiles(dir);
        }
        public override bool DataExists(string dataName)
        {
            //implement with file 

            if (Path.IsPathRooted(dataName))
            {
                if (DisableAbsolutePath) return false;
            }
            else
            {
                dataName = Path.Combine(_baseDir, dataName);
            }

            return System.IO.File.Exists(dataName);
        }
        public override byte[] ReadData(string dataName)
        {

            if (Path.IsPathRooted(dataName))
            {
                if (DisableAbsolutePath) return null;
            }
            else
            {
                dataName = Path.Combine(_baseDir, dataName);
            }

            return System.IO.File.ReadAllBytes(dataName);
        }
        public override void SaveData(string dataName, byte[] content)
        {

            if (Path.IsPathRooted(dataName))
            {
                if (DisableAbsolutePath) return;
            }
            else
            {
                dataName = Path.Combine(_baseDir, dataName);
            }

#if !__MOBILE__
            //TODO: review here, save data on android
            System.IO.File.WriteAllBytes(dataName, content);
#endif
        }
    }
}