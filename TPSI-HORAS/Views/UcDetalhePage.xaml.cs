using TPSI_HORAS.Models;
using TPSI_HORAS.ViewModels;

namespace TPSI_HORAS.Views;

public partial class UcDetalhePage : ContentPage
{
    private readonly UcDetalheViewModel _viewModel;

    public UcDetalhePage(UcDetalheViewModel viewModel)
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
        if (sender is Button { BindingContext: RegistoHoras registo })
            await ExecutarAsync(() => _viewModel.ApagarCommand.ExecuteAsync(registo));
    }

    // Evita que um erro inesperado feche a app sem explicação
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