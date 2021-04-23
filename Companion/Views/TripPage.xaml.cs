using Xamarin.Forms;
using Companion.ViewModels;
using System;
using Companion.Models;
using System.Threading;
using System.Diagnostics;

namespace Companion.Views
{
    public partial class TripPage : ContentPage
    {
        
        public TripPage()
        {            
            InitializeComponent();

            BindingContext =  App.BluetoothViewModel;
            //BindingContext = new TripViewModel();
            buttonClearPauseTrip.Text = "Clear";

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (App.BluetoothViewModel.BLEDevice == null)
            {
                await Navigation.PushAsync(new DeviceListPage());
            }
            else
            {
                if (App.BluetoothViewModel.Characteristics != null)
                {
                    await App.BluetoothViewModel.StartCharacteristicsUpdatesAsync();
                }
                else
                {
                    Debug.WriteLine("No Characteristics found");
                }



            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void Start_Clicked(object sender, System.EventArgs e)
        {
            //begin reading a snapshot of the data from CST every 1s, save to a RecordedEntry and save that to a Trip
            if (App.BluetoothViewModel.BLEDevice == null)
            {
                Navigation.PushAsync(new DeviceListPage());
            }
            else
            {
                //change the clear-pause button text
                buttonClearPauseTrip.Text = "Pause";
                if (App.BluetoothViewModel.CurrentTrip == null)     //Start recording a new CurrentTrip
                {
                    
                    App.BluetoothViewModel.IsDataCollecting = true;
                    //create a new trip
                    App.BluetoothViewModel.CurrentTrip = new Trip();
                    //get starting time
                    App.BluetoothViewModel.CurrentTrip.StartTime = DateTime.Now;


                    Debug.WriteLine("Starting StopWatch");
                    App.BluetoothViewModel.StopWatch.Start();

                    Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                    {
                        //Update ElapsedTime every second in the view model
                        App.BluetoothViewModel.ElapsedTime = App.BluetoothViewModel.StopWatch.Elapsed;
                        //create a new temp RecordedData
                        //copy the current RecordedData values collected from the CST onto temp
                        var temp = App.BluetoothViewModel.RecordedData;
                        //add ElapsedSeconds to temp
                        temp.ElapsedSeconds = App.BluetoothViewModel.ElapsedTime.Seconds;
                        //add temp to the end of the TripData list in the Trip
                        App.BluetoothViewModel.CurrentTrip.TripData.Add(temp);
                        Debug.WriteLine("New Recorded Entry added to trip");
                        //when IsDataCollection is true, timer loops
                        return App.BluetoothViewModel.IsDataCollecting;
                    });

                }
                else    //CurrentTrip has already existing data, re-engage timer
                {
                    App.BluetoothViewModel.IsDataCollecting = true;
                    Debug.WriteLine("Starting StopWatch");
                    App.BluetoothViewModel.StopWatch.Start();

                    Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                    {
                        //Update ElapsedTime every second in the view model
                        App.BluetoothViewModel.ElapsedTime = App.BluetoothViewModel.StopWatch.Elapsed;
                        //create a new temp RecordedData
                        //copy the current RecordedData values collected from the CST onto temp
                        var temp = App.BluetoothViewModel.RecordedData;
                        //add ElapsedSeconds to temp
                        temp.ElapsedSeconds = App.BluetoothViewModel.ElapsedTime.Seconds;
                        //add temp to the end of the TripData list in the Trip
                        App.BluetoothViewModel.CurrentTrip.TripData.Add(temp);
                        Debug.WriteLine("New Recorded Entry added to trip");
                        //when IsDataCollection is true, timer loops
                        return App.BluetoothViewModel.IsDataCollecting;
                    });
                }

            }
        }

        private void ClearPause_Clicked(object sender, System.EventArgs e)
        {
            //keep current time and do not do anything with the state of the trip
            //when IsDataCollection is true, timer stops
            if (App.BluetoothViewModel.IsDataCollecting)
            {
                App.BluetoothViewModel.IsDataCollecting = false;
                Debug.WriteLine("Stopping StopWatch");
                App.BluetoothViewModel.StopWatch.Stop();
                //change the clear-pause button text
                buttonClearPauseTrip.Text = "Clear";
            }
            else    //when IsDataCollection is false, this button sets CurrentTrip to null
            {
                App.BluetoothViewModel.CurrentTrip = null;
                Debug.WriteLine("Resetting StopWatch");
                App.BluetoothViewModel.StopWatch.Reset();
                App.BluetoothViewModel.ElapsedTime = App.BluetoothViewModel.StopWatch.Elapsed;
                Debug.WriteLine("CurrentTrip has been cleared!");

            }

        }

        private async void Finish_Clicked(object sender, System.EventArgs e)
        {
            //stop reading a snapshot of the data from CST, save the Trip into TripDatabase, disconnect the CST launch Statistics Page for new trip.
            App.BluetoothViewModel.IsDataCollecting = false;
            Debug.WriteLine("Stopping StopWatch");
            App.BluetoothViewModel.StopWatch.Stop();
            Debug.WriteLine("Resetting StopWatch");
            App.BluetoothViewModel.StopWatch.Reset();

            //get ending time
            App.BluetoothViewModel.CurrentTrip.EndTime = DateTime.Now;
            //prompt user to set goalprofile and name for trip
            App.BluetoothViewModel.CurrentTrip.Name = await DisplayPromptAsync("Add a name to the trip:", "Great Job!");
            //TODO: add a way to add GoalProfile selection here if possible

            //Add trip to database
            //await App.TripRepo.AddOrUpdateItems(App.BluetoothViewModel.CurrentTrip);
            Debug.WriteLine("Added CurrentTrip to the database!");

            //Thread.Sleep(1000);     //delay for ample time to save to database

            //set CurrentTrip to null to clear Trip Page
            var temp = App.BluetoothViewModel.CurrentTrip;   //saving copy before deleting
            App.BluetoothViewModel.CurrentTrip = null;
            App.BluetoothViewModel.ElapsedTime = App.BluetoothViewModel.StopWatch.Elapsed;
            Debug.WriteLine("CurrentTrip has been cleared!");

            //push the Performance Page of the temp CurrentTrip
            await Navigation.PushAsync(new PerformancePage(temp));

        }

        private void Instance_DeviceDisconnectedEvent(object sender, System.EventArgs e)
        {
            if (App.BluetoothViewModel.BLEDevice == null)
            {
                Navigation.PushAsync(new DeviceListPage());
            }
        }

    }

}