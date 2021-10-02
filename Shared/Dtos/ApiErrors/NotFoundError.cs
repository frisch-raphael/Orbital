using System.Net;
using Newtonsoft.Json;

namespace Shared
{
    namespace Api.ApiErrors
    {
        public partial class NotFoundError : ApiError
        {
            [JsonConstructor]
            public NotFoundError()
                : base(404, HttpStatusCode.NotFound.ToString())
            {
            }

            [JsonConstructor]

            public NotFoundError(string message)
                : base(404, HttpStatusCode.NotFound.ToString(), message)
            {
            }
        }
    }

}
