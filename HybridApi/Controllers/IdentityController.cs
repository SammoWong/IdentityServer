using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HybridApi.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var userName = User.Claims.First(u => u.Type == "email").Value;
            return Ok(userName);
        }
    }
}
