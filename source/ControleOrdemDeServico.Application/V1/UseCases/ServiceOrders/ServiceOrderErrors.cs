using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders;

public static class ServiceOrderErrors
{
    public static readonly Error CustomerRequired =
        Error.Validation("ServiceOrder.CustomerRequired", "Id do cliente é obrigatório.");

    public static readonly Error DescriptionRequired =
        Error.Validation("ServiceOrder.DescriptionRequired", "Descrição é obrigatória.");

    public static readonly Error DescriptionTooLong =
        Error.Validation("ServiceOrder.DescriptionTooLong", "Descrição deve ter no máximo 500 caracteres.");

    public static readonly Error PriceNegative =
        Error.Validation("ServiceOrder.PriceNegative", "Valor não pode ser negativo.");

    public static readonly Error CustomerNotFound =
        Error.NotFound("ServiceOrder.CustomerNotFound", "Cliente não encontrado.");

    public static readonly Error NotFound =
        Error.NotFound("ServiceOrder.NotFound", "Ordem de serviço não encontrada.");

    public static readonly Error IdRequired =
        Error.Validation("ServiceOrder.IdRequired", "Id da ordem de serviço é obrigatório.");

    public static readonly Error SearchCriteriaRequired =
        Error.Validation("ServiceOrder.SearchCriteriaRequired", "Informe pelo menos um filtro (cliente, status ou período).");

    public static readonly Error InvalidPeriod =
        Error.Validation("ServiceOrder.InvalidPeriod", "A data inicial não pode ser maior que a data final.");

    public static readonly Error InvalidStatusTransition =
        Error.Conflict("ServiceOrder.InvalidStatusTransition","Esta transição de status não é permitida.");

    public static readonly Error AlreadyFinished =
        Error.Conflict("ServiceOrder.AlreadyFinished","Uma ordem de serviço finalizada não pode alterar o status.");

    public static readonly Error PriceRequiredToFinish =
        Error.Validation("ServiceOrder.PriceRequiredToFinish","Uma ordem de serviço deve ter um valor para ser finalizada.");

    public static readonly Error PriceChangeNotAllowed =
        Error.Conflict("ServiceOrder.PriceChangeNotAllowed","O valor não pode ser alterado após a ordem de serviço ser finalizada.");

    public static readonly Error AttachmentFileRequired =
        Error.Validation("ServiceOrder.AttachmentFileRequired","Arquivo é obrigatório.");

    public static readonly Error AttachmentTooLarge =
        Error.Validation("ServiceOrder.AttachmentTooLarge", "Arquivo deve ter no máximo 5 MB.");

    public static readonly Error AttachmentInvalidContentType =
        Error.Validation("ServiceOrder.AttachmentInvalidContentType","Apenas imagens JPEG e PNG são permitidas.");

    public static readonly Error AttachmentInvalidExtension =
        Error.Validation("Attachment.InvalidExtension","A extensão do arquivo deve ser .jpg, .jpeg ou .png.");

    public static readonly Error AttachmentInvalidSignature =
        Error.Validation("Attachment.InvalidSignature","O conteúdo do arquivo não corresponde a um formato de imagem.");
}
