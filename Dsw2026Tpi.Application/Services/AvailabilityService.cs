using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Utils;
using Dsw2026Tpi.Application.Validation;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.WebSockets;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static Dsw2026Tpi.Application.Dtos.AvailabilityModel;

namespace Dsw2026Tpi.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IPersistence _persistence;
    public AvailabilityService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    private List<AvailabilitySlot> GenerateSlots(Guid doctorId,TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek) 
    {
        var actualDate = DateOnly.FromDateTime(DateTime.Now);
        var actualMonth = actualDate.Month;
        var actualYear = actualDate.Year;
        var numOfDays = DateTime.DaysInMonth(actualYear, actualMonth);
        ICollection<AvailabilitySlot> slots = [];

        for (int d = actualDate.Day; d <= numOfDays; d++)
        {
            var iterationDate = new DateOnly(actualYear, actualMonth, d);

            if (iterationDate.DayOfWeek == dayOfWeek)
            {
                var slotStartTime = startTime;
                while (slotStartTime < endTime)
                {
                    var slotEndTime = slotStartTime.AddMinutes(30);

                    if (slotEndTime > endTime)
                    {
                        break;
                    }
                    slots.Add(new AvailabilitySlot
                    {
                        Date = iterationDate,
                        StartTime = slotStartTime,
                        EndTime = slotEndTime,
                        DoctorId = doctorId
                    });
                    slotStartTime = slotEndTime;
                }
            }
        }
        return slots.ToList();
    }
    public async Task CreateAvailabilitiesAsync(AvailabilityModel.Request request)
    {
        var doctor = await _persistence.GetById<Doctor>(request.DoctorId);
        if (doctor == null)
        {
            throw new EntityNotFoundException($"No se encontró un doctor con el ID {request.DoctorId}");
        }
        var actualDate = DateOnly.FromDateTime(DateTime.Now);
        var actualMonth = actualDate.Month;
        var actualYear = actualDate.Year;
        var numOfDays = DateTime.DaysInMonth(actualYear, actualMonth);

        var avaRules = new List<Availability>();

        foreach (var daySchedule in request.Days)
        {
            var dayOfWeek = daySchedule.Day.toDayOfWeek();
            if (daySchedule.EndTime < daySchedule.StartTime.AddMinutes(30))
            {
                throw new ArgumentOutOfRangeException("El horario de inicio debe ser 30 minutos menor al horario de fin.");
            }
            var avaRule = new Availability
            {
                DoctorId = doctor.Id,
                Month = (byte)actualMonth,
                Year = (short)actualYear,
                DayOfWeek = (byte)dayOfWeek,
                StartTime = daySchedule.StartTime,
                EndTime = daySchedule.EndTime,
                Slots = []
            };

            avaRule.Slots = GenerateSlots(request.DoctorId, daySchedule.StartTime, daySchedule.EndTime, dayOfWeek);
            if (avaRule.Slots.Any())
            {
                avaRules.Add(avaRule);
            }
        }
        if (avaRules.Any())
        {
            await _persistence.AddRange(avaRules);
        }
    }
    public async Task UpdateAvailabilitiesAsync(AvailabilityModel.Request request)
    {
        var availabilities = await _persistence.GetFiltered<Availability>((a => a.DoctorId == request.DoctorId), nameof(Availability.Slots));
        //List<Availability> updatedRules = [];
        if (!availabilities.Any()) 
        {
            throw new EntityNotFoundException($"No se encontraron disponibilidades asociadas a la ID {request.DoctorId}. Revise la ID ingresada o intente crear una nueva disponibilidad");
        }
        foreach (var rule in availabilities) 
        {
            foreach (var daySchedule in request.Days)
            {
                var dayOfWeek = daySchedule.Day.toDayOfWeek();
                if (rule.DayOfWeek == (byte)dayOfWeek)
                {
                    //var bookedSlots = rule.Slots.Where(s => s.Status != AvailabilitySlotStatus.AVAILABLE); La línea de abajo es más eficiente respecto a la versión anterior. Es mejor un Any() que un Count() en este caso.
                    if (rule.Slots.Any(s => s.Status != AvailabilitySlotStatus.AVAILABLE))
                    {
                        throw new InvalidOperationException("Existen turnos reservados/bloqueados para la disponiblidad actual. No es posible modificar los horarios asignados"); //TODO: Mejorar el manejo de este caso
                    }
                    rule.StartTime = daySchedule.StartTime;
                    rule.EndTime = daySchedule.EndTime;
                    rule.Slots.Clear();//el clear entiendo que sirve para la trazabilidad. Así EF entiende que primero se borró (por ende borra los registros) y después se agregaron los nuevos.
                    //TODO: Directamente los regeneramos, creo que es un bardo hacer lógica para modificar los slots existentes.
                    foreach(var slot in GenerateSlots(request.DoctorId, daySchedule.StartTime, daySchedule.EndTime, dayOfWeek))
                    {
                        rule.Slots.Add(slot);
                    }
                    //updatedRules.Add(rule);
                }
            } 
        }
        if (availabilities.Any()) 
        {
            await _persistence.UpdateRange(availabilities.ToList());
        }
    }
}