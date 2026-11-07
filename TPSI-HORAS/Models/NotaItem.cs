namespace TPSI_HORAS.Models;

public record NotaItem(int UcId, string Codigo, string Nome, double? Valor)
{
    public bool TemNota => Valor is not null;

    public string NotaTexto => Valor is { } v ? v.ToString("0.#") : "Sem nota";
}