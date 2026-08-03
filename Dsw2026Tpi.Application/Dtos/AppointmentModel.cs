using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public record AppointmentModel
{
    public record Request(Guid DoctorId, Guid AvailabilitySlotId, PatientAux Patient, string Reason);
    public record PatientAux(string Dni);
    public record PatientResponse(
        Guid Id,
        Guid DoctorId,
        string DoctorName,
        string SpecialityName,
        DateOnly Date,
        TimeOnly StartTime,
        string Reason,
        string Status
    );

    public record PatientDto(string Dni, string Name);
    public record DoctorDto(Guid Id, string Name, SpecialtyDto Specialty);
    public record SpecialtyDto(Guid Id, string Name);
    public record Response(Guid Id, AppointmentStatus Status, DateOnly Date, PatientDto Patient, DoctorDto Doctor);

}