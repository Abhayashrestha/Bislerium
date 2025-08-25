using Microsoft.AspNetCore.Identity;

namespace Bislerium.API.Respositories.Repository
{
    public interface ITokenRespository
    {
        string createJwtToken(IdentityUser user, List<string> roles);
    }
}
