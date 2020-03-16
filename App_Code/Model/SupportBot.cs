using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

public class SupportBot
{
	static TelegramBotClient _client;
	static object _lockobj = new object();
	public static void SendMessage(string chat, string mess)
	{
		try
		{
			if (string.IsNullOrEmpty(chat))
			{
				return;
			}
			StartConnection();
			lock (_lockobj)
			{
				_client.SendTextMessageAsync(int.Parse(chat), mess, ParseMode.Html).GetAwaiter().GetResult();
			}
		}
		catch (Exception e)
		{
			Logger.Log(e);
		}
	}
	public static void StartConnection()
	{
		lock (_lockobj)
		{
			if (_client == null)
			{
				try
				{
					ServicePointManager.Expect100Continue = true;
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

					_client = new TelegramBotClient(Settings.CurrentSettings.TELEGRAMSUPPORTTOKEN);
					_client.GetMeAsync().Wait();
					_client.OnUpdate += BotOnUpdateReceived;
					_client.StartReceiving();
				}
				catch (Exception e)
				{
					Logger.Log(e);
				}
			}
		}
	}
	static string _comSubs = "/SUBSCRIBE:";
	private static async void BotOnUpdateReceived(object sender, UpdateEventArgs e)
	{
		var message = e.Update.Message;
		if (message != null && Settings.CurrentSettings.TELEGRAMSUPPORTGROUP == message.Chat.Id.ToString())
		{
			//ignore company chats
			return;
		}

		if (message == null || message.Type != MessageType.Text) return;
		string text = message.Text.ToUpper();
		if (text == "/START")
		{
			await _client.SendTextMessageAsync(message.Chat.Id, $"Hi, {message.Chat.FirstName}, type /subscribe:xxx");
			await _client.SendTextMessageAsync(message.Chat.Id, "where xxx is the your phone number in intentional format: +375...");
			return;
		}
		else if (text.StartsWith(_comSubs))
		{
			string num = Regex.Replace(text.Remove(0, "_comSubs".Length), "[^0-9]", "");
			await _client.SendTextMessageAsync(message.Chat.Id, $"Your number is: {num}");
			MPSUser u = MPSUser.FindUserbyPhone(num);
			if (u == null)
			{
				await _client.SendTextMessageAsync(message.Chat.Id, $"User no found, phone: {num}");
				return;
			}
			await _client.SendTextMessageAsync(message.Chat.Id, $"User {u.PERSON_NAME} was found! Subscribing...");
			u.SUPCHATID = message.Chat.Id.ToString();
			u.Store();
			await _client.SendTextMessageAsync(message.Chat.Id, $"User {u.PERSON_NAME} has been subscribed!");
			return;
		}
		List<MPSUser> ls = MPSUser.EnumAllSupporters();
		foreach (var user in ls)
		{
			if (user.SUPCHATCLIENTID == message.Chat.Id.ToString())
			{
				await _client.SendTextMessageAsync(user.SUPCHATID, message.Text);
				return;
			}
			if (user.SUPCHATID == message.Chat.Id.ToString())
			{
				if (!string.IsNullOrEmpty(user.SUPCHATCLIENTID))
				{
					await _client.SendTextMessageAsync(user.SUPCHATCLIENTID, message.Text);
					return;
				}
				else
				{
					await _client.SendTextMessageAsync(user.SUPCHATID, "type /select to select user to answer");
					return;
				}
			}
		}
		foreach (var user in ls)
		{
			await _client.SendTextMessageAsync(message.Chat.Id, $"{message.From.FirstName} {message.From.LastName} Sent the message:");
			await _client.SendTextMessageAsync(user.SUPCHATID, message.Text);
		}
		await _client.SendTextMessageAsync(message.Chat.Id, "Hi! Response to your message is pending...");
	}
	static SupportBot()
	{
		StartConnection();
	}
}