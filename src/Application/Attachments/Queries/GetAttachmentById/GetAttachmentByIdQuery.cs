﻿using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Queries.GetAttachmentById;

public sealed record GetAttachmentByIdQuery(Guid ShipmentId, Guid AttachmentId) : IQuery;

