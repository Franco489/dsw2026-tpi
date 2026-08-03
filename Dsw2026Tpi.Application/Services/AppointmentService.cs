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
using System.Net.WebSockets;

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

        var newAppoiment = new Appointment 
        {
            Reason = request.Reason,
            Patient = patient,
            PatientId = patient.Id,
            AvailabilitySlot = availabilitySlot,
            AvailabilitySlotId = availabilitySlot.Id
        };
        await _persistence.Add(newAppoiment);
        return new AppointmentModel.ResponseCreate(newAppoiment);
        
    }

    //ver turnos activos del paciente
    public async Task<IEnumerable<AppointmentModel.PatientResponse>> GetPatientAppointmentsAsync(int dni)
    {
        var patient = await _persistence.First<Patient>(p => p.Dni == dni.ToString());
        if (patient == null) 
        {
            throw new EntityNotFoundException($"No existe el paciente con DNI {dni}");
        }
        List<AppointmentModel.PatientResponse> result = [];
        var appointments = await _persistence.GetFiltered<Appointment>((a => a.PatientId == patient.Id && a.Status == AppointmentStatus.BOOKED),
            "Patient", "AvailabilitySlot", "AvailabilitySlot.Availability", "AvailabilitySlot.Availability.Doctor", "AvailabilitySlot.Availability.Doctor.Speciality");
        //TODO: falta una excepcion acá en caso de que la lista sea null

        foreach (var a in appointments) 
        {
            //TODO: Hay que traer al doctor y la epecialida....
            result.Add(new AppointmentModel.PatientResponse
                (
                    a.PatientId,
                    a.AvailabilitySlot.DoctorId,
                    a.AvailabilitySlot.Availability.Doctor.Name,
                    a.AvailabilitySlot.Availability.Doctor.Speciality.Name,
                    a.AvailabilitySlot.Date,
                    a.AvailabilitySlot.StartTime,
                    a.Reason,
                    a.Status.ToString()
                ));
        }
       return result;
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