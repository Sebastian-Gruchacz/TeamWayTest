namespace TmwServices.Core
{
    /// <summary>
    /// Base decorated operation state
    /// </summary>
    public class ActionResponse
    {
        private readonly ActionState _state;
        private readonly string _errorMessage;

        protected ActionResponse(ActionState state)
        {
            _state = state;
            _errorMessage = null;
        }

        protected ActionResponse(ActionState state, string errorMessage)
        {
            _state = state;
            _errorMessage = errorMessage;
        }

        /// <summary>
        /// Returns TRUE when ActionResponse is in Success state
        /// </summary>
        public bool IsSuccess => _state == ActionState.Success;

        /// <summary>
        /// Error message for failed result
        /// </summary>
        public string ErrorMessage => _errorMessage;
    }
}