using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using static Twilio.Rest.Api.V2010.Account.Call.FeedbackSummaryResource;

namespace WABot.Controllers
{
    [Route("[controller]")]
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

        public string Get()
        {
            return "Running...";
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(RequestMessage request)
        {
            TwilioClient.Init(configuration["Twilio:AccountSid"], configuration["Twilio:AuthToken"]);

            var messageOptions = new CreateMessageOptions(new PhoneNumber($"whatsapp:{request.NumberPhone}"))
            {
                From = new PhoneNumber($"whatsapp:{configuration["Twilio:From"]}"),
                Body = request.Message
            };

            var message = await MessageResource.CreateAsync(messageOptions);
            if (message.Status == StatusEnum.Completed)
            {
                
            }

            return Ok(message);

        }

    }

    public class RequestMessage
    {
        [JsonProperty("numberPhone")]
        public string NumberPhone { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
