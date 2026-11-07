using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TPSI_HORAS.Models;
using TPSI_HORAS.Services;

namespace TPSI_HORAS.ViewModels;

public partial class NotasViewModel(INotaService notaService) : ObservableObject
{
    [ObservableProperty]
    private string _mediaTexto = "–";

    [ObservableProperty]
    private string _resumoTexto = string.Empty;

    public ObservableCollection<NotaItem> Notas { get; } = [];

    [RelayCommand]
    private async Task CarregarAsync()
    {
        var itens = await notaService.ObterTodasAsync();

        Notas.Clear();
        foreach (var item in itens)
            Notas.Add(item);

        var valores = itens.Where(i => i.TemNota).Select(i => i.Valor!.Value).ToList();

        MediaTexto = valores.Count > 0 ? valores.Average().ToString("0.0") : "–";
        ResumoTexto = $"{valores.Count} de {itens.Count} UCs com nota";
    }

    [RelayCommand]
    private async Task EditarAsync(NotaItem item)
    {
        var resposta = await Shell.Current.DisplayPromptAsync(
            item.Codigo,
            "Nota de 0 a 20 (deixa vazio para remover)",
            accept: "Guardar",
            cancel: "Cancelar",
            keyboard: Keyboard.Numeric,
            maxLength: 5,
            initialValue: item.Valor?.ToString("0.#") ?? string.Empty);

        // null = o utilizador cancelou
        if (resposta is null)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(resposta))
            {
                await notaService.RemoverAsync(item.UcId);
            }
            else if (TentarLer(resposta, out var valor))
            {
                await notaService.DefinirAsync(item.UcId, valor);
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Valor inválido", "Escreve um número, por exemplo 14 ou 14,5.", "OK");
                return;
            }

            await CarregarAsync();
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlertAsync("Não foi possível guardar", ex.Message, "OK");
        }
    }

    // Aceita vírgula ou ponto como separador decimal
    private static bool TentarLer(string texto, out double valor) =>
        double.TryParse(texto.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out valor);
}