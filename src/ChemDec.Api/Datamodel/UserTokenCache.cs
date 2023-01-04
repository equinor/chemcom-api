using System;

namespace ChemDec.Api.Datamodel
{
    public partial class UserTokenCache
    {
        public int UserTokenCacheId { get; set; }

        public string webUserUniqueId { get; set; }

        public byte[] cacheBits { get; set; }

        public DateTime LastWrite { get; set; }
    }





}
