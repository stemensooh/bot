using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WABot.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TelegramWebHookController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TelegramWebHookController> logger;
        private readonly ITelegramBotClient bot;
        public TelegramWebHookController(IConfiguration configuration, ILogger<TelegramWebHookController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;

            bot = new TelegramBotClient(configuration["Telegram:Token"]);
        }

        [HttpGet]
        public async Task<string> Get()
        {
            await Run();
            logger.LogInformation("Running...");
            return "Running...";
        }

        [HttpPost]
        public async Task<IActionResult> Post(RequestTelegram request)
        {
            try
            {
                User me = await bot.GetMeAsync();
                //Console.Title = me.Username ?? "My awesome Bot";


                ChatId chat = new ChatId(request.Username);

                //Message message = await bot.SendTextMessageAsync(chat, request.Message);


                Message message = await bot.SendTextMessageAsync(
                    chatId: chat,
                    text: request.Message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html

                );


                return Ok(message);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private async Task Run()
        {
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            int offset = 0;
            int seconds = int.Parse(configuration["TimerSeconds"]);
            while (timeMeasure.Elapsed.TotalSeconds < seconds)
            {
                var updates = await bot.GetUpdatesAsync(offset);
                foreach (var update in updates)
                {
                    if (update.Message.Text != null)
                    {
                        string messageText = update.Message.Text;
                        try
                        {
                            string reply = "";
                            string usuario = update.Message.Chat.FirstName != null ? (update.Message.Chat.FirstName + " " + update.Message.Chat.LastName ?? "") : update.Message.Chat.Username ?? "Sin usuario";
                            int segundosRestantes = seconds - Convert.ToInt32(timeMeasure.Elapsed.TotalSeconds);
                            //string menu = "\ncomandos:\n 1. /start. \n 2. Escribe cualquier texto";

                            string menuFinal = "De clic en uno de los enlaces";
                            menuFinal += "\n";
                            menuFinal += "Menú:\n";
                            menuFinal += "1. Volver a iniciar el servicio: http://wa-bot-net-core.azurewebsites.net/TelegramWebHook";
                            menuFinal += "\n";
                            menuFinal += "2. Conocer tiempo restante: /verificar";
                            menuFinal += "\n";
                            menuFinal += "3. Detener servicio: /detener";
                            menuFinal += "\n";
                            menuFinal += "4. Ver menú: /start";

                            if (messageText.Contains("/verificar"))
                            {
                                reply = $"Hola {usuario}. El servicio estará disponible durante {segundosRestantes} segundos más";
                            }
                            else if (messageText.Contains("/detener"))
                            {
                                reply = $"Hola {usuario}. El servicio se ha detenido. Si quiere volver a iniciarlo de clic en el siguiente enlace:\nhttp://wa-bot-net-core.azurewebsites.net/TelegramWebHook";
                                //seconds = 0;
                            }
                            else if (messageText.Contains("/start"))
                            {
                                reply = $"Hola {usuario}.\n{menuFinal}";
                            }
                            else
                            {
                                reply = $"Hola {usuario}. ¿Quiére ver el menú? /start";
                            }

                            if (!string.IsNullOrEmpty(reply))
                            {
                                await bot.SendTextMessageAsync(update.Message.Chat.Id, reply);
                            }

                            

                            //if (messageText == "/start")
                            //{

                            //    string reply = $"Hola {usuario} el servicio esta iniciado, recuerda que solo estará disponible durante {segundosRestantes} segundos más";
                            //    reply += "";

                            //    await bot.SendTextMessageAsync(update.Message.Chat.Id, reply);
                            //}
                            //else
                            //{
                            //    string usuario = update.Message.Chat.FirstName != null ? (update.Message.Chat.FirstName + " " + update.Message.Chat.LastName ?? "") : update.Message.Chat.Username ?? "Sin usuario";
                            //    string reply = $"Hola {usuario} Tiempo restante: {segundosRestantes} segundos.{menu}";
                            //    await bot.SendTextMessageAsync(update.Message.Chat.Id, reply);
                            //}

                            if (2128421587 != update.Message.Chat.Id)
                            {
                                reply = $"Hola Bolivar, el usuario {usuario} [{update.Message.Chat.Id}] ha utilizado tu Servicio y ha enviado el siguiente mensaje:\n{messageText}";
                                await bot.SendTextMessageAsync(2128421587, reply);
                            }

                            //count++;
                            // No else because the IF loop only applies to /IBot commands
                        }
                        catch (System.AggregateException)
                        {
                            // Not a valid command, ignore it
                        }
                    }
                    offset = update.Id + 1;
                }
                await Task.Delay(1000);
            }

            timeMeasure.Stop();
        }
    }

    public class RequestTelegram
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("instanceId")]
        public string Message { get; set; }
    }
}
