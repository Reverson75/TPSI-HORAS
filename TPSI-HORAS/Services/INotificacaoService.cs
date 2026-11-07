using TPSI_HORAS.Models;

namespace TPSI_HORAS.Services;

public interface INotificacaoService
{
    Task<bool> GarantirPermissaoAsync();
    Task AgendarAsync(Avaliacao avaliacao, string codigoUc, DateTime momento);
    void Cancelar(int notificacaoId);
}