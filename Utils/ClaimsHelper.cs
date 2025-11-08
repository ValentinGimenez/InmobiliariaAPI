using System.Security.Claims;

namespace _net_integrador.Utils
{
    public static class ClaimsHelper
    {
        public static int GetUserIdOrThrow(this ClaimsPrincipal user)
        {
            var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out var id) || id <= 0)
                throw new UnauthorizedAccessException("Token inválido.");
            return id;
        }
    }
}
