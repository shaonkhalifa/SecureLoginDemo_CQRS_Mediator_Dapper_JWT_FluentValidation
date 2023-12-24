using Azure;
using MadiatrProject.Model;
using MediatR;

namespace MadiatrProject.Fillter;

public class ErrorCodeMappingExceptionFilter<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TResponse : ApiResponse
{
    private readonly Dictionary<Type, int> _errorCodeMappings = new Dictionary<Type, int>
{
    { typeof(InvalidOperationException), 400 },

};
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var errorCode = _errorCodeMappings.TryGetValue(ex.GetType(), out int code) ? code : 500;
            TResponse errorResponse = Activator.CreateInstance<TResponse>();


            typeof(TResponse).GetProperty("IsSuccess")?.SetValue(errorResponse, false);
            typeof(TResponse).GetProperty("Message")?.SetValue(errorResponse, ex.Message);
            typeof(TResponse).GetProperty("Code")?.SetValue(errorResponse, errorCode);

            return errorResponse;
            //if (typeof(TResponse) == typeof(ApiResponse))
            //{
            //    object errorResponse = new ApiResponse
            //    {
            //        IsSuccess = false,
            //        Message = ex.Message,
            //        Code = errorCode
            //    };
            //    return (TResponse)errorResponse;
            //}
            //else
            //{
            //    // Handle the case where TResponse is not ApiResponse
            //    // You may need to customize this part based on your actual TResponse type
            //    throw;
            //}
        }
    }
}
