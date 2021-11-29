using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WABot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsappWebHookController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<WebHookController> logger;

        public WhatsappWebHookController(IConfiguration configuration, ILogger<WebHookController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
    }
}
