using System.Linq;
using EfCoreRepository;
using Models.Models;

namespace Dal.Profiles
{
    public class UserProfile : EntityProfile<User>
    {
        public override void Update(User entity, User dto)
        {
            entity.Name = dto.Name;
            entity.LastLoginTime = dto.LastLoginTime;
        }

        public override IQueryable<User> Include<TQueryable>(TQueryable queryable)
        {
            return queryable;
        }
    }
}