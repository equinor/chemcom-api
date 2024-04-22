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
    Task<Comment> GetByIdAsync(Guid id);
    Task<List<Comment>> GetByShipmentIdAsync(Guid shipmentId);
    Task InsertAsync(Comment comment);
}