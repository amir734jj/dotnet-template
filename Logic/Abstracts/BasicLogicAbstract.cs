using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;

namespace Logic.Abstracts;

public abstract class BasicLogicAbstract<T> : IBasicLogic<T> where T: class
{
    protected abstract IBasicCrud<T> GetBasicCrudDal();

    public virtual Task<IEnumerable<T>> GetAll()
    {
        return GetBasicCrudDal().GetAll();
    }

    public virtual Task<T> Get(int id)
    {
        return GetBasicCrudDal().Get(id);
    }

    public virtual Task<T> Save(T instance)
    {
        return GetBasicCrudDal().Save(instance);
    }

    public virtual Task<T> Delete(int id)
    {
        return GetBasicCrudDal().Delete(id);
    }

    public virtual Task<T> Update(int id, T dto)
    {
        return GetBasicCrudDal().Update(id, dto);
    }

    public async Task<T> Update(int id, Action<T> updater)
    {
        var entity = await GetBasicCrudDal().Get(id);

        updater(entity);
            
        return await GetBasicCrudDal().Update(id, entity);
    }
}