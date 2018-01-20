//MIT, 2018, WinterDev


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

}