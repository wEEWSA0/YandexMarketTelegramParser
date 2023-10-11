using MarketTelegramParser;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TelegramBotShit.Bot;

public class Bot
{
    private TelegramBotClient _botClient;
    private CancellationTokenSource _cancellationTokenSource;

    public Bot(string apiToken)
    {
        _botClient = new TelegramBotClient(apiToken);
        _cancellationTokenSource = new CancellationTokenSource();

        Console.WriteLine("Выполнена инициализация TelegramBotClient");
    }

    public void Start()
    {
        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        BotMessageRouter botMessageRouter = new(_botClient, _cancellationTokenSource);

        BotRequestHandlers botRequestHandlers = new BotRequestHandlers(botMessageRouter);

        _botClient.StartReceiving(
            botRequestHandlers.HandleUpdateAsync,
            botRequestHandlers.HandlePollingErrorAsync,
            receiverOptions,
            _cancellationTokenSource.Token
        );

        Console.WriteLine("Выполнен запуск бота");
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        Console.WriteLine("Выполнена остановка бота");
    }
}