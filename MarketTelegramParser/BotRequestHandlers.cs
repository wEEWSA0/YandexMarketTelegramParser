using MarketTelegramParser;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotShit.Bot;

public class BotRequestHandlers
{
    private BotMessageRouter _botMessageRouter;

    internal BotRequestHandlers(BotMessageRouter botMessageRouter)
    {
        _botMessageRouter = botMessageRouter;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        long chatId = 0;

        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message != null)
                    {
                        if (update.Message.Type != MessageType.Text)
                        {
                            Console.WriteLine("Recived unexpected message type: " + update.Message.Type);

                            break;
                        }

                        chatId = update.Message.Chat.Id;

                        Console.WriteLine($"Принято входящее сообщение: chatId={chatId}, UpdateType={update.Type}");

                        await Task.Run(async () => await _botMessageRouter.RouteMessage(chatId, update.Message), cancellationToken);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected exception in HandleUpdateAsync: " + ex);
        }
        
        Console.WriteLine($"Выполенна обработка входящего сообщения: chatId = {chatId}");
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        string errorMessage = "empty";
        switch (exception)
        {
            case ApiRequestException:
            {
                var ex = exception as ApiRequestException;
                errorMessage = $"Telegram API Error:\n[{ex.ErrorCode}]\n{ex.Message}";
            }
                break;
            default:
            {
                errorMessage = exception.ToString();
            }
                break;
        }

        return Task.CompletedTask;
    }
}