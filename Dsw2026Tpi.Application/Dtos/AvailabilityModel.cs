using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Dtos;

public record AvailabilityModel
{
    public record Request(Guid DoctorId, List<DaySchedule> Days);
    public record Response(Guid DoctorId, List<DaySchedule> Days);
    public record DaySchedule(string Day,TimeOnly StartTime, TimeOnly EndTime);
    public record DayScheduleResponse(Guid id, string Day, TimeOnly StartTime, TimeOnly EndTime);
}
