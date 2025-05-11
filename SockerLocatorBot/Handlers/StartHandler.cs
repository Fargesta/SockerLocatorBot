using SockerLocatorBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SockerLocatorBot.Handlers
{
    internal class StartHandler(ILogger<StartHandler> logger, ITelegramBotClient botClient, IUserService userService) : IBotHandler
    {
        public bool CanHandle(Update update)
        {
            if(update.Message?.Type is Telegram.Bot.Types.Enums.MessageType.Text &&
                update.Message.Text is not null &&
                update.Message.Text.Equals("/start"))
            {
                return true;
            }
            return false;
        }

        public async Task HandleUpdate(Update update, CancellationToken cancellationToken)
        {

            logger.LogInformation("StartHandler: {Update}", update);

            var user = await userService.GetUserAsync(update, cancellationToken);
            if (user is null)
            {
                user = await userService.CreateUserAsync(update, cancellationToken);
                logger.LogInformation("User created: {User}", user);

                await botClient.SendMessage(
                    chatId: update.Message!.Chat.Id,
                    text: $"Hi {user.FirstName}!\n" +
                        "Welcome to Socker Locator Bot!\n" +
                        "Please wait until moderator approve your join\n" +
                        "Meanwhile you can find instructions and commands list by typing /help command.",
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                logger.LogInformation("User found: {User}", user);
                await botClient.SendMessage(
                    chatId: update.Message!.Chat.Id,
                    text: $"Hi again {user.FirstName}!\n" +
                        "You can find instructions and commands list by typing /help command.",
                    cancellationToken: cancellationToken
                );
            }

        }
    }
}
