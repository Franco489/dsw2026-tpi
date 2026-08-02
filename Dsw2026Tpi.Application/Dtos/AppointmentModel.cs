using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public record AppointmentModel
{
    public record Request(Guid DoctorId, Guid AvailabilitySlotId, PatientAux Patient, string Reason);
    public record ResponseCreate(Appointment NewAppointment);
    public record PatientAux(string Dni);
    public record PatientResponse(
        Guid Id,
        Guid DoctorId,
        string DoctorName,
        string SpecialtyName,
        DateTime Date,
        TimeSpan StartTime,
        string Reason,
        string Status
    );

    public record SearchResponse(
        Guid Id,
        string Specialty,
        string Doctor,
        string PatientName,
        string PatientDni,
        DateTime Date,
        TimeSpan AvailableTime,
        string Status
    );
}