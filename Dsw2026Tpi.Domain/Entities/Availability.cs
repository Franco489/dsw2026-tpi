using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Dsw2026Tpi.Domain.Utils;

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

        public void GenerateSlots(Guid doctorId, TimeOnly startTime,
        TimeOnly endTime, DayOfWeek dayOfWeek, DateOnly actualDate, int numOfDays)
        {
            ICollection<AvailabilitySlot> slots = [];

            for (int d = actualDate.Day; d <= numOfDays; d++)
            {
                var iterationDate = new DateOnly(actualDate.Year, actualDate.Month, d);

                if (iterationDate.DayOfWeek == dayOfWeek && !iterationDate.IsHoliday())
                {
                    var slotStartTime = startTime;
                    while (slotStartTime < endTime)
                    {
                        var slotEndTime = slotStartTime.AddMinutes(30);

                        if (slotEndTime > endTime)
                        {
                            break;
                        }
                        slots.Add(new AvailabilitySlot
                        {
                            Date = iterationDate,
                            StartTime = slotStartTime,
                            EndTime = slotEndTime,
                            DoctorId = doctorId
                        });
                        slotStartTime = slotEndTime;
                    }
                }
            }
            Slots = slots.ToList();
        }
    }
}
