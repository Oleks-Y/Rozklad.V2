using System;

namespace Rozklad.V2.Helpers
{
    public class FireTime
    {
        public TimeSpan Time { get; set; }

        public int NumberOfDay { get; set; }

        public int NumberOfWeek  { get; set; }

        public TimeSpan LessonTime { get; set; }
        public override bool Equals(object? obj)
        {
            return obj is FireTime fireTime1 
                   && fireTime1.Time == Time
                   && fireTime1.NumberOfDay == NumberOfDay 
                   && fireTime1.NumberOfWeek == NumberOfWeek;
        }
    }
}