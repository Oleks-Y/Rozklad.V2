using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Rozklad.V2.Entities;
using Rozklad.V2.Exceptions;
using Rozklad.V2.Helpers;
using Serilog;
using Serilog.Core;


namespace Rozklad.V2.Google
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly string rozkladCalendarTag = "rozklad";

        private readonly string apikey;

        private Dictionary<int, string> daysNames = new Dictionary<int, string>
        {
            {1, "MO"},
            {2, "TU"},
            {3, "WE"},
            {4, "TH"},
            {5, "FR"},
            {6, "SA"}
        };
        
        private readonly string _applicationName = "Rozklad";
        private string _apiUrl = "https://www.googleapis.com/calendar/v3";

        public GoogleCalendarService(string apiKey)
        {
            this.apikey = apiKey;
        }

        public async Task<IEnumerable<Event>> SetNotificationsInCalendar(CalendarSettings settings)
        {
            // for all lessons from settings calculate firetime
            var fireTimes = calculateFireTimes(settings);
            // for all firetimes set event in calendar
            var eventsToAdd = fireTimes.Select(fireTime => createEvent(fireTime)).ToList();
            List<Event> calendarEvents = new List<Event>();
            HttpClient client = new HttpClient();
            foreach (var eventToAdd in eventsToAdd)
            {
                var eventInCalendar =  await createEventInCalendar(client,settings.CalendarId, settings.AccessToken, eventToAdd);
                calendarEvents.Add(eventInCalendar);
            }
            return calendarEvents;
        }
        /// <summary>
        /// update reminders for each event
        /// </summary>
        /// <param name="settings">setting of calendar notifications for user </param>
        /// <param name="eventsToUpdate"> list of events for single user </param>
        /// <returns></returns>
        public async Task UpdateNottificationsInCalednar(CalendarSettings settings, IEnumerable<string> eventsToUpdateIds)
        {
            // for all evetns in list update 
            //      update every event 
            // only filed that we can change - timeBefforeLesson
            var client = new HttpClient();
            foreach (var calendarEventId in eventsToUpdateIds)
            {
                var eventForPatch = createEventForPatch(calendarEventId, settings.NotificationsSettings.TimeBeforeLesson);
                await this.patchEventInCalendar(client:client,eventForPatch,settings.CalendarId,settings.AccessToken);
            }
            
        }
        public async Task DeleteNotificationsInCalendar(CalendarSettings settings,IEnumerable<string> eventsToDelete)
        {
            var client = new HttpClient();
            foreach (var calendarEventToDelete in eventsToDelete)
            {
                await this.deleteCalendarIvent(client, calendarEventToDelete, settings.CalendarId, settings.AccessToken);
            }
        }

        public async Task<string> GetCalendarIdForUser(string accessToken)
        {
            var client = new HttpClient();
            var calendarList = await getAllCalendars(client, accessToken);
            // check, if calendar from our app exists alreay
            // Todo Create more unique name 
            var rozkladCalendar = calendarList.Items.FirstOrDefault(c => c.Summary == "Розклад занять");
            if (rozkladCalendar == null)
            {
                rozkladCalendar = await createNewCalendar(client, accessToken);
            }

            // if not exists, creating new one 
            // then return id of that calendar
            return rozkladCalendar.Id;
        }
        
        /// <summary>
        /// Create all checkPoints for lessons
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        // todo separate firetime and claendar logic
        private IEnumerable<FireTime> calculateFireTimes(CalendarSettings settings)
        {
            var fireTimes = settings.Lessons.Select(lesson => new FireTime
                {
                    NumberOfWeek = lesson.Week,
                    Lesson = lesson,
                    Time = DateTime.Parse(lesson.TimeStart)
                        .AddMinutes(-settings.NotificationsSettings.TimeBeforeLesson)
                        .TimeOfDay,
                    TimeBefforeLesson = settings.NotificationsSettings.TimeBeforeLesson,
                    NumberOfDay = lesson.DayOfWeek
                })
                .ToList();
            return fireTimes;
        }

        private Event createEvent(FireTime fireTime)
        {
            var firstDay = NotificationsConfig.FirstDayOfSemester.AddDays(fireTime.Lesson.DayOfWeek - 1);
            if (fireTime.NumberOfWeek == 2)
            {
                firstDay = firstDay.AddDays(7);
            }

            // +1 because in our database days starts at 1, but here they starts on 0
            var reccuranceString =
                $"RRULE:FREQ=WEEKLY;BYDAY={daysNames[fireTime.NumberOfDay]};UNTIL=20210628;INTERVAL=2";
            return new Event
            {
                Summary = fireTime.Lesson.Subject.Name,
                Start = new EventDateTime
                {
                    DateTime = firstDay.Add(fireTime.LessonTime),
                    TimeZone = "Europe/Kiev"
                },
                End = new EventDateTime
                {
                    DateTime = firstDay.Add(fireTime.LessonTime).AddHours(1).AddMinutes(30),
                    TimeZone = "Europe/Kiev"
                },
                Recurrence = new List<string> {reccuranceString},
                Description = "// here will be link !", // todo add link 
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new List<EventReminder>
                    {
                        new EventReminder {Method = "popup", Minutes = fireTime.TimeBefforeLesson}
                    }
                }
            };
        }

        private Event createEventForPatch(string eventId,int timeBefforeLesson)
        {
            return new Event
            {
                Id = eventId,
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new List<EventReminder>()
                    {
                        new EventReminder { Method = "popup", Minutes = timeBefforeLesson}
                    }
                }
            };
        }
        #region requests to API

        private async Task<CalendarList> getAllCalendars(HttpClient client, string accessToken)
        {
            HttpResponseMessage response;
            using (var request =
                new HttpRequestMessage(HttpMethod.Get, _apiUrl + $"/users/me/calendarList?key={apikey}"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Add("Accept", "application/json");
                response = await client.SendAsync(request);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException($"calendarList returned statusCode {response.StatusCode} " +
                                        $"with data {await response.Content.ReadAsStringAsync()}");
            }

            // todo add error handling of 400, 500 
            var contentString = await response.Content.ReadAsStringAsync();
            var listCalendars = JsonSerializer.Deserialize<CalendarList>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return listCalendars;
        }

        private async Task<CalendarListEntry> createNewCalendar(HttpClient client, string accessTokem)
        {
            HttpResponseMessage response;
            var bodyString = JsonSerializer.Serialize(new Calendar
            {
                Description = "Календар для встановлення сповіщень",
                ETag = rozkladCalendarTag,
                Kind = "calendar#calendar",
                // todo add groupName here 
                Summary = "Розклад занять"
            }, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            using (var request =
                new HttpRequestMessage(HttpMethod.Post, _apiUrl + $"/calendars?key={apikey}"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokem);
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent(bodyString);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.SendAsync(request);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException(
                    $"wrong response code was returned from method POST calendars/ : {response.StatusCode}\n {await response.Content.ReadAsStringAsync()}");
            }

            var calendar = JsonSerializer.Deserialize<CalendarListEntry>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            return calendar;
        }

        /// <summary>
        /// Creates new event in google calendar and returns new event instance
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="accessToken"></param>
        /// <param name="calendarEvent"></param>
        /// <returns></returns>
        /// <exception cref="HttpException"></exception>
        private async Task<Event> createEventInCalendar(HttpClient client,string calendarId, string accessToken, Event calendarEvent)
        {
            var bodyString = JsonSerializer.Serialize(calendarEvent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            });

            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Post,
                _apiUrl + $"/calendars/{calendarId}/events?sendNotifications=true&apiKey={apikey}"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent(bodyString);
                response = await client.SendAsync(request);
            }
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException(
                    $"wrong response code was returned from method POST events/ : {response.StatusCode}\n {await response.Content.ReadAsStringAsync()}");
            }

            var eventFromCalendar = JsonSerializer.Deserialize<Event>(await response.Content.ReadAsStringAsync());
            
            return eventFromCalendar;
        }
        private async Task<Event> patchEventInCalendar(HttpClient client, Event eventToUpdate, string calendarId, string accessToken)
        {
            var bodyString = JsonSerializer.Serialize(eventToUpdate, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            });
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Patch, _apiUrl + $"/calendars/{calendarId}/events/{eventToUpdate.Id}?apiKey={apikey}"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent(bodyString);
                response = await client.SendAsync(request);
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException(
                    $"wrong response code was returned from method POST events/ : {response.StatusCode}\n {await response.Content.ReadAsStringAsync()}");
            }
            
            var eventFromCalendar = JsonSerializer.Deserialize<Event>(await response.Content.ReadAsStringAsync());
            
            return eventFromCalendar;
        }

        private async Task deleteCalendarIvent(HttpClient client, string eventToDeleteId, string calendarId,
            string accessToken)
        {
            HttpResponseMessage response;

            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/calendars/{calendarId}/events/{eventToDeleteId}?apiKey={apikey}" ))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Add("Accept", "application/json");
                response = await client.SendAsync(request);
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException(
                    $"wrong response code was returned from method POST events/ : {response.StatusCode}\n {await response.Content.ReadAsStringAsync()}");
            }
        }
        #endregion
    }
}