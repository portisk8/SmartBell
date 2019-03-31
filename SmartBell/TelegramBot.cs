using SmartBell.Core.Configuration;
using SmartBell.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace SmartBell
{
	public class TelegramBot: ITelegramBot
	{
		private ITelegramBotClient _botClient;
		private IGeneralConfig _generalConfig;

		public TelegramBot(IGeneralConfig generalConfig)
		{
			_generalConfig = generalConfig;
		}
		public void Init()
		{
			_botClient = new TelegramBotClient(_generalConfig.TelegramBotKey);
			var me = _botClient.GetMeAsync().Result;
			Console.WriteLine(
			  $"{DateTime.Now} > TelegramBot connected > I am user {me.Id} and my name is {me.FirstName}."
			);
			_botClient.OnMessage += Bot_OnMessage;
			_botClient.StartReceiving();
			Thread.Sleep(int.MaxValue);
		}

		private async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Message.Text != null)
			{
				Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

				await _botClient.SendTextMessageAsync(
				  chatId: e.Message.Chat,
				  text: "You said:\n" + e.Message.Text
				);
			}
			if (e.Message.Photo != null)
			{
			}
		}
	}
}
