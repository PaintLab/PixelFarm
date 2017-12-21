//MIT 2014,WinterDev

using System;
namespace OpenTK
{
#if NETSTANDARD1_6 || NETCOREAPP1_1
    public class SerializableAttribute : Attribute
    {

    }

#endif
    public class XmlIgnoreAttribute : Attribute
    {
    }
}

#if NETSTANDARD1_6 || NETCOREAPP1_1
namespace System.Security
{
    public class SuppressUnmanagedCodeSecurity : Attribute { }
}
namespace System.Runtime.Serialization
{
    public interface ISerializable { }

}
 

#endif