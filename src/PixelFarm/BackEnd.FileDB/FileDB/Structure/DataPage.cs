//MIT, 2015, Mauricio David
namespace Numeria.IO
{
    internal class DataPage : BasePage
    {
        public const int DATA_HEADER_SIZE = 8;
        public const int DATA_PER_PAGE = 4088; 
        //so 4088 +8 => 4096 

        public override PageType Type { get { return PageType.Data; } }  //  1 byte

        public bool IsEmpty { get; set; }                                //  1 byte
        public short DataBlockLength { get; set; }                       //  2 bytes

        public byte[] DataBlock { get; set; }

        public DataPage(uint pageID)
        {
            PageID = pageID;
            IsEmpty = true;
            DataBlockLength = 0;
            NextPageID = uint.MaxValue;
            DataBlock = new byte[DataPage.DATA_PER_PAGE];
        }
    }
}
