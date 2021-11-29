using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WABot.Api;
using WABot.Helpers.Json;

namespace WABot.Controllers
{
    /// <summary>
    /// Controller for processing requests coming from chat-api.com
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    public class WebHookController : ControllerBase
    {
        /// <summary>
        /// A static object that represents the API for a given controller.
        /// </summary>
        private readonly WaApi api;
        private static readonly string welcomeMessage = "Bot's menu: \n" +
                                                        "1. chatid - Get chatid\n" +
                                                        "2. file doc/gif,jpg,png,pdf,mp3,mp4 - Get a file in the desired format\n" +
                                                        "3. ogg - Get a voice message\n" +
                                                        "4. geo - Get the geolocation\n" +
                                                        "5. group - Create a group with a bot";
        private readonly IConfiguration configuration;
        private readonly ILogger<WebHookController> logger;

        public WebHookController(IConfiguration configuration, ILogger<WebHookController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.api = new WaApi(configuration["ChatApi:Intance"], configuration["ChatApi:Token"]);
        }

        /// <summary>
        /// Handler of post requests received from chat-api
        /// </summary>
        /// <param name="data">Serialized json object</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Post(Answer data)
        {
            logger.LogInformation(JsonConvert.SerializeObject(data));

            foreach (var message in data.Messages)
            {
                if (message.FromMe)
                    continue;

                switch (message.Body.Split()[0].ToLower())
                {
                    case "chatid":
                        return await api.SendMessage(message.ChatId, $"Your ID: {message.ChatId}");
                    case "file":
                        var texts = message.Body.Split();
                        if (texts.Length > 1)
                            return await api.SendFile(message.ChatId, texts[1]);
                        break;
                    case "ogg":
                        return await api.SendOgg(message.ChatId);
                    case "geo":
                        return await api.SendGeo(message.ChatId);
                    case "group":
                        return await api.CreateGroup(message.Author);
                    default:
                        return await api.SendMessage(message.ChatId, welcomeMessage);
                }             
            }
            return "";          
        }

        [HttpGet]
        public string Get()
        {
            logger.LogInformation("Running...");
            return "Running...";
        }
    }
}
