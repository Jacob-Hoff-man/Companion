using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Companion.Data;
using Companion.Models;
using Companion.ViewModels;

namespace Companion
{



    public partial class App : Application
    {
        //Singleton Data allocation - Repos will initialize with constructor of corresponding page
        public static GoalProfileRepo GoalProfileRepo;
        public static TripRepo TripRepo;

        //Singleton BluetoothViewModel
        public static BluetoothViewModel BluetoothViewModel; 

        public App()
        {
            InitializeComponent();
            GoalProfileRepo = new GoalProfileRepo();
            TripRepo = new TripRepo();
            BluetoothViewModel = new BluetoothViewModel();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}
