using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Validation;
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
        var doctor = await _persistence.GetById<Doctor>(request.DoctorId);
        if(doctor == null)
        {
            throw new ArgumentException($"No se encontró un doctor con el ID {request.DoctorId}");
        }

        //Obtiene solo la fecha actual, sin la hora
        var hoy = DateOnly.FromDateTime(DateTime.Now);

        //Lo mismo con el mes y el año actual
        var mesActual = hoy.Month;
        var anioActual = hoy.Year;

        //Obtiene la cantidad de días del mes actual en base al año
        var diasEnElMes = DateTime.DaysInMonth(anioActual, mesActual);

        //Crea una lista para las nuevas reglas de disponibilidad
        var nuevasReglas = new List<Availability>();

       //La request tiene una lista con cada uno de los dias de la semana, se itera sobre cada uno de ellos
        foreach (var scheduleDto in request.Days)
        {
            var diaCsharp = TraducirDia(scheduleDto.day);


            //Se crea una nueva regla de disponibilidad, con el mes, año, día de la semana, hora de inicio y hora de fin del turno del doctor
            var regla = new Availability
            {
                DoctorId = request.DoctorId,
                Month = (byte)mesActual,
                Year = (short)anioActual,
                DayOfWeek = (byte)diaCsharp,
                StartTime = scheduleDto.startTime,
                EndTime = scheduleDto.endTime,

                //Crea una lista de slots de disponibilidad vacía
                Slots = new List<AvailabilitySlot>() 
            };

            //Se itera desde el día actual hasta el último día del mes, para crear los slots de disponibilidad
            for (int dia = hoy.Day; dia <= diasEnElMes; dia++)
            {
                var fechaIteracion = new DateOnly(anioActual, mesActual, dia);

                //Verificamos si el día de la iteración coincide con el día de la semana del turno del doctor
                if (fechaIteracion.DayOfWeek == diaCsharp)
                {
                    //Seteamos como hora de inicio, la hora de inicio del turno del doctor
                    var horaInicioTurno = scheduleDto.startTime;

                    //Mientras la hora de inicio del turno del paciente sea menor a la hora de fin del turno del doctor, se crean los slots de disponibilidad
                    while (horaInicioTurno < scheduleDto.endTime)
                    {
                        var horaFinTurno = horaInicioTurno.AddMinutes(30);

                        //Si el turno termina después de la hora de fin del turno del doctor, se rompe el ciclo
                        if (horaFinTurno > scheduleDto.endTime) break;

                            //Se calcula la hora de fin del turno en base a la hora de inicio
                    //Se crea un slot de disponibilidad con la fecha de la iteración, hora de inicio y hora de fin del turno de 30 minutos
                        regla.Slots.Add(new AvailabilitySlot
                        {
                            Date = fechaIteracion,
                            StartTime = horaInicioTurno,
                            EndTime = horaFinTurno
                            
                        });

                        //Se establece el inicio del siguiente turno como la hora de fin del turno actual
                        horaInicioTurno = horaFinTurno;
                    }
                }
            }
            //En resumen, primero iteramos sobre cada dia de la semana que el doctor tiene disponible
            //Luego iteramos sobre cada dia del mes contando desde el dia actual
            //Si el dia que el doctor tiene disponible coincide con el dia de la iteracion, se crean los slots de disponibilidad


            //Si la regla tiene slots de disponibilidad, se agrega a la lista de nuevas reglas
            if (regla.Slots.Any())
            {
                nuevasReglas.Add(regla);
            }
        }
       await _persistence.AddRange(nuevasReglas);

    }

    //Convirte el dia de entrada en español a un DayOfWeek de C#
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



      
    public async Task UpdateAvailability (AvailabilityModel.Request request)
    {
        var doctor = await _persistence.GetById<Doctor>(request.DoctorId);
        if (doctor == null)
        {
            throw new ArgumentException($"No se encontró un doctor con el ID {request.DoctorId}");
        }

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
        await _persistence.UpdateRange<Availability>(nuevasReglas);


    }

}
