using Microsoft.Extensions.Logging;
using TPSI_HORAS.Data;
using TPSI_HORAS.Services;
using TPSI_HORAS.ViewModels;
using TPSI_HORAS.Views;
#if !WINDOWS
using Plugin.LocalNotification;
#endif

namespace TPSI_HORAS;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if !WINDOWS
        builder.UseLocalNotification();
#endif

        // Dados e serviços (um só de cada para toda a app)
        builder.Services.AddSingleton<AppDatabase>();
        builder.Services.AddSingleton<INotificacaoService, NotificacaoService>();
        builder.Services.AddSingleton<IUcService, UcService>();
        builder.Services.AddSingleton<IAvaliacaoService, AvaliacaoService>();
        builder.Services.AddSingleton<INotaService, NotaService>();

        // Ecrãs e respetivos ViewModels
        builder.Services.AddTransient<UcsViewModel>();
        builder.Services.AddTransient<UcsPage>();
        builder.Services.AddTransient<UcDetalheViewModel>();
        builder.Services.AddTransient<UcDetalhePage>();
        builder.Services.AddTransient<AgendaViewModel>();
        builder.Services.AddTransient<AgendaPage>();
        builder.Services.AddTransient<NovaAvaliacaoViewModel>();
        builder.Services.AddTransient<NovaAvaliacaoPage>();
        builder.Services.AddTransient<NotasViewModel>();
        builder.Services.AddTransient<NotasPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}