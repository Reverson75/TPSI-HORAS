using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace TPSI_HORAS.Models;

public class Uc
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public int HorasTotais { get; set; }

    public int Ordem { get; set; }
}