using Bislerium.API.Model.Dto;
using Bislerium.API.Respositories.Respository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.API.Respositories.Implementation
{
    public class UserRespository: IUserRespository
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserRespository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user != null ? await _userManager.DeleteAsync(user) : IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        public async Task<UserListDto> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user != null ? new UserListDto { UserName = user.UserName, Email = user.Email, PhoneNumber = user.PhoneNumber } : null;
        }


        public async Task<IdentityResult> UpdateUser(string id, UserUpdateDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.UserName = userDto.UserName;
                user.Email = userDto.Email;
                user.PhoneNumber = userDto.PhoneNumber;
                return await _userManager.UpdateAsync(user);
            }

            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }
    }
}
