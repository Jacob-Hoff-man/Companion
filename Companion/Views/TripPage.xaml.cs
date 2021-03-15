using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinEssentials = Xamarin.Essentials;

namespace Companion.Views
{
    public partial class TripPage : ContentPage
    {
        private readonly IAdapter _bluetoothAdapter;
        private List<IDevice> _gattDevices = new List<IDevice>();

        public TripPage()
        {
            InitializeComponent();

            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            _bluetoothAdapter.DeviceDiscovered += (sender, foundBleDevice) =>
            {
                if (foundBleDevice.Device != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name))
                {
                    _gattDevices.Add(foundBleDevice.Device);
                }
            };
        }

        private async Task<bool> PermissionsGrantedAsync()
        {
            var locationPermissionStatus = await XamarinEssentials.Permissions.CheckStatusAsync<XamarinEssentials.Permissions.LocationAlways>();

            if (locationPermissionStatus != XamarinEssentials.PermissionStatus.Granted)
            {
                var status = await XamarinEssentials.Permissions.RequestAsync<XamarinEssentials.Permissions.LocationAlways>();
                return status == XamarinEssentials.PermissionStatus.Granted;
            }
            return true;
        }

        private async void SearchButton_Clicked(object sender, EventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchButton.IsEnabled = false);
            FoundBLEDevicesListView.ItemsSource = null;

            if (!await PermissionsGrantedAsync())
            {
                await DisplayAlert("Permission required", "Application needs location permission", "OK");
                IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchButton.IsEnabled = true);
                return;
            }

            _gattDevices.Clear();

            foreach (var device in _bluetoothAdapter.ConnectedDevices)
                _gattDevices.Add(device);

            await _bluetoothAdapter.StartScanningForDevicesAsync();

            FoundBLEDevicesListView.ItemsSource = _gattDevices.ToArray();
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchButton.IsEnabled = true);
        }

        private async void FoundBluetoothDevicesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchButton.IsEnabled = false);
            IDevice selectedItem = e.Item as IDevice;

            if (selectedItem.State == DeviceState.Connected)
            {
                await Navigation.PushAsync(new TripBluetoothDataPage(selectedItem));
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(false, true);
                    await _bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);
                    await Navigation.PushAsync(new TripBluetoothDataPage(selectedItem));
                }
                catch
                {
                    await DisplayAlert("Error connecting", $"Error connecting to BLE device: {selectedItem.Name ?? "N/A"}", "Retry");
                }
            }

            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchButton.IsEnabled = true);
        }
    }
}