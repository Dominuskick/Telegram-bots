
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;

namespace Cs_my_doc_bot3
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = "5747434056:AAG20Tfi8YTW8p2AMP0XEJEutaXVRMXlqqw";
            var client = new Doc_Bot(token);

            client.Start();
            Console.ReadLine();
        }
    }

    public class Doc_Bot
    {
        private readonly TelegramBotClient bot;
        private InlineKeyboardMarkup inlineKeyboard;

        static string[] tempNamefile;
        static string[] tempNameDirectory;

        private static string TempPath = @"C:\";

        public Doc_Bot(string token)
        {
            bot = new TelegramBotClient(token);
        }

        public void Start()
        {
            bot.StartReceiving(Update, Error);
        }

        private static InlineKeyboardMarkup GetDirectorie(string Path)
        {
            tempNamefile = Directory.GetFiles(Path);
            tempNameDirectory = System.IO.Directory.GetDirectories(Path);


            var tempButton = new InlineKeyboardButton[tempNameDirectory.Length][];

            int i = 0;
            foreach (var item in tempNameDirectory)
            {
                tempButton[i] = new[] { InlineKeyboardButton.WithCallbackData(item, item) };
                i++;
            }
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(tempButton);
            return inlineKeyboard;
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            Message? message = update.Message;

            if (message.Text != null)
            {
                if (message.Chat.Username == "Dominuskick")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Hi Fillostrat");
                    try
                    {
                        var inlineKeyboard = GetDirectorie(TempPath);

                        if (message.Text.ToLower().StartsWith("/keys"))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Нажмите на папку:", replyMarkup: inlineKeyboard);
                        }

                    }
                    catch (Exception)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "File dont find");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "You do not have access");

                }
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "NOOOOOOOOOOOOOOO");
            }
        }
        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}