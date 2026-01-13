using Microsoft.AspNetCore.Mvc;
using OsService.Domain.ResultPattern;

namespace OsService.ApiService.Extensions;

/// <summary>
/// Métodos de extensão para converter instâncias de retorno
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Conversão de status
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="controller"></param>
    /// <param name="onSuccess"></param>
    /// <returns></returns>
    public static IActionResult ToActionResult<T>(
    this Result<T> result,
    ControllerBase controller,
    Func<T, IActionResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            var body = new
            {
                isSuccess = true,
                data = result.Data
            };

            if (onSuccess is not null)
                return onSuccess(result.Data);

            return controller.Ok(body);
        }

        var error = result.Error;

        var errorBody = new
        {
            isSuccess = false,
            error = new
            {
                code = error.Code,
                message = error.Message
            }
        };

        return controller.StatusCode((int)error.StatusCode, errorBody);
    }
}



