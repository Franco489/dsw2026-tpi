using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IAppointmentService 
{
    Task<AppointmentModel.ResponseCreate> CreateAppointment(AppointmentModel.Request request);
    Task<IEnumerable<AppointmentModel.PatientResponse>> GetPatientAppointmentsAsync(int dni);
    Task CancelAppointmentAsync(Guid id);
    Task<Pagination<AppointmentModel.ResponseCreate>> CombinedSearch(int pageSize, int pageIndex, Guid? specialtyId, Guid? doctorId, string dni, DateOnly? date);
}