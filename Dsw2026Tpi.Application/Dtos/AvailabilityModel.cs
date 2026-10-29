using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public record AvailabilityModel
{
    public record Request(Guid DoctorId, List<DayScheduleModel> Days);
    public record Response(Guid DoctorId, List<DayScheduleModel> Days); //TODO: si no se usa, borrar
    public record DayScheduleModel(string Day, TimeOnly StartTime, TimeOnly EndTime);
    public record DayScheduleResponse(Guid Id, string Day, TimeOnly StartTime, TimeOnly EndTime);
}
