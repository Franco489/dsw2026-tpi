using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Appointment: EntityBase
    {
        public DateTime? AttendedAt { get; set; }
        public DateTime? CancelledAt { get; set;}
        public AppointmentStatus Status { get; private set; } = AppointmentStatus.BOOKED;
        public string Reason { get; set; } = string.Empty;
        public Patient Patient { get; set; }
        public Guid PatientId { get; set; }

        public void Update(AppointmentStatus status, string reason = "") 
        {
            Status = status;
            Reason = reason;
        }
    }
}
