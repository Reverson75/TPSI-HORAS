using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPSI_HORAS.Models;
using TPSI_HORAS.Services;

namespace TPSI_HORAS.ViewModels;

public partial class AgendaViewModel(IAvaliacaoService avaliacaoService) : ObservableObject
{
    public ObservableCollection<AvaliacaoItem> Avaliacoes { get; } = [];

    [RelayCommand]
    private async Task CarregarAsync()
    {
        var todas = await avaliacaoService.ObterTodasAsync();

        Avaliacoes.Clear();
        foreach (var item in todas)
            Avaliacoes.Add(item);
    }

    [RelayCommand]
    private Task NovaAsync() => Shell.Current.GoToAsync(AppShell.RotaNovaAvaliacao);

    [RelayCommand]
    private async Task ApagarAsync(AvaliacaoItem item)
    {
        var confirmou = await Shell.Current.DisplayAlertAsync(
            "Apagar avaliação", $"Apagar \"{item.Titulo}\"?", "Apagar", "Cancelar");

        if (!confirmou)
            return;

        await avaliacaoService.ApagarAsync(item.Id);
        await CarregarAsync();
    }
}