using Microsoft.AspNetCore.Mvc;
using TestProject.Domain.Common;

namespace TestProject.WebApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleFailure(Result result)
    {
        var statusCode = result.Error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(
            statusCode: statusCode,
            title: result.Error.Code,
            detail: result.Error.Message);
    }

    protected IActionResult ToActionResult<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : HandleFailure(result);

    protected IActionResult ToActionResult(Result result) =>
        result.IsSuccess ? NoContent() : HandleFailure(result);
}
