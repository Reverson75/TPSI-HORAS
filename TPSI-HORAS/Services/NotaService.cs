using TPSI_HORAS.Data;
using TPSI_HORAS.Models;

namespace TPSI_HORAS.Services;

public class NotaService(AppDatabase database) : INotaService
{
    private const double NotaMinima = 0;
    private const double NotaMaxima = 20;

    public async Task<IReadOnlyList<NotaItem>> ObterTodasAsync()
    {
        var conexao = await database.ObterAsync();

        var ucs = await conexao.Table<Uc>().OrderBy(u => u.Ordem).ToListAsync();
        var notas = (await conexao.Table<NotaUc>().ToListAsync())
            .ToDictionary(n => n.UcId, n => n.Valor);

        return ucs
            .Select(u => new NotaItem(
                u.Id, u.Codigo, u.Nome,
                notas.TryGetValue(u.Id, out var valor) ? (double?)valor : null))
            .ToList();
    }

    public async Task<double?> ObterAsync(int ucId)
    {
        var conexao = await database.ObterAsync();

        var nota = await conexao.Table<NotaUc>().Where(n => n.UcId == ucId).FirstOrDefaultAsync();
        return nota?.Valor;
    }

    public async Task DefinirAsync(int ucId, double valor)
    {
        if (double.IsNaN(valor) || valor < NotaMinima || valor > NotaMaxima)
            throw new InvalidOperationException($"A nota tem de estar entre {NotaMinima} e {NotaMaxima}.");

        var conexao = await database.ObterAsync();

        var uc = await conexao.Table<Uc>().Where(u => u.Id == ucId).FirstOrDefaultAsync()
                 ?? throw new InvalidOperationException("UC não encontrada.");

        valor = Math.Round(valor, 1);

        var existente = await conexao.Table<NotaUc>().Where(n => n.UcId == uc.Id).FirstOrDefaultAsync();

        if (existente is null)
        {
            await conexao.InsertAsync(new NotaUc { UcId = uc.Id, Valor = valor, DataRegisto = DateTime.Now });
            return;
        }

        existente.Valor = valor;
        existente.DataRegisto = DateTime.Now;
        await conexao.UpdateAsync(existente);
    }

    public async Task RemoverAsync(int ucId)
    {
        var conexao = await database.ObterAsync();
        await conexao.Table<NotaUc>().DeleteAsync(n => n.UcId == ucId);
    }
}