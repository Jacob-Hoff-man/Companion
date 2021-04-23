using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Companion.Models;
using SQLite;

namespace Companion.Data
{
    public class GoalProfileRepo : IRepo<GoalProfile>
    {
        private SQLiteAsyncConnection _connection;

        public GoalProfileRepo()
        {
        }

        public async Task DeleteRepoItems()
        {
            await _connection.DropTableAsync<GoalProfile>();
            await _connection.CreateTableAsync<GoalProfile>();
        }

        public async Task DeleteRepoItem(GoalProfile item)
        {
            if (item.ID != 0)
            {
                _ = await _connection.DeleteAsync<GoalProfile>(item.ID);
            }
            else
            {
               //none
            }
        }

        public async Task AddOrUpdateItems(GoalProfile item)
        {
            if (item.ID != 0)
            {
                _ = await _connection.UpdateAsync(item);
            }
            else
            {
                _ = await _connection.InsertAsync(item);
            }
        }

        public Task<List<GoalProfile>> GetRepoItems() => _connection.Table<GoalProfile>().ToListAsync();

        public async Task Initialize()
        {
            if (_connection != null) return;

            _connection = new SQLiteAsyncConnection(Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "goalprofiles.db3"));

            await _connection.CreateTableAsync<GoalProfile>();
        }


    }

}
