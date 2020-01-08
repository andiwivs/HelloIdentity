using AspNetDemoSite.Models;
using Identity.Contract.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AspNetDemoSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ActionResult> Index()
        {
            var userId = Guid.Parse("cb2292c1-3add-4d63-b0a8-35ad8121c0a1");

            var user = await _userService.GetByIdentity(userId);

            return View("Index", new HomeViewModel
            {
                Debug = user == null ? "NOT FOUND" : user.Username
            });
        }
    }
}