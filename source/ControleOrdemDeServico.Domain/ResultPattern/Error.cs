using System.Net;

namespace OsService.Domain.ResultPattern;

public sealed record Error(string Code, string Message, HttpStatusCode StatusCode)
{
    public static Error Validation(string code, string message) =>
        new(code, message, HttpStatusCode.BadRequest);

    public static Error Conflict(string code, string message) =>
        new(code, message, HttpStatusCode.Conflict);

    public static Error NotFound(string code, string message) =>
        new(code, message, HttpStatusCode.NotFound);

    public static Error Unexpected(string code, string message) =>
        new(code, message, HttpStatusCode.InternalServerError);

    public static readonly Error None =
        new("None", string.Empty, HttpStatusCode.OK);
}