//MIT, 2015, Mauricio David
namespace Numeria.IO
{
    internal enum PageType
    {
        /// <summary>
        /// Data = 1
        /// </summary>
        Data = 1, 
        /// <summary>
        /// Index = 2
        /// </summary>
        Index = 2
    }

    internal abstract class BasePage
    {
        public const int PAGE_SIZE = 4096;

        public uint PageID { get; set; }
        public abstract PageType Type { get; }
        public uint NextPageID { get; set; }
    }
}
