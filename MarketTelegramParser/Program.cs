using TelegramBotShit.Bot;

const string apiToken = "6240825883:AAFCv45aIy1r8j-ClYyfXgE5QxIeBbB3B88";

Bot bot = new(apiToken);

bot.Start();

Console.ReadLine();