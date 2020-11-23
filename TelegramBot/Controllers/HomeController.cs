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
            client.OnCallbackQuery += Client_OnCallbackQuery;

            client.StartReceiving();
        }

        private async void Client_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            if(answer.DeleteQuestion(e.CallbackQuery.Data).Result)
            {
                await messageService.DeleteMessage(client, e.CallbackQuery.Message.Chat, e.CallbackQuery.Message.MessageId);
            }
        }

        private async void Client_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var text = e?.Message?.Text;
            var name = $"{e.Message.Chat.FirstName} {e.Message.Chat.LastName}";

            if (string.IsNullOrEmpty(text))
            {
                await messageService.SendTextMessage(client, e.Message.Chat,
                    $"{name}, ви відправили пусте повідомлення.");

                return;
            }

            if (text.Contains("/start"))
            {
                await messageService.SendStartMessage(client, e.Message.Chat);

            }
            else if (text.Equals(answer.Beck))
            {
                await messageService.SendPreviousMenu(client, e.Message.Chat);
            }
            else if (answer.IsQuestion)
            {
                await answer.CreateQuestion(name, e.Message.Chat.Username, text);

                await messageService.SendTextMessage(client, e.Message.Chat,
                    $"{name}, ми відповімо на ваше запитання найближчим часом.");
            }
            else
            {
                await messageService.SendMessage(client, e.Message.Chat, text);
            }
        }
        public IActionResult Index()
        {
            return Content("Home");
        }
    }
}
