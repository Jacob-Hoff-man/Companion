using System;
using System.IO;
using Companion.Data;
using Companion.iOS.Data;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_IOS))]

namespace Companion.iOS.Data
{
    public class SQLite_IOS: ISQLite
    {
        public SQLite_IOS()
        {
        }

        public SQLite.SQLiteConnection GetConnection()
        {
            var fileName = "Database.db3";
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentPath, "..", "Library");
            var path = Path.Combine(libraryPath, fileName);
            var connection = new SQLite.SQLiteConnection(path);

            return connection;
        }
    }
}
