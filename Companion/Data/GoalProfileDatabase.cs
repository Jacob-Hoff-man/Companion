using System;
using SQLite;
using Xamarin.Forms;
using Companion.Data;
using Companion.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Companion.Data
{
    public class GoalProfileDatabase
    {

        private static object locker = new object();
        private SQLiteConnection database;

        public GoalProfileDatabase()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<GoalProfile>();
        }

        public IEnumerator<GoalProfile> GetGoalProfileEnumerated()
        {
            lock(locker)
            {
                if(this.database.Table<GoalProfile>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return this.database.Table<GoalProfile>().GetEnumerator();
                }
            }
        }

        public GoalProfile GetGoalProfile()
        {
            lock(locker)
            {
                if(database.Table<GoalProfile>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<GoalProfile>().First();
                }
            }
        }

        public int SaveGoalProfile(GoalProfile goalProfile)
        {
            lock(locker)
            {
                if(goalProfile.ID != 0)
                {
                    database.Update(goalProfile);
                    Debug.WriteLine("Added to database.");
                    return goalProfile.ID;
                }
                else
                {
                    Debug.WriteLine("Added to empty database.");
                    return database.Insert(goalProfile);
                }
            }
        }

        public int DeleteGoalProfile(int id)
        {
            lock(locker)
            {
                return database.Delete<GoalProfile>(id);
            }
        }

        public void DeleteAllGoalProfiles()
        {
            lock(locker)
            {
                database.DropTable<GoalProfile>();
                database.CreateTable<GoalProfile>();
            }
        }

    }

}
