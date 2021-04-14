using System;
using System.Diagnostics;
using Companion.Models;

namespace Companion.ViewModels
{
    public class TripViewModel : BaseViewModel
    {
        private Trip _currentTrip { get; set; }

        private RecordedData _recordedData;
        public RecordedData RecordedData
        {
            get
            {
                return _recordedData;
            }
            set
            {
                _recordedData = value;
                OnPropertyChanged();
                Debug.WriteLine("A new RecordedData was set.");
            }

        }

        public TripViewModel()
        {
            RecordedData = new RecordedData
            {
                Speed = 1.0F,
                Distance = 0.0f,
                AverageSpeed = 0.0F,
                Acceleration = 0.0F,
                Incline = 0.0F,
                Latitude = 0.0F,
                Longitude = 0.00F,
                Compass = 0.0F,
                Altitude = 0.0F,
                Power = 0.0F,
                Calories = 0.0F,
                GearRatio = 0.0F,
                Cadence = 0.0F,
                WindSpeed = 0.0F,
                WindDirection = "NW"
            };
        }


    }
}
