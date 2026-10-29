using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Doctor: EntityBase
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public Specialty Speciality { get; set; }
        public Guid SpecialityId { get; set; }
        public List<Availability> AvailabilityRules { get; set; } = [];

        protected Doctor()
        {
            
        }

        public Doctor(string name, string licenseNumber, Guid specialityId)
        {
            Name = name;
            LicenseNumber = licenseNumber;
            SpecialityId = specialityId;
        }

        public List<Availability> CreateAvailabilities(List<DaySchedule> DayScheduleList)
        {
            foreach (var daySchedule in DayScheduleList)
            {
                if (daySchedule.EndTime < daySchedule.StartTime.AddMinutes(30))
                {
                    throw new ArgumentOutOfRangeException("El horario de inicio debe ser 30 minutos menor al horario de fin.");
                }
                var dayOfWeek = daySchedule.Day.toDayOfWeek();
                AvailabilityRules.Add(new Availability
                {
                    DoctorId = doctor.Id,
                    Month = (byte)actualDate.Month,
                    Year = (short)actualDate.Year,
                    DayOfWeek = (byte)dayOfWeek,
                    StartTime = daySchedule.StartTime,
                    EndTime = daySchedule.EndTime,
                    Slots = []
                }); 
            }
        }

        public void Update(string name, string licenseNumber, Guid specialityId)
        {
            Name = name;
            LicenseNumber = licenseNumber;
            SpecialityId = specialityId;
        }
    }
}
