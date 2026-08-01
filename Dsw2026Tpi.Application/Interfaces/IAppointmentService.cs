using Dsw2026Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAppointmentService 
{
    Task CreateAppointment(AppointmentModel.Request request);
// 1. Obtener turnos activos del paciente por DNI
    Task<IEnumerable<AppointmentModel.PatientResponse>> GetPatientAppointmentsAsync(int dni);

    // 2. Cancelar turno por ID
    Task CancelAppointmentAsync(Guid id);

    // 3. Búsqueda avanzada de turnos (Admin)
    Task<IEnumerable<AppointmentModel.SearchResponse>> SearchAppointmentsAsync(Guid? specialtyId, Guid? doctorId, string? dni, DateTime? date);
}