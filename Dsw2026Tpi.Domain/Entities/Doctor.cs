using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Doctor: EntityBase
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public Speciality Speciality { get; set; }
        public Guid SpecialityId { get; set; }
        public List<Availability> AvailabilityRules { get; set; }
    }
}
