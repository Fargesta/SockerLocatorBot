using DriveManager.Interfaces;
using DriveManager.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DriveManager.Extensions
{
    public static class GoogleDriveServiceCollectionExtension
    {
        public static IServiceCollection AddGoogleDrive(this IServiceCollection services, Action<GoogleDriveOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);

            services.AddSingleton(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<GoogleDriveOptions>>().Value;

                GoogleCredential credential = File.Exists(opts.CredentialsPath)
                    ? GoogleCredential.FromFile(opts.CredentialsPath)
                    : throw new FileNotFoundException("Credentials file not found.", opts.CredentialsPath);

                credential = credential.CreateScoped(DriveService.ScopeConstants.Drive);
                return new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = opts.ApplicationName,
                });
            });
            services.AddScoped<IGoogleDriveService, GoogleDriveService>();
            return services;
        }
    }
}
