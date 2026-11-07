using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPSI_HORAS.Models;
using TPSI_HORAS.Services;

namespace TPSI_HORAS.ViewModels;

public partial class UcsViewModel(IUcService ucService) : ObservableObject
{
    private IReadOnlyList<UcResumo> _todas = [];

    [ObservableProperty]
    private string _pesquisa = string.Empty;

    [ObservableProperty]
    private string _textoGlobal = string.Empty;

    public ObservableCollection<UcResumo> Ucs { get; } = [];

    [RelayCommand]
    private async Task CarregarAsync()
    {
        _todas = await ucService.ObterResumosAsync();
        TextoGlobal = $"{_todas.Sum(u => u.HorasFeitas)} de {_todas.Sum(u => u.HorasTotais)} h assistidas";
        Filtrar();
    }

    [RelayCommand]
    private Task AbrirAsync(UcResumo uc) =>
    Shell.Current.GoToAsync($"{AppShell.RotaDetalhe}?id={uc.Id}");

    partial void OnPesquisaChanged(string value) => Filtrar();

    private void Filtrar()
    {
        var termo = Pesquisa.Trim();

        var resultado = string.IsNullOrEmpty(termo)
            ? _todas
            : _todas.Where(u =>
                u.Nome.Contains(termo, StringComparison.CurrentCultureIgnoreCase) ||
                u.Codigo.Contains(termo, StringComparison.CurrentCultureIgnoreCase));

        Ucs.Clear();
        foreach (var uc in resultado)
            Ucs.Add(uc);
    }
}