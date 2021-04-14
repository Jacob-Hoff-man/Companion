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
using System.Collections.Generic;

namespace Companion.ViewModels
{

    public class BluetoothViewModel : BaseViewModel
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

        public List<ICharacteristic> Characteristics { get; set; }
        //
        private bool IsUpdatingCharacteristics;

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
            }
        }
        //


        public BluetoothViewModel()
        {
            RecordedData = new RecordedData
            {
                Speed = 0.0F,
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

            IBLE = CrossBluetoothLE.Current;
            AdapterBLE = CrossBluetoothLE.Current.Adapter;
            DeviceList = new ObservableCollection<IDevice>();


            //event handling for BLE adapter
            AdapterBLE.DeviceDiscovered += Adapter_DeviceDiscovered;
            AdapterBLE.DeviceConnected += Adapter_DeviceConnected;
            AdapterBLE.DeviceDisconnected += Adapter_DeviceDisconnected;
            AdapterBLE.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;


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
                Debug.WriteLine("GetServices() for Guid: {0} was successful!", guid);
                return;
            }
            else
            {
                Debug.WriteLine("GetServices() failed.");
            }
        }

        public async void GetCharacteristicsList()
        {
            if(Service == null)
            {
                Debug.WriteLine("GetCharacteristics() failed. no Service was detected.");
                return;
            }

            Characteristics = new List<ICharacteristic>(await Service.GetCharacteristicsAsync());
            if(Service != null)
            {
                Debug.WriteLine("GetCharacteristics() for Service: {0} was successful!", Service);
                return;
            }
            else
            {
                Debug.WriteLine("GetCharacteristics() failed.");
            }
        }

        public void StartCharacteristicsUpdates()
        {
            IsUpdatingCharacteristics = true;

            if (Characteristics.Count == 0)
            {
                Debug.WriteLine("StartCharacteristicsUpdates() failed, there are no Characteristics found");
                return;
            }

            foreach (ICharacteristic ch in Characteristics)
            {
                Debug.WriteLine($"current ch: {ch.Name}\n");

                if (ch.CanRead)
                {
                    ch.StartUpdatesAsync();
                    ch.ValueUpdated += Characteristic_ValueUpdated;
                }
            }

            Debug.WriteLine("StartCharacteristicsUpdates() successful");

        }

        public void StopCharacteristicsUpdates()
        {


            foreach (ICharacteristic ch in Characteristics)
            {
                Debug.WriteLine($"current ch: {ch.Name}\n");

                if (ch.CanRead)
                {
                    ch.StopUpdatesAsync();
                    ch.ValueUpdated -= Characteristic_ValueUpdated;
                }
            }

            IsUpdatingCharacteristics = false;
            Debug.WriteLine("StopCharacteristicsUpdates() successful");
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

        void Characteristic_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            //Triggers any time any of the characteristics' values are updated.
            //update the specific parameter in RecordedData using switch statement
            //note: most likely need to convert the values at characteristics into floats? make conversion extension? idk how data is sent yet
            //System.BitConverter.ToSingle(Characteristic, 0) convert byte [] -> float??
            //System.Text.Encoding.UTF8.GetChars(e.Characteristic.Value) convert byte[] -> string??

            RecordedData temp = new RecordedData()
            {
                Speed = 0.0F,
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

            switch (e.Characteristic.Name)
            {
                case "Speed":
                    temp.Speed = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Distance":
                    temp.Distance = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "AverageSpeed":
                    temp.AverageSpeed = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Acceleration":
                    temp.Acceleration = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Incline":
                    temp.Incline = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Latitude":
                    temp.Latitude = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Longitude":
                    temp.Longitude = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Compass":
                    temp.Compass = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Altitude":
                    temp.Altitude = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Power":
                    temp.Power = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Calories":
                    temp.Calories = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "GearRatio":
                    temp.GearRatio = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "Cadence":
                    temp.Cadence = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "WindSpeed":
                    temp.WindSpeed = System.BitConverter.ToSingle(e.Characteristic.Value, 0);
                    break;
                case "WindDirection":
                    temp.WindDirection = System.BitConverter.ToString(e.Characteristic.Value);
                    break;
                default:
                    Debug.WriteLine("Characteristic with the following name is unknown: {0}", e.Characteristic.Name);
                    return;
            }

            this.RecordedData = temp;
            Debug.WriteLine("Characteristic with the following name was updated: {0}", e.Characteristic.Name);
            return;

        }
    }
}
