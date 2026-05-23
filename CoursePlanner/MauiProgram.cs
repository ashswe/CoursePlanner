using CoursePlanner.Data;
using CoursePlanner.Services;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using CoursePlanner.Pages;

namespace CoursePlanner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("QuickingRegular-gw5KY.otf", "QuickingRegular");
                    fonts.AddFont("Alteixsans.otf", "Alteixsans");
                    fonts.AddFont("HussarBold.otf", "HussarBold");
                });

                builder.Services.AddSingleton<AppDatabase>();
                builder.Services.AddSingleton<IShare>(Share.Default);
                builder.Services.AddSingleton<IShareService, ShareService>();
                builder.Services.AddSingleton<AuthService>();

                builder.Services.AddTransient<Login>();
                builder.Services.AddTransient<TermsOverview>();
                builder.Services.AddTransient<TermDetails>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
