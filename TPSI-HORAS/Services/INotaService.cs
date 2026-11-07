using TPSI_HORAS.Models;

namespace TPSI_HORAS.Services;

public interface INotaService
{
    Task<IReadOnlyList<NotaItem>> ObterTodasAsync();
    Task<double?> ObterAsync(int ucId);
    Task DefinirAsync(int ucId, double valor);
    Task RemoverAsync(int ucId);
}