using TPSI_HORAS.Views;

namespace TPSI_HORAS;

public partial class AppShell : Shell
{
    public const string RotaDetalhe = "detalhe-uc";
    public const string RotaNovaAvaliacao = "nova-avaliacao";

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(RotaDetalhe, typeof(UcDetalhePage));
        Routing.RegisterRoute(RotaNovaAvaliacao, typeof(NovaAvaliacaoPage));
    }
}