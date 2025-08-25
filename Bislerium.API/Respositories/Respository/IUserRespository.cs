using Bislerium.API.Model.Dto;
using Microsoft.AspNetCore.Identity;

namespace Bislerium.API.Respositories.Respository
{
    public interface IUserRespository
    {
        Task<UserListDto> GetUserById(string id);
        Task<IdentityResult> UpdateUser(string id, UserUpdateDto userDto);
        Task<IdentityResult> DeleteUser(string id);
    }
}
