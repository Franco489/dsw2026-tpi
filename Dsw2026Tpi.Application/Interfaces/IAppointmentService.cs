using Dsw2026Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAppointmentService 
{
    Task CreateAppointment(AppointmentModel.request request);
//obtener turnos activos del paciente por DNI
    Task<IEnumerable<AppointmentModel.patientResponse>> GetPatientAppointmentsAsync(int dni);

//cancelar turno por ID
    Task CancelAppointmentAsync(Guid id);

//búsqueda avanzada de turnos (admin)
    Task<IEnumerable<AppointmentModel.searchResponse>> SearchAppointmentsAsync(Guid? specialtyId, Guid? doctorId, string? dni, DateTime? date);

 //completar o actualizar el perfil del paciente
    Task CompleteProfileAsync(ProfileModel.UpdateProfileRequest request);
}