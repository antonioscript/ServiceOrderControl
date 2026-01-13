using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using OsService.Application.V1.Abstractions.Persistence;
using OsService.Domain.Entities;
using OsService.Domain.ResultPattern;

namespace OsService.Application.V1.UseCases.ServiceOrders.UploadServiceOrderAttachment;

public partial class UploadServiceOrderAttachment
{
    public sealed class Handler(
        IServiceOrderRepository serviceOrders,
        IAttachmentRepository attachments,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IWebHostEnvironment env)
        : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            // 1) Validação básica de arquivo
            var fileValidation = await ValidateAsync(request.File, ct);
            if (fileValidation.IsFailure)
                return Result.Failure<Response>(fileValidation.Error);

            var validated = fileValidation.Data;

            // 2) Ordem de serviço precisa existir
            var exists = await serviceOrders.ExistsAsync(request.ServiceOrderId, ct);
            if (!exists)
                return Result.Failure<Response>(ServiceOrderErrors.NotFound);

            // 3) Monta paths (streaming: copia direto pro FileStream)
            var uploadsRoot = Path.Combine(env.ContentRootPath, "data", "uploads");
            var osFolder = Path.Combine(uploadsRoot, request.ServiceOrderId.ToString());
            var typeFolder = Path.Combine(osFolder, request.Type.ToString().ToLowerInvariant());

            Directory.CreateDirectory(typeFolder);

            var physicalName = $"{Guid.NewGuid():N}{validated.Extension}";
            var fullPath = Path.Combine(typeFolder, physicalName);

            await using (var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await request.File.CopyToAsync(stream, ct);
            }

            var entity = new AttachmentEntity
            {
                ServiceOrderId = request.ServiceOrderId,
                Type = request.Type,
                FileName = validated.SafeFileName,    
                ContentType = validated.ContentType,
                SizeBytes = request.File.Length,
                StoragePath = fullPath,
                UploadedAt = DateTime.UtcNow
            };

            await attachments.AddAsync(entity, ct);
            await unitOfWork.CommitAsync(ct);

            var response = mapper.Map<Response>(entity);
            return Result.Success(response);
        }
    }
}
