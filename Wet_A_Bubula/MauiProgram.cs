using Microsoft.Extensions.Logging;
using Wet_A_Bubula.Repositories;
using Wet_A_Bubula.Model;
using Wet_A_Bubula.ViewModels;

namespace Wet_A_Bubula
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<HomePage2ViewModel>();
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddTransient<RegistryPageViewModel>();
            builder.Services.AddTransient<RegistryPage2ViewModel>();
            builder.Services.AddTransient<VisitsPageViewModel>();

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<RegistryPage>();
            builder.Services.AddTransient<RegistryPage2>();
            builder.Services.AddTransient<VisitsPage>();
#endif

            return builder.Build();
        }
    }
}
