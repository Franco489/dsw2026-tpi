using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Validation;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dsw2026Tpi.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IPersistence _persistence;

    public AppointmentService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task<AppointmentModel.ResponseCreate> CreateAppointment(AppointmentModel.Request request)
    {
       
        var doctor = await _persistence.GetById<Doctor>(request.DoctorId, "AvailabilityRules", "AvailabilityRules.Slots");
        var availabilitySlot = await _persistence.First<AvailabilitySlot>(s => s.Id == request.AvailabilitySlotId && s.Availability.DoctorId == request.DoctorId);
        var patient = await _persistence.First<Patient>(p => p.Dni == request.Patient.Dni);

        AppointmentValidator.ValidateCreate(doctor, availabilitySlot, patient, request.Reason);

        var newAppoiment = await _persistence.Add(new Appointment 
        {
            Reason = request.Reason,
            Patient = patient,
            PatientId = patient.Id,
            AvailabilitySlot = availabilitySlot,
            AvailabilitySlotId = availabilitySlot.Id
        });

        return new AppointmentModel.ResponseCreate(newAppoiment);
        
    }

    //ver turnos activos del paciente
    public async Task<IEnumerable<AppointmentModel.PatientResponse>> GetPatientAppointmentsAsync(int dni)
    {
        var appointments = await _persistence.GetAll<Appointment>();
        var dniString = dni.ToString();

        return appointments
            .Where(a => a.Patient != null && a.Patient.Dni == dniString && a.CancelledAt == null)
            .Select(a => new AppointmentModel.PatientResponse(
                a.Id,
                Guid.Empty,
                string.Empty,
                string.Empty,
                DateTime.MinValue,
                TimeSpan.Zero,
                a.Reason ?? string.Empty,
                "BOOKED"
            ));
    }

    //cancelar un turno
    public async Task CancelAppointmentAsync(Guid id)
    {
        var appointment = await _persistence.GetById<Appointment>(id);
        AppointmentValidator.ValidateDelete(appointment);
        appointment.CancelledAt = DateTime.UtcNow;
        await _persistence.Update<Appointment>(appointment);
    }

    //búsqueda avanzada de turnos (Admin)
    public async Task<IEnumerable<AppointmentModel.SearchResponse>> SearchAppointmentsAsync(Guid? specialtyId, Guid? doctorId, string? dni, DateTime? date)
    {
        var appointments = await _persistence.GetAll<Appointment>();
        var query = appointments.AsQueryable();

        if (!string.IsNullOrEmpty(dni))
        {
            query = query.Where(a => a.Patient != null && a.Patient.Dni == dni);
        }

        return query.Select(a => new AppointmentModel.SearchResponse(
            a.Id,
            string.Empty,
            string.Empty,
            a.Patient != null ? a.Patient.Name : string.Empty,
            a.Patient != null ? a.Patient.Dni : string.Empty,
            DateTime.MinValue,
            TimeSpan.Zero,
            a.CancelledAt != null ? "CANCELLED" : "BOOKED"
        ));
    }
}