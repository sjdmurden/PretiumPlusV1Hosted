using Microsoft.AspNetCore.Mvc;

namespace CSV_reader.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/NotFound")]
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
