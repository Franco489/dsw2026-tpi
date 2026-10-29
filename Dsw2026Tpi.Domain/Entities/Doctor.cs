using Dsw2026Tpi.Domain.Utils;
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

        public bool HasAvailabilities() 
        {
            return AvailabilityRules.Any();
        }

        public void CreateAvailabilities(List<DaySchedule> DayScheduleList)
        {
            var actualDate = DateOnly.FromDateTime(DateTime.Now);

            var numOfDays = DateTime.DaysInMonth(actualDate.Year, actualDate.Month);

            foreach (var daySchedule in DayScheduleList)
            {
                if (daySchedule.EndTime < daySchedule.StartTime.AddMinutes(30))
                {
                    throw new ArgumentOutOfRangeException("El horario de inicio debe ser 30 minutos menor al horario de fin.");
                }
                var dayOfWeek = daySchedule.Day.toDayOfWeek();
                var availabilityRule = new Availability
                {
                    DoctorId = Id,
                    Month = (byte)actualDate.Month,
                    Year = (short)actualDate.Year,
                    DayOfWeek = (byte)dayOfWeek,
                    StartTime = daySchedule.StartTime,
                    EndTime = daySchedule.EndTime,
                    Slots = []
                };
                availabilityRule.GenerateSlots(Id, daySchedule.StartTime,
                daySchedule.EndTime, dayOfWeek, actualDate, numOfDays);
                if (availabilityRule.Slots.Any()) 
                {
                    AvailabilityRules.Add(availabilityRule);
                }
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
