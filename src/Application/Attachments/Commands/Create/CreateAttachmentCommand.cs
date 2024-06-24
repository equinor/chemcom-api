using Application.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Commands.Create;

public sealed record CreateAttachmentCommand : ICommand
{
    public CreateAttachmentCommand(Guid shipmentId, string path, string extension, string contentType, byte[] fileContents, User user)
    {
        ShipmentId = shipmentId;
        Path = path;
        Extension = extension;
        ContentType = contentType;
        FileContents = fileContents;
        User = user;
    }

    public Guid ShipmentId { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public string ContentType { get; set; }
    public byte[] FileContents { get; set; }
    public User User { get; set; }
}
