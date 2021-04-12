using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Companion.ViewModels;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Forms;

namespace Companion.Views
{
    public partial class DeviceListPage : ContentPage
    {

        public DeviceListPage()
        {

            InitializeComponent();
            listView.ItemsSource = BluetoothViewModel.Instance.DeviceList;
              

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BluetoothViewModel.Instance.StartScanning();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async void Device_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            BluetoothViewModel.Instance.StopScanning();
            try
            {
                BluetoothViewModel.Instance.BLEDevice = e.Item as IDevice;
                var device = e.Item as IDevice;
                if (BluetoothViewModel.Instance.AdapterBLE.ConnectedDevices.Count == 0)
                {
                    await BluetoothViewModel.Instance.AdapterBLE.ConnectToDeviceAsync(device);
                    await Navigation.PopAsync();
                }
                else
                {
                    await BluetoothViewModel.Instance.AdapterBLE.DisconnectDeviceAsync(device);
                }
            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error", "Could not connect to :" + ex.DeviceId, "OK");
            }

        }
    }
}
