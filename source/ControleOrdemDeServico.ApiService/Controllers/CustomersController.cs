using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.ApiService.Extensions;
using OsService.Application.V1.UseCases.Customers.GetCustomerByContact;
using OsService.Application.V1.UseCases.Customers.GetCustomerById;
using static OsService.Application.V1.UseCases.Customers.CreateCustomer.CreateCustomer;

namespace OsService.ApiService.Controllers;

/// <summary>
/// Expõe operações para criação e consulta de clientes.
/// </summary>
[ApiController]
[Route("v1/customers")]
[Produces("application/json")]
public sealed class CustomersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <param name="cmd">Dados do cliente a ser criado.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="201">Cliente criado com sucesso. Retorna o identificador gerado.</response>
    /// <response code="400">Erro de validação (nome, e-mail, telefone ou documento inválidos).</response>
    /// <response code="409">Já existe cliente com o mesmo documento ou telefone.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerCommand cmd,
        CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);

        return result.ToActionResult(this, id => CreatedAtAction(nameof(GetById), new { id }, new { id }));
    }

    /// <summary>
    /// Consulta os dados de um cliente pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="404">Nenhum cliente encontrado para o identificador informado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Pesquisa um cliente por telefone ou documento.
    /// </summary>
    /// <param name="phone">Telefone do cliente.</param>
    /// <param name="document">Documento do cliente.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="400">Critério de pesquisa ausente ou inválido.</response>
    /// <response code="404">Nenhum cliente encontrado para os critérios informados.</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
