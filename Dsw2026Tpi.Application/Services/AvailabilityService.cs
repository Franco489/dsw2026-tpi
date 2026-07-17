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
            var avaRule = new Availability
            {
                DoctorId = doctor.Id,
                Doctor = doctor,
                Month = (byte)actualMonth,
                Year = (short)actualYear,
                DayOfWeek = (byte)dayOfWeek,
                StartTime = daySchedule.StartTime,
                EndTime = daySchedule.EndTime
            };

            for (int d = actualDate.Day; d <= numOfDays; d++)
            {
                var iterationDate = new DateOnly(actualYear, actualMonth, d);
                var slotStartTime = daySchedule.StartTime;
                if (iterationDate.DayOfWeek == dayOfWeek)
                {
                    while (slotStartTime < daySchedule.EndTime)
                    {
                        var slotEndTime = slotStartTime.AddMinutes(30);

                        if (slotEndTime > daySchedule.EndTime) break;
                        avaRule.Slots.Add(new AvailabilitySlot
                        {
                            Date = iterationDate,
                            StartTime = slotStartTime,
                            EndTime = slotEndTime
                        });
                        slotStartTime = slotEndTime;
                    }
                }
                d += 5; // Nos ahorramos un par de iteraciones saltando a la siguiente semana
                avaRules.Add(avaRule);
            }
            await _persistence.AddRange(avaRules); // TODO: <- Optimizar. No sé si esta sea la forma más efectiva. 
            foreach (var a in avaRules) 
            {
                await _persistence.AddRange(a.Slots.ToList());
            }
            //TODO: El método funciona parcialmente. A veces registra los slots y a veces no. 
        }
    }
}
