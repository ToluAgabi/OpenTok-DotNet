using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTokSDK.Exceptions
{
    public class OpenTokException : Exception
    {
        private Exception exception;
        private string message;

        public OpenTokException()
        {
        }

        public OpenTokException(string message)
            : base(message)
        {
            this.message = message;
        }

        public OpenTokException(string message, Exception exception)
            : base(message)
        {
            this.message = message;
            this.exception = exception;
        }

        //GGB override Message property
        public string GetMessage()
        {
            return message;
        }

        public Exception GetException()
        {
            return exception;
        }
    }

    public class OpenTokArgumentException : OpenTokException
    {
        public OpenTokArgumentException(string message)
            : base(message)
        {
        }
    }

    public class OpenTokWebException : OpenTokException
    {
        public OpenTokWebException(string message, Exception exception)
            : base(message, exception)
        {
        }

        public OpenTokWebException(string message)
            : base(message)
        {
        }
    }


}
