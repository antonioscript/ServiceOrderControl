using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OsService.ApiService.Extensions;

/// <summary>
/// Manipula exceções não tratadas no pipeline HTTP e converte em uma resposta
/// padronizada no formato <see cref="ProblemDetails"/>.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Cria uma nova instância de <see cref="GlobalExceptionHandler"/>.
    /// </summary>
    /// <param name="logger">Instância de logger utilizada para registrar as exceções.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tenta tratar uma exceção não tratada ocorrida durante o processamento da requisição HTTP.
    /// </summary>
    /// <param name="httpContext">O contexto HTTP atual.</param>
    /// <param name="exception">A exceção disparada.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>
    /// <c>true</c> se a exceção foi tratada e uma resposta foi escrita;
    /// caso contrário, <c>false</c>.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "Unhandled exception. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
            traceId,
            httpContext.Request.Path,
            httpContext.Request.Method);

        var problem = new ProblemDetails
        {
            Title = "Erro inesperado",
            Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
            Status = (int)HttpStatusCode.InternalServerError,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Type = "https://httpstatuses.com/500"
        };

        problem.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = problem.Status!.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
