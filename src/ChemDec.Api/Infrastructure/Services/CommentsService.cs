using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChemDec.Api.Datamodel;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static ChemDec.Api.Controllers.Handlers.ShipmentHandler;

namespace ChemDec.Api.Infrastructure.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly ChemContext _dbContext;
        private readonly LoggerHelper _loggerHelper;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private TelemetryClient telemetry = new TelemetryClient();

        public CommentsService(ChemContext dbContext, UserService userService, IMapper mapper, LoggerHelper loggerHelper)
        {
            _dbContext = dbContext;
            _loggerHelper = loggerHelper;
            _userService = userService;
            _mapper = mapper;
        }

        //NOTE: Initial implementation of the project has lot of code smells and rookie errors.
        //      This is an attempt to re-implement and to write some clean code.        
        public async Task<List<string>> AddComment(Initiator initiator, string comment, Guid shipmentId, Guid senderId)
        {
            var validationErrors = new List<string>();
            var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(ps => ps.Id == shipmentId);
            if (shipment == null)
            {
                validationErrors.Add($"cannot find the shipment with Id {shipmentId}");
                return validationErrors;
            }
            validationErrors = await Validate(initiator, shipmentId, senderId, shipment);
            if (validationErrors.Any())
            {
                return validationErrors;
            }            

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    shipment.Status = "Changed";
                    var newComment = new Datamodel.Comment
                    {
                        Id = Guid.NewGuid(),
                        ShipmentId = shipmentId,
                        CommentText = comment
                    };

                    _dbContext.Comments.Add(newComment);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return null;
        }


        private async Task<List<string>> Validate(Initiator initiator, Guid shipmentId, Guid senderId, Datamodel.Shipment shipment)
        {
            var validationErrors = new List<string>();

            var user = await _userService.GetCurrentUser();
            if (user == null)
            {
                validationErrors.Add("You do not have access to save from this plant");
            }

            //TODO: Refacor to middleware
            //Note: Validate save from this installation
            if (initiator == Initiator.Offshore && user != null && user.Roles.Any(s => s.Installation != null && s.Installation.Id == senderId) == false)
            {
                validationErrors.Add("You do not have access to save from this installation");
            }

            //TODO: Refacor to middleware
            //Note: Validate save from this plant
            var sender = await _dbContext.Installations.Where(w => w.Id == senderId).ProjectTo<PlantReference>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            var role = user.Roles.FirstOrDefault(u => u.Id == shipment.SenderId.ToString());
            if (role != null)
            {
                var receiver = role.Installation.ShipsTo.FirstOrDefault();
                if (initiator == Initiator.Onshore && user.Roles.Any(s => s.Installation != null && s.Installation.Id == receiver.Id) == false)
                {
                    validationErrors.Add("You do not have access to save from this plant");
                }
            }
            else
            {
                validationErrors.Add("You do not have access to save from this plant");
            }

            return validationErrors;
        }

    }
}
