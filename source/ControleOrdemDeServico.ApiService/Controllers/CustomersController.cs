using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.Application.V1.Features.Customers.CreateCustomer;
using OsService.Application.V1.Features.Customers.GetCustomerById;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("v1/customers")]
public sealed class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return Ok(id);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetCustomerByIdResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);

        //TODO: Não vai precisar disso, depois rever os retornos
        if (result is null)
            return NotFound();

        return Ok(result);
    }
}