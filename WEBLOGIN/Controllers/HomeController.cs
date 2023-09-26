using JWTLOGINAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using WEBLOGIN.Models;

namespace WEBLOGIN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController
        (
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUser user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var respone = await httpClient.PostAsync("http://localhost:5033/api/Auth/RegisterUser/Register", stringContent))
                {
                    string token = await respone.Content.ReadAsStringAsync();
                    if (token != "Successfuly done")
                    {
                        ViewBag.Massege = "Invalid user";
                        return null;
                    }
                }
            }
            return Redirect("~/Home/RegsterSuccess");
        }


        [HttpGet]
        public async Task<IActionResult> LoginUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginUser user)
        {
            
            using (var httpClient = new HttpClient())
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var respone = await httpClient.PostAsync("http://localhost:5033/api/Auth/Login/Login", stringContent))
                {
                    string token = await respone.Content.ReadAsStringAsync();
                    if (token == "Invalid credentials")
                    {
                        ViewBag.Massege = "Invalid user";
                        return View();
                    }
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, user.Password, true, lockoutOnFailure: false);
                    HttpContext.Session.SetString("JWToken", token);

                    var query1 = (from loguser in _userManager.Users
                                  where loguser.Email == user.UserName
                                  select new { UserId = loguser.Id, }).ToList();
                    IdentityUser identityUser = new IdentityUser
                    {
                        UserName = user.UserName,
                        Email = user.UserName,
                        Id = query1[0].UserId

                    };
                    var roles = await _userManager.GetRolesAsync(identityUser);

                    if (roles[0].ToString() == "Admin")
                    {
                        return Redirect("~/Home/LoginSuccessAdmin");
                    }
                    else
                    {
                        return Redirect("~/Home/LoginSuccessCustomer");
                    }
                }
            }
            return Redirect("~/Home/Index");
        }
        public IActionResult GetSessionValue()
        {
            string sessionValue = HttpContext.Session.GetString("JWToken");

            if (sessionValue != null)
            {                
                return Content("Session Value: " + sessionValue);
            }
            else
            {
                return Content("Session Value not found");
            }
        }
        public IActionResult LoginSuccessAdmin()
        {
            return View();
        }
        public IActionResult LoginSuccessCustomer()
        {
            return View();
        }
        public IActionResult RegsterSuccess()
        {
            return View();
        }
        public async Task<IActionResult> LogOutUser()
        {           
            HttpContext.Session.Remove("JWToken");
             await _signInManager.SignOutAsync();
            return Redirect("~/Home/Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}