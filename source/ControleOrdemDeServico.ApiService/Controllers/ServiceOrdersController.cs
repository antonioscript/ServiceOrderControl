using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.ApiService.Extensions;
using OsService.Application.V1.UseCases.ServiceOrders.ChangeServiceOrderStatus;
using OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderAttachments;
using OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderById;
using OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;
using OsService.Application.V1.UseCases.ServiceOrders.SearchServiceOrders;
using OsService.Application.V1.UseCases.ServiceOrders.UpdateServiceOrderPrice;
using OsService.Application.V1.UseCases.ServiceOrders.UploadServiceOrderAttachment;
using OsService.Domain.Enums;
using static OsService.Application.V1.UseCases.ServiceOrders.ChangeServiceOrderStatus.ChangeServiceOrderStatus;

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


    /// <summary>
    /// Altera o status da Ordem de Serviço.
    /// </summary>
    /// <remarks>
    /// Regras:
    /// - Aberta -> EmExecução (permitido)
    /// - EmExecução -> Finalizada (permitido, exige valor)
    /// - Aberta -> Finalizada (409)
    /// - Finalizada -> qualquer outro (409)
    /// </remarks>
    /// <response code="200">Status alterado com sucesso</response>
    /// <response code="400">Validação inválida</response>
    /// <response code="404">OS não encontrada</response>
    /// <response code="409">Transição de status não permitida</response>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeServiceOrderCommand body,
        CancellationToken ct)
    {
        var cmd = body with { Id = id };

        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }


    /// <summary>
    /// Define ou altera o valor da Ordem de Serviço.
    /// </summary>
    /// <response code="200">Valor atualizado com sucesso</response>
    /// <response code="400">Validação inválida</response>
    /// <response code="404">OS não encontrada</response>
    /// <response code="409">Não é permitido alterar o valor para OS finalizada</response>
    [HttpPut("{id:guid}/price")]
    public async Task<IActionResult> UpdatePrice(
        Guid id,
        [FromBody] UpdateServiceOrderPrice.UpdateServiceOrderPriceCommand body,
        CancellationToken ct)
    {
        var cmd = body with { Id = id };

        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Anexa uma imagem "antes" do serviço.
    /// </summary>
    /// <response code="200">Anexo salvo com sucesso</response>
    /// <response code="400">Validação inválida ou arquivo ausente</response>
    /// <response code="404">OS não encontrada</response>
    [HttpPost("{serviceOrderId:guid}/attachments/before")]
    public async Task<IActionResult> UploadBefore(
        Guid serviceOrderId,
        IFormFile file,
        CancellationToken ct)
    {
        var cmd = new UploadServiceOrderAttachment.Command(
            ServiceOrderId: serviceOrderId,
            Type: AttachmentType.Before,
            File: file);

        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Anexa uma imagem "depois" do serviço.
    /// </summary>
    /// <response code="200">Anexo salvo com sucesso</response>
    /// <response code="400">Validação inválida ou arquivo ausente</response>
    /// <response code="404">OS não encontrada</response>
    [HttpPost("{serviceOrderId:guid}/attachments/after")]
    public async Task<IActionResult> UploadAfter(
        Guid serviceOrderId,
        IFormFile file,
        CancellationToken ct)
    {
        var cmd = new UploadServiceOrderAttachment.Command(
            ServiceOrderId: serviceOrderId,
            Type: AttachmentType.After,
            File: file);

        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Lista todos os anexos (antes/depois) de uma OS.
    /// </summary>
    /// <response code="200">Lista de anexos</response>
    /// <response code="404">OS não encontrada</response>
    [HttpGet("{id:guid}/attachments")]
    public async Task<IActionResult> GetAttachments(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetServiceOrderAttachments.Query(id);
        var result = await mediator.Send(query, ct);
        return result.ToActionResult(this);
    }


}