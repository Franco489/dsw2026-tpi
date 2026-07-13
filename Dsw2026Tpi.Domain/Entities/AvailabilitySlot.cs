using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class AvailabilitySlot: EntityBase
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public AvailabilitySlotStatus Status { get; set; }
        public Availability Availability { get; set; }
        public Guid AvailabilityId { get; set; }
        public Guid DoctorId { get; set; }
    }
}
