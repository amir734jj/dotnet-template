using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Models;

namespace Logic.Crud
{
    public class BlogLogic : BasicLogicAbstract<Blog>, IBlogLogic
    {
        private readonly IBasicCrud<Blog> _blogDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        public BlogLogic(IEfRepository repository)
        {
            _blogDal = repository.For<Blog>();
        }

        /// <summary>
        /// Returns DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrud<Blog> GetBasicCrudDal()
        {
            return _blogDal;
        }
    }
}