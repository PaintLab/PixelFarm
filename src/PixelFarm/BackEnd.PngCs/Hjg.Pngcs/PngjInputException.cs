//Apache2, 2012, Hernan J Gonzalez, https://github.com/leonbloy/pngcs
namespace Hjg.Pngcs
{

    using System;

    /// <summary>
    /// Exception associated with input (reading) operations
    /// </summary>
    [Serializable]
    public class PngjInputException : PngjException
    {
        private const long serialVersionUID = 1L;

        public PngjInputException(String message, Exception cause)
            : base(message, cause)
        {
        }

        public PngjInputException(String message)
            : base(message)
        {
        }

        public PngjInputException(Exception cause)
            : base(cause)
        {
        }
    }
}
