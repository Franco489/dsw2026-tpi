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

    public async Task CreateDoctor(DoctorModel.Request request)
    {
        DoctorValidator.Validate(request);
        var speciality = await _persistence.GetById<Speciality>(request.SpecialityId);
        if(speciality == null)
        {
            throw new EntityNotFoundException(nameof(Speciality));
        }

        var doctor = new Doctor(request.Name, request.LicenseNumber, request.SpecialityId);
        await _persistence.Add(doctor);
    }

    public async Task<Pagination<AvailabilityModel.DaySchedule>> GetAvailabilities(Guid id, int pageSize, int pageIndex)
    {
        var doctor = await _persistence.GetById<Doctor>(id);
        if (doctor == null)
        {
            throw new EntityNotFoundException(nameof(Doctor));
        }

        var slots = await _persistence.Paginate<AvailabilitySlot, DateOnly>(pageSize, pageIndex,
                                                                   s => s.Availability.DoctorId == id, s => s.Date);
        return slots.Map(s => new AvailabilityModel.DaySchedule(s.Date.ToString("dd/MM/yyyy"), s.EndTime, s.StartTime));


    }

    public async Task<Pagination<DoctorModel.Response>> GetAll(int pageSize, int pageIndex, string? name = null)
    {
        var doctors = await _persistence.Paginate<Doctor, string>(pageSize, pageIndex, d => string.IsNullOrWhiteSpace(name) && !d.Deleted ||
                                                   d.Name.Contains(name) && !d.Deleted, x => x.Name, nameof(Doctor.Speciality));

        return doctors.Map(d => new DoctorModel.Response(d.Id, d.Name, d.LicenseNumber,
            new DoctorModel.SpecialityDto(d.Speciality?.Id, d.Speciality?.Name)));
    }
    public async Task UpdateDoctor(Guid id, DoctorModel.Request request)
    {
        var speciality = await _persistence.GetById<Speciality>(request.SpecialityId);
        if (speciality == null)
        {
            throw new EntityNotFoundException(nameof(Speciality));
        }

        var doctor = await _persistence.GetById<Doctor>(id);
        if (doctor == null)
        {
            throw new EntityNotFoundException(nameof(Doctor));
        }
        DoctorValidator.Validate(request);
        doctor.Update(request.Name, request.LicenseNumber, request.SpecialityId);
        await _persistence.Update(doctor);
    }

    public async Task DeleteDoctor(Guid id)
    {
        var doctor = await _persistence.GetById<Doctor>(id);
        if (doctor == null)
        {
            throw new EntityNotFoundException(nameof(Doctor));
        }
        doctor.Delete();
        await _persistence.Update(doctor);
    }

    public async Task ReactivateDoctor(Guid id)
    {
        var doctor = await _persistence.GetById<Doctor>(id);
        if (doctor == null)
        {
            throw new EntityNotFoundException(nameof(Doctor));
        }
        doctor.Activate();
        await _persistence.Update(doctor);
    }
}
