using Microsoft.AspNetCore.Http;
using OsService.Domain.ResultPattern;
using System.Buffers;
using System.Text.RegularExpressions;

namespace OsService.Application.V1.UseCases.ServiceOrders.UploadServiceOrderAttachment;

public partial class UploadServiceOrderAttachment
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; 

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png"
    ];

    private static readonly string[] AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png"
    ];

    public sealed record ValidatedFile(
        string SafeFileName,
        string ContentType,
        string Extension);

    public static async Task<Result<ValidatedFile>> ValidateAsync(
        IFormFile file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return Result.Failure<ValidatedFile>(ServiceOrderErrors.AttachmentFileRequired);

        if (file.Length > MaxFileSizeBytes)
            return Result.Failure<ValidatedFile>(ServiceOrderErrors.AttachmentTooLarge);

        if (!AllowedContentTypes.Contains(file.ContentType))
            return Result.Failure<ValidatedFile>(ServiceOrderErrors.AttachmentInvalidContentType);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return Result.Failure<ValidatedFile>(ServiceOrderErrors.AttachmentInvalidExtension);

        var header = ArrayPool<byte>.Shared.Rent(8);
        try
        {
            await using var stream = file.OpenReadStream();
            var read = await stream.ReadAsync(header.AsMemory(0, 8), ct);

            if (!IsValidSignature(header, read, ext))
                return Result.Failure<ValidatedFile>(ServiceOrderErrors.AttachmentInvalidSignature);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(header);
        }

        var safeName = SanitizeFileName(file.FileName);

        return Result.Success(new ValidatedFile(
            SafeFileName: safeName,
            ContentType: file.ContentType!,
            Extension: ext));
    }

    private static string SanitizeFileName(string original)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(original
            .Where(c => !invalidChars.Contains(c))
            .ToArray());

        if (string.IsNullOrWhiteSpace(cleaned))
            cleaned = "file";

        return cleaned.Length > 100 ? cleaned[..100] : cleaned;
    }

    private static bool IsValidSignature(byte[] header, int read, string ext)
    {
        // JPEG: FF D8 FF
        if (ext is ".jpg" or ".jpeg")
        {
            return read >= 3 &&
                   header[0] == 0xFF &&
                   header[1] == 0xD8 &&
                   header[2] == 0xFF;
        }

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (ext is ".png")
        {
            return read >= 8 &&
                   header[0] == 0x89 &&
                   header[1] == 0x50 &&
                   header[2] == 0x4E &&
                   header[3] == 0x47 &&
                   header[4] == 0x0D &&
                   header[5] == 0x0A &&
                   header[6] == 0x1A &&
                   header[7] == 0x0A;
        }

        return false;
    }
}
