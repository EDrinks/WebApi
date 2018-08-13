using System;
using System.Security.Claims;
using EDrinks.Common;
using Microsoft.AspNetCore.Http;

namespace EDrinks.WebApi.Services
{
    public class StreamResolver : IStreamResolver
    {
        private readonly IHttpContextAccessor _context;

        public StreamResolver(IHttpContextAccessor context)
        {
            _context = context;
        }
        
        public string GetStream()
        {
            string userId = _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrWhiteSpace(userId)) throw new Exception("invalid user id for stream resolver");
            
            return userId;
        }
    }
}