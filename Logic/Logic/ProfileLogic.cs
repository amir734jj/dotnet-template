using System.Threading.Tasks;
using Logic.Interfaces;
using Models.Models;
using Models.ViewModels.Api;

namespace Logic.Logic
{
    public class ProfileLogic : IProfileLogic
    {
        private readonly IUserLogic _userLogic;

        public ProfileLogic(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }


        public async Task<ProfileViewModel> Get(User user)
        {
            return new ProfileViewModel
            {
                Email = user.Email,
                Description = user.Description,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };
        }

        public async Task Update(User user, ProfileViewModel profileViewModel)
        {
            await _userLogic.Update(user.Id, entity =>
            {
                entity.Name = profileViewModel.Name;
                entity.Description = profileViewModel.Description;
                entity.Email = profileViewModel.Email;
                entity.PhoneNumber = profileViewModel.PhoneNumber;
            });
        }
    }
}