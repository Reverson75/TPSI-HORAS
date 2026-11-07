using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPSI_HORAS.Models;
using TPSI_HORAS.Services;

namespace TPSI_HORAS.ViewModels;

[QueryProperty(nameof(UcId), "id")]
public partial class UcDetalheViewModel(
    IUcService ucService,
    IAvaliacaoService avaliacaoService,
    INotaService notaService) : ObservableObject
{
    [ObservableProperty]
    private int _ucId;

    [ObservableProperty]
    private UcResumo? _resumo;

    [ObservableProperty]
    private string _proximaTexto = "Nenhuma marcada";

    [ObservableProperty]
    private string _notaTexto = "Sem nota";

    public ObservableCollection<RegistoHoras> Historico { get; } = [];

    [RelayCommand]
    private async Task CarregarAsync()
    {
        Resumo = await ucService.ObterResumoAsync(UcId);

        Historico.Clear();
        foreach (var registo in await ucService.ObterHistoricoAsync(UcId))
            Historico.Add(registo);

        await CarregarInfoAsync();
    }

    private async Task CarregarInfoAsync()
    {
        if (Resumo is null)
            return;

        // A lista vem ordenada por data, por isso a primeira futura é a próxima
        var proxima = (await avaliacaoService.ObterTodasAsync())
            .FirstOrDefault(a => a.CodigoUc == Resumo.Codigo && a.DataHora >= DateTime.Now);

        ProximaTexto = proxima is null
            ? "Nenhuma marcada"
            : $"{proxima.Tipo.Nome()} · {proxima.DataHora:d MMM} ({proxima.Prazo})";

        var nota = await notaService.ObterAsync(UcId);
        NotaTexto = nota is { } valor ? $"{valor:0.#} / 20" : "Sem nota";
    }

    [RelayCommand]
    private Task AbaterAsync(string horasTexto) =>
        int.TryParse(horasTexto, out var horas) ? RegistarAsync(horas) : Task.CompletedTask;

    [RelayCommand]
    private async Task AbaterOutroAsync()
    {
        var resposta = await Shell.Current.DisplayPromptAsync(
            "Horas assistidas",
            "Quantas horas queres abater?",
            accept: "Registar",
            cancel: "Cancelar",
            keyboard: Keyboard.Numeric,
            maxLength: 2);

        if (int.TryParse(resposta, out var horas))
            await RegistarAsync(horas);
    }

    [RelayCommand]
    private async Task ApagarAsync(RegistoHoras registo)
    {
        var confirmou = await Shell.Current.DisplayAlertAsync(
            "Apagar registo",
            $"Apagar {registo.Horas} h de {registo.Data:dd/MM/yyyy}?",
            "Apagar",
            "Cancelar");

        if (!confirmou)
            return;

        await ucService.ApagarRegistoAsync(registo.Id);
        await CarregarAsync();
    }

    private async Task RegistarAsync(int horas)
    {
        try
        {
            await ucService.RegistarHorasAsync(UcId, horas, DateTime.Today);
            await CarregarAsync();
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlertAsync("Não foi possível registar", ex.Message, "OK");
        }
    }
}