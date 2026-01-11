using Microsoft.AspNetCore.Mvc;
using OsService.Domain.ResultPattern;

namespace OsService.ApiService.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller,
        Func<T, IActionResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            if (onSuccess is not null)
                return onSuccess(result.Data);

            return controller.Ok(result.Data);
        }

        var error = result.Error;

        var body = new
        {
            code = error.Code,
            message = error.Message
        };

        return controller.StatusCode((int)error.StatusCode, body);
    }
}


//public static IActionResult ToActionResult<T>(
//    this Result<T> result,
//    ControllerBase controller,
//    Func<T, IActionResult>? onSuccess = null)
//    {
//        if (result.IsSuccess)
//        {
//            var body = new
//            {
//                isSuccess = true,
//                data = result.Data
//            };

//            if (onSuccess is not null)
//                return onSuccess(result.Data);

//            return controller.Ok(body);
//        }

//        var error = result.Error;

//        var errorBody = new
//        {
//            isSuccess = false,
//            error = new
//            {
//                code = error.Code,
//                message = error.Message
//            }
//        };

//        return controller.StatusCode((int)error.StatusCode, errorBody);
//    }
