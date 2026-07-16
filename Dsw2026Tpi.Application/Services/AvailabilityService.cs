using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
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
        
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var mesActual = hoy.Month;
        var anioActual = hoy.Year;
        var diasEnElMes = DateTime.DaysInMonth(anioActual, mesActual);

      
        var nuevasReglas = new List<Availability>();

       
        foreach (var scheduleDto in request.Days)
        {
            var diaCsharp = TraducirDia(scheduleDto.day);

           
            var regla = new Availability
            {
                DoctorId = request.DoctorId,
                Month = (byte)mesActual,
                Year = (short)anioActual,
                DayOfWeek = (byte)diaCsharp,
                StartTime = scheduleDto.startTime,
                EndTime = scheduleDto.endTime,
                Slots = new List<AvailabilitySlot>() 
            };

            
            for (int dia = hoy.Day; dia <= diasEnElMes; dia++)
            {
                var fechaIteracion = new DateOnly(anioActual, mesActual, dia);

                if (fechaIteracion.DayOfWeek == diaCsharp)
                {
                    
                    var horaInicioTurno = scheduleDto.startTime;

                    while (horaInicioTurno < scheduleDto.endTime)
                    {
                        var horaFinTurno = horaInicioTurno.AddMinutes(30);

                        
                        if (horaFinTurno > scheduleDto.endTime) break;

                        
                        regla.Slots.Add(new AvailabilitySlot
                        {
                            Date = fechaIteracion,
                            StartTime = horaInicioTurno,
                            EndTime = horaFinTurno
                            
                        });

                        
                        horaInicioTurno = horaFinTurno;
                    }
                }
            }

            
            if (regla.Slots.Any())
            {
                nuevasReglas.Add(regla);
            }
        }
       await _persistence.pruebas(nuevasReglas);

    }

    private DayOfWeek TraducirDia(string diaEspanol)
    {
        return diaEspanol.Trim().ToUpper() switch
        {
            "LUNES" => DayOfWeek.Monday,
            "MARTES" => DayOfWeek.Tuesday,
            "MIÉRCOLES" or "MIERCOLES" => DayOfWeek.Wednesday,
            "JUEVES" => DayOfWeek.Thursday,
            "VIERNES" => DayOfWeek.Friday,
            "SÁBADO" or "SABADO" => DayOfWeek.Saturday,
            "DOMINGO" => DayOfWeek.Sunday,
            _ => throw new ArgumentException($"El día '{diaEspanol}' no es válido.")
        };
    }


}
