
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
            //var token = "5459996216:AAHlWw8Gvj2k1zQk9PiOPfJKX9two3n2I5U";
            var client = new Doc_Bot(token);

            client.Start();
            Console.ReadLine();
        }
    }

    public class Doc_Bot
    {
        private readonly TelegramBotClient bot;

        static string[] tempNamefile;
        static string[] tempNameDirectory;
        static long chatId = 0;
        static int num = 0;

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


            var tempButton = new InlineKeyboardButton[tempNameDirectory.Length + tempNamefile.Length][];

            int i = 0;
            foreach (var item in tempNameDirectory)
            {
                Console.WriteLine(item);
                tempButton[i] = new[] { InlineKeyboardButton.WithCallbackData(item, @$"{item}") };
                i++;
                num++;
            }

            foreach (var item in tempNamefile)
            {
                Console.WriteLine(item);
                tempButton[i] = new[] { InlineKeyboardButton.WithCallbackData(item, @$"{item}") };
                i++;
                num++;
                int k;
            }
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(tempButton);
            return inlineKeyboard;
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            Message? message = update.Message;

            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null && update.CallbackQuery.Message != null)
            {
                //if (update.CallbackQuery.Data.StartsWith(@"C:\"))
                {
                    FileAttributes attr = System.IO.File.GetAttributes($@"{update.CallbackQuery.Data}");
                    if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        await using Stream stream = System.IO.File.OpenRead(update.CallbackQuery.Data);

                        await botClient.SendDocumentAsync(update.CallbackQuery.Message.Chat.Id, new InputOnlineFile(stream, "Dumpstack.log"));
                    }
                    else
                    {
                        //await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id.ToString(), update.CallbackQuery.Message.MessageId, "test");
                        Console.WriteLine(update.CallbackQuery.Data);
                        var inlineKeyboard = GetDirectorie($"{update.CallbackQuery.Data}");
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Нажмите на папку:", replyMarkup: inlineKeyboard);
                    }
                }
            }
            if(update.Type == UpdateType.Message)
            {
                if (message.Chat.Username == "Dominuskick")
                {
                    chatId = message.Chat.Id;
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
        }
        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}