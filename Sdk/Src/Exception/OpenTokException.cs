using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTokSDK.Exceptions
{
    public class OpenTokException : Exception
    {
        private int code;
        private string message;

        public OpenTokException()
        {
        }

        public OpenTokException(string message)
            : base(message)
        {
            this.message = message;
        }

        public OpenTokException(string message, int code)
            : base(message)
        {
            this.message = message;
            this.code = code;
        }

        //GGB override Message property
        public string GetMessage()
        {
            return message;
        }

        public int GetErrorCode()
        {
            return code;
        }
    }

    public class OpenTokInvalidArgumentException : OpenTokException
    {
        public OpenTokInvalidArgumentException(string message)
            : base(message, 400)
        {
        }
    }

    public class OpenTokRequestException : OpenTokException
    {
        public OpenTokRequestException(string message, int code)
            : base(message, code)
        {
        }
    }

    public class OpenTokSessionNotFoundException : OpenTokException
    {
        public OpenTokSessionNotFoundException(string message)
            : base(message, 404)
        {
        }
    }
    
}
