using System.Data.Entity;
using System.Linq;
using EfCoreRepository;
using Models.Models;

namespace Dal.Profiles
{
    public class UserProfile : EntityProfile<User>
    {
        public override void Update(User entity, User dto)
        {
            entity.Role = dto.Role;
            entity.Name = dto.Name;
            entity.Email = dto.Email;
            entity.Description = dto.Description;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.LastLoginTime = dto.LastLoginTime;
            entity.Photo = dto.Photo;
        }

        public override IQueryable<User> Include<TQueryable>(TQueryable queryable)
        {
            return queryable.Include(x => x.Blogs);
        }
    }
}