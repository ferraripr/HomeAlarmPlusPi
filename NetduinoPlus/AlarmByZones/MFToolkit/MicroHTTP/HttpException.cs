using System;

namespace MFToolkit.Net.Web
{
    public class HttpException : Exception
    {
        private string _message;
        private HttpStatusCode _error = HttpStatusCode.InternalServerError;

        public HttpException()
        {
        }

        public HttpException(HttpStatusCode error)
        {
            _error = error;
        }

        public HttpException(HttpStatusCode error, string message)
            : this(error)
        {
            _message = message;
        }

        public override string Message
        {
            get
            {
                if (_message != null && _message.Length > 0)
                    return _message;

                return null;
            }
        }

        public HttpStatusCode Code
        {
            get { return _error; }
        }
    }
}
