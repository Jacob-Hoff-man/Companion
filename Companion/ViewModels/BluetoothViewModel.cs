using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading;
using System.Threading.Tasks;

using Companion.Models;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;
using Plugin.BLE.Abstractions.EventArgs;

using System.Windows.Input;

namespace Companion.ViewModels
{

    public class BluetoothViewModel
    {
        //singleton
        private static readonly Lazy<BluetoothViewModel> LazyViewModel = new Lazy<BluetoothViewModel>(() => new BluetoothViewModel());
        public static BluetoothViewModel Instance
        {
            get { return LazyViewModel.Value; }
        }
        //
        //bluetooth parameters
        public IBluetoothLE IBLE;
        public IAdapter AdapterBLE { get; set; }
        public IDevice BLEDevice { get; set; }
        public ObservableCollection<IDevice> DeviceList { get; set; }
        public IService Service { get; set; }
        //
        //Characteristics... Janky way of doing this :(
        public ICharacteristic Characteristic_Speed { get; set; }
        public ICharacteristic Characteristic_Distance { get; set; }
        public ICharacteristic Characteristic_AverageSpeed { get; set; }
        public ICharacteristic Characteristic_Acceleration { get; set; }
        public ICharacteristic Characteristic_Incline { get; set; }
        public ICharacteristic Characteristic_Latitude { get; set; }
        public ICharacteristic Characteristic_Longitude { get; set; }
        public ICharacteristic Characteristic_Compass { get; set; }
        public ICharacteristic Characteristic_Altitude { get; set; }
        public ICharacteristic Characteristic_Power { get; set; }
        public ICharacteristic Characteristic_Calories { get; set; }
        public ICharacteristic Characteristic_GearRatio { get; set; }
        public ICharacteristic Characteristic_Cadence { get; set; }
        public ICharacteristic Characteristic_WindSpeed { get; set; }
        public ICharacteristic Characteristic_WindDirection { get; set; }
        //
        private bool IsUpdatingCharacteristics;
        public RecordedData RecordedData { get; set; }
        //


        public BluetoothViewModel()
        {
            
            IBLE = CrossBluetoothLE.Current;
            AdapterBLE = CrossBluetoothLE.Current.Adapter;
            DeviceList = new ObservableCollection<IDevice>();


            //event handling for BLE adapter
            AdapterBLE.DeviceDiscovered += Adapter_DeviceDiscovered;
            AdapterBLE.DeviceConnected += Adapter_DeviceConnected;
            AdapterBLE.DeviceDisconnected += Adapter_DeviceDisconnected;
            AdapterBLE.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;


        }

        void Save()
        {
            //i don't know if this conversion is going to work naturally, but eh fuck it lessssgo
            var newRecordedData = new RecordedData
            {
                Speed = System.BitConverter.ToSingle(Characteristic_Speed.Value, 0),
                Distance = System.BitConverter.ToSingle(Characteristic_Distance.Value, 0),
                AverageSpeed = System.BitConverter.ToSingle(Characteristic_AverageSpeed.Value, 0),
                Acceleration = System.BitConverter.ToSingle(Characteristic_Acceleration.Value, 0),
                Incline = System.BitConverter.ToSingle(Characteristic_Incline.Value, 0),
                Latitude = System.BitConverter.ToSingle(Characteristic_Latitude.Value, 0),
                Longitude = System.BitConverter.ToSingle(Characteristic_Longitude.Value, 0),
                Compass = System.BitConverter.ToSingle(Characteristic_Compass.Value, 0),
                Altitude = System.BitConverter.ToSingle(Characteristic_Altitude.Value, 0),
                Power = System.BitConverter.ToSingle(Characteristic_Power.Value, 0),
                Calories = System.BitConverter.ToSingle(Characteristic_Calories.Value, 0),
                GearRatio = System.BitConverter.ToSingle(Characteristic_GearRatio.Value, 0),
                Cadence = System.BitConverter.ToSingle(Characteristic_Cadence.Value, 0),
                WindSpeed = System.BitConverter.ToSingle(Characteristic_WindSpeed.Value, 0),
                WindDirection = "NW"    //**need to make byte [] into string or something **
            };

            RecordedData = newRecordedData;
        }

        //BLE stuff

        private string GetStateText()
        {
            switch (IBLE.State)
            {
                case BluetoothState.Unknown:
                    return "Unknown BLE state.";
                case BluetoothState.Unavailable:
                    return "BLE is not available on this device.";
                case BluetoothState.Unauthorized:
                    return "You are not allowed to use BLE.";
                case BluetoothState.TurningOn:
                    return "BLE is warming up, please wait.";
                case BluetoothState.On:
                    return "BLE is on.";
                case BluetoothState.TurningOff:
                    return "BLE is turning off. That's sad!";
                case BluetoothState.Off:
                    return "BLE is off. Turn it on!";
                default:
                    return "Unknown BLE state.";
            }
        }

        public async void StartCharacteristicsUpdates()
        {
            try
            {
                IsUpdatingCharacteristics = true;

                Characteristic_Speed.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Distance.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_AverageSpeed.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Acceleration.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Incline.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Latitude.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Longitude.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Compass.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Altitude.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Power.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Calories.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_GearRatio.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Cadence.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_WindSpeed.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_WindDirection.ValueUpdated -= Characteristic_ValueUpdated;

                Characteristic_Speed.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Distance.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_AverageSpeed.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Acceleration.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Incline.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Latitude.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Longitude.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Compass.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Altitude.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Power.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Calories.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_GearRatio.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_Cadence.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_WindSpeed.ValueUpdated += Characteristic_ValueUpdated;
                Characteristic_WindDirection.ValueUpdated += Characteristic_ValueUpdated;

                Debug.WriteLine("Starting Characteristic Updates");
                await Characteristic_Speed.StartUpdatesAsync();
                await Characteristic_Distance.StartUpdatesAsync();
                await Characteristic_AverageSpeed.StartUpdatesAsync();
                await Characteristic_Acceleration.StartUpdatesAsync();
                await Characteristic_Incline.StartUpdatesAsync();
                await Characteristic_Latitude.StartUpdatesAsync();
                await Characteristic_Longitude.StartUpdatesAsync();
                await Characteristic_Compass.StartUpdatesAsync();
                await Characteristic_Altitude.StartUpdatesAsync();
                await Characteristic_Power.StartUpdatesAsync();
                await Characteristic_Calories.StartUpdatesAsync();
                await Characteristic_GearRatio.StartUpdatesAsync();
                await Characteristic_Cadence.StartUpdatesAsync();
                await Characteristic_WindSpeed.StartUpdatesAsync();
                await Characteristic_WindDirection.StartUpdatesAsync();

            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while running StartCharacteristicsUpdates():  " + e.Message);
            }
        }

        public async void StopCharacteristicsUpdates()
        {
            try
            {
                IsUpdatingCharacteristics = false;

                Debug.WriteLine("Stopping Characteristics Updates");
                await Characteristic_Speed.StopUpdatesAsync();
                await Characteristic_Distance.StopUpdatesAsync();
                await Characteristic_AverageSpeed.StopUpdatesAsync();
                await Characteristic_Acceleration.StopUpdatesAsync();
                await Characteristic_Incline.StopUpdatesAsync();
                await Characteristic_Latitude.StopUpdatesAsync();
                await Characteristic_Longitude.StopUpdatesAsync();
                await Characteristic_Compass.StopUpdatesAsync();
                await Characteristic_Altitude.StopUpdatesAsync();
                await Characteristic_Power.StopUpdatesAsync();
                await Characteristic_Calories.StopUpdatesAsync();
                await Characteristic_GearRatio.StopUpdatesAsync();
                await Characteristic_Cadence.StopUpdatesAsync();
                await Characteristic_WindSpeed.StopUpdatesAsync();
                await Characteristic_WindDirection.StopUpdatesAsync();

                Characteristic_Speed.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Distance.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_AverageSpeed.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Acceleration.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Incline.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Latitude.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Longitude.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Compass.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Altitude.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Power.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Calories.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_GearRatio.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_Cadence.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_WindSpeed.ValueUpdated -= Characteristic_ValueUpdated;
                Characteristic_WindDirection.ValueUpdated -= Characteristic_ValueUpdated;

            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while running StopCharacteristicsUpdates():  " + e.Message);
            }
        }

        public async void GetService(Guid guid)
        {
            if (BLEDevice == null)
            {
                Debug.WriteLine("GetServices() failed. no BLEDevice detected.");
                return;
            }

            Service = await BLEDevice.GetServiceAsync(guid);
            if (Service != null)
            {
                Debug.WriteLine("GetServices() for Guid: {guid} was successful!");
            }
            else
            {
                Debug.WriteLine("GetServices() failed.");
            }
        }

        public async void GetCharacteristics()
        {
            if (Service == null)
            {
                Debug.WriteLine("GetServices() failed. No Service was found.");
                return;
            }

            Characteristic_Speed = await Service.GetCharacteristicAsync(GattIdentifiers.SpeedID);
            Characteristic_Distance = await Service.GetCharacteristicAsync(GattIdentifiers.DistanceID);
            Characteristic_AverageSpeed = await Service.GetCharacteristicAsync(GattIdentifiers.AverageSpeedID);
            Characteristic_Acceleration = await Service.GetCharacteristicAsync(GattIdentifiers.AccelerationID);
            Characteristic_Incline = await Service.GetCharacteristicAsync(GattIdentifiers.InclineID);
            Characteristic_Latitude = await Service.GetCharacteristicAsync(GattIdentifiers.LatitudeID);
            Characteristic_Longitude = await Service.GetCharacteristicAsync(GattIdentifiers.LongitudeID);
            Characteristic_Compass = await Service.GetCharacteristicAsync(GattIdentifiers.CompassID);
            Characteristic_Altitude = await Service.GetCharacteristicAsync(GattIdentifiers.AltitudeID);
            Characteristic_Power = await Service.GetCharacteristicAsync(GattIdentifiers.PowerID);
            Characteristic_Calories = await Service.GetCharacteristicAsync(GattIdentifiers.CaloriesID);
            Characteristic_GearRatio = await Service.GetCharacteristicAsync(GattIdentifiers.GearRatioID);
            Characteristic_Cadence = await Service.GetCharacteristicAsync(GattIdentifiers.CadenceID);
            Characteristic_WindSpeed = await Service.GetCharacteristicAsync(GattIdentifiers.WindSpeedID);
            Characteristic_WindDirection = await Service.GetCharacteristicAsync(GattIdentifiers.WindDirectionID);


            //this is mainly for debugging
            if (Characteristic_Speed == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Distance == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_AverageSpeed == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Acceleration == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Incline == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Latitude == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Longitude == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Compass == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Altitude == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Power == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Calories == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_GearRatio == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_Cadence == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_WindSpeed == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            if (Characteristic_WindDirection == null)
            {
                Debug.WriteLine("GetCharacteristics() failed to get one or more characteristics!");
                return;
            }

            //


        }

        public async void StartScanning()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                AdapterBLE.ScanMode = ScanMode.Balanced;
           

                if(!IBLE.Adapter.IsScanning)
                {
                    Debug.WriteLine("Beginning a scan!");
                    await AdapterBLE.StartScanningForDevicesAsync();
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception in StartScanning():  " + e.Message);
            }

        }

        public async void StopScanning()
        {
            
            if (AdapterBLE.IsScanning)
            {
                Debug.WriteLine("Still scanning, stopping the scan");
                await AdapterBLE.StopScanningForDevicesAsync();
            }
            else
            {
                Debug.WriteLine("Adapter is not scanning, but StopScanning() is called");
            }    
        }


        public void DisconnectDevice()
        {
            if (BLEDevice != null)
            {
                AdapterBLE.DisconnectDeviceAsync(BLEDevice);
                BLEDevice.Dispose();
                BLEDevice = null;
                Debug.WriteLine("DisconnectDevice() complete");
            }
        }

        //

        void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            DeviceList.Add(e.Device);
            Debug.WriteLine("Added new device");
        }

        void Adapter_DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine("Device already connected");
            
        }

        void Adapter_DeviceDisconnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            //DeviceDisconnectedEvent?.Invoke(sender,e);
            Debug.WriteLine("Device already disconnected");
        }

        void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            AdapterBLE.StopScanningForDevicesAsync();
            Debug.WriteLine("Timeout", "Bluetooth scan timeout elapsed");
        }

        void Characteristic_ValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            //Triggers any time any of the characteristics' values are updated.
            //Create a new RecordedEntry and update it with all of the Characteristics
            //note: most likely need to convert the values at characteristics into floats? make conversion extension? idk how data is sent yet
            Debug.WriteLine("A Characteristic value was updated");

            Save();

            Debug.WriteLine("A new RecordedData was generated");
        }


    }
}
