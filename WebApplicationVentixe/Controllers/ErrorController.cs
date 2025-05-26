using Microsoft.AspNetCore.Mvc;

namespace WebApplicationVentixe.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View("~/Views/Shared/Errors/_Error404.cshtml");
        }
        [Route("Error/403")]
        public IActionResult Error403()
        {
            Response.StatusCode = 403;
            return View("~/Views/Shared/Errors/_Error403.cshtml");
        }
        [Route("Error/401")]
        public IActionResult Error401()
        {
            Response.StatusCode = 401;
            return View("~/Views/Shared/Errors/_Error401.cshtml");
        }
    }
}
