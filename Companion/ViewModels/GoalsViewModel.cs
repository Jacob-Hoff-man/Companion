using System;
using Companion.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Diagnostics;

namespace Companion.ViewModels
{
    public class GoalsViewModel : BaseViewModel
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private DateTime _date;
        public DateTime BirthDate
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        private Gender _gender;
        public Gender Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        private int _feet;
        public int Feet
        {
            get
            {
                return _feet;
            }
            set
            {
                _feet = value;
                OnPropertyChanged();
            }
        }

        private int _inches;
        public int Inches
        {
            get
            {
                return _inches;
            }
            set
            {
                _inches = value;
                OnPropertyChanged();
            }
        }

        private double _weight;
        public double Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
                OnPropertyChanged();
            }
        }

        private double _tireDiameter;
        public double TireDiameter
        {
            get
            {
                return _tireDiameter;
            }
            set
            {
                _tireDiameter = value;
                OnPropertyChanged();
            }
        }

        private GoalMode _goalMode;
        public GoalMode GoalMode
        {
            get
            {
                return _goalMode;
            }
            set
            {
                _goalMode = value;
                OnPropertyChanged();
            }
        }

        private int _targetWeight;
        public int TargetWeight
        {
            get
            {
                return _targetWeight;
            }
            set
            {
                _targetWeight = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<GoalProfile> _goalProfiles;
        public ObservableCollection<GoalProfile> GoalProfiles
        {
            get
            {
                return _goalProfiles;
            }
            set
            {
                _goalProfiles = value;
                OnPropertyChanged();
            }
        }

        //Commands
        Command _saveGoalProfileCommand;
        public Command SaveGoalProfileCommand => _saveGoalProfileCommand ?? (_saveGoalProfileCommand = new Command(Save)); 
        //



        public GoalsViewModel()
        {
            var count = 0;
            //get list of GoalProfiles stored in database, if any
            var enumerator = App.GoalProfileDatabase.GetGoalProfileEnumerated();
            if (enumerator == null)
            {
                Debug.WriteLine("No values found in GoalProfileDatabase. Adding sample value.\n");
                App.GoalProfileDatabase.SaveGoalProfile(new GoalProfile { Name = "Cricket", Weight = 69 });
            }

            //enumerator != null, while .MoveNext()
            while (enumerator.MoveNext())
            {
                Debug.WriteLine("Values detected in GoalProfileDatabase. Adding to this.GoalProfiles in GoalsViewModel\n");
                count++;
                this.GoalProfiles.Add(enumerator.Current);
                Debug.WriteLine("current value from GoalProfileDatabase: {0}\n", enumerator.Current.Name);
            }
            Debug.WriteLine("Total items added to this.GoalProfiles in GoalsViewModel: {0}\n", count);


        }

        void Save()
        {
            Debug.WriteLine("Attempting to save goal profile");

            var newGoalProfile = new GoalProfile
            {
                Name = this.Name,
                BirthDate = this.BirthDate,
                Gender = this.Gender,
                Feet = this.Feet,
                Inches = this.Inches,
                Weight = this.Weight,
                TireDiameter = this.TireDiameter,
                GoalMode = this.GoalMode,
                TargetWeight = this.TargetWeight

            };

            Debug.WriteLine("GoalProfile generated", Name);

            this.GoalProfiles.Add(newGoalProfile);
            var result = App.GoalProfileDatabase.SaveGoalProfile(newGoalProfile);

            Debug.WriteLine("GoalProfileDatabase rows added: {0}", result);
        }
    }
}
