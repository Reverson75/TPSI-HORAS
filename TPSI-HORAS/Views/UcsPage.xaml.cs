using TPSI_HORAS.ViewModels;
using TPSI_HORAS.Models;

namespace TPSI_HORAS.Views;

public partial class UcsPage : ContentPage
{
    private readonly UcsViewModel _viewModel;

    public UcsPage(UcsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await _viewModel.CarregarCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erro", $"{ex.GetType().Name}: {ex.Message}", "OK");
        }
    }

    private async void AoSelecionar(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not UcResumo uc)
            return;

        // Limpa a seleção para a mesma UC poder ser aberta outra vez
        ((CollectionView)sender!).SelectedItem = null;

        await _viewModel.AbrirCommand.ExecuteAsync(uc);
    }
}