using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using Db = ChemDec.Api.Datamodel;

namespace ChemDec.Api.Model
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }

        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }


    public class NewAttachment
    {
        public IFormFile Attachment { get; set; }
        public string Shipment { get; set; }
    }   

    public class AttachmentResponse
    {
        public Stream Attachment { get; set; }
        public string Path { get; set; }
        public string MimeType { get; set; }
    }
    public class AttachmentProfile : Profile
    {
        public AttachmentProfile()
        {
            CreateMap<Attachment, Db.Attachment>();
            CreateMap<Db.Attachment, Attachment>();
        }
    }
}
