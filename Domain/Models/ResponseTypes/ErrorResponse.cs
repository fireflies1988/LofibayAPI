using Domain.Enums;

namespace Domain.Models.ResponseTypes
{
    /// <summary>
    /// An error occurred in processing the request, i.e. an exception was thrown.
    /// </summary>
    public class ErrorResponse<T> : BaseResponse<T>
    {
        public override string? Status => StatusTypes.Error;
    }

    public class ErrorReponse : BaseResponse<object>
    {
        public override string? Status => StatusTypes.Error;
    }
}
