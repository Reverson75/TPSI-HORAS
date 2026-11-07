using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace TPSI_HORAS.Models;

public class Avaliacao
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int UcId { get; set; }

    public TipoAvaliacao Tipo { get; set; }

    public DateTime DataHora { get; set; }

    // Minutos de antecedência do lembrete; null = sem lembrete
    public int? LembreteAntesMin { get; set; }

    public string? Notas { get; set; }

    // Id usado para cancelar/reagendar a notificação
    public int NotificacaoId { get; set; }
}