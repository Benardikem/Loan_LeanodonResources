using System;

namespace Web.Codes
{
    public class InvalidDividendException : Exception
    {
        public InvalidDividendException()
        : base()
        {
        }

        public InvalidDividendException(string message)
            : base(message)
        {
        }

        public InvalidDividendException(string message, Exception e)
            : base(message, e)
        {
           
        }
    }
}