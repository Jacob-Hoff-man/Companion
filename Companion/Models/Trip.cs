using System;
using System.Collections.Generic;
using System.ComponentModel;
using SQLite;


namespace Companion.Models
{
    public class Trip
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<RecordedData> TripData { get; set; }
        public GoalProfile GoalProfile { get; set; }

        public Trip()
        {
            TripData = new List<RecordedData>();

        }
    
    }

    public class RecordedData
    {
        //all of these parameters are collected via ble
        public float Speed { get; set; }
        public float Distance { get; set; }
        public float AverageSpeed { get; set; }
        public float Acceleration { get; set; }
        public float Incline { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Compass { get; set; }
        public float Altitude { get; set; }
        public float Power { get; set; }
        public float Calories { get; set; }
        public float GearRatio { get; set; }
        public float Cadence { get; set; }
        public float Torque { get; set; }
        public float Mass => Torque / 0.175F;
        public float WindSpeed { get; set; }
        public float WindDirection { get; set; }

        //used to record trips, not sent via ble
        public int ElapsedSeconds { get; set; }

        public RecordedData()
        {
            Speed = 0.0F;
            Distance = 0.0f;
            AverageSpeed = 0.0F;
            Acceleration = 0.0F;
            Incline = 0.0F;
            Latitude = 0.0F;
            Longitude = 0.00F;
            Compass = 0.0F;
            Altitude = 0.0F;
            Power = 0.0F;
            Calories = 0.0F;
            GearRatio = 0.0F;
            Cadence = 0.0F;
            Torque = 0.0F;
            WindSpeed = 0.0F;
            WindDirection = 0.0F;
        }

    }
}
