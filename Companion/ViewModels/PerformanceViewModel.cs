using System;
using Companion.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Diagnostics;
using Microcharts.Forms;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Companion.ViewModels
{
    public class PerformanceViewModel : BaseViewModel
    {

        private ObservableCollection<Trip> _trips;
        public ObservableCollection<Trip> Trips
        {
            get
            {
                return _trips;
            }
            set
            {
                _trips = value;
                OnPropertyChanged();
            }
        }

        public Trip displayTrip;
        public int Count;

        public List<Microcharts.ChartEntry> SpeedEntries;
        public List<Microcharts.ChartEntry> DistanceEntries;
        public List<Microcharts.ChartEntry> AverageSpeedEntries;
        public List<Microcharts.ChartEntry> AccelerationEntries;
        public List<Microcharts.ChartEntry> InclineEntries;
        public List<Microcharts.ChartEntry> LatitudeEntries;
        public List<Microcharts.ChartEntry> LongitudeEntries;
        public List<Microcharts.ChartEntry> AltitudeEntries;
        public List<Microcharts.ChartEntry> PowerEntries;
        public List<Microcharts.ChartEntry> CaloriesEntries;
        public List<Microcharts.ChartEntry> GearRatioEntries;
        public List<Microcharts.ChartEntry> CadenceEntries;
        public List<Microcharts.ChartEntry> TorqueEntries;
        public List<Microcharts.ChartEntry> MassEntries;


        public PerformanceViewModel(Trip trip)
        {
            displayTrip = trip;
            Count = trip.TripData.Count;

            var speed = new float[Count];
            var distance = new float[Count];
            var averageSpeed = new float[Count];
            var acceleration = new float[Count];
            var incline = new float[Count];
            var latitude = new float[Count];
            var longitude = new float[Count];
            var altitude = new float[Count];
            var power = new float[Count];
            var calories = new float[Count];
            var gearRatio = new float[Count];
            var cadence = new float[Count];
            var torque = new float[Count];
            var mass = new float[Count];


            for (int i = 0; i < Count; i++)
            {
                speed[i] = trip.TripData[i].Speed;
                distance[i] = trip.TripData[i].Distance;
                averageSpeed[i] = trip.TripData[i].AverageSpeed;
                acceleration[i] = trip.TripData[i].Acceleration;
                incline[i] = trip.TripData[i].Incline;
                latitude[i] = trip.TripData[i].Latitude;
                longitude[i] = trip.TripData[i].Longitude;
                altitude[i] = trip.TripData[i].Altitude;
                power[i] = trip.TripData[i].Power;
                calories[i] = trip.TripData[i].Calories;
                gearRatio[i] = trip.TripData[i].GearRatio;
                cadence[i] = trip.TripData[i].Cadence;
                torque[i] = trip.TripData[i].Torque;
                mass[i] = trip.TripData[i].Mass;
            }

            SpeedEntries = new List<Microcharts.ChartEntry>();
            DistanceEntries = new List<Microcharts.ChartEntry>();
            AverageSpeedEntries = new List<Microcharts.ChartEntry>();
            AccelerationEntries = new List<Microcharts.ChartEntry>();
            InclineEntries = new List<Microcharts.ChartEntry>();
            LatitudeEntries = new List<Microcharts.ChartEntry>();
            LongitudeEntries = new List<Microcharts.ChartEntry>();
            AltitudeEntries = new List<Microcharts.ChartEntry>();
            PowerEntries = new List<Microcharts.ChartEntry>();
            CaloriesEntries = new List<Microcharts.ChartEntry>();
            GearRatioEntries = new List<Microcharts.ChartEntry>();
            CadenceEntries = new List<Microcharts.ChartEntry>();
            TorqueEntries = new List<Microcharts.ChartEntry>();
            MassEntries = new List<Microcharts.ChartEntry>();

            //var list = new List<List<Microcharts.ChartEntry>>()
            //{
            //    SpeedEntries,
            //    DistanceEntries,
            //    AverageSpeedEntries,
            //    AccelerationEntries,
            //    InclineEntries,
            //    LatitudeEntries,
            //    LongitudeEntries,
            //    AltitudeEntries,
            //    PowerEntries,
            //    CaloriesEntries,
            //    GearRatioEntries,
            //    CadenceEntries,
            //    TorqueEntries,
            //    MassEntries
            //};

            //foreach(List<Microcharts.ChartEntry> entries in list)
            //{
            //    foreach(float i in )
            //}

            foreach(float i in speed)
            {
                var entry = new Microcharts.ChartEntry(i);
                SpeedEntries.Add(entry);
            }

            foreach (float i in distance)
            {
                var entry = new Microcharts.ChartEntry(i);
                DistanceEntries.Add(entry);
            }

            foreach (float i in averageSpeed)
            {
                var entry = new Microcharts.ChartEntry(i);
                AverageSpeedEntries.Add(entry);
            }

            foreach (float i in acceleration)
            {
                var entry = new Microcharts.ChartEntry(i);
                AccelerationEntries.Add(entry);
            }

            foreach (float i in incline)
            {
                var entry = new Microcharts.ChartEntry(i);
                InclineEntries.Add(entry);
            }

            foreach (float i in latitude)
            {
                var entry = new Microcharts.ChartEntry(i);
                LatitudeEntries.Add(entry);
            }

            foreach (float i in longitude)
            {
                var entry = new Microcharts.ChartEntry(i);
                LongitudeEntries.Add(entry);
            }

            foreach (float i in altitude)
            {
                var entry = new Microcharts.ChartEntry(i);
                AltitudeEntries.Add(entry);
            }

            foreach (float i in power)
            {
                var entry = new Microcharts.ChartEntry(i);
                PowerEntries.Add(entry);
            }

            foreach (float i in calories)
            {
                var entry = new Microcharts.ChartEntry(i);
                CaloriesEntries.Add(entry);
            }

            foreach (float i in gearRatio)
            {
                var entry = new Microcharts.ChartEntry(i);
                GearRatioEntries.Add(entry);
            }

            foreach (float i in cadence)
            {
                var entry = new Microcharts.ChartEntry(i);
                CadenceEntries.Add(entry);
            }

            foreach (float i in torque)
            {
                var entry = new Microcharts.ChartEntry(i);
                TorqueEntries.Add(entry);
            }

            foreach (float i in mass)
            {
                var entry = new Microcharts.ChartEntry(i);
                MassEntries.Add(entry);
            }

            


            //InitAsync();
        }

        async void InitAsync()
        {
            //initializing the trip repo and loading in the database
            await App.TripRepo.Initialize();
            Trips = new ObservableCollection<Trip>(await App.TripRepo.GetRepoItems());
        }
    }
}
