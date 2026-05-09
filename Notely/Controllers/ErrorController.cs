using Microsoft.AspNetCore.Mvc;

namespace Notely.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Status(int statusCode)
        {
            if (statusCode == 401 || statusCode == 403)
            {
                return View("AccessDenied");
            }

            return View("NotFound");
        }

        [Route("Error/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("Error/NotFound")]
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }
    }
}
