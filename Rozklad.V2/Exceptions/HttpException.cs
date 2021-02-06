using System;

namespace Rozklad.V2.Exceptions
{
    public class HttpException : Exception
    {
        public HttpException(string errorMessage): base(errorMessage) 
        {
            
        }
    }
}