using System;
using Telegram.Bot;

public class TestChannel
{
	public static void SendMessage(string message)
	{
		try
		{
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