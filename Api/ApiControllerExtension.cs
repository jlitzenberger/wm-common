using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace WM.Common.Api
{
    public static class ApiControllerExtension
    {
        //public class ValidationException : IHttpActionResult
        //{
        //    public HttpStatusCode Status { get; set; }
        //    public string Message { get; set; }
        //    public int Code { get; set; }
        //    public string MoreInfo { get; set; }

        //    public HttpRequestMessage Request { get; private set; }

        //    public ValidationException(HttpRequestMessage request, string message)
        //    {
        //        this.Request = request;
        //        this.Message = message;
        //    }

        //    public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        //    {
        //        return System.Threading.Tasks.Task.FromResult(ExecuteResult());
        //    }

        //    public HttpResponseMessage ExecuteResult()
        //    {
        //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        //        response.Content = new StringContent(Message);
        //        response.RequestMessage = Request;
        //        return response;
        //    }
        //}

        //public static class ApiControllerExtension
        //{
        //    public static ValidationException BadRequest(ApiController controller, string message)
        //    {
        //        return new ValidationException(controller.Request, message);
        //    }
        //}
        
        public static HttpError CreateHttpError(Exception ex)
        {
            HttpError obj = new HttpError();

            obj.Message = ex.Message;
            obj.StackTrace = ex.StackTrace;

            return obj;
        }
    }
}