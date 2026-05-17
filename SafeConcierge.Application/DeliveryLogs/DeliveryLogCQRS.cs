using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.DeliveryLogs.Commands;

public record UploadSignatureCommand(Guid DeliveryLogId, string SignatureBase64, Guid RequestedByUserId) : IRequest<Result<string>>;
