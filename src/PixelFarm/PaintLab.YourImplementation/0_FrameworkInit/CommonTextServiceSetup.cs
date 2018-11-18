//MIT, 2017-present, WinterDev

using System.IO;
using System.Collections.Generic;
using Typography.FontManagement;

namespace YourImplementation
{


    public static class CommonTextServiceSetup
    {
        static bool s_isInit;
        static Typography.FontManagement.InstalledTypefaceCollection s_intalledTypefaces;


        public static IInstalledTypefaceProvider FontLoader
        {
            get
            {
                return s_intalledTypefaces;
            }
        }
        public static void SetupDefaultValues()
        {
            //--------
            //This is optional if you don't use Typography Text Service.            
            //-------- 
            if (s_isInit)
            {
                return;
            }

            s_isInit = true;
            s_intalledTypefaces = new InstalledTypefaceCollection();
            s_intalledTypefaces.SetFontNameDuplicatedHandler((existing, newone) =>
            {
                return FontNameDuplicatedDecision.Skip;
            });
            s_intalledTypefaces.LoadSystemFonts();
            s_intalledTypefaces.SetFontNotFoundHandler((collection, fontName, subFam) =>
            {
                //This is application specific ***
                //
                switch (fontName.ToUpper())
                {
                    default:
                        {

                        }
                        break;
                    case "SANS-SERIF":
                        {
                            //temp fix
                            InstalledTypeface ss = collection.GetInstalledTypeface("Microsoft Sans Serif", "REGULAR");
                            if (ss != null)
                            {
                                return ss;
                            }
                        }
                        break;
                    case "SERIF":
                        {
                            //temp fix
                            InstalledTypeface ss = collection.GetInstalledTypeface("Palatino linotype", "REGULAR");
                            if (ss != null)
                            {
                                return ss;
                            }
                        }
                        break;
                    case "TAHOMA":
                        {
                            switch (subFam)
                            {
                                case "ITALIC":
                                    {
                                        InstalledTypeface anotherCandidate = collection.GetInstalledTypeface(fontName, "NORMAL");
                                        if (anotherCandidate != null)
                                        {
                                            return anotherCandidate;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case "MONOSPACE":
                        //use Courier New
                        return collection.GetInstalledTypeface("Courier New", subFam);
                    case "HELVETICA":
                        return collection.GetInstalledTypeface("Arial", subFam);
                }
                return null;
            });
            //--------------------
            InstalledTypefaceCollection.SetAsSharedTypefaceCollection(s_intalledTypefaces);

        }



    }


}