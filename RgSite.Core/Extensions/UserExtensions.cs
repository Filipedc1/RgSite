using RgSite.Core.Models;
using System.Linq;
using System.Security.Claims;

namespace RgSite.Core.Extensions
{
    public static class UserExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(RoleConstants.Admin);

        public static bool IsCustomer(this ClaimsPrincipal user)
            => user.IsInRole(RoleConstants.Customer);

        public static bool IsSalon(this ClaimsPrincipal user)
            => user.IsInRole(RoleConstants.Salon);

        public static string GetUsername(this ClaimsPrincipal user)
            => user.Identity.Name.Split('@').FirstOrDefault() ?? string.Empty;

        public static string GetUserEmail(this ClaimsPrincipal user)
            => user.Identity.Name;

        public static string GetUserId(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
