namespace TmwServices.Core
{
    using System;
    using System.Net;

    /// <summary>
    /// Base decorated operation status
    /// </summary>
    /// <remarks>To be used when one would like to return a bit more than simple success / failure response, but would not like to throw exceptions.</remarks>
    public class ActionResponse
    {
        private readonly HttpStatusCode _status;
        private readonly string _errorMessage;

        protected ActionResponse(HttpStatusCode status)
        {
            _status = status;

            if (!IsSuccess)
            {
                throw new ArgumentException($"Cannot specify non-success Status with constructor without [{nameof(ErrorMessage)}] ", nameof(status));
            }

            _errorMessage = null;
        }

        protected ActionResponse(HttpStatusCode status, string errorMessage)
        {
            _status = status;

            if (IsSuccess)
            {
                throw new ArgumentException($"Cannot provide [{nameof(errorMessage)}] when status is \"{HttpStatusCode.OK}\".", nameof(errorMessage));
            }

            _errorMessage = errorMessage;
        }

        /// <summary>
        /// Returns TRUE when ActionResponse is in Success status
        /// </summary>
        public bool IsSuccess => (int) _status / 100 == 2;

        /// <summary>
        /// Error message for failed result
        /// </summary>
        public string ErrorMessage => _errorMessage;

        /// <summary>
        /// Gets the success status object.
        /// </summary>
        public static ActionResponse Ok { get; } = new ActionResponse(HttpStatusCode.OK);
    }
}