using Dsw2026Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAppointmentService 
{
    Task CreateAppointment(AppointmentModel.request request);
// 1. Obtener turnos activos del paciente por DNI
    Task<IEnumerable<AppointmentModel.patientResponse>> GetPatientAppointmentsAsync(int dni);

    // 2. Cancelar turno por ID
    Task CancelAppointmentAsync(Guid id);

    // 3. Búsqueda avanzada de turnos (Admin)
    Task<IEnumerable<AppointmentModel.searchResponse>> SearchAppointmentsAsync(Guid? specialtyId, Guid? doctorId, string? dni, DateTime? date);
}