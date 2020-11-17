using System;

namespace Rozklad.V2.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Lesson Lesson { get; set; }
    }
}