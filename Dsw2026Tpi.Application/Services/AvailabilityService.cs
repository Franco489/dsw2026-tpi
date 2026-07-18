using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Validation;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Application.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IPersistence _persistence;
    public AvailabilityService(IPersistence persistence)
    {
        _persistence = persistence;
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
            if(daySchedule.StartTime >= daySchedule.EndTime) 
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

            for (int d = actualDate.Day; d <= numOfDays; d++)
            {
                var iterationDate = new DateOnly(actualYear, actualMonth, d);

                if (iterationDate.DayOfWeek == dayOfWeek)
                {
                    var slotStartTime = daySchedule.StartTime;
                    while (slotStartTime < daySchedule.EndTime)
                    {
                        var slotEndTime = slotStartTime.AddMinutes(30);

                        if (slotEndTime > daySchedule.EndTime)
                        { 
                            break; 
                        }
                        avaRule.Slots.Add(new AvailabilitySlot
                        {
                            Date = iterationDate,
                            StartTime = slotStartTime,
                            EndTime = slotEndTime,
                            DoctorId = request.DoctorId
                        });
                        slotStartTime = slotEndTime;
                    }
                }
            }
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
}
