using TPSI_HORAS.Data;
using TPSI_HORAS.Models;

namespace TPSI_HORAS.Services;

public class AvaliacaoService(AppDatabase database, INotificacaoService notificacoes) : IAvaliacaoService
{
    private const int MaxCaracteresNotas = 200;
    private const int MaxLembreteMinutos = 60 * 24 * 30;

    public async Task<IReadOnlyList<AvaliacaoItem>> ObterTodasAsync()
    {
        var conexao = await database.ObterAsync();

        var avaliacoes = await conexao.Table<Avaliacao>().OrderBy(a => a.DataHora).ToListAsync();
        var ucs = (await conexao.Table<Uc>().ToListAsync()).ToDictionary(u => u.Id);

        return avaliacoes
            .Where(a => ucs.ContainsKey(a.UcId))
            .Select(a => new AvaliacaoItem(a.Id, ucs[a.UcId].Codigo, a.Tipo, a.DataHora, a.Notas))
            .ToList();
    }

    public async Task<bool> AdicionarAsync(
        int ucId, TipoAvaliacao tipo, DateTime dataHora, int? lembreteAntesMin, string? notas)
    {
        if (!Enum.IsDefined(tipo))
            throw new InvalidOperationException("Tipo de avaliação inválido.");

        if (dataHora <= DateTime.Now)
            throw new InvalidOperationException("A data e hora têm de estar no futuro.");

        if (lembreteAntesMin is < 0 or > MaxLembreteMinutos)
            throw new InvalidOperationException("Lembrete inválido.");

        DateTime? momentoLembrete = lembreteAntesMin is { } minutos
            ? dataHora.AddMinutes(-minutos)
            : null;

        if (momentoLembrete <= DateTime.Now)
            throw new InvalidOperationException("Esse lembrete já passou. Escolhe um lembrete mais curto.");

        notas = notas?.Trim();
        if (notas is { Length: > MaxCaracteresNotas })
            throw new InvalidOperationException($"As notas podem ter no máximo {MaxCaracteresNotas} caracteres.");

        var conexao = await database.ObterAsync();

        var uc = await conexao.Table<Uc>().Where(u => u.Id == ucId).FirstOrDefaultAsync()
                 ?? throw new InvalidOperationException("UC não encontrada.");

        var avaliacao = new Avaliacao
        {
            UcId = uc.Id,
            Tipo = tipo,
            DataHora = dataHora,
            LembreteAntesMin = lembreteAntesMin,
            Notas = string.IsNullOrEmpty(notas) ? null : notas
        };

        await conexao.InsertAsync(avaliacao);

        // O Id da avaliação serve também de Id da notificação
        avaliacao.NotificacaoId = avaliacao.Id;
        await conexao.UpdateAsync(avaliacao);

        if (momentoLembrete is not { } momento || !await notificacoes.GarantirPermissaoAsync())
            return false;

        await notificacoes.AgendarAsync(avaliacao, uc.Codigo, momento);
        return true;
    }

    public async Task ApagarAsync(int avaliacaoId)
    {
        var conexao = await database.ObterAsync();

        var avaliacao = await conexao.FindAsync<Avaliacao>(avaliacaoId);
        if (avaliacao is null)
            return;

        // Cancela o lembrete para não disparar uma avaliação que já não existe
        notificacoes.Cancelar(avaliacao.NotificacaoId);
        await conexao.DeleteAsync(avaliacao);
    }
}