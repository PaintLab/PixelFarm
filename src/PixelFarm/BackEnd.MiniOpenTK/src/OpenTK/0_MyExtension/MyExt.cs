//MIT, 2017-present, WinterDev

namespace OpenTK.Graphics
{
    public static class PlatformAddressPortal
    {
        public static GraphicsContext.GetAddressDelegate GetAddressDelegate;
    }
    
 
}


#if !MINIMAL
namespace OpenTK
{
    class XmlIgnoreAttribute : System.Attribute { }
}
#endif
