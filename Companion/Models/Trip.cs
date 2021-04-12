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
        public List<RecordedData> TripData { get; set; }
        public GoalProfile GoalProfile { get; set; }

        public Trip() { }
    
    }

    public class RecordedData
    {

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
        public float WindSpeed { get; set; }
        public string WindDirection { get; set; }

        public RecordedData()
        {

        }

        //private float _speed;
        //public float Speed
        //{
        //    get
        //    {
        //        return _speed;
        //    }
        //    set
        //    {
        //        _speed = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _distance;
        //public float Distance
        //{
        //    get
        //    {
        //        return _distance;
        //    }
        //    set
        //    {
        //        _distance = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _averageSpeed;
        //public float AverageSpeed
        //{
        //    get
        //    {
        //        return _averageSpeed;
        //    }
        //    set
        //    {
        //        _averageSpeed = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _acceleration;
        //public float Acceleration
        //{
        //    get
        //    {
        //        return _acceleration;
        //    }
        //    set
        //    {
        //        _acceleration = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _incline;
        //public float Incline
        //{
        //    get
        //    {
        //        return _incline;
        //    }
        //    set
        //    {
        //        _incline = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _latitude;
        //public float Latitude
        //{
        //    get
        //    {
        //        return _latitude;
        //    }
        //    set
        //    {
        //        _latitude = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _longitude;
        //public float Longitude
        //{
        //    get
        //    {
        //        return _longitude;
        //    }
        //    set
        //    {
        //        _longitude = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _compass;
        //public float Compass
        //{
        //    get
        //    {
        //        return _compass;
        //    }
        //    set
        //    {
        //        _compass = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _altitude;
        //public float Altitude
        //{
        //    get
        //    {
        //        return _altitude;
        //    }
        //    set
        //    {
        //        _altitude = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _power;
        //public float Power
        //{
        //    get
        //    {
        //        return _power;
        //    }
        //    set
        //    {
        //        _power = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _calories;
        //public float Calories
        //{
        //    get
        //    {
        //        return _calories;
        //    }
        //    set
        //    {
        //        _calories = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _gearRatio;
        //public float GearRatio
        //{
        //    get
        //    {
        //        return _gearRatio;
        //    }
        //    set
        //    {
        //        _gearRatio = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _cadence;
        //public float Cadence
        //{
        //    get
        //    {
        //        return _cadence;
        //    }
        //    set
        //    {
        //        _cadence = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private float _windSpeed;
        //public float WindSpeed
        //{
        //    get
        //    {
        //        return _windSpeed;
        //    }
        //    set
        //    {
        //        _windSpeed = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private string _windDirection;
        //public string WindDirection
        //{
        //    get
        //    {
        //        return _windDirection;
        //    }
        //    set
        //    {
        //        _windDirection = value;
        //        OnPropertyChanged();
        //    }
        //}


    }
}
