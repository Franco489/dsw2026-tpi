using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Interfaces;

public interface IDoctorService
{
    Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null);
    Task CreateDoctor(DoctorModel.Request request);
    Task UpdateDoctor(Guid id, DoctorModel.Request request);
    Task DeleteDoctor(Guid id);
    Task ReactivateDoctor(Guid id);
}
