using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallbackController : Controller
    {
        // this is a fallback controller on server side if the url routed by UI
        // is not found in API of the server. To use we need to add MapFallbackController in program.cs
        // This can be handled in UI as well by setting useHash property of
        // routerModule.forRoot() to 'true' which then adds a # in the url for angular routing
        public IActionResult Index()
        {
            return PhysicalFile(Path
                .Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), 
                "text/HTML");
        }
    }
}
