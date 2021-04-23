using System;
using SQLite;
namespace Companion.Data
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
