using System;
using SQLite;

namespace Companion.Models
{
    public class GoalProfile
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        //user information
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public virtual Gender Gender { get; set; }
        public int Feet { get; set; }
        public int Inches { get; set; }
        public float Weight { get; set; }
        public float TireDiameter { get; set; }

        //goals information
        public virtual GoalMode GoalMode { get; set; }
        public int TargetWeight { get; set; }

        public GoalProfile()
        {
        }

    }

    public enum Gender { male, female, unspecified }
    public enum GoalMode { loseWeight, increaseSpeed, increaseDistance, marathon }

}
