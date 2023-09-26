using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JWTLOGINAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public string Get()
        {
            return "Log In Successfully";
        }
    }
}
