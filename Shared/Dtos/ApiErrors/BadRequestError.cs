using System.Net;

namespace Shared
{
    namespace Api.ApiErrors
    {
        public class BadRequestError : ApiError
        {
            //private string Field { get; set; }

            public BadRequestError()
                : base(400, HttpStatusCode.BadRequest.ToString())
            {
            }
            public BadRequestError(string message)
                : base(400, HttpStatusCode.BadRequest.ToString(), message)
            {
            }
        }
    }

}
