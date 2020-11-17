using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Rozklad.V2.Helpers;
using Rozklad.V2.Services;
using Xunit;

namespace Rozkald.V2.Tests
{
    public class SchedulerServiceTests
    {

        [Fact]
        public void TestTimeStamps()
        {
            // Arrange
            var mock = new Mock<IRozkladRepository>();
            mock.Setup(r => r.GetAllNotificationsFireTimes()).Returns(
                Task.Run(() => new List<FireTime>()
                {
                    new FireTime()
                    {
                        Time = new TimeSpan(8, 25, 0),
                        NumberOfDay = 1,
                        NumberOfWeek = 2
                    },
                    new FireTime()
                    {
                        Time = new TimeSpan(10, 0, 0),
                        NumberOfDay = 1,
                        NumberOfWeek = 2
                    }
                }.AsEnumerable()));
            var scheduler = new SchedulerService(mock.Object);
            // Act 
            var result = scheduler.GetJobSchedules().ToArray();
            // Assert 
            Assert.True(result[0].CronExpression=="0 25 8 ? * MON *");
            Assert.True(result[1].CronExpression=="0 0 10 ? * MON *");
        }
        
    }
}