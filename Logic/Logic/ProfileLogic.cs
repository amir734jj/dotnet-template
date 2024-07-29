using System.Threading.Tasks;
using Logic.Interfaces;
using Models.Models;
using Models.ViewModels.Api;

namespace Logic.Logic;

public class ProfileLogic(IUserLogic userLogic) : IProfileLogic
{
    public Task<ProfileViewModel> Get(User user)
    {
        return Task.FromResult(new ProfileViewModel
        {
            Email = user.Email,
            Description = user.Description,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        });
    }

    public async Task Update(User user, ProfileViewModel profileViewModel)
    {
        await userLogic.Update(user.Id, entity =>
        {
            entity.Name = profileViewModel.Name;
            entity.Description = profileViewModel.Description;
            entity.Email = profileViewModel.Email;
            entity.PhoneNumber = profileViewModel.PhoneNumber;
        });
    }
}