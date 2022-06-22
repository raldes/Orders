using Orders.Domain.Exceptions;
using System.Net;

namespace Orders.App.Mappers
{
    public class HttpStatusCodeMapper : IHttpStatusCodeMapper
    {
        private readonly Dictionary<Type, HttpStatusCode> _exceptionsMappings;

        public HttpStatusCodeMapper()
        {
            _exceptionsMappings = GetMappings();
        }

       public HttpStatusCode GetHttpStatusCode(Exception exception)
        {
            var result = _exceptionsMappings.TryGetValue(exception.GetType(), out HttpStatusCode outStatusCode);

            if(!result) return HttpStatusCode.InternalServerError;

            return outStatusCode;
        }
        
        private Dictionary<Type, HttpStatusCode> GetMappings()
        {
            var exceptionsMappings = new Dictionary<Type, HttpStatusCode>
            {
                {typeof(CreateOrderDomainException), HttpStatusCode.InsufficientStorage },
            };
            return exceptionsMappings;
        }
    }
}


