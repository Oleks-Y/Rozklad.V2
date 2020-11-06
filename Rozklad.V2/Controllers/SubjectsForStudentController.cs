using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Rozklad.V2.Controllers
{
    [Route("/student/subjects")]
    [Authorize]
    public class SubjectsForStudentController : ControllerBase
    {
        
    }
}