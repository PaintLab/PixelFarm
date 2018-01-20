//MIT, 2018, WinterDev

using System.IO;
namespace PixelFarm.Platforms
{
    public abstract class StorageServiceProvider
    {
        public abstract bool DataExists(string dataName);
        public abstract void SaveData(string dataName, byte[] content);
        public abstract byte[] ReadData(string dataName);
        public Stream ReadDataStream(string dataName)
        {
            byte[] data = ReadData(dataName);
            return new MemoryStream(data);
        }
    }

    public static class StorageService
    {
        static StorageServiceProvider s_provider;
        public static void RegisterProvider(StorageServiceProvider provider)
        {
            s_provider = provider;
        }
        public static StorageServiceProvider Provider
        {
            get { return s_provider; }
        }
    }
}