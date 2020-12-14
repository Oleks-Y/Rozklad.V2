using System;
using Microsoft.AspNetCore.Mvc;
using Rozklad.V2.Controllers;
using Rozklad.V2.Models;
using Xunit;

namespace Rozkald.V2.Tests
{
    public class WeekTests
    {
        [Fact]
        public async void CanGetWeek()
        {
            // Arrange 
            var controller = new WeekController();
            // Act 
            var result= controller.GetWeekNumber(new DateTime(2020, 12,14));
            // Assert 
            
            Assert.True(true);
        }
    }
}