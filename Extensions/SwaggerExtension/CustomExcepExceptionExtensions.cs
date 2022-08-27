using InBranchNotification.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swagger.Extension
{
    public class CustomExcepExceptionExtensions
    {

        private readonly RequestDelegate _next;

        public CustomExcepExceptionExtensions(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var objectResponse = new ObjectResponse();
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                 

                string msg = error.Message;
                switch (msg)
                {
                    case var s when msg.Contains("401"):
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;

                        break;
                    case var s when msg.Contains("400"):
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                objectResponse.Error = new[] { error?.Message };
                var result = JsonSerializer.Serialize(objectResponse);


                await response.WriteAsync(result);
            }

            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                switch (context.Response.StatusCode)
                {
                    case 401:
                        objectResponse.Error = new[] { context.Response.StatusCode.ToString(), "Your are not authourized" };
                        break;
                    case 400:
                        objectResponse.Error = new[] { context.Response.StatusCode.ToString(), "you issued a bad request" };
                        break;
                    default:
                        objectResponse.Error = new[] { context.Response.StatusCode.ToString(), "An error occured" };
                        break;
                }



                var json = JsonSerializer.Serialize(objectResponse);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
