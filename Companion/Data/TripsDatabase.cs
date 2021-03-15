using System;
using SQLite;
using Xamarin.Forms;
using Companion.Data;
using Companion.Models;
using System.Collections.Generic;

namespace Companion.Data
{
    public class TripsDatabase
    {

        static object locker = new object();

        SQLiteConnection database;

        public TripsDatabase()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Trip>();
        }

        public IEnumerator<Trip> GetTripEnumerated()
        {
            lock (locker)
            {
                if (this.database.Table<Trip>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return this.database.Table<Trip>().GetEnumerator();
                }
            }
        }

        public Trip GetTrip()
        {
            lock (locker)
            {
                if (database.Table<Trip>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<Trip>().First();
                }
            }
        }

        public int SaveTrip(Trip trip)
        {
            lock (locker)
            {
                if (trip.ID != 0)
                {
                    database.Update(trip);
                    return trip.ID;
                }
                else
                {
                    return database.Insert(trip);
                }
            }
        }

        public int DeleteTrip(int id)
        {
            lock (locker)
            {
                return database.Delete<Trip>(id);
            }
        }

        public void DeleteAllTrips()
        {
            lock (locker)
            {
                database.DropTable<Trip>();
                database.CreateTable<Trip>();
            }
        }

    }

}

