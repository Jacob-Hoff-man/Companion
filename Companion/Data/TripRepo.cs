using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Companion.Models;
using SQLite;

namespace Companion.Data
{
    public class TripRepo : IRepo<Trip>
    {
        private SQLiteAsyncConnection _connection;

        public TripRepo()
        {
        }

        public async Task DeleteRepoItems()
        {
            await _connection.DropTableAsync<Trip>();
            await _connection.CreateTableAsync<Trip>();
        }

        public async Task AddOrUpdateItems(Trip item)
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

        public Task<List<Trip>> GetRepoItems() => _connection.Table<Trip>().ToListAsync();

        public async Task Initialize()
        {
            if (_connection != null) return;

            _connection = new SQLiteAsyncConnection(Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "trips.db3"));

            await _connection.CreateTableAsync<Trip>();
        }
    }
}
