using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Companion.Data
{
    public interface IRepo<T>
    {
        //Task Initialize();
        Task<List<T>> GetRepoItems();
        Task AddOrUpdateItems(T item);
    }
}
