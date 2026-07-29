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

    #region Lógica de Feriados

    private List<HolidayDto> GetHolidays()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "holidays.json");

        if (!File.Exists(filePath))
            return new List<HolidayDto>();

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<HolidayDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<HolidayDto>();
    }

    private bool IsHoliday(DateOnly date)
    {
        var holidays = GetHolidays();
        // convierto el DateOnly a DateTime para comparar con el json
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        return holidays.Any(h => h.Date.Date == dateTime.Date);
    }

    #endregion

    private List<AvailabilitySlot> GenerateSlots(Guid doctorId, TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek)
    {
        var actualDate = DateOnly.FromDateTime(DateTime.Now);
        var actualMonth = actualDate.Month;
        var actualYear = actualDate.Year;
        var numOfDays = DateTime.DaysInMonth(actualYear, actualMonth);
        ICollection<AvailabilitySlot> slots = [];

        for (int d = actualDate.Day; d <= numOfDays; d++)
        {
            var iterationDate = new DateOnly(actualYear, actualMonth, d);

            // SI EL DÍA ES FERIADO, NO SE GENERAN SLOTS PARA ESTE DÍA
            if (IsHoliday(iterationDate))
            {
                continue;
            }

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
                    if (rule.Slots.Any(s => s.Status != AvailabilitySlotStatus.AVAILABLE))
                    {
                        throw new InvalidOperationException("Existen turnos reservados/bloqueados para la disponiblidad actual. No es posible modificar los horarios asignados");
                    }
                    rule.StartTime = daySchedule.StartTime;
                    rule.EndTime = daySchedule.EndTime;
                    rule.Slots.Clear();

                    foreach (var slot in GenerateSlots(request.DoctorId, daySchedule.StartTime, daySchedule.EndTime, dayOfWeek))
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
    }
}