using TPSI_HORAS.Models;
using TPSI_HORAS.ViewModels;

namespace TPSI_HORAS.Views;

public partial class NotasPage : ContentPage
{
    private readonly NotasViewModel _viewModel;

    public NotasPage(NotasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ExecutarAsync(() => _viewModel.CarregarCommand.ExecuteAsync(null));
    }

    private async void AoEditar(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: NotaItem item })
            await ExecutarAsync(() => _viewModel.EditarCommand.ExecuteAsync(item));
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