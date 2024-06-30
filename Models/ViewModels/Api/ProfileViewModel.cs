using Models.Enums;

namespace Models.ViewModels.Api;

public class ProfileViewModel
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Description { get; set; }
        
    public RoleEnum Role { get; set; }
}