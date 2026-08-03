using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
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

    public async Task CreateAppointment(AppointmentModel.request request)
    {
        var doctor = await _persistence.GetById<Doctor>(request.doctorId);
        if (doctor == null)
        {
            throw new Exception("No existe el Doctor");
        }

        var pacientes = await _persistence.GetAll<Patient>(); // No se puede filtrar aqui directamente porq es lazy
        var paciente = pacientes.SingleOrDefault(d => d.Dni == request.paciente.dni);

        if (paciente == null)
        {
            throw new Exception("No existe el Paciente");
        }

        var disponibilidad = await _persistence.GetById<AvailabilitySlot>(request.availabilityId);
        if (disponibilidad == null || disponibilidad.Status == AvailabilitySlotStatus.BOOKED)
        {
            throw new Exception("No disponible o Turno ya ocupado"); // Esto en realidad son 2 validaciones distintas
        }

        var cita = new Appointment
        {
            AttendedAt = null,
            CancelledAt = null,
            Reason = request.reason,
            Patient = paciente,
            PatientId = paciente.Id
        };

        disponibilidad.Status = AvailabilitySlotStatus.BOOKED; // una vez creada la cita, ya bloqueo ese turno que habia disponible

        await _persistence.Add<Appointment>(cita); // guardamos la cita
        await _persistence.Update<AvailabilitySlot>(disponibilidad); // actualizamos el estado del turno (tecnicamente se hace antes pero se entiende, aqui lo actualizamos en la bd)
    }

    //ver turnos activos del paciente
    public async Task<IEnumerable<AppointmentModel.patientResponse>> GetPatientAppointmentsAsync(int dni)
    {
        var appointments = await _persistence.GetAll<Appointment>();
        var dniString = dni.ToString();

        return appointments
            .Where(a => a.Patient != null && a.Patient.Dni == dniString && a.CancelledAt == null)
            .Select(a => new AppointmentModel.patientResponse(
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
        if (appointment == null)
        {
            throw new Exception("La cita no existe");
        }

        if (appointment.CancelledAt != null)
        {
            throw new Exception("El turno ya se encuentra cancelado");
        }

        appointment.CancelledAt = DateTime.UtcNow;
        await _persistence.Update<Appointment>(appointment);
    }

    //búsqueda avanzada de turnos (Admin)
    public async Task<IEnumerable<AppointmentModel.searchResponse>> SearchAppointmentsAsync(Guid? specialtyId, Guid? doctorId, string? dni, DateTime? date)
    {
        var appointments = await _persistence.GetAll<Appointment>();
        var query = appointments.AsQueryable();

        if (!string.IsNullOrEmpty(dni))
        {
            query = query.Where(a => a.Patient != null && a.Patient.Dni == dni);
        }

        return query.Select(a => new AppointmentModel.searchResponse(
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
    // 4. Completar / actualizar perfil del paciente
    public async Task CompleteProfileAsync(ProfileModel.UpdateProfileRequest request)
    {
        var patients = await _persistence.GetAll<Patient>();
        var patient = patients.FirstOrDefault(p => p.Dni == request.Dni);

        if (patient == null)
        {
            throw new EntityNotFoundException($"No se encontró un paciente registrado con ese DNI {request.Dni}");
        }

        patient.Name = request.Name;
        patient.PhoneNumber = request.Phone;

        await _persistence.Update(patient);
    }
}