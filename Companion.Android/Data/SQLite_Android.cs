using System;
using System.IO;
using Companion.Data;
using Companion.Droid.Data;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]

namespace Companion.Droid.Data
{
    public class SQLite_Android: ISQLite
    {
        public SQLite_Android() { }
        public SQLite.SQLiteConnection GetConnection()
        {
            var sqliteFileName = "Database.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFileName);
            var connection = new SQLite.SQLiteConnection(path);

            return connection;
        }

    }
}
