using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.ApiService.Extensions;
using OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderAttachments;
using OsService.Application.V1.UseCases.ServiceOrders.GetServiceOrderById;
using OsService.Application.V1.UseCases.ServiceOrders.OpenServiceOrder;
using OsService.Application.V1.UseCases.ServiceOrders.SearchServiceOrders;
using OsService.Application.V1.UseCases.ServiceOrders.UpdateServiceOrderPrice;
using OsService.Application.V1.UseCases.ServiceOrders.UploadServiceOrderAttachment;
using OsService.Domain.Enums;
using static OsService.Application.V1.UseCases.ServiceOrders.ChangeServiceOrderStatus.ChangeServiceOrderStatus;

namespace OsService.ApiService.Controllers;

/// <summary>
/// Expõe operações para abertura, consulta e atualização de Ordens de Serviço.
/// </summary>
[ApiController]
[Route("v1/service-orders")]
[Produces("application/json")]
public sealed class ServiceOrdersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Abre uma nova Ordem de Serviço vinculada a um cliente existente.
    /// </summary>
    /// <param name="cmd">Comando contendo os dados necessários para abertura da OS.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="201">OS criada com sucesso. Retorna os dados da OS criada.</response>
    /// <response code="400">Erro de validação (descrição, preço ou outros campos inválidos).</response>
    /// <response code="404">Cliente não encontrado.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Open(
        [FromBody] OpenServiceOrder.Command cmd,
        CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);

        return result.ToActionResult(this, so => Created($"/v1/service-orders/{so.Id}", so)); //TODO: Trocar por aquele que pega o ID
    }

    /// <summary>
    /// Consulta uma Ordem de Serviço pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da OS.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">OS encontrada. Retorna os detalhes da OS.</response>
    /// <response code="400">Id informado em formato inválido.</response>
    /// <response code="404">Nenhuma OS encontrada para o identificador informado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetServiceOrderById.Query(id), ct);
        return result.ToActionResult(this);
    }

    //TODO: Aqui tem o erro de Enum (pra variar) e possivelmente normalização da data

    /// <summary>
    /// Lista ordens de serviço filtrando por cliente, status e/ou período.
    /// </summary>
    /// <param name="query">
    /// Objeto de consulta contendo os filtros de pesquisa
    /// (cliente, status e intervalo de datas).
    /// </param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Lista de ordens de serviço que atendem aos critérios informados.</response>
    /// <response code="400">Critérios de busca ausentes ou inválidos.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// <param name="id">Identificador da OS a ser atualizada.</param>
    /// <param name="body">Comando contendo o novo status e dados adicionais.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <remarks>
    /// Regras de transição de status:
    /// - Aberta → EmExecução (permitido)
    /// - EmExecução → Finalizada (permitido, exige valor)
    /// - Aberta → Finalizada (retorna 409)
    /// - Finalizada → qualquer outro status (retorna 409)
    /// </remarks>
    /// <response code="200">Status alterado com sucesso.</response>
    /// <response code="400">Dados de validação inválidos.</response>
    /// <response code="404">OS não encontrada.</response>
    /// <response code="409">Transição de status não permitida para a OS.</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    /// <param name="id">Identificador da OS.</param>
    /// <param name="body">Comando contendo o novo valor da OS.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Valor atualizado com sucesso.</response>
    /// <response code="400">Dados de validação inválidos.</response>
    /// <response code="404">OS não encontrada.</response>
    /// <response code="409">Não é permitido alterar o valor de uma OS finalizada.</response>
    [HttpPut("{id:guid}/price")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    /// Anexa uma imagem de "antes" do serviço à Ordem de Serviço.
    /// </summary>
    /// <param name="serviceOrderId">Identificador da OS.</param>
    /// <param name="file">Arquivo de imagem a ser anexado.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Anexo salvo com sucesso.</response>
    /// <response code="400">Validação inválida ou arquivo ausente.</response>
    /// <response code="404">OS não encontrada.</response>
    [HttpPost("{serviceOrderId:guid}/attachments/before")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Anexa uma imagem de "depois" do serviço à Ordem de Serviço.
    /// </summary>
    /// <param name="serviceOrderId">Identificador da OS.</param>
    /// <param name="file">Arquivo de imagem a ser anexado.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Anexo salvo com sucesso.</response>
    /// <response code="400">Validação inválida ou arquivo ausente.</response>
    /// <response code="404">OS não encontrada.</response>
    [HttpPost("{serviceOrderId:guid}/attachments/after")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// Lista todos os anexos (antes e depois) de uma Ordem de Serviço.
    /// </summary>
    /// <param name="id">Identificador da OS.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Lista de anexos retornada com sucesso.</response>
    /// <response code="404">OS não encontrada.</response>
    [HttpGet("{id:guid}/attachments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttachments(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetServiceOrderAttachments.Query(id);
        var result = await mediator.Send(query, ct);
        return result.ToActionResult(this);
    }
}
