using System.Threading.Tasks;
using Models.Models;
using Models.ViewModels.Api;

namespace Logic.Interfaces;

public interface IProfileLogic
{
    Task<ProfileViewModel> Get(User user);
        
    Task Update(User user, ProfileViewModel profile);
}