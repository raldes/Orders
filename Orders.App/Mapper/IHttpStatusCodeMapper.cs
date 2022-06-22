using System;
using System.Net;

namespace Orders.App.Mappers
{
    public interface IHttpStatusCodeMapper
    {
        HttpStatusCode GetHttpStatusCode(Exception exception);
    }
}
