using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Validation;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;

namespace Dsw2026Tpi.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IPersistence _persistence;

    public AppointmentService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<AppointmentModel.Response> CreateAppointment(AppointmentModel.Request request)
    {
        var doctor = await _persistence.GetById<Doctor>(request.DoctorId, "AvailabilityRules", "AvailabilityRules.Slots", "Speciality");
        var availabilitySlot = await _persistence.First<AvailabilitySlot>(s => s.Id == request.AvailabilitySlotId && s.Availability.DoctorId == request.DoctorId);
        var patient = await _persistence.First<Patient>(p => p.Dni == request.Patient.Dni);

        AppointmentValidator.ValidateCreate(doctor, availabilitySlot, patient, request.Reason);

        var newAppoiment = new Appointment
        {
            Reason = request.Reason,
            Patient = patient,
            PatientId = patient.Id,
            AvailabilitySlot = availabilitySlot,
            AvailabilitySlotId = availabilitySlot.Id,
        };
        try
        {
            await _persistence.Add(newAppoiment);
        }
        catch (DbUpdateConcurrencyException Cex) 
        {
            throw new ConflictException(nameof(ErrorCodes.APPOINTMENT_CONFLICT), ErrorCodes.APPOINTMENT_CONFLICT);
        }
        
        return new AppointmentModel.Response(newAppoiment.Id, newAppoiment.Status.ToString(), availabilitySlot.Date,
            new AppointmentModel.PatientDto(patient.Dni, patient.Name),
            new AppointmentModel.DoctorDto(doctor.Id, doctor.Name,
                new AppointmentModel.SpecialtyDto(doctor.Speciality.Id, doctor.Speciality.Name))
        );
    }
    public async Task<IEnumerable<AppointmentModel.Response>> GetPatientAppointmentsAsync(string dni)
    {
        var patient = await _persistence.First<Patient>(p => p.Dni.Equals(dni));
        if (patient == null)
        {
            throw new EntityNotFoundException(dni);
        }

        List<AppointmentModel.Response> result = [];
        var appointments = await _persistence.GetFiltered<Appointment>((a => a.PatientId == patient.Id && a.Status == AppointmentStatus.BOOKED),
            "Patient", "AvailabilitySlot", "AvailabilitySlot.Availability", "AvailabilitySlot.Availability.Doctor", "AvailabilitySlot.Availability.Doctor.Speciality");

        foreach (var a in appointments)
        {
            result.Add(new AppointmentModel.Response
            (
                a.Id, a.Status.ToString(), a.AvailabilitySlot.Date,
                new AppointmentModel.PatientDto(a.Patient.Dni, a.Patient.Name),
                new AppointmentModel.DoctorDto(a.AvailabilitySlot.DoctorId, a.AvailabilitySlot.Availability.Doctor.Name,
                    new AppointmentModel.SpecialtyDto(a.AvailabilitySlot.Availability.Doctor.SpecialityId, a.AvailabilitySlot.Availability.Doctor.Speciality.Name))
            ));
        }

        return result;
    }

    // cancelar un turno
    public async Task CancelAppointmentAsync(Guid id)
    {
        var appointment = await _persistence.GetById<Appointment>(id, "AvailabilitySlot");
        AppointmentValidator.ValidateDelete(appointment);
        appointment.Cancel();
        await _persistence.Update<Appointment>(appointment);
    }

    public async Task<AppointmentModel.ResponseDates> GetAppointmentsByDate (DateOnly date)
    {
        var appointments = await _persistence.GetFiltered<Appointment>(a => a.AvailabilitySlot.Date == date, "Patient", "AvailabilitySlot"
            , "AvailabilitySlot.Availability", "AvailabilitySlot.Availability.Doctor", "AvailabilitySlot.Availability.Doctor.Speciality");
        if (!appointments.Any())
        {
            throw new EntityNotFoundException(nameof(Appointment));
        }
        var response = new List<AppointmentModel.Response>();
        foreach (var a in appointments)
        {
            response.Add(new AppointmentModel.Response(a.Id, a.Status.ToString(), a.AvailabilitySlot.Date,
                    new AppointmentModel.PatientDto(a.Patient.Dni, a.Patient.Name),
                    new AppointmentModel.DoctorDto(a.AvailabilitySlot.DoctorId, a.AvailabilitySlot.Availability.Doctor.Name,
                    new AppointmentModel.SpecialtyDto(a.AvailabilitySlot.Availability.Doctor.SpecialityId, a.AvailabilitySlot.Availability.Doctor.Speciality.Name))));
        }
        return new AppointmentModel.ResponseDates(response);
    }

    // búsqueda combinada de turnos
    public async Task<Pagination<AppointmentModel.Response>> CombinedSearch(int pageSize, int pageIndex, Guid? specialtyId, Guid? doctorId, string? dni, DateOnly? date)
    {

        Expression<Func<Appointment, bool>> combinedPredicate = a =>
            (string.IsNullOrEmpty(dni) || a.Patient.Dni == dni) &&
            (!doctorId.HasValue || a.AvailabilitySlot.Availability.Doctor.Id == doctorId) &&
            (!specialtyId.HasValue || a.AvailabilitySlot.Availability.Doctor.SpecialityId == specialtyId) &&
            (!date.HasValue || a.AvailabilitySlot.Date == date);

        var response = await _persistence.Paginate<Appointment, string>(pageSize, pageIndex,
            combinedPredicate,
            r => r.Patient.Dni, "Patient", "AvailabilitySlot.Availability.Doctor", "AvailabilitySlot.Availability.Doctor.Speciality"
        );

        return response.Map(r => new AppointmentModel.Response(r.Id, r.Status.ToString(), r.AvailabilitySlot.Date,
            new AppointmentModel.PatientDto(r.Patient.Dni, r.Patient.Name),
            new AppointmentModel.DoctorDto(r.AvailabilitySlot.DoctorId, r.AvailabilitySlot.Availability.Doctor.Name,
                new AppointmentModel.SpecialtyDto(r.AvailabilitySlot.Availability.Doctor.SpecialityId, r.AvailabilitySlot.Availability.Doctor.Speciality.Name)))
        );
    }
}