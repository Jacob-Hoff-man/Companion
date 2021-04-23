using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Companion.ViewModels;
using Companion.Models;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Companion.Views
{
    public partial class DeviceListPage : ContentPage
    {

        public DeviceListPage()
        {
            InitializeComponent();
            listView.ItemsSource = App.BluetoothViewModel.DeviceList;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.BluetoothViewModel.StartScanning();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.BluetoothViewModel.DeviceList.Clear();
        }

        async void Device_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            App.BluetoothViewModel.StopScanning();

            try
            {
                var device = e.Item as IDevice;
                App.BluetoothViewModel.BLEDevice = device;

                await App.BluetoothViewModel.AdapterBLE.ConnectToDeviceAsync(device);

            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error", "Could not connect to :" + ex.DeviceId, "OK");
                App.BluetoothViewModel.BLEDevice = null;
                await Navigation.PopAsync();
                return;
            }

            Debug.WriteLine(App.BluetoothViewModel.BLEDevice.Id);

            var services = await App.BluetoothViewModel.BLEDevice.GetServicesAsync() as List<IService>;
            App.BluetoothViewModel.Services = new List<IService>();
            foreach (IService s in services)
            {
                Debug.WriteLine(s.Id);

                //if it is CSTService, GPSService, or PedalService, add to App Services list.
                if ((s.Id.ToString() == GattIdentifiers.CSTServiceID) || (s.Id.ToString() == GattIdentifiers.GPSServiceID) || (s.Id.ToString() == GattIdentifiers.PedalServiceID))
                {
                    App.BluetoothViewModel.Services.Add(s);
                }

            }

            //if 
            //App.BluetoothViewModel.Service = services[2];

            App.BluetoothViewModel.GetCharacteristicsList();

            foreach (ICharacteristic ch in App.BluetoothViewModel.Characteristics)
            {
                Debug.WriteLine(ch.Id);
            }

            await Navigation.PopAsync();

        }
    }
}
