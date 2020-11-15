using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Telegram.Bot;
using TelegramBot.Services;

namespace TelegramBot.Controllers
{
    public class HomeController : Controller
    {
        private ILogger logger;
        private AnswerService answer;
        private MessageService messageService;
        private TelegramBotClient client;
        public HomeController(ILogger<HomeController> log, 
            AnswerService answerService,
            MessageService messageService)
        {
            logger = log;
            answer = answerService;
            this.messageService = messageService;

            client = new TelegramBotClient("1189866721:AAHRjTujaTUiLF0dGlBm9xylz7X1aPPBXuY")
                { Timeout = TimeSpan.FromSeconds(10) };

            var me = client.GetMeAsync().Result;

            logger.LogInformation($"Id: {me.Id}, Name bot: {me.FirstName}");

            client.OnMessage += Client_OnMessage;

            client.StartReceiving();
        }

        private void Client_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var text = e?.Message?.Text;

            if (text == null)
                return;

            if (text.Contains("/start"))
            {
                messageService.SendStartMessage(client, e.Message.Chat);

            } else if(text.Equals(answer.Beck))
            {
                messageService.SendPreviousMenu(client, e.Message.Chat);
            } 
            else
            {
                messageService.SendMessage(client, e.Message.Chat, text);
            }
        }
        public IActionResult Index()
        {
            return Content("Home");
        }
    }
}
