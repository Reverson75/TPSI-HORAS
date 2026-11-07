namespace TPSI_HORAS.Models;

public record AvaliacaoItem(int Id, string CodigoUc, TipoAvaliacao Tipo, DateTime DataHora, string? Notas)
{
    public string Titulo => $"{Tipo.Nome()} · {CodigoUc}";

    public string QuandoTexto => DataHora.ToString("ddd d MMM yyyy · HH:mm");

    public bool TemNotas => !string.IsNullOrWhiteSpace(Notas);

    public string Prazo => (DataHora.Date - DateTime.Today).Days switch
    {
        < 0 => "Passou",
        0 => "Hoje",
        1 => "Amanhã",
        var dias => $"{dias} dias"
    };
}