using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SockerLocatorBot;
using SockerLocatorBot.Handlers;
using SockerLocatorBot.Interfaces;
using SockerLocatorBot.Services;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var options = sp.GetRequiredService<IOptions<AppSettings>>().Value;
        var botToken = options.BotConfiguration.BotToken;
        if(string.IsNullOrWhiteSpace(botToken))
        {
            throw new ArgumentException("Bot token is not configured.");
        }
        TelegramBotClientOptions opts = new(botToken);

        return new TelegramBotClient(opts, httpClient);
    });
builder.Services.AddMemoryCache();

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddScoped<IStateService, StateService>();

builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, SharedLocationHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, NewOrFindCallbackHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, WaitForImageHandler>());

builder.Services.AddHostedService<PollingService>();

var host = builder.Build();
host.Run();
