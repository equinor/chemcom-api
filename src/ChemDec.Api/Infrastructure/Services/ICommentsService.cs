using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ChemDec.Api.Controllers.Handlers.ShipmentHandler;

namespace ChemDec.Api.Infrastructure.Services
{
    public interface ICommentsService
    {
        Task<List<string>> AddComment(Initiator initiator, string comment, Guid shipmentId, Guid senderId);
    }
}
