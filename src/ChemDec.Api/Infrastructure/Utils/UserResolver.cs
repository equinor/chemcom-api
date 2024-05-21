using ChemDec.Api.Infrastructure.Security;
using LazyCache;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChemDec.Api.Infrastructure.Utils
{
    public class UserResolver
    {
        private readonly IHttpContextAccessor _context;

        public UserResolver(IHttpContextAccessor context)
        {
            _context = context;          
        }

       
        public ClaimsPrincipal GetCurrentUserPrincipal()
        {
            return _context.HttpContext.User;
        }

        public string GetCurrentUserId()
        {
            return _context.HttpContext.User.Identity.Name;
        }
        public string GetCurrentUserDisplayName()
        {
            return _context.HttpContext.User.FindFirstValue("name");
        }
    }
}
