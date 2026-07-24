using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Availability: EntityBase
    {
        public byte Month {  get; set; }
        public short Year { get; set; }
        public byte DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public Doctor Doctor { get; set; }
        public Guid DoctorId { get; set; }
        public ICollection<AvailabilitySlot> Slots { get; set; } = [];

    }
}
