using System;

namespace Rozklad.V2.Helpers
{
    public static class NotificationsConfig
    {
        public static int[] Weeks { get; set; } = new[] {1, 2};
        // Configuration of notifications  
        public static int[] Days { get; set; } = new[] {1,2,3,4,5,6};
        public static int[] TimesBeforeLesson { get; set; } = new[] { 5,10,15,20,25,30};
        public static TimeSpan[] LessonsTimes { get; set; } = new[]
        {
            new TimeSpan(8,30,0), 
            new TimeSpan(10,25,0), 
            new TimeSpan(12,20,0), 
            new TimeSpan(14,15,0), 
            new TimeSpan(16,10,0), 
        };

        public static bool IsFirstWeekEven { get; set; } = true;

        public static DateTime FirstDayOfSemester { get; set; } = DateTime.Parse("2021-01-01");
    
    }
}