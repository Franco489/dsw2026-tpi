using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Services;

public class AvailabilityService
{
    private readonly IPersistence _persistence;
    public AvailabilityService(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public async Task CreateAvailabilitiesAsync(AvailabilityModel.Request request)
    {
        // 1. Obtenemos la fecha de hoy para saber dónde estamos parados
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var mesActual = hoy.Month;
        var anioActual = hoy.Year;
        var diasEnElMes = DateTime.DaysInMonth(anioActual, mesActual);

        // Lista maestra para guardar todas las reglas que vamos a generar
        var nuevasReglas = new List<Availability>();

        // 2. Iteramos sobre los días que mandó el Administrador en el JSON
        foreach (var scheduleDto in request.Days)
        {
            var diaCsharp = TraducirDia(scheduleDto.day);

            // Creamos la "Regla Padre"
            var regla = new Availability
            {
                DoctorId = request.DoctorId,
                Month = (byte)mesActual,
                Year = (short)anioActual,
                DayOfWeek = (byte)diaCsharp,
                StartTime = scheduleDto.startTime,
                EndTime = scheduleDto.endTime,
                Slots = new List<AvailabilitySlot>() // Preparamos la lista para los turnos físicos
            };

            // 3. Buscamos los días restantes del mes que coincidan con el día pedido
            for (int dia = hoy.Day; dia <= diasEnElMes; dia++)
            {
                var fechaIteracion = new DateOnly(anioActual, mesActual, dia);

                if (fechaIteracion.DayOfWeek == diaCsharp)
                {
                    // ¡Encontramos una fecha que coincide! (ej. Un lunes futuro)
                    // 4. Fraccionamos el horario en bloques de 30 minutos
                    var horaInicioTurno = scheduleDto.startTime;

                    while (horaInicioTurno < scheduleDto.endTime)
                    {
                        var horaFinTurno = horaInicioTurno.AddMinutes(30);

                        // Por si el Administrador manda un horario impar (ej. termina 10:15)
                        if (horaFinTurno > scheduleDto.endTime) break;

                        // Creamos el bloque físico de 30 minutos
                        regla.Slots.Add(new AvailabilitySlot
                        {
                            Date = fechaIteracion,
                            StartTime = horaInicioTurno,
                            EndTime = horaFinTurno
                            // El Status ya nace en AVAILABLE por defecto gracias a tu clase de Dominio
                        });

                        // Avanzamos el reloj 30 minutos para la siguiente vuelta del while
                        horaInicioTurno = horaFinTurno;
                    }
                }
            }

            // Si la regla generó al menos un turno físico, la agregamos a la lista para guardar
            if (regla.Slots.Any())
            {
                nuevasReglas.Add(regla);
            }
        }

        // 5. Guardar en la base de datos (Acá usarías tu UnitOfWork o DbContext
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
