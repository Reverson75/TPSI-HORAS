namespace TPSI_HORAS.Models;

public static class TipoAvaliacaoExtensions
{
    public static string Nome(this TipoAvaliacao tipo) => tipo switch
    {
        TipoAvaliacao.Teste => "Teste",
        TipoAvaliacao.Trabalho => "Trabalho",
        TipoAvaliacao.Apresentacao => "Apresentação",
        _ => tipo.ToString()
    };
}