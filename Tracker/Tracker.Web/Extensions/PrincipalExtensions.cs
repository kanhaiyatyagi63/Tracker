using System.Security.Claims;
using System.Security.Principal;

namespace Tracker.Web.Extensions
{
    public static class PrincipalExtensions
    {
        public static string? GetUserId(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            if (claimsIdentity == null)
            {
                return null;
            }
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value;
        }

        public static string? GetUserName(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            if (claimsIdentity == null)
            {
                return null;
            }
            var giveNameClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            if (giveNameClaim?.Value != null)
            {
                return giveNameClaim?.Value;
            }

            var nameClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            return nameClaim?.Value;
        }

        public static IList<string> GetUserRoles(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            if (claimsIdentity == null)
            {
                return Enumerable.Empty<string>().ToList();
            }
            var claims = claimsIdentity.FindAll(ClaimTypes.Role);

            return claims.Select(x => x.Value).Distinct().ToList();
        }
    }
}
