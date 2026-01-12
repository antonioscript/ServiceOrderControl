using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.ApiService.Extensions;
using OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderById;
using OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;
using OsService.Application.V1.UseCases.ServiceOrders.SearchServiceOrders;
using OsService.Domain.Enums;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("v1/service-orders")]
public sealed class ServiceOrdersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Abre uma nova Ordem de Serviço vinculada a um cliente existente.
    /// </summary>
    /// <response code="201">OS criada com sucesso</response>
    /// <response code="400">Erro de validação (descrição, preço, etc.)</response>
    /// <response code="404">Cliente não encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(OpenServiceOrder.Response), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Open(
        [FromBody] OpenServiceOrder.Command cmd,
        CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);

        // Sucesso → 201 + Location /v1/service-orders/{id}
        return result.ToActionResult(
            this,
            so => Created($"/v1/service-orders/{so.Id}", so)); //TODO: Trocar por aquele que pega o ID
    }


    /// <summary>
    /// Consulta uma Ordem de Serviço pelo Id.
    /// </summary>
    /// <response code="200">OS encontrada</response>
    /// <response code="400">Id inválido</response>
    /// <response code="404">OS não encontrada</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetServiceOrderById.Query(id), ct);
        return result.ToActionResult(this);
    }

    //TODO: Aqui tem o erro de Enum (pra variar) e possivelmente normalização da data

    /// <summary>
    /// Lista ordens de serviço filtrando por cliente, status e/ou período.
    /// </summary>
    /// <param name="customerId">Id do cliente (opcional).</param>
    /// <param name="status">Status da OS (opcional).</param>
    /// <param name="from">Data inicial de abertura (opcional).</param>
    /// <param name="to">Data final de abertura (opcional).</param>
    /// <response code="200">Lista de ordens de serviço.</response>
    /// <response code="400">Critérios de busca inválidos.</response>
    [HttpGet]
    public async Task<IActionResult> Search(
    [FromQuery] SearchServiceOrders.Query query,
    CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        return result.ToActionResult(this);
    }


}