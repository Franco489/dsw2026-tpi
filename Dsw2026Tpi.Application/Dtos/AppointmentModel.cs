using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public record AppointmentModel
{
    public record request(Guid doctorId, Guid availabilityId, patientAux paciente, string reason);
    public record patientAux(string dni);
    public record patientResponse(
        Guid id,
        Guid doctorId,
        string doctorName,
        string specialtyName,
        DateTime date,
        TimeSpan startTime,
        string reason,
        string status
    );

    public record searchResponse(
        Guid id,
        string specialty,
        string doctor,
        string patientName,
        string patientDni,
        DateTime date,
        TimeSpan availableTime,
        string status
    );
}