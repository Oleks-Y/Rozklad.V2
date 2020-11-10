using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rozklad.V2.Entities;
using Rozklad.V2.Services;

namespace Rozklad.V2.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/groups")]
    public class GroupsController : ControllerBase
    {

        private readonly IRozkladRepository _rozkladRepository;

        public GroupsController(IRozkladRepository rozkladRepository)
        {
            _rozkladRepository = rozkladRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetGroups()
        {
            // get groups 
            // select names and send as list

            var groups = await _rozkladRepository.GetAllGroupsAsync();

            var groupNames = groups.Select(g => g.Group_Name.Replace(" ", ""));

            return Ok(groupNames);
        }
        

    }
}