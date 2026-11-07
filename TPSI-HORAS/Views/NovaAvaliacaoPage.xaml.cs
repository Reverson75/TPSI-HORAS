using TPSI_HORAS.ViewModels;

namespace TPSI_HORAS.Views;

public partial class NovaAvaliacaoPage : ContentPage
{
    private readonly NovaAvaliacaoViewModel _viewModel;

    public NovaAvaliacaoPage(NovaAvaliacaoViewModel viewModel)
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
            await DisplayAlertAsync("Erro", ex.Message, "OK");
        }
    }
}