//Apache2, 2012, Hernan J Gonzalez, https://github.com/leonbloy/pngcs
namespace Hjg.Pngcs
{

    using System;
    /// <summary>
    /// Gral exception class for PNGCS library
    /// </summary>
    [Serializable]
    public class PngjException : Exception
    {
        private const long serialVersionUID = 1L;

        public PngjException(String message, Exception cause)
            : base(message, cause)
        {
        }

        public PngjException(String message)
            : base(message)
        {
        }

        public PngjException(Exception cause)
            : base(cause.Message, cause)
        {
        }
    }
}
