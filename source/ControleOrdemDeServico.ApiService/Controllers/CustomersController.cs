using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.ApiService.Extensions;
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
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult(this, id => CreatedAtAction(nameof(GetById), new { id }, new { id }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);
        return result.ToActionResult(this);
    }
}