//MIT 2014,WinterDev

using System;
namespace OpenTK
{
#if NETSTANDARD1_6
    public class Serializable : Attribute
    {

    }

#endif
    public class XmlIgnoreAttribute : Attribute
    {
    }
}

#if NETSTANDARD1_6
namespace System.Security
{
    public class SuppressUnmanagedCodeSecurity : Attribute { }
}
namespace System.Runtime.Serialization
{
    public interface ISerializable { }

}
namespace System.Threading
{
    public class Thread
    {
    }
}

#endif