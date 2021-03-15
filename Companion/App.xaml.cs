using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Companion.Data;
using Companion.Models;

namespace Companion
{



    public partial class App : Application
    {
        static GoalProfileDatabase goalProfileDatabase;
        static TripsDatabase tripsDatabase;

        public App()
        {
            InitializeComponent();

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

        public static GoalProfileDatabase GoalProfileDatabase
        {
           get
           {
                if(goalProfileDatabase == null)
                {
                    goalProfileDatabase = new GoalProfileDatabase();
                }
                return goalProfileDatabase;

           }
        }

        public static TripsDatabase TripsDatabase
        {
            get
            {
                if(tripsDatabase == null)
                {
                    tripsDatabase = new TripsDatabase();
                }
                return tripsDatabase;

            }
        }

    }
}
