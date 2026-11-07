using TPSI_HORAS.Models;
using Plugin.LocalNotification.Core.Models;

#if !WINDOWS
using Plugin.LocalNotification;
#endif

namespace TPSI_HORAS.Services;

public class NotificacaoService : INotificacaoService
{
#if WINDOWS
    // App Windows sem pacote: notificações locais não ativas nesta versão
    public Task<bool> GarantirPermissaoAsync() => Task.FromResult(false);

    public Task AgendarAsync(Avaliacao avaliacao, string codigoUc, DateTime momento) => Task.CompletedTask;

    public void Cancelar(int notificacaoId) { }
#else
    public async Task<bool> GarantirPermissaoAsync()
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled())
            return true;

        return await LocalNotificationCenter.Current.RequestNotificationPermission();
    }

    public async Task AgendarAsync(Avaliacao avaliacao, string codigoUc, DateTime momento)
    {
        var pedido = new NotificationRequest
        {
            NotificationId = avaliacao.NotificacaoId,
            Title = $"{avaliacao.Tipo.Nome()} · {codigoUc}",
            Description = $"{avaliacao.DataHora:dddd, d MMM} às {avaliacao.DataHora:HH:mm}",
            Schedule = new NotificationRequestSchedule { NotifyTime = momento }
        };

        await LocalNotificationCenter.Current.Show(pedido);
    }

    public void Cancelar(int notificacaoId) =>
        LocalNotificationCenter.Current.Cancel(notificacaoId);
#endif
}