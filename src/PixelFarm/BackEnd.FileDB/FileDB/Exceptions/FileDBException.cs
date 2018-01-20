//MIT, 2015, Mauricio David
using System;
namespace Numeria.IO
{
    public class FileDBException : ApplicationException
    {
        public FileDBException(string message)
            : base(message)
        {
        }

        public FileDBException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }
    }
}
