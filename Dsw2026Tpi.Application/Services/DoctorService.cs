using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Validation;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
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
        var doctors = await _persistence.Paginate<Doctor, string>(pageSize, pageIndex, d => string.IsNullOrWhiteSpace(name) && !d.Deleted ||
                                                   d.Name.Contains(name) && !d.Deleted, x => x.Name, nameof(Doctor.Speciality));

        return doctors.Map(d => new DoctorModel.Response(d.Id, d.Name, d.LicenseNumber,
            new DoctorModel.SpecialityDto(d.Speciality?.Id, d.Speciality?.Name)));
    }


    public async Task<List<AvailabilityModel.DayScheduleResponse>> GetAvailabilities(Guid id)
    {
        var doctor = await _persistence.GetById<Doctor>(id, "AvailabilityRules", "AvailabilityRules.Slots");
        if (doctor == null)
        {
            throw new EntityNotFoundException(nameof(Doctor));
        }
        if (doctor.AvailabilityRules is null)
        {
            return new List<AvailabilityModel.DayScheduleResponse>();
        }

        var availabilities = new List<AvailabilityModel.DayScheduleResponse>();

        foreach (var rule in doctor.AvailabilityRules)
        {
            foreach (var slot in rule.Slots)
            {
                var dto = new AvailabilityModel.DayScheduleResponse(slot.Id, slot.Date.ToShortDateString() , slot.StartTime, slot.EndTime);
                availabilities.Add(dto);
               
            }
        }

        return availabilities;
    }

    public async Task<DoctorModel.Response> CreateDoctor(DoctorModel.Request request)
    {
        DoctorValidator.Validate(request);
        var speciality = await _persistence.GetById<Speciality>(request.SpecialityId);
        if(speciality == null)
        {
            throw new EntityNotFoundException(nameof(Speciality));
        }

        var doctor = new Doctor(request.Name, request.LicenseNumber, request.SpecialityId);
        await _persistence.Add(doctor);

        return new DoctorModel.Response(doctor.Id, doctor.Name, doctor.LicenseNumber,
            new DoctorModel.SpecialityDto(speciality.Id, speciality.Name));
    }


    public async Task<DoctorModel.Response> UpdateDoctor(Guid id, DoctorModel.Request request)
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

        return new DoctorModel.Response(doctor.Id, doctor.Name, doctor.LicenseNumber,
            new DoctorModel.SpecialityDto(speciality.Id, speciality.Name));
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
