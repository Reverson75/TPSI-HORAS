using System;
using System.Collections.Generic;
using System.Text;

using TPSI_HORAS.Models;

namespace TPSI_HORAS.Services;

public interface IUcService
{
    Task<IReadOnlyList<UcResumo>> ObterResumosAsync();
    Task<UcResumo?> ObterResumoAsync(int ucId);
    Task<IReadOnlyList<RegistoHoras>> ObterHistoricoAsync(int ucId);
    Task RegistarHorasAsync(int ucId, int horas, DateTime data);
    Task ApagarRegistoAsync(int registoId);
}