using System;
using System.Collections.Generic;
using System.Text;

namespace TPSI_HORAS.Models;

public record UcResumo(int Id, string Codigo, string Nome, int HorasTotais, int HorasFeitas)
{
    public double Progresso => HorasTotais == 0 ? 0 : (double)HorasFeitas / HorasTotais;

    public string TextoHoras => $"{HorasFeitas} / {HorasTotais} h";

    public int HorasEmFalta => Math.Max(0, HorasTotais - HorasFeitas);

    public string Rotulo => $"{Codigo} – {Nome}";
}