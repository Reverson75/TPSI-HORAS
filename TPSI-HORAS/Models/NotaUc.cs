using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace TPSI_HORAS.Models;

public class NotaUc
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Hoje: uma nota por UC. Para várias notas no futuro, basta remover [Unique].
    [Unique]
    public int UcId { get; set; }

    public double Valor { get; set; }

    public DateTime DataRegisto { get; set; }
}