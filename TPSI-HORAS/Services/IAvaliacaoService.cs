using TPSI_HORAS.Models;

namespace TPSI_HORAS.Services;

public interface IAvaliacaoService
{
    Task<IReadOnlyList<AvaliacaoItem>> ObterTodasAsync();

    /// <returns>true se foi agendado um lembrete.</returns>
    Task<bool> AdicionarAsync(
        int ucId, TipoAvaliacao tipo, DateTime dataHora, int? lembreteAntesMin, string? notas);

    Task ApagarAsync(int avaliacaoId);
}