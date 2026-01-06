using System.Diagnostics.CodeAnalysis;

namespace Titanic.API.Http;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum HttpMethodType
{
    GET,
    POST,
    PUT,
    PATCH,
    DELETE
}