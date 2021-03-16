using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        T GetById(int id);
        void Insert(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
