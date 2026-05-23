using CoursePlanner.Data;
using CoursePlanner.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using CoursePlanner.Seed;

namespace CoursePlanner
{
    public partial class App : Application
    {
        public App(AppDatabase database)
        {
            InitializeComponent();

            Task.Run(async () => await SeedData.InitializeAsync(database));
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var page = IPlatformApplication.Current?.Services.GetRequiredService<Login>();
            return new Window(new NavigationPage(page));
        }
    }
}