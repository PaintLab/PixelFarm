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

            System.IO.File.WriteAllBytes(dataName, content);
        } 
    } 
}