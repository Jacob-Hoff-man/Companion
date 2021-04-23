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
        
        //bluetooth parameters
        //
        public IBluetoothLE IBLE;
        public IAdapter AdapterBLE { get; set; }
        public IDevice BLEDevice { get; set; }
        public ObservableCollection<IDevice> DeviceList { get; set; }
        public List<IService> Services { get; set; }
        public List<ICharacteristic> Characteristics { get; set; }
        public bool IsUpdatingCharacteristics = false;
        //


        //recording trip data parameters
        //
        public Trip CurrentTrip;
        public bool IsDataCollecting = false;
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
                //Debug.WriteLine("RecordedData was changed.\n");
                //Debug.WriteLine(this.RecordedData.Speed);
                //Debug.WriteLine(this.RecordedData.Distance);
                //Debug.WriteLine(this.RecordedData.AverageSpeed);
                //Debug.WriteLine(this.RecordedData.Acceleration);
                //Debug.WriteLine(this.RecordedData.Incline);
                //Debug.WriteLine(this.RecordedData.Latitude);
                //Debug.WriteLine(this.RecordedData.Longitude);
                //Debug.WriteLine(this.RecordedData.Compass);
                //Debug.WriteLine(this.RecordedData.Altitude);
                //Debug.WriteLine(this.RecordedData.Power);
                //Debug.WriteLine(this.RecordedData.Calories);
                //Debug.WriteLine(this.RecordedData.GearRatio);
                //Debug.WriteLine(this.RecordedData.WindSpeed);
                //Debug.WriteLine(this.RecordedData.WindDirection);
            }
        }

        public Stopwatch StopWatch;
        private TimeSpan _elapsedTime;
        public TimeSpan ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
            set
            {
                _elapsedTime = value;
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
                Torque = 0F,
                WindSpeed = 0.0F,
                WindDirection = 0.0F
            };
            StopWatch = new Stopwatch();

            IBLE = CrossBluetoothLE.Current;
            AdapterBLE = CrossBluetoothLE.Current.Adapter;
            DeviceList = new ObservableCollection<IDevice>();

            //event handling for BLE adapter
            AdapterBLE.DeviceDiscovered += Adapter_DeviceDiscovered;
            AdapterBLE.DeviceConnected += Adapter_DeviceConnected;
            AdapterBLE.DeviceDisconnected += Adapter_DeviceDisconnected;
            AdapterBLE.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            IBLE.StateChanged += (s, e) =>
            {
                Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
            };

        }

        //BLE stuff
        //
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

        //public async void GetService(Guid guid)
        //{
        //    if (BLEDevice == null)
        //    {
        //        Debug.WriteLine("GetServices() failed. no BLEDevice detected.");
        //        return;
        //    }

        //    Service = await BLEDevice.GetServiceAsync(guid);
        //    if (Service != null)
        //    {
        //        Debug.WriteLine("GetServices() for Guid: {0} was successful!", guid);
        //        return;
        //    }
        //    else
        //    {
        //        Debug.WriteLine("GetServices() failed.");
        //    }
        //}

        public async void GetCharacteristicsList()
        {
            if (Services == null)
            {
                Debug.WriteLine("GetCharacteristicsList() failed. no Services detected.");
                return;
            }

            Characteristics = new List<ICharacteristic>();

            foreach (IService service in Services)
            {
                //at this point, Services should only contain the necessary services
                //get all characteristics
                var temp = await service.GetCharacteristicsAsync();
                foreach(ICharacteristic ch in temp)
                {
                    Characteristics.Add(ch);
                    Debug.WriteLine("{ch} added to Characteristics list");
                }
            }

            if(Characteristics != null)
            {
                Debug.WriteLine("GetCharacteristicsList() made some characteristics available");
                return;
            }
            else
            {
                Debug.WriteLine("GetCharacteristicsList() failed.");
            }
        }

        public async Task StartCharacteristicsUpdatesAsync()
        {
            IsUpdatingCharacteristics = true;
            
            if (Characteristics.Count == 0)
            {
                Debug.WriteLine("StartCharacteristicsUpdates() failed, there are no Characteristics found");
                return;
            }

            List<Task> StartUpdatesTasks = new List<Task>();
            foreach (ICharacteristic ch in Characteristics)
            {
                Debug.WriteLine($"current ch: {ch.Id}\n");


                if (ch.CanRead)
                {

                    switch(ch.Id.ToString())
                    {
                        case GattIdentifiers.SpeedID:
                            ch.ValueUpdated += CharacteristicSpeed_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Speed Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.DistanceID:
                            ch.ValueUpdated += CharacteristicDistance_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Distance Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.AverageSpeedID:
                            ch.ValueUpdated += CharacteristicAverageSpeed_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("AverageSpeed Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.AccelerationID:
                            ch.ValueUpdated += CharacteristicAcceleration_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Acceleration Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.InclineID:
                            ch.ValueUpdated += CharacteristicIncline_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Incline Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.LatitudeID:
                            ch.ValueUpdated += CharacteristicLatitude_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Latitude Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.LongitudeID:
                            ch.ValueUpdated += CharacteristicLongitude_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Longitude Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.CompassID:
                            ch.ValueUpdated += CharacteristicCompass_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Compass Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.AltitudeID:
                            ch.ValueUpdated += CharacteristicAltitude_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Altitude Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.PowerID:
                            ch.ValueUpdated += CharacteristicPower_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Power Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.CaloriesID:
                            ch.ValueUpdated += CharacteristicCalories_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Calories Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.GearRatioID:
                            ch.ValueUpdated += CharacteristicGearRatio_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("GearRatio Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.CadenceID:
                            ch.ValueUpdated += CharacteristicCadence_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Cadence Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.TorqueID:
                            ch.ValueUpdated += CharacteristicTorque_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("Torque Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.WindSpeedID:
                            ch.ValueUpdated += CharacteristicWindSpeed_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("WindSpeed Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.WindDirectionID:
                            ch.ValueUpdated += CharacteristicWindDirection_ValueUpdated;
                            await ch.StartUpdatesAsync();
                            Debug.WriteLine("WindDirection Characteristic connected: {0}", ch.Id.ToString());
                            break;
                        default:
                            Debug.WriteLine("Characteristic with the following name is unknown: {0}", ch.Id);
                            return;
                    }
                }
                else
                {
                    Debug.WriteLine("Could not read ch", ch.Id.ToString());
                }
            }

            Debug.WriteLine("StartCharacteristicsUpdates() successful");

        }

        public void StopCharacteristicsUpdates()
        {


            foreach (ICharacteristic ch in Characteristics)
            {
                Debug.WriteLine($"current ch: {0}\n", ch.Id.ToString());

                if (ch.CanRead)
                {

                    switch (ch.Id.ToString())
                    {
                        case GattIdentifiers.SpeedID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicSpeed_ValueUpdated;
                            Debug.WriteLine("Speed Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.DistanceID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicDistance_ValueUpdated;
                            Debug.WriteLine("Distance Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.AverageSpeedID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicAverageSpeed_ValueUpdated;
                            Debug.WriteLine("AverageSpeed Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.AccelerationID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicAcceleration_ValueUpdated;
                            Debug.WriteLine("Acceleration Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.InclineID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicIncline_ValueUpdated;
                            Debug.WriteLine("Incline Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.LatitudeID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicLatitude_ValueUpdated;
                            Debug.WriteLine("Latitude Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.LongitudeID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicLongitude_ValueUpdated;
                            Debug.WriteLine("Longitude Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.CompassID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicCompass_ValueUpdated;
                            Debug.WriteLine("Compass Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.AltitudeID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicAltitude_ValueUpdated;
                            Debug.WriteLine("Altitude Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.PowerID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicPower_ValueUpdated;
                            Debug.WriteLine("Power Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.CaloriesID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicCalories_ValueUpdated;
                            Debug.WriteLine("Calories Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.GearRatioID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicGearRatio_ValueUpdated;
                            Debug.WriteLine("GearRatio Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.CadenceID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicCadence_ValueUpdated;
                            Debug.WriteLine("Cadence Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.TorqueID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicTorque_ValueUpdated;
                            Debug.WriteLine("Torque Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.WindSpeedID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicWindSpeed_ValueUpdated;
                            Debug.WriteLine("WindSpeed Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        case GattIdentifiers.WindDirectionID:
                            ch.StopUpdatesAsync();
                            ch.ValueUpdated -= CharacteristicWindDirection_ValueUpdated;
                            Debug.WriteLine("WindDirection Characteristic disconnected: {0}", ch.Id.ToString());
                            break;
                        default:
                            Debug.WriteLine("Characteristic with the following name is unknown: {0}", ch.Id);
                            return;
                    }
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

        //Adapter Event Handlers
        //
        void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            //The CST's BLE broadcast includes a name, this will filter our DeviceList when searching
            if (e.Device.Name != null)
            {
                DeviceList.Add(e.Device);
                Debug.WriteLine("Added new device");
            }

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
        //

        //Characteristic Event Handlers
        //Triggers any time any of the characteristics' values are updated.
        //note: most likely need to convert the values at characteristics into floats? make conversion extension? idk how data is sent yet
        //System.BitConverter.ToSingle(Characteristic, 0) convert byte [] -> float??
        //System.Text.Encoding.UTF8.GetChars(e.Characteristic.Value) convert byte[] -> string??
        void CharacteristicSpeed_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Speed = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //    this.RecordedData = temp;
            //});
            this.RecordedData = temp;

            Debug.WriteLine("Speed Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicDistance_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Distance = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            //    this.RecordedData = temp;
            //});

            Debug.WriteLine("Distance Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicAverageSpeed_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.AverageSpeed = System.BitConverter.ToSingle(e.Characteristic.Value, 0);


            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("AverageSpeed Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicAcceleration_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Acceleration = System.BitConverter.ToSingle(e.Characteristic.Value, 0);


            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Acceleration Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicIncline_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Incline = System.BitConverter.ToSingle(e.Characteristic.Value, 0);


            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Incline Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicLatitude_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Latitude = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Latitude Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicLongitude_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Longitude = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Longitude Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicCompass_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Compass = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Compass Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicAltitude_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Altitude = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Altitude Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicPower_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Power = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Power Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicCalories_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Calories = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Calories Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicGearRatio_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.GearRatio = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("GearRatio Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicCadence_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Cadence = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Cadence Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicTorque_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.Torque = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("Torque Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicWindSpeed_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            temp.WindSpeed = System.BitConverter.ToSingle(e.Characteristic.Value, 0);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("WindSpeed Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }

        void CharacteristicWindDirection_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            RecordedData temp = this.RecordedData;
            //temp.WindDirection = System.Text.Encoding.UTF8.GetString(e.Characteristic.Value);
            System.BitConverter.ToSingle(e.Characteristic.Value, 0);    //changed WindDirection to a float

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.RecordedData = temp;
            });

            Debug.WriteLine("WindDirection Characteristic was updated: {0}", e.Characteristic.Id.ToString());
            return;
        }
        //
    }
}
