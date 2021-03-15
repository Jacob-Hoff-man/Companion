using System;
using System.Collections.Generic;
using SQLite;

namespace Companion.Models
{
    public class Trip
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }
        public List<RecordedData> TripData { get; set; }
        public GoalProfile GoalProfile { get; set; }

        public Trip() { }
    
    }

    public class RecordedData
    {
        public RecordedData()
        {

        }


        public char Power { get; set; }
        public short Energy { get; set; }
        public ushort Speed { get; set; }
        public double Distance { get; set; }
        public short Incline { get; set; }
        public ushort GearRatio { get; set; }
        public byte Cadence { get; set; }
        public ushort Torque { get; set; }
        public byte WindSpeed { get; set; }
        public byte WindDirection { get; set; }
        public byte Compass { get; set; }

    }
}
