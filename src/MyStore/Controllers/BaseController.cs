using Microsoft.AspNetCore.Mvc;

namespace MyStore.Controllers
{
    [Route("[controller]")]
    public abstract class BaseController : Controller
    {
    }
}