using Application.Common.Repositories;
using Domain.Comments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories;


public sealed class CommentsRepository : ICommentsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CommentsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Comment> GetByIdAsync(Guid id)
    {
        return await _dbContext.Comments.FindAsync(id);
    }

    public async Task InsertAsync(Comment comment)
    {
        await _dbContext.Comments.AddAsync(comment);
    }

    public void Delete(Comment comment)
    {
        _dbContext.Comments.Remove(comment);
    }

    public async Task<List<Comment>> GetByShipmentIdAsync(Guid shipmentId)
    {
        return await _dbContext.Comments.Where(c => c.ShipmentId == shipmentId).ToListAsync();
    }
}
