using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Utils;
using Dsw2026Tpi.Application.Validation;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using static Dsw2026Tpi.Application.Dtos.AvailabilityModel;

namespace Dsw2026Tpi.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IPersistence _persistence;

    public AvailabilityService(IPersistence persistence)
    {
        _persistence = persistence;
    }
   
    private List<AvailabilitySlot> GenerateSlots(Guid doctorId,TimeOnly startTime, 
        TimeOnly endTime, DayOfWeek dayOfWeek, DateOnly actualDate, int numOfDays) 
    {
        ICollection<AvailabilitySlot> slots = [];

        for (int d = actualDate.Day; d <= numOfDays; d++)
        {
            var iterationDate = new DateOnly(actualDate.Year, actualDate.Month,d);

            if (iterationDate.DayOfWeek == dayOfWeek && !iterationDate.IsHoliday())
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

    public async Task<AvailabilityModel.Response> CreateAvailabilitiesAsync(AvailabilityModel.Request request)
    {
        var doctor = await _persistence.GetById<Doctor>(request.DoctorId, "AvailabilityRules");
        if (doctor == null)
        {
            throw new EntityNotFoundException(request.DoctorId.ToString());
        }
        if (doctor.AvailabilityRules.Count() > 0)
        {
            throw new ConflictException(nameof(ErrorCodes.AVAILABILITY_CONFLICT), ErrorCodes.AVAILABILITY_CONFLICT);
        }
        var actualDate = DateOnly.FromDateTime(DateTime.Now);

        var numOfDays = DateTime.DaysInMonth(actualDate.Year, actualDate.Month);

        var avaRules = new List<Availability>();

        foreach (var daySchedule in request.Days)
        {
            if (daySchedule.EndTime < daySchedule.StartTime.AddMinutes(30))
            {
                throw new ArgumentOutOfRangeException("El horario de inicio debe ser 30 minutos menor al horario de fin.");
            }
            var dayOfWeek = daySchedule.Day.toDayOfWeek();
            var avaRule = new Availability
            {
                DoctorId = doctor.Id,
                Month = (byte)actualDate.Month,
                Year = (short)actualDate.Year,
                DayOfWeek = (byte)dayOfWeek,
                StartTime = daySchedule.StartTime,
                EndTime = daySchedule.EndTime,
                Slots = []
            };

            avaRule.Slots = GenerateSlots(request.DoctorId, daySchedule.StartTime, 
                daySchedule.EndTime, dayOfWeek, actualDate, numOfDays);
            if (avaRule.Slots.Any())
            {
                avaRules.Add(avaRule);
            }
        }
        if (avaRules.Any())
        {
            await _persistence.AddRange(avaRules);
        }

        return new AvailabilityModel.Response(doctor.Id, request.Days);
    }

    public async Task<AvailabilityModel.Response> UpdateAvailabilitiesAsync(AvailabilityModel.Request request)
    {
        var availabilities = await _persistence.GetFiltered<Availability>((a => a.DoctorId == request.DoctorId), nameof(Availability.Slots));

        if (!availabilities.Any()) 
        {
            throw new EntityNotFoundException($"No se encontraron disponibilidades asociadas a la ID {request.DoctorId}. Revise la ID ingresada o intente crear una nueva disponibilidad");
        }
        var actualDate = DateOnly.FromDateTime(DateTime.Now);
        var numOfDays = DateTime.DaysInMonth(actualDate.Year, actualDate.Month);
        foreach (var rule in availabilities) 
        {
            foreach (var daySchedule in request.Days)
            {
                if (daySchedule.EndTime < daySchedule.StartTime.AddMinutes(30))
                {
                    throw new ArgumentOutOfRangeException("El horario de inicio debe ser 30 minutos menor al horario de fin.");
                }
                var dayOfWeek = daySchedule.Day.toDayOfWeek();
                if (rule.DayOfWeek == (byte)dayOfWeek)
                {
                    if (rule.Slots.Any(s => s.Status != AvailabilitySlotStatus.AVAILABLE))
                    {
                        throw new InvalidOperationException("Existen turnos reservados/bloqueados para la disponiblidad actual. No es posible modificar los horarios asignados");
                    }
                    rule.StartTime = daySchedule.StartTime;
                    rule.EndTime = daySchedule.EndTime;
                    rule.Slots.Clear();

                    foreach(var slot in GenerateSlots(request.DoctorId, daySchedule.StartTime, daySchedule.EndTime, dayOfWeek, actualDate, numOfDays))
                    {
                        rule.Slots.Add(slot);
                    }
                }
            }
        }
        if (availabilities.Any())
        {
            await _persistence.UpdateRange(availabilities.ToList());
        }
        return new AvailabilityModel.Response(request.DoctorId, request.Days);
    }
}