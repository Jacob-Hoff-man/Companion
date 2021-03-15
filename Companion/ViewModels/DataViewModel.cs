
using Companion.Models;
using System.Collections.ObjectModel;

namespace Companion.ViewModels
{
    public class DataViewModel
    {
        public ObservableCollection<GoalProfile> GoalProfiles { get; set; }
        public ObservableCollection<Trip> Trips { get; set; }

        public DataViewModel()
        {
            GoalProfiles = new ObservableCollection<GoalProfile>();
            var enumerator = App.GoalProfileDatabase.GetGoalProfileEnumerated();

            if (enumerator == null)
            {
                App.GoalProfileDatabase.SaveGoalProfile(new GoalProfile { Name = "Cricket", Weight = 69 });
            }

            while (enumerator.MoveNext())
            {
                this.GoalProfiles.Add(enumerator.Current);
            }

        }

        public void AddGoalProfile()
        {
            
        }
    }
}
