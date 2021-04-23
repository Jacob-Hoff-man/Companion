using System;
using System.Collections.Generic;
using Companion.Models;
using Companion.ViewModels;
using Xamarin.Forms;

namespace Companion.Views
{
    public partial class PerformancePage : ContentPage
    {
        PerformanceViewModel PerformanceViewModel;

        public PerformancePage(Trip trip)
        {
            InitializeComponent();
            PerformanceViewModel = new PerformanceViewModel(trip);
            BindingContext = PerformanceViewModel;
            Chart1.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.SpeedEntries };
            Chart2.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.DistanceEntries };
            Chart3.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.AverageSpeedEntries };
            Chart4.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.AccelerationEntries };
            Chart5.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.InclineEntries };
            Chart6.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.LatitudeEntries };
            Chart7.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.LongitudeEntries };
            Chart8.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.AltitudeEntries };
            Chart9.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.PowerEntries };
            Chart10.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.CaloriesEntries };
            Chart11.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.GearRatioEntries };
            Chart12.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.CadenceEntries };
            Chart13.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.TorqueEntries };
            Chart14.Chart = new Microcharts.LineChart { Entries = PerformanceViewModel.MassEntries };

        }
    }
}
