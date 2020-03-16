using System;
using System.Net;
using Telegram.Bot;

public class TestChannel
{
	public static void SendMessage(string message)
	{
		try
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			TelegramBotClient client = new TelegramBotClient(Settings.CurrentSettings.TELEGRAMTESTTOKEN);
			client.GetMeAsync().Wait();
			client.SendTextMessageAsync(Settings.CurrentSettings.TELEGRAMTESTCHANNEL, message, Telegram.Bot.Types.Enums.ParseMode.Html).Wait();
		}
		catch (Exception e)
		{
			Logger.Log(e);
		}
	}
}