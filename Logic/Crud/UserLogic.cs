using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Models;

namespace Logic.Crud;

public class UserLogic(IEfRepository repository) : BasicLogicAbstract<User>, IUserLogic
{
    private readonly IBasicCrud<User> _userDal = repository.For<User>();

    protected override IBasicCrud<User> GetBasicCrudDal()
    {
        return _userDal;
    }
}