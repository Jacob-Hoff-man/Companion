using System;
using System.Collections.Generic;
using Companion.Models;
using Companion.ViewModels;

using Xamarin.Forms;

namespace Companion.Views
{
    public partial class TripListPage : ContentPage
    {
        PerformanceViewModel PerformanceViewModel;

        public TripListPage()
        {
            InitializeComponent();
            //PerformanceViewModel = new PerformanceViewModel();
            //BindingContext = PerformanceViewModel;
        }

        async void Trip_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new PerformancePage(e.Item as Trip));
        }
    }
}
