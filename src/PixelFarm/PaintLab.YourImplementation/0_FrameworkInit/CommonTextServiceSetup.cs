//MIT, 2017-present, WinterDev

using System.IO;
using System.Collections.Generic;
using Typography.FontManagement;

namespace YourImplementation
{


    public static class CommonTextServiceSetup
    {

        static InstalledTypefaceCollection s_intalledTypefaces;

        public static IInstalledTypefaceProvider FontLoader => s_intalledTypefaces;

        public static void SetupDefaultValues()
        {

            if (s_intalledTypefaces != null)
            {
                return;
            }

            s_intalledTypefaces = new InstalledTypefaceCollection();
            s_intalledTypefaces.SetFontNameDuplicatedHandler((existing, newone) => FontNameDuplicatedDecision.Skip);

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


            //if you don't want to load entire system fonts
            //then you can add only specfic font by yourself
            //when the service can' resolve the requested font
            //=> the service will ask at 'SetFontNotFoundHandler'

            s_intalledTypefaces.LoadSystemFonts();
            //--------------------
            InstalledTypefaceCollection.SetAsSharedTypefaceCollection(s_intalledTypefaces);
        }

        public static void SetCustomFontFolder(string customFontFolder)
        {
            s_intalledTypefaces.LoadFontsFromFolder(customFontFolder);
        }


    }


}