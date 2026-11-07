using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TPSI_HORAS.Models;

public class RegistoHoras
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int UcId { get; set; }

    public DateTime Data { get; set; }

    public int Horas { get; set; }
}
