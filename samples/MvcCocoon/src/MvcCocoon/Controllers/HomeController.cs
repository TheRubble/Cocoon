using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcCocoon.Models;
using ReCode.Cocoon.Proxy.Session;
using SharedStuff;

namespace MvcCocoon.Controllers
{
    public class HomeController : Controller
    {
        private readonly CocoonSession _session;

        public HomeController(CocoonSession session)
        {
            _session = session;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string id)
        {
            // HttpContext.Session.SetString("anything","make a cookie");
            if (!string.IsNullOrEmpty(id))
            {
               await _session.SetAsync("CocoonSessionShare", id);    
            }
            return View();
        }

        [HttpGet("privacy")]
        public async Task<ActionResult> Privacy()
        {
            var foo = await _session.GetAsync<Foo>("foo");
            foo.Bar = "quux";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("hack")]
        public async Task<string> GetHack()
        {
            var cartId = await _session.GetAsync<string>("CartId");
            return cartId;
        }
    }
}
