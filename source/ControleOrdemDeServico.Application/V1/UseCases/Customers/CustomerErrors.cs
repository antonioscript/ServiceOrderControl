using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.Customers;

public static class CustomerErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Customer.NameRequired", "Nome é obrigatório.");

    public static readonly Error NameTooShort =
        Error.Validation("Customer.NameTooShort", "Nome deve ter pelo menos 2 caracteres.");

    public static readonly Error NameTooLong =
        Error.Validation("Customer.NameTooLong", "Nome deve ter no máximo 150 caracteres.");

    public static readonly Error InvalidEmail =
        Error.Validation("Customer.InvalidEmail", "E-mail inválido.");

    public static readonly Error EmailTooLong =
        Error.Validation("Customer.EmailTooLong", "E-mail deve ter no máximo 120 caracteres.");

    public static readonly Error PhoneTooLong =
        Error.Validation("Customer.PhoneTooLong", "Telefone deve ter no máximo 30 caracteres.");

    public static readonly Error DocumentTooLong =
        Error.Validation("Customer.DocumentTooLong", "Documento deve ter no máximo 30 caracteres.");

    public static readonly Error DocumentAlreadyExists =
        Error.Conflict("Customer.DocumentAlreadyExists", "Já existe um cliente com este documento.");

    public static readonly Error PhoneAlreadyExists =
        Error.Conflict("Customer.PhoneAlreadyExists", "Já existe um cliente com este telefone.");

    public static readonly Error SearchCriteriaRequired =
        Error.Validation("Customer.SearchCriteriaRequired", "Informe pelo menos telefone ou documento.");

    public static readonly Error NotFound =
        Error.NotFound("Customer.NotFound", "Cliente não encontrado.");
}
