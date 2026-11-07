using System;
using System.Collections.Generic;
using System.Text;

using SQLite;
using TPSI_HORAS.Data;
using TPSI_HORAS.Models;

namespace TPSI_HORAS.Services;

public class UcService(AppDatabase database) : IUcService
{
    private const int MaxHorasPorRegisto = 24;

    public async Task<IReadOnlyList<UcResumo>> ObterResumosAsync()
    {
        var conexao = await database.ObterAsync();

        var ucs = await conexao.Table<Uc>().OrderBy(u => u.Ordem).ToListAsync();
        var registos = await conexao.Table<RegistoHoras>().ToListAsync();

        var horasPorUc = registos
            .GroupBy(r => r.UcId)
            .ToDictionary(g => g.Key, g => g.Sum(r => r.Horas));

        return ucs
            .Select(u => Criar(u, horasPorUc.GetValueOrDefault(u.Id)))
            .ToList();
    }

    public async Task<UcResumo?> ObterResumoAsync(int ucId)
    {
        var conexao = await database.ObterAsync();

        var uc = await conexao.Table<Uc>().Where(u => u.Id == ucId).FirstOrDefaultAsync();
        if (uc is null)
            return null;

        return Criar(uc, await HorasFeitasAsync(conexao, ucId));
    }

    public async Task<IReadOnlyList<RegistoHoras>> ObterHistoricoAsync(int ucId)
    {
        var conexao = await database.ObterAsync();

        return await conexao.Table<RegistoHoras>()
            .Where(r => r.UcId == ucId)
            .OrderByDescending(r => r.Data)
            .ToListAsync();
    }

    public async Task RegistarHorasAsync(int ucId, int horas, DateTime data)
    {
        if (horas < 1 || horas > MaxHorasPorRegisto)
            throw new InvalidOperationException($"Indica entre 1 e {MaxHorasPorRegisto} horas.");

        if (data.Date > DateTime.Today)
            throw new InvalidOperationException("A data não pode ser no futuro.");

        var conexao = await database.ObterAsync();

        var uc = await conexao.Table<Uc>().Where(u => u.Id == ucId).FirstOrDefaultAsync()
                 ?? throw new InvalidOperationException("UC não encontrada.");

        var emFalta = uc.HorasTotais - await HorasFeitasAsync(conexao, ucId);
        if (horas > emFalta)
            throw new InvalidOperationException($"Só faltam {emFalta} h nesta UC.");

        await conexao.InsertAsync(new RegistoHoras
        {
            UcId = ucId,
            Horas = horas,
            Data = data.Date
        });
    }

    public async Task ApagarRegistoAsync(int registoId)
    {
        var conexao = await database.ObterAsync();
        await conexao.DeleteAsync<RegistoHoras>(registoId);
    }

    private static UcResumo Criar(Uc uc, int horasFeitas) =>
        new(uc.Id, uc.Codigo, uc.Nome, uc.HorasTotais, horasFeitas);

    private static async Task<int> HorasFeitasAsync(SQLiteAsyncConnection conexao, int ucId)
    {
        var registos = await conexao.Table<RegistoHoras>().Where(r => r.UcId == ucId).ToListAsync();
        return registos.Sum(r => r.Horas);
    }
}