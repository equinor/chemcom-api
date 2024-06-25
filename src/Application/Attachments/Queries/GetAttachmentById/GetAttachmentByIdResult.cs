using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Queries.GetAttachmentById;

public sealed record GetAttachmentByIdResult(Stream AttchmentStream, string FileName, string ContentType);

