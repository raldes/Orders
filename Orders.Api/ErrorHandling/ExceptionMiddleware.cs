using Microsoft.Data.SqlClient;
using Npgsql;
using Orders.App.Mappers;
using Orders.Domain.Exceptions;
using System.Net;

namespace Orders.Api.ErrorHandling
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly IHttpStatusCodeMapper _httpStatusCodeMapper;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IHttpStatusCodeMapper httpStatusCodeMapper = null)
        {
            _loggerFactory = loggerFactory;
            _next = next;
            _logger = _loggerFactory.CreateLogger("Values");
            _httpStatusCodeMapper = httpStatusCodeMapper;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var errorDetail = new ErrorDetail();

            switch (exception)
            {
                case SqlException:

                    var exc = exception as SqlException;
                    errorDetail.StatusCode = exc.Number;
                    errorDetail.Message = "Sql exception. " + exc.Message + GetInnerExceptionsMessage(exc);
                    break;

                case PostgresException:
                    var pexc = exception as PostgresException;
                    errorDetail.StatusCode = pexc.ErrorCode;
                    errorDetail.Message = "Postgres exception. " + pexc.Message + ". Details: " + pexc.Detail;
                    break;

                case ExceptionBase:
                    var cexc = exception as ExceptionBase;
                    errorDetail.StatusCode = (int)_httpStatusCodeMapper.GetHttpStatusCode(cexc);
                    errorDetail.Message = "Custom exception. " + exception.Message; 
                    break;

                default:
                    // unhandled error
                    errorDetail.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorDetail.Message = "Internal Server Error. " + GetInnerExceptionsMessage(exception);
                    break;
            }

            _logger.LogError($"Code: {errorDetail.StatusCode} Detail: {errorDetail.Message}");

            context.Response.StatusCode = errorDetail.StatusCode;
            await context.Response.WriteAsync(errorDetail.ToString());
        }

        string GetInnerExceptionsMessage(Exception ex)
        {
            string Details = ex.Message;

            ex = ex.InnerException;

            while (ex != null)
            {
                Details += "Inner Exception: " + ex.Message;

                if (ex is PostgresException)
                {
                    var pexc = ex as PostgresException;
                    Details += " Detail:" + pexc.Detail;
                }

                ex = ex.InnerException;

            }

            return Details;
        }
    }
}
