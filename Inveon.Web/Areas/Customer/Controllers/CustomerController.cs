using Inveon.Web.Hubs;
using Inveon.Web.Models;
using Inveon.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Inveon.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly IProductService _productService;
        private readonly IHubContext<MessageHub> _messageHub;

        public CustomerController(ILogger<CustomerController> logger, IProductService productService, IHubContext<MessageHub> messageHub)
        {
            _logger = logger;
            _productService = productService;
            _messageHub = messageHub;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(list);
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

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var role = User.Claims.Where(u => u.Type == "role")?.FirstOrDefault()?.Value;
            if (role == "Admin")
            {
                // return Redirect("~/Admin/Admin");
                return RedirectToAction("Git", "Admin", new { area = "Admin" });
            }
            //buradan IdentityServer daki login sayfasına gidiliyor.
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [Route("Message")]
        [HttpPost]
        public IActionResult Message([FromBody] Message message)
        {
            _messageHub.Clients.All.SendAsync("lastMessage", message);
            return Accepted();
        }

        [Route("Partial")]
        [HttpPost]
        public ActionResult DisplayNewMessageBox([FromBody] Message message)
        {
            return PartialView("_MessageBoxPartial", message);
        }
    }
}