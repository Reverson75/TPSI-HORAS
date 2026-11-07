using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPSI_HORAS.Models;
using TPSI_HORAS.Services;

namespace TPSI_HORAS.ViewModels;

public partial class NovaAvaliacaoViewModel(
    IUcService ucService,
    IAvaliacaoService avaliacaoService) : ObservableObject
{
    private static readonly TipoAvaliacao[] TiposValores = Enum.GetValues<TipoAvaliacao>();

    private static readonly OpcaoLembrete[] OpcoesLembrete =
    [
        new("Sem lembrete", null),
        new("1 hora antes", 60),
        new("1 dia antes", 60 * 24),
        new("2 dias antes", 60 * 24 * 2),
        new("1 semana antes", 60 * 24 * 7)
    ];

    public ObservableCollection<UcResumo> Ucs { get; } = [];

    public IReadOnlyList<string> Tipos { get; } = TiposValores.Select(t => t.Nome()).ToList();

    public IReadOnlyList<string> Lembretes { get; } = OpcoesLembrete.Select(o => o.Texto).ToList();

    [ObservableProperty]
    private UcResumo? _ucSelecionada;

    [ObservableProperty]
    private int _tipoIndex;

    [ObservableProperty]
    private DateTime _data = DateTime.Today;

    [ObservableProperty]
    private TimeSpan _hora = new(10, 0, 0);

    [ObservableProperty]
    private int _lembreteIndex = 2;

    [ObservableProperty]
    private string _notas = string.Empty;

    [RelayCommand]
    private async Task CarregarAsync()
    {
        if (Ucs.Count > 0)
            return;

        foreach (var uc in await ucService.ObterResumosAsync())
            Ucs.Add(uc);
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (UcSelecionada is null)
        {
            await Shell.Current.DisplayAlertAsync("Falta a UC", "Escolhe a UC da avaliação.", "OK");
            return;
        }

        try
        {
            var lembreteMin = Escolher(OpcoesLembrete, LembreteIndex).MinutosAntes;

            var lembreteAgendado = await avaliacaoService.AdicionarAsync(
                UcSelecionada.Id,
                Escolher(TiposValores, TipoIndex),
                Data.Date + Hora,
                lembreteMin,
                Notas);

            if (lembreteMin is not null && !lembreteAgendado)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Avaliação guardada",
                    "Não foi possível agendar o lembrete. Verifica se as notificações estão ativas para a app.",
                    "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlertAsync("Não foi possível guardar", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private Task CancelarAsync() => Shell.Current.GoToAsync("..");

    private static T Escolher<T>(T[] lista, int indice) =>
        lista[Math.Clamp(indice, 0, lista.Length - 1)];
}