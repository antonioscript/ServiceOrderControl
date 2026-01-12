using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.ApiService.Extensions;
using OsService.Application.V1.UseCases.Customers.CreateCustomer;
using OsService.Application.V1.UseCases.Customers.GetCustomerByContact;
using OsService.Application.V1.UseCases.Customers.GetCustomerById;
using static OsService.Application.V1.UseCases.Customers.CreateCustomer.CreateCustomer;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("v1/customers")]
public sealed class CustomersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Criação de um novo cliente
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="ct"></param>
    /// <response code="201">Customer created successfully. Returns the generated id.</response>
    /// <response code="400">Validation error (name, email, phone, document).</response>
    /// <response code="409">Duplicate document or phone.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult(this, id => CreatedAtAction(nameof(GetById), new { id }, new { id }));
    }

    /// <summary>
    /// Consulta um cliente por Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <response code="200">Customer found.</response>
    /// <response code="404">Customer not found.</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Pesquisa um cliente por telefone ou documento
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="document"></param>
    /// <param name="ct"></param>
    /// <response code="200">Customer found.</response>
    /// <response code="400">Missing or invalid search criteria.</response>
    /// <response code="404">Customer not found for the given criteria.</response>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? phone,
        [FromQuery] string? document,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetCustomerByContactQuery(phone, document), ct);

        return result.ToActionResult(this);
    }
}