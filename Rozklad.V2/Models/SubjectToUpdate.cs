using System.Collections.Generic;

namespace Rozklad.V2.Models
{
    public class SubjectToUpdate
    {
        public string Name { get; set; }
        public string Teachers { get; set; }
        public string LessonsZoom { get; set; }
        public string LabsZoom { get; set; } 
        public string LessonsAccessCode { get; set; }
        public string LabsAccessCode { get; set; }
    }
}