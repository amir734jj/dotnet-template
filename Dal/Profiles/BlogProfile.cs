using System.Data.Entity;
using System.Linq;
using EfCoreRepository;
using Models.Models;

namespace Dal.Profiles
{
    public class BlogProfile : EntityProfile<Blog>
    {
        public override void Update(Blog entity, Blog dto)
        {
            entity.Text = dto.Text;
            entity.Title = dto.Title;
        }

        public override IQueryable<Blog> Include<TQueryable>(TQueryable queryable)
        {
            return queryable.Include(x => x.Owner);
        }
    }
}