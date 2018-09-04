using System.Collections.Generic;

namespace EDrinks.Common
{
    public class HandlerResult
    {
        public ResultCode ResultCode { get; set; }
    }

    public class HandlerResult<T> : HandlerResult
    {
        public T Payload { get; set; }
        
        public List<string> ErrorMessages { get; set; }
    }
}