using System;
using System.Collections.Generic;
using System.Text;

using SQLite;
using TPSI_HORAS.Models;

namespace TPSI_HORAS.Data;

public class AppDatabase
{
    private const string NomeFicheiro = "tpsi_horas.db3";

    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

    private readonly SemaphoreSlim _bloqueio = new(1, 1);
    private SQLiteAsyncConnection? _conexao;

    public async Task<SQLiteAsyncConnection> ObterAsync()
    {
        if (_conexao is not null)
            return _conexao;

        await _bloqueio.WaitAsync();
        try
        {
            if (_conexao is not null)
                return _conexao;

            // Pasta privada da app: nenhuma outra app lê este ficheiro
            var caminho = Path.Combine(FileSystem.AppDataDirectory, NomeFicheiro);
            var conexao = new SQLiteAsyncConnection(caminho, Flags);

            await conexao.CreateTableAsync<Uc>();
            await conexao.CreateTableAsync<RegistoHoras>();
            await conexao.CreateTableAsync<Avaliacao>();
            await conexao.CreateTableAsync<NotaUc>();

            await UcSeed.PopularAsync(conexao);

            _conexao = conexao;
            return _conexao;
        }
        finally
        {
            _bloqueio.Release();
        }
    }
}