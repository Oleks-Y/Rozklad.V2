using System;
using Rozklad.V2.Helpers;

namespace Rozklad.V2.Scheduler
{
    public class NotificationJob : INotificationJob
    {
        // Get all lessons at that time 
        // Call notificate method 

        public void Execute(FireTime fireTime)
        {
            Console.WriteLine("Here will be send notifications");
        }
    }
}