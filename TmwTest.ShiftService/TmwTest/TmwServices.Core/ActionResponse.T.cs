using System.Net;

namespace TmwServices.Core
{
    using System;

    public class ActionResponse<T> : ActionResponse
    {
        private readonly T _value;

        public ActionResponse(T value, HttpStatusCode successStatus = HttpStatusCode.OK) : base(successStatus)
        {
            _value = value;
        }

        public ActionResponse(HttpStatusCode status, string errorMessage) : base(status, errorMessage)
        {
            
        }

        public T Value
        {
            get
            {
                if (!this.IsSuccess)
                {
                    throw new InvalidOperationException(@"ActionResponse object is not in the Success status.");
                }

                return _value;
            }
        }
    }
}
