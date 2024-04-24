using Domain.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Repositories;

public interface ICommentsRepository
{
    void Delete(Comment comment);
    Task<Comment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Comment>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task InsertAsync(Comment comment, CancellationToken cancellationToken = default);
}