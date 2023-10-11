using Telegram.Bot;
using Telegram.Bot.Types;

using TelegramBotShit.Bot;

namespace MarketTelegramParser;
internal class BotMessageRouter
{
    private TelegramBotClient _botClient;
    private CancellationTokenSource _cancellationTokenSource;

    private YandexMarketParser _yandexMarketParser;

    public BotMessageRouter(TelegramBotClient botClient, CancellationTokenSource tokenSource)
    {
        _botClient = botClient;
        _cancellationTokenSource = tokenSource;

        _yandexMarketParser = new();
    }

    public async Task RouteMessage(long chatId, Message message)
    {
        var text = message.Text!;

        if (text.Contains("/find"))
        {
            var itemName = text.Substring(6);
            int? maxCost = null;

            if (string.IsNullOrEmpty(itemName))
            {
                await SendTextMessageAsync(chatId, "Неверное имя товара");
                return;
            }

            var splitted = itemName.Split("cost");

            if (splitted.Length > 1)
            {
                itemName = splitted[0];

                maxCost = int.Parse(splitted[1].Replace(" ", ""));
            }

            for (int i = 0; i < itemName.Count();)
            {
                if (itemName[i] == ' ')
                {
                    itemName = itemName.Remove(i, 1);
                }
                else
                {
                    break;
                }
            }

            await SendTextMessageAsync(chatId, "Ищем подходящий товар...");

            List<MarketItem>? marketItems;

            if (maxCost is null)
            {
                marketItems = _yandexMarketParser.ParseByName(itemName);
            }
            else
            {
                marketItems = _yandexMarketParser.ParseByNameWithCostLimits(itemName, 0, (int)maxCost);
            }

            if (marketItems.Count == 0)
            {
                await SendTextMessageAsync(chatId, "Не найдено");
                return;
            }

            MarketItem item = marketItems.First();
            var result = $"{item.Title}\r\n\r\n{item.ImageUrl}\r\n\r\n<b>{item.Price.ToString("n0")} ₽</b>";

            await SendTextMessageAsync(chatId, result);

            return;
        }

        await SendTextMessageAsync(chatId,
            "Чтобы получить товары по вашему запросу, напишите\r\n" +
            "\r\n/find <b>название товара</b>" +
            "\r\n/find <b>название товара</b> cost <b>максимальная цена</b>");
    }

    private async Task<Message> SendTextMessageAsync(long chatId, string text)
    {
        return await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            cancellationToken: _cancellationTokenSource.Token);
    }
}
