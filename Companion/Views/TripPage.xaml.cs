using Xamarin.Forms;
using Companion.ViewModels;
using System;
using Companion.Models;

namespace Companion.Views
{
    public partial class TripPage : ContentPage
    {

        private bool IsDataCollecting = false;

        //returns the value of the page's binding context
        private TripViewModel TripViewModel => BindingContext as TripViewModel;

        public TripPage()
        {            
            InitializeComponent();

            BindingContext =  BluetoothViewModel.Instance.RecordedData;
            //BindingContext = new TripViewModel();

        }

        private void InitializeData()
        {

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BluetoothViewModel.Instance.BLEDevice == null)
            {
                //Navigation.PushAsync(new DeviceListPage());
            }
            else
            {
                //there is a BLEDevice connected
                //get CST Service
                BluetoothViewModel.Instance.GetService(GattIdentifiers.CSTServiceID);

                //get all characteristics from CST Service loaded
                BluetoothViewModel.Instance.GetCharacteristicsList();

                //start updating characteristics and observe for updates
                BluetoothViewModel.Instance.StartCharacteristicsUpdates();


            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }



        private void Start_Clicked(object sender, System.EventArgs e)
        {
            //begin reading a snapshot of the data from CST every 1s, save to a RecordedEntry and save that to a Trip
            if (BluetoothViewModel.Instance.BLEDevice == null)
            {
                Navigation.PushAsync(new DeviceListPage());
            }
            else
            {
                IsDataCollecting = true;
                //here
                //Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                //{

                //});
            }
        }

        private void Pause_Clicked(object sender, System.EventArgs e)
        {
            
            TripViewModel.RecordedData = new RecordedData
            {
                Speed = 69.0F,
                Distance = 12.345F,
                AverageSpeed = 0.02F,
                Acceleration = 0.00F,
                Incline = 3.70F,
                Latitude = 0.00F,
                Longitude = 0.001F,
                Compass = 20.00F,
                Altitude = 4320F,
                Power = 0.0F,
                Calories = 2.20F,
                GearRatio = 0.50F,
                Cadence = 24.6F,
                WindSpeed = 3.39F,
                WindDirection = "NW"


            };

            //stop reading a snapshot of the data from CST every 1s, wait until start/finish is clicked.
            if (BluetoothViewModel.Instance.BLEDevice != null)
            {

            }


        }

        private void Finish_Clicked(object sender, System.EventArgs e)
        {
            //stop reading a snapshot of the data from CST every 1s, disconnect from CST, save the Trip into TripDatabase, launch Statistics Page for new trip.
            if (BluetoothViewModel.Instance.BLEDevice != null)
            {

            }

        }

        private void Instance_DeviceDisconnectedEvent(object sender, System.EventArgs e)
        {
            if (BluetoothViewModel.Instance.BLEDevice == null)
            {
                Navigation.PushAsync(new DeviceListPage());
            }
        }

    }

}