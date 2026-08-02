using Dsw2026Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAppointmentService 
{
    Task<AppointmentModel.ResponseCreate> CreateAppointment(AppointmentModel.Request request);
    Task<IEnumerable<AppointmentModel.PatientResponse>> GetPatientAppointmentsAsync(int dni);
    Task CancelAppointmentAsync(Guid id);
    Task<IEnumerable<AppointmentModel.SearchResponse>> SearchAppointmentsAsync(Guid? specialtyId, Guid? doctorId, string? dni, DateTime? date);
}