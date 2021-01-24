namespace TmwServices.Core
{
    using System;
    using System.Net;

    public class ActionResponse<T> : ActionResponse
    {
        private readonly T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResponse{T}"/> class for SUCCESS result.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="successStatus">The success status.</param>
        public ActionResponse(T value, HttpStatusCode successStatus = HttpStatusCode.OK) : base(successStatus)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResponse{T}"/> class for FAILED result.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="errorMessage">The error message.</param>
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
