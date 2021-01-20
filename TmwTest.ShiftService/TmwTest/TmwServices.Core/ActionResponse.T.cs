namespace TmwServices.Core
{
    using System;

    public class ActionResponse<T> : ActionResponse
    {
        private readonly T _value;

        public ActionResponse(T value) : base(ActionState.Success)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                if (!this.IsSuccess)
                {
                    throw new InvalidOperationException(@"ActionResponse object is not in the Success state.");
                }

                return _value;
            }
        }
    }
}
