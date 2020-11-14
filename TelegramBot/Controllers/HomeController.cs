using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Services;

namespace TelegramBot.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment hostEnvironment;
        private ILogger logger;
        private AnswerService answer;
        private TelegramBotClient client;
        public HomeController(ILogger<HomeController> log, IWebHostEnvironment environment, AnswerService answerService)
        {
            logger = log;
            hostEnvironment = environment;
            answer = answerService;

            client = new TelegramBotClient("1189866721:AAHRjTujaTUiLF0dGlBm9xylz7X1aPPBXuY")
                { Timeout = TimeSpan.FromSeconds(10) };

            var me = client.GetMeAsync().Result;

            logger.LogInformation($"Id: {me.Id}, Name bot: {me.FirstName}");

            client.OnMessage += Client_OnMessage;
            client.StartReceiving();
        }

        private async void Client_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var text = e?.Message?.Text;

            if (text == null)
                return;

            if (text.Contains("/start"))
            {
                logger.LogInformation(text);

                await client.SendTextMessageAsync(e.Message.Chat, "Виберіть пункт із списку нижче:", 
                    replyMarkup: answer.CreateMenu(answer.GetMainMenu()));
            } else
            {
                var category = answer.GetCategory(text);

                if (category?.Categories?.Count > 0)
                {
                    await client.SendTextMessageAsync(e.Message.Chat, "Виберіть пункт із списку нижче:",
                        replyMarkup: answer.CreateMenu(category.Categories));
                }
                else
                {
                    var answ = answer.GetAnswer(category.Id);
                    if (!answ.IsFile)
                    {
                        await client.SendTextMessageAsync(e.Message.Chat, answ.Text);
                    }
                    else
                    {
                        var file = new FileStream($"{hostEnvironment.WebRootPath}/{answ.Text}", FileMode.Open);

                        await client.SendDocumentAsync(e.Message.Chat,
                            new InputOnlineFile(file, answ.Text));

                        file.Close();
                    }
                }
            }
        }
        public async void SendMessage(ChatId id, string text)
        {
            await client.SendTextMessageAsync(id, text);
        }
        public IActionResult Index()
        {
            return Content("Home");
        }
    }
}
