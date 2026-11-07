using TPSI_HORAS.Models;
using TPSI_HORAS.ViewModels;

namespace TPSI_HORAS.Views;

public partial class AgendaPage : ContentPage
{
    private readonly AgendaViewModel _viewModel;

    public AgendaPage(AgendaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ExecutarAsync(() => _viewModel.CarregarCommand.ExecuteAsync(null));
    }

    private async void AoApagar(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: AvaliacaoItem item })
            await ExecutarAsync(() => _viewModel.ApagarCommand.ExecuteAsync(item));
    }

    private async Task ExecutarAsync(Func<Task> acao)
    {
        try
        {
            await acao();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro", ex.Message, "OK");
        }
    }
}