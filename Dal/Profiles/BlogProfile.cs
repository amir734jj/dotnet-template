using System.Linq;
using EfCoreRepository;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Dal.Profiles;

public class BlogProfile : EntityProfile<Unit>
{
    protected override void Update(Unit entity, Unit dto)
    {
        entity.Number = dto.Number;
        entity.Address = dto.Address;
    }

    protected override IQueryable<Unit> Include<TQueryable>(TQueryable queryable)
    {
        return queryable.Include(x => x.Owner);
    }
}