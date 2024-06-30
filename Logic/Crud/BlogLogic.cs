using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Models;

namespace Logic.Crud;

public class BlogLogic(IEfRepository repository) : BasicLogicAbstract<Unit>, IBlogLogic
{
    private readonly IBasicCrud<Unit> _blogDal = repository.For<Unit>();

    protected override IBasicCrud<Unit> GetBasicCrudDal()
    {
        return _blogDal;
    }
}