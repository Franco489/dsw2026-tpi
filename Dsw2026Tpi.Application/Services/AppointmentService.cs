using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
        
        if (pacientes == null)
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
        

        
    }






}
