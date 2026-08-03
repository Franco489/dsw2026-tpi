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
        public AvailabilitySlotStatus Status { get; private set; } = AvailabilitySlotStatus.AVAILABLE;
        public Availability Availability { get; set; }
        public Guid AvailabilityId { get; set; }
        public Guid DoctorId { get; set; }

        public void Book()
        {
            if (Status == AvailabilitySlotStatus.BOOKED)
            {
                throw new InvalidOperationException("El turno ya se encuentra reservado.");//TODO:Ver que execpcion va
            }
            Status = AvailabilitySlotStatus.BOOKED;
        }

        public void Cancel()
        {
            if (Status == AvailabilitySlotStatus.AVAILABLE)
            {
                throw new InvalidOperationException("El turno ya se encuentra disponible.");
            }
            Status = AvailabilitySlotStatus.AVAILABLE;
        }

    }
}
