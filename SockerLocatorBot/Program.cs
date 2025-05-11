using DbManager.Extensions;
using DriveManager.Extensions;
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

builder.Services.AddGoogleDrive(opts =>
{
    builder.Configuration.GetSection($"{nameof(AppSettings.GoogleDrive)}").Bind(opts);
});

builder.Services.AddPostgresDb(builder.Configuration);

builder.Services.AddMemoryCache();

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, SharedLocationHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, NewOrFindHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, ImageHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, SocketTypeHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, DescriptionHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, SaveLocationHandler>());
builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IBotHandler, StartHandler>());

builder.Services.AddHostedService<PollingService>();

var host = builder.Build();
host.Run();
