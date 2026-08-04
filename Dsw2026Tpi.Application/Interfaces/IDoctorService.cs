using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IDoctorService
{
    Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null);
    Task<DoctorModel.Response> CreateDoctor(DoctorModel.Request request);
    Task<DoctorModel.Response> UpdateDoctor(Guid id, DoctorModel.Request request);
    Task DeleteDoctor(Guid id);
    Task ReactivateDoctor(Guid id);
    Task<Pagination<AvailabilityModel.DayScheduleResponse>> GetAvailabilities(Guid id, int pageSize, int pageIndex);
}
