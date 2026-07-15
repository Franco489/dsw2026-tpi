using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Validation;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;

namespace Dsw2026Tpi.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IPersistence _persistence;

    public DoctorService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var doctors = await _persistence.Paginate<Doctor, string>(pageSize, pageIndex, d => string.IsNullOrWhiteSpace(name) ||
                                                   d.Name.Contains(name), x => x.Name, nameof(Doctor.Speciality));

        return doctors.Map(d => new DoctorModel.Response(d.Id, d.Name, d.LicenseNumber,
            new DoctorModel.SpecialityDto(d.Speciality?.Id, d.Speciality?.Name)));
    }

    public async Task AddDoctor(DoctorModel.Request request)
    {
        var speciality = await _persistence.GetById<Speciality>(request.SpecialityId);
        if (speciality == null)
        {
            throw new EntityNotFoundException($"{request.SpecialityId}");
        }

        var doctor = new Doctor(request.Name, request.LicenseNumber, request.SpecialityId);
        await _persistence.Add<Doctor>(doctor);
    }

    public async Task UpdateDoctor(Guid id, DoctorModel.Request request)
    {
        var doctor = await _persistence.GetById<Doctor>(id);
        if (doctor == null)
        {
            throw new EntityNotFoundException($"{id}");
        }

        var speciality = await _persistence.GetById<Speciality>(request.SpecialityId);
        if (speciality == null)
        {
            throw new EntityNotFoundException($"{request.SpecialityId}");
        }

        doctor.Update(request.SpecialityId, request.Name, request.LicenseNumber);
        await _persistence.Update<Doctor>(doctor);
    }
}
