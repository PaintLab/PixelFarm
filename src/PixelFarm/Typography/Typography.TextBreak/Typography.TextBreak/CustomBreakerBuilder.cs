//MIT, 2016-2017, WinterDev
// some code from icu-project
// © 2016 and later: Unicode, Inc. and others.
// License & terms of use: http://www.unicode.org/copyright.html#License


using System.IO;
namespace Typography.TextBreak
{
    public interface IIcuDataProvider
    {
        Stream GetDataStream(string strmUrl);
    }
    public static class CustomBreakerBuilder
    {
        static ThaiDictionaryBreakingEngine thaiDicBreakingEngine;
        static LaoDictionaryBreakingEngine laoDicBreakingEngine;
        static bool isInit;
        static IIcuDataProvider s_dataProvider;
        public static void Setup(IIcuDataProvider dataProvider)
        {
            if (isInit) return;

            s_dataProvider = dataProvider;
            InitAllDics();

            isInit = true;
        }
        static void InitAllDics()
        {
             

            if (thaiDicBreakingEngine == null)
            {
                var customDic = new CustomDic();
                thaiDicBreakingEngine = new ThaiDictionaryBreakingEngine();
                thaiDicBreakingEngine.SetDictionaryData(customDic);//add customdic to the breaker
                customDic.SetCharRange(thaiDicBreakingEngine.FirstUnicodeChar, thaiDicBreakingEngine.LastUnicodeChar);

                using (Stream data = s_dataProvider.GetDataStream("thaidict.txt"))
                {
                    customDic.LoadFromDataStream(data);
                }
            }
            if (laoDicBreakingEngine == null)
            {
                var customDic = new CustomDic();
                laoDicBreakingEngine = new LaoDictionaryBreakingEngine();
                laoDicBreakingEngine.SetDictionaryData(customDic);//add customdic to the breaker
                customDic.SetCharRange(laoDicBreakingEngine.FirstUnicodeChar, laoDicBreakingEngine.LastUnicodeChar);
                using (Stream data = s_dataProvider.GetDataStream("laodict.txt"))
                {
                    customDic.LoadFromDataStream(data);
                }
            }

        }
         
        public static CustomBreaker NewCustomBreaker()
        {
            if (!isInit)
            {
                InitAllDics();
                isInit = true;
            }
            var breaker = new CustomBreaker();
            breaker.AddBreakingEngine(thaiDicBreakingEngine);
            breaker.AddBreakingEngine(laoDicBreakingEngine);
            return breaker;
        }
    }
}