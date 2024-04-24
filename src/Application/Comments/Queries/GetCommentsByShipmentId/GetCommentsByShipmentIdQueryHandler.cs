using Application.Common;
using Application.Common.Repositories;
using Domain.Comments;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Queries.GetCommentsByShipmentId;

public sealed class GetCommentsByShipmentIdQueryHandler : IQueryHandler<GetCommentsByShipmentIdQuery, Result<GetCommentsByShipmentIdResult>>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;

    public GetCommentsByShipmentIdQueryHandler(ICommentsRepository commentsRepository, IShipmentsRepository shipmentsRepository)
    {
        _commentsRepository = commentsRepository;
        _shipmentsRepository = shipmentsRepository;
    }

    public async Task<Result<GetCommentsByShipmentIdResult>> ExecuteAsync(GetCommentsByShipmentIdQuery query, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(query.ShipmentId, cancellationToken);
        if (shipment is null)
        {
            return Result<GetCommentsByShipmentIdResult>.NotFound(new List<string> { "Shipment not found" });
        }

        List<Comment> comments = await _commentsRepository.GetByShipmentIdAsync(query.ShipmentId, cancellationToken);
        if (comments is null)
        {
            return Result<GetCommentsByShipmentIdResult>.NotFound(new List<string> { "Comments not found" });
        }

        GetCommentsByShipmentIdResult result = GetCommentsByShipmentIdResult.Map(comments);
        return Result<GetCommentsByShipmentIdResult>.Success(result);
    }
}
