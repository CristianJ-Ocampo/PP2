using Gestion_alumnos.Servicios;
using Microsoft.Extensions.Logging;
using Gestion_alumnos.ViewModels;

namespace Gestion_alumnos
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
            builder.Logging.AddDebug();
#endif

            // Servicio de datos compartidos
            builder.Services.AddSingleton<BaseDatosService>();
            builder.Services.AddSingleton<DatosService>();
            builder.Services.AddSingleton<RiesgoService>();

            // Pantallas de nuestro sistema
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<CalendarioPage>();

            // Navegación principal
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<AsistenciaPage>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<CalendarioViewModel>();
            builder.Services.AddTransient<AsistenciaViewModel>();
            builder.Services.AddTransient<EntregaPage>();
            builder.Services.AddTransient<EntregaViewModel>();
            builder.Services.AddTransient<PanelPage>();
            builder.Services.AddTransient<PanelViewModel>();
            builder.Services.AddTransient<AdministracionPage>();
            builder.Services.AddTransient<TrabajosPage>();
            return builder.Build();
        }
    }
}