using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IBasicLogic<T>
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> Get(int id);

        Task<T> Save(T instance);
        
        Task<T> Delete(int id);

        Task<T> Update(int id, T dto);

        Task<T> Update(int id, Action<T> updater);
    }
}