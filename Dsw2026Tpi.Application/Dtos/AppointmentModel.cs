using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public record AppointmentModel
{
    public record request(Guid doctorId, Guid availabilityId, patientAux paciente, string reason);
    public record patientAux(string dni);

}
