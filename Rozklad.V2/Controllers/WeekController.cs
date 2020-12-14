using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Rozklad.V2.Entities;
using Rozklad.V2.Helpers;
using Rozklad.V2.Models;

namespace Rozklad.V2.Controllers
{
    [Route("api/week")]
    [ApiController]
    public class WeekController : ControllerBase
    {
        [HttpGet("{date:datetime}")]
        public ActionResult<WeekDto> GetWeekNumber(DateTime date)
        {
            var weekOfYear= CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            var numberOfWeek = 0;
            if (NotificationsConfig.IsFirstWeekEven)
            {
                numberOfWeek = weekOfYear % 2 == 0 ? 1 : 2;
            }
            if (!NotificationsConfig.IsFirstWeekEven)
            {
                numberOfWeek = weekOfYear % 2 == 0 ? 2 : 1;
            }

            return Ok(new WeekDto{WeekNumber = numberOfWeek});
        }
    }
}