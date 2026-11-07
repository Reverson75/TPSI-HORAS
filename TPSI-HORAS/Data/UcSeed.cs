using SQLite;
using TPSI_HORAS.Models;

namespace TPSI_HORAS.Data;

internal static class UcSeed
{
    private static readonly (string Codigo, string Nome, int Horas)[] Dados =
    [
        ("UC00597", "Comunicar em língua portuguesa", 50),
        ("UC00598", "Efetuar operações e cálculos matemáticos aplicados a projetos da área de informática", 50),
        ("UC00599", "Interagir em inglês nas atividades do setor da informática", 50),
        ("UC00600", "Analisar as funções e estrutura da organização", 25),
        ("UC00601", "Analisar e planear sistemas de informação", 50),
        ("UC00602", "Modelar bases de dados relacionais", 25),
        ("UC00603", "Criar a estrutura de uma base de dados e programar em SQL", 50),
        ("UC00604", "Programar para a web, na vertente frontend (cliente-side)", 50),
        ("UC00605", "Programar para a web, na vertente servidor (server-side)", 50),
        ("UC00245", "Desenvolver algoritmos", 25),
        ("UC00606", "Desenvolver programas em linguagem estruturada", 50),
        ("UC00607", "Desenvolver programas complexos em linguagem estruturada", 50),
        ("UC00608", "Desenvolver programas em linguagem orientada a objetos", 50),
        ("UC00609", "Desenvolver de aplicações móveis (plataforma Android)", 50),
        ("UC00610", "Criar e integrar bases de dados noSQL nas apps", 25),
        ("UC00611", "Configurar redes de comunicação de dados", 25),
        ("UC00612", "Administrar sistemas operativos de rede", 25),
        ("UC00613", "Gerir políticas de segurança em sistemas informáticos", 25),
        ("UC00614", "Integrar sistemas de informação", 50),
        ("UC00615", "Desenvolver projeto de tecnologias e programação de sistemas de informação", 50),
        ("UC00616", "Implementar as normas de segurança e saúde no trabalho no setor de Informática", 25),
        ("UC00617", "Utilizar serviços Git e GitHub", 25),
        ("UC00621", "Criar aplicações em React", 25),
        ("UC00623", "Programar com sistemas de Inteligência Artificial", 25),
        ("UC00624", "Desenvolver de aplicações móveis (plataforma iOS)", 50),
        ("UC00627", "Instalar e configurar servidores Web", 25),
        ("UCFCT_TPSI", "Formação em Contexto de Trabalho", 575)
    ];

    // Corre em cada arranque: acrescenta o que falta e corrige nomes, horas e ordem.
    // As horas registadas pelo utilizador não são tocadas (ligam-se à UC pelo Id).
    public static async Task PopularAsync(SQLiteAsyncConnection conexao)
    {
        var existentes = (await conexao.Table<Uc>().ToListAsync())
            .ToDictionary(u => u.Codigo);

        for (var i = 0; i < Dados.Length; i++)
        {
            var (codigo, nome, horas) = Dados[i];
            var ordem = i + 1;

            if (!existentes.TryGetValue(codigo, out var uc))
            {
                await conexao.InsertAsync(new Uc
                {
                    Codigo = codigo,
                    Nome = nome,
                    HorasTotais = horas,
                    Ordem = ordem
                });
            }
            else if (uc.Nome != nome || uc.HorasTotais != horas || uc.Ordem != ordem)
            {
                uc.Nome = nome;
                uc.HorasTotais = horas;
                uc.Ordem = ordem;
                await conexao.UpdateAsync(uc);
            }
        }
    }
}