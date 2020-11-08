using Rozklad.V2.Models;
using Rozklad.V2.Services;
using Xunit;

namespace Rozkald.V2.Tests
{
    public class TokenValidatorTests
    {
        [Fact]
        public void Can_Validate()
        {
            // Arrange 
            var validator = new TelegramValidationService("1449814836:AAGAM-7vBxeWVbt5YKG7U6vWHJczsSH0YMM");
           
            var TelegramUser = new TelegramUser
            {
                auth_date= 1604788467,
                first_name = "🎃лександр",
                hash = "61080738ed29bcc3262d0da4e3ca3ddf74bbd2fe46f9052c8260f66be289bdad",
                id = 456955082,
                last_name = "Яцевський",
                photo_url = "https://t.me/i/userpic/320/xm4A9Sn5GqnlFABoptK3Pl85TW5z2JUYV0H27kXeah0.jpg",
                username = "No_Niked"
            };
            // Act 
            var result = validator.Validate(TelegramUser);
            // Assert
            Assert.True(result);

        }
    }
}