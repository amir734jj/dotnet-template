using System.Threading.Tasks;
using Dal;
using Logic.Interfaces;

namespace Logic.Logic;

public class UserSetup(EntityDbContext dbContext) : IUserSetup
{
    private readonly EntityDbContext _dbContext = dbContext;

    public async Task Setup(int userId)
    {

    }
}