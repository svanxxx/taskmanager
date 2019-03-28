using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

public class TasksBot
{
	static TelegramBotClient _client;
	static object _lockobj = new object();
	public static void SendMessage(string chat, string mess)
	{
		if (string.IsNullOrEmpty(chat))
		{
			return;
		}
		StartConnection();
		_client.SendTextMessageAsync(int.Parse(chat), mess, ParseMode.Html).GetAwaiter().GetResult();
	}
	public static void StartConnection()
	{
		lock (_lockobj)
		{
			if (_client == null)
			{
				_client = new TelegramBotClient(Settings.CurrentSettings.TELEGRAMTASKSTOKEN);
				_client.GetMeAsync().Wait();
				_client.OnUpdate += BotOnUpdateReceived;
				_client.StartReceiving();
			}
		}
	}
	static string _comSubs = "/SUBSCRIBE:";
	private static async void BotOnUpdateReceived(object sender, UpdateEventArgs e)
	{
		var message = e.Update.Message;
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
			u.CHATID = message.Chat.Id.ToString();
			u.Store();
			await _client.SendTextMessageAsync(message.Chat.Id, $"User {u.PERSON_NAME} has been subscribed!");
			return;
		}
		await _client.SendTextMessageAsync(message.Chat.Id, "Wrong command. Press /start to see more information");
	}
	static TasksBot()
	{
		StartConnection();
	}
}