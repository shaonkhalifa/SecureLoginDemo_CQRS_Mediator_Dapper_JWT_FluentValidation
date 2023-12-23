using FluentValidation;
using MadiatrProject.Model;
using Newtonsoft.Json;
using System.Net;

namespace MadiatrProject.Middlewares
{
    public class ExecptionHandlingMiddleware
    {
        private readonly ILogger<ExecptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;
       

        public ExecptionHandlingMiddleware(RequestDelegate next, ILogger<ExecptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (Exception exeption)
            {
                _logger.LogError(exeption, exeption.Message);
                await HandleExceptionAsync(context, exeption);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var re = new ResponseModel();
            switch (ex)
            {
                case ApplicationException aex:
                    if (ex.Message.Contains("Invalid Token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        re.Message = ex.Message;
                        break;
                    }
                    re.StatusCode = (int)HttpStatusCode.BadRequest;
                    re.Message = "Application Exception Occured,please retry after sometime.";
                    re.Description = aex.Message;
                    re.StatusType= " ";
                    break;
                case ArgumentNullException an:
                    re.StatusCode = (int)HttpStatusCode.NotFound;
                    re.Message = "No Content Avaliable";
                    re.Description = an.Message;
                    re.StatusType = " ";
                    break;
                case InvalidOperationException ioe:
                    re.StatusCode = (int)HttpStatusCode.BadRequest;
                    re.Message = "Invalid Oparation";
                    re.Description = ioe.Message;
                    re.StatusType = " ";
                    break;
                case ValidationException bl:
                    re.StatusCode = (int)HttpStatusCode.BadRequest;
                    re.Message += bl.Message;
                    re.Description += bl.Message;
                    re.StatusType = " ";
                    break;
                default:
                    re.StatusCode = (int)HttpStatusCode.InternalServerError;
                    re.Message = "Internal server error!";
                    re.Description = ex.Message;
                    re.StatusType= " ";
                    break;
            }
            _logger.LogError(ex.Message);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(re));
         
            // ResponseModel re=new ResponseModel();
            //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //// Customize the error response as needed
            //var response = new
            //{
            //    error = new
            //    {
            //        message = "An error occurred while processing your request.",
            //        details = ex.Message // You can choose to include more details here
            //    }
            //};

            //return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
       
    }
}
